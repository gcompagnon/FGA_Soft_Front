
-- NIV 0: Copie simplement les lignes de la table PTF_FGA dans PTF_TRANSPARISE

--DROP PROCEDURE Trans0
CREATE PROCEDURE Trans0
       @date DATETIME
AS 

--DECLARE @date AS DATETIME
--SET @date='31/07/2012'

DECLARE @niveau AS INT
SET @niveau = 0

--Nettoyage
DELETE FROM PTF_TRANSPARISE WHERE dateinventaire = @date AND numero_niveau=@niveau

--1.{Composition MANDAT FGA = > la base de la transparisation}
SELECT Groupe,Dateinventaire,Compte,ISIN_Ptf,Libelle_Ptf,code_Titre,isin_titre,Libelle_Titre,Valeur_Boursiere,
	   Coupon_Couru,Valeur_Comptable,Coupon_Couru_Comptable,PMV,PMV_CC,Type_Produit,Devise_Titre,Secteur,Sous_Secteur,
	   Pays,Emetteur,Rating,Grp_Emetteur,maturite,duration,sensibilite,coursclose,quantite,coupon,rendement
	   
-- avec le groupe OPCVM	   
--INTO #MANDAT FROM PTF_FGA WHERE Groupe <> 'OPCVM' AND Dateinventaire= @date 
INTO #MANDAT FROM PTF_FGA WHERE Dateinventaire= @date 



--2.{titres non transparisables}
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
	[Numero_Niveau] [int] NOT NULL,
	[Zone_Géo] [varchar](60) NULL,[Type_actif] [varchar](60) NULL,[Type_de_dette] [varchar](60) NULL,
	[Groupe_rating] [varchar](4) NULL,[col1] [varchar](30) NULL,[col2] [varchar](30) NULL,[Vie_residuelle] [float] NULL,
	[Tranche_de_maturite] [varchar](15) NULL,[coupon] [float] NULL,[rendement] [float] NULL)


--7. TITRES DETENUS EN DIRECT ou NON TRANSPARISABLE---------------------------------------------------------------------------------------------------------
insert into #NO_TRANS -- niveau1
SELECT ntt.Groupe,ntt.Dateinventaire,ntt.Compte,ntt.ISIN_Ptf,ntt.Libelle_Ptf,ntt.code_Titre,ntt.isin_titre,ntt.Libelle_Titre,ntt.Valeur_Boursiere,
	   ntt.Coupon_Couru,ntt.Valeur_Comptable,ntt.Coupon_Couru_Comptable,ntt.PMV,ntt.PMV_CC,ntt.Type_Produit,ntt.Devise_Titre,ntt.Secteur,ntt.Sous_Secteur,
	   ntt.Pays,ntt.Emetteur,ntt.Rating,ntt.Grp_Emetteur,ntt.maturite,ntt.duration,ntt.sensibilite,ntt.coursclose,ntt.quantite,
	   CASE WHEN ntt.code_Titre='Cash Mandat' THEN 1 ELSE NULL END AS 'pct_det_Niv_0',
	   '' AS 'Isin_origine_Niv_1', NULL AS 'pct_det_Niv_1',NULL AS 'Isin_proxy_Niv_1',
	   '' AS 'Isin_origine_Niv_2', NULL AS 'pct_det_Niv_2',NULL AS 'Isin_proxy_Niv_2',
	   '' AS 'Isin_origine_Niv_3', NULL AS 'pct_det_Niv_3',NULL AS 'Isin_proxy_Niv_3',	   
	   @niveau AS Numero_Niveau,
	   CASE WHEN co.Pays IS NULL THEN zg.Zone ELSE co.Pays END AS 'Zone_Géo', CASE WHEN co.Types IS NULL THEN ta.Types ELSE co.Types END AS 'Type_Actif', tdd.types AS Type_de_dette,Replace(Replace(ntt.Rating,'+',''),'-','') AS Groupe_rating,' ' AS col1,' ' AS col2,CASE WHEN co.Types LIKE '%Obligation' OR ta.types LIKE '%Obligation%' THEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 ELSE NULL END AS 'Vie_residuelle', 
	   --CASE WHEN ntt.maturite IS NULL OR co.Types NOT LIKE '%Obligation' OR ta.types NOT LIKE '%Obligation%' THEN NULL ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)<=0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END AS Tranche_de_maturite,
	   CASE WHEN ntt.maturite IS NULL OR co.Types NOT LIKE '%Obligation' OR ta.types NOT LIKE '%Obligation%' THEN NULL ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)<0 THEN 'Non disponible' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 1 THEN '0-1 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 3 THEN '1-3 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 5 THEN '3-5 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 7 THEN '5-7 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 10 THEN '7-10 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 15 THEN '10-15 ans' ELSE CASE WHEN DATEDIFF(Day,ntt.dateinventaire,ntt.maturite)/365.25 < 20 THEN '15-20 ans' ELSE '+ 20ans' END END END END END END END END END AS Tranche_de_maturite,
	   ntt.coupon,ntt.rendement
FROM #MANDAT ntt -- niveau 0
	LEFT OUTER JOIN ZONE_GEOGRAPHIQUE zg ON ntt.Pays = zg.Pays 
	LEFT OUTER JOIN PTF_TYPE_ACTIF ta ON ntt.Type_produit = ta.produit
	LEFT OUTER JOIN PTF_CARAC_OPCVM co ON co.Libelle = ntt.Sous_secteur	
	LEFT OUTER JOIN PTF_TYPE_DE_DETTE tdd ON ntt.Secteur = tdd.libelle
-- controle rapide:
--select SUM(Valeur_Boursiere + Coupon_Couru) from #NO_TRANS
--select SUM(Valeur_Boursiere + Coupon_Couru) from #NO_TRANStmp 


--10. Mise à jour de la table des positions en transparisées 
INSERT INTO [PTF_TRANSPARISE]
           ([Groupe],[Dateinventaire],[Compte],[ISIN_Ptf],[Libelle_Ptf],[code_Titre],[isin_titre]
           ,[Libelle_Titre],[Valeur_Boursiere],[Coupon_Couru],[Valeur_Comptable],[Coupon_Couru_Comptable]
           ,[PMV],[PMV_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur]
           ,[Pays],[Emetteur],[Rating],[Grp_Emetteur],[maturite],[duration]
           ,[sensibilite],[coursclose],[quantite],[pct_det_Niv_0],[Isin_origine_Niv_1],[pct_det_Niv_1]
           ,[Isin_proxy_Niv_1],[Isin_origine_Niv_2],[pct_det_Niv_2],[Isin_proxy_Niv_2]
           ,[Isin_origine_Niv_3],[pct_det_Niv_3],[Isin_proxy_Niv_3]
           ,[Numero_Niveau],[Zone_Géo],[Type_actif],[Type_de_dette]
           ,[Groupe_rating],[col1],[col2],[Vie_residuelle],[Tranche_de_maturite]
           ,[coupon],[rendement])          
SELECT * FROM #NO_TRANS


DROP TABLE #MANDAT
DROP TABLE #NO_TRANS

