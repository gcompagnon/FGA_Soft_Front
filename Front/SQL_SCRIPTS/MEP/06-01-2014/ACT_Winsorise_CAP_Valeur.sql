
/****** Object:  StoredProcedure [dbo].[ACT_Winsorise_CAP_Valeur]    Script Date: 11/13/2013 17:17:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Aurélien Brandicourt
-- Create date: 30/11/2012
-- Description:	Winsorize les valeurs dans ACT_DATA_FACTSET à une date donnée
-- =============================================
ALTER PROCEDURE [dbo].[ACT_Winsorise_CAP_Valeur] 
	-- Add the parameters for the stored procedure here
	@tab VARCHAR(50),
	@col VARCHAR(50),
	@winsmin float = 0,
	@winsmax float = 100,
	@capMin float = 0,
	@capMax float = 100
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

--DECLARE @winsmin AS FLOAT, @winsmax AS float, @capMin AS FLOAT, @capMax AS float, @tab VARCHAR (50), @col VARCHAR(200)
--SET @winsmin = 5
--SET @winsmax = 95
--SET @capMin = -100
--SET @capMax = 100
--SET @col = 'ROTE_NTM'
--SET @tab = '#values'
--DECLARE @date AS datetime
--SET @date = '08/01/2013'

--DROP TABLE #values
--SELECT ISIN, fga_sector AS id_fga, SXXP, ROTE_NTM
--INTO #values
--FROM ACT_DATA_FACTSET
--WHERE date = @date


SET @col = quotename(@col)
SET @tab = quotename(@tab)
SET @winsmin = @winsmin / 100
SET @winsmax = @winsmax / 100

-- 1) Extrapolation linéaire de la valeur sur les bornes @winsmin et @winsmax */
DECLARE @sql AS NVARCHAR(MAX)
SELECT @sql = N' 
;WITH datas(ISIN, id_fga, ' + @col + N') AS
(
	SELECT ISIN, CAST(id_fga AS integer) AS id_fga, ' + @col + N'
	FROM ' + @tab + N'
	WHERE ' + @col + N' IS NOT NULL
)
SELECT
	d.ISIN
	, d.id_fga
	, ' + @col + N'
	, (	SELECT prev 
		FROM (
				SELECT	ISIN,
						(ROW_NUMBER() OVER (ORDER BY ' + @col + N') - 2.0) AS prev
				FROM datas 
				WHERE id_fga = d.id_fga
			) t_prev
		WHERE t_prev.ISIN = d.ISIN) / NULLIF(s.cnt - 1, 0) AS prev_rank
	, (	SELECT curr
		FROM (
				SELECT ISIN,
						(ROW_NUMBER() OVER (ORDER BY ' + @col + N') - 1.0) AS curr
				FROM datas 
				WHERE id_fga = d.id_fga
			) t_curr
		WHERE t_curr.ISIN = d.ISIN) / NULLIF(s.cnt - 1, 0) AS curr_rank
	, (	SELECT next 
		FROM (
				SELECT ISIN,
						(ROW_NUMBER() OVER (ORDER BY ' + @col + N') - 0.0) AS next
				FROM datas 
				WHERE id_fga = d.id_fga
			) t_next
		WHERE t_next.ISIN = d.ISIN) / NULLIF(s.cnt - 1, 0) AS next_rank
INTO #winsor_rank
FROM datas d
INNER JOIN (
				SELECT 
					id_fga
					, COUNT(' + @col + N') AS cnt
				FROM datas
				group by id_fga
	) s ON s.id_fga = d.id_fga
	
SELECT
	min.id_fga
	, min.wins_min_value
	, max.wins_max_value
INTO ##winsor_value
FROM (
	SELECT
		t1.id_fga
		, CASE
			WHEN t1.' + @col + N' = t2.' + @col + N' THEN t1.' + @col + N'
			ELSE t1.' + @col + N' + (t2.' + @col + N' - t1.' + @col + N') * ((' + CAST(@winsmin as varchar) + N' - t1.curr_rank) / (t2.curr_rank - t1.curr_rank))
		END AS wins_min_value
	FROM #winsor_rank t1
	INNER JOIN #winsor_rank t2 ON t1.id_fga = t2.id_fga
	WHERE (t1.curr_rank = ' + CAST(@winsmin as varchar) + N' OR (t1.curr_rank < ' + CAST(@winsmin as varchar) + N' AND t1.next_rank > ' + CAST(@winsmin as varchar) + N'))
	  AND (t2.curr_rank = ' + CAST(@winsmin as varchar) + N' OR (t2.curr_rank > ' + CAST(@winsmin as varchar) + N' AND t2.prev_rank < ' + CAST(@winsmin as varchar) + N'))
	) min
INNER JOIN (
	SELECT
		t1.id_fga
		, CASE
			WHEN t1.' + @col + N' = t2.' + @col + N' THEN t1.' + @col + N'
			ELSE t1.' + @col + N' + (t2.' + @col + N' - t1.' + @col + N') * ((' + CAST(@winsmax as varchar) + N' - t1.curr_rank) / (t2.curr_rank - t1.curr_rank))
		END AS wins_max_value
	FROM #winsor_rank t1
	INNER JOIN #winsor_rank t2 ON t1.id_fga = t2.id_fga
	WHERE (t1.curr_rank = ' + CAST(@winsmax as varchar) + N' OR (t1.curr_rank < ' + CAST(@winsmax as varchar) + N' AND t1.next_rank > ' + CAST(@winsmax as varchar) + N'))
	  AND (t2.curr_rank = ' + CAST(@winsmax as varchar) + N' OR (t2.curr_rank > ' + CAST(@winsmax as varchar) + N' AND t2.prev_rank < ' + CAST(@winsmax as varchar) + N'))
	) max ON min.id_fga = max.id_fga
ORDER BY min.id_fga

DROP TABLE #winsor_rank'
--SELECT @sql
EXEC(@sql)
SELECT *
INTO #winsor_value
FROM ##winsor_value
DROP TABLE ##winsor_value




-- 2) CAP winsor_values to MIN/MAX
--DECLARE @capMin AS FLOAT, @capMax AS float
--SET @capMin = 0
--SET @capMax = 100
UPDATE #winsor_value
SET wins_min_value = @capMin
WHERE wins_min_value < @capMin

UPDATE #winsor_value
SET wins_max_value = @capMax
WHERE wins_max_value > @capMax

--SELECT * FROM #winsor_value


-- 3) Winsorize to MIN/MAX values
SET @sql = N'
UPDATE ' + @tab + N'
SET ' + @col + N' =
	CASE
		WHEN ' + @col + N' IS NULL THEN wins_min_value
		WHEN ' + @col + N' < wins_min_value THEN wins_min_value
		WHEN ' + @col + N' > wins_max_value THEN wins_max_value
		ELSE ' + @col + N'
	END
FROM ' + @tab + N' val
INNER JOIN #winsor_value wins ON wins.id_fga = val.id_fga'
--SELECT @sql
EXEC(@sql)

--SELECT * FROM #values ORDER BY id_fga, ROTE_NTM


DROP TABLE #winsor_value

END
