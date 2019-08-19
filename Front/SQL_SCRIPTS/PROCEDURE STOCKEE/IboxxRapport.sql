--
-- Remplie la table TX_IBOXX_RAPPORT pour une date donnée
-- la table permet de construire le rapport de comparaison entre prime oblig et iboxx corporate


ALTER PROCEDURE IboxxRapport
		@date_inventaire DATETIME,
		@compte VARCHAR(7)
AS 



DECLARE @date_inventaire AS DATETIME
SET @date_inventaire='03/06/2015'
DECLARE @compte AS VARCHAR(7)
SET @compte= '6100034'
--EXECUTE IboxxRapport '01/12/2011', '6100034'


-- perimettre complet du ptf
SELECT * INTO #ptf_fga FROM PTF_FGA fga WHERE dateinventaire=@date_inventaire and compte=@compte 

-- pour eviter que les libelles soient identiques, et provoquent un bug d insertion dans TX_IBOXX_RAPPORT_PRIME
update #ptf_fga
set Libelle_titre = 'Liquidité('+code_titre+')'
from #ptf_fga
where Libelle_titre = 'Liquidité(Future)'

--Argent total dans prime oblig
declare @total as float
set @total=(SELECT SUM(fga.valeur_boursiere + fga.coupon_couru) FROM #ptf_fga fga where dateinventaire=@date_inventaire and compte=@compte)

-- le perimetre s occupe de tout le portefeuille hors derives et liquidites et OPCVM : titres vifs
select * into #ptf_fga_vifs FROM PTF_FGA WHERE dateinventaire=@date_inventaire and compte=@compte
 and type_produit NOT LIKE 'CALL%' and type_produit NOT LIKE 'FUTURE%' and type_produit NOT LIKE 'SWAP%' and type_produit <> 'Cash' 

-- les valeurs de la colonne ptf sont calculées par rapport à l actif hors treso.
declare @total_vifs as float
set @total_vifs=(SELECT SUM(fga.valeur_boursiere + fga.coupon_couru) FROM #ptf_fga_vifs fga )

-- mise à jour du cas particulier: TCHEQUE, REPUBLIQUE  pour République Tchèque
-- dans les autres cas, pour enlever les caracteres accentués , on utilise p.french COLLATE Latin1_General_CI_AI

update #ptf_fga_vifs
set Pays = 'TCHEQUE, REPUBLIQUE'
where Pays ='République Tchèque'

update #ptf_fga_vifs
set Pays = 'RUSSIE, FEDERATION DE'
where Pays ='RUSSIE'


update #ptf_fga_vifs
set Pays = 'FRANCE'
where Sous_secteur = 'OPCVM TRESORERIE' and Pays ='Not Available'

-------------------------------------DEBUT ------------------------------------------

-------------------------------------ONGLET rapport de réference ------------------------------------------

DELETE FROM TX_IBOXX_RAPPORT_PRIME where date = @date_inventaire

--LIGNE VIE MOYENNE
SELECT
		libelle_titre,
		DATEDIFF(Day,getdate(),fga.maturite)/365.25 As 'date',
		'libelle' = CASE 
			 WHEN      DATEDIFF(Day,getdate(),fga.maturite)/365.25 IS NULL THEN 'X'
			 WHEN      DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 1 THEN '< 1 an'
			 WHEN 1 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25  and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 3 THEN '1-3 ans'
			 WHEN 3 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25  and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 5 THEN '3-5 ans'
			 WHEN 5 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25  and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 7 THEN '5-7 ans'
			 WHEN 7 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25  and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 10 THEN '7-10 ans'
			 WHEN 10 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25 and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 15 THEN '10-15 ans'
			 ELSE '15-30 ans'
			 --ELSE '> 30 ans'
		END,
		'vie_residuelle' = CASE 
			 WHEN      DATEDIFF(Day,getdate(),fga.maturite)/365.25 IS NULL THEN 0
			 WHEN      DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 1 THEN 1
			 WHEN 1 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25  and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 3 THEN 3
			 WHEN 3 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25  and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 5 THEN 5
			 WHEN 5 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25  and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 7 THEN 7
			 WHEN 7 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25  and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 10 THEN 10
			 WHEN 10 <= DATEDIFF(Day,getdate(),fga.maturite)/365.25 and DATEDIFF(Day,getdate(),fga.maturite)/365.25 < 15 THEN 15
			 ELSE 30
		END,
		fga.valeur_boursiere + fga.coupon_couru As 'evaluation',
		fga.sensibilite,
		(fga.valeur_boursiere + fga.coupon_couru)/@total_vifs*fga.sensibilite As 'apport_sensibilite'
INTO #tmp
FROM #ptf_fga fga
WHERE 
	dateinventaire = @date_inventaire and
	compte = @compte

--SELECT * from #tmp order by date

INSERT INTO TX_IBOXX_RAPPORT_PRIME
(date,niveau,libelle_titre,vie_residuelle,evaluation,sensibilite,actif,apport_sensibilite)
SELECT 	@date_inventaire As date,
		'vie' As niveau,
		'libelle_titre' = CASE vie_residuelle
			 WHEN 0 THEN 'X'
			 WHEN 1 THEN '< 1 an'    
			 WHEN 3 THEN '1-3 ans'
			 WHEN 5 THEN '3-5 ans'
			 WHEN 7 THEN '5-7 ans'
			 WHEN 10 THEN '7-10 ans'
			 WHEN 15 THEN '10-15 ans'
			 ELSE '15-30 ans'
		END,
		vie_residuelle,
		SUM(evaluation) As 'evaluation',
		0 As 'sensibilite',
		SUM(evaluation)/@total *100 As 'actif', 
		SUM(apport_sensibilite) AS 'apport_sensibilite'
FROM #tmp
GROUP BY vie_residuelle

DROP TABLE #tmp


--DETAIL PORTEFEUILLE
INSERT INTO TX_IBOXX_RAPPORT_PRIME
(date,niveau,code_titre,libelle_titre,rating,nominal,tx_rdt,cours,coupon_couru,vie_residuelle,evaluation,duration,sensibilite,actif,apport_sensibilite)
SELECT
		@date_inventaire AS date,
		'titre' As niveau,
		fga.code_titre,
		fga.libelle_titre, 
		fga.rating,
		fga.quantite As 'nominal', 
		fga.coupon As tx_rdt,
		fga.coursclose As 'cours', 
		case 
		when fga.quantite = 0 then 0
		else fga.Coupon_couru*100/fga.quantite end As 'coupon_couru',
		DATEDIFF(Day,getdate(),fga.maturite)/365.25 As 'vie_residuelle',
		fga.valeur_boursiere+fga.coupon_couru 'evaluation',
		fga.duration,
		fga.sensibilite,
		(fga.coupon_couru+fga.valeur_boursiere)/@total*100 As actif,
		(fga.coupon_couru+fga.valeur_boursiere)/@total_vifs * fga.sensibilite As apport_sensibilite
FROM
	#ptf_fga fga
WHERE 
	fga.dateinventaire = @date_inventaire and
	fga.compte = @compte
ORDER BY
	vie_residuelle

/*
SELECT 
	niveau,
	code_titre,
	libelle_titre,  
	ROUND(nominal,2) As 'Nominal', 
	ROUND(tx_rdt,2) As 'Tx rdt %', 
	ROUND(cours,2) As 'Cours close', 
	ROUND(coupon_couru,2) As 'Coupon couru', 
	ROUND(evaluation,2) As 'Evaluation', 
	ROUND(vie_residuelle,2) As 'Vie résiduelle', 
	ROUND(duration,2) As 'Duration', 
	ROUND(sensibilite,2) As 'Sensibilité', 
	rating As 'Rating', 
	ROUND(actif,2) As 'Actif %', 
	ROUND(apport_sensibilite,2) As 'Apport sensibilité' 
FROM TX_IBOXX_RAPPORT_PRIME where date = '27/07/2011' 
ORDER BY vie_residuelle
*/









DELETE FROM TX_IBOXX_RAPPORT where date=@date_inventaire

------------------------------------------------LEVEL2---------------------------------------------------------
INSERT INTO TX_IBOXX_RAPPORT 
(date,niveau,level2,level4,debt,country,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
SELECT  @date_inventaire As 'date',
		'level2' As 'niveau',
		i.level2,
		'' As 'level4',
		'' AS 'debt',
		'' AS 'country',
		SUM(i.IndexWeight) As 'poids_Iboxx',
		'' As 'poids_prime_oblig',
		'' As 'ecart_poids',
		SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'apport_Iboxx',
		'' As 'apport_prime_oblig',
		'' As 'ecart_apport'
from TX_IBOXX i where i.date= @date_inventaire
GROUP BY i.Level2 
-- Pour les données de portefeuille, prendre en priorite le secteur de l iboxx
SELECT 
case 
when i.Level2 is null then ic.level2
else i.Level2 end as 'Level2',
       (fga.valeur_boursiere + fga.coupon_couru) as 'poids',
       ((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite) As 'sensi'
into #tmp_level2
FROM #ptf_fga fga
LEFT OUTER JOIN TX_IBOXX_RAPPORT as ir on ir.date=@date_inventaire and ir.niveau='level2'
LEFT OUTER JOIN TX_IBOXX_CORRESPONDANCE as ic on ic.sous_secteur = fga.sous_secteur and ic.level2 = ir.Level2
LEFT OUTER JOIN TX_IBOXX as i on i.date=@date_inventaire and i.ISIN =fga.code_Titre
WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 	   

SELECT 
level2,
       SUM(poids)/@total*100 as 'poids',
       SUM(sensi)/@total_vifs As 'sensi'
into #tmp_grp_level2
FROM #tmp_level2
GROUP BY level2 

select * from #tmp_grp_level2
select * from #tmp_level2

update TX_IBOXX_RAPPORT
set poids_prime_oblig  = l2.poids,
	apport_prime_oblig = l2.sensi
from TX_IBOXX_RAPPORT ir, #tmp_grp_level2 l2
where  ir.niveau='level2' and ir.Level2 = l2.level2  and ir.date=@date_inventaire
drop table #tmp_level2
drop table #tmp_grp_level2



------------------------------------------------LEVEL4---------------------------------------------------------
INSERT INTO TX_IBOXX_RAPPORT 
(date,niveau,level2,level4,debt,country,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
SELECT  @date_inventaire As 'date',
		'level4' As 'niveau',
		i.level2,
		i.level4,
		'' AS 'debt',
		'' AS 'country',
		SUM(IndexWeight) As 'poids_Iboxx',
		'' As 'poids_prime_oblig',
		'' As 'ecart_poids',
		SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'apport_Iboxx',
		'' As 'apport_prime_oblig',
		'' As 'ecart_apport'
from TX_IBOXX i where i.date=@date_inventaire GROUP BY i.Level2, i.Level4

SELECT ic.level2,
       ic.level4,
       SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as 'poids',
       SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As 'sensi'
into #tmp_level4
FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic, TX_IBOXX_RAPPORT ir 
WHERE  fga.dateinventaire=@date_inventaire and ir.date=@date_inventaire and fga.compte=@compte 
	   and ir.niveau='level4' and ir.Level2 = ic.level2 and ir.Level4 = ic.level4 and fga.sous_secteur = ic.sous_secteur
GROUP BY ic.level2,ic.level4
--SELECT * FROM #tmp_level4

update TX_IBOXX_RAPPORT
set poids_prime_oblig= l4.poids,
	apport_prime_oblig = l4.sensi
from TX_IBOXX_RAPPORT ir, #tmp_level4 l4
where  ir.niveau='level4' and ir.Level2 = l4.level2 and ir.Level4 = l4.level4  and ir.date=@date_inventaire
drop table #tmp_level4




------------------------------------------------TIER---------------------------------------------------------
INSERT INTO TX_IBOXX_RAPPORT 
(date,niveau,level2,level4,debt,country,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
SELECT  @date_inventaire As 'date',
		'tier' As 'niveau',
		level2,
		level4,
		CASE WHEN tier ='*' THEN 'Seniors' ELSE tier END AS 'debt',
		'' AS 'country',
		SUM(IndexWeight) As 'poids_Iboxx',
		'' As 'poids_prime_oblig',
		'' As 'ecart_poids',
		SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'apport_Iboxx',
		'' As 'apport_prime_oblig',
		'' As 'ecart_apport'
from TX_IBOXX i where i.date=@date_inventaire and i.level2 IN('Financials','Non-Financials')  
GROUP BY i.Level2,i.Level4,i.tier

SELECT ic.level2,
 ic.level4,
 ic.debt,
 SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as 'poids',
 SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As 'sensi'
into #tmp_debt
FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic, TX_IBOXX_RAPPORT ir 
WHERE  fga.dateinventaire=@date_inventaire and ir.date=@date_inventaire and fga.compte=@compte 
	   and ir.niveau='tier' and ir.Level2 = ic.level2 and ir.Level4 = ic.level4 and ir.debt = ic.debt and fga.sous_secteur = ic.sous_secteur
GROUP BY ic.level2, ic.level4, ic.debt

--Select * from #tmp_debt
update TX_IBOXX_RAPPORT
set poids_prime_oblig= d.poids,
	apport_prime_oblig = d.sensi
from TX_IBOXX_RAPPORT ir, #tmp_debt d
where  ir.niveau='tier' and ir.Level2 = d.level2 and ir.Level4 = d.level4 and ir.debt = d.debt  and ir.date=@date_inventaire
drop table #tmp_debt


------------------------------------------------COUNTRY---------------------------------------------------------
INSERT INTO TX_IBOXX_RAPPORT 
(date,niveau,level2,level4,debt,country,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
SELECT  @date_inventaire As 'date',
		'country' As 'niveau',
		level2,
		level4,
		CASE WHEN tier ='*' THEN 'Seniors' ELSE tier END AS 'debt',
		country,
		SUM(IndexWeight) As 'poids_Iboxx',
		'' As 'poids_prime_oblig',
		'' As 'ecart_poids',
		SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'apport_Iboxx',
		'' As 'apport_prime_oblig',
		'' As 'ecart_apport'
from TX_IBOXX i 
where 
	date=@date_inventaire --AND (i.Level4='Banks' OR level2='Non-Financials') AND i.tier In('*','LT2') 
GROUP BY i.Level2,i.Level4,i.tier, i.country

SELECT ic.level2,
 ic.level4,
 ic.debt,
 p.libelle_anglais As country,
 SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as 'poids',
 SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As 'sensi'
into #tmp_pays
FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic, TX_IBOXX_RAPPORT ir, PAYS p 
WHERE  fga.dateinventaire=@date_inventaire and ir.date=@date_inventaire and fga.compte=@compte 
	   and ir.niveau='tier' and ir.Level2 = ic.level2 and ir.Level4 = ic.level4 and ir.debt = ic.debt and fga.sous_secteur = ic.sous_secteur and fga.pays=p.libelle
GROUP BY ic.level2, ic.level4, ic.debt, p.libelle_anglais

--Select * from #tmp_pays
update TX_IBOXX_RAPPORT
set poids_prime_oblig= tp.poids,
	apport_prime_oblig = tp.sensi
from TX_IBOXX_RAPPORT ir, #tmp_pays tp
where  ir.niveau='country' and ir.Level2 = tp.level2 and ir.Level4 = tp.level4 and ir.debt = tp.debt  and ir.date=@date_inventaire and tp.country=ir.country
drop table #tmp_pays

--ligne particulière FUTURE/liquidités
declare @poids_liquidite as float
set @poids_liquidite= (	SELECT SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as 'poids'
						FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic
						WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
						and ic.level2='Liquidités' and fga.sous_secteur = ic.sous_secteur)
declare @sensi_liquidite as float
set @sensi_liquidite= (	SELECT SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs as 'sensi'
						FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic
						WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
						and ic.level2='Liquidités' and fga.sous_secteur = ic.sous_secteur)

INSERT INTO TX_IBOXX_RAPPORT (date,niveau,level2,level4,debt,country,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
VALUES (@date_inventaire,'liquidités','Liquidités','','','',0.0,@poids_liquidite,0,0,@sensi_liquidite,0)



declare @poids_future as float
set @poids_future= (	SELECT SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as 'poids'
						FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic
						WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
						and fga.type_produit LIKE '%FUTURE%' and fga.sous_secteur = ic.sous_secteur)
declare @sensi_future as float
set @sensi_future= (	SELECT SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs as 'sensi'
						FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic
						WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
						and fga.type_produit LIKE '%FUTURE%' and fga.sous_secteur = ic.sous_secteur)

INSERT INTO TX_IBOXX_RAPPORT (date,niveau,level2,level4,debt,country,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
VALUES (@date_inventaire,'futures','Futures','','','',0,@poids_future,0,0,@sensi_future,0)


declare @poids_agencies as float
set @poids_agencies= (	SELECT SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as poids
						FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic
						WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
						and ic.level2='agencies' and fga.sous_secteur = ic.sous_secteur)
declare @sensi_agencies as float
set @sensi_agencies= (	SELECT SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs as sensi
						FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic
						WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
						and ic.level2='agencies' and fga.sous_secteur = ic.sous_secteur)

INSERT INTO TX_IBOXX_RAPPORT (date,niveau,level2,level4,debt,country,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
VALUES (@date_inventaire,'agencies','Agencies','','','',0,@poids_agencies,0,0,@sensi_agencies,0)


declare @poids_foncier as float
set @poids_foncier= (	SELECT SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as poids
						FROM #ptf_fga fga
						WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte and
						fga.sous_secteur LIKE '%EMPRUNT FONCIER ET HYPOTHECAIRE%')
						
declare @sensi_foncier as float
set @sensi_foncier= (	SELECT SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs as sensi
						FROM #ptf_fga fga
						WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
						and fga.sous_secteur LIKE '%EMPRUNT FONCIER ET HYPOTHECAIRE%')

INSERT INTO TX_IBOXX_RAPPORT (date,niveau,level2,level4,debt,country,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
VALUES (@date_inventaire,'foncier','Foncier','','','',0,@poids_foncier,0,0,@sensi_foncier,0)



UPDATE TX_IBOXX_RAPPORT
SET 	ecart_poids = poids_prime_oblig-poids_Iboxx,
		ecart_apport = apport_prime_oblig- apport_Iboxx
WHERE date=@date_inventaire




/*
SELECT niveau,
		level2 As 'Secteur iBoxx niveau 2',
		Level4 As 'Secteur iBoxx niveau 4',
		debt As 'Type de dette',
		country As 'Pays',
		ROUND(poids_Iboxx,2) As 'iBoxx %',
		ROUND(poids_prime_oblig,2) As 'Prime oblig %',
		ROUND(ecart_poids,2) As 'Ecart %',
		ROUND(apport_Iboxx,2) As 'Apport sensi iBoxx',
		ROUND(apport_prime_oblig,2) As 'Apport sensi Prime oblig',
		ROUND(ecart_apport,2) As 'Ecart apport sensi'
FROM TX_IBOXX_RAPPORT where date = '27/07/2011' 
ORDER BY level2,level4,debt,country
*/












/*
declare @date_inventaire as DATETIME
set @date_inventaire = '27/07/2011'
declare @compte as VARCHAR(7)
set @compte = '6100034'
declare @total as float
set @total=(SELECT SUM(fga.valeur_boursiere + fga.coupon_couru) FROM #ptf_fga fga where dateinventaire=@date_inventaire and compte=@compte)
*/

DELETE FROM TX_IBOXX_RAPPORT_RATING where date=@date_inventaire

------------------------------------------------TOTAL RATING---------------------------------------------------------
INSERT INTO TX_IBOXX_RAPPORT_RATING
(date,niveau,groupe,rating,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
SELECT  DISTINCT @date_inventaire As 'date',
				'tot_rating' As 'niveau', 
				groupe_rating As 'groupe',
				'' As 'rating',
				'' As 'poids_Iboxx',
				'' As 'poids_prime_oblig',
				'' As 'ecart_poids',
				'' As 'apport_Iboxx',
				'' As 'apport_prime_oblig',
				'' As 'ecart_apport'				
FROM TX_RATING where groupe_rating <> 'NULL'


SELECT  i.rating As 'groupe',
		SUM(i.IndexWeight) As 'poids',
		SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'sensi'
INTO #tmp_groupe_rating_iboxx
FROM TX_IBOXX i
WHERE i.date=@date_inventaire
GROUP BY i.rating 

UPDATE TX_IBOXX_RAPPORT_RATING
SET		poids_Iboxx  = i.poids,
		apport_Iboxx = i.sensi
from #tmp_groupe_rating_iboxx i, TX_IBOXX_RAPPORT_RATING ir 
where 
	ir.date=@date_inventaire and
	ir.groupe = i.groupe and ir.niveau='tot_rating'

DROP TABLE #tmp_groupe_rating_iboxx


------------------------------------------------Lignes des futures et liquidités---------------------------------------------------------

SELECT fga.code_Titre 
INTO #RATING_LISTE_FUTURE
FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic
WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
and fga.type_produit LIKE '%FUTURE%' and fga.sous_secteur = ic.sous_secteur

declare @poids_future2 as float
set @poids_future2= (	SELECT SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as poids
						FROM #ptf_fga fga
						WHERE  fga.code_titre in (select code_titre from #RATING_LISTE_FUTURE) )
declare @sensi_future2 as float
set @sensi_future2= (	SELECT SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs as sensi
						FROM #ptf_fga fga
						WHERE  fga.code_titre in (select code_titre from #RATING_LISTE_FUTURE) )

INSERT INTO TX_IBOXX_RAPPORT_RATING (date,niveau,groupe,rating,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
VALUES (@date_inventaire,'futures','Futures','',0,@poids_future2,0,0,@sensi_future2,0)



SELECT fga.code_Titre 
INTO #RATING_LISTE_LIQUIDITES
FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic
WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
and ic.level2='Liquidités' and fga.sous_secteur = ic.sous_secteur


declare @poids_liquidite2 as float
set @poids_liquidite2= (	SELECT SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 as poids
						FROM #ptf_fga fga
						WHERE  fga.code_titre in (select code_titre from #RATING_LISTE_LIQUIDITES) )
declare @sensi_liquidite2 as float
set @sensi_liquidite2= (	SELECT SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs as sensi
						FROM #ptf_fga fga
						WHERE  fga.code_titre in (select code_titre from #RATING_LISTE_LIQUIDITES) )
						
INSERT INTO TX_IBOXX_RAPPORT_RATING (date,niveau,groupe,rating,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
VALUES (@date_inventaire,'liquidités','Liquidités','',0,@poids_liquidite2,0,0,@sensi_liquidite2,0)

---------------------------------------------------- FIN Lignes des futures et liquidités-----------------------------------------------

-- les lignes total_rating, pour la catégorie , utiliser la table de correspondance: TX_RATING
SELECT  r.groupe_rating As 'groupe',
		SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 As 'poids',
		SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As 'sensi'
INTO #tmp_groupe_rating
FROM #ptf_fga fga, TX_RATING r
WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte and fga.rating <> 'NULL' and
	   r.agence='Interne' and r.type_rating='LT' and r.rating=fga.rating
	   and fga.code_titre not in (select code_titre from #RATING_LISTE_LIQUIDITES)
	   and fga.code_titre not in (select code_titre from #RATING_LISTE_FUTURE)	   
GROUP BY r.groupe_rating

UPDATE TX_IBOXX_RAPPORT_RATING
SET poids_prime_oblig  = gr.poids,
	apport_prime_oblig = gr.sensi
FROM TX_IBOXX_RAPPORT_RATING ir, #tmp_groupe_rating gr
WHERE  ir.niveau='tot_rating' and ir.groupe = gr.groupe  and ir.date=@date_inventaire
drop table #tmp_groupe_rating


------------------------------------------------RATING---------------------------------------------------------
INSERT INTO TX_IBOXX_RAPPORT_RATING
(date,niveau,groupe,rating,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
SELECT  @date_inventaire As 'date',
		'rating' As 'niveau', 
		r.groupe_rating As 'groupe',
		fga.rating As 'rating',
		'' As 'poids_Iboxx',
		SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 As 'poids_prime_oblig',
		'' As 'ecart_poids',
		'' As 'apport_Iboxx',
		SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As 'apport_prime_oblig',
		'' As 'ecart_apport'
FROM #ptf_fga fga, TX_RATING r
WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte and fga.rating <> 'NULL' and
	   r.agence='Interne' and r.type_rating='LT' and r.rating=fga.rating
	   and fga.code_titre not in (select code_titre from #RATING_LISTE_LIQUIDITES)
	   and fga.code_titre not in (select code_titre from #RATING_LISTE_FUTURE)	   
GROUP BY r.groupe_rating,fga.rating

--Calcul de l ecart
UPDATE TX_IBOXX_RAPPORT_RATING
SET 	ecart_poids = poids_prime_oblig-poids_Iboxx,
		ecart_apport = apport_prime_oblig- apport_Iboxx
WHERE date=@date_inventaire and niveau IN ('tot_rating','futures','liquidités')

--SELECT DISTINCT rating from #ptf_fga where compte = @compte and dateinventaire=@date_inventaire

--Select * from TX_IBOXX_RAPPORT_RATING where date = '27/07/2011' order by groupe, rating






--RAPPORT STRATE----------------------------------------------------------------------------------------------------
DELETE FROM TX_IBOXX_RAPPORT_STRATES where date=@date_inventaire

--IBOXX Strates
SELECT  
		i.life As 'test',
		CASE WHEN i.life >= 15 THEN '15 ans +' ELSE CASE WHEN i.life >= 10 THEN '10-15 ans' ELSE CASE WHEN i.life >= 7 THEN '7-10 ans' ELSE CASE WHEN i.life >= 5 THEN '5-7 ans' ELSE CASE WHEN i.life >= 3 THEN '3-5 ans' ELSE CASE WHEN i.life >= 1 THEN '1-3 ans' ELSE '< 1 an' END END END END END END  As 'strates',
		i.IndexWeight As 'poids',
		(i.IndexWeight * i.AnnualModDuration)/100  As 'sensi'
INTO #tmp_strates2
FROM TX_IBOXX i
WHERE  i.date=@date_inventaire 


SELECT 	i.strates, 
		SUM(i.poids) As poids, 
		SUM(i.sensi) As sensi
INTO #tmp_strates_group2
FROM #tmp_strates2 i 
GROUP BY i.strates
--SELECT * FROM #tmp_strates_group


INSERT INTO TX_IBOXX_RAPPORT_STRATES
(date,niveau,strates,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
SELECT  @date_inventaire As 'date',
		'temps' As 'niveau',
		 t.strates As 'strates',
		 t.poids As 'poids_Iboxx',
		 '' As 'poids_prime_oblig',
		 '' As 'ecart_poids',
		 t.sensi As 'apport_Iboxx',
		 '' As apport_prime_oblig,
		 '' As ecart_apport 
from #tmp_strates_group2 t


--PRIME OBLIG  Strates
SELECT  
		fga.secteur,
		DATEDIFF(Day,getdate(),fga.maturite)/365.25 As 'test',
		CASE WHEN DATEDIFF(Day,getdate(),fga.maturite)/365.25 >= 15 THEN '15 ans +' ELSE CASE WHEN DATEDIFF(Day,getdate(),fga.maturite)/365.25 >= 10 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,getdate(),fga.maturite)/365.25 >= 7 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,getdate(),fga.maturite)/365.25 >= 5 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,getdate(),fga.maturite)/365.25 >= 3 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,getdate(),fga.maturite)/365.25 >= 1 THEN '1-3 ans' ELSE '< 1 an' END END END END END END  As 'strates',
		(fga.valeur_boursiere + fga.coupon_couru)/@total_vifs*100 As 'poids',
		((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As 'sensi'
INTO #tmp_strates
FROM #ptf_fga_vifs fga
--SELECT * FROM #tmp_strates

SELECT  
		'temps' As 'niveau', 
		strates  As strates,
		SUM(poids) As 'poids_prime_oblig',
		SUM(sensi) As 'apport_prime_oblig'
INTO #tmp_strates_group
FROM #tmp_strates
GROUP BY strates


update TX_IBOXX_RAPPORT_STRATES
set poids_prime_oblig  = t.poids_prime_oblig,
	apport_prime_oblig = t.apport_prime_oblig
from TX_IBOXX_RAPPORT_STRATES ir, #tmp_strates_group t
where  ir.niveau='temps' and ir.strates = t.strates and ir.date=@date_inventaire


INSERT INTO TX_IBOXX_RAPPORT_STRATES
(date,niveau,strates,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
SELECT  @date_inventaire As 'date',
		'temps' As 'niveau',
		 t.strates As 'strates',
		 '' As 'poids_Iboxx',
		 t.poids_prime_oblig ,
		 '' As 'ecart_poids',
		 '' As 'apport_Iboxx',
		 t.apport_prime_oblig,
		 '' As ecart_apport 
from #tmp_strates_group t
where t.strates not in (select strates from #tmp_strates_group2)

drop table #tmp_strates2
drop table #tmp_strates_group2
drop table #tmp_strates
drop table #tmp_strates_group



declare @sensi_future3 as float
set @sensi_future3= (	SELECT SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs as sensi
						FROM #ptf_fga fga,TX_IBOXX_CORRESPONDANCE ic
						WHERE  fga.dateinventaire=@date_inventaire and fga.compte=@compte 
						and ic.level2 lIKE '%Emprunt%' and fga.sous_secteur = ic.sous_secteur)

INSERT INTO TX_IBOXX_RAPPORT_STRATES (date,niveau,strates,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
VALUES (@date_inventaire,'futures','Futures',0,0,0,0,@sensi_future3,0)


UPDATE TX_IBOXX_RAPPORT_STRATES
SET 	ecart_poids = poids_prime_oblig-poids_Iboxx,
		ecart_apport = apport_prime_oblig- apport_Iboxx
WHERE date=@date_inventaire

--Select * from TX_IBOXX_RAPPORT_STRATES where date = @date_inventaire order by  niveau


--RAPPORT PAR EMISSION--------------------------------------------------------------------------------------------------
DELETE FROM TX_IBOXX_RAPPORT_EMETTEUR where date = @date_inventaire

INSERT INTO TX_IBOXX_RAPPORT_EMETTEUR 
(date,niveau,emetteur,isin_titre,emission,poids_prime_oblig, apport_prime_oblig)
SELECT 
		@date_inventaire AS date,
		'emetteur' As 'Niveau',
		CASE WHEN fga.emetteur IS NULL THEN 'Liquidité' ELSE LEFT(fga.emetteur,60) END As emetteur,
		'' As isin_titre,
		'' As emission,
		SUM(fga.coupon_couru+fga.valeur_boursiere)/@total_vifs*100 As poids_prime_oblig,
		--'' AS poids_iboxx,
		--'' As ecart_poids,
		--'' As Pays_Risque,
		--'' As apport_Iboxx,
		SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As apport_prime_oblig
		--'' As ecart_apport
FROM #ptf_fga_vifs fga
GROUP BY 
	fga.emetteur
--HAVING(SUM(fga.coupon_couru+fga.valeur_boursiere)/@total*100) > 0


INSERT INTO TX_IBOXX_RAPPORT_EMETTEUR 
(date,niveau,emetteur,emission,isin_titre,pays_risque,poids_prime_oblig,apport_prime_oblig)
SELECT 
		@date_inventaire AS date,
		'emission' As 'Niveau',
		CASE WHEN fga.emetteur IS NULL THEN 'Liquidité' ELSE LEFT(fga.emetteur,60) END As emetteur,
		libelle_titre As emisson,
		isin_titre,
		UPPER(fga.pays) as 'Pays_risque',
		SUM(fga.coupon_couru+fga.valeur_boursiere)/@total_vifs*100 As poids_prime_oblig,
		SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As apport_prime_oblig
FROM #ptf_fga_vifs fga
GROUP BY 
	fga.emetteur,
	fga.libelle_titre,
	fga.isin_titre,
	fga.pays,
	sensibilite
--HAVING(SUM(fga.coupon_couru+fga.valeur_boursiere)/@total*100) > 0

SELECT f.isin_titre,i.indexWeight,(i.IndexWeight * i.AnnualModDuration)/100 As 'sensi' 
INTO
#iboxx
FROM #ptf_fga_vifs f, TX_iboxx i 
where 
i.date=@date_inventaire and i.isin=f.isin_titre


UPDATE TX_IBOXX_RAPPORT_EMETTEUR
SET poids_iboxx= i.indexWeight,
	apport_iboxx = i.sensi
FROM #iboxx i, TX_IBOXX_RAPPORT_EMETTEUR e 
where 
	e.date=@date_inventaire and e.isin_titre = i.isin_titre and e.niveau='emission'
DROP TABLE #iboxx

--combler les trous
SELECT emetteur, SUM(apport_iboxx) As apport_iboxx, SUM(poids_iboxx) As poids_iboxx 
into #poids_iboxx 
FROM TX_IBOXX_RAPPORT_EMETTEUR where date=@date_inventaire and isin_titre<>'' and emission <>'' group by emetteur 

UPDATE TX_IBOXX_RAPPORT_EMETTEUR
SET poids_iboxx = p.poids_iboxx, apport_iboxx = p.apport_iboxx
FROM TX_IBOXX_RAPPORT_EMETTEUR e, #poids_iboxx p
where 
	e.date=@date_inventaire and isin_titre='' and emission ='' and e.emetteur=p.emetteur	
DROP TABLE #poids_iboxx


UPDATE TX_IBOXX_RAPPORT_EMETTEUR
SET ecart_poids= poids_prime_oblig - poids_iboxx ,
	ecart_apport = apport_prime_oblig - apport_iboxx
FROM TX_IBOXX_RAPPORT_EMETTEUR e 
where 
	e.date=@date_inventaire --and e.niveau='emission'

/*
SELECT	niveau, 
		emetteur As 'Emetteur',
		isin_titre As 'Isin',
		emission AS 'Emission',
		pays_risque as 'Pays Risque',
		ROUND(poids_prime_oblig,2)  As 'Prime oblig %',
		ROUND(poids_iboxx,2)  As 'IBOXX %',
		ROUND(ecart_poids,2) As 'Ecart %',
		ROUND(apport_prime_oblig,3)  As 'Apport sensi Prime oblig',
		ROUND(apport_iboxx,3)  As 'Apport sensi IBOXX',
		ROUND(ecart_apport,3) As 'Ecart apport sensi'			
FROM TX_IBOXX_RAPPORT_EMETTEUR 
WHERE date=@date_inventaire 
order by emetteur, emission
*/




--RAPPORT PAR EMETTEUR (IBOXX)--------------------------------------------------------------------------------------------------
--EMETTEUR

DELETE FROM TX_IBOXX_RAPPORT_EMETTEUR2 WHERE date=@date_inventaire 
--niveau emetteur
INSERT INTO TX_IBOXX_RAPPORT_EMETTEUR2
(date,niveau,emetteur,isin_titre,emission,pays_risque,poids_Iboxx,apport_Iboxx)
SELECT  @date_inventaire As 'date',
		'emetteur' As 'niveau',
		LEFT(i.issuerName,60) As 'emetteur',
		'' As 'isin_titre',
		'' As 'emission',
		'' As 'pays_risque',
		SUM(i.IndexWeight) As 'poids_Iboxx',
		SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'apport_Iboxx'
from TX_IBOXX i where i.date=@date_inventaire
GROUP BY i.issuerName 
--niveau emission
INSERT INTO TX_IBOXX_RAPPORT_EMETTEUR2
(date,niveau,emetteur,isin_titre,emission,pays_risque,poids_Iboxx,poids_prime_oblig,ecart_poids,apport_Iboxx,apport_prime_oblig,ecart_apport)
SELECT  @date_inventaire As 'date',
		'emission' As 'niveau',
		LEFT(i.issuerName,60) As 'emetteur',
		i.isin As 'isin_titre',
		i.issuerTicker + ' ' + convert(varchar,i.CouponRate) + '% '+Convert(varchar,i.MaturityDate,103) As 'Emission',
		p.french as 'Pays_Risque',         
		SUM(i.IndexWeight) As 'poids_Iboxx',
		SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 As 'poids_prime_oblig',
		SUM(fga.valeur_boursiere + fga.coupon_couru)/@total*100 - SUM(i.IndexWeight) As 'ecart_poids',
		SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'apport_Iboxx',
		SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As 'apport_prime_oblig',
		SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs  - SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'ecart_apport'
from TX_IBOXX i
	LEFT OUTER JOIN #ptf_fga fga ON fga.dateinventaire= @date_inventaire and compte=@compte and fga.isin_titre =i.isin
	LEFT OUTER JOIN TX_IBOXX_CNTRY_OF_RISK as pays on pays.isin = i.isin
    LEFT OUTER JOIN ref.country as p on p.iso2 = pays.iso2	
where 
	i.date=@date_inventaire 
GROUP BY i.issuerName, i.isin, p.french,i.issuerTicker,i.CouponRate,i.MaturityDate,p.french
ORDER BY i.issuerName 


--rajouter libelle titre connu
UPDATE TX_IBOXX_RAPPORT_EMETTEUR2
SET 	
	emission = fga.libelle_titre
from TX_IBOXX_RAPPORT_EMETTEUR2 ir, #ptf_fga fga
where  
	ir.niveau='emission' and ir.date=@date_inventaire and 
	ir.isin_titre=fga.isin_titre and 
	fga.dateinventaire= @date_inventaire and fga.compte=@compte


SELECT emetteur,
 SUM(apport_prime_oblig) As apport_prime_oblig,
  SUM(poids_prime_oblig) As poids_prime_oblig 
into #poids_prime_oblig
FROM TX_IBOXX_RAPPORT_EMETTEUR2 where date=@date_inventaire and isin_titre<>'' and emission <>'' group by emetteur 

UPDATE TX_IBOXX_RAPPORT_EMETTEUR2
SET poids_prime_oblig = p.poids_prime_oblig, apport_prime_oblig = p.apport_prime_oblig
FROM TX_IBOXX_RAPPORT_EMETTEUR2 e, #poids_prime_oblig p
where 
	e.date=@date_inventaire  and isin_titre='' and emission ='' and e.emetteur=p.emetteur	
DROP TABLE #poids_prime_oblig

UPDATE TX_IBOXX_RAPPORT_EMETTEUR2
SET ecart_poids= poids_prime_oblig - poids_iboxx ,
	ecart_apport = apport_prime_oblig - apport_iboxx
FROM TX_IBOXX_RAPPORT_EMETTEUR2 e 
where 
	e.date=@date_inventaire

/*
SELECT	niveau,
		emetteur As 'Emetteur',
		isin_titre As 'Isin',
		emission As 'Emission',
		Pays_Risque,
		poids_iboxx As 'Prime oblig %',
		poids_prime_oblig As 'IBOXX %',
		ecart_poids As 'Ecart %',
		apport_iboxx As 'Apport sensi Prime oblig',
		apport_prime_oblig As 'Apport sensi IBOXX',
		ecart_apport As 'Ecart apport sensi'
FROM TX_IBOXX_RAPPORT_EMETTEUR2
WHERE date=@date_inventaire
ORDER by emetteur
*/





--RAPPORT PAYS-----------------------------------------------------------------------------------

DELETE FROM TX_IBOXX_RAPPORT_PAYS where date = @date_inventaire

--ligne pays - financiere : sur l iboxx
INSERT INTO TX_IBOXX_RAPPORT_PAYS 
(date,niveau,zone,iso2,pays,poids_iboxx,apport_iboxx)
SELECT  @date_inventaire As 'date',
		'paysfi' as 'niveau',
		case
		when z.libelle is null then 'NC'
		else z.libelle end As 'zone',
		cntry_of_risk.iso2 as 'iso2',
		case
		when p.french is null then 'NC Financial'
		else p.french + ' financials' end As 'pays',
		SUM(i.IndexWeight) As 'poids_Iboxx',
		SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'apport_Iboxx'
FROM TX_iboxx i
LEFT OUTER JOIN TX_IBOXX_CNTRY_OF_RISK as cntry_of_risk on cntry_of_risk.isin = i.isin
LEFT OUTER JOIN ref.country as p on p.iso2 = cntry_of_risk.iso2
LEFT OUTER JOIN Association_pays_zone as apz on apz.id_pays= p.iso2 
LEFT OUTER JOIN Zone as z on z.id = apz.id_zone
WHERE i.date=@date_inventaire and i.level2='Financials'
GROUP BY cntry_of_risk.iso2,p.french, z.libelle
--ligne pays - financiere : sur le ptf
SELECT
	@date_inventaire As 'date',
	z.libelle As 'zone', 
	p.iso2 as 'iso2',
	p.french + ' financials' As 'pays', 
	SUM(fga.valeur_boursiere + fga.coupon_couru)/@total_vifs*100 as 'poids_prime_oblig', 
	SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As 'apport_prime_oblig'
INTO #prime_pays_fi 
FROM #ptf_fga_vifs fga
LEFT OUTER JOIN ref.country as p on p.french COLLATE Latin1_General_CI_AI = fga.Pays COLLATE Latin1_General_CI_AI 
LEFT OUTER JOIN Association_pays_zone as apz on apz.id_pays= p.iso2 
LEFT OUTER JOIN Zone as z on z.id = apz.id_zone
WHERE fga.secteur LIKE '%CORPORATES FINANCIERES%'	
GROUP BY p.french,p.iso2,z.libelle

-- mise à jour sur le ptf pour les lignes existantes deja dans l iboxx
UPDATE TX_IBOXX_RAPPORT_PAYS
SET poids_prime_oblig= pr.poids_prime_oblig,
	apport_prime_oblig = pr.apport_prime_oblig
FROM #prime_pays_fi pr, TX_IBOXX_RAPPORT_PAYS p 
where 
	p.date=@date_inventaire and pr.pays COLLATE Latin1_General_CI_AI =p.pays COLLATE Latin1_General_CI_AI and pr.zone=p.zone and p.niveau='paysfi'

-- les lignes qui sont dans le ptf et pas ds l iboxx
INSERT INTO TX_IBOXX_RAPPORT_PAYS 
(date,niveau,zone,iso2,pays,poids_iboxx,apport_iboxx,poids_prime_oblig,apport_prime_oblig)
SELECT  @date_inventaire As 'date',
		'paysfi' as 'niveau',
		fi.zone As 'zone',
		fi.iso2 as 'iso2',
		fi.pays as 'pays',
		0 as 'poids_Iboxx',
		0 as 'apport_Iboxx',
		fi.poids_prime_oblig as 'poids_prime_oblig', 
		fi.apport_prime_oblig as 'apport_prime_oblig'
from #prime_pays_fi as fi
where fi.iso2 not in (select iso2 from TX_IBOXX_RAPPORT_PAYS where date = @date_inventaire and niveau='paysfi' and iso2 is not null) 
	
--ligne pays - hors-financiere pour l iboxx
INSERT INTO TX_IBOXX_RAPPORT_PAYS 
(date,niveau,zone,iso2,pays,poids_iboxx,apport_iboxx)
SELECT  @date_inventaire As 'date',
		'payshorsfi' as 'niveau',
		case
		when z.libelle is null then 'NC'
		else z.libelle end As 'zone',
		cntry_of_risk.iso2 as 'iso2',
		case
		when p.french is null then 'NC hors financials'
		else p.french + ' hors financials' end As 'pays',
		SUM(i.IndexWeight) As 'poids_Iboxx',
		SUM(i.IndexWeight * i.AnnualModDuration)/100 As 'apport_Iboxx'
FROM TX_iboxx i
LEFT OUTER JOIN TX_IBOXX_CNTRY_OF_RISK as cntry_of_risk on cntry_of_risk.isin = i.isin
LEFT OUTER JOIN ref.country as p on p.iso2 = cntry_of_risk.iso2
LEFT OUTER JOIN Association_pays_zone as apz on apz.id_pays= p.iso2 
LEFT OUTER JOIN Zone as z on z.id = apz.id_zone
WHERE i.date=@date_inventaire and i.level2 <> 'Financials'
GROUP BY cntry_of_risk.iso2,p.french, z.libelle

--ligne pays - hors-financiere pour le ptf , les lignes communes avec l iboxx
SELECT
	@date_inventaire As 'date',
	z.libelle As 'zone', 
	p.iso2 as 'iso2',
	p.french + ' hors financials' As 'pays', 
	SUM(fga.valeur_boursiere + fga.coupon_couru)/@total_vifs*100 as 'poids_prime_oblig', 
	SUM((fga.valeur_boursiere + fga.coupon_couru)*fga.sensibilite)/@total_vifs As 'apport_prime_oblig'
INTO #prime_pays_hors_fi 
FROM #ptf_fga_vifs fga
LEFT OUTER JOIN ref.country as p on p.french COLLATE Latin1_General_CI_AI = fga.Pays COLLATE Latin1_General_CI_AI 
LEFT OUTER JOIN Association_pays_zone as apz on apz.id_pays= p.iso2 
LEFT OUTER JOIN Zone as z on z.id = apz.id_zone
WHERE fga.secteur NOT LIKE '%CORPORATES FINANCIERES%'	
GROUP BY p.french ,p.iso2,z.libelle

UPDATE TX_IBOXX_RAPPORT_PAYS
SET poids_prime_oblig= pr.poids_prime_oblig,
	apport_prime_oblig = pr.apport_prime_oblig
FROM #prime_pays_hors_fi pr, TX_IBOXX_RAPPORT_PAYS p 
where 
	p.date=@date_inventaire and pr.pays COLLATE Latin1_General_CI_AI =p.pays COLLATE Latin1_General_CI_AI and pr.zone=p.zone and p.niveau='payshorsfi'

--ligne pays - hors-financiere les lignes qui sont dans le ptf et pas ds l iboxx
INSERT INTO TX_IBOXX_RAPPORT_PAYS 
(date,niveau,zone,iso2,pays,poids_iboxx,apport_iboxx,poids_prime_oblig,apport_prime_oblig)
SELECT  @date_inventaire As 'date',
		'payshorsfi' as 'niveau',
		fi.zone As 'zone',
		fi.iso2 as 'iso2',
		fi.pays as 'pays',
		0 as 'poids_Iboxx',
		0 as 'apport_Iboxx',
		fi.poids_prime_oblig as 'poids_prime_oblig', 
		fi.apport_prime_oblig as 'apport_prime_oblig'
from #prime_pays_hors_fi as fi
where fi.iso2 not in (select iso2 from TX_IBOXX_RAPPORT_PAYS where date = @date_inventaire and niveau='payshorsfi' and iso2 is not null) 

-- mettre ds lignes à 0 pour les lignes inexistantes (pas de lignes pays financials, pour une ligne payshorsfi existante) 
INSERT INTO TX_IBOXX_RAPPORT_PAYS 
(date,niveau,zone,iso2,pays,poids_iboxx,apport_iboxx,poids_prime_oblig,apport_prime_oblig)
SELECT distinct @date_inventaire As 'date',
		'paysfi' as 'niveau',
		fi.zone As 'zone',
		fi.iso2 as 'iso2',
		p.french + ' financials' as 'pays',
		0 as 'poids_Iboxx',
		0 as 'apport_Iboxx',
		0 as 'poids_prime_oblig', 
		0 as 'apport_prime_oblig'
from TX_IBOXX_RAPPORT_PAYS as fi
LEFT OUTER JOIN ref.country as p on p.iso2 = fi.iso2
where fi.iso2 not in (select distinct iso2 from TX_IBOXX_RAPPORT_PAYS where date = @date_inventaire and niveau='paysfi' ) 
and niveau='payshorsfi'
and  fi.date = @date_inventaire
-- mettre ds lignes à 0 pour les lignes inexistantes (pas de lignes pays hors financials, pour une ligne paysfi existante) 
INSERT INTO TX_IBOXX_RAPPORT_PAYS 
(date,niveau,zone,iso2,pays,poids_iboxx,apport_iboxx,poids_prime_oblig,apport_prime_oblig)
SELECT distinct @date_inventaire As 'date',
		'paysfi' as 'niveau',
		fi.zone As 'zone',
		fi.iso2 as 'iso2',
		p.french + ' hors financials' as 'pays',
		0 as 'poids_Iboxx',
		0 as 'apport_Iboxx',
		0 as 'poids_prime_oblig', 
		0 as 'apport_prime_oblig'
from TX_IBOXX_RAPPORT_PAYS as fi
LEFT OUTER JOIN ref.country as p on p.iso2 = fi.iso2
where fi.iso2 not in (select distinct iso2 from TX_IBOXX_RAPPORT_PAYS where date = @date_inventaire and niveau='payshorsfi' ) 
and niveau='paysfi'
and  fi.date = @date_inventaire
--ligne recapitulant le pays
INSERT INTO TX_IBOXX_RAPPORT_PAYS 
(date,niveau,zone,iso2,pays,poids_iboxx,apport_iboxx,poids_prime_oblig,apport_prime_oblig)
SELECT  @date_inventaire As 'date',
		'pays' as 'niveau',
		zone As 'zone',
		i.iso2,
		case 
		when p.french is null then 'NC pays du risk a renseigner'
		else p.french end As 'pays',
		SUM(poids_Iboxx) As 'poids_Iboxx',
		SUM(apport_Iboxx) As 'apport_Iboxx',
		SUM(poids_prime_oblig) As 'poids_prime_oblig',
		SUM(apport_prime_oblig) As 'apport_prime_oblig'
FROM TX_IBOXX_RAPPORT_PAYS i
LEFT OUTER JOIN ref.country as p on p.iso2 = i.iso2
where date=@date_inventaire
group by zone, p.french,i.iso2

--ligne recapitulatif de la zone géographique
INSERT INTO TX_IBOXX_RAPPORT_PAYS 
(date,niveau,zone,iso2,pays,poids_iboxx,apport_iboxx,poids_prime_oblig,apport_prime_oblig)
SELECT  @date_inventaire As 'date',
		'zone' As 'niveau',
		zone As 'Zone',
		'' as iso2,
		'' As pays,
		SUM(poids_Iboxx) As 'poids_Iboxx',
		SUM(apport_Iboxx) As 'apport_Iboxx',
		SUM(poids_prime_oblig) As 'poids_prime_oblig',
		SUM(apport_prime_oblig) As 'apport_prime_oblig'
FROM TX_IBOXX_RAPPORT_PAYS r
WHERE r.Date = @date_inventaire and niveau = 'pays'
group by zone

--calcul ecart ibox/ptf
UPDATE TX_IBOXX_RAPPORT_PAYS
SET ecart_poids= poids_prime_oblig - poids_Iboxx,
	ecart_apport = apport_prime_oblig - apport_Iboxx 
FROM TX_IBOXX_RAPPORT_PAYS 
where 
	date=@date_inventaire 

/*
-- la sortie utilisé dans le code VB de iboxx.vb
SELECT	
		niveau As 'Niveau',
		Zone As 'Zone',
		pays As 'Pays',
		poids_iboxx 'IBOXX %',
		CASE WHEN poids_prime_oblig IS NULL THEN 0 ELSE poids_prime_oblig END As 'Prime oblig %',
		CASE WHEN ecart_poids IS NULL THEN poids_iboxx ELSE ecart_poids END AS 'Ecart poids',
		apport_iboxx AS 'Apport sensi IBOXX',
		CASE WHEN apport_prime_oblig IS NULL THEN 0 ELSE apport_prime_oblig END As 'Prime oblig %',
		CASE WHEN ecart_apport IS NULL THEN apport_iboxx ELSE ecart_apport END AS 'Ecart apport sensi'
FROM TX_IBOXX_RAPPORT_PAYS
Where date = '02/06/2014' and Niveau <> 'pays'and Niveau <> 'zone'
ORDER BY zone,pays
*/
DROP TABLE #prime_pays_fi
DROP TABLE #prime_pays_hors_fi
DROP TABLE #ptf_fga_vifs
DROP TABLE #ptf_fga
DROP TABLE #RATING_LISTE_FUTURE
DROP TABLE #RATING_LISTE_LIQUIDITES