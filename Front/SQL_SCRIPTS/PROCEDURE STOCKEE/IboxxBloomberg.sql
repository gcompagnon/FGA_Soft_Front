/*
Récupere positions communes entre iboxx et prime oblig
*/
   
DROP PROCEDURE IboxxBloomberg
    
GO

CREATE PROCEDURE IboxxBloomberg

	   @date   DATETIME
AS 

--DECLARE @date AS DATETIME
--SET @date='26/09/2011'

declare @total as float
set @total=(SELECT SUM(fga.valeur_boursiere + fga.coupon_couru) FROM PTF_FGA fga where dateinventaire=@date and compte='6100034')

-- ytm pondere de l iboxx
declare @iboxxYTM as float
set @iboxxYTM=(select SUM(indexWeight*AnnualYield)/100 from TX_IBOXX where date =@date)
declare @iboxxMD as float
set @iboxxMD=(select SUM(indexWeight*AnnualModDuration)/100 from TX_IBOXX where date =@date)
declare @iboxxDuration as float
set @iboxxDuration=(select SUM(indexWeight*Duration)/100 from TX_IBOXX where date =@date)


SELECT f.isin_titre, SUM(f.valeur_boursiere + f.coupon_couru)/@total*100  As 'poids'
into #poids
FROM PTF_FGA f
WHERE  f.dateinventaire=@date and f.compte='6100034'
GROUP BY isin_titre
--SELECT * FROm #poids



SELECT DISTINCT
	f.isin_titre As 'Isin', 
	f.libelle_titre As 'Issuer Name',
	f.pays As 'Pays',
	f.maturite as 'Maturite',
	f.quantite As 'Nominal',
	f.sous_secteur As 'Sous Secteur',
	CASE WHEN i.tier = '*' THEN 'SEN' ELSE i.tier END AS 'Dette',
	p.poids As '%',
	'','','','','','','','','','','','','','','','','','','','','','','',
	'' AS 'YTM',
	'' AS 'MD',
	'' AS 'Duration'
	--'=BDP(INDIRECT(ADRESSE(LIGNE();1;3)) & '+ CHAR(34) + ' CORP'+CHAR(34)+';INDIRECT(ADRESSE(1;COLONNE();1)))'  as 'RTG_SP_LT_LC_ISSUER_CREDIT' ,
FROM PTF_FGA f
	LEFT OUTER JOIN  TX_IBOXX i ON f.isin_titre=i.isin  and i.date=@date
	LEFT OUTER JOIN  #poids p ON p.isin_titre=f.isin_titre
WHERE  f.dateinventaire=@date and f.compte='6100034' and f.isin_titre NOT IN('Cash OPCVM')

UNION

SELECT 	'QW5A' As 'Isin', 
	'**Iboxx**' As 'Issuer Name',
	'' As 'Pays',
	'' As 'maturite',
	'' As 'Nominal',
	'' As 'Sous Secteur',
	'' As 'Dette',
	'' AS 'poids',
	'','','','','','','','','','','','','','','','','','','','','','','',
	@iboxxDuration AS 'Duration',
	@iboxxMD AS 'MD',	
	@iboxxYTM AS 'YTM'
ORDER by libelle_titre

DROP TABLE #poids


/*
SELECT DISTINCT
	i.isin As 'Isin', 
	f.libelle_titre As 'Issuer Name',
	CASE WHEN i.tier = '*' THEN 'SEN' ELSE i.tier END AS 'Dette'
	--'=BDP(INDIRECT(ADRESSE(LIGNE();1;3)) & '+ CHAR(34) + ' CORP'+CHAR(34)+';INDIRECT(ADRESSE(1;COLONNE();1)))'  as 'RTG_SP_LT_LC_ISSUER_CREDIT' ,
FROM PTF_FGA f, TX_IBOXX i
WHERE f.isin_titre=i.isin and f.dateinventaire=@date and i.date=@date and f.compte='6100034'
ORDER by libelle_titre
*/
/*
SELECT f.isin_titre FROm PTF_FGA f where f.dateinventaire='24/08/2011' and f.compte='6100034'
EXCEPT 
SELECT i.isin FROM TX_IBOXX i WHERE  i.date='24/08/2011' 
SELECT DISTINCt libelle_titre FROM PTF_FGA where isin_titre IN ('Cash OPCVM',
'FR0000102873',
'FR0000180994',
'FR0000475709',
'FR0010337667',
'FR0010359687',
'FR0010397885',
'FR0010664599',
'FR0010948257',
'RXU1',
'XS0204937634',
'XS0495012428',
'XS0613543957',
'XS0640936067')
*/
/*

*/

--EXEC IboxxBloomberg '24/08/2011'
--SELECT * FROM TX_IBOXX
--SELECT * FROM PTF_FGA where compte='6100034'

