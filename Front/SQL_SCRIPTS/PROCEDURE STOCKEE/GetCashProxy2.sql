/*
 Ajout les lignes de cash dans la table PTF_PROXY lors de l'ajout d'un nouveau fichier excel dans la table
 TODO : voir si on ne peut pas mettre une faux pays pour les lignes de cash (exposition non pas à un pays, mais à l'Euro)
*/
--drop  PROCEDURE   GetCashProxy2    
GO

ALTER PROCEDURE GetCashProxy2

       @date DATETIME
AS 

--declare @date as datetime
--set  @date = '29/11/2013'


-- lister les code proxy du type inventaire pour l actif net
create table #CODE (code_proxy varchar(60) COLLATE DATABASE_DEFAULT, Libelle_Proxy varchar(60) COLLATE DATABASE_DEFAULT)

insert into #CODE 
SELECT DISTINCT p.code_proxy, p.Libelle_Proxy
FROM PTF_PROXY as p
left outer join PTF_PARAM_PROXY as paramP on paramP.code_proxy = p.code_proxy and paramP.Date = p.Date
WHERE (paramP.Source <> 'OPCVM' or paramP.Source is null) AND p.date = @date 
 

 
SELECT
	@date  As date,
	c.code_proxy,
	c.libelle_proxy AS libelle_proxy,
	'Liquidité(OPCVM)' As code_titre,
	'Liquidité(OPCVM)' As libelle_titre, 
	1 - SUM(isnull(p.Poids_VB,0) + isnull(p.Poids_CC,0)) AS poids_vb,
	0 AS poids_cc,
	'Cash' As type_produit,
	'EUR' AS devise_titre,
	'Liquidité' As secteur,
	'Liquidité' As sous_secteur,
	'France' As pays 
into #LIQUIDITE
FROM PTF_PROXY p 
	RIGHT JOIN #CODE c ON c.code_proxy = p.code_Proxy 	
WHERE date= @date and ( type_produit <> 'Cash' or type_produit is null)
GROUP BY c.code_proxy,c.libelle_proxy 

-- mettre les lignes de liquidité seulement non nuls
INSERT INTO PTF_PROXY
(date,code_proxy,libelle_proxy,code_titre,libelle_titre,poids_vb,poids_cc,type_produit,devise_titre,secteur,sous_secteur,pays)
select * from #LIQUIDITE where poids_vb <> 0

 
DROP TABLE #CODE
DROP TABLE #LIQUIDITE 

