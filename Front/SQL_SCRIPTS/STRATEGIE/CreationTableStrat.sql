--Suppression des tables deja exisantes (Surtout valable pour des tests)
Drop table [dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]
Drop table [dbo].[STRAT_TITRE_GRILLE]
Drop table [dbo].[STRAT_GRILLE]
Drop table [dbo].[STRAT_ALLOCATIONCIBLE]
Drop table [dbo].[STRAT_ALLOCATION]

CREATE TABLE [dbo].[STRAT_GRILLE](
	[Id] [varchar](10) NOT NULL,
	[Type] [varchar](12) NOT NULL,
	[Decomposition] [varchar](20) NOT NULL,
	PRIMARY KEY ([Id]) 
)


CREATE TABLE [dbo].[STRAT_TITRE_GRILLE](
	[ISIN_titre] [varchar](15) NOT NULL,
	[Id_Grille] [varchar] (10) NOT NULL REFERENCES STRAT_GRILLE(id),
	[Date] [datetime] NOT NULL,
	[Poids] [float] NOT NULL,
	[Source] [varchar](20) NULL,
PRIMARY KEY ([ISIN_titre],[Id_Grille]) 
)


CREATE TABLE [dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE](
	[Champ] [varchar](15) NOT NULL,
	[Valeur] [varchar] (60) NOT NULL,
	[Grille] [varchar] (10) NOT NULL REFERENCES STRAT_GRILLE(id),
PRIMARY KEY ([Champ],[Valeur]) 
)

Create table STRAT_ALLOCATIONCIBLE (					
		[Date] dateTime Not null, 
		Groupe varchar(25) Not null,
		indiceReference varchar(15) Not null
		PRIMARY KEY ([Date],groupe) 
)


Create table STRAT_ALLOCATION (					
		[Date] dateTime Not Null, 
		Groupe varchar(25) Not Null,
		valeur_boursiere float Not Null,
		PourcentGrille float Not Null,
		id_grille varchar(10) Not Null REFERENCES STRAT_GRILLE(id)
		PRIMARY KEY ([Date],groupe,id_grille)
)

Create table STRAT_VALORISATION_MANDAT (
	DateDeb Datetime NOT null,
	DateFin Datetime NOT null,
	Groupe varchar (25) NOT null ,
	Id_grille [varchar](10) NOT NULL,
	Valeur_boursiere float,
	Part_ptf float,
	Bench float,
	BenchReel float,
	EcartBench float,
	Perfgrille float,
	Plue_value float,
	Contribution float

Primary key clustered (
	DateDeb,
	DateFin, 
	Groupe, 
	Id_grille
	)
	FOREIGN KEY([id_grille]) REFERENCES [dbo].[STRAT_GRILLE] ([Id])
)
GO

SET ANSI_PADDING OFF
GO

-- Insertion des différentes grilles existantes
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('Act_Euro','Action','Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('Act_ExEuro','Action','Ex Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('Act_US','Action','Amerique')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('Act_Asie','Action','Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('Act_Emerg','Action','Emergent')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('Obl_Etat','Obligation','Etat')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('Obl_Credit','Obligation','Credit')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('Obl_Inf','Obligation','Inflation')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('Monetaire','Monetaire','Monetaire')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_GRILLE]([Id],[Type],[Decomposition])VALUES('NonAffecte','Autres','Autres')


-- Insertion des correspondances entre le titre et la grille.
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions France','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Autriche','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Allemagne','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Belgique','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Pays-Bas','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Portugal','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Espagne','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Finlande','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Irlande','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Grèce','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Italie','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Luxembourg','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions France general','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions France indiciel','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions France PMC','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions Euro general','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions Euro indiciel','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions Euro PMC','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions Euro','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions GB','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Danemark','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Suisse','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Suède','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Norvège','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM Actions Ex Euro','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM Actions Ex-Euro','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions US','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Canada','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Etats-Unis','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions USA general','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions USA indiciel','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions USA PMC','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions USA','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Japon','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions Japon','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions Japon incidiciel','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions Asie indiciel','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions Asie','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Australie','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions Asie Pacifique','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Obligations indexées sur l''inflation','Obl_Inf')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Secteur','Corporates non financieres','Obl_Credit')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Secteur','Corporates financieres banques','Obl_Credit')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Secteur','Corporates financieres assurances','Obl_Credit')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Secteur','Corporates financieres services financiers','Obl_Credit')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','Agencies non garanties','Obl_Credit')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','Agencies','Obl_Credit')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','Emprunt foncier et hypothecaire','Obl_Credit')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Secteur','Emprunts d''etat','Obl_Etat')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Code_Titre','SWAP','Obl_Etat')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','Agencies garanties','Obl_Etat')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM Obligations credit','Obl_Credit')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM Obligations etat','Obl_Etat')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM Obligations inflation','Obl_Inf')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Secteur','Fonds tresorerie','Monetaire')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions emergent','Act_Emerg')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Sous_Secteur','OPCVM actions emergentes internationales','Act_Emerg')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Secteur','Liquidité','Monetaire')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Bermudes','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Bahamas','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Brésil','Act_Emerg')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Chypre','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions République Tchèque','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions TCHEQUE, REPUBLIQUE','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Royaume-Uni','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Grece','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Guernesey','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Hong-Kong','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Hong Kong','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Hongrie','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Indonésie','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Islande','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Israel','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Jersey','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Corée du Sud','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions COREE, REPUBLIQUE DE','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Iles Caiman','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions CAIMANES, ILES','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Lituanie','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Mexique','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Malaisie','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Antilles Hollandaises','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Norvege','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Nouvelle Zélande','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Nouvelle-Zelande','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Panama','Act_US')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Pologne','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Russie','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions SUEDE','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Singapour','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Slovaquie','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Supranational','Act_Euro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Thailande','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Iles Vierges Britanniques','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions CHINE','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions FEROE, ILES','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions GABON','Act_Emerg')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions INDE','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions TAIWAN, PROVINCE DE CHINE','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions PAPOUASIE-NOUVELLE-GUINEE','Act_Emerg')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions ILE DE MAN','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions MARSHALL, ILES','Act_ExEuro')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions PHILIPPINES','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Coree Sud','Act_Asie')
INSERT INTO [E2DBFGA01].[dbo].[STRAT_CORRESPONDANCE_TITRE_GRILLE]([Champ],[Valeur],[Grille])VALUES('Type_Produit','Actions Taiwan','Act_Asie')


Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','ARCELOR MITTAL France',231)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','AUXIA',249)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','CAPREVAL',234)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','IDENTITES MUTUELLE',228)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','INPR',227)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','IRCEM MUTUELLE',215)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','IRCEM PREVOYANCE',214)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','MM AGIRC',188)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','MM ARRCO',188)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','SAPREM',261)
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','UNMI',217)
--Titres dont on ne connait pas encore l'indice de référence
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013,IRCEM RETRAITE')
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','MM MUTUELLE')
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','MMP')
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','OPCVM')
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','QUATREM')
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','AUTRES')
Insert into STRAT_ALLOCATIONCIBLE values ('19/06/2013','CMAV')
