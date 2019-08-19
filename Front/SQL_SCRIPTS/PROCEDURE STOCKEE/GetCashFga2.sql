/*
 Ajout les lignes de cash dans la table PTF_FGA lors de l'ajout d'un nouveau fichier excel dans la table
*/
       
GO

CREATE PROCEDURE GetCashFga2

       @date DATETIME
AS 

--cash a mettre en valeur boursiere
SELECT  
		groupe,
		compte, 
		SUM(Valeur_Boursiere+Coupon_Couru) AS Somme 
into #tmp_fga FROM PTF_FGA 
WHERE DateInventaire =  @date 
GROUP BY groupe, compte 

--ligne de cash
INSERT INTO PTF_FGA
(groupe,dateinventaire,compte,isin_ptf,libelle_ptf,code_titre,isin_titre,libelle_titre,valeur_boursiere,coupon_couru,valeur_comptable,coupon_couru_comptable,PMV,PMV_CC,type_produit,devise_titre,secteur,sous_secteur,pays)
SELECT 
		f.groupe,
		@date AS dateinventaire,		
		p.compte,
		p.isin_ptf,
		p.libelle_ptf,
		CASE WHEN f.groupe = 'OPCVM' THEN 'Cash OPCVM'  ELSE 'Cash Mandat' END AS code_titre,
		CASE WHEN f.groupe = 'OPCVM' THEN 'Cash OPCVM'  ELSE 'Cash Mandat' END AS isin_titre,
		CASE WHEN f.groupe = 'OPCVM' THEN 'Liquidité(OPCVM)'  ELSE 'Liquidité(Mandat)' END AS libelle_titre, 
		AN-somme AS valeur_boursiere,
		0 As coupon_couru,
		AN-somme AS valeur_comptable,
		0 AS coupon_couru_comptable,
		0 As PMV,
		0 AS PMV_CC,
		'Cash' AS type_produit,
		'EUR' AS devise_titre,
		'Liquidité' As secteur,
		'Liquidité' AS sous_secteur,
		'France' AS pays
FROM PTF_AN p, #tmp_fga f 
where p.compte=f.compte and p.date = @date 
ORDER BY compte 

drop table #tmp_fga


--SELECT SUM(valeur_boursiere) FROM PTF_FGA WHERE dateinventaire='24/08/2011' and secteur='Liquidité'