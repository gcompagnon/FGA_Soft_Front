
--------------------------------------------------------------------------------
-- Creation des proxy sur chaque derive dans PTF_FGA:
--           option sur sous jacent Indices => proxy sur modèle  Indice
--           future => proxy sur un modèle défini dans la table #TRANSCO_DERIVE_PROXY
--------------------------------------------------------------------------------
--------------------------------------------------------------------------------
-- date d inventaire
declare @dateTransparence datetime
set @dateTransparence = '30/06/2015'

--------------------------------------------------------------------------------
-- Table de correspondance entre le libellé du derive et le proxy à utiliser
create table #TRANSCO_DERIVE_PARAM_PROXY ( libelle_titre varchar(60) collate database_default,  code_proxy varchar(60) collate database_default, Libelle_Proxy varchar(60) collate database_default)
-- Table de correspondance entre le libellé du derive et le proxy à utiliser
CREATE TABLE #TRANSCO_DERIVE_PROXY ( proxy_type varchar(20) collate database_default not null,[suffixe_code_titre] [varchar](20) collate database_default NOT NULL,[suffixe_Libelle_Titre] [varchar](60) collate database_default NULL,[Poids_VB] [float] NULL,	[Poids_CC] [float] NULL,[Type_Produit] [varchar](60) collate database_default NULL,	[Devise_Titre] [varchar](3) collate database_default NULL,	[Secteur] [varchar](60) collate database_default NULL,	[Sous_Secteur] [varchar](60) collate database_default NULL,	[Pays] [varchar](30) collate database_default NULL,	[Emetteur] [varchar](60) collate database_default NULL,	[Rating] [varchar](4) collate database_default NULL,	[Groupe_Emet] [varchar](60) collate database_default NULL,	[Maturité] [datetime] NULL,	[Duration] [float] NULL,	[Sensibilite] [float] NULL)
--------------------------------------------------------------------------------
--------------------------------------------------------------------------------
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'Euro Stoxx 50', 'MSCI_EMU_LargeCap','MSCI650008 proxy pour SX5E/STOXX 50 EURO')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'EURO STOXX 50 Price EUR', 'MSCI_EMU_LargeCap','MSCI650008 proxy pour SX5E/STOXX 50 EURO')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'DJ Euro Stoxx Banks Price', 'MSCI_E_BANKS','MXEM0BK proxy pour STOXX EURO BANQUE')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'EURO STOXX Banks Price EUR', 'MSCI_E_BANKS','MXEM0BK proxy pour STOXX EURO BANQUE')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'STANDARD & POOR 500', 'MSCI_US','MXUS proxy pour SP500NET')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'EURO STOXX FOOD AND BEVERAGE', 'MSCI_E_FOOD_BEV', 'MXEM0FB proxy pour STOXX EURO ALIMENTATION')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'FTSE 100', 'MSCI_UK', 'MXGB proxy pour Footsie 100')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'NIKKEI 225 STOCK AVERAGE', 'NKY', 'NIKKEI 225 STOCK AVERAGE')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'STOXX 600 BASIC RESOURCE EUR PR', 'MSCI_Eu_MATERIALS', 'MXEU0ML proxy pour STOXX 600 PRODUITS DE BASE')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'DAX', 'MSCI_DE', 'M3DE proxy pour DAX 30')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'STOXX 600 AUTOMOBILES ET PART', 'MSCI_Eu_AUTOPARTS', 'MXEU0AC proxy pour STOXX 600 AUTO')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'STOXX 600 OIL & GAS', 'MSCI_Eu_ENERGY', 'MXEU0EG proxy pour STOXX 600 OIL')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'EURO STOXX Oil & Gas Price EUR', 'MSCI_Eu_ENERGY', 'MXEU0EG proxy pour STOXX 600 OIL')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'CAC 40', 'MSCI_FR', 'MSCI650018 proxy pour CAC40')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'CAC 40 Index', 'MSCI_FR', 'MSCI650018 proxy pour CAC40')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'EURO STOXX TELECOMS PRICE', 'MSCI_E_TELECOM', 'MXEM0TS proxy pour EURO STOXX TELECOMS')
insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'EURO STOXX Telecommunications', 'MSCI_E_TELECOM', 'MXEM0TS proxy pour EURO STOXX TELECOMS')



-- 
--insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'Future sur Dividendes Eurostoxx50 12/2015', 'FUTURES_DIV', 'Future sur Dividendes')
--insert into #TRANSCO_DERIVE_PARAM_PROXY VALUES( 'FUTURE EUROSTOXX BANKS DEC 13', 'FUTURES_STX_BANK', 'Future sur StoxxBank')

-- MODELE POUR LES FUTURES SUR DIVIDEND
insert into #TRANSCO_DERIVE_PROXY VALUES( 'FUTURES_DIV','_A',' - Actions',0.25,0,'FUTURES ACTIONS EN EURO','EUR','FONDS ACTIONS','OPCVM ACTIONS EURO','Allemagne', 'N/A', NULL,'N/A',NULL,NULL,NULL)
insert into #TRANSCO_DERIVE_PROXY VALUES( 'FUTURES_DIV','_O',' - Taux',0.75,0,'FUTURES ACTIONS EN EURO','EUR','FONDS OBLIGATIONS','OPCVM OBLIGATIONS EURO','Allemagne', 'N/A', NULL,'N/A',NULL,NULL,NULL)

--------------------------------------------------------------------------------
-- DEBUT TRAITEMENT - NE PAS MODIFIER
--------------------------------------------------------------------------------

select distinct code_titre, libelle_titre, emetteur as 'ssjacent'
into #LIST_OPTIONS
from ptf_fga 
where        dateinventaire =@dateTransparence and 
               ( type_produit='BENCHMARK' or Type_Produit like 'OPTION%' )
               and left(rtrim(code_titre),12) not in (select distinct isin_titre from ptf_param_proxy where date=@dateTransparence)


select distinct code_titre, libelle_titre ,emetteur as 'ssjacent'
into #LIST_FUTURES
from ptf_fga 
where        dateinventaire =@dateTransparence and 
               type_produit like 'FUTURES %' 
               and left(rtrim(code_titre),12) not in (select distinct isin_titre from ptf_param_proxy where date=@dateTransparence)
-- Les futures: parametrage avec un code_titre
insert into PTF_PARAM_PROXY
select distinct @dateTransparence, left(rtrim(code_titre),12), d.libelle_titre, left(rtrim(code_titre),12), d.libelle_titre, 'PROXY' as Source
from #LIST_FUTURES as d
left outer join #TRANSCO_DERIVE_PARAM_PROXY as transco on transco.libelle_titre = d.libelle_titre  or transco.libelle_titre = d.ssjacent
where transco.libelle_titre is not null

insert into PTF_PROXY
select distinct @dateTransparence as Date, d.code_titre as Code_proxy, 
 d.libelle_titre as Libelle_Proxy, left(rtrim(code_titre),12)+transco_proxy.[suffixe_code_titre] as code_titre,
 d.libelle_titre+transco_proxy.suffixe_libelle_titre as Libelle_Titre, [Poids_VB],[Poids_CC],[Type_Produit],[Devise_Titre],[Secteur],
 [Sous_Secteur],[Pays],[Emetteur],[Rating],[Groupe_Emet],[Maturité],[Duration],[Sensibilite]
from #LIST_FUTURES as d
left outer join #TRANSCO_DERIVE_PARAM_PROXY as param_proxy on param_proxy.libelle_titre = d.libelle_titre or param_proxy.libelle_titre = d.ssjacent
left outer join #TRANSCO_DERIVE_PROXY as transco_proxy on transco_proxy.proxy_type = param_proxy.code_proxy
where param_proxy.libelle_titre is not null


-- liste des options sans definition disponible
select distinct code_titre, 'Option sur ' + d.libelle_titre as 'Libelle_titre', ssjacent,'PROXY OPTION A DEFINIR dans la table transco #TRANSCO_DERIVE_PARAM_PROXY de la requete SQL' as erreur
from #LIST_OPTIONS as d
where d.libelle_titre not in (select libelle_titre from #TRANSCO_DERIVE_PARAM_PROXY)
and d.ssjacent not in (select libelle_titre from #TRANSCO_DERIVE_PARAM_PROXY)
union
select distinct code_titre, d.libelle_titre as 'Libelle_titre', ssjacent, 'PROXY A DEFINIR dans la table transco #TRANSCO_DERIVE_PARAM_PROXY et TRANSCO_DERIVE_PROXY de la requete SQL' as erreur
from #LIST_FUTURES as d
where d.libelle_titre not in (select libelle_titre from #TRANSCO_DERIVE_PARAM_PROXY)
and d.ssjacent not in (select libelle_titre from #TRANSCO_DERIVE_PARAM_PROXY)



-- Les options: parametrage sur un proxy existant
insert into PTF_PARAM_PROXY
select distinct @dateTransparence, left(rtrim(code_titre),12), 'Option sur '+d.libelle_titre, transco.code_proxy, transco.Libelle_Proxy, 'PROXY' as Source
from #LIST_OPTIONS as d
left outer join #TRANSCO_DERIVE_PARAM_PROXY as transco on transco.libelle_titre = d.libelle_titre or transco.libelle_titre = d.ssjacent
where transco.libelle_titre is not null

/*
pour traiter le pbme des code option trop court
select * from #LIST_OPTIONS
delete from #LIST_OPTIONS where code_Titre ='SX5E 1215P2750'


-- Les options: parametrage sur un proxy existant
insert into PTF_PARAM_PROXY
select distinct '30/06/2015', left(rtrim(code_titre),12), 'Option sur '+d.libelle_titre, transco.code_proxy, transco.Libelle_Proxy, 'PROXY' as Source
from #LIST_OPTIONS as d
left outer join #TRANSCO_DERIVE_PARAM_PROXY as transco on transco.libelle_titre = d.libelle_titre or transco.libelle_titre = d.ssjacent
where transco.libelle_titre is not null


insert into PTF_PARAM_PROXY
select distinct '30/06/2015', 'SX5E 1215P2_', 'Option sur '+d.libelle_titre, transco.code_proxy, transco.Libelle_Proxy, 'PROXY' as Source
from #LIST_OPTIONS as d
left outer join #TRANSCO_DERIVE_PARAM_PROXY as transco on transco.libelle_titre = d.libelle_titre or transco.libelle_titre = d.ssjacent
where transco.libelle_titre is not null
*/

drop table #TRANSCO_DERIVE_PARAM_PROXY
drop table #TRANSCO_DERIVE_PROXY
drop table #LIST_OPTIONS
drop table #LIST_FUTURES


--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
--VALUES ('31/01/2014', 'NKY', 'NIKKEI 225 STOCK AVERAGE', 'NKY', 'NIKKEI 225 STOCK AVERAGE', 'PROXY')

--INSERT INTO [dbo].[PTF_PROXY] ([Date],[Code_Proxy],[Libelle_Proxy],[code_titre],[Libelle_Titre],[Poids_VB]
--           ,[Poids_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur],[Pays],[Emetteur],[Rating],[Groupe_Emet])
--           --,[Maturité],[Duration],[Sensibilite])
--     VALUES
--           ('31/01/2014' ,'NKY','NIKKEI 225 STOCK AVERAGE','NKY','NIKKEI 225 STOCK AVERAGE - Actions',1,0,'Actions Japon','JPY','FONDS ACTIONS','OPCVM ACTIONS ASIE PACIFIQUE',
--           'Japon','N/A',NULL,'N/A')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
--VALUES ('31/01/2014', 'SXPP', 'STOXX 600 BASIC RESOURCE EUR PR', 'SXPP', 'STOXX 600 BASIC RESOURCE EUR PR', 'PROXY')

--INSERT INTO [dbo].[PTF_PROXY] ([Date],[Code_Proxy],[Libelle_Proxy],[code_titre],[Libelle_Titre],[Poids_VB]
--           ,[Poids_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur],[Pays],[Emetteur],[Rating],[Groupe_Emet])
--           --,[Maturité],[Duration],[Sensibilite])
--     VALUES
--           ('31/01/2014' ,'SXPP','STOXX 600 BASIC RESOURCE EUR PR','SXPP','STOXX 600 BASIC RESOURCE EUR PR - Actions',1,0,'Actions Allemagne','EUR','FONDS ACTIONS','OPCVM ACTIONS EURO',
--           'Allemagne','N/A',NULL,'N/A')
           
    
--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
--VALUES ('22/01/2014', 'FTSE100', 'Footsie', 'FTSE100', 'Footsie 100', 'PROXY')

--INSERT INTO [dbo].[PTF_PROXY] ([Date],[Code_Proxy],[Libelle_Proxy],[code_titre],[Libelle_Titre],[Poids_VB]
--           ,[Poids_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur],[Pays],[Emetteur],[Rating],[Groupe_Emet])
--           --,[Maturité],[Duration],[Sensibilite])
--     VALUES
--           ('22/01/2014' ,'FTSE100','Footsie','FTSE100','Footsie - Actions',1,0,'Actions GB','GBP','FONDS ACTIONS','OPCVM ACTIONS EX EURO',
--           'Royaume-Uni','N/A',NULL,'N/A')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
--VALUES ('31/03/2014', 'DEDZ6', 'Future sur Dividendes Eurostoxx50 12/2016', 'DEDZ6', 'Future sur Dividendes Eurostoxx50 12/2016', 'PROXY')

--INSERT INTO [dbo].[PTF_PROXY] ([Date],[Code_Proxy],[Libelle_Proxy],[code_titre],[Libelle_Titre],[Poids_VB]
--           ,[Poids_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur],[Pays],[Emetteur],[Rating],[Groupe_Emet])
--           --,[Maturité],[Duration],[Sensibilite])
--     VALUES
--           ('31/03/2014' ,'DEDZ6','Future sur Dividendes Eurostoxx50 12/2016','DEDZ6_A','Future sur Dividendes Eurostoxx50 12/2016 - Actions',0.2500,0,'FUTURES ACTIONS EN EURO','EUR','FONDS ACTIONS','OPCVM ACTIONS EURO',
--           'Allemagne','N/A',NULL,'N/A')
           
--INSERT INTO [dbo].[PTF_PROXY] ([Date],[Code_Proxy],[Libelle_Proxy],[code_titre],[Libelle_Titre],[Poids_VB]
--           ,[Poids_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur],[Pays],[Emetteur],[Rating],[Groupe_Emet])
--           --,[Maturité],[Duration],[Sensibilite])
--     VALUES
--           ('31/03/2014' ,'DEDZ6','Future sur Dividendes Eurostoxx50 12/2016','DEDZ6_O','Future sur Dividendes Eurostoxx50 12/2016 - Taux',0.7500,0,'FUTURES ACTIONS EN EURO','EUR','FONDS OBLIGATIONS','OPCVM OBLIGATIONS EURO',
--           'Allemagne','N/A',NULL,'N/A')
--INSERT INTO [dbo].[PTF_PROXY] ([Date],[Code_Proxy],[Libelle_Proxy],[code_titre],[Libelle_Titre],[Poids_VB]
--           ,[Poids_CC],[Type_Produit],[Devise_Titre],[Secteur],[Sous_Secteur],[Pays],[Emetteur],[Rating],[Groupe_Emet])
--           --,[Maturité],[Duration],[Sensibilite])
--     VALUES
--           ('01/10/2014' ,'DAX','DAX','DAX_INDEX','DAX - Actions',1,0,'Actions Allemagne','EUR','FONDS ACTIONS','OPCVM ACTIONS EURO',
--           'Allemagne','N/A',NULL,'N/A')

--insert into PTF_PROXY VALUES ('28/11/2014','SXXP_AUTOM','DJS 600 Pr - AUTOMOBILE','SXXP_AUTOM_A','DJS 600 Pr - AUTOMOBILE',1,0,
--'Actions Allemagne','EUR','Consommation Discretionnaire','Constructeurs Automobiles','Allemagne','N/A',NULL,'Société de Gestion de Portefeuille',NULL,NULL,NULL)

--insert into PTF_PROXY VALUES ('28/11/2014','SXEP','STOXX 600 OIL & GAS','SXEP_A','STOXX 600 OIL & GAS',1,0,
--'Actions Allemagne','EUR','Energie','Compagnies Gazieres et Petrolieres Integ','Allemagne','N/A',NULL,'Société de Gestion de Portefeuille',NULL,NULL,NULL)

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
--VALUES ('28/11/2014', 'SXZ4', 'Future sur SXXP AUTOM 12/2014', 'SXXP_AUTOM', 'DJS 600 Pr - AUTOMOBILE 12/2014', 'PROXY')
	
--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'CAM5', 'FUTURE SUR EUROSTOXX BANKS JUN15', 'MSCI_Eu_BANKS', 'MXEU0BK proxy pour STOXX 600 BANQUE', 'PROXY')
--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'ESM5', 'FUTURE EMINI S&P500 JUN15', 'MSCI_US', 'MXUS proxy pour SP500NET', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'GXM5', 'FUTURE DAX JUN15', 'MSCI_DE', 'M3DE proxy pour DAX 30', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'ITM5', 'FUTURE SUR EUROSTOXX UTILITIES JUN15', 'MSCI_Eu_UTILITIES', 'MXEU0UL proxy pour STOXX 600 SERVICES AUX COLLECTIVITES', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'JSM5', 'FUTURE SUR EUROSTOXX BASIC RESOURCES JUN15', 'MSCI_Eu_MATERIALS', 'MXEU0ML proxy pour STOXX 600 PRODUITS DE BASE', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'QRM5', 'FUTURE SUR EUROSTOXX OIL&GAS JUN15', 'MSCI_E_ENERGY', 'MXEM0EG proxy pour STOXX EURO OIL', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'STM5', 'FUTURE SUR FTSE/MIB JUN15', 'FTSE_MIB', 'Borsa Italiana 40', 'PROXY')
--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'SXM5', 'FUTURE AUTO ET PIECES STOXX 600 JUN15', 'MSCI_Eu_AUTOPARTS', 'MXEU0AC proxy pour STOXX 600 AUTO', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'VGM5', 'FUTURE SUR EUROSTOXX 50 JUN15', 'MSCI_EMU_LargeCap', 'MSCI650008 proxy pour SX5E/STOXX 50 EURO', 'PROXY')
--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('31/03/2015', 'VOM5', 'FUTURE SUR EUROSTOXX ASSURANCE JUN15', 'MSCI_Eu_INSURANCE', 'MXEU0IS proxy pour STOXX 600 ASSURANCE', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES('31/03/2015','FR0010296061','LYXOR MSCI USA', 'MSCI_US', 'MXUS proxy pour SP500NET','PROXY')
 
-- insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES('31/03/2015','FR0011645647','LYXOR EStoxx BANK','MSCI_E_BANKS','MXEM0BK proxy pour STOXX EURO BANQUE','PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/04/2015', 'QOM5', 'FUTURE STOXX 600 TRAVEL & LEISURE JUN15', 'SXXP_TRAV', 'SXXP_TRAV proxy pour TRAVEL & LEISURE', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/04/2015', 'CFM5', 'FUTURE CAC40 10 EURO JUN15', 'MSCI_FR', 'MSCI650018 proxy pour CAC40', 'PROXY')
--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/04/2015', 'Z M5', 'FUTURE FTSE 100 INDEX JUN15', 'MSCI_UK', 'MXGB proxy pour Footsie 100', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/04/2015', 'SXEM5', 'FUTURE EURO STOXX SMALL JUN15', 'SCXE', 'STOXX SMALL CAP EURO', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('29/05/2015', 'SRIM5', 'FUTURE EURO STOXX REAL ESTATE JUN15', 'MSCI_E_REALESTATE', 'MXEM0RE proxy pour EURO STOXX REALESTATE', 'PROXY')
--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/06/2015', 'VGU5', 'FUTURE SUR EUROSTOXX 50 SEP15', 'MSCI_EMU_LargeCap', 'MSCI650008 proxy pour SX5E/STOXX 50 EURO', 'PROXY')
	

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/06/2015', 'AWU5', 'FUTURE SUR EUROSTOXX CONSTRUCTION SEP15', 'MSCI_E_REALESTATE', 'MXEM0RE proxy pour EURO STOXX REALESTATE', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/06/2015', 'ESU5', 'FUTURE EMINI S&P500 SEP15', 'MSCI_US', 'MXUS proxy pour SP500NET', 'PROXY')


--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/06/2015', 'GXU5', 'FUTURE DAX SEP15', 'MSCI_DE', 'M3DE proxy pour DAX 30', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/06/2015', 'QOU5', 'FUTURE STOXX 600 TRAVEL & LEISURE SEP15', 'SXXP_TRAV', 'SXXP_TRAV proxy pour TRAVEL & LEISURE', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/06/2015', 'SXEU5', 'FUTURE EURO STOXX SMALL SEP15', 'SCXE', 'STOXX SMALL CAP EURO', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/06/2015', 'Z U5', 'FUTURE FTSE 100 INDEX SEP15', 'MSCI_UK', 'MXGB proxy pour Footsie 100', 'PROXY')

--insert into PTF_PARAM_PROXY (date, isin_titre, libellé_titre, code_proxy, libellé_proxy, source )
-- VALUES ('30/06/2015', 'SXU5', 'FUTURE AUTO ET PIECES STOXX 600 SEP15', 'MSCI_Eu_AUTOPARTS', 'MXEU0AC proxy pour STOXX 600 AUTO', 'PROXY')
	
	
-- FUTURE A FAIRE EN PROXY DANS PTF_PARAM_PROXY
--BTAH5 BTAH5 MIDTERM EURO-OAT FUTURE MAR15

--CFH5 CFH5 FUTURE CAC40 10 EURO MAR15

--DUH5 DUH5 EURO-SCHATZ FUTURE MAR15

--ECH5 ECH5 FUTURE CHANGE EURO/USD MAR15


--FVH5 FVH5 FUTURE US 5 ANS MAR15

--G H5 G H5 LONG GILT FUTURE MAR1


--IKH5 IKH5 FUTURE EURO-BTP MAR15


--JBH5 JBH5 FUTURE JAPON 10 ANS MAR15

--OATH5 OATH5 EURO-OAT FUTURE MAR15

--OEH5 OEH5 EURO-BOBL FUTURE MAR15

--RPH5 RPH5 FUTURE DEVISE EUR/GBP MAR15

--RXH5 RXH5 EURO-BUND FUTURE MAR15

--RYH5 RYH5 FUTURE CHANGE EURO/YEN MAR15

--TYH5 TYH5 FUTURE US 10 ans MAR15
