select * from PTF_PARAM_PROXY where date='31/12/2014' and Code_Proxy not in (select distinct Code_Proxy from PTF_PROXY where DATE='31/12/2014')
DELETE FROM PTF_PARAM_PROXY where date='07/01/2015' and Code_Proxy ='CAC40'


--------------

select distinct p.code_proxy, p.Libelle_Proxy, pr.Libellé_Proxy
from PTF_PROXY as p
left outer join PTF_PARAM_PROXY as pr on pr.code_proxy = p.code_proxy and pr.date = p.date
where p.date = '31/12/2014' and pr.Libellé_Proxy is not null and  pr.Libellé_Proxy <>  p.Libelle_Proxy


select Code_proxy ,*
from PTF_PROXY
where code_titre = 'Liquidité(OPCVM)' and Date = '31/10/2012'

select Code_proxy ,*
from PTF_PROXY
where code_proxy = 'FR0010529743' and Date = '31/10/2012'

--==================================================================

execute Trans1 @date = '30/06/2015'
execute Trans2 @date = '30/06/2015'
execute Trans3 @date = '30/06/2015'
execute Trans4 @date = '30/06/2015'
execute Trans5 @date = '30/06/2015'

select * from PTF_TRANSPARISE where Dateinventaire  ='30/06/2015' and Compte ='6100097' and code_Titre ='AT0000652011'
juin 30 2015 12:00AM, 6100097, AT0000652011, SX5E 1215P27, , , 
--==================================================================

-- la table de transco type_produit en type actif est complete
select * from PTF_FGA where DateInventaire = '31/03/2015'
and ( Type_produit not in (select Produit from PTF_TYPE_ACTIF) or Type_produit in ('Action', 'Actions') )
and Sous_secteur not in (select Libelle from PTF_CARAC_OPCVM)
select * from PTF_PROXY where Date = '31/03/2015' 
and ( Type_produit not in (select Produit from PTF_TYPE_ACTIF) or Type_produit in ('Action', 'Actions') )
and Sous_secteur not in (select Libelle from PTF_CARAC_OPCVM)


select * from PTF_TYPE_ACTIF
select * from PTF_CARAC_OPCVM
select * from ref.PORTFOLIO where 
------------------------------------------------------------------------------------------------------------

select * from PTF_TYPE_ACTIF where Types = 'AS_PTF'
select * from PTF_CARAC_OPCVM
-- FUTURES DEVISE USD
insert into PTF_TYPE_ACTIF VALUES ('Obligations tx var perpetuelles','Obligations')
insert into PTF_TYPE_ACTIF VALUES ('FUTURES ACTIONS YEN','Actions')
insert into PTF_TYPE_ACTIF VALUES ('FUTURES ACTIONS GBP','Actions')
insert into PTF_TYPE_ACTIF VALUES ('FUTURES DEVISE USD','AS_PTF')
insert into PTF_TYPE_ACTIF VALUES ('Actions EAU','Actions')
insert into PTF_TYPE_ACTIF VALUES ('CALL SEK','Actions')
insert into PTF_TYPE_ACTIF VALUES ('Prise en pension','AS_PTF')
insert into PTF_TYPE_ACTIF VALUES ('Swaps longs','AS_PTF')
insert into PTF_TYPE_ACTIF VALUES ('Actions Islande','Actions') 
insert into PTF_TYPE_ACTIF VALUES ('Actions Afrique du Sud','Actions')
insert into PTF_TYPE_ACTIF VALUES ('Actions Malaysie','Actions')

insert into PTF_TYPE_ACTIF VALUES ('FUTURES DEVISE CAD','AS_PTF')
insert into PTF_TYPE_ACTIF VALUES ('FUTURES DEVISE GBP','AS_PTF')
insert into PTF_TYPE_ACTIF VALUES ('FUTURES TAUX JAPON','Obligations')
insert into PTF_TYPE_ACTIF VALUES ('Obligations JAPON','Obligations')
insert into PTF_TYPE_ACTIF VALUES ('FUTURES TAUX GB','Obligations')


-- modif car UNION PLus FR0000009987 est une sicav monétaire non classé en Action dans 6100030  FCP FEDERIS EURO ACTIONS
update PTF_TYPE_ACTIF
set Types = 'AS_PTF'
where Produit in ('Sicav', 'SICAV Australie', 'SICAV GBP', 'Sicav USD', 'SICAV YEN')

select * from PTF_TRANSPARISE where code_titre ='FR0000009987' and DateInventaire = '28/02/2014' and numero_niveau = 1
select * from PTF_TRANSPARISE where Type_produit = 'Sicav' and DateInventaire = '28/02/2014' and numero_niveau = 1
select * from PTF_TRANSPARISE where Type_produit = 'FUTURES ACTIONS USD' and DateInventaire >= '28/02/2014'
select * from SOUS_SECTEUR
select * from SECTEUR
insert into SOUS_SECTEUR VALUES('O GVT JP','O GVT','EMPRUNT D''ETAT JAPONAIS','TQA')
insert into SECTEUR VALUES('O CF IMMO','CORPORATES FINANCIERES IMMOBILIER','OMEGA')
insert into SOUS_SECTEUR VALUES('O CFI SENI','O CF IMMO','EMPRUNT IMMO SEN','OMEGA')

-----------------------------------------------------------------
-----------------------------------------------------------------
-- NETTOYAGE DES LIGNES CASH du PROXY et relance du caclcul cash
-- utilise pour supprimer les lignes de proxy à posteriori ou quand le proxy n est pas encore OK (type produit à null => la proc stock getcashproxy ne fonctionne pas)
delete from PTF_PROXY
where code_titre IN ( select code_titre from  #BLOOMBERG_REQUEST_NOUVELLE ) and Date = '30/06/2015'


delete from PTF_PROXY
where Type_produit = 'Cash' and Date = '30/06/2015'


execute GetCashProxy2 @date = '30/06/2015'


-----------------------------------------------------------------
-----------------------------------------------------------------



-----------------------------------------------------------------
-----------------------------------------------------------------

update PTF_PROXY
set Poids_CC = 0
where Date ='30/06/2015' and Poids_CC is null


-----------------------------------------------------------------
-----------------------------------------------------------------
-- CONTROLE et CORRECTION (des caract des instruments ds le proxy par ceux d OMEGA)
-----------------------------------------------------------------
-----------------------------------------------------------------
-----------------------------------------------------------------
--CONTROLE : les poids des proxy font entre 0.97 et 1.03
select Code_proxy,Libelle_Proxy, Count(*) as nbLignes, Sum(Poids_VB)as total , sum(isnull(Poids_CC,0)) as CC from PTF_PROXY 
where Date = '30/06/2015' and ( code_titre <> 'Liquidité(OPCVM)')
--and not ( total between 0.93 and 1.1)
group by Code_proxy,Libelle_Proxy
order by code_proxy

select Code_proxy,Libelle_proxy,Count(*) as nbLignes, Sum(Poids_VB+ isnull(Poids_CC,0)) as total
from PTF_PROXY 
where Date = '30/06/2015'
--and total <> 1
group by Code_proxy,Libelle_proxy

--CONTROLE : les poids des fonds hors cash dans l inventaire font entre 0.9 et 1.1
select p.compte,p.ISIN_Ptf, p.Libelle_Ptf, Count(*) as nbLignes, Sum(Valeur_Boursiere + coupon_couru )/a.AN as total
from PTF_FGA as p
left outer join PTF_AN as a on a.date = p.Dateinventaire and a.compte = p.Compte 
where Dateinventaire = '30/06/2015' and ( code_titre <> 'Cash Mandat' and code_Titre <> 'Cash OPCVM')
--and not ( total between 0.93 and 1.1)
group by p.compte,p.ISIN_Ptf, p.Libelle_Ptf,a.AN
order by total 


update PTF_PROXY
set Poids_CC = 0
where Poids_CC is null and Date='30/06/2015'

-- pour les proxy de convertible, il faut que le coupon soit pris en compte, MAIS le traitement GetCashProxy2
-- ne fonctionne pas ds ce cas -> suppression de la ligne de cash
delete from PTF_PROXY where date = '30/06/2015' and code_proxy ='FR0000180994' and code_titre ='Liquidité(OPCVM)'



-----------------------------------------------------------------
--CORRECTION : Proxy en PARAM "OLD" repris du dernier inventaire

declare @dateInv datetime
set @dateInv = '30/06/2015'

declare @dateRef datetime
set @dateRef = '29/05/2015'

insert into PTF_PARAM_PROXY
select @dateInv,param_proxy.Isin_titre, Libellé_titre, Code_proxy, Libellé_proxy,'OLD'
from PTF_PARAM_PROXY as param_proxy 
where isin_titre not in (select isin_titre from PTF_PARAM_PROXY where Date = @dateInv  )
and param_proxy.Date = @dateRef

select proxy.* 
into #RECOPY_OLD_PROXY
from PTF_PROXY as proxy 
where 
proxy.code_proxy in ( select distinct code_proxy from PTF_PARAM_PROXY where Source = 'OLD' and Date = @dateInv)
and proxy.Date = @dateRef

update #RECOPY_OLD_PROXY
set Date = @dateInv

update #RECOPY_OLD_PROXY
set Maturité = @dateInv
where Maturité <@dateInv


insert into PTF_PROXY
select * from #RECOPY_OLD_PROXY where code_proxy not in (select code_proxy from PTF_PROXY where date = @dateInv )

update PTF_PARAM_PROXY
set Source  ='PROXY'
where Source ='OLD' and Date = @dateInv

drop table #RECOPY_OLD_PROXY
--select distinct Libelle_Proxy from #RECOPY_OLD_PROXY

-------------------------------------------------------
-- Creation des  proxy de derives PROXY_DERIVES_SOUS_JACENT.sql
--C:\FGA_SOFT\DEVELOPPEMENT\PROJET\FGA_Soft_Front\Front\SQL_SCRIPTS\AUTOMATE\REFERENTIEL\PROXY_DERIVES_SOUS_JACENT.sql
-------------------------------------------------------

-- Creation des proxy Indices PROXY_ALIMENTATION_INDICES.sql
-- was (Creation des sous indices STOXX PROXY_STOXX_SECTORIEL.sql)
-------------------------------------------------------
update PTF_PARAM_PROXY
set Source  ='PROXY' --NO
where Source ='TODO' and Date = '30/06/2015'


update PTF_PROXY
set Maturité = '30/06/2015'
where Maturité <'30/06/2015' and date = '30/06/2015'


-----------------------------------------------------------------

select * from PTF_PROXY where date = '30/06/2015' and Code_Proxy = code_titre
update PTF_PROXY
set code_titre = code_titre+'_A'
where date = '30/01/2015' and Code_Proxy = code_titre




--CONTROLE : tous les proxy parametrés (PTF_PARAM_PROXY) sont décrits dans PTF_PROXY
select 'Il manque le detail du proxy',param.code_proxy,param.Libellé_Proxy  from PTF_PARAM_PROXY as param
left outer join PTF_PROXY as proxy on proxy.code_proxy = param.code_proxy and proxy.Date = param.Date
where param.Date = '30/06/2015'  and proxy.code_titre is null and ( param.Source = 'PROXY')
-----------------------------------------------------------------
-- COHERENCE PROXY OMEGA : pour les titres qui existent dans OMEGA, correction du proxy avec la référence OMEGA
-- Exception: il y a des titres qui sont définis comme proxy (code_titre = code_proxy
-----------------------------------------------------------------
-- liste des titres presents à la fois sur Omega et les proxy
-- ne pas prendre les proxy (titre qui sont modifiés par leurs caract ds le proxy)
drop table #doublonProxy
drop table #OMEGA_VALEURS

declare @dateInv datetime
set @dateInv = '30/06/2015'

select  distinct proxy.code_titre, proxy.Libelle_Titre, proxy.Type_Produit, proxy.Devise_Titre, proxy.Secteur, proxy.Sous_Secteur, proxy.Pays, proxy.Emetteur, proxy.Rating, proxy.Groupe_Emet, proxy.Maturité, proxy.Duration, proxy.Sensibilite
into #doublonProxy
from PTF_PROXY as proxy 
left outer join PTF_FGA as fga on fga.DateInventaire = @dateInv and fga.code_titre = proxy.code_titre
where proxy.Date = @dateInv and fga.code_titre is not null 
and proxy.code_titre <> proxy.code_proxy

--select code_titre, count(*) from #doublonProxy where code_titre not in ('Liquidité(OPCVM)')
--group by code_titre



select * from #doublonProxy
------------------------------
--- les valeurs venant d omega
select distinct fga.code_titre, fga.Libelle_Titre, fga.Type_Produit, fga.Devise_Titre, fga.Secteur, fga.Sous_Secteur, fga.Pays, fga.Emetteur, fga.Rating, fga.Grp_Emetteur, fga.Maturite, fga.Duration, fga.Sensibilite
into #OMEGA_VALEURS
from PTF_FGA as fga 
where fga.code_titre in (select code_titre from #doublonProxy)
and fga.DateInventaire = @dateInv
order by code_titre

--- les valeurs venant d omega


--select * from #OMEGA_VALEURS
------------------------------

-- COMPARAISON PROXY / OMEGA
select proxy.code_titre,
case when proxy.Libelle_Titre =  fga.Libelle_Titre then 'OK'
else proxy.Libelle_Titre + '<>'+fga.Libelle_Titre end as Libelle_Titre_Proxy_Omega,

case when proxy.Type_Produit =  fga.Type_Produit then 'OK'
else proxy.Type_Produit + '<>'+fga.Type_Produit end as Type_Produit_Proxy_Omega,

case when proxy.Devise_Titre =  fga.Devise_Titre then 'OK'
else proxy.Devise_Titre + '<>'+fga.Devise_Titre end as Devise_Titre_Proxy_Omega,

case when proxy.Secteur =  fga.Secteur then 'OK'
else proxy.Secteur + '<>'+fga.Secteur end as Secteur_Proxy_Omega,

case when proxy.Sous_Secteur =  fga.Sous_Secteur then 'OK'
else proxy.Sous_Secteur + '<>'+fga.Sous_Secteur end as Sous_Secteur_Proxy_Omega,

case when proxy.Pays =  fga.Pays then 'OK'
else proxy.Pays + '<>'+fga.Pays end as Pays_Proxy_Omega,

case when proxy.Emetteur =  fga.Emetteur then 'OK'
else proxy.Emetteur + '<>'+fga.Emetteur end as Emetteur_Proxy_Omega,

case when proxy.Rating =  fga.Rating then 'OK'
else proxy.Rating + '<>'+fga.Rating end as Rating_Proxy_Omega,

case when proxy.Groupe_Emet =  fga.Grp_Emetteur then 'OK'
else proxy.Groupe_Emet + '<>'+fga.Grp_Emetteur end as Grp_Emetteur_Proxy_Omega,

case when proxy.Maturité =  fga.Maturite then 'OK'
else convert(varchar,proxy.Maturité,103) + '<>'+convert(varchar,fga.Maturite,103) end as Maturité_Proxy_Omega,

case when proxy.Duration =  convert(float,fga.Duration) then 'OK'
else convert(varchar,proxy.Duration) + '<>'+convert(varchar,fga.Duration) end as Duration_Proxy_Omega,

case when proxy.Sensibilite =  convert(float,fga.Sensibilite) then 'OK'
else convert(varchar,proxy.Sensibilite) + '<>'+convert(varchar,fga.Sensibilite) end as Sensibilite_Proxy_Omega
from #doublonProxy as proxy
left outer join #OMEGA_VALEURS as fga on fga.code_titre = proxy.code_titre


-- mise à jour des caracteristiques des titres vu en proxy avec les donnees Omega
update PTF_PROXY
set Libelle_Titre = inv.Libelle_Titre,
 Type_Produit = inv.Type_Produit,
 Devise_Titre = inv.Devise_Titre,
 Secteur = inv.Secteur,
 Sous_Secteur = inv.Sous_Secteur,
 Pays = inv.Pays,
 Emetteur = inv.Emetteur,
 Rating = inv.Rating,
 Groupe_Emet = inv.Grp_Emetteur,
 Maturité = inv.Maturite,
 Duration = inv.Duration,
 Sensibilite = inv.Sensibilite
from #OMEGA_VALEURS as inv 
left outer join PTF_PROXY as proxy on proxy.code_titre = inv.code_titre 
where proxy.Date = @dateInv

select * from #OMEGA_VALEURS


------------------------------
-- FIN COHERENCE PROXY OMEGA
------------------------------

------------------------------
-- CONTROLE CORRECTION PROXY : les champs indispensables sont renseignés en utilisant le proxy le plus ancien
------------------------------
drop table #NonRenseigneProxy
drop table #PROXY_VALEURS_REFERENCE
declare @dateInv datetime
set @dateInv = '30/06/2015'

declare @dateProxyReference datetime
set @dateProxyReference = '29/05/2015'

select * 
into #NonRenseigneProxy
from PTF_PROXY where Date = @dateInv
and (
Devise_Titre is null or
Secteur is null or Sous_Secteur is null or
Pays is null or Emetteur is null or
Groupe_Emet is null)
and (Type_produit <> 'Cash' or Type_Produit is null)

--- les valeurs venant du proxy actuel (comme les convertibles definit dans le proxy ou les fonds)
---  FR0000180994 AXA 3.75%00-17 CV SUB. ou LU0225244705	US SELECT GROWTH I $
delete from #NonRenseigneProxy
where code_titre in (select code_proxy from PTF_PROXY where Date = @dateInv)

select * from #NonRenseigneProxy

--- les valeurs venant du proxy le plus ancien
select distinct oldproxy.code_titre, oldproxy.Libelle_Titre, oldproxy.Type_Produit, oldproxy.Devise_Titre, oldproxy.Secteur, oldproxy.Sous_Secteur, oldproxy.Pays, oldproxy.Emetteur, oldproxy.Rating, oldproxy.Groupe_Emet, oldproxy.Maturité, oldproxy.Duration, oldproxy.Sensibilite
into #PROXY_VALEURS_REFERENCE
from PTF_PROXY as oldproxy 
where oldproxy.code_titre in (select code_titre from #NonRenseigneProxy)
and oldproxy.Date = @dateProxyReference
order by code_titre

select * from #PROXY_VALEURS_REFERENCE

-- On reprend les caract static d un proxy plus anciens
update PTF_PROXY
set Libelle_Titre = inv.Libelle_Titre,
 Type_Produit = inv.Type_Produit,
 Devise_Titre = inv.Devise_Titre,
 Secteur = inv.Secteur,
 Sous_Secteur = inv.Sous_Secteur,
 Pays = inv.Pays,
 Emetteur = inv.Emetteur,
-- Rating = inv.Rating, donnee à maj
 Groupe_Emet = inv.Groupe_Emet,
 Maturité = inv.Maturité
-- Duration = inv.Duration,         donnee à maj
-- Sensibilite = inv.Sensibilite    donnee à maj
from #PROXY_VALEURS_REFERENCE as inv 
left outer join PTF_PROXY as proxy on proxy.code_titre = inv.code_titre 
where proxy.Date = @dateInv
---------------------------------------------------------------------
-- les lignes qui sont à renseigner par Bloomberg/excel:
declare @dateInv datetime
set @dateInv = '30/06/2015'

drop table #BLOOMBERG_REQUEST_NOUVELLE
select * 
into #BLOOMBERG_REQUEST_NOUVELLE
from PTF_PROXY where Date = @dateInv
and (
Devise_Titre is null or Devise_Titre = '' or
Secteur is null or Sous_Secteur is null or 
Pays is null or Emetteur is null or Emetteur = '' or  
Groupe_Emet is null or Groupe_Emet = '')
and ( Type_produit <> 'Cash' or Type_Produit is null )
and Code_Proxy not in ('CERT_EUR', 'CERT_exEUR', 'SP500NET')

select distinct Code_titre,Libelle_Titre 
from #BLOOMBERG_REQUEST_NOUVELLE 



-- de type obligataire: rating, et maturité obligatoire pour l expo
drop table #BLOOMBERG_REQUEST_RATING
select * 
into #BLOOMBERG_REQUEST_RATING
from PTF_PROXY where Date = @dateInv
and ( Type_produit like 'Obligation%' )
and ( rating is null or rating = '' or
Maturité is null or Maturité = '')
and Type_produit <> 'Cash'


-- de type obligataire: rating, et maturité obligatoire pour l expo
drop table #BLOOMBERG_REQUEST_RATING_FGA
select * 
into #BLOOMBERG_REQUEST_RATING_FGA
from PTF_FGA where Dateinventaire  = @dateInv
and ( Type_produit like 'Obligation%' )
and ( rating is null or rating = '' or
Maturite is null or Maturite = '')
and Type_produit <> 'Cash'


select distinct Code_titre,Libelle_Titre 
from #BLOOMBERG_REQUEST_RATING
select distinct Code_titre,Libelle_Titre 
from #BLOOMBERG_REQUEST_RATING_FGA


-- de type obligataire: duration et sensi à renseigner 
drop table #BLOOMBERG_REQUEST_DURATION
select * 
into #BLOOMBERG_REQUEST_DURATION
from PTF_PROXY where Date = @dateInv
and Type_produit like 'Obligation%'
and ( 
Duration is null or Sensibilite is null)
and Type_produit <> 'Cash'
and code_titre not in (select code_titre from #BLOOMBERG_REQUEST_NOUVELLE)

drop table #BLOOMBERG_REQUEST_DURATION_FGA
select * 
into #BLOOMBERG_REQUEST_DURATION_FGA
from PTF_FGA where Dateinventaire  = @dateInv
and Type_produit like 'Obligation%'
and ( 
Duration is null or Sensibilite is null)
and Type_produit <> 'Cash'
and code_titre not in (select code_titre from #BLOOMBERG_REQUEST_NOUVELLE)



select distinct code_titre, Libelle_titre from #BLOOMBERG_REQUEST_DURATION
select distinct code_titre, Libelle_titre from #BLOOMBERG_REQUEST_DURATION_FGA

-- de type obligataire: obligation convertible
--  'Obligations Convertibles'
-- objectif Ajouter une ligne en action

-- a metttre avec les proxys:
insert into PTF_PROXY
select '30/08/2013',code_titre, Libelle_titre,  code_titre, Libelle_titre, 1,0,Type_produit,devise_titre,secteur,sous_secteur,
pays,emetteur,rating,grp_emetteur,Maturite,duration,sensibilite
from PTF_FGA where DateInventaire = '31/07/2013' 
and Type_produit like 'Obligation%Conv%'
and Type_produit <> 'Cash'
and code_titre not in (select code_titre from #BLOOMBERG_REQUEST_NOUVELLE)


drop table #BLOOMBERG_REQUEST_CONVERTIBLE
select * 
into #BLOOMBERG_REQUEST_CONVERTIBLE
from PTF_PROXY where Date = '30/04/2013' 
and Type_produit like 'Obligation%Conv%'
and Type_produit <> 'Cash'
and code_titre not in (select code_titre from #BLOOMBERG_REQUEST_NOUVELLE)

select code_titre,code_proxy,libelle_Proxy,code_titre+' corp ISIN',Libelle_titre, Poids_VB,Poids_CC from #BLOOMBERG_REQUEST_CONVERTIBLE order by Libelle_titre



-------------------------------------------------
-- CONTROLE AVANT TRANSPARENCE
-- Les colonnes resultats de la transparence (type_actif, zone geographique) sont d'un code renseignée dans une table de codification
-- Si il y a des valeurs non renseignées , il faut donc analyser, et ajouter si besoin une codification
-------------------------------------------------


begin transaction
update PTF_PROXY
set Groupe_Emet = 'Société de Gestion de Portefeuille',
Pays = 'Not Available',
maturité = NULL
where Secteur in ('FONDS ACTIONS','FONDS TRESORERIE','FONDS OBLIGATIONS') and date = '30/06/2015'
and Type_produit not like 'FUTURES%'
commit
rollback

select * from PTF_PROXY where date = '30/06/2015' and  Secteur in ('FONDS ACTIONS','FONDS TRESORERIE','FONDS OBLIGATIONS') 


-- type actif en OPCVM ... ne doit plus exister dans les fonds transparises
-- si ce sont des Trackers , ajouter un Param_Proxy
select * from PTF_PROXY where date = '30/06/2015' and (type_produit  like 'SICAV%' or type_produit is null or type_produit = 'Fonds Commun de Placement')
and Secteur NOT IN ('FONDS TRESORERIE', 'FONDS ACTIONS', 'FONDS OBLIGATIONS')
and code_titre not in (select Isin_Titre from PTF_PARAM_PROXY where date = '29/05/2015')


-- AJOUT D UN PROXY SUR ETF
--select * from PTF_PARAM_PROXY where date = '30/09/2013' and code_proxy = 'SXXP_PETRO'
--insert into PTF_PARAM_PROXY VALUES ('29/05/2015', 'FR0010344960', 'LYX.STOXX EUR.600 OIL GAS ETF','SXXP_PETRO','DJS 600 Pr - PETROLE ET GAZ', 'PROXY')

--insert into PTF_PARAM_PROXY VALUES ('28/02/2014', 'FR0010616979', 'EXANE CERES','FR0010616979','EXANE CERES', 'PROXY')
--INSERT INTO [E0DBFGA01].[dbo].[PTF_PROXY]           ([Date],[Code_Proxy] ,[Libelle_Proxy],[code_titre],[Libelle_Titre]
--           ,[Poids_VB],[Poids_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur],[Pays],[Emetteur],[Groupe_Emet])
--     VALUES ('28/02/2014','LU0284634564','EXANE CERES','LU0284634564_O','EXANE CERES',
--     1.0000,0,'Sicav','EUR','FONDS OBLIGATIONS','OPCVM OBLIGATIONS EURO','France','Exane', 'Exane')
--insert into PTF_PARAM_PROXY VALUES ('31/12/2014', 'FR0011891506', 'NATIXIS ACTIONS EURO PME C €','FR0011891506','NATIXIS ACTIONS EURO PME C €', 'PROXY')
--INSERT INTO [E0DBFGA01].[dbo].[PTF_PROXY]           ([Date],[Code_Proxy] ,[Libelle_Proxy],[code_titre],[Libelle_Titre]
--           ,[Poids_VB],[Poids_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur],[Pays],[Emetteur],[Groupe_Emet])
--     VALUES ('31/12/2014', 'FR0011891506', 'NATIXIS ACTIONS EURO PME C €','FR0011891506_A','NATIXIS ACTIONS EURO PME C €',
--     1.0000,0,'Sicav','EUR','FONDS ACTIONS','OPCVM ACTIONS EURO','Not Available','Natixis', 'Société de Gestion de Portefeuille')
--insert into PTF_PARAM_PROXY VALUES ('27/02/2015', 'FR0010296061', 'LYXOR MSCI USA','MSCI_US','MXUS proxy pour SP500NET', 'PROXY')
--insert into PTF_PARAM_PROXY VALUES ('27/02/2015', 'FR0011645647', 'LYXOR EStoxx BANK','MSCI_E_BANKS','MXEM0BK proxy pour STOXX EURO BANQUE', 'PROXY')

--select * from PTF_PROXY where DATE ='31/12/2014' and Code_Proxy = 'LU0284634564'
--select * from PTF_PARAM_PROXY where DATE ='28/02/2014' and Code_Proxy = 'LU0284634564'




-- type produit est correct et est renseigné dans PTF_TYPE_ACTIF
select * from PTF_PROXY where date = '30/06/2015' and type_produit not in (select Produit from PTF_TYPE_ACTIF where Produit not in ('Action','Actions'))

update PTF_PROXY
set Secteur = 'FONDS ACTIONS', Sous_secteur = 'OPCVM ACTIONS EURO'
where Date = '29/05/2015' and code_titre IN ( 'FR0010168781') 


-- Attrribution des caract pour les fonds
update PTF_PROXY
set Secteur = 'FONDS ACTIONS', Sous_secteur = 'OPCVM ACTIONS EURO'
where Date = '31/03/2014' and code_titre IN ( 'LU0322251520' )

update PTF_PROXY
set Secteur = 'FONDS ACTIONS', Sous_secteur = 'OPCVM ACTIONS EURO'
,Pays ='Allemagne'
where Date = '31/03/2015' and code_titre IN ( 'FR0011891506' )


update PTF_PROXY
set Secteur = 'FONDS ACTIONS', Sous_secteur = 'OPCVM ACTIONS EUROPE'
where Date = '30/06/2015' and code_titre IN ( 'FR0010558841')


update PTF_PROXY
set Secteur = 'FONDS ACTIONS', Sous_secteur = 'OPCVM ACTIONS USA'
,Pays ='Etats-Unis'
where Date = '29/05/2015' and code_titre IN ( 'LU0225244705','FR0010688275')

update PTF_PROXY
set Secteur = 'FONDS ACTIONS', Sous_secteur = 'OPCVM OR ET MATIERES PREMIERES'
where Date = '28/02/2013' and code_titre IN ( '' )

select * from SOUS_SECTEUR where id not like '%_OLD'

update PTF_PROXY
set Secteur = 'FONDS ACTIONS', Sous_secteur = 'OPCVM ACTIONS MONDE'
,Groupe_Emet ='Société de Gestion de Portefeuille', 
Pays = 'Not Available',
maturité = NULL
where Date = '29/05/2015' and code_titre IN ( 'FR0010312124' )

update PTF_PROXY
set Secteur = 'FONDS ACTIONS', Sous_secteur = 'OPCVM ACTIONS ASIE PACIFIQUE'
,Groupe_Emet ='Société de Gestion de Portefeuille', 
Pays = 'Not Available',
maturité = NULL
where Date = '28/02/2014' and code_titre IN ( 'JP3027630007' )


update PTF_PROXY
set Secteur = 'FONDS TRESORERIE', Sous_secteur = 'OPCVM TRESORERIE'
,Groupe_Emet ='Société de Gestion de Portefeuille', 
Pays = 'France',
maturité = NULL
where Date = '31/12/2014' and code_titre IN ( 'FR0000291239','FR0007435920' )


update PTF_PROXY
set Secteur = 'FONDS OBLIGATIONS', Sous_secteur = 'OPCVM OBLIGATIONS CREDIT'
,Groupe_Emet ='Société de Gestion de Portefeuille', 
Pays = 'Allemagne',
maturité = NULL
where Date = '29/05/2015' and code_titre IN ( 'LU1054336893')

--- Les secteurs et sous secteurs sont coherents avec le type de produit

-- les fonds
select *  from PTF_PROXY where  Date = '30/06/2015' and type_produit like 'Fonds Commun de Placement' and sous_Secteur not in 
(select libelle from SOUS_SECTEUR where  id like 'F% %'  and id not like '%_OLD') and Secteur like  'FONDS%'

-- Les pays non renseignés sur les fonds : on essaye d etre exaustif mais impossible pour des fonds MONDE
select *  from PTF_PROXY where  Date = '30/06/2015' and type_produit like 'Fonds Commun de Placement' and pays not in 
(select Pays from dbo.ZONE_GEOGRAPHIQUE where Pays <> 'Not Available') and Secteur like  'FONDS%'

-- mettre france pour les fonds EURO et USA pour les USA
update PTF_PROXY
set Pays = 'France'
where Date = '30/06/2015' and Sous_secteur in  ('OPCVM ACTIONS EURO','OPCVM TRESORERIE','OPCVM OBLIGATIONS EURO') and pays not in 
(select Pays from dbo.ZONE_GEOGRAPHIQUE where Pays <> 'Not Available')
update PTF_PROXY
set Pays = 'Etats-Unis'
where Date = '30/06/2015' and Sous_secteur in  ('OPCVM ACTIONS USA','OPCVM OBLIGATIONS USA') and pays not in 
(select Pays from dbo.ZONE_GEOGRAPHIQUE where Pays <> 'Not Available')

-- cas particulier : les fonds actions
select * from PTF_PROXY where  Date = '30/06/2015' and ( type_produit like 'Action%' or type_produit = 'Droits d''attribution') and sous_Secteur not in 
(select libelle from SOUS_SECTEUR where id like 'A %' and id not like '%_OLD') and Secteur <> 'FONDS ACTIONS'


-- les fonds
select *  from PTF_PROXY where  Date = '30/06/2015' and type_produit like 'Action%' and sous_Secteur not in 
(select libelle from SOUS_SECTEUR where id like 'FA %' and id not like '%_OLD') and Secteur = 'FONDS ACTIONS'

-- controle general : plus que de la liquidité
select *  from PTF_PROXY where  Date = '30/06/2015'  and sous_Secteur not in 
(select libelle from SOUS_SECTEUR where  id not like '%_OLD')  and code_titre <> 'Liquidité(OPCVM)'


--COHERENCE SECTEUR SOUS SECTEUR
select ss.libelle, sous_secteur,s.libelle,secteur,ss.id,s.id,*  from PTF_PROXY as p
left outer join  SOUS_SECTEUR as ss on ss.libelle = p.sous_secteur 
left outer join  SECTEUR as s on s.id = ss.id_secteur 
where  Date = '30/06/2015' and Secteur <> s.libelle

-- correction : mettre le secteur , lié au sous secteur
update PTF_PROXY
set secteur = s.libelle
from PTF_PROXY as p
left outer join  SOUS_SECTEUR as ss on ss.libelle = p.sous_secteur
left outer join  SECTEUR as s on s.id = ss.id_secteur
where  Date = '30/06/2015' and Secteur <> s.libelle

select *  from PTF_PROXY where  Date = '30/06/2015' and type_produit like 'Obligation%' and Secteur not in 
(select libelle from SECTEUR where id like 'O %' and id not like '%_OLD')

select *  from PTF_PROXY where  Date = '30/06/2015' and type_produit like 'Action%' and Secteur not in 
(select libelle from SECTEUR where id like 'A %' and id not like '%_OLD')and Secteur <> 'FONDS ACTIONS'

select *  from PTF_PROXY where  Date = '30/06/2015' and type_produit <> 'Cash' and Secteur not in 
(select libelle from SECTEUR where id not like '%_OLD')

-- des secteurs incorrects car de type Actions pour des fonds
select *  from PTF_PROXY where  Date = '30/06/2015' and  sous_Secteur in 
(select libelle from SOUS_SECTEUR where id like 'A %' and id not like '%_OLD') and ( type_produit not like 'Action%'  and type_produit <> 'Droits d''attribution' and type_produit <> 'Droits d''attribution en GBP')

-- METTRE A JOUR LES SOUS SECTEUR des indices venant de statpro
select distinct p2.code_titre, p1.Sous_Secteur ,p2.Sous_Secteur as "ss"  
into #SECTEUR_ACTION
from PTF_PROXY as p1 
left outer join PTF_PROXY as p2 on p2.date = '30/06/2015'  and p2.code_titre = p1.code_titre 
where p1.date = '30/06/2015' 
and p1.Sous_Secteur <>p2.Sous_Secteur
and p2.Type_Produit like 'Actions%'
order by p2.code_titre

select * from #SECTEUR_ACTION

begin transaction
update PTF_PROXY 
set Sous_secteur = s.sous_secteur
from PTF_PROXY as p
left outer join #SECTEUR_ACTION as s on s.code_titre = p.code_titre 
where p.Date = '30/06/2015' and s.code_titre is not null
commit
rollback
drop table #SECTEUR_ACTION
----------------------------------


-- les secteurs sont mis en type actions, et ensuite retransco en typage obligataires
--CORRECTION DES SOUS SECTEURS OBLIGATION 
declare @dateInv datetime
set @dateInv = '30/06/2015'
update PTF_PROXY set Sous_secteur = 'EMPRUNT SENIOR ASSURANCE' where Date =@dateInv and Sous_secteur = 'EMPRUNT ASSURANCE'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT ASSURANCE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'ASSURANCE'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT AUTOMOBILE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'AUTOMOBILE'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT B&S DE CONSOMMATION SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'B&S DE CONSOMMATION'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT B&S DE CONSOMMATION SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'BIENS DE CONSOMMATION'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT B&S INDUSTRIELS SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'B&S INDUSTRIELS'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT BANQUE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'BANQUES'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT CHIMIE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'CHIMIE'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT DISTRIBUTION SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'DISTRIBUTION'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT ENERGIE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'ENERGIE'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT MEDIA SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'MEDIAS'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT PRODUITS DE BASE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'PRODUITS DE BASE'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT SANTE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'SANTE'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT SERVICES FINANCIERS SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'SERVICES FINANCIERS'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT SERVICES FINANCIERS SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'FONDS OBLIGATIONS'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT TELECOMMUNICATIONS SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'TELECOMMUNICATIONS'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT VOYAGES ET LOISIRS SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'VOYAGES ET LOISIRS'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT TECHNOLOGIE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'TECHNOLOGIE'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT CONSTRUCTION SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'IMMOBILIER'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT ALIMENTAIRE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'ALIMENTAIRE - BOISSON'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT MEDIA SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'SERVICES AUX CONSOMMATEURS'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT ENERGIE SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'PETROLE ET GAZ'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT SERVICES AUX COLLECTIVITES SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'SERVICES AUX COLLECTIVITES'
UPDATE PTF_PROXY set Sous_secteur = 'EMPRUNT CONSTRUCTION SEN' where Date = @dateInv and  type_produit like 'Obligation%' and Sous_secteur = 'CONSTRUCTION'



update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT PORTUGAIS',
Emetteur = 'PORTUGAL',Groupe_emet = 'PORTUGAL'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and ( Emetteur = 'Portuguese Republic' or Pays = 'Portugal' )

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT FINLANDAIS',
Emetteur = 'FINLANDE',Groupe_emet = 'FINLANDE'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and ( Emetteur = 'REPUBLIC OF FINLAND' or Pays = 'Finlande' )


update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT ALLEMAND',
Emetteur = 'ALLEMAGNE',Groupe_emet = 'ALLEMAGNE'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and ( Emetteur = 'Federal Republic of Germany' or Pays ='Allemagne')

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT EURO'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and ( Pays ='Supranational' or Pays = 'SLOVAQUIE')

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT ITALIEN',
Emetteur = 'ITALIE',Groupe_emet = 'ITALIE'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and ( Emetteur = 'Republic of Italy' or Pays = 'Italie' )

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT FRANCAIS',
Emetteur = 'FRANCE',Groupe_emet = 'FRANCE'
where Date = @dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and pays ='France'

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT ESPAGNOL',
Emetteur = 'ESPAGNE',Groupe_emet = 'ESPAGNE'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and Pays = 'Espagne'

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT HOLLANDAIS',
Emetteur = 'PAYS BAS',Groupe_emet = 'PAYS BAS'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and Pays = 'Pays-Bas'

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT BELGE',
Emetteur = 'BELGIQUE',Groupe_emet = 'BELGIQUE'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and Pays = 'Belgique'

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT AUTRICHIEN',
Emetteur = 'AUTRICHE',Groupe_emet = 'AUTRICHE'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and Pays = 'Autriche'

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT ITALIEN',
Emetteur = 'ITALIE',Groupe_emet = 'ITALIE'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and Pays = 'Italie'

update PTF_PROXY
set Sous_secteur = 'EMPRUNT D''ETAT AMERICAIN',
Emetteur = 'ETATS-UNIS',Groupe_emet = 'ETATS-UNIS'
where Date =@dateInv and Sous_secteur = 'EMPRUNTS D''ETAT' and Pays = 'Etats-Unis'

update PTF_PROXY
set Sous_secteur = 'EMPRUNT SERVICES FINANCIERS SEN'
where Date = @dateInv and Sous_secteur = 'EMPRUNT BANQUE SEN' 

update PTF_PROXY
set Sous_secteur = 'EMPRUNT BANQUE SEN'
where Date = @dateInv and Sous_secteur = 'EMPRUNT SERVICES FINANCIERS SEN'  and Secteur = 'CORPORATES FINANCIERES BANQUES'


-- les zone geo sont toutes dispo
select * from PTF_PROXY where date ='27/02/2015' and Pays not in (select Pays from  ZONE_GEOGRAPHIQUE)
------------------------------
-- FIN CONTROLE PROXY
------------------------------
-----------------------------------------------------------------
-- TRANSCO et FORCAGE
declare @dateInv datetime
set @dateInv = '30/06/2015'
--XS0386772924	TESCO PLC	BBB+
update PTF_PROXY
set Rating = 'BBB+'
where date = @dateInv and code_titre = 'XS0386772924'
--FR0120251238	CAISSE D'AMORT DETTE SOC	AA+
update PTF_PROXY
set Rating = 'AA+'
where date = @dateInv and code_titre = 'FR0120251238'
--FR0121161857	DEXIA CREDIT LOCAL	BBB
update PTF_PROXY
set Rating = 'BBB'
where date = @dateInv and code_titre = 'FR0121161857'
--XS0736639542	ING BANK NV	A
update PTF_PROXY
set Rating = 'A'
where date = @dateInv and code_titre = 'XS0736639542'
--XS0778801356	CREDIT AGRICOLE CIB	A
update PTF_PROXY
set Rating = 'A'
where date = @dateInv and code_titre = 'XS0778801356'
-- BE0312711808 TBILL BE 13-02-14
update PTF_PROXY
set Rating = 'AA'
where date = @dateInv and code_titre = 'BE0312711808'
-- XS0878860344 UNICREDIT BK PUT EUR3M + 60 BP 24-01-14
update PTF_PROXY
set Rating = 'BBB'
where date = @dateInv and code_titre = 'XS0878860344'

--XS1039391864	INTESA SANPAOLO E3M+10BPS 27-02-15 PUT  BBB 
--XS0989842322	UNICREDIT TR 11-14 PUT  BBB
--XS1028600473	ORANGE 4.25 PERP   BBB
update PTF_PROXY
set Rating = 'BBB'
where date = @dateInv and code_titre IN ( 'XS1039391864' , 'XS0989842322', 'XS1028600473')

--BE0312704738	TBILL BE 19-06-14 AA
update PTF_PROXY
set Rating = 'AA'
where date = @dateInv and code_titre = 'BE0312704738'

--et pour le fonds 
--FR0007038138	AMUNDI TRESO 3 MOIS mettre A
update PTF_PROXY
set Rating = 'A'
where date = @dateInv and code_titre = 'FR0007038138'
--Mail Perrine 09/09/2014
update PTF_PROXY
set Rating = 'AA'
where date = @dateInv and code_titre = 'BE0312707764'
update PTF_PROXY
set Rating = 'A'
where date = @dateInv and code_titre in ( 'FR0011949361' , 'FR0011949361', 'XS0811150522')
update PTF_PROXY
set Rating = 'BBB'
where date = @dateInv and code_titre in ( 'FR0121967428', 'XS1058100758')
   

--BE0005646204 AGEAS - STRIP VVPR est une action et non une obligation.
update PTF_PROXY
set Type_Produit='Droits d''attribution',
Secteur = 'Finance',
Sous_Secteur = 'Assurances Multirisques'
where date = @dateInv and code_titre = 'BE0005646204'



-----------------------------------------------------------------


-- tests pour la date de maturité sur les Oblig callable (la date de maturite doit rester stable)
select f.Maturite,f.Libelle_Titre,p.Maturite,p.dateinventaire,f.* from PTF_FGA as f
left outer join PTF_FGA as p on p.code_titre = f.code_titre and p.dateinventaire > '30/09/2013'
where f.DateInventaire = '30/06/2015'	and f.Type_produit in (
'Obligations tx fixe perpetuelles',
'Obligations Taux Fixe avec Call',
'Obligations Taux Variable avec Call' )
and f.Maturite <> p.Maturite



-- forcer les maturites pour les titres echus
update PTF_PROXY
set Maturité = '30/06/2015'
where Date = '30/06/2015' 
and maturité < '30/06/2015'


-----------------------------------------------------------------
--CONTROLE : Actif Net est retrouvé sur les mandats
-- au niveau d omega

drop table #CONTROLE_AN
drop table #CONTROLE_AN2
drop table #CONTROLE_AN2_AN

select p.DateInventaire,sum( Valeur_boursiere + Coupon_couru ) as Total, a.AN, p.Compte ,p.Libelle_Ptf 
into #CONTROLE_AN
from PTF_FGA as p
left outer join PTF_AN as a on a.date = DateInventaire and a.compte = p.Compte
where p.DateInventaire = '30/06/2015' 
--and p.Groupe <> 'OPCVM'
group by p.DateInventaire,p.Compte ,p.Libelle_Ptf ,a.AN

select * from #CONTROLE_AN where ABS(Total - AN )/Total > 10E-15


-- Comparaison sur le mois precedent
select p.DateInventaire,sum( Valeur_boursiere + Coupon_couru ) as Total, a.AN, p.Compte ,p.Libelle_Ptf 
into #CONTROLE_AN2
from PTF_FGA as p
left outer join PTF_AN as a on a.date = DateInventaire and a.compte = p.Compte
where p.DateInventaire = '29/05/2015' 
--and p.Groupe <> 'OPCVM'
group by p.DateInventaire,p.Compte ,p.Libelle_Ptf ,a.AN

select 100*(A.AN-B.AN)/B.AN as evolutionAN, A.*, B.DateInventaire as DateInventaireM2,B.Total as TotalM2,B.AN as AN_M2,B.Compte as Compte_M2,B.Libelle_Ptf as Libelle_Ptf_M2
into #CONTROLE_AN2_AN
from #CONTROLE_AN as A
FULL JOIN #CONTROLE_AN2 as B on B.Compte = A.Compte

select * from #CONTROLE_AN2_AN 
where evolutionAN is null
Union
select * from #CONTROLE_AN2_AN
where evolutionAN is not null
--order by ABS(evolutionAN) desc


-- Controle des type_actif à Mixte. pas de fonds en Mixte (exposition)
select * from PTF_TRANSPARISE where dateInventaire = '30/06/2015' and numero_niveau = 5 and type_actif = 'Mixte'
and type_produit <> 'Cash' and sous_secteur <> 'OPCVM TRESORERIE'


drop table #CONTROLE_AN
-- au niveau de la transparence 
select sum( Valeur_boursiere + Coupon_couru ) as Total, a.AN, p.Compte ,p.Libelle_Ptf 
into #CONTROLE_AN
from PTF_TRANSPARISE as p
left outer join PTF_AN as a on a.date = DateInventaire and a.compte = p.Compte
where p.DateInventaire = '30/06/2015' and p.numero_Niveau = 5
--and p.Groupe <> 'OPCVM'
group by p.Compte ,p.Libelle_Ptf ,a.AN

select * from #CONTROLE_AN where ABS(Total - AN )/Total > 10E-15


drop table #CONTROLE_AN

select * from PTF_TRANSPARISE where Dateinventaire ='01/04/2015' and Compte ='04227' and Numero_Niveau =3
and Libelle_Titre like '%'
select * from PTF_PROXY where DATE ='01/04/2015' and Libelle_Proxy  like '%CERT%'

-----------------------------------------------------------------
--CONTROLE : la transparence est totale: plus de code proxy dans le ptf_transparise
declare @dateInv datetime
set @dateInv = '30/06/2015'
select * 
from PTF_TRANSPARISE as p
where  p.DateInventaire = @dateInv and p.numero_Niveau = 5
and p.code_titre in (select isin_titre from PTF_PARAM_PROXY 
where Date = @dateInv and Source <> 'NO')
and p.code_titre not in (
select code_titre from PTF_PROXY where Date = @dateInv and code_titre = code_proxy )
--- résultat seulement les lignes non transparise
--=> aucune ligne


--CONTROLE : on retrouve bien les codes proxy dans l inventaire
declare @dateInv datetime
set @dateInv = '30/06/2015'
select Isin_titre, Libellé_Titre 
from PTF_PARAM_PROXY as p
where  p.Date = @dateInv 
and  p.Isin_titre not in (select distinct code_titre from PTF_FGA
where DateInventaire = @dateInv )

--- résultat: des proxy qui sont anciens





------------------------------
-- CONTROLE INVENTAIRE OMEGA : qualité des données (tous les champs sont renseignés)
------------------------------

-- le typage du mandat est bien renseigné pour tous les comptes de mandat ds notre table de ref:
-- O obligataire,  A acion ou D Diversifie
select distinct Compte,Libelle_Ptf from PTF_FGA where Dateinventaire >= '29/05/2015'
and Groupe <> 'OPCVM'
and Compte not in (select Compte from ref.Portfolio)

select * from ref.Portfolio order by libelle 


--insert into ref.Portfolio
--VALUES ('44002','CAPREVAL ACTIONS', 'A')
--insert into ref.Portfolio
--VALUES ('5300253','U.N.M.I PRATIC', 'D')

select *  from PTF_FGA where  Dateinventaire  = '30/06/2015' and type_produit like 'Fonds Commun de Placement' and sous_Secteur not in 
(select libelle from SOUS_SECTEUR where  id like 'F% %'  and id not like '%_OLD') and Secteur like  'FONDS%'

select *  from PTF_FGA where  Dateinventaire  = '30/06/2015' and type_produit like '%Action%' and sous_Secteur not in 
(select libelle from SOUS_SECTEUR where  id like 'A% %'  and id not like '%_OLD')

select *  from PTF_FGA where  Dateinventaire  = '30/06/2015' and type_produit like '%Oblig%' and sous_Secteur not in 
(select libelle from SOUS_SECTEUR where  id like 'O% %'  and id not like '%_OLD')


-- ca particuliers les swaps longs du FCP treso qui n ont pas de secteur/sous secteurs 
select * 
from PTF_FGA where DateInventaire = '30/06/2015' 
and (
Devise_Titre is null or Devise_Titre = '' or
Secteur is null or Sous_Secteur is null or
Secteur = ''  or Sous_Secteur = ''  or
Pays is null or Emetteur is null or
Pays ='' or Emetteur ='' or
Grp_Emetteur is null or Grp_Emetteur = '')
and ( Type_produit <> 'Cash' or Type_Produit is null )

-- de type obligataire: rating, et maturité obligatoire pour l expo
-- sauf pour les swaps monetaire
select * 
from PTF_FGA where DateInventaire = '30/06/2015'
and ( Type_produit like 'Obligation%' or Type_produit like 'Swaps %' or Type_produit like 'FUTURES TAUX %' ) --or Secteur = 'FONDS OBLIGATIONS' )
and ( rating is null or 
Maturite is null )
--and Type_produit <> 'Cash'



-- Pays groupe emetteur ?
select * 
from PTF_FGA where DateInventaire = '30/06/2015' 
and ( ( Pays is null )
or ( 
Pays ='') )
and Type_produit <> 'Cash'

select * from PTF_TRANSPARISE where Dateinventaire ='29/05/2015' and numero_niveau = 5 and
 (maturite is not null and maturite <> '01/01/1900')  and Tranche_de_maturite is null


-- de type obligataire: duration à renseigner si possible
select * 
from PTF_FGA where DateInventaire = '30/06/2015' 
and ( Type_produit like 'Obligation%' )-- or Secteur = 'FONDS OBLIGATIONS' )
and ( 
Duration is null or Sensibilite is null or Sensibilite = 0)
and Type_produit <> 'Cash'



-- Date de maturité dans le passé ? (remboursement non effectuée si 1 jour de retard)
select * 
from PTF_FGA where DateInventaire = '30/06/2015'
and ( Type_produit like 'Obligation%' ) 
and ( Maturite < DateInventaire)
and Type_produit <> 'Cash'

------------------------------
-- CONTROLE TRANSPARENCE : qualité des données (tous les champs sont renseignés)
------------------------------
-- la zone geographique est non nul : sinon renseigner le pays dans la table  ZONE_GEOGRAPHIQUE
--insert into ZONE_GEOGRAPHIQUE values('Colombie','Emergents')

select *
from PTF_TRANSPARISE where DateInventaire = '30/06/2015' and numero_niveau = 5
and 
Zone_Géo is null 
and ( Type_produit <> 'Cash' or Type_Produit is null )




select * from ZONE_GEOGRAPHIQUE
--update ZONE_GEOGRAPHIQUE
--set Zone ='Emergents'
--where Pays in ( 'Brésil', 'Russie','Inde', 'Chine')

--insert into ZONE_GEOGRAPHIQUE values('QATAR','Emergents')

--insert into ZONE_GEOGRAPHIQUE values('Brésil','Autre')
--insert into ZONE_GEOGRAPHIQUE values('Inde','Asie')
--insert into ZONE_GEOGRAPHIQUE values('Guernesey','Europe ex Euro')
--insert into ZONE_GEOGRAPHIQUE values('Malaysie','Asie')
--insert into ZONE_GEOGRAPHIQUE values('EMIRATS ARABES UNIS','Autre')
--insert into ZONE_GEOGRAPHIQUE values('AFRIQUE DU SUD', 'Emergents')
--insert into ZONE_GEOGRAPHIQUE values('NIGERIA', 'Emergents')
--insert into ZONE_GEOGRAPHIQUE values('TURQUIE', 'Emergents')
--insert into ZONE_GEOGRAPHIQUE values('KAZAKHSTAN', 'Emergents')
--insert into ZONE_GEOGRAPHIQUE values('KENYA', 'Emergents')
--insert into ZONE_GEOGRAPHIQUE values('ISRAEL', 'Emergents')
insert into ZONE_GEOGRAPHIQUE values('SLOVENIE', 'Zone Euro')
insert into ZONE_GEOGRAPHIQUE values('CROATIE', 'Europe ex Euro')
insert into ZONE_GEOGRAPHIQUE values('BULGARIE', 'Europe ex Euro')

--- Les secteurs et sous secteurs sont coherents avec le type de produit
select *  from PTF_FGA where  DateInventaire = '30/06/2015' and type_produit like 'Obligation%' and Secteur not in 
(select libelle from SECTEUR where id like 'O %' and id not like '%_OLD')

select *  from PTF_FGA where  DateInventaire = '30/06/2015' and type_produit like 'Obligation%' and sous_Secteur not in 
(select libelle from SOUS_SECTEUR where id like 'O %' and id not like '%_OLD')

select *  from PTF_FGA where  DateInventaire = '30/06/2015' and type_produit like 'Action%' and Secteur not in 
(select libelle from SECTEUR where id like 'A %' and id not like '%_OLD')

select *  from PTF_FGA where  DateInventaire = '30/06/2015' and type_produit like 'Action%' and sous_Secteur not in 
(select libelle from SOUS_SECTEUR where id like 'A %' and id not like '%_OLD')


------------------------------
-- FIN CONTROLE INVENTAIRE OMEGA
------------------------------
select * from PTF_PROXY where Date ='30/06/2015'
select * from PTF_PARAM_PROXY where Date ='30/06/2015'

update PTF_PROXY set Poids_VB = 0.7133 where date = '31/12/2012' and code_proxy = 'CERT_EUR'
 and code_titre ='MCXE'
update PTF_PROXY set Poids_VB = 0.2867 where date = '31/12/2012' and code_proxy = 'CERT_EUR'
and code_titre ='SCXE'
update PTF_PROXY set Poids_VB = 0.6866 where date = '31/12/2012' and code_proxy = 'CERT_exEUR' 
and code_titre ='MCXA'
update PTF_PROXY set Poids_VB = 0.3134 where date = '31/12/2012' and code_proxy = 'CERT_exEUR' 
and code_titre ='SCXA'

