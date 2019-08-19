------------------------------------------------------*
-- Thomson Reuters EIKON : Portfolio Analytics , extraction pour intégration
-- extraction de l historique des fonds de Hubert
-- 6100001 FEDERIS ACTIONS 
-- 6100002 CAC40
-- 6100033 FEDERIS IRC ACTIONS
-- ACT_001 PTF MODELE GARP n°1
-------------------------------------------------------
-- Traitement des Preferred Equity  
-- FR0000120073	AIR LIQUIDE
-- a la place de
-- FR0011336254	AIR LIQUIDE PDF 2015

-- FR0000120321	L'OREAL
-- a la place de
-- FR0011356229	L'OREAL PDF 2015
-------------------------------------------------------

declare @dateInventaire as datetime
set  @dateInventaire = '08/01/2014'
--select max(date) from PTF_AN where compte ='6100001'
--select * from PTF_FGA where Dateinventaire =@dateInventaire 
--select * from PTF_FGA where Compte = 'ACT001'


create table #PORTFOLIO_LIST ( groupe char(15) COLLATE DATABASE_DEFAULT,code char(20)  COLLATE DATABASE_DEFAULT, compte char(10) COLLATE DATABASE_DEFAULT)
insert into  #PORTFOLIO_LIST (groupe,code, compte) VALUES ('FGA_ACTIONS','FEDERISFRANCE','6100002') 
insert into  #PORTFOLIO_LIST (groupe,code, compte) VALUES ('FGA_ACTIONS','FEDERISIRCACTIONS','6100033') 
insert into  #PORTFOLIO_LIST (groupe,code, compte) VALUES ('FGA_ACTIONS','FEDERISACTIONS','6100001') 
insert into  #PORTFOLIO_LIST (groupe,code, compte) VALUES ('FGA_ACTIONS','FEDERISMODELEGARP1','ACT001') 


create table #PROXY_ISIN ( ISINtarget char(15)  COLLATE DATABASE_DEFAULT ,libelleTarget varchar(60)  COLLATE DATABASE_DEFAULT, ISINsource char(15)  COLLATE DATABASE_DEFAULT, libelleSource varchar(60)  COLLATE DATABASE_DEFAULT)
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR',NULL,'Cash OPCVM','Liquidité sur le fond') 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR','Cash Future VOZ3','VOZ3_cash','Liquidité(Future)') 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR','Cash Future VGZ3','VGZ3_cash','Liquidité(Future)') 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR','Cash Future CAZ3','CAZ3_cash','Liquidité(Future)') 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR','Cash Future CFX3','CFX3_cash','Liquidité(Future)') 

insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR',NULL,'DBKGR1213C40_C',NULL) 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR',NULL,'CP1FP1113C50_C',NULL) 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR',NULL,'BN1FP1213C60_C',NULL) 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR',NULL,'RTZGR1113C33_C',NULL) 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR',NULL,'BN1FP1213C60_C',NULL) 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR',NULL,'MTNA1213C135_C',NULL) 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('EUR',NULL,'CP1FP1213C50_C',NULL) 


insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('FR0000120073',NULL,'FR0011336254','AIR LIQUIDE PDF 2015') 
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('FR0000120321',NULL,'FR0011356229','L''OREAL PDF 2015') 

insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('FR0000131104',' OPTION SUR BNP PARIBAS','BN1FP1213C60','BNP PARIBAS')
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('FR0000125338',' OPTION SUR CAP GEMINI SA','CP1FP1113C50','CAP GEMINI SA')
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('FR0000125338',' OPTION SUR CAP GEMINI SA','CP1FP1213C50','CAP GEMINI SA')
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('DE0005140008',' OPTION SUR DEUTSCHE BANK','DBKGR1213C40','DEUTSCHE BANK')
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('FR0000120073',' OPTION SUR AIR LIQUIDE','FR0011336254','AIR LIQUIDE')
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('FR0000120321',' OPTION SUR L''OREAL','FR0011356229','L''OREAL')
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('LU0323134006',' OPTION SUR ARCELORMITTAL','MTNA1213C135','ARCELORMITTAL')
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('NL0000009132',' OPTION SUR AKZO NOBEL','NL0010448924','AKZO NOBEL')
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('NL0000009355',' OPTION SUR UNILEVER NV','NL0010448973','UNILEVER NV')
insert into  #PROXY_ISIN (ISINtarget, libelleTarget, ISINsource,libelleSource) VALUES ('GB0007188757',' OPTION SUR RIO TINTO','RTZGR1113C33','RIO TINTO')


-- modele de fichier CSV à charger pour les ptfs d Hubert dans le groupe FGA_Actions
select 
ptf.groupe as groupe,
ptf.code as code_ptf,
convert (char(10),dateInventaire,103) as date_ptf,
case 
when proxy.ISINtarget is not null then proxy.ISINtarget
else code_titre end as ISIN,
case 
when type_produit = 'Cash'  then 'CASH'
when type_produit like 'FUTURES %' then type_produit
when grp_emetteur ='Société de Gestion de Portefeuille' then emetteur
else grp_emetteur end as issuer,
case
when proxy.libelleTarget is not null then proxy.libelleTarget
else Libelle_Titre end as label,
Valeur_Boursiere as price, 
case 
when type_produit = 'Cash'  then 1
else quantite end as quantity 
from PTF_FGA as fga
left outer join #PORTFOLIO_LIST as ptf on ptf.compte = fga.compte
left outer join #PROXY_ISIN as proxy on proxy.ISINsource = fga.code_titre
where fga.dateInventaire = @dateInventaire and ptf.groupe is not null 
--and ptf.code = 'FEDERISFRANCE'
order by DateInventaire,code_ptf,issuer desc

drop table #PORTFOLIO_LIST
drop table #PROXY_ISIN

--='6100033'

--='6100002'
