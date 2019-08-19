-- ANALYSE DE L IBOXX PAR PAYS puis par Level 2 de secteur (Financials Non-Financials)

ALTER PROCEDURE IBoxxPays
       @dateJ DATETIME,
       @dateJ1 DATETIME,
	   @dateJ2   DATETIME,
	   @durAdjusted BIT
AS 
/*
DECLARE @dateJ As DATETIME
SET @dateJ = '30/11/2012'
DECLARE @dateJ1 As DATETIME
SET @dateJ1 = '31/10/2012'
DECLARE @dateJ2 As DATETIME
SET @dateJ2 = '30/12/2011'
DECLARE @durAdjusted As BIT
SET @durAdjusted = 0
*/


CREATE TABLE #tmp(	
	ordre char(40),
	--niveauLevel4 VARCHAR(160), 
	--niveauDette VARCHAR(5),
	poidsJ FLOAT, 
	Level VARCHAR(160) COLLATE DATABASE_DEFAULT, 
	niveauLevel2 VARCHAR(60) COLLATE DATABASE_DEFAULT,
	ASMJ FLOAT,
	ASMJ1 FLOAT,
	ASMJ2 FLOAT,
	DELTAJ1 FLOAT,
	DELTAJ2 FLOAT	
)


--------------------------------------------- J-------------------------------------------------------------
-- total iboxx
IF @durAdjusted = 1
BEGIN
	INSERT INTO #tmp(ordre,poidsJ, Level,niveauLevel2,ASMJ)
	SELECT 
		'AAAAAAAA' as 'ordre',
		SUM(indexWeight) As 'poidsJ',
		--'a' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		'' As niveauLevel2,
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ

	-- total Pays
	INSERT INTO #tmp(ordre,poidsJ,Level,niveauLevel2,ASMJ)
	SELECT 
		country + 'AAAAAAAAA',
		SUM(indexWeight) As 'poidsJ',
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'TOTAL' As niveauLevel2,
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM TX_IBOXX 
	where date=@dateJ group by country


	-- total pays / ventilé par le level2
	INSERT INTO #tmp(ordre,poidsJ,Level,niveauLevel2,ASMJ)
	SELECT 
		country + level2,
		SUM(indexWeight) As 'poidsJ',
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'                   ' + level2 As niveauLevel2,	
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM TX_IBOXX 
	where date=@dateJ group by country, Level2
END
ELSE
-- en ponderé sans adjustement
BEGIN
	INSERT INTO #tmp(ordre,poidsJ, Level,niveauLevel2,ASMJ)
	SELECT 
		'AAAAAAAA' as 'ordre',
		SUM(indexWeight) As 'poidsJ',
		--'a' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		'' As niveauLevel2,
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ

	-- total Pays
	INSERT INTO #tmp(ordre,poidsJ,Level,niveauLevel2,ASMJ)
	SELECT 
		country + 'AAAAAAAAA',
		SUM(indexWeight) As 'poidsJ',
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'TOTAL' As niveauLevel2,
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM TX_IBOXX 
	where date=@dateJ group by country


	-- total pays / ventilé par le level2
	INSERT INTO #tmp(ordre,poidsJ,Level,niveauLevel2,ASMJ)
	SELECT 
		country + level2,
		SUM(indexWeight) As 'poidsJ',
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'                   ' + level2 As niveauLevel2,	
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM TX_IBOXX 
	where date=@dateJ group by country, Level2
END

--------------------------------------------- J-1-------------------------------------------------------------
CREATE TABLE #tmp_dateJ1(
	ordre char(40),
	Level VARCHAR(160) COLLATE DATABASE_DEFAULT,
	niveauLevel2 VARCHAR(60) COLLATE DATABASE_DEFAULT,
	ASMJ1 FLOAT
)
-- total iboxx
IF @durAdjusted = 1
BEGIN
	INSERT INTO #tmp_dateJ1 (ordre,Level,niveauLevel2,ASMJ1) 
	SELECT 
		'AAAAAAAA' as 'ordre',
		--'a' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		'' As niveauLevel2,
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1
	UNION
	-- total Level2
	SELECT 
		country + 'AAAAAAAAA',
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'TOTAL' As niveauLevel2,
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1 group by country
	UNION
	-- total pays / ventilé par le level2
	SELECT 
		country + level2,
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'                   ' + level2 As niveauLevel2,
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM TX_IBOXX 
	where date=@dateJ1 group by country, Level2
END
ELSE
-- en ponderé sans adjustement
BEGIN
	INSERT INTO #tmp_dateJ1 (ordre,Level,niveauLevel2,ASMJ1) 
	SELECT 
		'AAAAAAAA' as 'ordre',
		--'a' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		'' As niveauLevel2,
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1
	UNION
	-- total Level2
	SELECT 
		country + 'AAAAAAAAA',
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'TOTAL' As niveauLevel2,
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1 group by country
	UNION
	-- total pays / ventilé par le level2
	SELECT 
		country + level2,
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'                   ' + level2 As niveauLevel2,
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM TX_IBOXX 
	where date=@dateJ1 group by country, Level2
END

UPDATE #tmp
SET ASMJ1 = td.ASMJ1
FROM #tmp t, #tmp_dateJ1 td
WHERE t.level = td.level  and t.niveauLevel2=td.niveauLevel2
-- and t.niveauDette = td.niveauDette


--------------------------------------------- J-2-------------------------------------------------------------
CREATE TABLE #tmp_dateJ2(
	ordre char(40),
	Level VARCHAR(160) COLLATE DATABASE_DEFAULT,
	niveauLevel2 VARCHAR(60) COLLATE DATABASE_DEFAULT,
	ASMJ2 FLOAT
)
-- total iboxx
IF @durAdjusted = 1
BEGIN
	INSERT INTO #tmp_dateJ2 (ordre,Level,niveauLevel2,ASMJ2) 
		SELECT 
		'AAAAAAAA' as 'ordre',
		--'a' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		'' As niveauLevel2,
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2
	UNION
	-- total Level2
	SELECT 
		country + 'AAAAAAAAA',
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'TOTAL' As niveauLevel2,
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2 group by country
	UNION
	-- total pays / ventilé par le level2
	SELECT 
		country + level2,
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'                   ' + level2 As niveauLevel2,
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ2'
	FROM TX_IBOXX 
	where date=@dateJ2 group by country, Level2
END
ELSE
-- en ponderé sans adjustement
BEGIN
	INSERT INTO #tmp_dateJ2 (ordre,Level,niveauLevel2,ASMJ2) 
		SELECT 
		'AAAAAAAA' as 'ordre',
		--'a' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		'' As niveauLevel2,
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2
	UNION
	-- total Level2
	SELECT 
		country + 'AAAAAAAAA',
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'TOTAL' As niveauLevel2,
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2 group by country
	UNION
	-- total pays / ventilé par le level2
	SELECT 
		country + level2,
		--level2 As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette, 
		'      ' + country As 'Level', 
		'                   ' + level2 As niveauLevel2,
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ2'
	FROM TX_IBOXX 
	where date=@dateJ2 group by country, Level2
END


UPDATE #tmp
SET ASMJ2 = td.ASMJ2
FROM #tmp t, #tmp_dateJ2 td
WHERE t.level = td.level and t.niveauLevel2=td.niveauLevel2
 -- and t.niveauLevel4=td.niveauLevel4 and t.niveauDette = td.niveauDette

UPDATE #tmp
SET DELTAJ2 = ASMJ - ASMJ2,
	DELTAJ1 = ASMJ - ASMJ1




SELECT --ordre,
	ROUND(poidsJ,2) As 'Poids J',
	level AS 'Level',
	niveauLevel2,
	ROUND(ASMJ2,2) As 'ASM J-2',  
	ROUND(ASMJ1,2) As 'ASM J-1', 
	ROUND(ASMJ,2) As 'ASM J', 
	ROUND(DELTAJ2,2) As 'DELTA ASM J - J-2', 
	ROUND(DELTAJ1,2) As 'DELTA ASM J - J-1'
FROM #tmp i
order by ordre



DROP TABLE #tmp
DROP TABLE #tmp_dateJ1
DROP TABLE #tmp_dateJ2


--SELECT * FROM TX_RATING WHERE date='30/08/2011'
--EXECUTE IBoxxASM '20/10/2011', '18/10/2011', '13/10/2011'