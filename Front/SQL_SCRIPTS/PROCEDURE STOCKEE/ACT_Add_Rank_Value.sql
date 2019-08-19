
/****** Object:  StoredProcedure [dbo].[ACT_Add_Rank_Value]    Script Date: 05/29/2013 11:25:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Aurélien Brandicourt
-- Create date: 13/12/2012
-- Description:	Met à jour le rang de la valeur au sein de son secteur en fonction de ses scores Quant et Qual
-- =============================================
ALTER PROCEDURE [dbo].[ACT_Add_Rank_Value] 
	-- Add the parameters for the stored procedure here
	@Date As DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	--DECLARE @Date AS Datetime 
	--SET @Date='17/12/2012'
	
	
	/* ************************************ */
	--1) Remove existing records
	DELETE FROM ACT_VALEUR_RANK
	WHERE date=@date

	/* ************************************ */
	--2) Compute new ranks
	INSERT INTO ACT_VALEUR_RANK (id_value, date, rank_quant, rank_qual, rank_total)
	SELECT val.ID
		, fac.date
		--, (GARPN_TOTAL_S + TOTAL_NOTE) / 2 AS score_total
		, ( SELECT quant
			FROM (
					SELECT ISIN, ROW_NUMBER() OVER(ORDER BY GARPN_TOTAL_S DESC) AS Quant
					FROM ACT_DATA_FACTSET fac2
					WHERE date=fac.date
						AND fac2.fga_sector = fac.fga_sector
				 ) t
			WHERE t.ISIN = fac.ISIN
		) AS Quant
		, ( SELECT qual
			FROM (
					SELECT ISIN, ROW_NUMBER() OVER(ORDER BY GARPN_NOTE_S DESC) AS Qual
					FROM ACT_DATA_FACTSET fac2
					WHERE date=fac.date
						AND fac2.fga_sector = fac.fga_sector
				 ) t
			WHERE t.ISIN = fac.ISIN
		) AS Qual
		, ( SELECT total
			FROM (
					SELECT id, ROW_NUMBER() OVER(ORDER BY total DESC) AS total--, score, note
					FROM (
							SELECT val2.id AS id
									--, COALESCE(GARPN_NOTE_S, 0) AS note
									--, COALESCE(GARPN_TOTAL_S, 0) AS score
									, (COALESCE(GARPN_NOTE_S, 0) + COALESCE(GARPN_TOTAL_S, 0)) AS total
							FROM ACT_DATA_FACTSET fac2
							INNER JOIN ACT_VALEUR val2 ON val2.ISIN = fac2.ISIN
							WHERE date=fac.date --'12/12/2012'
								AND val2.id_fga = val.id_fga--20
					) t2
			) t
			WHERE t.id = val.id
		) AS Total
	FROM ACT_DATA_FACTSET fac
	LEFT OUTER JOIN ACT_VALEUR val ON val.ISIN = fac.ISIN
	WHERE date=@Date

END

GO
