--
-- PRE-Requis: les codes Proxy et Isin des fonds détenteurs sont des varchar(12)

-- Transparise tous les portefeuilles au niv1:
--   1. Lister les OPCVMS FGA dans la table #OPCVM
--   2. Prendre les lignes de tous les groupes avec le groupe OPCVM: represente la base pour la transparisation
--   3. Lister les proxy 'OPCVM' dans #OPCVM_FGA avec le code ISIN du fonds à ouvrir
--   4. Lister les lignes en positions de Fonds FGA (identifiés dans le groupe #OPCVM) dans la table  #OPCVM_FGA
--   5. Lister les lignes en positions de Fonds Externe (identifiés dans la table PTF_PARAM_PROXY) à mettre dans la table #PROXY
--   6. mettre les lignes restantes (non tranparisables) dans la table #NO_TRANStmp
--      A ce stade , l ensemble des lignes est décomposé dans #OPCVM_FGA (les fonds FGA) , #PROXY (les fonds externes) et #NO_TRANStmp (le reste)
--   7. Ajout d'1 ligne pour 1 ligne des positions non transaprisable dans #NO_TRANS
--   8. Ouverture des lignes en fonds FGA pour passer la transparence dans #Lignes_OPCVM
--   9. Ouverture des lignes en fonds externe pour passer la transparence dans #Lignes_PROXY
--  10. stockage du résultat des 3 phases dans PTF_TRANSPARISE

--/////////////////////////////////////////////
--// TODO : gestion de la trace dans PTF_TRANSPARISE : ajouter des colonnes pour le niveau4
--/////////////////////////////////////////////

--DROP PROCEDURE Trans4
ALTER PROCEDURE Trans4
       @date DATETIME
AS 

--DECLARE @date AS DATETIME
--SET @date='31/03/2015'

DECLARE @niveau AS INT
SET @niveau = 4

--Nettoyage
DELETE FROM [PTF_TRANSPARISE] WHERE dateinventaire = @date AND numero_niveau=@niveau

--1.{Composition OPCVM FGA: les compositions des fonds }
SELECT Groupe,Dateinventaire,Compte,ISIN_Ptf,Libelle_Ptf,code_Titre,isin_titre,Libelle_Titre,Valeur_Boursiere,
	   Coupon_Couru,Valeur_Comptable,Coupon_Couru_Comptable,PMV,PMV_CC,Type_Produit,Devise_Titre,Secteur,Sous_Secteur,
	   Pays,Emetteur,Rating,Grp_Emetteur,maturite,duration,sensibilite,coursclose,quantite,coupon,rendement
INTO #OPCVM FROM PTF_FGA WHERE Groupe = 'OPCVM'  AND Dateinventaire= @date
and Isin_Ptf <> 'FR0010250597'
and Libelle_Ptf <>'FCP FEDERIS TRESORERIE'
-- le fonds Federis tréso n est pas transparisé avec ses swaps ...

--2.{Composition MANDAT FGA = > la base de la transparisation}
SELECT Groupe,Dateinventaire,Compte,ISIN_Ptf,Libelle_Ptf,code_Titre,isin_titre,Libelle_Titre,Valeur_Boursiere,
	   Coupon_Couru,Valeur_Comptable,Coupon_Couru_Comptable,PMV,PMV_CC,Type_Produit,Devise_Titre,Secteur,Sous_Secteur,
	   Pays,Emetteur,Rating,Grp_Emetteur,maturite,duration,sensibilite,coursclose,quantite,coupon,rendement,
	   pct_det_Niv_0,Isin_proxy_Niv_1,Isin_origine_Niv_1,pct_det_Niv_1,
	   Isin_proxy_Niv_2,Isin_origine_Niv_2,pct_det_Niv_2,
	   Isin_proxy_Niv_3,Isin_origine_Niv_3,pct_det_Niv_3
 -- avec le groupe OPCVM	   
 --INTO #MANDAT FROM PTF_TRANSPARISE WHERE Groupe <> 'OPCVM' AND Dateinventaire= @date AND numero_niveau=@niveau-1	   
 INTO #MANDAT FROM PTF_TRANSPARISE WHERE Dateinventaire= @date AND numero_niveau=@niveau-1



--RECUPERE LES DIFFERENTS TYPE A TRANSPARISER
-- calcul du taux de detention au niveau 0, cad: ValeurB+CC / ActifNet de l'OPCVM
-- le fonds FGA est detenu à x% dans un mandat au niveau 1, par ex.
CREATE TABLE #OPCVM_FGA([pct_det_Niv_0] [float] NULL, [Isin_proxy_Niv_1] [varchar](12) COLLATE DATABASE_DEFAULT NULL,
    [Isin_origine_Niv_1] [varchar](12) COLLATE DATABASE_DEFAULT NULL,[pct_det_Niv_1] [float] NULL,[Isin_proxy_Niv_2] [varchar](12) COLLATE DATABASE_DEFAULT NULL,
  	[Isin_origine_Niv_2] [varchar](12) COLLATE DATABASE_DEFAULT NULL,[pct_det_Niv_2] [float] NULL,[Isin_proxy_Niv_3] [varchar](12) COLLATE DATABASE_DEFAULT NULL,
	[Isin_origine_Niv_3] [varchar](12) COLLATE DATABASE_DEFAULT NULL,[pct_det_Niv_3] [float] NULL,[Isin_proxy_Niv_4] [varchar](12) COLLATE DATABASE_DEFAULT NULL,
	[Groupe] [varchar](25) COLLATE DATABASE_DEFAULT NULL,[Dateinventaire] [datetime] NOT NULL,
	[Compte] [varchar](60) COLLATE DATABASE_DEFAULT NOT NULL,[ISIN_Ptf] [varchar](12) COLLATE DATABASE_DEFAULT NULL,[Libelle_Ptf] [varchar](60) COLLATE DATABASE_DEFAULT NULL,
	[code_Titre] [varchar](15) COLLATE DATABASE_DEFAULT NOT NULL,[isin_titre] [varchar](15) COLLATE DATABASE_DEFAULT NULL,[Libelle_Titre] [varchar](60) COLLATE DATABASE_DEFAULT NULL,
	[Valeur_Boursiere] [float] NULL,[Coupon_Couru] [float] NULL,[Valeur_Comptable] [float] NULL,
	[Coupon_Couru_Comptable] [float] NULL,[PMV] [float] NULL,[PMV_CC] [float] NULL,[Type_Produit] [varchar](60) COLLATE DATABASE_DEFAULT NULL,
	[Devise_Titre] [varchar](3) COLLATE DATABASE_DEFAULT NULL,[Secteur] [varchar](60) COLLATE DATABASE_DEFAULT NULL,[Sous_Secteur] [varchar](60) COLLATE DATABASE_DEFAULT NULL,
	[Pays] [varchar](60) COLLATE DATABASE_DEFAULT NULL,[Emetteur] [varchar](60) COLLATE DATABASE_DEFAULT NULL,[Rating] [varchar](4) COLLATE DATABASE_DEFAULT NULL,
	[Grp_Emetteur] [varchar](60) COLLATE DATABASE_DEFAULT NULL,[maturite] [datetime] NULL,[duration] [float] NULL,
	[sensibilite] [float] NULL,[coursclose] [float] NULL,[quantite] [float] NULL,	
	[coupon] [float] NULL,[rendement] [float] NULL)

--3.{Lignes transparisables Proxy type.OPCVM}
-- on remplace les codes des titres avec le code du proxy
INSERT INTO #OPCVM_FGA
SELECT ma.pct_det_Niv_0,ma.Isin_proxy_Niv_1,ma.Isin_origine_Niv_1,ma.pct_det_Niv_1,
       ma.Isin_proxy_Niv_2,ma.Isin_origine_Niv_2,ma.pct_det_Niv_2,
	   ma.Isin_proxy_Niv_3,ma.Isin_origine_Niv_3,ma.pct_det_Niv_3,
       pp.isin_titre AS 'Isin_proxy_Niv_4',--le code du fond à la source       
	   Groupe,Dateinventaire,ma.Compte,ma.ISIN_Ptf,ma.Libelle_Ptf,
	   pp.code_proxy as 'code_Titre', pp.code_proxy as 'isin_titre',  -- on utilise le proxy qui est un OPCVM FGA dont l inventaire est disponible sur #OPCVM
	   'Proxy de ' + Libelle_Titre as 'Libelle_Titre', -- le libelle est inutile
	   Valeur_Boursiere,Coupon_Couru,Valeur_Comptable,Coupon_Couru_Comptable,PMV,PMV_CC,Type_Produit,Devise_Titre,Secteur,Sous_Secteur,
	   Pays,Emetteur,Rating,Grp_Emetteur,maturite,duration,ma.sensibilite,coursclose,quantite,ma.coupon,ma.rendement   
FROM #MANDAT ma ,PTF_PARAM_PROXY pp 
where pp.Source = 'OPCVM' AND  pp.Isin_Titre = ma.isin_titre and pp.date= @date

--4.{Lignes transparisables OPCVM_FGA}
INSERT INTO #OPCVM_FGA
SELECT ma.pct_det_Niv_0,ma.Isin_proxy_Niv_1,ma.Isin_origine_Niv_1,ma.pct_det_Niv_1,
       ma.Isin_proxy_Niv_2,ma.Isin_origine_Niv_2,ma.pct_det_Niv_2,
	   ma.Isin_proxy_Niv_3,ma.Isin_origine_Niv_3,ma.pct_det_Niv_3,
	   NULL AS 'Isin_proxy_Niv_4',
	   Groupe,Dateinventaire,ma.Compte,ma.ISIN_Ptf,ma.Libelle_Ptf,
	   code_Titre,isin_titre,Libelle_Titre,Valeur_Boursiere,
	   Coupon_Couru,Valeur_Comptable,Coupon_Couru_Comptable,PMV,PMV_CC,Type_Produit,Devise_Titre,Secteur,Sous_Secteur,
	   Pays,Emetteur,Rating,Grp_Emetteur,maturite,duration,ma.sensibilite,coursclose,quantite,ma.coupon,ma.rendement
FROM  #MANDAT ma 
WHERE ma.Isin_Titre in (SELECT DISTINCT isin_ptf FROM #OPCVM)

--5.{Lignes transparisable Proxy type.PROXY : fonds externes}
SELECT  ma.pct_det_Niv_0,ma.Isin_proxy_Niv_1,ma.Isin_origine_Niv_1,ma.pct_det_Niv_1,
       ma.Isin_proxy_Niv_2,ma.Isin_origine_Niv_2,ma.pct_det_Niv_2,
	   ma.Isin_proxy_Niv_3,ma.Isin_origine_Niv_3,ma.pct_det_Niv_3,
	   pp.code_proxy AS 'Isin_proxy_Niv_4',
	   Groupe,Dateinventaire,ma.Compte,ma.ISIN_Ptf,ma.Libelle_Ptf,
	   code_Titre,ma.isin_titre,Libelle_Titre,Valeur_Boursiere,
	   Coupon_Couru,Valeur_Comptable,Coupon_Couru_Comptable,PMV,PMV_CC,Type_Produit,Devise_Titre,Secteur,Sous_Secteur,
	   Pays,Emetteur,Rating,Grp_Emetteur,maturite,duration,ma.sensibilite,coursclose,quantite,ma.coupon,ma.rendement  
INTO #PROXY 
FROM #MANDAT ma, PTF_PARAM_PROXY pp
where pp.Source = 'PROXY' AND  pp.Isin_Titre = ma.isin_titre AND pp.date= @date

--6.{titres non transparisables}
SELECT *
INTO #NO_TRANStmp 
FROM #MANDAT
WHERE Isin_Titre not in 
(SELECT distinct Isin_Titre FROM #OPCVM_FGA
 UNION 
 SELECT distinct Isin_Titre FROM #PROXY
 UNION
 select distinct Isin_Titre from PTF_PARAM_PROXY p where p.Source = 'OPCVM' AND p.date= @date 
 )

CREATE TABLE #NO_TRANS(	[Groupe] [varchar](25) NULL,[Dateinventaire] [datetime] NOT NULL,
	[Compte] [varchar](60) NOT NULL,[ISIN_Ptf] [varchar](12) NULL,	[Libelle_Ptf] [varchar](60) NULL,
	[code_Titre] [varchar](15) NOT NULL,[isin_titre] [varchar](15) NULL,[Libelle_Titre] [varchar](60) NULL,
	[Valeur_Boursiere] [float] NULL,[Coupon_Couru] [float] NULL,[Valeur_Comptable] [float] NULL,
	[Coupon_Couru_Comptable] [float] NULL,[PMV] [float] NULL,[PMV_CC] [float] NULL,[Type_Produit] [varchar](60) NULL,
	[Devise_Titre] [varchar](3) NULL,[Secteur] [varchar](60) NULL,[Sous_Secteur] [varchar](60) NULL,
	[Pays] [varchar](60) NULL,[Emetteur] [varchar](60) NULL,[Rating] [varchar](4) NULL,
	[Grp_Emetteur] [varchar](60) NULL,[maturite] [datetime] NULL,[duration] [float] NULL,
	[sensibilite] [float] NULL,[coursclose] [float] NULL,[quantite] [float] NULL,
	[pct_det_Niv_0] [float] NULL, 
	[Isin_origine_Niv_1] [varchar](12) NULL,[pct_det_Niv_1] [float] NULL,[Isin_proxy_Niv_1] [varchar](12) NULL,
	[Isin_origine_Niv_2] [varchar](12) NULL,[pct_det_Niv_2] [float] NULL,[Isin_proxy_Niv_2] [varchar](12) NULL,
	[Isin_origine_Niv_3] [varchar](12) NULL,[pct_det_Niv_3] [float] NULL,[Isin_proxy_Niv_3] [varchar](12) NULL,
	[Isin_origine_Niv_4] [varchar](12) NULL,[pct_det_Niv_4] [float] NULL,[Isin_proxy_Niv_4] [varchar](12) NULL,
	[Numero_Niveau] [int] NOT NULL,
	[Zone_Géo] [varchar](60) NULL,[Type_actif] [varchar](60) NULL,[Type_de_dette] [varchar](60) NULL,
	[Groupe_rating] [varchar](4) NULL,[col1] [varchar](30) NULL,[col2] [varchar](30) NULL,[Vie_residuelle] [float] NULL,
	[Tranche_de_maturite] [varchar](15) NULL,[coupon] [float] NULL,[rendement] [float] NULL)

--7. TITRES DETENUS EN DIRECT ou NON TRANSPARISABLE---------------------------------------------------------------------------------------------------------
insert into #NO_TRANS -- niveau4
SELECT ntt.Groupe,ntt.Dateinventaire,ntt.Compte,ntt.ISIN_Ptf,ntt.Libelle_Ptf,ntt.code_Titre,ntt.isin_titre,ntt.Libelle_Titre,ntt.Valeur_Boursiere,
	   ntt.Coupon_Couru,ntt.Valeur_Comptable,ntt.Coupon_Couru_Comptable,ntt.PMV,ntt.PMV_CC,ntt.Type_Produit,ntt.Devise_Titre,ntt.Secteur,ntt.Sous_Secteur,
	   ntt.Pays,ntt.Emetteur,ntt.Rating,ntt.Grp_Emetteur,ntt.maturite,ntt.duration,ntt.sensibilite,ntt.coursclose,ntt.quantite,
	   ntt.pct_det_Niv_0,ntt.Isin_origine_Niv_1,ntt.pct_det_Niv_1,ntt.Isin_proxy_Niv_1,
	   ntt.Isin_origine_Niv_2,ntt.pct_det_Niv_2,ntt.Isin_proxy_Niv_2,	   
	   ntt.Isin_origine_Niv_3,
	   CASE WHEN ntt.code_Titre='Cash Mandat' THEN 1 ELSE ntt.pct_det_Niv_3 END AS 'pct_det_Niv_3',
	   ntt.Isin_proxy_Niv_3,
	   '' AS 'Isin_origine_Niv_4', NULL AS 'pct_det_Niv_4',NULL AS 'Isin_proxy_Niv_4',
	   4 AS Numero_Niveau,
	   CASE WHEN co.Pays IS NULL THEN zg.Zone ELSE co.Pays END AS 'Zone_Géo',
	   CASE WHEN co.Types IS NOT NULL AND co.Types <> 'AS_PTF' THEN co.Types -- transco pour OPCVM
			WHEN ta.Types IS NOT NULL AND ta.Types <> 'AS_PTF' THEN ta.Types -- transco pour les actifs sauf cash et monétaire
			WHEN mandat.type_mandat = 'A' THEN 'Actions'
			WHEN mandat.type_mandat = 'O' THEN 'Obligations'
			WHEN mandat.type_mandat = 'D' THEN 'Mixte'
	    ELSE 'OPCVM' END AS 'Type_Actif',
	   tdd.types AS Type_de_dette,Replace(Replace(ntt.Rating,'+',''),'-','') AS Groupe_rating,' ' AS col1,' ' AS col2,CASE WHEN co.Types LIKE '%Obligation' OR ta.types LIKE '%Obligation%' THEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 ELSE NULL END AS 'Vie_residuelle', 	   
	   --CASE WHEN ntt.maturite IS NULL OR co.Types NOT LIKE '%Obligation' OR ta.types NOT LIKE '%Obligation%' THEN NULL ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)<=0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END AS Tranche_de_maturite,	   
	   CASE WHEN ntt.maturite IS NULL OR co.Types NOT LIKE '%Obligation' OR ta.types NOT LIKE '%Obligation%' THEN NULL ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)<0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 1 THEN '0-1 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END END AS Tranche_de_maturite,
	   ntt.coupon,ntt.rendement
FROM #NO_TRANStmp ntt -- niveau 3
	LEFT OUTER JOIN ZONE_GEOGRAPHIQUE zg ON ntt.Pays = zg.Pays 
	LEFT OUTER JOIN PTF_TYPE_ACTIF ta ON ntt.Type_produit = ta.produit -- Si la ligne est un actif, utiliser la correspondance type_produit type_actif de cette table
	LEFT OUTER JOIN PTF_CARAC_OPCVM co ON co.Libelle = ntt.Sous_secteur -- Si la ligne est un OPCVM, utiliser le type actif de cette table	
	LEFT OUTER JOIN ref.PORTFOLIO mandat ON mandat.Compte = ntt.Compte -- Pour récupérer le type du mandat
	LEFT OUTER JOIN PTF_TYPE_DE_DETTE tdd ON ntt.Secteur = tdd.libelle
-- controle rapide:
--select SUM(Valeur_Boursiere + Coupon_Couru) from #NO_TRANS
--select SUM(Valeur_Boursiere + Coupon_Couru) from #NO_TRANStmp 

--8. TRANSPARISE les lignes en fonds FGA-------------------------------------------------------------------------------------------------------------
-- eclatement de chaque ligne en fonds FGA en utilisant les inventaires #OPCVM 
SELECT po.Groupe,po.Dateinventaire,po.Compte,po.ISIN_Ptf,po.Libelle_Ptf,
	   fga.code_Titre,fga.isin_titre,fga.Libelle_Titre,
	   po.pct_det_Niv_3 * fga.Valeur_Boursiere As 'Valeur_Boursiere',
	   po.pct_det_Niv_3 * fga.Coupon_Couru AS 'Coupon_Couru',
	   po.pct_det_Niv_3 * fga.Valeur_Comptable AS 'Valeur_Comptable',
	   po.pct_det_Niv_3 * fga.Coupon_Couru_Comptable AS 'Coupon_Couru_Comptable',
	   po.pct_det_Niv_3 * fga.PMV AS 'PMV',
	   po.pct_det_Niv_3 * fga.PMV_CC AS 'PMV_CC',
	   fga.Type_Produit,fga.Devise_Titre,fga.Secteur,fga.Sous_Secteur,
	   fga.Pays,fga.Emetteur,fga.Rating,fga.Grp_Emetteur,fga.maturite,fga.duration,fga.sensibilite,fga.coursclose,
	   po.pct_det_Niv_3 * fga.quantite AS quantite,
	   po.pct_det_Niv_0 AS 'pct_det_Niv_0',
	   po.Isin_origine_Niv_1 AS 'Isin_origine_Niv_1', po.pct_det_Niv_1 AS 'pct_det_Niv_1', Isin_proxy_Niv_1 AS 'Isin_proxy_Niv_1',
	   po.Isin_origine_Niv_2 AS 'Isin_origine_Niv_2', po.pct_det_Niv_2 AS 'pct_det_Niv_2', Isin_proxy_Niv_2 AS 'Isin_proxy_Niv_2',
	   po.Isin_origine_Niv_3, po.pct_det_Niv_3, Isin_proxy_Niv_3,
	   isnull(po.Isin_proxy_Niv_4,po.isin_titre) AS 'Isin_origine_Niv_4',  -- isin du fonds source (si proxy.OPCVM, c est le fonds initial)
	   CASE WHEN an.AN > 0 THEN po.pct_det_Niv_3 * ( fga.Valeur_Boursiere + fga.Coupon_Couru) / an.AN-- poids du fonds Source dans le mandat si c'est un fond
					  ELSE NULL END AS 'pct_det_Niv_4',  -- si ce n'est pas un fonds FGA : vide
       CASE WHEN po.Isin_proxy_Niv_4 is not null THEN po.isin_titre
                                                 ELSE NULL END AS 'Isin_proxy_Niv_4',   -- isin du fonds utilisé en proxy (OPCVM FGA)	   
	   4 AS 'Numero_Niveau',
	   CASE WHEN co.Pays IS NULL THEN zg.Zone ELSE co.Pays END AS 'Zone_Géo',
	   CASE WHEN co.Types IS NOT NULL AND co.Types <> 'AS_PTF' THEN co.Types -- transco pour OPCVM
			WHEN ta.Types IS NOT NULL AND ta.Types <> 'AS_PTF' THEN ta.Types -- transco pour les actifs sauf cash et monétaire
			WHEN mandat.type_mandat = 'A' THEN 'Actions'
			WHEN mandat.type_mandat = 'O' THEN 'Obligations'
			WHEN mandat.type_mandat = 'D' THEN 'Mixte'
	    ELSE 'OPCVM' END AS 'Type_Actif',
	   tdd.types AS Type_de_dette,Replace(Replace(fga.Rating,'+',''),'-','') AS Groupe_rating,' ' AS col1,' ' AS col2,CASE WHEN co.Types LIKE '%Obligation' OR ta.types LIKE '%Obligation%' THEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 ELSE NULL END AS 'Vie_residuelle', 	   
	   --CASE WHEN fga.maturite IS NULL OR co.Types NOT LIKE '%Obligation' OR ta.types NOT LIKE '%Obligation%' THEN NULL ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)<=0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END AS Tranche_de_maturite, 	  	   
	   CASE WHEN fga.maturite IS NULL OR co.Types NOT LIKE '%Obligation' OR ta.types NOT LIKE '%Obligation%' THEN NULL ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)<0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 1 THEN '0-1 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,fga.dateinventaire,fga.maturite)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END END AS Tranche_de_maturite,
	   fga.coupon,fga.rendement
INTO #Lignes_OPCVM  -- niveau 4
FROM  #OPCVM_FGA po -- niveau 3
	LEFT OUTER JOIN #OPCVM fga ON fga.ISIN_Ptf = po.isin_Titre  AND fga.Dateinventaire= @date
	LEFT OUTER JOIN PTF_AN an ON an.isin_Ptf = fga.isin_titre AND an.date= @date -- Pour récupérer l ActifNet si l'isin est un fonds 
	LEFT OUTER JOIN ZONE_GEOGRAPHIQUE zg ON fga.Pays = zg.Pays
	LEFT OUTER JOIN PTF_TYPE_ACTIF ta ON fga.Type_produit = ta.produit
	LEFT OUTER JOIN PTF_CARAC_OPCVM co ON co.Libelle = fga.Sous_secteur
	LEFT OUTER JOIN ref.PORTFOLIO mandat ON mandat.Compte = po.Compte -- Pour récupérer le type du mandat	
	LEFT OUTER JOIN PTF_TYPE_DE_DETTE tdd ON fga.Secteur = tdd.libelle 
-- controle rapide
--select SUM(Valeur_Boursiere + Coupon_Couru) from  #Lignes_OPCVM
--select SUM(Valeur_Boursiere + Coupon_Couru) from #OPCVM_FGA 

--9. TRANSPARISE PROXY.PROXY-------------------------------------------------------------------------------------------------------------
SELECT  po.Groupe,po.DateInventaire,po.Compte,po.ISIN_Ptf,po.Libelle_Ptf,
CASE WHEN p.code_Titre = 'Liquidité(OPCVM)'  THEN 'Cash OPCVM' ELSE p.code_Titre END AS 'code_Titre',
CASE WHEN p.code_Titre = 'Liquidité(OPCVM)'  THEN 'Cash OPCVM' ELSE p.code_Titre END AS 'isin_titre',p.Libelle_Titre,
		p.Poids_VB*po.Valeur_Boursiere As 'Valeur_Boursiere',
		p.Poids_CC*po.Coupon_couru AS 'Coupon_Couru',
		p.Poids_VB*po.Valeur_Comptable AS 'Valeur_Comptable',
		Poids_CC*po.Coupon_couru_Comptable AS 'Coupon_Couru_Comptable',
		Poids_VB*po.PMV AS 'PMV',
		Poids_CC*po.PMV AS 'PMV_CC',
		p.Type_Produit,p.Devise_Titre,p.Secteur,p.Sous_Secteur,
		p.Pays,p.Emetteur,p.Rating,p.Groupe_Emet,p.maturité,p.duration,p.sensibilite,
		NULL AS 'coursclose', Poids_VB*po.quantite AS 'quantite', --approximation des quantités détenues
		po.pct_det_Niv_0 AS 'pct_det_Niv_0',
	    po.Isin_origine_Niv_1 AS 'Isin_origine_Niv_1', po.pct_det_Niv_1 AS 'pct_det_Niv_1', po.Isin_proxy_Niv_1 AS 'Isin_proxy_Niv_1',		   
	    po.Isin_origine_Niv_2, po.pct_det_Niv_2, po.Isin_proxy_Niv_2,
		po.Isin_origine_Niv_3, po.pct_det_Niv_3, po.Isin_proxy_Niv_3, 
		left(po.isin_titre,12) AS 'Isin_origine_Niv_4', -- isin du fonds source
		CASE WHEN an.AN > 0 THEN (p.Poids_VB*po.Valeur_Boursiere + p.Poids_CC*po.Coupon_couru) / an.AN -- poids du fonds Source dans le mandat si c'est un fond
		ELSE NULL END AS 'pct_det_Niv_4',  -- si ce n'est pas un fonds FGA : vide        
		left(po.Isin_proxy_Niv_4,12) AS 'Isin_proxy_Niv_4',   -- code du proxy utilisé
		4 AS 'Numero_Niveau',
		CASE WHEN co.Pays IS NULL THEN zg.Zone ELSE co.Pays END AS 'Zone_Géo',
	   CASE WHEN co.Types IS NOT NULL AND co.Types <> 'AS_PTF' THEN co.Types -- transco pour OPCVM
			WHEN ta.Types IS NOT NULL AND ta.Types <> 'AS_PTF' THEN ta.Types -- transco pour les actifs sauf cash et monétaire
			WHEN mandat.type_mandat = 'A' THEN 'Actions'
			WHEN mandat.type_mandat = 'O' THEN 'Obligations'
			WHEN mandat.type_mandat = 'D' THEN 'Mixte'
	    ELSE 'OPCVM' END AS 'Type_Actif',
		tdd.types AS Type_de_dette,Replace(Replace(p.Rating,'+',''),'-','') AS Groupe_rating,' ' AS col1,' ' AS col2,CASE WHEN co.Types LIKE '%Obligation' OR ta.types LIKE '%Obligation%' THEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 ELSE NULL END AS 'Vie_residuelle', 
		--CASE WHEN p.maturité IS NULL OR co.Types NOT LIKE '%Obligation' OR ta.types NOT LIKE '%Obligation%' THEN NULL ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)<=0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END AS 'Tranche_de_maturite', 
		CASE WHEN p.maturité IS NULL OR co.Types NOT LIKE '%Obligation' OR ta.types NOT LIKE '%Obligation%' THEN NULL ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)<0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 1 THEN '0-1 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,po.dateinventaire,p.maturité)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END END AS 'Tranche_de_maturite', 
		0 AS coupon,0 AS rendement
INTO #Lignes_PROXY --Niveau4
FROM  #PROXY po  -- niveau 3
	LEFT OUTER JOIN PTF_PROXY p ON p.code_proxy = po.Isin_proxy_Niv_4 AND p.Date= @date
	LEFT OUTER JOIN PTF_AN an ON an.isin_Ptf = p.code_titre AND an.date= @date -- Pour récupérer l ActifNet si l'isin est un fonds FGA	(rare pour un proxy)
	LEFT OUTER JOIN ZONE_GEOGRAPHIQUE zg ON p.Pays = zg.Pays
	LEFT OUTER JOIN PTF_TYPE_ACTIF ta ON p.Type_produit = ta.produit
	LEFT OUTER JOIN PTF_CARAC_OPCVM co ON co.Libelle = p.Sous_secteur
	LEFT OUTER JOIN ref.PORTFOLIO mandat ON mandat.Compte = po.Compte -- Pour récupérer le type du mandat		
	LEFT OUTER JOIN PTF_TYPE_DE_DETTE tdd ON p.Secteur = tdd.libelle 
---- controle rapide
--select SUM(Valeur_Boursiere + Coupon_Couru) from  #Lignes_PROXY
--select SUM(Valeur_Boursiere + Coupon_Couru) from #PROXY 

--10. Mise à jour de la table des positions en transparisées 
INSERT INTO [PTF_TRANSPARISE]
           ([Groupe],[Dateinventaire],[Compte],[ISIN_Ptf],[Libelle_Ptf],[code_Titre],[isin_titre]
           ,[Libelle_Titre],[Valeur_Boursiere],[Coupon_Couru],[Valeur_Comptable],[Coupon_Couru_Comptable]
           ,[PMV],[PMV_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur]
           ,[Pays],[Emetteur],[Rating],[Grp_Emetteur],[maturite],[duration]
           ,[sensibilite],[coursclose],[quantite],[pct_det_Niv_0],[Isin_origine_Niv_1],[pct_det_Niv_1]
           ,[Isin_proxy_Niv_1],[Isin_origine_Niv_2],[pct_det_Niv_2],[Isin_proxy_Niv_2]
           ,[Isin_origine_Niv_3],[pct_det_Niv_3],[Isin_proxy_Niv_3],[Numero_Niveau],[Zone_Géo],[Type_actif],[Type_de_dette]
           ,[Groupe_rating],[col1],[col2],[Vie_residuelle],[Tranche_de_maturite]
           ,[coupon],[rendement])          
SELECT [Groupe],[Dateinventaire],[Compte],[ISIN_Ptf],[Libelle_Ptf],[code_Titre],[isin_titre]
           ,[Libelle_Titre],[Valeur_Boursiere],[Coupon_Couru],[Valeur_Comptable],[Coupon_Couru_Comptable]
           ,[PMV],[PMV_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur]
           ,[Pays],[Emetteur],[Rating],[Grp_Emetteur],[maturite],[duration]
           ,[sensibilite],[coursclose],[quantite],[pct_det_Niv_0],[Isin_origine_Niv_1],[pct_det_Niv_1]
           ,[Isin_proxy_Niv_1],[Isin_origine_Niv_2],[pct_det_Niv_2],[Isin_proxy_Niv_2]
           ,[Isin_origine_Niv_3],[pct_det_Niv_3],[Isin_proxy_Niv_3],[Numero_Niveau],[Zone_Géo],[Type_actif],[Type_de_dette]
           ,[Groupe_rating],[col1],[col2],[Vie_residuelle],[Tranche_de_maturite]
           ,[coupon],[rendement] FROM #NO_TRANS

INSERT INTO [PTF_TRANSPARISE]
           ([Groupe],[Dateinventaire],[Compte],[ISIN_Ptf],[Libelle_Ptf],[code_Titre],[isin_titre]
           ,[Libelle_Titre],[Valeur_Boursiere],[Coupon_Couru],[Valeur_Comptable],[Coupon_Couru_Comptable]
           ,[PMV],[PMV_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur]
           ,[Pays],[Emetteur],[Rating],[Grp_Emetteur],[maturite],[duration]
           ,[sensibilite],[coursclose],[quantite],[pct_det_Niv_0],[Isin_origine_Niv_1],[pct_det_Niv_1]
           ,[Isin_proxy_Niv_1],[Isin_origine_Niv_2],[pct_det_Niv_2],[Isin_proxy_Niv_2]
           ,[Isin_origine_Niv_3],[pct_det_Niv_3],[Isin_proxy_Niv_3],[Numero_Niveau],[Zone_Géo],[Type_actif],[Type_de_dette]
           ,[Groupe_rating],[col1],[col2],[Vie_residuelle],[Tranche_de_maturite]
           ,[coupon],[rendement])  
SELECT [Groupe],[Dateinventaire],[Compte],[ISIN_Ptf],[Libelle_Ptf],[code_Titre],[isin_titre]
           ,[Libelle_Titre],[Valeur_Boursiere],[Coupon_Couru],[Valeur_Comptable],[Coupon_Couru_Comptable]
           ,[PMV],[PMV_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur]
           ,[Pays],[Emetteur],[Rating],[Grp_Emetteur],[maturite],[duration]
           ,[sensibilite],[coursclose],[quantite],[pct_det_Niv_0],[Isin_origine_Niv_1],[pct_det_Niv_1]
           ,[Isin_proxy_Niv_1],[Isin_origine_Niv_2],[pct_det_Niv_2],[Isin_proxy_Niv_2]
           ,[Isin_origine_Niv_3],[pct_det_Niv_3],[Isin_proxy_Niv_3],[Numero_Niveau],[Zone_Géo],[Type_actif],[Type_de_dette]
           ,[Groupe_rating],[col1],[col2],[Vie_residuelle],[Tranche_de_maturite]
           ,[coupon],[rendement] FROM #Lignes_OPCVM

INSERT INTO [PTF_TRANSPARISE]
           ([Groupe],[Dateinventaire],[Compte],[ISIN_Ptf],[Libelle_Ptf],[code_Titre],[isin_titre]
           ,[Libelle_Titre],[Valeur_Boursiere],[Coupon_Couru],[Valeur_Comptable],[Coupon_Couru_Comptable]
           ,[PMV],[PMV_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur]
           ,[Pays],[Emetteur],[Rating],[Grp_Emetteur],[maturite],[duration]
           ,[sensibilite],[coursclose],[quantite],[pct_det_Niv_0],[Isin_origine_Niv_1],[pct_det_Niv_1]
           ,[Isin_proxy_Niv_1],[Isin_origine_Niv_2],[pct_det_Niv_2],[Isin_proxy_Niv_2]
           ,[Isin_origine_Niv_3],[pct_det_Niv_3],[Isin_proxy_Niv_3],[Numero_Niveau],[Zone_Géo],[Type_actif],[Type_de_dette]
           ,[Groupe_rating],[col1],[col2],[Vie_residuelle],[Tranche_de_maturite]
           ,[coupon],[rendement])  
SELECT [Groupe],[Dateinventaire],[Compte],[ISIN_Ptf],[Libelle_Ptf],[code_Titre],[isin_titre]
           ,[Libelle_Titre],[Valeur_Boursiere],[Coupon_Couru],[Valeur_Comptable],[Coupon_Couru_Comptable]
           ,[PMV],[PMV_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur]
           ,[Pays],[Emetteur],[Rating],[Groupe_Emet],[maturité],[duration]
           ,[sensibilite],[coursclose],[quantite],[pct_det_Niv_0]
           ,[Isin_origine_Niv_1],[pct_det_Niv_1],[Isin_proxy_Niv_1]
           ,[Isin_origine_Niv_2],[pct_det_Niv_2],[Isin_proxy_Niv_2]
           --           ,[Isin_origine_Niv_3],[pct_det_Niv_3],[Isin_proxy_Niv_3]
           --,case 
           --when [Isin_origine_Niv_3] ='' then 'N4'+SUBSTRING([Isin_origine_Niv_4],3,12)
           --when [Isin_origine_Niv_4] ='' then 'N3'+SUBSTRING([Isin_origine_Niv_3],3,12)
           --else SUBSTRING([Isin_origine_Niv_3],1,6)+SUBSTRING([Isin_origine_Niv_4],1,6) end as [Isin_origine_Niv_3],
           ,ROW_NUMBER ( ) OVER ( ORDER BY Isin_origine_Niv_3,Isin_origine_Niv_4) as [Isin_origine_Niv_3],
           --,SUBSTRING([Isin_origine_Niv_3],6,6)+SUBSTRING([Isin_origine_Niv_4],6,6) 
           [pct_det_Niv_4] as [pct_det_Niv_3],
           [Isin_proxy_Niv_4] as [Isin_proxy_Niv_3]
           ,[Numero_Niveau],[Zone_Géo],[Type_actif],[Type_de_dette]
           ,[Groupe_rating],[col1],[col2],[Vie_residuelle],[Tranche_de_maturite]
           ,[coupon],[rendement] FROM #Lignes_PROXY


DROP TABLE #OPCVM
DROP TABLE #MANDAT
DROP TABLE #OPCVM_FGA
DROP TABLE #PROXY
DROP TABLE #NO_TRANStmp
DROP TABLE #NO_TRANS
DROP TABLE #Lignes_OPCVM
DROP TABLE #Lignes_PROXY
