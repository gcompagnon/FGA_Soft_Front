-- WINSORIZED a 90%
-- inf     : borne inf    : 5.0
-- sup     : borne sup    : 95.0
-- ***     : date         : 14/09/2011
-- col     : colonne      : SALES_TREND
-- Wins    : colonne win  : SALES_TREND_W
-- Marche  : colonne Zscore Marcher : SALES_TREND_WM



-- 1) WINSORIZATION
SELECT TOP petit PERCENT col into #inf FROM ACT_DATA_FACTSET where date = '***' and col IS NOT NULL ORDER BY col
DECLARE @minValue as float 
SET @minValue = ( select MAX (col) from #inf )

SELECT TOP grand PERCENT col into #sup  FROM ACT_DATA_FACTSET where date = '***' and col IS NOT NULL ORDER BY col
DECLARE @maxValue as float 
SET @maxValue = ( select max (col) from #sup )

UPDATE ACT_DATA_FACTSET
SET Wins = 
    CASE 
	   WHEN col IS NULL	 THEN 0
       WHEN col < @minValue THEN @minValue
       WHEN col > @maxValue THEN @maxValue
       ELSE col 
	END
WHERE date = '***'

--donnée sur bancaire fausse pour les 2 colonnes 
UPDATE ACT_DATA_FACTSET
SET CAPEX_ON_SALES_NTM_W = 0,	
	EBIT_MARGIN_NTM_W = 0,
	EV_EBITDA_NTM_INVERSE_W = 0,
	EV_EBITDA_NTM_ON_AVG_10_INVERSE_W = 0	
where date = '***' and ICB_SECTOR LIKE '8%'

--si crise ne mettre pas plus que 100
UPDATE ACT_DATA_FACTSET
SET EPS_NTM_GROWTH_W = 100
WHERE date = '***' and EPS_NTM_GROWTH_W >= 100

DROP TABLE #inf
DROP TABLE #sup



-- 3) ZCORE MARCHE = ABS
SELECT 
	SUM(STOXX_600) As 'poids',
	SUM(STOXX_600*Wins)/SUM(STOXX_600) As 'moy',
	SQRT( SUM(STOXX_600*SQUARE(Wins))/SUM(STOXX_600) - SQUARE(SUM(STOXX_600*Wins)/SUM(STOXX_600)) ) As 'vol' 
INTO #mo
FROM ACT_DATA_FACTSET
WHERE date='***' and col IS NOT NULL

UPDATE ACT_DATA_FACTSET
SET Marche =
    CASE 
       WHEN col IS NULL THEN NULL
	   WHEN m.vol = 0 THEN NULL
       ELSE (Wins - m.moy)/m.vol 
	END
FROM ACT_DATA_FACTSET a, #mo m
WHERE date = '***' 
--voir procedure stockée pour EBIT, CAPEX, EBITDA

DROP TABLE #mo



