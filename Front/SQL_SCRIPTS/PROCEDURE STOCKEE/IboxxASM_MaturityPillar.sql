/*
Calcul du ta
*/
ALTER PROCEDURE IBoxxASM_MatPillar
       @dateJ DATETIME,
       @dateJ1 DATETIME,
	   @dateJ2   DATETIME,
	   @durAdjusted BIT
AS 

/*
DECLARE @dateJ As DATETIME
SET @dateJ = '16/05/2013'
DECLARE @dateJ1 As DATETIME
SET @dateJ1 = '14/05/2013'
DECLARE @dateJ2 As DATETIME
SET @dateJ2 = '18/04/2013'
DECLARE @durAdjusted As BIT
SET @durAdjusted = 'false'
*/

CREATE TABLE #tmp(
	maturityPillar VARCHAR(60)COLLATE DATABASE_DEFAULT,
	Ordre_Tranche TINYINT,
	niveauLevel2 VARCHAR(60)COLLATE DATABASE_DEFAULT,
	--niveauLevel4 VARCHAR(160), 
	--niveauDette VARCHAR(5),
	poidsJ FLOAT, 
	Level VARCHAR(160)COLLATE DATABASE_DEFAULT,
	ASMJ FLOAT,
	ASMJ1 FLOAT,
	ASMJ2 FLOAT,
	DELTAJ1 FLOAT,
	DELTAJ2 FLOAT	
)

--------------------------------------------- J-------------------------------------------------------------
select 
Date,MaturityDate,
CASE WHEN MaturityDate IS NULL THEN NULL ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)<0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 1 THEN '0-1 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END END AS Tranche_de_maturite,
CASE WHEN MaturityDate IS NULL THEN 10 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)<0 THEN 9 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 1 THEN 1 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 3 THEN 2 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 5 THEN 3 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 7 THEN 4 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 10 THEN 5 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 15 THEN 6 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 20 THEN 7 ELSE 8 END END END END END END END END END AS Ordre_Tranche,
indexWeight,
level2,
MarketValue,
OADuration,
AssetSwapMargin
INTO #TMP_IBOXX
FROM TX_IBOXX where date=@dateJ

-- total iboxx
IF @durAdjusted = 1
BEGIN
	INSERT INTO #tmp(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		'a' As maturityPillar,
		0 as Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM #TMP_IBOXX where date=@dateJ
	-- total maturityPillar
	INSERT INTO #tmp(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette,
		'      ' + Tranche_de_maturite As 'Level',
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM #TMP_IBOXX where date=@dateJ group by Tranche_de_maturite,Ordre_Tranche
	-- total Level2
	INSERT INTO #tmp(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		level2 As niveauLevel2, 
		--Level4 As niveauLevel4,
		--'' As niveauDette, 
		'             ' + level2 As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ'
	FROM #TMP_IBOXX where date=@dateJ group by Tranche_de_maturite,Ordre_Tranche,level2
END
ELSE
-- en ponderé sans adjustement
BEGIN
	INSERT INTO #tmp(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		'a' As maturityPillar,
		0 as Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM #TMP_IBOXX where date=@dateJ
	-- total maturityPillar
	INSERT INTO #tmp(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette,
		'      ' + Tranche_de_maturite As 'Level',
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM #TMP_IBOXX where date=@dateJ group by Tranche_de_maturite,Ordre_Tranche
	-- total Level2
	INSERT INTO #tmp(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		level2 As niveauLevel2, 
		--Level4 As niveauLevel4,
		--'' As niveauDette, 
		'             ' + level2 As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ'
	FROM #TMP_IBOXX where date=@dateJ group by Tranche_de_maturite,Ordre_Tranche,level2
END

--------------------------------------------- J-1-------------------------------------------------------------

CREATE TABLE #tmp_dateJ1(	
	maturityPillar VARCHAR(60)COLLATE DATABASE_DEFAULT,
	Ordre_Tranche TINYINT,
	niveauLevel2 VARCHAR(60)COLLATE DATABASE_DEFAULT,
	--niveauLevel4 VARCHAR(160), 
	--niveauDette VARCHAR(5),
	poidsJ FLOAT, 
	Level VARCHAR(160)COLLATE DATABASE_DEFAULT,
	ASMJ1 FLOAT	
)

select 
Date,MaturityDate,
CASE WHEN MaturityDate IS NULL THEN NULL ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)<0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 1 THEN '0-1 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END END AS Tranche_de_maturite,
CASE WHEN MaturityDate IS NULL THEN 10 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)<0 THEN 9 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 1 THEN 1 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 3 THEN 2 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 5 THEN 3 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 7 THEN 4 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 10 THEN 5 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 15 THEN 6 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 20 THEN 7 ELSE 8 END END END END END END END END END AS Ordre_Tranche,
indexWeight,
level2,
MarketValue,
OADuration,
AssetSwapMargin
INTO #TMP_IBOXX2
FROM TX_IBOXX where date=@dateJ1

-- total iboxx
IF @durAdjusted = 1
BEGIN
	INSERT INTO #tmp_dateJ1(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ1)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		'a' As maturityPillar,
		0 as Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM #TMP_IBOXX2 where date=@dateJ1
	-- total maturityPillar
	INSERT INTO #tmp_dateJ1(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ1)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette,
		'      ' + Tranche_de_maturite As 'Level',
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM #TMP_IBOXX2 where date=@dateJ1 group by Tranche_de_maturite,Ordre_Tranche
	-- total Level2
	INSERT INTO #tmp_dateJ1(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ1)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		level2 As niveauLevel2, 
		--Level4 As niveauLevel4,
		--'' As niveauDette, 
		'             ' + level2 As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM #TMP_IBOXX2 where date=@dateJ1 group by Tranche_de_maturite,Ordre_Tranche,level2
	
END
ELSE
-- en ponderé sans adjustement
BEGIN
	INSERT INTO #tmp_dateJ1(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ1)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		'a' As maturityPillar,
		0 as Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM #TMP_IBOXX2 where date=@dateJ1
	-- total maturityPillar
	INSERT INTO #tmp_dateJ1(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ1)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette,
		'      ' + Tranche_de_maturite As 'Level',
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM #TMP_IBOXX2 where date=@dateJ1 group by Tranche_de_maturite,Ordre_Tranche
	-- total Level2
	INSERT INTO #tmp_dateJ1(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ1)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		level2 As niveauLevel2, 
		--Level4 As niveauLevel4,
		--'' As niveauDette, 
		'             ' + level2 As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM #TMP_IBOXX2 where date=@dateJ1 group by Tranche_de_maturite,Ordre_Tranche,level2
END

	UPDATE #tmp
	SET ASMJ1 = td.ASMJ1
	FROM #tmp t, #tmp_dateJ1 td
	WHERE t.niveauLevel2 = td.niveauLevel2 and t.maturityPillar=td.maturityPillar and t.Ordre_Tranche = td.Ordre_Tranche

--------------------------------------------- J-2-------------------------------------------------------------
CREATE TABLE #tmp_dateJ2(
	maturityPillar VARCHAR(60)COLLATE DATABASE_DEFAULT,
	Ordre_Tranche TINYINT,
	niveauLevel2 VARCHAR(60)COLLATE DATABASE_DEFAULT,
	--niveauLevel4 VARCHAR(160), 
	--niveauDette VARCHAR(5),
	poidsJ FLOAT, 
	Level VARCHAR(160)COLLATE DATABASE_DEFAULT,
	ASMJ2 FLOAT	
)

select 
Date,MaturityDate,
CASE WHEN MaturityDate IS NULL THEN NULL ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)<0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 1 THEN '0-1 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END END AS Tranche_de_maturite,
CASE WHEN MaturityDate IS NULL THEN 10 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)<0 THEN 9 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 1 THEN 1 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 3 THEN 2 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 5 THEN 3 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 7 THEN 4 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 10 THEN 5 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 15 THEN 6 ELSE CASE WHEN DATEDIFF(Day,Date,MaturityDate)/365.25 < 20 THEN 7 ELSE 8 END END END END END END END END END AS Ordre_Tranche,
indexWeight,
level2,
MarketValue,
OADuration,
AssetSwapMargin
INTO #TMP_IBOXX3
FROM TX_IBOXX where date=@dateJ2

-- total iboxx
IF @durAdjusted = 1
BEGIN
	INSERT INTO #tmp_dateJ2(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ2)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		'a' As maturityPillar,
		0 as Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM #TMP_IBOXX3 where date=@dateJ2
	-- total maturityPillar
	INSERT INTO #tmp_dateJ2(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ2)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette,
		'      ' + Tranche_de_maturite As 'Level',
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM #TMP_IBOXX3 where date=@dateJ2 group by Tranche_de_maturite,Ordre_Tranche
	-- total Level2
	INSERT INTO #tmp_dateJ2(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ2)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		level2 As niveauLevel2, 
		--Level4 As niveauLevel4,
		--'' As niveauDette, 
		'             ' + level2 As 'Level', 
		SUM(MarketValue*OADuration*AssetSwapMargin)/SUM(MarketValue*OADuration) AS 'ASMJ1'
	FROM #TMP_IBOXX3 where date=@dateJ2 group by Tranche_de_maturite,Ordre_Tranche,level2
	
END
ELSE
-- en ponderé sans adjustement
BEGIN
	INSERT INTO #tmp_dateJ2(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ2)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		'a' As maturityPillar,
		0 as Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4, 
		--'' As niveauDette, 
		'IBoxx € corporates' As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM #TMP_IBOXX3 where date=@dateJ2
	-- total maturityPillar
	INSERT INTO #tmp_dateJ2(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ2)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		'' As niveauLevel2, 
		--'' As niveauLevel4,
		--'' As niveauDette,
		'      ' + Tranche_de_maturite As 'Level',
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM #TMP_IBOXX3 where date=@dateJ2 group by Tranche_de_maturite,Ordre_Tranche
	-- total Level2
	INSERT INTO #tmp_dateJ2(poidsJ,maturityPillar,Ordre_Tranche,niveauLevel2,Level,ASMJ2)
	SELECT 
		SUM(indexWeight) As 'poidsJ',
		Tranche_de_maturite As maturityPillar,
		Ordre_Tranche,
		level2 As niveauLevel2, 
		--Level4 As niveauLevel4,
		--'' As niveauDette, 
		'             ' + level2 As 'Level', 
		SUM(MarketValue*AssetSwapMargin)/SUM(MarketValue) AS 'ASMJ1'
	FROM #TMP_IBOXX3 where date=@dateJ2 group by Tranche_de_maturite,Ordre_Tranche,level2

END
	
	UPDATE #tmp
	SET ASMJ2 = td.ASMJ2
	FROM #tmp t, #tmp_dateJ2 td
	WHERE t.niveauLevel2 = td.niveauLevel2 and t.maturityPillar=td.maturityPillar and t.Ordre_Tranche = td.Ordre_Tranche





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
FROM #tmp order by Ordre_Tranche,niveauLevel2



DROP TABLE #tmp
DROP TABLE #tmp_dateJ1
DROP TABLE #tmp_dateJ2

drop table #TMP_IBOXX
drop table #TMP_IBOXX2
drop table #TMP_IBOXX3
--SELECT * FROM TX_IBOXX WHERE date='30/08/2011'

--EXECUTE IBoxxASM '20/10/2011', '18/10/2011', '13/10/2011'