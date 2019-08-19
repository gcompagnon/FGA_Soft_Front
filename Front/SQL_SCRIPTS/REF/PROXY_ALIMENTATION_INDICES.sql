----------------
-- EXTRACTION DES INDICES POUR ALIMENTER LES PROXY
-- Parametre: Date des indices (MSCI pour les Actions et Barclays pour les Gov) , et date utilisée pour le proxy
----------------

declare @dateIndicesMSCI datetime
set @dateIndicesMSCI = '30/06/2015'

declare @dateIndicesBarclays datetime
set @dateIndicesBarclays = @dateIndicesMSCI

declare @dateProxy datetime
set @dateProxy = @dateIndicesMSCI

--select * from PTF_PROXY where Date = '28/11/2014' and Code_Proxy  ='SP500NET'
--select @dateIndicesMSCI,@dateIndicesBarclays,@dateProxy

-------------------------------------------------------------------
-- LISTE DES MAPPINGS PROXY: Pour les Indices Gov / EuroMts
-------------------------------------------------------------------

create TABLE #CONFIG_PROXY_GOV (code_proxy nvarchar(20), libelle_proxy nvarchar(60),codeIndex [nvarchar](12), holdingType nvarchar(20))

INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_ALL','BT11TREU proxy pour EuroMtS Global','BT11TREU','BARCLAYS_RETURN')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_1_3','BTS1TREU proxy pour EuroMtS 1-3','BTS1TREU','BARCLAYS_RETURN')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_3_5','BTS3TREU proxy pour EuroMtS 3-5','BTS3TREU','BARCLAYS_RETURN')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_5_7','BTS5TREU proxy pour EuroMtS 5-7','BTS5TREU','BARCLAYS_RETURN')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_7_10','BT7YTREU proxy pour EuroMtS 7-10','BT7YTREU','BARCLAYS_RETURN')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_10_15','BT10TREU proxy pour EuroMtS 10-15','BT10TREU','BARCLAYS_RETURN')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_15+','BT15TREU proxy pour EuroMtS 15+','BT15TREU','BARCLAYS_RETURN')

-------------------------------------------------------------------
---- MAPPING PROXY ACTIONS
-------------------------------------------------------------------

create TABLE #CONFIG_PROXY_ACTION (code_proxy nvarchar(20), libelle_proxy nvarchar(60),codeIndex [nvarchar](12), holdingType nvarchar(20))

INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_US','MXUS proxy pour SP500NET','MXUS','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Europe','MXEU proxy pour SXXP/STOXX EUROPE','MXEU','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_EMU_LargeCap','MSCI650008 proxy pour SX5E/STOXX 50 EURO','MSCI650008','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_EMU','MXEM proxy pour SXXE/STOXX EURO','MXEM','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_ExEMU','MXEUM proxy pour SXXA/STOXX EUROPE ex Euro','MXEUM','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_EMU_MidCap','MSCI652525 proxy pour MCXE/STOXX MID CAP EURO','MSCI652525','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_exEMU_MidCap','MSCI652528 proxy pour MCXA/STOXX MID CAP EX-EURO','MSCI652528','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_UK','MXGB proxy pour Footsie 100','MXGB','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_DE','M3DE proxy pour DAX 30','M3DE','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_FR','MSCI650018 proxy pour CAC40','MSCI650018','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_ENERGY','MXEU0EG proxy pour STOXX 600 OIL','MXEU0EG','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_AUTOPARTS','MXEU0AC proxy pour STOXX 600 AUTO','MXEU0AC','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_BANKS','MXEU0BK proxy pour STOXX 600 BANQUE','MXEU0BK','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_HEALTH_CARE','MXEU0HC proxy pour STOXX 600 SANTE','MXEU0HC','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_CONSUMER_SVC','MXEU0HR proxy pour STOXX 600 B&S CONSO','MXEU0HR','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_INSURANCE','MXEU0IS proxy pour STOXX 600 ASSURANCE','MXEU0IS','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_FOOD_BEV','MXEU0FB proxy pour STOXX 600 ALIMENTATION','MXEU0FB','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_MATERIALS','MXEU0ML proxy pour STOXX 600 PRODUITS DE BASE','MXEU0ML','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_UTILITIES','MXEU0UL proxy pour STOXX 600 SERVICES AUX COLLECTIVITES','MXEU0UL','MSCI_SI')

INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_ENERGY','MXEM0EG proxy pour STOXX EURO OIL','MXEM0EG','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_AUTOPARTS','MXEM0AC proxy pour STOXX EURO AUTO','MXEM0AC','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_BANKS','MXEM0BK proxy pour STOXX EURO BANQUE','MXEM0BK','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_HEALTH_CARE','MXEM0HC proxy pour STOXX EURO SANTE','MXEM0HC','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_CONSUMER_SVC','MXEM0HR proxy pour STOXX EURO B&S CONSO','MXEM0HR','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_INSURANCE','MXEM0IS proxy pour STOXX EURO ASSURANCE','MXEM0IS','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_FOOD_BEV','MXEM0FB proxy pour STOXX EURO ALIMENTATION','MXEM0FB','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_MATERIALS','MXEM0ML proxy pour STOXX EURO PRODUITS DE BASE','MXEM0ML','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_UTILITIES','MXEM0UL proxy pour STOXX EURO SERVICES AUX COLLECTIVITES','MXEM0UL','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_TELECOM','MXEM0TS proxy pour EURO STOXX TELECOMS','MXEM0TS','MSCI_SI')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_REALESTATE','MXEM0RE proxy pour EURO STOXX REALESTATE','MXEM0RE','MSCI_SI')

-------------------------------------------------------------------
-- TABLE PROXY de travail
-------------------------------------------------------------------

CREATE TABLE #INDEX_COMPO(
	source [nvarchar](10) NOT NULL,
	code_proxy [nvarchar](20) NOT NULL,
    libelle_proxy [nvarchar](60) NOT NULL,
	IndexDate datetime NOT NULL,
	IndexCode [nvarchar](12) NOT NULL,	
	ISIN [nvarchar](12) NOT NULL,
	[AssetId] [bigint] NOT NULL,
	MarketValue float,
	MarketValue_Cur [nchar](4),
	MarketValueTotal float,
	MarketValueTotal_Cur [nchar](4),
	IndexTotal int,
    FaceAmount float,
	FaceAmount_Cur [nchar](4),
    BookValue float,
	BookValue_Cur [nchar](4),
	[Quantity] [real] NULL,
	[Weight] [float] NULL
	)

-------------------------------------------------------------------
-- PRE TRAITEMENT : MISE EN PLACE DES TRANSCO FGA
-------------------------------------------------------------------
--TRANSCO

create TABLE #TRANSCO_TYPE_PRODUIT (iso char(2), pays varchar(60) , type_produit varchar(60) not null) 
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('FR','France','Actions France')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('DE','Allemagne','Actions Allemagne')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('NL','Pays-Bas','Actions Pays-bas')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('FI','Finlande','Actions Finlande')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('AT','Autriche','Actions Autriche')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('PT','Portugal','Actions Portugal')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('IT','Italie','Actions Italie')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('IE','Irlande','Actions Irlande')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('GR','Grèce','Actions Grèce')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('ES','Espagne','Actions Espagne')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('BE','Belgique','Actions Belgique')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('LU','Luxembourg','Actions Luxembourg')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('DK','Danemark','Actions Danemark')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('GB','Royaume-Uni','Actions GB')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('SE','Suède','Actions Suède')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('NO','Norvège','Actions Norvège')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('CH','Suisse','Actions Suisse')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('US','Etats-Unis','Actions US')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('AU','Australie','Actions Australie')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('CA','Canada','Actions Canada')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('JP','Japon','Actions Japon')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('XX','NON ATTRIBUE','Action')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('IM','Royaume-Uni','Actions GB')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('KY','Ile Cayman','Actions Ile Cayman')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('HK','Hong-Kong','Actions Hong-Kong')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('BM','Bermudes','Actions Bermudes')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('CN','Chine','Actions Chine')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('CY','Grèce','Actions Grèce')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('JE','Royaume-Uni','Actions GB')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('SG','Singapour','Actions Singapour')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('KR','Corée','Actions Coree Sud')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('SK','Slovaquie','Actions Slovaquie')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('TW','Taiwan','Actions taiwan')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('GG','Guernesey','Actions GB')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('IN','Inde','Actions Inde')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('MY','Malaysie','Actions Malaysie')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('TH','Thailande','Actions Thailande')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('PH','Philippines','Actions Philippines')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('CZ','République Tchèque','Actions République Tchèque')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('NZ','Nouvelle Zélande','Actions Nouvelle Zélande')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('BR','Brésil','Actions Brésil')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('IS','Islande','Actions Islande')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('HU','Hongrie','Actions Hongrie')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('AE','EMIRATS ARABES UNIS','Actions EAU')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('ZA','AFRIQUE DU SUD','Actions Afrique du Sud')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('KE','KENYA','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('NG','NIGERIA','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('RU','RUSSIE','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('TR','TURQUIE','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('KZ','KAZAKHSTAN','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('BG','BULGARIE','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('HR','CROATIE','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('MX','MEXIQUE','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('SI','SLOVENIE','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('QA','QATAR','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('OM','OMAN','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('IL','Israel','Actions Israel')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('EG','Egypte','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('ID','Indonesie','Actions')
INSERT INTO #TRANSCO_TYPE_PRODUIT VALUES ('PL','Pologne','Actions')

create TABLE #TRANSCO_SOUS_SECTEUR (msci_code char(8), sous_secteur varchar(60) not null) 
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('10101010','Forage gazier et petrolier')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('10101020','Equipement et Services pour l industrie')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('10102010','Compagnies Gazieres et Petrolieres Integ')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('10102020','Exploration et Production de Petrole et')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('10102030','Raffinage et Commercialisation de Petrol')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('10102040','Stockage et Transport de Petrole et de G')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('10102050','Charbon et Combustibles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15101010','Produits Chimiques de Base')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15101020','Produits Chimiques Diversifies')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15101030','Engrais et Produits Chimiques Agricoles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15101040','Gaz Industriels')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15101050','Produits Chimiques Specialises')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15102010','Materiels de Construction')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15103010','Conteneurs en Verre ou en Metal')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15103020','Emballages en Papier')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15104010','Aluminium')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15104020','Metaux Diversifies et Exploitation Minie')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15104030','Or')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15104040','Metaux et Mineraux Precieux')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15104045','Argent')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15104050','Acier')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15105010','Industrie du Bois')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('15105020','Produits a Base de Papier')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20101010','Industrie Aerospatiale et de Defense')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20102010','Produits pour l industrie de Constructio')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20103010','Construction et Ingenierie')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20104010','Composants et equipement electriques')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20104020','Equipement Electriques Lourds')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20105010','Conglomerats Industriels')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20106010','Engins de BTP et Camions')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20106015','Machines Agricoles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20106020','Machines Industrielles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20107010','Societes Commerciales et de Distribution')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20201010','Imprimerie Commerciale')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20201050','Services d Environnement et Lies aux Loc')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20201060','Services et Provisions de Bureau')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20201070','Services d Aide Divers')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20201080','Services de Securite et d Alarme')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20202010','Services Lies aux Ressources Humaines et')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20202020','Services de Recherche et Conseil')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20301010','Courrier Fret Aerien et Logistique')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20302010','Compagnies Aeriennes')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20303010','Transport Maritime')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20304010','Transport Ferroviaire')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20304020','Transport Routier')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20305010','Services Aeroportuaires')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20305020','Autoroutes et Voies Ferrees')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('20305030','Ports et Services Maritimes')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25101010','Pieces Detachees et Equipement d Automob')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25101020','Pneus et Caoutchouc')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25102010','Constructeurs Automobiles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25102020','Constructeurs de Motocycles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25201010','Electronique Grand Public')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25201020','Ameublement')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25201030','Constructions d Habitations Residentiell')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25201040','Appareils Electromenagers')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25201050','Articles Menagers et Divers')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25202010','Produits de Loisirs et Jouets')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25202020','Photographic Products')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25203010','Habillement Accessoires et Produits de')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25203020','Chaussures')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25203030','Textiles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25301010','Casinos et Salles de Jeu')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25301020','Hotels Centres de Vacances et Croisiere')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25301030','Centres de Loisirs')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25301040','Restaurants')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25302010','Services Educatifs')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25302020','Services Clientele Specialises')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25401010','Publicite')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25401020','Radiotelevision')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25401025','Cable et Satellite')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25401030','Films et Divertissements')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25401040','Edition')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25501010','Grossistes')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25502010','Vente au Detail par Catalogue')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25502020','Vente au Detail sur Internet')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25503010','Grands magasins')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25503020','Distribution Diversifiee')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25504010','Vente au Detail de Vetements')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25504020','Vente au Detail de Produits Informatique')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25504030','Vente au Detail de Produits pour la Mais')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25504040','Magasins Specialises')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25504050','Vente au Detail de Produits pour l Autom')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('25504060','Vente au Detail d Ameublement')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30101010','Pharmacie et Distribution de Medicaments')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30101020','Grossistes en Produits Alimentaires')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30101030','Distribution Alimentaires')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30101040','Hypermarches et Super Centres')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30201010','Brasseurs')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30201020','Distillateurs et Negociants de Vins')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30201030','Boissons Non alcoolisees')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30202010','Produits Agricoles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30202030','Aliments et Viandes Conditionnes')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30203010','Tabac')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30301010','Produits Domestiques')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('30302010','Hygiene et Beaute')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35101010','Equipement Medicaux')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35101020','Fournitures Medicales')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35102010','Distribution de Produits de Sante')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35102015','Services de Sante')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35102020','Infrastructures Medicales')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35102030','Gestion Integree des Soins de Sante')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35103010','Technologies des Soins de Sante')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35201010','Biotechnologie')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35202010','Produits Pharmaceutiques')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('35203010','Outils et Services Appliques aux Science')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40101010','Banques Diversifiees')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40101015','Banques Regionales')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40102010','Epargne et Prets Hypothecaires')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40201020','Autres Services Financiers Diversifies')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40201030','Holdings Multisectoriels')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40201040','Institutions Financieres Specialisees')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40202010','Credit a la Consommation')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40203010','Banques de Depot et de Gestion des Biens')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40203020','Services Bancaires d Investissement et C')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40203030','Marches de Capitaux Diversifies')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40301010','Courtiers en Assurance')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40301020','Assurances Vie et Assurances Maladie')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40301030','Assurances Multirisques')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40301040','Assurances de Biens et Assurances de Ris')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40301050','Reassurance')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40402010','SII a Exploitation Diversifiee')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40402020','SII Industrielles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40402030','SII Specialisees en Credit Hypothecaire')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40402035','SII Hotel et centres de vacances')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40402040','SII Bureautiques')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40402045','SII Sante')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40402050','SII Residentielles')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40402060','SII au Detail')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40402070','SII Specialisees')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40403010','Activites Immobilieres Diversifiees')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40403020','Societes d Exploitation de Biens Immobil')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40403030','Promotion Immobiliere')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('40403040','Services Immobiliers')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45101010','Logiciels et Services d Internet')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45102010','Conseils Informatiques et Autres Service')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45102020','Traitement des Donnees et Services Impar')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45103010','Logiciels d Application')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45103020','Logiciels Systeme')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45103030','Logiciel de Divertissement a Domicile')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45201020','Equipement de Telecommunications')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45202010','Computer Hardware')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45202020','Computer Storage & Peripherals')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45202030','Materiel Informatique et Peripheriques')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45203010','Equipement et Instruments electroniques')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45203015','Composants Electroniques')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45203020','Services de Fabrication Electronique')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45203030','Distributeurs de Technologie')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45204010','Office Electronics')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45301010','Equipement de Semi conducteurs')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('45301020','Semi conducteurs')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('50101010','Gerants de Telecommunication Specialises')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('50101020','Services de Telecommunication Integres')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('50102010','Services de Telecommunication Mobile')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('55101010','Electricite')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('55102010','Gaz')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('55103010','Services aux Collectivites')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('55104010','Eau')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('55105010','Producteurs Energie Independants et Comm')
INSERT INTO #TRANSCO_SOUS_SECTEUR VALUES ('55105020','Energie renouvelable')

create TABLE #TRANSCO_SECTEUR (msci_code char(2), secteur varchar(60) not null) 
INSERT INTO #TRANSCO_SECTEUR VALUES ('10','Energie')
INSERT INTO #TRANSCO_SECTEUR VALUES ('15','Materiels')
INSERT INTO #TRANSCO_SECTEUR VALUES ('20','Industrie')
INSERT INTO #TRANSCO_SECTEUR VALUES ('25','Consommation Discretionnaire')
INSERT INTO #TRANSCO_SECTEUR VALUES ('30','Biens de Consommation de Base')
INSERT INTO #TRANSCO_SECTEUR VALUES ('35','Sante')
INSERT INTO #TRANSCO_SECTEUR VALUES ('40','Finance')
INSERT INTO #TRANSCO_SECTEUR VALUES ('45','Technologies de l information')
INSERT INTO #TRANSCO_SECTEUR VALUES ('50','Telecommunications')
INSERT INTO #TRANSCO_SECTEUR VALUES ('55','Services aux Collectivites')
-----------------------------------------------------------------------------
-----------------------------------------------------------------------------
-- Boucle sur toutes les configurations des proxy
-----------------------------------------------------------------------------
-----------------------------------------------------------------------------
DECLARE @code_proxy nvarchar(20), @libelle_proxy nvarchar(60),@codeIndex [nvarchar](12), @holdingType nvarchar(20)

DECLARE proxy_cursor2 CURSOR FOR
SELECT code_proxy,libelle_proxy,codeIndex,holdingType FROM #CONFIG_PROXY_ACTION

OPEN proxy_cursor2;

FETCH NEXT FROM proxy_cursor2 INTO @code_proxy,@libelle_proxy,@codeIndex,@holdingType;

WHILE @@FETCH_STATUS = 0
BEGIN
	insert into #INDEX_COMPO
	select 'ACTION' as 'source', @code_proxy as 'code_proxy', @libelle_proxy as 'Libelle_proxy',* 
	from ref_holding.INDEX_LISTING(@codeIndex, @dateIndicesMSCI,@holdingType)

   FETCH NEXT FROM proxy_cursor2 INTO @code_proxy,@libelle_proxy,@codeIndex,@holdingType;
END

CLOSE proxy_cursor2;
DEALLOCATE proxy_cursor2;

--1 transco Secteur FGA  (table SOUS_SECTEUR)
insert into PTF_PROXY
select @dateProxy as 'Date',p.code_proxy as 'Code_Proxy', p.libelle_proxy as 'Libelle_Proxy', p.ISIN as 'code_titre', a.FinancialInstrumentName as'Libelle_Titre',p.MarketValue/p.MarketValueTotal / 1E6 as 'Poids_VB',0 as 'Poids_CC', 
 t1.type_produit as 'Type_Produit', p.MarketValue_Cur as 'Devise_Titre',
s.secteur  as 'Secteur',
ss.sous_secteur as 'Sous_secteur',
t1.pays as 'Pays',
a.FinancialInstrumentName As 'Emetteur',
NULL AS 'Rating',
a.FinancialInstrumentName As 'Groupe_Emet',
  NULL as 'Maturité',
  NULL as 'Duration',
  NULL as 'Sensibilite'
 from #INDEX_COMPO as p
left outer join ref_security.ASSET as a on a.Id = p.AssetId
left outer join ref_common.IDENTIFICATION as id on id.Id = a.IdentificationId
left outer join #TRANSCO_TYPE_PRODUIT as t1 on t1.iso = id.Country COLLATE SQL_Latin1_General_CP1_CI_AS
left outer join  ref_security.ASSET_CLASSIFICATION as class on class.AssetId = p.AssetId and class.Source ='MSCI'
left outer join #TRANSCO_SOUS_SECTEUR as ss on ss.msci_code  = class.Classification4 COLLATE SQL_Latin1_General_CP1_CI_AS
left outer join #TRANSCO_SECTEUR as s on s.msci_code = class.Classification1  COLLATE SQL_Latin1_General_CP1_CI_AS
where p.source = 'ACTION'
and p.ISIN <> 'XXXXXXXXXXXX' -- ne pas prendre les detachements de div

-- fin creation proxy Actions
-----------------------------------------------------------------------------
-----------------------------------------------------------------------------
-- proxy Obligations Gov / BARCLAYS
-----------------------------------------------------------------------------
-----------------------------------------------------------------------------

create TABLE #TRANSCO_SOUS_SECTEUR_BARCLAYS (pays varchar(20), sous_secteur varchar(60) not null) 
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Allemagne','EMPRUNT D''ETAT ALLEMAND')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Espagne','EMPRUNT D''ETAT ESPAGNOL')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Etats-Unis','EMPRUNT D''ETAT AMERICAIN')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Finlande','EMPRUNT D''ETAT FINLANDAIS')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Italie','EMPRUNT D''ETAT ITALIEN')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('France','EMPRUNT D''ETAT FRANCAIS')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Pays-Bas','EMPRUNT D''ETAT HOLLANDAIS')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Portugal','EMPRUNT D''ETAT PORTUGAIS')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Canada','EMPRUNT D''ETAT CANADIEN')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Belgique','EMPRUNT D''ETAT BELGE')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Royaume-Uni','EMPRUNT D''ETAT BRITANNIQUE')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Japon','EMPRUNT D''ETAT JAPONAIS')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Autriche','EMPRUNT D''ETAT AUTRICHIEN')
INSERT INTO #TRANSCO_SOUS_SECTEUR_BARCLAYS VALUES ('Irlande','EMPRUNT D''ETAT IRLANDAIS')

create TABLE #TRANSCO_RATINGS (rating varchar(4), ratingMoodys varchar(4))     
INSERT INTO #TRANSCO_RATINGS VALUES ('AAA','Aaa')
INSERT INTO #TRANSCO_RATINGS VALUES ('AA+','Aa1')
INSERT INTO #TRANSCO_RATINGS VALUES ('AA','Aa2')
INSERT INTO #TRANSCO_RATINGS VALUES ('AA-','Aa3')
INSERT INTO #TRANSCO_RATINGS VALUES ('A+','A1')
INSERT INTO #TRANSCO_RATINGS VALUES ('A','A2')
INSERT INTO #TRANSCO_RATINGS VALUES ('A-','A3')
INSERT INTO #TRANSCO_RATINGS VALUES ('BBB+','Baa1')
INSERT INTO #TRANSCO_RATINGS VALUES ('BBB','Baa2')
INSERT INTO #TRANSCO_RATINGS VALUES ('BBB-','Baa3')
INSERT INTO #TRANSCO_RATINGS VALUES ('BB+','Ba1')
INSERT INTO #TRANSCO_RATINGS VALUES ('BB','Ba2')
INSERT INTO #TRANSCO_RATINGS VALUES ('BB-','Ba3')
INSERT INTO #TRANSCO_RATINGS VALUES ('B+','B1')
INSERT INTO #TRANSCO_RATINGS VALUES ('B','B2')
INSERT INTO #TRANSCO_RATINGS VALUES ('B-','B3')
INSERT INTO #TRANSCO_RATINGS VALUES ('CCC+','Caa1')
INSERT INTO #TRANSCO_RATINGS VALUES ('CCC','Caa2')
INSERT INTO #TRANSCO_RATINGS VALUES ('CCC-','Caa3')
INSERT INTO #TRANSCO_RATINGS VALUES ('CC','Ca')
INSERT INTO #TRANSCO_RATINGS VALUES ('C','C')
INSERT INTO #TRANSCO_RATINGS VALUES ('NR','NR')


-- boucle pour récuperer l ensemble des titres

DECLARE proxy_cursor CURSOR FOR
SELECT code_proxy,libelle_proxy,codeIndex,holdingType FROM #CONFIG_PROXY_GOV

OPEN proxy_cursor;

FETCH NEXT FROM proxy_cursor INTO @code_proxy,@libelle_proxy,@codeIndex,@holdingType;

WHILE @@FETCH_STATUS = 0
BEGIN
	insert into #INDEX_COMPO
	select 'GOV' as 'source',@code_proxy as 'code_proxy', @libelle_proxy as 'Libelle_proxy',*
	from ref_holding.INDEX_LISTING(@codeIndex,@dateIndicesBarclays,@holdingType)

   FETCH NEXT FROM proxy_cursor INTO @code_proxy,@libelle_proxy,@codeIndex,@holdingType;
END

CLOSE proxy_cursor;
DEALLOCATE proxy_cursor;

-- recuperation des emetteurs, rating
select distinct AssetId, IssuerName , Country, ROW_NUMBER() OVER(partition by AssetId order by AssetId) AS Row 
INTO #ISSUEROLE
from ref_issuer.ROLE as issuer
where issuer.AssetId in (select AssetId from #INDEX_COMPO) and issuer.Discriminator ='IssuerRole'

select * 
into #ISSUERS
from #ISSUEROLE 
where Row = 1
order by AssetId 

select rdate.AssetId,r2.Value 
into #RATINGS
from (
select AssetId,max( valueDate) as ValueDate
from ref_rating.RATING as r 
where r.AssetId in (select AssetId from #INDEX_COMPO) and r.RatingScheme ='BARCLAYS'
and ValueDate <= @dateProxy
group by AssetId
) as rdate
left outer join ref_rating.RATING as r2  on rdate.AssetId = r2.AssetId and r2.ValueDate = rdate.ValueDate 
and r2.RatingScheme ='BARCLAYS'

insert into PTF_PROXY
select @dateProxy as 'Date', l.code_proxy as 'Code_Proxy',l.libelle_proxy as 'Libelle_Proxy',l.ISIN as 'code_titre',a.FinancialInstrumentName as 'Libelle_Titre',l.MarketValue / (l.MarketValueTotal) as 'Poids_VB',0 as 'Poids_CC', 
  'Obligations Taux Fixe' as 'Type_Produit',p.Price_Cur as 'Devise_Titre',
  'EMPRUNTS D''ETAT' as 'Secteur',
  soussecteur.sous_secteur as 'Sous_secteur',
  c1.french as 'Pays',
  c1.french  as 'Emetteur',
  tr.rating as 'Rating',
  c1.french as 'Groupe_Emet',
  a.MaturityDate as 'Maturité',
  p.Debt_Duration as 'Duration',
  p.Debt_Sensitivity as 'Sensibilite'
from #INDEX_COMPO as l
left outer join ref_security.ASSET as a on a.Id = l.AssetId
left outer join ref_security.DEBT as d on d.Id = l.AssetId
left outer join #RATINGS as r on r.AssetId = l.AssetId 
left outer join ref_security.PRICE as p on p.SecurityId = l.AssetId and p.Date = l.IndexDate and p.Price_Source = 'BARCLAYS'
left outer join ref_common.IDENTIFICATION as i on i.Id = a.IdentificationId
left outer join ref_security.ASSET_CLASSIFICATION as ac on  ac.AssetId = l.AssetId and ac.Source ='BARCLAYS'
left outer join #ISSUERS as issuer on  issuer.AssetId = l.AssetId 
left outer join ref.COUNTRY as c1 on c1.iso2 = issuer.Country
left outer join #TRANSCO_SOUS_SECTEUR_BARCLAYS as soussecteur on soussecteur.pays = c1.french COLLATE SQL_Latin1_General_CP1_CI_AS
left outer join #TRANSCO_RATINGS as tr on tr.ratingMoodys = r.Value COLLATE SQL_Latin1_General_CP1_CI_AS
--left outer join ref.COUNTRY as c2 on c2.iso2 = i.Country
where l.source = 'GOV'

-- fin creation proxy Obligations
drop table #INDEX_COMPO
drop table #ISSUEROLE
drop table #ISSUERS
drop table #RATINGS

drop table #TRANSCO_SECTEUR
drop table #TRANSCO_SOUS_SECTEUR
drop table #TRANSCO_TYPE_PRODUIT
drop table #CONFIG_PROXY_ACTION
drop table #CONFIG_PROXY_GOV
drop table #TRANSCO_SOUS_SECTEUR_BARCLAYS
drop table #TRANSCO_RATINGS

-------------------------------------------------------------------
-- TRAITEMENT DE REMPLACEMENT DES PROXYS ORIGINAUX PAR CEUX DES INDICES
-------------------------------------------------------------------
declare @dateProxy datetime
set @dateProxy = '29/05/2015'

create TABLE #CONFIG_PROXY_GOV (code_proxy nvarchar(20), libelle_proxy nvarchar(60),code_proxy_back [nvarchar](20), libelle_proxy_back nvarchar(60) )

INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_ALL','BT11TREU proxy pour EuroMtS Global','EMTX_G','EMTX 1 - 15+ years')		
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_1_3','BTS1TREU proxy pour EuroMtS 1-3','EMTX_A','EMTX 1-3 years')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_3_5','BTS3TREU proxy pour EuroMtS 3-5','EMTX_B','EMTX 3 - 5 years')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_5_7','BTS5TREU proxy pour EuroMtS 5-7','EMTX_C','EMTX 5 - 7 years')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_7_10','BT7YTREU proxy pour EuroMtS 7-10','EMTX_D','EMTX 7 - 10 years')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_10_15','BT10TREU proxy pour EuroMtS 10-15','EMTX_E','EMTX 10 - 15 years')
INSERT INTO #CONFIG_PROXY_GOV VALUES ('BARCLAYS_EUROT_15+','BT15TREU proxy pour EuroMtS 15+','EMTX_F','EMTX 15+ years')

create TABLE #CONFIG_PROXY_ACTION (code_proxy nvarchar(20), libelle_proxy nvarchar(60),code_proxy_back [nvarchar](20), libelle_proxy_back nvarchar(60) )

INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_US','MXUS proxy pour SP500NET','SP500NET','SP500NET')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Europe','MXEU proxy pour SXXP/STOXX EUROPE','SXXP','STOXX EUROPE')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_EMU_LargeCap','MSCI650008 proxy pour SX5E/STOXX 50 EURO','SX5E','STOXX 50 EURO')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_EMU','MXEM proxy pour SXXE/STOXX EURO','SXXE','STOXX EURO')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_ExEMU','MXEUM proxy pour SXXA/STOXX EUROPE ex Euro','SXXA','STOXX EUROPE ex Euro')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_EMU_MidCap','MSCI652525 proxy pour MCXE/STOXX MID CAP EURO','MCXE','STOXX MID CAP EURO')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_exEMU_MidCap','MSCI652528 proxy pour MCXA/STOXX MID CAP EX-EURO','MCXA','STOXX MID CAP EX-EURO')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_UK','MXGB proxy pour Footsie 100','F100','FOOTSIE 100')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_DE','M3DE proxy pour DAX 30','DAX','Indice DAX')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_FR','MSCI650018 proxy pour CAC40','CAC40','Indice CAC 40')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_ENERGY','MXEU0EG proxy pour STOXX 600 OIL','SXXP_PETRO','DJS 600 Pr - PETROLE ET GAZ')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_ENERGY','MXEU0EG proxy pour STOXX 600 OIL','SXEP','DJS 600 Pr - PETROLE ET GAZ')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_AUTOPARTS','MXEU0AC proxy pour STOXX 600 AUTO','SXXP_AUTOM','DJS 600 Pr - AUTOMOBILE')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_BANKS','MXEU0BK proxy pour STOXX 600 BANQUE','SXXP_BANQU','DJS 600 Pr - BANQUES')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_HEALTH_CARE','MXEU0HC proxy pour STOXX 600 SANTE','SXXP_SANTE','DJS 600 Pr - SANTE')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_CONSUMER_SVC','MXEU0HR proxy pour STOXX 600 B&S CONSO','SXXP_CONSO','DJS 600 Pr - B&S DE CONSOMMATION')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_INSURANCE','MXEU0IS proxy pour STOXX 600 ASSURANCE','SXXP_ASSUR','DJS 600 Pr - ASSURANCE')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_FOOD_BEV','MXEU0FB proxy pour STOXX 600 ALIMENTATION','SXXP_ALIME','DJS 600 Pr - ALIMENTAIRE - BOISSON')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_MATERIALS','MXEU0ML proxy pour STOXX 600 PRODUITS DE BASE','SXXP_PRODU','DJS 600 Pr - PRODUITS DE BASE')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_Eu_UTILITIES','MXEU0UL proxy pour STOXX 600 SERVICES AUX COLLECTIVITES','SXXP_SERVI','DJS 600 Pr - SERVICES AUX COLLECTIVITES')

INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_ENERGY','MXEM0EG proxy pour STOXX EURO OIL','SXXE_PETRO','DJES Pr - PETROLE ET GAZ')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_AUTOPARTS','MXEM0AC proxy pour STOXX EURO AUTO','SXXE_AUTOM','DJES Pr - AUTOMOBILE')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_BANKS','MXEM0BK proxy pour STOXX EURO BANQUE','SXXE_BANQU','DJES Pr - BANQUES')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_HEALTH_CARE','MXEM0HC proxy pour STOXX EURO SANTE','SXXE_SANTE','DJES Pr - SANTE')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_CONSUMER_SVC','MXEM0HR proxy pour STOXX EURO B&S CONSO','SXXE_CONSO','DJES Pr - B&S DE CONSOMMATION')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_INSURANCE','MXEM0IS proxy pour STOXX EURO ASSURANCE','SXXE_ASSUR','DJES Pr - ASSURANCE')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_FOOD_BEV','MXEM0FB proxy pour STOXX EURO ALIMENTATION','SXXE_ALIME','DJES Pr - ALIMENTAIRE - BOISSON')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_MATERIALS','MXEM0ML proxy pour STOXX EURO PRODUITS DE BASE','SXXE_PRODU','DJES Pr - PRODUITS DE BASE')
INSERT INTO #CONFIG_PROXY_ACTION VALUES ('MSCI_E_UTILITIES','MXEM0UL proxy pour STOXX EURO SERVICES AUX COLLECTIVITES','SXXE_SERVI','DJES Pr - SERVICES AUX COLLECTIVITES')

-- passage aux nouveaux code proxy
update PTF_PARAM_PROXY
set Code_Proxy = config.code_proxy,
Libellé_Proxy = config.libelle_proxy, 
Source = 'PROXY'
from PTF_PARAM_PROXY as paramP
left outer join #CONFIG_PROXY_GOV as config on config.code_proxy_back = paramP.code_proxy  COLLATE SQL_Latin1_General_CP1_CI_AS 
where paramP.Date = @dateProxy and config.code_proxy is not null

update PTF_PARAM_PROXY
set Code_Proxy = config.code_proxy,
Libellé_Proxy = config.libelle_proxy, 
Source = 'PROXY'
from PTF_PARAM_PROXY as paramP
left outer join #CONFIG_PROXY_ACTION as config on config.code_proxy_back = paramP.code_proxy  COLLATE SQL_Latin1_General_CP1_CI_AS 
where paramP.Date = @dateProxy and config.code_proxy is not null

---- ROLLBACK
--update PTF_PARAM_PROXY
--set Code_Proxy = config.code_proxy_back,
--Libellé_Proxy = config.libelle_proxy_back, 
--Source = 'NO'
--from PTF_PARAM_PROXY as paramP
--left outer join #CONFIG_PROXY_GOV as config on config.code_proxy = paramP.code_proxy  COLLATE SQL_Latin1_General_CP1_CI_AS 
--where paramP.Date = '30/01/2015' and config.code_proxy is not null

--update PTF_PARAM_PROXY
--set Code_Proxy = config.code_proxy_back,
--Libellé_Proxy = config.libelle_proxy_back, 
--Source = 'PROXY'
--from PTF_PARAM_PROXY as paramP
--left outer join #CONFIG_PROXY_ACTION as config on config.code_proxy = paramP.code_proxy  COLLATE SQL_Latin1_General_CP1_CI_AS 
--where paramP.Date = '30/01/2015' and config.code_proxy is not null

drop table #CONFIG_PROXY_ACTION
drop table #CONFIG_PROXY_GOV
