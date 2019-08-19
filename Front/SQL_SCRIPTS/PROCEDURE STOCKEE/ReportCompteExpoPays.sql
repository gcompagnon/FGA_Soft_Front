USE [E1DBFGA01]
GO

/****** Object:  StoredProcedure [dbo].[ReportCompteExpoPays]    Script Date: 09/18/2012 10:04:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







 
/** configuration du rapport SupervisionGroupe:
Les regroupements/groupes des comptes clients sont déduits des positions Omega 
presentes dans les inventaires en transparence

Pour chaque groupe, des rubriques définies par libellé sont fournies avec leur valeur.(NULL pour une rubrique non définies pour un groupe) 

Chaque groupe possède un ordre prédéfini pour l'affichage (cet ordre est unique pour un groupe donné)
Dans un groupe donné, la rubrique possède un ordre en priorité de l'ordre naturel (alphabétique)

Chaque rapport est identifié par une clé et une date (se rapportant à l'inventaire) ce qui garantie que les données sont cohérentes entre elles suivant ces 2 valeurs.
**/  

--USE FGA_DEV
-- avec l'utilisateur fga 
--GO
-- execute as fga
-- DROP PROCEDURE ReportMonitoringGroupe


ALTER PROCEDURE [dbo].[ReportCompteExpoPays] 
        @dateInventaire  datetime ,
        @niveauInventaire tinyint ,
        @cleReport char(20) OUTPUT,
        @comptes varchar(500)
AS

/** Parametres: --------------------------------------**/
/*
declare @dateInventaire as datetime
set  @dateInventaire = '31/01/2012'
declare @niveauInventaire as tinyint
set @niveauInventaire = 3
declare @cleReport as char(20)
*/
IF @cleReport IS NULL
BEGIN
  set @cleReport = 'CompteExpoPays'
END

/** Parametres: ---FIN--------------------------------**/

-- test d existence de la date demandée pour l'inventaire
/** TODO **/
--select distinct DateInventaire from  [PTF_TRANSPARISE] order by DateInventaire
-- select distinct Groupe from  [PTF_TRANSPARISE] where DateInventaire = '2011-08-24'

-- test d existence du niveau d inventaire
/** TODO **/

-- suppression pour la date demandée mais on pourra archiver sur une autre table ces rapports 
delete from [PTF_RAPPORT]  
   where date = @dateInventaire 
   and cle = @cleReport

declare @incClassCompartiment as float
set @incClassCompartiment = 0 /** 1 pour encours, 2 pour Oblig, 3 pour divers, 4 pour monétaire, 5 pour Actions **/
declare @incClassRubrique as float
set @incClassRubrique = 0     /** A  l interieur d un compartiment, va de 0 à n pour la décomposition **/ 



/**CREATION D'UNE TABLE CONTENANT LES COMPTES ET LEUR NUMERO D'ORDRE******************/
declare @ordre tinyint
set @ordre=1

CREATE TABLE #comptes (Groupe varchar(25) COLLATE DATABASE_DEFAULT primary key , ordreGroupe smallint null)

DECLARE @OrderID varchar(25), @Pos int
DECLARE @comptesBis varchar(500)
SET @comptesBis=@comptes

SET @comptes = LTRIM(RTRIM(@comptes))+ ','
SET @Pos = CHARINDEX(',', @comptes, 1)

IF REPLACE(@comptes, ',', '') <> ''
BEGIN
	WHILE @Pos > 0
	BEGIN
		SET @OrderID = LTRIM(RTRIM(LEFT(@comptes, @Pos - 1)))
		IF @OrderID <> ''
		BEGIN
			INSERT INTO #comptes (Groupe,ordreGroupe) VALUES (@OrderID,@ordre) --Use Appropriate conversion
			set @ordre=@ordre+1
		END
		SET @comptes = RIGHT(@comptes, LEN(@comptes) - @Pos)
		SET @Pos = CHARINDEX(',', @comptes, 1)

	END
END

/**************************************************************************/

/*
/** la table des colonnes calculees / des sur-groupes  **/
create table #GroupeCalcule  ( groupe varchar(25) primary key , ordreGroupe tinyint null)
insert #GroupeCalcule values ('RETRAITE',3)
insert #GroupeCalcule values ('ASSURANCE',13)
insert #GroupeCalcule values ('FGA',0)

create table #GroupeCalculeComposant  ( groupeCalcule varchar(25), groupeComposant varchar(25) null)
insert #GroupeCalculeComposant values ('RETRAITE', 'MM AGIRC')
insert #GroupeCalculeComposant values ('RETRAITE', 'MM ARRCO')
insert #GroupeCalculeComposant values ('ASSURANCE', 'MMP')
insert #GroupeCalculeComposant values ('ASSURANCE', 'MUT2M')
insert #GroupeCalculeComposant values ('ASSURANCE', 'CMAV')
insert #GroupeCalculeComposant values ('ASSURANCE', 'QUATREM')
insert #GroupeCalculeComposant values ('ASSURANCE', 'AUXIA')
insert #GroupeCalculeComposant values ('ASSURANCE', 'CAPREVAL')
insert #GroupeCalculeComposant values ('ASSURANCE', 'INPR')
insert #GroupeCalculeComposant values ('ASSURANCE', 'SAPREM')
insert #GroupeCalculeComposant values ('ASSURANCE', 'AUTRES')

declare @maxGroup int 
set @maxGroup = 13
--Les nouveaux groupes seront places arbitrairement à la suite


/** on met dans le groupe FGA l ensemble des groupes */
insert #GroupeCalculeComposant 
 select distinct 'FGA',Groupe
 from [PTF_TRANSPARISE]
 where Dateinventaire = @dateInventaire
 and Numero_Niveau = @niveauInventaire
 and Groupe <> 'OPCVM'

-- temporaire pour stocker les groupes présents dans les inventaires transparisés 
   insert into #Groupe
  select distinct Groupe, -1 as ordreGroupe   
  from [PTF_TRANSPARISE]
  where Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and Groupe not in (select groupe from #Groupe)

insert #GroupeCalculeComposant select 'EXTERNE', Groupe from #Groupe where ordreGroupe = -1 and Groupe <> 'OPCVM'

  update #Groupe 
  set   ordreGroupe = @maxGroup,
  @maxGroup = @maxGroup +1 
  where ordreGroupe = -1
        
insert #GroupeCalcule values ('EXTERNE',@maxGroup) 
*/

                
  /**********************************/
  -- specification du rapport :
  -- chaque zone de 1 à maxZone
  -- constitué de 1 à n Pays
  /*********************************/
  declare @maxZone as tinyint
  set @maxZone = 7
  -- definition des zones de pays: 
  create table #pays_zone ( numero tinyint, ordrePays tinyint, idPays varchar(30) COLLATE DATABASE_DEFAULT, libelleZone varchar(60)  COLLATE DATABASE_DEFAULT )
   --Zone 0 : France seule
   insert into #pays_zone values (1,1,'FR','Zone 1 - France')

   --Zone pays zone1: Allemagne, Pays-bas, Finlande et Autriche
   insert into #pays_zone values (2,1,'DE','Zone 1 - ex France')
   insert into #pays_zone values (2,2,'NL','Zone 1 - ex France')
   insert into #pays_zone values (2,3,'FI','Zone 1 - ex France')
   insert into #pays_zone values (2,4,'AT','Zone 1 - ex France')
  
  --Zone 2: est la zone Portugal, Italie, Ireland, Grèce et Espagne
   insert into #pays_zone values (3,1,'PT','Zone 2')
   insert into #pays_zone values (3,2,'IT','Zone 2')
   insert into #pays_zone values (3,3,'IE','Zone 2')
   insert into #pays_zone values (3,4,'GR','Zone 2')
   insert into #pays_zone values (3,5,'ES','Zone 2')
  
  
  --Zone 1 extension : est la zone des autres pays en zone euro
   insert into #pays_zone values (4,1,'BE','Extension Zone 1')
   insert into #pays_zone values (4,2,'LU','Extension Zone 1')
   insert into #pays_zone values (4,3,'DK','Extension Zone 1')

  --Zone Europe-ex euro extension : est la zone des pays européens hors euro
   insert into #pays_zone values (5,1,'GB','Europe-ex euro')
   insert into #pays_zone values (5,2,'SE','Europe-ex euro')
   insert into #pays_zone values (5,3,'NO','Europe-ex euro')

  --Zone USA + Australie
   insert into #pays_zone values (6,1,'US','USA + Australie')
   insert into #pays_zone values (6,2,'AU','USA + Australie')
   
   --Zone 3 : Autres pays (inclure Pays is null)
   insert into #pays_zone 
   select 7, row_number()  over (order by pays.libelle), pays.id ,'Reste' 
   from  [PAYS] as pays
   where pays.id not in (select idPays from  #pays_zone )
            
   -- formalisme du rapport: avec en priorité: pays d une des zones, et ensuite, décomposition par Govies, Agencies, covered, corporate, action, fonds actions/corporate/divers.      
   create table #rapport_pays ( ordrePays  int,
                                numeroZone tinyint,
                                libelleZone varchar(60) COLLATE DATABASE_DEFAULT,
                                ordre  tinyint,
                                pays  varchar(30) COLLATE DATABASE_DEFAULT,
                                rubrique  varchar(60) COLLATE DATABASE_DEFAULT,
                                libelle  varchar(60) COLLATE DATABASE_DEFAULT,
                                code1  varchar(60) COLLATE DATABASE_DEFAULT,   -- utilisé pour le sous secteur
                                code2  varchar(60)  COLLATE DATABASE_DEFAULT)  -- utilisé pour le type_produit
   
--   select * from  #pays_zone   
   
   
   
   insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone,  1,         
         pays.libelle,
         pays.libelle+'_GOVIES',
         'EMPRUNT D''ETAT(tous types)',
         secteur.libelle, null         
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur IN ('O GVT' )

insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone,  2,         
         pays.libelle,
         pays.libelle+'_GOVIES_INFL',
         'EMPRUNT D''ETAT(Indexé Inflation)',
         secteur.libelle, 'Obligations Indexées sur l''Inflation'
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur IN ('O GVT' )
           
         
insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone, 3,         
         pays.libelle,
         pays.libelle+'_AGENCIES',
         'AGENCES',
         secteur.libelle, null        
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur IN ('O AGENCIES' )
         
       
insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone, 4,         
         pays.libelle,
         pays.libelle+'_COVERED',
         'OBLIGATIONS SECURISEES',
         secteur.libelle, null
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur = 'O COVERED' 
         
insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone, 5,         
         pays.libelle,
         pays.libelle+'_OBLIGATIONS',
         'OBLIGATIONS',
         secteur.libelle, null         
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur like 'O CF%' or  secteur.id_secteur IN ( 'F OBLIG', 'O CNF')
                  
                  
insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone, 6,         
         pays.libelle,
         pays.libelle+'_ACTIONS',
         'ACTIONS',
         secteur.libelle, null         
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur like 'A %' or  secteur.id_secteur = ( 'F ACTIONS')


insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone, 7,
         pays.libelle,
         pays.libelle+'_DIVERS',
         'DIVERSIFIE',
         secteur.libelle, null
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur = ( 'F DIVERS')

insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone, 8,
         pays.libelle,
         pays.libelle+'_TRESO',
         'TRESORERIE',
         secteur.libelle, null
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur = ( 'F TRESO')


insert into #rapport_pays 	  
select   zone.ordrePays ,zone.numero,zone.libelleZone, 9,
         pays.libelle,
         pays.libelle+'_CASH',
         'LIQUIDITE',
         'Liquidité', null         
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
----------------------------------------           
--select * from #rapport_pays   order by ordrePays
   
create table #temp (cle varchar(40) COLLATE DATABASE_DEFAULT,groupe varchar(40) COLLATE DATABASE_DEFAULT,ordreGroupe int,date datetime,
	rubrique varchar(50) COLLATE DATABASE_DEFAULT,sousRubrique varchar(50) COLLATE DATABASE_DEFAULT,libelle varchar(40) COLLATE DATABASE_DEFAULT,classementRubrique int,
	classementSousRubrique int,valeur float)   
	
/****Calcul par Groupe***************************************************************/
 insert 
  into  #temp
(cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)    
  select @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 'TOTAL' as rubrique,
		 'TOTAL_HOLDING' as sousRubrique,
		 'ENCOURS global' as libelle,
		  0,
		  0,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
    from  #comptes  as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  group by  rapport.Groupe,rapport.ordreGroupe
/********************************************************************************/
   
   
/****Calcul par pays****************************************************************/
   create table #encours_pays(groupe varchar (25) COLLATE DATABASE_DEFAULT,ordreGroupe tinyint,pays varchar(60) COLLATE DATABASE_DEFAULT, ordrePays int ,valeur float)

  insert into #encours_pays
  select rapport.Groupe as groupe,		 
		 rapport.ordreGroupe, 		 
		 rapport.pays, 
		 100*rapport.numeroZone+rapport.ordrePays as ordrePays,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from (select distinct pays,ordrePays,numeroZone, groupe, ordreGroupe from #rapport_pays, #comptes where pays <> 'Not Available' 
                                               and numeroZone < @maxZone 
        ) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.pays, rapport.ordrePays,rapport.numeroZone
  
  -- Pour la zone 3 (les autres pays) on consolide sur une rubrique 'Autres Pays'
 insert into #encours_pays
  select rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 		 
		 'Autres Pays', 
		 @maxZone *100,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from (select distinct pays, groupe, ordreGroupe from #rapport_pays, #comptes where pays <> 'Not Available' and numeroZone >=@maxZone) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  group by  rapport.Groupe,rapport.ordreGroupe
  
  /** pour les lignes en Pays is Null ou Not Available **/
 insert into #encours_pays
  select rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 		 
		 'NON ATTRIBUE', 
		 (@maxZone+1) *100,		 		 
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from #comptes as rapport
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and ( tr.Pays = 'Not Available' or  tr.Pays is null  or Sous_Secteur is null)  
  group by  rapport.Groupe,rapport.ordreGroupe
  
 insert 
  into  #temp
(cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)    
  select @cleReport as cle,
		 encours.Groupe as groupe,		 
		 encours.ordreGroupe , 
		 @dateInventaire,  
		 encours.pays as rubrique,
		 encours.pays+'_HOLDING' as sousRubrique,
		 'ENCOURS sous total' as libelle,
		  encours.ordrePays,
		  0,
         encours.valeur
  from #encours_pays as encours 
/**********************************************************************************/   
   
/*****Calcul par secteurs*****************************************************************************/
-- pour les rubriques utilisant le code1
 insert 
  into #temp
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 rapport.pays as rubrique,
		 rapport.rubrique as sousRubrique, -- ensemble des codes sous secteurs 
		 rapport.libelle as libelle,
		 100*rapport.numeroZone+rapport.ordrePays,
		  rapport.ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #comptes where code2 is null) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code1 -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone   -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.pays, rapport.rubrique, rapport.libelle, rapport.ordre,rapport.ordrePays,rapport.numeroZone
  
  -- pour les rubriques utilisant le code1 et le code2
 insert 
  into #temp 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 rapport.pays as rubrique,
		 rapport.rubrique as sousRubrique, -- ensemble des codes sous secteurs 
		 rapport.libelle as libelle,
		 100*rapport.numeroZone+rapport.ordrePays,
		  rapport.ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #comptes where code2 is not null) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code1 -- les codes des sous secteurs
  and tr.type_produit = rapport.code2 -- les codes des types d actifs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone   -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.pays, rapport.rubrique, rapport.libelle, rapport.ordre,rapport.ordrePays,rapport.numeroZone


-- Pour la zone 3 (les autres pays) on consolide sur une rubrique 'Autres Pays'
-- en code1
 insert 
  into #temp 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 'Autres Pays' as rubrique,
		 'Autres Pays_'+rapport.libelle as sousRubrique, 
		 rapport.libelle as libelle,
		  @maxZone *100,
		  rapport.ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #comptes where code2 is null) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code1 -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
  and rapport.numeroZone >=@maxZone
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.libelle, rapport.ordre
  
  -- Pour la zone 3 (les autres pays) on consolide sur une rubrique 'Autres Pays'
  -- en code1 et code2
  insert 
  into #temp 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 'Autres Pays' as rubrique,
		 'Autres Pays_'+rapport.libelle as sousRubrique, 
		 rapport.libelle as libelle,
		  @maxZone *100,
		  rapport.ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #comptes where code2 is not null) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code1 -- les codes des sous secteurs
  and tr.type_produit = rapport.code2 -- les codes des types produits
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
  and rapport.numeroZone >=@maxZone
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.libelle, rapport.ordre
/**********************************************************************************/   
   
/****Partie non classifié**********************************************************/
insert 
  into  PTF_RAPPORT_NIV2 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 'NON ATTRIBUE' as rubrique, -- Pays Non attribué ou Not Available
		 rapport.rubrique as sousRubrique, -- ensemble des codes sous secteurs 
		 rapport.libelle as libelle,
		  (@maxZone+1) *100,
		  rapport.ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #comptes where Pays = 'Not Available' and code2 is null ) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and ( tr.Pays is null or tr.Pays = 'Not Available')
  and  tr.sous_Secteur = rapport.code1 -- les codes des sous secteurs
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.pays, rapport.rubrique, rapport.libelle, rapport.ordre,rapport.ordrePays

/** pour les lignes en Sous_secteur is null **/
 insert 
  into #temp 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 'NON ATTRIBUE' as rubrique, -- Pays Non attribué ou Not Available
		 'Not Available_NonClassifie' as sousRubrique,
		 'NON CLASSIFIE' as libelle,  -- secteur null
		  (@maxZone+1) *100,
		  9,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from #comptes as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and ( tr.sous_Secteur is null  or tr.sous_Secteur = 'Not Available' )
  group by  rapport.Groupe,rapport.ordreGroupe
/**********************************************************************************/


/*******Calcul agregaztion par Zones***********************************************/
insert 
  into #temp 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 rapport.libelleZone as rubrique,
		 'ZONE_'+convert(varchar,rapport.numeroZone)+'_'+rapport.libelle as sousRubrique, 
		 rapport.libelle as libelle,
		  rapport.numeroZone*100,
		  rapport.ordre, -- ordre de la sous rubrique
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #comptes where code2 is null) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code1 -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone  -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.libelle, rapport.ordre,rapport.libelleZone,rapport.numeroZone


 insert 
  into #temp 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 rapport.libelleZone as rubrique,
		 'ZONE_'+convert(varchar,rapport.numeroZone)+'_'+rapport.libelle as sousRubrique, 
		 rapport.libelle as libelle,
		  rapport.numeroZone*100,
		  rapport.ordre, -- ordre de la sous rubrique
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #comptes where code2 is not null) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code1 -- les codes des sous secteurs
  and tr.type_produit = rapport.code2 -- le type d actif
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone  -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.libelle, rapport.ordre,rapport.libelleZone,rapport.numeroZone


-- calcul de l encours par zone

 insert 
  into #temp 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
 select  @cleReport as cle,
		 rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 
		 @dateInventaire,  
		 rapport.libelleZone as rubrique,
		 'ZONE_'+convert(varchar,rapport.numeroZone)+'_GLOBAL' as sousRubrique, 
		 'ENCOURS pour la Zone' as libelle,
		  rapport.numeroZone*100,
		  0, -- code pour le global
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #comptes where code2 is null) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.compte = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code1 -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone  -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.libelleZone,rapport.numeroZone
/**********************************************************************************/
   
insert 
into #temp 
(cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
select cle,
		@comptesBis as groupe,
		0 as ordreGroupe,
		@dateInventaire,
		rubrique,sousRubrique,libelle,
		classementRubrique,
		classementSousRubrique,
		sum(valeur) as valeur
from #temp where cle='CompteExpoPays'
group by cle,date, rubrique, classementRubrique,sousRubrique,classementSousRubrique,libelle
   
   
   
   
   
   
   
   
  insert 
  into  PTF_RAPPORT_NIV2 
  (cle,groupe, ordreGroupe,date,rubrique,sousRubrique,libelle, classementRubrique,classementSousRubrique,  valeur )
   select * from #temp
   order by ordreGroupe, classementRubrique, classementSousRubrique
   
--drop table #Groupe
--drop table #GroupeCalcule	
--drop table #GroupeCalculeComposant 

drop table #comptes
drop table #pays_zone
drop table #rapport_pays
drop table #temp
drop table #encours_pays









GO

