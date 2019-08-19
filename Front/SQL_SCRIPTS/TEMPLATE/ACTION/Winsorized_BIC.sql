-- WINSORIZED a 90%
-- inf     : borne inf    : 5.0
-- sup     : borne sup    : 95.0
-- ***     : date         : 14/09/2011
-- col     : colonne      : SALES_TREND
-- Wins    : colonne win  : SALES_TREND_W
-- Secteur : colonne Zscore Secteur : SALES_TREND_WS




-- 1) WINSORIZATION
SELECT TOP petit PERCENT col into #inf FROM ACT_DATA_FACTSET where date = '***' and col IS NOT NULL and SXXP IS NOT NULL ORDER BY col
DECLARE @minValue as float 
SET @minValue = ( select MAX (col) from #inf )

SELECT TOP grand PERCENT col into #sup  FROM ACT_DATA_FACTSET where date = '***' and col IS NOT NULL and SXXP IS NOT NULL  ORDER BY col
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
WHERE date = '***' and SXXP IS NOT NULL 

--donnée sur bancaire fausse pour les 2 colonnes 
UPDATE ACT_DATA_FACTSET
SET EBIT_MARGIN_NTM_W = 0	
where date = '***' and ICB_SUPERSECTOR LIKE '8%' and SXXP IS NOT NULL 

--si crise ne mettre pas plus que 100
UPDATE ACT_DATA_FACTSET
SET EPS_GROWTH_NTM_W = 100
WHERE date = '***' and EPS_GROWTH_NTM_W >= 100 and SXXP IS NOT NULL 

DROP TABLE #inf
DROP TABLE #sup



-- 2) ZCORE SECTEUR = BIC
--poids pondéré => calcul de moyenne et volatilité
SELECT 
	s.libelle,
	FGA_SECTOR,
	SUM(SXXP) As 'poids',
	SUM(SXXP*Wins)/SUM(SXXP) As 'moy',
	CASE WHEN SUM(SXXP*SQUARE(Wins))/SUM(SXXP) - SQUARE(SUM(SXXP*Wins)/SUM(SXXP)) < 0 THEN NULL ELSE SQRT( SUM(SXXP*SQUARE(Wins))/SUM(SXXP) - SQUARE(SUM(SXXP*Wins)/SUM(SXXP)) )  END As 'vol' 
INTO #normal
FROM ACT_DATA_FACTSET, ACT_FGA_SECTOR s
WHERE 
	col IS NOT NULL and
	date='***' and 
	s.id = FGA_SECTOR
	and SXXP IS NOT NULL 
GROUP BY FGA_SECTOR, s.libelle

UPDATE ACT_DATA_FACTSET
SET Secteur =
    CASE 
       WHEN col IS NULL THEN NULL
	   WHEN n.vol = 0 THEN NULL
       ELSE (Wins - n.moy)/n.vol 
	END
FROM ACT_DATA_FACTSET a, #normal n
WHERE date = '***' and n.FGA_SECTOR = a.FGA_SECTOR and SXXP IS NOT NULL 

DROP TABLE #normal