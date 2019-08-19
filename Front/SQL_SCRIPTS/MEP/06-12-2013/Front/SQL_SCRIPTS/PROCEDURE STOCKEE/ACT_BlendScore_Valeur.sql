USE [E2DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[ACT_BlendScore_Valeur]    Script Date: 12/06/2013 09:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_BlendScore_Valeur]

	@Date As DATETIME,
	@Winsor_coef AS Float,
	@Class_name AS VARCHAR(20)

AS

-- TODO: define winsorisation percent

----DECLARE	@Date As DATETIME
----DECLARE	@Winsor_coef AS Float
----DECLARE	@Class_name AS VARCHAR(20)
----SET @Class_name = 'FGA_US'
----SET @date = '01/12/2013'
----SET @Winsor_coef = 90

--EXECUTE ACT_BlendScore_Valeur @Date, @Winsor_coef


/* ************************************ */
-- 0) Add unknown values to ACT_VALEUR
--INSERT INTO ACT_VALEUR (ISIN, TICKER_BLOOMBERG, LIBELLE, ID_FGA, IS_EURO)
--SELECT distinct fact.ISIN, tick.TICKER_BLOOMBERG, fact.company_name, fact.sector, fact.IS_EURO
--FROM DATA_FACTSET fact
--FULL OUTER JOIN	(-- TODO: REMOVE once ACT_TICKER is unused.
--						SELECT distinct t.ISIN, t.TICKER_BLOOMBERG
--						FROM ACT_TICKER t
--						INNER JOIN	(
--										SELECT ISIN, MAX(date) AS maxdate
--										FROM ACT_TICKER
--										GROUP BY ISIN
--									) last ON last.ISIN = t.ISIN
--					) tick ON tick.ISIN = fact.ISIN
--WHERE NOT fact.ISIN IN (SELECT ISIN FROM ACT_VALEUR)
--	AND NOT sector IS NULL

/* ************************************ */
-- 1) Récupère les derniers coefficients de chaque secteur.
--Remarque : dédouble NULL secteur de ACT_COEF_SECTEUR sur chaque secteur de ACT_COEf_SECTEUR n'ayant pas de coefficient
SELECT
	*
INTO #coefs
FROM (
	SELECT
		sect.id_fga AS id_fga
		, sect.id_critere AS id_critere
		, sect.coef AS coef
	FROM ACT_COEF_SECTEUR sect
	INNER JOIN (
			SELECT MAX(date) AS maxdate, id_critere, id_fga
			FROM ACT_COEF_SECTEUR
			WHERE date <= @date
			GROUP BY id_critere, id_fga
		) last ON last.id_critere = sect.id_critere
				AND last.id_fga = sect.id_fga
				AND sect.date = last.maxdate
	GROUP BY sect.id_fga, sect.id_critere, sect.coef
	UNION
	SELECT 
		CAST(fga.code AS integer) AS id_fga
		, crit.id_critere AS id_critere
		, crit.coef AS coef
	FROM (
			SELECT *
			FROM ref_security.SECTOR fga
			WHERE 
				NOT EXISTS (select id_fga
					FROM (
						SELECT MAX(date) AS maxdate, id_fga
						FROM ACT_COEF_SECTEUR
						WHERE date <= @date
						GROUP BY id_fga
					) tmp
					WHERE id_fga = fga.id)
				AND fga.class_name=@Class_name
		) fga
	CROSS JOIN (
			SELECT
				sect.id_fga AS id_fga
				, sect.id_critere AS id_critere
				, sect.coef AS coef
			FROM ACT_COEF_SECTEUR sect
			INNER JOIN (
					SELECT MAX(date) AS maxdate, id_critere
					FROM ACT_COEF_SECTEUR
					WHERE date <= @date AND id_fga IS NULL
					GROUP BY id_critere
				) last ON last.id_critere = sect.id_critere
					AND sect.date = last.maxdate
			WHERE sect.id_fga IS NULL
		) crit
	) tmp
ORDER BY id_fga, id_critere

--######################################--
--select * from #coefs
--######################################--

/* ************************************ */
-- 2) Récupère les critères enfants des coefficients.
SELECT distinct 
	crit.id_critere
	, crit.nom
	,	(SELECT nom FROM ACT_COEF_CRITERE
			WHERE id_critere = (	SELECT id_parent 
									FROM ACT_COEF_CRITERE
									WHERE id_critere=crit.id_parent
								)
				AND is_sector = 1
		) AS root
	, crit.CAP_min
	, crit.CAP_max
	, crit.inverse
INTO #criteres
FROM #coefs coefs
	INNER JOIN ACT_COEF_CRITERE crit on crit.id_critere = coefs.id_critere
WHERE
	crit.is_sector = 1
	AND crit.id_parent IS NOT NULL
	AND crit.id_parent NOT IN (	SELECT id_critere FROM ACT_COEF_CRITERE
								WHERE id_parent IS NULL)
ORDER BY nom

--######################################--
--SELECT * FROM #criteres
--######################################--

-- Récupère la liste des critères
DECLARE @crit_list VARCHAR(MAX)
SELECT @crit_list=COALESCE(@crit_list+N', ',N'')
	+ CASE WHEN inverse = 1 THEN N' 1/NULLIF(fac.' + nom + ',0)'  ELSE nom END
	+ N' AS "' + nom + N'"'
FROM #criteres

/* ************************************ */
-- 3) Récupère les différentes valeurs avec les critères précédents.
DECLARE @sql AS VARCHAR(max)
SET @sql = 'SELECT fac.ISIN
, (select FGA_SECTOR from DATA_FACTSET where FGA_SECTOR is not null and GICS_SUBINDUSTRY=fac.SECTOR) AS id_fga
, ' + @crit_list + ' INTO ##values
FROM DATA_FACTSET fac
WHERE fac.ISIN IS not null and fac.date=''' + CONVERT(VARCHAR,@date) + ''' and'
IF @Class_name = 'FGA_EU'
BEGIN
SET @sql = @sql + ' fac.MXEU is not null'
END
ELSE
BEGIN
SET @sql = @sql + ' fac.MXUSLC is not null'
END
EXEC(@sql)

SELECT *
INTO #values
FROM ##values
DROP TABLE ##values

--######################################--
--select * from #values order by id_fga, ISIN
--######################################--

/* ************************************ */
-- 4) Winsorisation & CAP sur #values
SELECT *
INTO #datas
FROM #criteres

DECLARE @crit VARCHAR(50),	@winsmin float,	@winsmax float, @capmin float,	@capmax float, @I INTEGER
SET @winsmin = (100 - @Winsor_coef) / 2
SET @winsmax = 100 - @winsmin
SET @I = 1

WHILE EXISTS(SELECT * FROM #datas)
	BEGIN
		-- Get critere and cap values
		SELECT TOP 1 @crit = nom, @capmin = CAP_min, @capmax = CAP_max
		FROM #datas 
		ORDER BY id_critere
		
		--SELECT @crit AS nom, @capmin AS capmin, @capmax AS capmax
		
		EXECUTE ACT_Winsorise_CAP_Valeur N'#values', --@tab
										 @crit,		--@col
										 @winsmin,	--@winsMin
										 @winsmax,	--@winsMax
										 @capmin,	--@capMin
										 @capmax	--@capMax
		DELETE FROM #datas
		WHERE nom = @crit
		
	END
DROP TABLE #datas

/* ************************************ */
-- 4.bis) Récupération des notes ISR.

SET @sql = 'SELECT isr.ISIN, val.id_fga, isr.[Note Actions] as ISR
INTO ##ISR
FROM ISR_NOTE isr
INNER JOIN  (
				SELECT ISIN, max(date) AS maxdate
				FROM ISR_NOTE
				WHERE Date <= ''' + CONVERT(VARCHAR,@date) + ''' 
				GROUP BY ISIN
			) last ON isr.ISIN = last.ISIN AND isr.date = maxdate
INNER JOIN #values val ON val.ISIN = isr.ISIN'
EXEC(@sql)

SELECT *
INTO #ISR
FROM ##ISR
DROP TABLE ##ISR

SELECT
	id_fga,
	AVG(ISR) as moy
INTO #Moy_
FROM #ISR
group by id_fga
SELECT
	val.id_fga,
	(CASE
		WHEN ABS(moy.moy - MAX(val.ISR)) > ABS(moy.moy - MIN(val.ISR))
		THEN ABS(moy.moy - MAX(val.ISR))
		ELSE ABS(moy.moy - MIN(val.ISR))
	END) as dst
INTO #Dst_
FROM #ISR val
INNER JOIN #Moy_ moy ON moy.id_fga=val.id_fga 
group by val.id_fga, moy.moy
order by val.id_fga
UPDATE val SET val.ISR=(CASE WHEN (val.ISR) > moy.moy
										THEN (5*(1+ABS((val.ISR)-moy.moy)/dst.dst))
										ELSE (5*(1-ABS((val.ISR)-moy.moy)/dst.dst))
									END)
FROM #ISR val
INNER JOIN #Moy_ moy ON moy.id_fga=val.id_fga
INNER JOIN #Dst_ dst ON dst.id_fga=val.id_fga
WHERE dst.dst <> 0
DROP TABLE #Moy_
DROP TABLE #Dst_

--######################################--
--select * from #values order by id_fga, ISIN
--select * from #ISR order by id_fga
--######################################--

/* ************************************ */
--6) calcul du score de chaque indicateur
SELECT nom
INTO #datas2
FROM #criteres

WHILE EXISTS(SELECT * FROM #datas2)
	BEGIN
		-- Get critere and cap values
		SELECT TOP 1 @crit = nom
		FROM #datas2
		
		-- calcul de la moyenne et la distance de chaque indicateur
		-- #Dst_ = distance || #O_ = Ecart type || #Moy_

		-- Calcul de la moyenne
		-- Calcul de la distance
		-- Calcul du score : data > Moy
		-- Calcul du score : data <= Moy
		SET @sql = 
		'SELECT
			id_fga,
			AVG(' + @crit + ') as moy
		INTO #Moy_
		FROM #values
		group by id_fga
				
		SELECT
			val.id_fga,
			(CASE
				WHEN ABS(moy.moy - MAX(val.' + @crit + ')) > ABS(moy.moy - MIN(val.' + @crit + '))
				THEN ABS(moy.moy - MAX(val.' + @crit + '))
				ELSE ABS(moy.moy - MIN(val.' + @crit + '))
			END) as dst
		INTO #Dst_
		FROM #values val
		INNER JOIN #Moy_ moy ON moy.id_fga=val.id_fga 
		group by val.id_fga, moy.moy
		order by val.id_fga

		UPDATE val SET val.' + @crit + '=(CASE WHEN (val.' + @crit + ') > moy.moy
												THEN (5*(1+ABS((val.' + @crit + ')-moy.moy)/dst.dst))
												ELSE (5*(1-ABS((val.' + @crit + ')-moy.moy)/dst.dst))
											END)
		FROM #values val
		INNER JOIN #Moy_ moy ON moy.id_fga=val.id_fga
		INNER JOIN #Dst_ dst ON dst.id_fga=val.id_fga
		WHERE dst.dst <> 0
		
		DROP TABLE #Moy_
		DROP TABLE #Dst_'
        
        EXEC(@sql)
        
		DELETE FROM #datas2
		WHERE nom = @crit
	END
DROP TABLE #datas2

--######################################--
--select * from #values order by id_fga, ISIN
--select * from #Moy_
--select * from #Dst_
--######################################--

--6 BIS) Save les Notes des champs pour les radars
UPDATE fac SET
		fac.N_EPS_CHG_NTM=val.EPS_CHG_NTM,
		fac.N_EPS_TREND=val.EPS_TREND,
		fac.N_EPS_VAR_RSD=val.EPS_VAR_RSD,
		fac.N_SALES_CHG_NTM=val.SALES_CHG_NTM,
		fac.N_SALES_TREND=val.SALES_TREND,
		fac.N_SALES_VAR_RSD=val.SALES_VAR_RSD,

		fac.N_PE_NTM=val.PE_NTM,
		fac.N_PE_ON_MED5Y=val.PE_ON_MED5Y,
		fac.N_PE_PREMIUM_ON_HIST=val.PE_PREMIUM_ON_HIST,
		fac.N_P_TBV_NTM=val.P_TBV_NTM,
		fac.N_PB_NTM=val.PB_NTM,
		fac.N_PB_ON_MED5Y=val.PB_ON_MED5Y,
		fac.N_P_TBV_ON_MED5Y=val.P_TBV_ON_MED5Y,
		fac.N_PB_PREMIUM_ON_HIST=val.PB_PREMIUM_ON_HIST,
		fac.N_DIV_YLD_NTM=val.DIV_YLD_NTM,
		
		fac.N_PBT_SALES_NTM=val.PBT_SALES_NTM,
		fac.N_EBIT_MARGIN_NTM=val.EBIT_MARGIN_NTM,
		fac.N_PBT_RWA_NTM=val.PBT_RWA_NTM,
		fac.N_FCF_TREND=val.FCF_TREND,
		fac.N_NET_DEBT_EBITDA_NTM=val.NET_DEBT_EBITDA_NTM,
		fac.N_ROE_NTM=val.ROE_NTM,
		fac.N_ROTE_NTM=val.ROTE_NTM,
		fac.N_COST_INCOME_NTM=val.COST_INCOME_NTM
FROM DATA_FACTSET fac
INNER JOIN #values val ON val.ISIN=fac.ISIN
WHERE fac.DATE=@Date

--7) calcul des scores Growth, Value, Quality, ISR
SELECT co.id_fga, cr.nom, cr.root, co.coef 
INTO #tmp
FROM #criteres cr
INNER JOIN #coefs co ON co.id_critere=cr.id_critere order by nom

SELECT nom
INTO #datas3
FROM #criteres

DECLARE @test as float
SET @test = 0
SELECT '****TEST****' AS 'ISIN', null AS 'id_fga', @test as 'value', 'TESTTESTTESTTESTTESTTESTTESTTEST' AS 'nom', 'TESTTESTTESTTESTTESTTESTTESTTEST' AS 'root', null AS 'coef'
INTO #tmp2

WHILE EXISTS(SELECT * FROM #datas3)
	BEGIN
		SELECT TOP 1 @crit = nom
		FROM #datas3
		
		SET @sql = 
		'INSERT INTO #tmp2(ISIN, id_fga, value, nom, root, coef)
		SELECT val.ISIN, val.id_fga, COALESCE(val.' + @crit + ', 0) as ''value'', ''' + @crit + ''' AS ''nom'', tmp.root, tmp.coef
		FROM #values val
		INNER JOIN #tmp tmp ON tmp.id_fga=val.id_fga
		WHERE tmp.nom = ''' + @crit + ''''
        
        EXEC(@sql)
		
		DELETE FROM #datas3
		WHERE nom = @crit
	END
DROP TABLE #datas3
DELETE FROM #tmp2
WHERE ISIN = '****TEST****'
		
SELECT distinct t1.ISIN, t1.id_fga
, (SELECT (SUM(t2.value*t2.coef)/SUM(t2.coef)) AS 'CROISSANCE' FROM #tmp2 t2 WHERE t2.ISIN=t1.ISIN AND t2.root='CROISSANCE') AS 'CROISSANCE'
, (SELECT (SUM(t2.value*t2.coef)/SUM(t2.coef)) AS 'QUALITE' FROM #tmp2 t2 WHERE t2.ISIN=t1.ISIN AND t2.root='QUALITE') AS 'QUALITE'
, (SELECT (SUM(t2.value*t2.coef)/SUM(t2.coef)) AS 'VALORISATION' FROM #tmp2 t2 WHERE t2.ISIN=t1.ISIN AND t2.root='VALORISATION') AS 'VALORISATION'
, (SELECT top 1 isr.ISR FROM #ISR isr where isr.ISIN=t1.ISIN) AS 'ISR'
INTO #CQVI_Score
FROM #tmp2 t1
ORDER by ISIN

--######################################--
--select * from #tmp
--select * from #tmp2 order by ISIN, root, nom
--select * from #CQVI_Score
--######################################--

--8) calcul du Score Final
SELECT distinct sc.ISIN, sc.id_fga,
(((SELECT SUM(t2.coef) FROM #tmp2 t2 WHERE t2.ISIN=t1.ISIN AND t2.root='CROISSANCE')*sc.CROISSANCE
+ (SELECT SUM(t2.coef) FROM #tmp2 t2 WHERE t2.ISIN=t1.ISIN AND t2.root='QUALITE')*sc.QUALITE
+ (SELECT SUM(t2.coef) FROM #tmp2 t2 WHERE t2.ISIN=t1.ISIN AND t2.root='VALORISATION')*sc.VALORISATION
+ (SELECT top 1 co.coef FROM ACT_COEF_CRITERE t2 INNER JOIN #coefs co ON co.id_critere=t2.id_critere
	WHERE sc.id_fga=co.id_fga AND t2.id_parent is null AND t2.nom='ISR')*sc.ISR)/100
) AS 'FINAL_SCORE', sc.QUALITE, sc.CROISSANCE, sc.VALORISATION, (sc.ISR) AS ISR
INTO #tmp3
FROM #tmp2 t1
INNER JOIN #CQVI_Score sc ON sc.ISIN=t1.ISIN

--######################################--
--select * from #tmp3 order by id_fga, ISIN
--######################################--

--8) Update de la table DATA_FACTSET
IF @Class_name = 'FGA_EU'
BEGIN
UPDATE fac SET
GARPN_YIELD_S = tmp.QUALITE,
GARPN_VALUE_S = tmp.VALORISATION,
GARPN_GROWTH_S = tmp.CROISSANCE,
GARPN_ISR_S = tmp.ISR,
GARPN_TOTAL_S = tmp.FINAL_SCORE
FROM DATA_FACTSET fac
INNER JOIN #tmp3 tmp ON tmp.ISIN=fac.ISIN
WHERE fac.MXEU is not null AND fac.MXUSLC is null AND fac.DATE=@Date
END
ELSE
BEGIN
UPDATE fac SET
GARPN_YIELD_S = tmp.QUALITE,
GARPN_VALUE_S = tmp.VALORISATION,
GARPN_GROWTH_S = tmp.CROISSANCE,
GARPN_ISR_S = tmp.ISR,
GARPN_TOTAL_S = tmp.FINAL_SCORE
FROM DATA_FACTSET fac
INNER JOIN #tmp3 tmp ON tmp.ISIN=fac.ISIN
WHERE fac.MXEU is null AND fac.MXUSLC is not null AND fac.DATE=@Date
END

UPDATE fac SET
ESG = (CASE WHEN isr.[Note Actions] = -1 THEN 'EXCLU' ELSE convert(VARCHAR, isr.[Note Actions]) END)
FROM DATA_FACTSET fac
INNER JOIN ISR_NOTE isr ON isr.ISIN=fac.ISIN
INNER JOIN  (
				SELECT ISIN, max(date) AS maxdate
				FROM ISR_NOTE
				WHERE Date <= @Date 
				GROUP BY ISIN
			) last ON isr.ISIN = last.ISIN AND isr.date = maxdate
WHERE fac.DATE=@Date


-- END) DROP TABLE => Liberation Mémoire
DROP TABLE #values
DROP TABLE #coefs
DROP TABLE #criteres
DROP TABLE #ISR
DROP TABLE #tmp
DROP TABLE #tmp2
DROP TABLE #tmp3
DROP TABLE #CQVI_Score

