
/****** Object:  StoredProcedure [dbo].[ReportMonitoringExpositionPaysSe]    Script Date: 09/18/2012 10:07:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



 
/** configuration du rapport Monitoring de l'exposition sur les emprunts d'états (souverains), actions ou corporate par rapport à une zone géo.
VUE sur la position
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
-- DROP PROCEDURE ReportMonitoringExpositionPays

ALTER PROCEDURE [dbo].[ReportMonitoringExpositionPaysSe]
        @dateInventaire  datetime ,
        @niveauInventaire tinyint ,
        @cleReport char(20) OUTPUT
AS

/** Parametres: --------------------------------------**/
/*
declare @dateInventaire as datetime
set  @dateInventaire = '13/06/2012'
declare @niveauInventaire as tinyint
set @niveauInventaire = 4
declare @cleReport as char(20)
*/

IF @cleReport IS NULL
BEGIN
  set @cleReport = 'MonitorExpoPaysSe'
END



/** Parametres: ---FIN--------------------------------**/

/*** execution en dehors de la procédure */

-- test d existence de la date demandée pour l'inventaire
/** TODO **/
--select distinct DateInventaire from  [PTF_TRANSPARISE] order by DateInventaire
-- select distinct Groupe from  [PTF_TRANSPARISE] where DateInventaire = '2011-08-24'

-- test d existence du niveau d inventaire
/** TODO **/

-- suppression pour la date demandée mais on pourra archiver sur une autre table ces rapports 
delete from [PTF_RAPPORT_NIV2]  
   where date = @dateInventaire 
   and cle = @cleReport

declare @incClassCompartiment as float
set @incClassCompartiment = 0 /** 1 pour encours, 2 pour Oblig, 3 pour divers, 4 pour monétaire, 5 pour Actions **/
declare @incClassRubrique as float
set @incClassRubrique = 0     /** A  l interieur d un compartiment, va de 0 à n pour la décomposition **/ 

create table #Groupe  ( groupe varchar(25) COLLATE DATABASE_DEFAULT primary key , ordreGroupe smallint null)
insert #Groupe values ('MM AGIRC',1)
insert #Groupe values ('MM ARRCO',2)

insert #Groupe values ('MMP',4)
insert #Groupe values ('INPR',5)
insert #Groupe values ('CAPREVAL',6)
insert #Groupe values ('CMAV',7)
insert #Groupe values ('MM MUTUELLE',8)
insert #Groupe values ('SAPREM',9)
insert #Groupe values ('AUXIA',10)
insert #Groupe values ('QUATREM',11)
insert #Groupe values ('AUTRES',12)
declare @maxGroup int 
set @maxGroup = 13
--Les nouveaux groupes seront places arbitrairement à la suite


/** la table des colonnes calculees / des sur-groupes  **/
create table #GroupeCalcule  ( groupe varchar(25) COLLATE DATABASE_DEFAULT  primary key , ordreGroupe tinyint null)
insert #GroupeCalcule values ('RETRAITE',3)
insert #GroupeCalcule values ('ASSURANCE',13)
insert #GroupeCalcule values ('FGA',0)

create table #GroupeCalculeComposant  ( groupeCalcule varchar(25) COLLATE DATABASE_DEFAULT , groupeComposant varchar(25) COLLATE DATABASE_DEFAULT  null)
insert #GroupeCalculeComposant values ('RETRAITE', 'MM AGIRC')
insert #GroupeCalculeComposant values ('RETRAITE', 'MM ARRCO')
insert #GroupeCalculeComposant values ('ASSURANCE', 'MMP')
insert #GroupeCalculeComposant values ('ASSURANCE', 'MM MUTUELLE')
insert #GroupeCalculeComposant values ('ASSURANCE', 'CMAV')
insert #GroupeCalculeComposant values ('ASSURANCE', 'QUATREM')
insert #GroupeCalculeComposant values ('ASSURANCE', 'AUXIA')
insert #GroupeCalculeComposant values ('ASSURANCE', 'CAPREVAL')
insert #GroupeCalculeComposant values ('ASSURANCE', 'INPR')
insert #GroupeCalculeComposant values ('ASSURANCE', 'SAPREM')
insert #GroupeCalculeComposant values ('ASSURANCE', 'AUTRES')

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
  
  
-- une table rassemblant tous les secteurs de type Obligataire 
    select libelle 
    into #secteurObligations
    from [SECTEUR] 
    where id like 'O CNF%' 
       or id like 'O CF%' 
       or id= 'F OBLIG' 
       or id =  'O GVT' 
       or id = 'O AGENCIES'
       or id = 'O COVERED'
       
       
       
/**********************************/
  -- specification du rapport :
  -- chaque zone de 1 à maxZone
  -- constitué de 1 à n Pays
  /*********************************/
  declare @maxZone as tinyint
  set @maxZone = 7
  -- definition des zones de pays: 
  create table #pays_zone ( numero tinyint, ordrePays tinyint, idPays varchar(30)COLLATE DATABASE_DEFAULT , libelleZone varchar(60) COLLATE DATABASE_DEFAULT  )
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
   insert into #pays_zone values (3,3,'GR','Zone 2')
   insert into #pays_zone values (3,4,'ES','Zone 2')
  
  
  --Zone 1 extension : est la zone des autres pays en zone euro
   insert into #pays_zone values (4,1,'BE','Extension Zone 1')   
   insert into #pays_zone values (4,2,'LU','Extension Zone 1')
   insert into #pays_zone values (4,3,'DK','Extension Zone 1')
   insert into #pays_zone values (4,4,'IE','Extension Zone 1')

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
                                libelleZone varchar(60)COLLATE DATABASE_DEFAULT ,
                                ordre  tinyint,
                                pays  varchar(30)COLLATE DATABASE_DEFAULT ,
                                rubrique  varchar(60)COLLATE DATABASE_DEFAULT ,
                                libelle  varchar(60)COLLATE DATABASE_DEFAULT ,
                                code  varchar(60) COLLATE DATABASE_DEFAULT )

insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone,  1,         
         pays.libelle,
         pays.libelle+'_GOVIES',
         'EMPRUNT D''ETAT',
         secteur.libelle         
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur IN ('O GVT' )
           
        
insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone, 2,         
         pays.libelle,
         pays.libelle+'_AGENCIES',
         'AGENCES',
         secteur.libelle         
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur IN ('O AGENCIES' )
       
insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone, 3,         
         pays.libelle,
         pays.libelle+'_COVERED',
         'OBLIGATIONS SECURISEES',
         secteur.libelle         
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur = 'O COVERED' 
         
insert into #rapport_pays 	  
select zone.ordrePays ,zone.numero,zone.libelleZone, 4,         
         pays.libelle,
         pays.libelle+'_OBLIGATIONS',
         'OBLIGATIONS',
         secteur.libelle         
  from  #pays_zone as zone
  left outer join [PAYS] as pays on pays.id = zone.idPays 
  join  [SOUS_SECTEUR] as secteur on secteur.id_secteur like 'O CF%' or  secteur.id_secteur IN ( 'F OBLIG', 'O CNF')
            


 /************************************************************/
 /***         DEBUT  CALCUL ENCOURS GROUPE PAR GROUPE     ****/
 /***           Pour la Duration Ponderee: ne prendre que ****/
 /***           l encours des secteurs Obligataires       ****/    
/*************************************************************/
 create table #encours(groupe varchar (25)COLLATE DATABASE_DEFAULT ,ordreGroupe tinyint, pays varchar(60)COLLATE DATABASE_DEFAULT , ordrePays int,rubrique varchar(120)COLLATE DATABASE_DEFAULT , libelle varchar(60)COLLATE DATABASE_DEFAULT , ordre tinyint,valeur float)

  insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)
  select rapport.Groupe as groupe,		 
		 rapport.ordreGroupe,
		 'TOTAL',
		 0 as ordrePays,
		 'TOTAL_HOLDING' as rubrique,
		 'SENSIBILITE global', 		 		 
		 0 as ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from (select distinct  groupe, ordreGroupe from #rapport_pays, #Groupe 
        ) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Secteur in (select libelle from #secteurObligations )
  group by  rapport.Groupe,rapport.ordreGroupe
  

  /**************************************************************/
 /***              FIN  CALCUL ENCOURS GROUPE PAR GROUPE    ****/
/***************************************************************/

 /************************************************************/
 /***              DEBUT  CALCUL ENCOURS PAYS PAR PAYS    ****/
/*************************************************************/
insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)    
  select rapport.Groupe as groupe,		 
		 rapport.ordreGroupe, 		 
		 rapport.pays, 
		 100*rapport.numeroZone+rapport.ordrePays as ordrePays,
		  rapport.pays+'_HOLDING',
		 'SENSIBILITE sous total',
		 0 as ordre,		 
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from (select distinct pays,ordrePays,numeroZone, groupe, ordreGroupe from #rapport_pays, #Groupe where pays <> 'Not Available' 
                                               and numeroZone < @maxZone 
        ) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.Secteur in (select libelle from #secteurObligations )
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.pays, rapport.ordrePays,rapport.numeroZone
  
  -- Pour la zone 3 (les autres pays) on consolide sur une rubrique 'Autres Pays'
 insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)    
  select rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 		 
		 'Autres Pays', 
		 @maxZone*100,
		  'Autres Pays_HOLDING',		 
		 'SENSIBILITE sous total',
		 0 as ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from (select distinct pays, groupe, ordreGroupe from #rapport_pays, #Groupe where pays <> 'Not Available' and numeroZone >=@maxZone) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.Secteur in (select libelle from #secteurObligations )
  group by  rapport.Groupe,rapport.ordreGroupe
  
  /** pour les lignes en Pays is Null ou Not Available **/
 insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)    
  select rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 		 
		 'NON ATTRIBUE', 
		 (@maxZone+1)*100,
		 'NON ATTRIBUE_HOLDING',
		 'SENSIBILITE sous total',			 		 
		 0, --pour global
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from #Groupe as rapport
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Secteur in (select libelle from #secteurObligations )
  and ( tr.Pays = 'Not Available' or  tr.Pays is null  or Sous_Secteur is null)    
  group by  rapport.Groupe,rapport.ordreGroupe
  
 
  /************************************************************/
 /***              FIN  CALCUL ENCOURS PAYS PAR PAYS    ****/
/*************************************************************/

  

 /******************************************************/
 /***              DEBUT CALCUL ENCOURS PAR ZONE ****/
/*******************************************************/
--  seules les zones de 1 à 3  
-- calcul de l encours par zone
  insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)
 select  rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 		 
		 rapport.libelleZone,
         rapport.numeroZone *100,
         'ZONE_'+convert(varchar,rapport.numeroZone)+'_'+rapport.libelle,
         rapport.libelle,
         rapport.ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #Groupe) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone  -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.libelleZone,rapport.numeroZone, rapport.libelle,rapport.ordre
  
  
  
  insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)  
 select  rapport.Groupe as groupe,		 
		 rapport.ordreGroupe , 		 
		 rapport.libelleZone as rubrique,    
		  rapport.numeroZone*100,
		 'ZONE_'+convert(varchar,rapport.numeroZone)+'_GLOBAL' ,
  		 'SENSIBILITE pour la Zone' as libelle,
		  0, -- code pour le global
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #Groupe) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  and tr.Secteur in (select libelle from #secteurObligations )
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone  -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.libelleZone,rapport.numeroZone
  
-- select * from #encours order by ordreGroupe, ordrePays, ordre
  
  /******************************************************/
 /***              FIN  CALCUL ENCOURS PAR ZONE     ****/
/*******************************************************/
 
  
/***********************************************************/
 /***              DEBUT CALCUL ENCOURS PAR SECTEUR      ****/
/***********************************************************/

                                
  insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)  
  select rapport.Groupe as groupe,
         rapport.ordreGroupe,		
		 rapport.pays,
		 100*rapport.numeroZone+rapport.ordrePays,
		 rapport.rubrique,
		 rapport.libelle as sousSecteur, -- ensemble des codes sous secteurs 
		 rapport.ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from (select * from #rapport_pays, #Groupe) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone  -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe,rapport.pays, rapport.rubrique,rapport.libelle,rapport.ordreGroupe,rapport.numeroZone,rapport.ordrePays,rapport.ordre

-- Pour la zone 3 (les autres pays) on consolide sur une rubrique 'Autres Pays'
insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)  
  select rapport.Groupe as groupe,
         rapport.ordreGroupe,				 
		 'Autres Pays' as pays,
		 @maxZone*100,
		 'Autres Pays_'+rapport.libelle,
		 rapport.libelle as sousSecteur, 
		 rapport.ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from (select * from #rapport_pays, #Groupe) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
  and rapport.numeroZone >=@maxZone
  group by  rapport.Groupe,rapport.ordreGroupe, rapport.libelle, rapport.ordreGroupe,rapport.ordre


-- select * from #encours
 /***********************************************************/
 /***              FIN CALCUL ENCOURS PAR SECTEUR        ****/
/***********************************************************/


  
 /**********************************************/
 /***              DEBUT  DONNEES NON CLASSIFIEES ****/
/**********************************************/

/** pour les lignes en Pays is Null **/
insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)  
  select rapport.Groupe as groupe,		 
         rapport.ordreGroupe,				 
		 'NON ATTRIBUE' as pays, -- Pays Non attribué ou Not Available
		 (@maxZone+1)*100,
		 rapport.rubrique,
		 rapport.libelle as sousSecteur,
		 rapport.ordre,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from (select * from #rapport_pays, #Groupe where Pays = 'Not Available' ) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and ( tr.Pays is null or tr.Pays = 'Not Available')
  and  tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  group by  rapport.Groupe,rapport.rubrique, rapport.libelle, rapport.ordreGroupe, rapport.ordre

/** pour les lignes en Sous_secteur is null **/
  insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)  
  select rapport.Groupe as groupe,	
	     rapport.ordreGroupe,	 
		 'NON ATTRIBUE' as pays, -- Pays Non attribué ou Not Available
		 (@maxZone+1)*100,
		 'Not Available_NonClassifie',
		 'NON CLASSIFIE' ,  -- secteur null
		 9,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from #Groupe as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.sous_Secteur is null  
  group by  rapport.Groupe,rapport.ordreGroupe,rapport.ordreGroupe
 

 /**********************************************/
 /***              FIN  DONNEES NON CLASSIFIEES    ****/
/**********************************************/


/************************************************************************/
 /***              DEBUT CALCUL SENSI PONDEREE                     ****/
 /***        stockage du nominateur : SUM ( Sensi * (Cours + CC ) )****/
 /***        note: cela permet de construire les groupes calculés simplement***/
/************************************************************************/
 
 -------------------    Par sous secteur  --------------------------------
 
 create table #sensibilite(groupe varchar (25)COLLATE DATABASE_DEFAULT ,
                                    pays varchar(30)COLLATE DATABASE_DEFAULT ,
                                    libelle varchar(60)COLLATE DATABASE_DEFAULT ,
                                    valeur float)
                                    
                                    
 insert into #sensibilite (groupe, pays, libelle, valeur)
 select  rapport.Groupe as groupe,		 
		 rapport.pays as pays,
		 rapport.libelle as sousSecteur,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur
  from (select * from #rapport_pays, #Groupe) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone  -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe,rapport.pays, rapport.libelle

-- Pour la zone 3 (les autres pays) on consolide sur une rubrique 'Autres Pays'
 insert into #sensibilite (groupe, pays, libelle, valeur)
 select  rapport.Groupe as groupe,		 
		 'Autres Pays' as pays,
		 rapport.libelle as sousSecteur,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur         
  from (select * from #rapport_pays, #Groupe) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
  and rapport.numeroZone >=@maxZone
  group by  rapport.Groupe, rapport.libelle
  -------------------    Par les non classifiees  --------------------------------
                                  
                                    
 insert into #sensibilite (groupe, pays, libelle, valeur)
 select rapport.Groupe as groupe,		 
		 'NON ATTRIBUE' as pays, -- Pays Non attribué ou Not Available
		 rapport.libelle as sousSecteur,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur
  from (select * from #rapport_pays, #Groupe where Pays = 'Not Available' ) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and ( tr.Pays is null or tr.Pays = 'Not Available')
  and  tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  group by  rapport.Groupe,rapport.rubrique, rapport.libelle

 
  /** pour les lignes en Pays is Null ou Not Available **/
 insert into #sensibilite (groupe, pays, libelle, valeur)
  select rapport.Groupe as groupe,		 
		 'NON ATTRIBUE', 
		 'SENSIBILITE sous total',			 		 
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur
  from #Groupe as rapport
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire    
  and ( tr.Pays = 'Not Available' or  tr.Pays is null  or Sous_Secteur is null)  
  group by  rapport.Groupe,rapport.ordreGroupe
-------------------    Par les zones  --------------------------------

insert into #sensibilite (groupe, pays, libelle, valeur)
 select  rapport.Groupe as groupe,		 
		 rapport.libelleZone,
		 'SENSIBILITE pour la Zone',
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur
  from (select * from #rapport_pays, #Groupe) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone  -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe, rapport.libelleZone
  
  
insert into #sensibilite (groupe, pays, libelle, valeur)
 select  rapport.Groupe as groupe,		 
		 rapport.libelleZone,
		 rapport.libelle,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur
  from (select * from #rapport_pays, #Groupe) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.sous_Secteur = rapport.code -- les codes des sous secteurs
  where rapport.pays <> 'Not Available' -- regroupement dans une partie
   and rapport.numeroZone < @maxZone  -- on filtre la zone 3 (autres Pays)
  group by  rapport.Groupe, rapport.libelleZone,rapport.libelle
  
  -------------------    Par les Pays  --------------------------------

  insert into #sensibilite (groupe, pays, libelle, valeur)
  select rapport.Groupe as groupe,		 
		 rapport.pays, 
		 'SENSIBILITE sous total',
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur
  from (select distinct pays,ordrePays,numeroZone, groupe, ordreGroupe from #rapport_pays, #Groupe where pays <> 'Not Available' 
                                               and numeroZone < @maxZone 
        ) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.Secteur in (select libelle from #secteurObligations )
  group by  rapport.Groupe,rapport.pays
  
  -- Pour la zone 3 (les autres pays) on consolide sur une rubrique 'Autres Pays'
 insert into #sensibilite (groupe, pays, libelle, valeur)
  select rapport.Groupe as groupe,		 		 
		 'Autres Pays',
		 'SENSIBILITE sous total', 
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur
  from (select distinct pays, groupe, ordreGroupe from #rapport_pays, #Groupe where pays <> 'Not Available' and numeroZone >=@maxZone) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Pays = rapport.pays
  and tr.Secteur in (select libelle from #secteurObligations )
  group by  rapport.Groupe
  
  /** pour les lignes en Pays is Null ou Not Available **/
 insert into #sensibilite (groupe, pays, libelle, valeur)
  select rapport.Groupe as groupe,		 
		 'NON ATTRIBUE',
		 'NON CLASSIFIE',
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur
  from #Groupe as rapport
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire = @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and ( tr.Pays = 'Not Available' or  tr.Pays is null  or Sous_Secteur is null)  
  group by  rapport.Groupe
  
  
  ----------- par les groupes en entier -------------------------------
 --create table #duration_groupe(groupe varchar (15),valeur float)

  insert into #sensibilite (groupe, pays, libelle, valeur)
  select rapport.Groupe as groupe,
  		 'TOTAL',
  		 'SENSIBILITE global',
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite ) as valeur
  from (select distinct  groupe, ordreGroupe from #rapport_pays, #Groupe 
        ) as rapport 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Groupe = rapport.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  and tr.Secteur in (select libelle from #secteurObligations )
  group by  rapport.Groupe
  
    
 


  /****************************************************/
 /***              FIN CALCUL DURATION PONDEREE    ****/
/****************************************************/

/********************************************************/
 /***              DEBUT  DONNEES CALCULEES           ****/
 /***           le nominateur de la DURATION est sommé ***/
 /***           à part de la somme d encours         *****/
/*********************************************************/
 
  -- specification des colonnes RETRAITE et ASSURANCE

 
  insert into #sensibilite (groupe, pays, libelle, valeur)
  select gr.groupe as groupe,
		 rapport.pays,
		 rapport.libelle,
         SUM( rapport.valeur ) as valeur
  from #GroupeCalcule as gr
  left outer join #sensibilite as rapport on rapport.groupe IN (select composant.groupeComposant from #GroupeCalculeComposant as composant where composant.groupeCalcule = gr.groupe)
 -- where gr.groupe IN ('ASSURANCE', 'RETRAITE', 'FGA') 
  group by  gr.groupe, rapport.pays,rapport.libelle
  
 insert into #encours(groupe,ordreGroupe,pays,ordrePays,rubrique,libelle,ordre,valeur)  
   select gr.groupe as groupe,
         gr.ordreGroupe,
		 rapport.pays,
		 rapport.ordrePays,
		 rapport.rubrique,
		 rapport.libelle,
		 rapport.ordre,
         SUM( rapport.valeur ) as valeur
  from #GroupeCalcule as gr
  left outer join #encours as rapport on rapport.groupe IN (select composant.groupeComposant from #GroupeCalculeComposant as composant where composant.groupeCalcule = gr.groupe)
  --where gr.groupe IN ('ASSURANCE', 'RETRAITE', 'FGA') 
  group by  gr.groupe,gr.ordreGroupe, rapport.pays,rapport.ordrePays,rapport.rubrique, rapport.libelle, rapport.ordre

  
 /**********************************************/
 /***              FIN  DONNEES CALCULEES  ****/
/**********************************************/

insert 
into  PTF_RAPPORT_NIV2 
(cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
select @cleReport as cle,
		 e.Groupe as groupe,		 
		 e.ordreGroupe , 
		 @dateInventaire as date,  
		 e.pays as rubrique,
		 e.rubrique as sousRubrique, -- ensemble des codes sous secteurs 
		 e.libelle as libelle,
		 e.ordrePays as classementRubrique ,
		 e.ordre as classementSousRubrique,
		 case 
			when e.valeur =0 then 0
			else d.valeur/e.valeur-- si les rendements ne sont pas renseignées : le résultat est nul, et la moyenne globale peut paraitre incorrecte
		 end as valeur
from #encours as e
  left outer join #sensibilite as d on d.groupe = e.groupe and d.pays = e.Pays and d.libelle = e.libelle
  order by ordreGroupe, ordrePays, ordre

 


drop table #Groupe
drop table #GroupeCalcule	
drop table #GroupeCalculeComposant
drop table #SecteurObligations
drop table #pays_zone
drop table #rapport_pays 
drop table #encours
drop table #sensibilite



GO

