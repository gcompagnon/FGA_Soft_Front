/*
 Ajout les dernieres informations connues de PTF_PROXY et PTF_PARAM_PROXY en changant la date
*/
       
GO

CREATE PROCEDURE GetProxyInfo

       @date DATETIME
AS 


DECLARE @datemax DATETIME
SET @datemax = (select max(p.date) from ptf_proxy p, ptf_param_proxy pp where p.date = pp.date)

INSERT INTO PTF_PROXY(date, code_proxy, libelle_proxy, code_titre, libelle_titre, poids_vb,poids_cc, type_produit, 
devise_titre,secteur,sous_secteur,pays,emetteur,rating,groupe_emet,maturité,duration)
SELECT @date AS date, code_proxy, libelle_proxy, code_titre, libelle_titre, poids_vb,poids_cc, type_produit, devise_titre,
secteur,sous_secteur,pays,emetteur,rating,groupe_emet,maturité,duration
FROM PTF_PROXY 
where date= @datemax

INSERT INTO PTF_PARAM_PROXY(date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source)
SELECT @date AS date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source
FROM PTF_PARAM_PROXY
where date = @datemax
