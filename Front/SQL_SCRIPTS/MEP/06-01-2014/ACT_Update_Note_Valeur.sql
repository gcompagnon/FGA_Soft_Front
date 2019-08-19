
/****** Object:  StoredProcedure [dbo].[ACT_Update_Note_Valeur]    Script Date: 05/29/2013 11:47:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Aurélien Brandicourt
-- Create date: 27/12/2012
-- Description:	Calcule la note qualitative relative d'un secteur à une date donnée par la méthode du centré-réduit.
-- =============================================
ALTER PROCEDURE [dbo].[ACT_Update_Note_Valeur] 
	-- Add the parameters for the stored procedure here
	@Date As DATETIME,
	@id_fga int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
	--DECLARE @Date AS Datetime 
	--SET @Date='17/12/2012'
	--DECLARE @id_fga AS int
	--SET @id_fga = 20

	
	/* ************************************ */
	--1) calcul la moyenne et l'écart type
	-- E_ = espérance || O_ = Ecart type
	-- Calcul de la moyenne

	SELECT v.ISIN
		, f.SXXP
 		, COALESCE(v.TOTAL_NOTE, 5) AS TOTAL_NOTE
 		, ROUND(COALESCE(v.TOTAL_NOTE, 5) - ( -- calcul sur le secteur
 							SELECT SUM(COALESCE(TOTAL_NOTE, 5)) / CAST(COUNT(*) AS FLOAT)
 							FROM ACT_VALEUR v2
 							INNER JOIN ACT_DATA_FACTSET f2 ON f2.ISIN = v2.ISIN
 							WHERE f2.fga_sector=f.fga_sector
 								AND f2.date = f.date
 								AND NOT f.SXXP IS NULL
 						)
 			, 12) AS Mid_TOTAL_NOTE
 		, CAST(NULL AS Float) AS GARPN_NOTE_S
	INTO #ScoreEspVsVari
	FROM ACT_VALEUR v
	INNER JOIN ACT_DATA_FACTSET f ON f.ISIN = v.ISIN
	WHERE id_fga = @id_fga
		AND f.date = @date

	--SELECT * FROM #ScoreEspVsVari
	
	-- Calcul l'espérance et l'écart type
	SELECT distinct
		(	SELECT SUM(TOTAL_NOTE)/CAST(COUNT(*) AS FLOAT)
				FROM #ScoreEspVsVari s2
				WHERE NOT SXXP IS NULL
		) AS E_TOTAL_NOTE
		, (	SELECT SQRT(1/CAST(COUNT(*) AS FLOAT) * SUM(SQUARE(MID_TOTAL_NOTE)))
				FROM #ScoreEspVsVari s2
				WHERE NOT SXXP IS NULL
		) AS O_TOTAL_NOTE
	Into #EspvsVari
	FROM #ScoreEspVsVari s
		
	--SELECT * FROM #EspvsVari



	/* ************************************ */
	-- 2) Normalisation des indicateurs (score centré réduit)
	-- SELECT * FROM #ScoreEspvsVari
	UPDATE #ScoreEspVsVari 
	SET		TOTAL_NOTE = (	SELECT
								CASE
									WHEN ev.O_TOTAL_NOTE = 0 THEN 0
									ELSE (s.TOTAL_NOTE - ev.E_TOTAL_NOTE)/ev.O_TOTAL_NOTE
								END
							FROM #EspvsVari ev
			)
	FROM #ScoreEspVsVari s


	/* ************************************ */
	-- 3) Calcul des scores
	UPDATE #ScoreEspvsVari
	SET GARPN_NOTE_S = TOTAL_NOTE
	FROM #ScoreEspvsVari val
	--SELECT * FROM #ScoreEspVsVari



	/* ************************************ */
	-- 4) Transformation en note relative
	DECLARE @coef_note AS float
	SELECT
		@coef_note = COALESCE(5 /NULLIF(CASE
											WHEN ABS(MAX(GARPN_NOTE_S)) > ABS(MIN(GARPN_NOTE_S))
											THEN ABS(MAX(GARPN_NOTE_S))
											ELSE ABS(MIN(GARPN_NOTE_S))
										 END, 0)
							, 0)
	FROM #ScoreEspVsVari
	WHERE NOT SXXP IS NULL
	--SELECT @coef_note
						
	UPDATE 
		ACT_DATA_FACTSET
	SET 
		GARPN_NOTE_S = 5 + @coef_NOTE * s.GARPN_NOTE_S
	FROM #ScoreEspVsVari s, ACT_DATA_FACTSET a
	WHERE 
		date=@date
		AND a.ISIN = s.ISIN



	/* ************************************ */
	-- 5) Update ranks.
	--DECLARE @Date AS Datetime 
	--SET @Date='12/12/2012'
		
	--EXECUTE ACT_Add_Rank_Value @date


	/* ************************************ */
	DROP TABLE #ScoreEspVsVari
	DROP TABLE #EspvsVari

	--SELECT * FROM #ScoreEspVsVari
	
	--SELECT * FROM #EspvsVari
END

GO


