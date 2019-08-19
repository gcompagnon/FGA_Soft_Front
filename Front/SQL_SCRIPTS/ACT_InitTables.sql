-- base valeur pour la base action

 -- Ajout d un champ factset 
 ALTER TABLE ACT_DATA_FACTSET 
 ADD BETA_1YR float
 ALTER TABLE ACT_DATA_FACTSET_AGR
 ADD BETA_1YR float
 
ALTER TABLE ACT_DATA_FACTSET ADD COST_INCOME_NTM float
ALTER TABLE ACT_DATA_FACTSET ADD EBIT_MARGIN_LTM float
ALTER TABLE ACT_DATA_FACTSET ADD EBIT_MARGIN_NTM_AVG5Y float
ALTER TABLE ACT_DATA_FACTSET ADD EBIT_MARGIN_ON_AVG5Y float
ALTER TABLE ACT_DATA_FACTSET ADD EBIT_MARGIN_PPTM float
ALTER TABLE ACT_DATA_FACTSET ADD EBIT_MARGIN_PTM float
ALTER TABLE ACT_DATA_FACTSET ADD EBIT_MARGIN_STM float
ALTER TABLE ACT_DATA_FACTSET ADD P_TBV_NTM float
ALTER TABLE ACT_DATA_FACTSET ADD PBT_GROWTH_NY float
ALTER TABLE ACT_DATA_FACTSET ADD PBT_RWA_CY float
ALTER TABLE ACT_DATA_FACTSET ADD PBT_RWA_LY float
ALTER TABLE ACT_DATA_FACTSET ADD PBT_RWA_NY float
ALTER TABLE ACT_DATA_FACTSET ADD PBT_SALES_DIFF_CY_NY float
ALTER TABLE ACT_DATA_FACTSET ADD PBT_SALES_DIFF_LY_CY float
ALTER TABLE ACT_DATA_FACTSET ADD PBT_SALES_NTM float
ALTER TABLE ACT_DATA_FACTSET ADD ROTE_NTM float

 
 ALTER TABLE ACT_DATA_FACTSET ADD GARPN_NOTE_S float
 ALTER TABLE ACT_DATA_FACTSET ADD GARPN_ISR_S float
 
 

 -- Lien entre un libelle (company_name de factset)/isin et un ticker bloomberg
 
 create table ACT_VALEUR (
 id int  identity(1,1) primary key,
 ISIN varchar(12) ,
 LIBELLE varchar(100),
 TICKER_BLOOMBERG varchar(12),
 id_fga int,
 IS_EURO varchar(1),
 EXCLUSION bit,
 TOTAL_NOTE float )
 
 
 -- table qui stocke le resultat des calculs de ranking ( utilise le champ de calcul GARPN_TOTAL_S)
 create table ACT_VALEUR_RANK (
 id int identity(1,1) primary key, 
 id_value int FOREIGN KEY REFERENCES ACT_VALEUR(id),
 date datetime,
 rank_quant int,
 rank_qual int,
 rank_total int)
 
 
--drop table ACT_COEF_CRITERE
-- Table des criteres pour la winsorization (utilisé dans Blend_score_Valeur)
create table ACT_COEF_CRITERE ( 
 id_critere  int identity(1,1) primary key,
 id_parent  int ,
 nom varchar(30) not null, 
 position int not null,
 description varchar(200),
 CAP_min float ,
 CAP_max float ,
 format tinyint , /* 0 normal,1 Facteur 2 Pourcentage ou null*/
 precision int , /* pour l affichage #0.000 , nb de zero affiché */
 groupe int ,
 inverse bit not null,
 is_sector bit not null )
 
 
 
 -- contient l ensemble des secteurs d un indice (code secteur + libelle)
 create table ACT_INDICE ( 
 id int primary key,
 libelle varchar(20) not null)
 
 insert into ACT_INDICE (id,libelle) values (1,'SXXP')
 
 -- contient les coefficients intra-sectoriels de l indice
 create table ACT_COEF_INDICE ( 
 id_critere int not null, --"cle etrangere" sur ACT_COEF_CRITERE
 id_indice int, -- si 0 , coef  par defaut pour la valeur 0
 date datetime not null,
 coef  float,
-- CONSTRAINT pk_ACT_COEF_INDICE PRIMARY KEY (id_critere,date)
  )
 
 -- contient les coefficients intra-sectoriels des secteur FGA
 create table ACT_COEF_SECTEUR (
 id_critere int not null, --"cle etrangere" sur ACT_COEF_CRITERE
 id_fga int , -- si la colonne est null, c est un critere commun à tous les secteurs
				--	ou  une cle etrangere sur ACT_FGA_SECTOR
 date  datetime not null,
 coef  float,

 )
 

--RECOMMANDATION sur les secteurs

-- table des commentaires, sur la reco, sur le changement
create table ACT_RECO_COMMENT (
id int  identity(1,1) primary key,
comment varchar(max) )

-- reco lié au sub sector
create table ACT_RECO_SECTOR (
 id_secteur int not null, 
 date datetime not null,
 type CHAR(3),
 recommandation VARCHAR(4),
 id_comment int FOREIGN KEY REFERENCES ACT_RECO_COMMENT(id),
 id_comment_change int FOREIGN KEY REFERENCES ACT_RECO_COMMENT(id),
 CONSTRAINT pk_ACT_RECO_SECTOR PRIMARY KEY (id_secteur,date,type)
 )
 
 -- la gestion de la table faite par procstoc: ACT_Add_Reco_Valeur
 create table ACT_RECO_VALEUR (
 id_valeur int identity(1,1) primary key,
 date datetime not null,
 ISIN varchar(12) ,
 reco_SXXP VARCHAR(4),
 reco_SXXE VARCHAR(4),
 reco_SXXA VARCHAR(4),
 id_comment int FOREIGN KEY REFERENCES ACT_RECO_COMMENT(id),
 id_comment_change int FOREIGN KEY REFERENCES ACT_RECO_COMMENT(id)
 )
 
 
 
 
 --table : qui n est plus utilisée ?
 --create table ACT_DATA_LIQUIDITY (
 --date,
 --isin,
 --libelle,
 --defaut,
 --forcer,
 --unions )
 
 
 
 -- Tables dzes notes (utilisés par l ecran baseActonNote)
 create table ACT_NOTE_TABLE(
 id_table int identity(1,1) primary key, 
 nom varchar(50) )
 
 create table ACT_NOTE_COLUMN(
 id_column int identity(1,1) primary key, 
 id_table int FOREIGN KEY REFERENCES ACT_NOTE_TABLE(id_table),
 nom varchar(50),
 is_activated bit,
 is_note bit,
 coef float,
 position int)
 
 create table ACT_NOTE_RECORD(
 id_record int identity(1,1) primary key, 
 id_column int FOREIGN KEY REFERENCES ACT_NOTE_COLUMN(id_column),
 id_valeur int  FOREIGN KEY REFERENCES ACT_VALEUR(id),
 note varchar(max)
 )
 
 -- base fichiers utilisé par les ecrans  Base de fichiers
 create table ACT_FILE(
 id int identity(1,1) primary key, 
 fname varchar(100),
 url varchar(255),
 description text
 )
 
 create table ACT_FILE_LINK(
 date datetime,
 id_file int FOREIGN KEY REFERENCES ACT_FILE(id),
 id_sector_icb int,
 id_sector_fga int,
 id_value int,
 onglet int,
 CONSTRAINT pk_ACT_FILE_LINK PRIMARY KEY (id_file,date)
 )
 ---------------------------------------------------------
 -- configuration des criteres 
 ---------------------------------------------------------
delete from ACT_COEF_CRITERE
delete from [ACT_COEF_SECTEUR]
delete from [ACT_COEF_INDICE]

DECLARE @idRootCritere AS int
DECLARE @idCatCritere AS int
DECLARE @idLeafCritere AS int

-- CONFIGURATION DES CRITERES POUR SELECTION DE VALEURS (onglet VALEUR/ANALYSE)

-- 1.0.0 ROOT : CROISSANCE
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (null,'CROISSANCE', 1, 'SelectionValeurs/Analyse - Croissance', 1, 100,0,1,null,0,1)

SET @idRootCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idRootCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,32)

-- 1.1.0 CATEGORIE Benefices par actions
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'EPS GROWTH', 1, 'SelectionValeurs/Analyse - BENEFICE PAR ACTION', 1, 100,0,1,null,0,1)


SET @idCatCritere = SCOPE_IDENTITY()


INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idCatCritere
,NULL -- coef par dfaut : pas de secteurs FGA
,'01/01/2013'
,20)

-- 1.1.1 critere factset pour BENEFICE PAR ACTION

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'EPS_GROWTH_NTM', 1, 'SelectionValeurs/Analyse - BPA - Croissance 12 prochains mois', 1, 100,0,1,1,0,1)
 
SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,5)
           
-- 1.1.2 critere factset pour BENEFICE PAR ACTION          
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'EPS_TREND_5YR', 2, 'SelectionValeurs/Analyse - BPA - tendance 5 ans', 1, 100,0,1,1,0,1)


SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,10)
         
-- 1.1.3 critere factset pour BENEFICE PAR ACTION          
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'EPS_RSD', 3, 'SelectionValeurs/Analyse - BPA - stabilité des variations 5 ans', 1, 100,0,1,1,0,1)


SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,5)



-- 1.2.0 CATEGORIE Chiffres d affaires

--  categorie 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'SALES GROWTH', 2, 'SelectionValeurs/Analyse - CHIFFRE D AFFAIRES', 1, 100,0,1,null,0,1)


SET @idCatCritere = SCOPE_IDENTITY()


INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idCatCritere
,NULL -- coef par dfaut : pas de secteurs FGA
,'01/01/2013'
,12)

-- 1.2.1 critere factset 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'SALES_GROWTH_NTM', 1, 'SelectionValeurs/Analyse - CA - Croissance 12 prochains mois', 1, 100,0,1,2,0,1)
 
SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,3)
           
-- 1.2.2 critere factset
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'SALES_TREND_5YR', 2, 'SelectionValeurs/Analyse - CA - tendance 5 ans', 1, 100,0,1,2,0,1)


SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,6)
         
-- 1.2.3 critere factset 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'SALES_RSD', 3, 'SelectionValeurs/Analyse - CA - stabilité des variations 5 ans', 1, 100,0,1,2,0,1)


SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,3)

-- FIN ROOT : CROISSANCE


-- 2.0.0 ROOT : QUALITE
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (null,'QUALITE', 2, 'SelectionValeurs/Analyse - Qualite', 1, 100,0,1,null,0,1)

SET @idRootCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idRootCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,20)

-- 2.1.0 CATEGORIE 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'EBIT MARGIN', 1, 'SelectionValeurs/Analyse - QUAL - Marge d exploitation', 1, 100,0,1,null,0,1)


SET @idCatCritere = SCOPE_IDENTITY()


INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idCatCritere
,NULL -- coef par dfaut : pas de secteurs FGA
,'01/01/2013'
,5)

-- 2.1.1 critere factset

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'EBIT_MARGIN_NTM', 1, 'SelectionValeurs/Analyse - QUAL - Marge d exploitation sur les 12 prochains mois', 1, 100,0,1,3,0,1)
 
SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,5)

-- 2.2.0 CATEGORIE 

--  categorie 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'FCF', 2, 'SelectionValeurs/Analyse - QUAL - Pérennité de la croissance', 1, 100,0,1,3,0,1)

SET @idCatCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idCatCritere
,NULL -- coef par dfaut : pas de secteurs FGA
,'01/01/2013'
,10)

-- 2.2.1 critere factset 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'FCF_TREND_5YR', 1, 'SelectionValeurs/Analyse - QUAL - tendance FCF 5 ans', 1, 100,0,1,4,0,1)
 
SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,5)
           
-- 2.2.2 critere factset
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'NET_DEBT_EBITDA_NTM', 2, 'SelectionValeurs/Analyse - QUAL - dette nette/EBITDA 12 prochains mois', 1, 100,0,1,4,1,1)

SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,5)
         


-- 2.3.0 CATEGORIE 

--  categorie 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'ROE', 3, 'SelectionValeurs/Analyse - QUAL - Rentabilité', 1, 100,0,1,null,0,1)

SET @idCatCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idCatCritere
,NULL -- coef par dfaut : pas de secteurs FGA
,'01/01/2013'
,5)

-- 2.3.1 critere factset 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'ROE_NTM', 1, 'SelectionValeurs/Analyse - QUAL - RoE 12 prochains mois', 1, 100,0,1,5,0,1)
 
SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,5)
           
-- 2.3.2 critere factset
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'COST_INCOME_NTM', 2, 'SelectionValeurs/Analyse - QUAL - Charges/Revenus 12 prochains mois', 1, 100,0,1,5,1,1)

SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,0)

-- FIN ROOT : QUALITE

-- 3.0.0 ROOT : VALORISATION
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (null,'VALORISATION', 3, 'SelectionValeurs/Analyse - Valorisation', 1, 100,0,1,null,0,1)

SET @idRootCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idRootCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,28)

-- 3.1.0 CATEGORIE
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'P/E', 1, 'SelectionValeurs/Analyse - Cours/Benefice par Actions', 1, 100,0,1,null,0,1)


SET @idCatCritere = SCOPE_IDENTITY()


INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idCatCritere
,NULL -- coef par dfaut : pas de secteurs FGA
,'01/01/2013'
,12)

-- 3.1.1 critere factset

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PE_NTM', 1, 'SelectionValeurs/Analyse - VALO - P/E 12 prochains mois', 1, 100,0,1,6,1,1)
 
SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,4)
           
-- 3.1.2 critere factset
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PE_NTM_AVG5Y', 2, 'SelectionValeurs/Analyse - VALO - P/E 12 prochains mois /historique 5 ans', 1, 100,0,1,6,1,1)


SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,4)
         
-- 3.1.3 critere factset
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PE_VS_IND_ON_AVG5Y', 3, 'SelectionValeurs/Analyse - VALO - P/E 12 procains mois relatif /historique 5 ans relatif', 1, 100,0,1,6,1,1)


SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,4)



-- 3.2.0 CATEGORIE

--  categorie 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'P/B', 2, 'SelectionValeurs/Analyse - VALO - Cours/Actif Net comptable par action', 1, 100,0,1,null,0,1)


SET @idCatCritere = SCOPE_IDENTITY()


INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idCatCritere
,NULL -- coef par dfaut : pas de secteurs FGA
,'01/01/2013'
,12)

-- 3.2.1 critere factset 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PB_NTM', 1, 'SelectionValeurs/Analyse - VALO - P/B 12 prochains mois', 1, 100,0,1,null,1,1)
 
SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,4)
           
-- 3.2.2 critere factset
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PB_NTM_AVG5Y', 2, 'SelectionValeurs/Analyse - VALO - 12 prochains mois /historique 5 ans', 1, 100,0,1,7,1,1)


SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,4)
         
-- 3.2.3 critere factset 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PB_VS_IND_ON_AVG5Y', 3, 'SelectionValeurs/Analyse - VALO -  12 prochains mois relatif /historique 5 ans relatif', 1, 100,0,1,7,1,1)


SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,4)

-- 3.3.0 CATEGORIE

--  categorie 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'YLD', 3, 'SelectionValeurs/Analyse - VALO - Rendement', 1, 100,0,1,null,0,1)


SET @idCatCritere = SCOPE_IDENTITY()


INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idCatCritere
,NULL -- coef par dfaut : pas de secteurs FGA
,'01/01/2013'
,4)

-- 3.3.1 critere factset 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'DIV_YLD_NTM', 1, 'SelectionValeurs/Analyse - VALO - Rendement 12 prochains mois', 1, 100,0,1,8,0,1)
 
SET @idLeafCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idLeafCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,4)
           
-- FIN ROOT : VALORISATION

-- 4.0.0 ROOT : ISR
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (null,'ISR', 4, 'SelectionValeurs/Analyse - ISR', 1, 100,0,1,null,0,1)

SET @idRootCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
     VALUES (
           @idRootCritere
           ,NULL -- coef par dfaut : pas de secteurs FGA
           ,'01/01/2013'
           ,20)

-- 4.1.0 CATEGORIE
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'Note ISR', 1, 'SelectionValeurs/Analyse - Note ISR', 1, 100,0,1,null,0,1)


SET @idCatCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idCatCritere
,NULL -- coef par dfaut : pas de secteurs FGA
,'01/01/2013'
,20)

-- FIN ROOT : ISR

--******************** CONFIG CRITERE DE SELECTION VALEURS  EXCEPTION: Banques*******************
-- 3 criteres à ajouter
-- et les coeefficients a reprendre et modifier
INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
select [id_critere],25,'02/01/2013',[coef] from [ACT_COEF_SECTEUR]
where id_critere in (select id_critere from ACT_COEF_CRITERE where is_sector = 1 )
and id_fga is null

-- ROOT : QUALITE
-- 2.1.0 CATEGORIE 

update [ACT_COEF_SECTEUR]
set coef = 10 
where id_fga =25 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'EBIT MARGIN' and is_sector = 1 and id_fga =25)

update [ACT_COEF_SECTEUR]
set coef = 0 
where id_fga =25 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'EBIT_MARGIN_NTM' and is_sector = 1 and id_fga =25)

-- 2.2.1 critere factset
declare @idCatCritere as int
set @idCatCritere = (select id_critere from ACT_COEF_CRITERE 
where nom = 'EBIT MARGIN' and is_sector = 1 )

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PBT_RWA_TREND_5YR', 1, 'PBT/RWA' , 1, 100,0,2,3,0,1)

declare @idLeafCritere as int
set @idLeafCritere = (select [id_critere] from ACT_COEF_CRITERE where nom = 'PBT_SALES_NTM' and is_sector = 1)

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idLeafCritere
,25 -- coef pour le secteur Banque
,'01/01/2013'
,10)



update [ACT_COEF_SECTEUR]
set coef = 0 
where id_fga =25 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'FCF' and is_sector = 1 and id_fga =25)

update [ACT_COEF_SECTEUR]
set coef = 0 
where id_fga =25 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'FCF_TREND_5YR' and is_sector = 1 and id_fga =25)

update [ACT_COEF_SECTEUR]
set coef = 0 
where id_fga =25 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'NET_DEBT_EBITDA_NTM' and is_sector = 1 and id_fga =25)

-- 2.1.1 critere factset
declare @idCatCritere as int
set @idCatCritere = (select id_critere from ACT_COEF_CRITERE 
where nom = 'ROE' and is_sector = 1 )

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'ROTE_NTM', 2, 'RoTE' , 1, 100,0,1,5,0,1)


declare @idLeafCritere as int
set @idLeafCritere = (select [id_critere] from ACT_COEF_CRITERE where nom = 'ROTE_NTM' and is_sector = 1)

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idLeafCritere
,25 -- coef pour le secteur Banque
,'01/01/2013'
,5)

update [ACT_COEF_SECTEUR]
set coef = 10 
where id_fga =25 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'ROE' and is_sector = 1 and id_fga =25)

update [ACT_COEF_SECTEUR]
set coef = 0 
where id_fga =25 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'ROE_NTM' and is_sector = 1 and id_fga =25)

update [ACT_COEF_SECTEUR]
set coef = 5
where id_fga =25 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'COST_INCOME_NTM' and is_sector = 1 and id_fga =25)
--------
-- 2.2.1 critere factset
declare @idCatCritere as int
set @idCatCritere = (select id_critere from ACT_COEF_CRITERE 
where nom = 'P/B' and is_sector = 1 )

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'P_TBV_NTM', 1, 'P/TBV' , 1, 100,0,1,7,1,1)

declare @idLeafCritere as int
set @idLeafCritere = (select [id_critere] from ACT_COEF_CRITERE where nom = 'P_TBV_NTM' and is_sector = 1)

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idLeafCritere
,25 -- coef pour le secteur Banque
,'01/01/2013'
,4)


update [ACT_COEF_SECTEUR]
set coef = 0
where id_fga =25 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'PB_NTM' and is_sector = 1 and id_fga =25)
-- FIN pour Banque

--******************** FIN CONFIG CRITERE DE SELECTION VALEURS*******************

--******************** CONFIG CRITERE DE SELECTION VALEURS  EXCEPTION: Assurances*******************
-- 3 criteres à ajouter
-- et les coeefficients a reprendre et modifier
INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])
select [id_critere],26,'03/01/2013',[coef] from [ACT_COEF_SECTEUR]
where id_critere in (select id_critere from ACT_COEF_CRITERE where is_sector = 1 )
and id_fga is null

-- ROOT : QUALITE
-- 2.1.0 CATEGORIE 

update [ACT_COEF_SECTEUR]
set coef = 10 
where id_fga =26 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'EBIT MARGIN' and is_sector = 1 and id_fga =26)

update [ACT_COEF_SECTEUR]
set coef = 0 
where id_fga =26 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'EBIT_MARGIN_NTM' and is_sector = 1 and id_fga =26)


-- 2.2.1 critere factset
declare @idCatCritere as int
set @idCatCritere = (select id_critere from ACT_COEF_CRITERE 
where nom = 'EBIT MARGIN' and is_sector = 1 )

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PBT_SALES_NTM', 1, 'PBT/sales' , 1, 100,0,1,3,0,1)

declare @idLeafCritere as int
set @idLeafCritere = (select [id_critere] from ACT_COEF_CRITERE where nom = 'PBT_SALES_NTM' and is_sector = 1)

INSERT INTO [ACT_COEF_SECTEUR]([id_critere],[id_fga],[date],[coef])VALUES ( 
@idLeafCritere
,26 -- coef pour le secteur Banque
,'01/01/2013'
,10)


update [ACT_COEF_SECTEUR]
set coef = 0 
where id_fga =26 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'FCF' and is_sector = 1 and id_fga =26)

update [ACT_COEF_SECTEUR]
set coef = 0 
where id_fga =26 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'FCF_TREND_5YR' and is_sector = 1 and id_fga =26)

update [ACT_COEF_SECTEUR]
set coef = 0 
where id_fga =26 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'NET_DEBT_EBITDA_NTM' and is_sector = 1 and id_fga =26)

update [ACT_COEF_SECTEUR]
set coef = 10 
where id_fga =26 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'ROE' and is_sector = 1 and id_fga =26)

update [ACT_COEF_SECTEUR]
set coef = 10 
where id_fga =26 and id_critere = (select id_critere from ACT_COEF_CRITERE
 where nom = 'ROE_NTM' and is_sector = 1 and id_fga =26)


delete from [ACT_COEF_SECTEUR] where coef = 0

--******************** FIN CONFIG CRITERE DE SELECTION VALEURS*******************



DECLARE @idRootCritere AS int
DECLARE @idCatCritere AS int
DECLARE @idLeafCritere AS int

--****************************************************************************
-- CONFIGURATION DES CRITERES POUR ALLOCATION SECTORIELLE (onglet SECTEUR/ANALYSE)

 insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (null,'CROISSANCE', 1, 'Croissance', 1, 100,0,1,null,0,0)

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (null,'QUALITE', 1, 'Qualité', 2, 100,0,1,null,0,0)

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (null,'VALORISATION', 1, 'Valorisation', 3, 100,0,1,null,0,0)

 
SET @idRootCritere = ( SELECT id_critere from ACT_COEF_CRITERE where nom = 'CROISSANCE' and is_sector = 0)
 -- 1.1  BPA
 
INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idRootCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,40)


--  categorie 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'BPA', 1, NULL, 1, 100,0,1,null,0,0)

SET @idCatCritere = SCOPE_IDENTITY()

INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idCatCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,25)

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values  (@idCatCritere,'EPS_GROWTH_NTM', 1, 'Cr EPS %', 1, 100,2,1,1,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,7)

		   
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'EPS_TREND_5YR', 2, 'EPS Trend', 1, 100,2,1,1,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,12)
		   
		   
 insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'EPS_RSD', 3, 'EPS CV', 1, 100,2,1,1,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,6)

-- 1.2  CHIFFRE D AFFAIRE		   
--  categorie 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'Chiffre d''affaires', 1, NULL, 1, 100,0,1,null,0,0)
SET @idCatCritere = SCOPE_IDENTITY()
INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idCatCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,15)
		   
 insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'SALES_GROWTH_NTM', 1, 'Cr Sales %', 1, 100,2,1,2,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,3)

		   
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'SALES_TREND_5YR', 2, 'Sales Trend', 1, 100,2,1,2,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,8)
		   
		   
 insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'SALES_RSD', 3, 'Sales CV', 1, 100,2,1,2,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,4)

-- 2.0 Qualite
SET @idRootCritere = ( SELECT id_critere from ACT_COEF_CRITERE where nom = 'QUALITE'  and is_sector = 0)

INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idRootCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,25)

 -- 2.1  EBIT MARGIN
 --  categorie 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'Marge d''exploitations', 1, NULL, 1, 100,0,1,null,0,0)
SET @idCatCritere = SCOPE_IDENTITY()
INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idCatCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,15)
	

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'EBIT_MARGIN_TREND_5YR', 1, 'EBIT Margin Trend', 1, 100,2,1,3,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,10)

		   
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'EBIT_MARGIN_RSD', 2, 'EBIT Margin CV', 1, 100,2,1,3,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,5)		   

-- 2.2  ROE
--  categorie 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'ROE', 1, NULL, 1, 100,0,1,null,0,0)
SET @idCatCritere = SCOPE_IDENTITY()
INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idCatCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,10)
			   
 insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'ROE_NTM', 1, 'ROE %', 1, 100,2,1,4,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,10)


		   
-- 3.0 Valorisation
SET @idRootCritere = ( SELECT id_critere from ACT_COEF_CRITERE where nom = 'VALORISATION'  and is_sector = 0)
 
INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idRootCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,35) 

 -- 3.1  P/E
 --  categorie 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'P/E', 1, NULL, 1, 100,0,1,null,0,0)
SET @idCatCritere = SCOPE_IDENTITY()
INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idCatCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,15)
	

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PE_NTM_INVERSE', 1, 'PE x', 1, 100,2,1,5,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,5)

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PE_ON_AVG5Y_INVERSE', 2, 'PE / Avg5Y', 1, 100,2,1,5,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,5)
 		   
 		   
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PE_VS_IND_ON_AVG5Y_INVERSE', 3, 'PE rel/ Avg5Y', 1, 100,2,1,5,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,5)

 -- 3.2  P/B
  --  categorie 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'P/B', 1, NULL, 1, 100,0,1,null,0,0)
SET @idCatCritere = SCOPE_IDENTITY()
INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idCatCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,15)
	
	
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PB_NTM_INVERSE', 1, 'PB x', 1, 100,2,1,6,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,5)

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PB_ON_AVG5Y_INVERSE', 2, 'PB / Avg5Y', 1, 100,2,1,6,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,5)
 		    		   
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'PB_VS_IND_ON_AVG5Y_INVERSE', 3, 'PB rel/ Avg5Y', 1, 100,2,1,6,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,5)
		   		   
 -- 3.3  Rendement
 --  categorie 
insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idRootCritere,'YLD', 1, NULL, 1, 100,0,1,null,0,0)
SET @idCatCritere = SCOPE_IDENTITY()
INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idCatCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,5)	 

insert into ACT_COEF_CRITERE (/*id_critere,*/ id_parent,nom, position, description,CAP_min,CAP_max, format, precision,groupe, inverse,is_sector)
 values
 (@idCatCritere,'DIV_YLD_NTM', 1, 'Dvd Yld %', 1, 100,2,1,7,0,0)

SET @idLeafCritere  = SCOPE_IDENTITY()

 INSERT INTO [ACT_COEF_INDICE]([id_critere],[id_indice],[date],[coef])
     VALUES(
			@idLeafCritere 
           ,1 -- SXXP
           ,'01/01/2013'
           ,5)
 		   
--******************** FIN  CONFIGURATION DES CRITERES POUR ALLOCATION SECTORIELLE (onglet SECTEUR/ANALYSE)*******************




--****************************************************************************
-- CONFIGURATION DES CRITERES qualitattifs POUR SELECTION VALEURS (onglet Valeurs/Analyse)
DECLARE @idTable AS int
insert into ACT_NOTE_TABLE (nom) Values ('Banques')
SET @idTable  = SCOPE_IDENTITY()
insert into ACT_NOTE_COLUMN (id_table, nom, is_activated,is_note, coef, position)
 Values (@idTable, 'Solvabilité',1,1,Null, 0)
 insert into ACT_NOTE_COLUMN (id_table, nom, is_activated,is_note, coef, position)
 Values (@idTable, 'Liquidité',1,1,Null, 1)

		   