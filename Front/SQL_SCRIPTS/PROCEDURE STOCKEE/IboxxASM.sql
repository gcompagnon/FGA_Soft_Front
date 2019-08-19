

ALTER PROCEDURE IBoxxASM
       @dateJ DATETIME,
       @dateJ1 DATETIME,
	   @dateJ2   DATETIME,
	   @durAdjusted BIT
AS 


DECLARE @dateJ As DATETIME
SET @dateJ = '24/05/2016'
DECLARE @dateJ1 As DATETIME
SET @dateJ1 = '24/05/2016'
DECLARE @dateJ2 As DATETIME
SET @dateJ2 = '24/05/2016'
DECLARE @durAdjusted As BIT
SET @durAdjusted = 'true'


CREATE TABLE #tmp(
	niveauLevel2 VARCHAR(60) COLLATE DATABASE_DEFAULT, 
	niveauLevel4 VARCHAR(160) COLLATE DATABASE_DEFAULT, 
	niveauDette VARCHAR(5) COLLATE DATABASE_DEFAULT,
	poidsJ FLOAT, 
	Level VARCHAR(160) COLLATE DATABASE_DEFAULT, 
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
	INSERT INTO #tmp(poidsJ,niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		'a' As niveauLevel2, 
		'' As niveauLevel4, 
		'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ
	-- total Level2
	INSERT INTO #tmp(poidsJ,niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		level2 As niveauLevel2, 
		'' As niveauLevel4,
		'' As niveauDette, 
		'      ' + level2 As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ group by level2
	-- total Level4
	INSERT INTO #tmp(poidsJ,niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		'' As niveauDette, 
		'             ' + level4 As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ group by level2,level4 
	--total dette
	INSERT INTO #tmp(poidsJ,niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		tier As niveauDette, 
		CASE WHEN tier = '*' THEN '                  ' + 'SEN' ELSE '                  ' + tier END As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ and level2 In ('Financials') group by level2,level4, tier
		
END
ELSE
-- en ponderé sans adjustement
BEGIN
	INSERT INTO #tmp(poidsJ,niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		'a' As niveauLevel2, 
		'' As niveauLevel4, 
		'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ
	-- total Level2
	INSERT INTO #tmp(poidsJ,niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		level2 As niveauLevel2, 
		'' As niveauLevel4,
		'' As niveauDette, 
		'      ' + level2 As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ group by level2
	-- total Level4
	INSERT INTO #tmp(poidsJ,niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		'' As niveauDette, 
		'             ' + level4 As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ group by level2,level4 
	--total dette
	INSERT INTO #tmp(poidsJ,niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		tier As niveauDette, 
		CASE WHEN tier = '*' THEN '                  ' + 'SEN' ELSE '                  ' + tier END As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM TX_IBOXX where date=@dateJ and level2 In ('Financials') group by level2,level4, tier
END



--------------------------------------------- J-1-------------------------------------------------------------
CREATE TABLE #tmp_dateJ1(
	niveauLevel2 VARCHAR(60) COLLATE DATABASE_DEFAULT,
	niveauLevel4 VARCHAR(160) COLLATE DATABASE_DEFAULT, 
	niveauDette VARCHAR(5) COLLATE DATABASE_DEFAULT,	
	Level VARCHAR(160) COLLATE DATABASE_DEFAULT,
	ASMJ1 FLOAT
)

-- total iboxx
IF( @durAdjusted = 1)
BEGIN	
	INSERT INTO #tmp_dateJ1 (niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ1) 
	SELECT 
		'a' As niveauLevel2, 
		'' As niveauLevel4, 
		'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'	
	FROM TX_IBOXX where date=@dateJ1
	UNION
	-- total Level2
	SELECT 
		level2 As niveauLevel2, 
		'' As niveauLevel4,
		'' As niveauDette, 
		'   ' + level2 As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1 group by level2
	UNION
	-- total Level4
	SELECT 
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		'' As niveauDette, 
		'         ' + level4 As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1 group by level2,level4 
	UNION
	--total dette
	SELECT 
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		tier As niveauDette, 
		CASE WHEN tier = '*' THEN '                  ' + 'SEN' ELSE '                  ' + tier END As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1 and level2 In ('Financials') group by level2,level4, tier

	UPDATE #tmp
	SET ASMJ1 = td.ASMJ1
	FROM #tmp t, #tmp_dateJ1 td
	WHERE t.niveauLevel2 = td.niveauLevel2 and t.niveauLevel4=td.niveauLevel4 and t.niveauDette = td.niveauDette
END	
ELSE
-- en ponderé sans adjustement
BEGIN
	INSERT INTO #tmp_dateJ1 (niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ1) 
	SELECT 
		'a' As niveauLevel2, 
		'' As niveauLevel4, 
		'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1
	UNION
	-- total Level2
	SELECT 
		level2 As niveauLevel2, 
		'' As niveauLevel4,
		'' As niveauDette, 
		'   ' + level2 As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1 group by level2
	UNION
	-- total Level4
	SELECT 
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		'' As niveauDette, 
		'         ' + level4 As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1 group by level2,level4 
	UNION
	--total dette
	SELECT 
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		tier As niveauDette, 
		CASE WHEN tier = '*' THEN '                  ' + 'SEN' ELSE '                  ' + tier END As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM TX_IBOXX where date=@dateJ1 and level2 In ('Financials') group by level2,level4, tier

	UPDATE #tmp
	SET ASMJ1 = td.ASMJ1
	FROM #tmp t, #tmp_dateJ1 td
	WHERE t.niveauLevel2 = td.niveauLevel2 and t.niveauLevel4=td.niveauLevel4 and t.niveauDette = td.niveauDette
END 


--------------------------------------------- J-2-------------------------------------------------------------
CREATE TABLE #tmp_dateJ2(
	niveauLevel2 VARCHAR(60) COLLATE DATABASE_DEFAULT,  
	niveauLevel4 VARCHAR(160) COLLATE DATABASE_DEFAULT,
	niveauDette VARCHAR(5) COLLATE DATABASE_DEFAULT,
	Level VARCHAR(160) COLLATE DATABASE_DEFAULT,
	ASMJ2 FLOAT
)

-- total iboxx
IF( @durAdjusted = 1)
BEGIN	
	INSERT INTO #tmp_dateJ2 (niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ2) 
	SELECT 
		'a' As niveauLevel2, 
		'' As niveauLevel4, 
		'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ2'	
	FROM TX_IBOXX where date=@dateJ2
	UNION
	-- total Level2
	SELECT 
		level2 As niveauLevel2, 
		'' As niveauLevel4,
		'' As niveauDette, 
		'   ' + level2 As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2 group by level2
	UNION
	-- total Level4
	SELECT 
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		'' As niveauDette, 
		'         ' + level4 As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2 group by level2,level4 
	UNION
	--total dette
	SELECT 
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		tier As niveauDette, 
		CASE WHEN tier = '*' THEN '                  ' + 'SEN' ELSE '                  ' + tier END As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2 and level2 In ('Financials') group by level2,level4, tier
END	
ELSE
-- en ponderé sans adjustement
BEGIN
	INSERT INTO #tmp_dateJ2 (niveauLevel2,niveauLevel4,niveauDette,Level,ASMJ2) 
	SELECT 
		'a' As niveauLevel2, 
		'' As niveauLevel4, 
		'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2
	UNION
	-- total Level2
	SELECT 
		level2 As niveauLevel2, 
		'' As niveauLevel4,
		'' As niveauDette, 
		'   ' + level2 As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2 group by level2
	UNION
	-- total Level4
	SELECT 
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		'' As niveauDette, 
		'         ' + level4 As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2 group by level2,level4 
	UNION
	--total dette
	SELECT 
		level2 As niveauLevel2, 
		Level4 As niveauLevel4,
		tier As niveauDette, 
		CASE WHEN tier = '*' THEN '                  ' + 'SEN' ELSE '                  ' + tier END As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ2'
	FROM TX_IBOXX where date=@dateJ2 and level2 In ('Financials') group by level2,level4, tier
END


UPDATE #tmp
SET ASMJ2 = td.ASMJ2
FROM #tmp t, #tmp_dateJ2 td
WHERE t.niveauLevel2 = td.niveauLevel2 and t.niveauLevel4=td.niveauLevel4 and t.niveauDette = td.niveauDette



UPDATE #tmp
SET DELTAJ2 = ASMJ - ASMJ2,
	DELTAJ1 = ASMJ - ASMJ1

SELECT 
	ROUND(poidsJ,2) As 'Poids J',
	level AS 'Level',
	ROUND(ASMJ2,2) As 'ASM J-2',  
	ROUND(ASMJ1,2) As 'ASM J-1', 
	ROUND(ASMJ,2) As 'ASM J', 
	ROUND(DELTAJ2,2) As 'DELTA ASM J - J-2', 
	ROUND(DELTAJ1,2) As 'DELTA ASM J - J-1'
FROM #tmp order by niveauLevel2, niveauLevel4, niveauDette




DROP TABLE #tmp
DROP TABLE #tmp_dateJ1
DROP TABLE #tmp_dateJ2

SELECT * FROM TX_IBOXX WHERE date='24/05/2016'

EXECUTE IBoxxASM '18/05/2016', '06/05/2016', '04/05/2016','true'