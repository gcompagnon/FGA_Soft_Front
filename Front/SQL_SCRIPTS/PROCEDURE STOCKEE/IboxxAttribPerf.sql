DROP PROCEDURE IboxxAttribPerf

GO 

CREATE PROCEDURE IboxxAttribPerf
		@compte VARCHAR(7),
		@dateJ1 DATETIME,
		@dateJ DATETIME
AS 

/*
DECLARE @compte AS VARCHAR(7)
SET @compte= '6100034'
DECLARE @dateJ1 AS DATETIME
SET @dateJ1= '16/09/2011'
DECLARE @dateJ AS DATETIME
SET @dateJ= '19/09/2011'
EXECUTE IboxxAttribPerf '6100034', '16/09/2011', '19/09/2011'
*/


/* COMMENTAIRE ALAIN
Un deuxième tableau émetteur modulable :
En premier, une liste déroulante permettra de se focaliser sur un secteur en particulier.
Après le choix du secteur, on obtiendra la liste des émetteurs de ce secteur.
Exemple si on choisit les banques :
BNP  -40bp
BofA +20bp
CITI   +15bp
SG    -78bp
...
On obtient ainsi le résultat suivant : La sélection des titres BNP dans le portefeuille comparée à la selection des titres bancaires dans l'indice a coûté -40bp de performance relative au fonds.
En associant les deux tableaux, on peut savoir si l'allocation sectorielle du fonds a contribué positivement ou négativement à la performance relative et si le fait d'avoir plus ou moins d'un émetteur en particulier a été un bon choix parmi les autres émetteurs du secteur.
*/


--iboxx-------------------------------------------------------------------------------
SELECT
	i.isin,
	CASE WHEN i.tier='*' THEN 'Seniors' ELSE i.tier END As 'tier',
	i.level4, 
	i.IndexWeight, 
	i.indexPrice  
Into #infoJ1
FROM TX_IBOXX i where i.date= @dateJ1
--SELECT * FROM #infoJ1

SELECT 
	i.isin,
	i.level4,
	CASE WHEN i.tier='*' THEN 'Seniors' ELSE i.tier END As 'tier',
	j.IndexWeight as IndexWeightJ1, 
	i.indexPrice  as indexPriceJ,
	j.indexPrice  as indexPriceJ1 
Into #infoIboxx
FROM TX_IBOXX i, #infoJ1 j
WHERE i.date= @dateJ and i.isin = j.isin 
--SELECT * FROM #infoIboxx

SELECT
	level4,
	SUM(IndexWeightJ1) AS poidsLevel4Iboxx,
	(SUM((indexPriceJ * IndexWeightJ1)/IndexPriceJ1)/ SUM(IndexWeightJ1))-1 As perfLevel4Iboxx
Into #perfLevel4Iboxx
FROM #infoIboxx i
GROUP BY  i.level4
--SELECT * FROM #perfLevel4Iboxx	

SELECT
	level4,
	tier,
	SUM(IndexWeightJ1) AS poidsLevel4Iboxx,
	(SUM((indexPriceJ * IndexWeightJ1)/IndexPriceJ1)/ SUM(IndexWeightJ1))-1 As perfLevel4Iboxx
Into #perfLevel4TierIboxx
FROM #infoIboxx i
GROUP BY  i.level4, i.tier

SELECT
	level4,
	tier,
	isin,
	SUM(IndexWeightJ1) AS poidsLevel4Iboxx,
	(SUM((indexPriceJ * IndexWeightJ1)/IndexPriceJ1)/ SUM(IndexWeightJ1))-1 As perfLevel4Iboxx
Into #perfLevel4TierIsinIboxx
FROM #infoIboxx i
GROUP BY  i.level4, i.tier, i.isin
--SELECT * FROM #perfLevel4TierIboxx	

DECLARE @perfGlobIboxx As FLOAT
SET @perfGlobIboxx = (SELECT (SUM((indexPriceJ * IndexWeightJ1)/IndexPriceJ1)/ SUM(IndexWeightJ1))-1 As perfLevel4Iboxx FROM #infoIboxx i)
--SELECT @perfGlobIboxx



-- prime oblig : table de référence----------------------------------------------------------------------
SELECT isin_titre, libelle_titre, secteur, sous_secteur, valeur_boursiere, coupon_couru 
INTO #fga 
FROM PTF_FGA fga 
where dateinventaire=@dateJ1 and compte=@compte
--SELECT * FROm #fga

declare @total as float
set @total=(SELECT SUM(fga.valeur_boursiere + fga.coupon_couru) FROM #fga fga )

--level4 fga
SELECT	
		ic.level4,
		SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as 'poids'
INTO #fgaLevel4 
FROM #fga fga,TX_IBOXX_CORRESPONDANCE ic
WHERE  fga.sous_secteur = ic.sous_secteur
GROUP BY ic.level4
--SELECT * FROm #fgaLevel4

SELECT	
		ic.level4,
		ic.debt,  
		SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as 'poids'
INTO #fgaTier 
FROM #fga fga,TX_IBOXX_CORRESPONDANCE ic
WHERE  fga.sous_secteur = ic.sous_secteur
GROUP BY ic.level4,ic.debt
--SELECT * FROm #fgaTier

SELECT	
		ic.level4,
		ic.debt,
		fga.isin_titre,
		fga.libelle_titre,  
		SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as 'poids'
INTO #fgaTierIsin 
FROM #fga fga,TX_IBOXX_CORRESPONDANCE ic
WHERE  fga.sous_secteur = ic.sous_secteur  
GROUP BY ic.level4,ic.debt,fga.isin_titre,fga.libelle_titre



--affichage------------------------------------------------------------------------------------
SELECT 
	'0000 TOTAL 0000' As Level4,
	'' As Tier,
	'' As Isin,
	'' As Libelle,
    NULL As 'Perf relative Ispé - Iglobale',
	NULL As 'Poids relative FGA vs I',
	ROUND(SUM(100*(l.perfLevel4Iboxx  - @perfGlobIboxx) * (f.poids - l.poidsLevel4Iboxx)),2) As 'Performance'
FROM #perfLevel4Iboxx l, #fgaLevel4 f
WHERE f.level4 = l.level4

UNION

SELECT 
	l.level4 As level4,
	'' As tier,
	'' As isin,
	'' As libelle,
    ROUND((l.perfLevel4Iboxx  - @perfGlobIboxx)*100,2) As 'perf relative I',
	ROUND((f.poids - l.poidsLevel4Iboxx),2) As 'poids relative FGA vs I',
	ROUND(100*(l.perfLevel4Iboxx  - @perfGlobIboxx) * (f.poids - l.poidsLevel4Iboxx),2) As 'perf'
FROM #perfLevel4Iboxx l, #fgaLevel4 f
WHERE f.level4 = l.level4

UNION

SELECT 
	l.level4 As level4,
	l.tier As tier,
	'' As isin,
	'' As libelle,
    ROUND((l.perfLevel4Iboxx  - @perfGlobIboxx)*100,2) As 'perf relative I',
	ROUND((f.poids - l.poidsLevel4Iboxx),2) As 'poids relative FGA vs I',
	ROUND(100*(l.perfLevel4Iboxx  - @perfGlobIboxx) * (f.poids - l.poidsLevel4Iboxx),2) As 'perf'
FROM #perfLevel4TierIboxx l, #fgaTier f
WHERE f.level4 = l.level4 and l.tier=f.debt

UNION

SELECT 
	l.level4 As level4,
	l.tier As tier,
	l.isin As isin,
	f.libelle_titre As libelle,
    ROUND((l.perfLevel4Iboxx  - @perfGlobIboxx)*100,2) As 'perf relative I',
	ROUND((f.poids - l.poidsLevel4Iboxx),2) As 'poids relative FGA vs I',
	ROUND(100*(l.perfLevel4Iboxx  - @perfGlobIboxx) * (f.poids - l.poidsLevel4Iboxx),2) As 'perf'
FROM #perfLevel4TierIsinIboxx l, #fgaTierIsin f 
WHERE f.level4 = l.level4  and l.isin=f.isin_titre 

ORDER BY level4, tier, libelle


--Suppression des tables temporaires
DROP TABLE #infoJ1
DROP TABLE #infoIboxx
DROP TABLE #perfLevel4Iboxx
DROP TABLE #perfLevel4TierIboxx
DROP TABLE #perfLevel4TierIsinIboxx
DROP TABLE #FGA
DROP TABLE #FGALevel4
DROP TABLE #FGATier
DROP TABLE #FGATierIsin

-- EXECUTE IboxxAttribPerf '6100034', '16/09/2011', '19/09/2011'
-- SELECT * FROM PTF_FGA where compte='6100034' and dateinventaire='16/09/2011' and sous_secteur lIKE '%EMPRUNT TIER 1 BANQUE%'
-- SELECT * FROM TX_IBOXX where date='16/09/2011' and level4='banks'