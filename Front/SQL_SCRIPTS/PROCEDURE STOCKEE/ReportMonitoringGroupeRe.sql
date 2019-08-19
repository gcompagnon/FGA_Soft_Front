
/****** Object:  StoredProcedure [dbo].[ReportMonitoringGroupeRe]    Script Date: 09/18/2012 10:08:19 ******/
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


ALTER PROCEDURE [dbo].[ReportMonitoringGroupeRe] 
        @dateInventaire  datetime ,
        @niveauInventaire tinyint ,
        @cleReport char(20) OUTPUT
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
  set @cleReport = 'MonitoringGroupeRe'
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

create table #Groupe  ( groupe varchar(25) COLLATE DATABASE_DEFAULT primary key , ordreGroupe smallint null)
insert #Groupe values ('MM AGIRC',1)
insert #Groupe values ('MM ARRCO',2)

insert #Groupe values ('MMP',4)
insert #Groupe values ('INPR',5)
insert #Groupe values ('CAPREVAL',6)
insert #Groupe values ('CMAV',7)
insert #Groupe values ('MM MUTUELLE',8)
insert #Groupe values ('AUXIA',9)
insert #Groupe values ('QUATREM',10)
insert #Groupe values ('AUTRES',11)
declare @maxGroup int 
set @maxGroup = 12
--Les nouveaux groupes seront places arbitrairement à la suite


/** la table des colonnes calculees / des sur-groupes  **/
create table #GroupeCalcule  ( groupe varchar(25) COLLATE DATABASE_DEFAULT primary key , ordreGroupe tinyint null)
insert #GroupeCalcule values ('RETRAITE',3)
insert #GroupeCalcule values ('ASSURANCE',12)
insert #GroupeCalcule values ('FGA',0)

create table #GroupeCalculeComposant  ( groupeCalcule varchar(25)COLLATE DATABASE_DEFAULT, groupeComposant varchar(25) COLLATE DATABASE_DEFAULT null)
insert #GroupeCalculeComposant values ('RETRAITE', 'MM AGIRC')
insert #GroupeCalculeComposant values ('RETRAITE', 'MM ARRCO')
insert #GroupeCalculeComposant values ('ASSURANCE', 'MMP')
insert #GroupeCalculeComposant values ('ASSURANCE', 'MM MUTUELLE')
insert #GroupeCalculeComposant values ('ASSURANCE', 'CMAV')
insert #GroupeCalculeComposant values ('ASSURANCE', 'QUATREM')
insert #GroupeCalculeComposant values ('ASSURANCE', 'AUXIA')
insert #GroupeCalculeComposant values ('ASSURANCE', 'CAPREVAL')
insert #GroupeCalculeComposant values ('ASSURANCE', 'INPR')
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
 
/************************************************************************/
/***              DEBUT CALCUL DURATION PONDEREE                     ****/
/***        stockage du nominateur : SUM ( Duration * (Cours + CC ) )****/
/***        note: cela permet de construire les groupes calculés simplement***/
/***           Pour la Duration Ponderee: ne prendre que             ****/
/***           l encours des secteurs Obligataires                   ****/    
/************************************************************************/
 create table #rendement(groupe varchar (25) COLLATE DATABASE_DEFAULT ,ordreGroupe tinyint,rubrique varchar(120) COLLATE DATABASE_DEFAULT ,
						libelle varchar(60) COLLATE DATABASE_DEFAULT ,ordre float,valeur float)
    
 /********************************************************************/
 -- 1.0 ENCOURS
 -- les encours: somme de toutes les lignes 
set @incClassCompartiment = @incClassCompartiment + 1
  insert 
  into  [PTF_RAPPORT] 
  (cle,groupe,ordreGroupe, date,rubrique,libelle, classementRubrique,  valeur )
  select @cleReport as cle,
		 gr.Groupe as groupe,		 
		 gr.ordreGroupe , 
		 @dateInventaire,  
		 'encours' as rubrique,
		 'Encours',
		 @incClassCompartiment ,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from #Groupe as gr 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = gr.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  group by  gr.Groupe,gr.ordreGroupe

  
      
  /**********************************/
  -- 2.0 COMPARTIMENT OBLIGATAIRE  @incClassCompartiment = 1  
  --  TOTAL Obligations (doit etre à la somme EMPRUNT D'ETAT, AGENCIES, EMPRUNT FONCIER ET HYPO, CORPORATES, OPCVM OBLIG et AUTRES )
  -- pour les type_actif = obligations  et oc (meme si la conversion est en actions) et les cas particuliers (dérivés PUT CALL sur oblig)
  /*********************************/
  set @incClassCompartiment = @incClassCompartiment + 1
  set  @incClassRubrique = 0
  insert 
  into  #rendement
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select gr.groupe as groupe,
		 gr.ordreGroupe as ordreGroupe,		 
		  'OBLIGATIONS - Total' as rubrique,
 		  'OBLIGATIONS - Total' as libelle,
		  @incClassCompartiment,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from  #groupe  as gr 
  left outer join [PTF_TRANSPARISE] as tr   
  on tr.Groupe = gr.Groupe  
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and (  tr.Type_actif IN ( 'Obligations', 'Obligations Convertibles' ) 
  or    secteur in (  select libelle from #secteurObligations )  )
  group by  gr.groupe,  gr.ordreGroupe
  

      
/********************************************************************/
 -- 2.1 les emprunts etats  / Sovereigns / Govies @incClassCompartiment=1  @incClassRubrique=0
  -- liste des couples Groupe, sous secteurs des emprunts d etat avec la clé id_secteur='O GVT'
  --- liste des pays émetteurs de dettes en zone euro ( à supprimer lorsque que l'on aura une classification plus granulaire)  
  set @incClassRubrique = 1
    
  select sousSecteur.id, sousSecteur.libelle
  into #secteurEtatsZoneEuro
  from [SOUS_SECTEUR] as sousSecteur
  where  sousSecteur.id in ('O GVT ALL','O GVT AUT','O GVT BEL','O GVT ESP','O GVT FIN','O GVT FRA','O GVT GRE','O GVT HON','O GVT IRL','O GVT ITA','O GVT PB','O GVT POR','O GVT SUE' )
  
   --select * from  #secteurEtatsZoneEuro  
  select  gr.groupe ,gr.ordreGroupe,
  CASE when sousSecteur.id IN (select id from  #secteurEtatsZoneEuro )   
       then 'GOVIES_EURO' 
       else 'GOVIES_AUTRES'
  END as 'rubriquePrincipale',
  sousSecteur.libelle as 'rubrique',
  CASE when sousSecteur.id IN (select id from  #secteurEtatsZoneEuro ) 
       then 100* @incClassCompartiment+@incClassRubrique+0.1  -- les govies Zone Europe  seront consolidées
       else 100* @incClassCompartiment+@incClassRubrique+1    -- les govies des  zones ex Euro
  END as 'classement',
   CASE  when sousSecteur.id IN (select id from  #secteurEtatsZoneEuro ) 
        then REPLACE ( sousSecteur.libelle , 'EMPRUNT D''ETAT' , '            ' ) 
		else  sousSecteur.libelle 
  END as 'libelle'
  into #groupe_secteurEtats 
  from #groupe as gr,
  [SOUS_SECTEUR] as sousSecteur
  where sousSecteur.id_secteur = 'O GVT'   
      
  /** consolidation des Sovereign hors USA */
  insert 
  into  #rendement
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		grSecteur.ordreGroupe as ordreGroupe,
		  'GOVIES_EURO' as rubrique,
		  'EMPRUNT D''ETAT Zone Euro' as libelle,
		 100* @incClassCompartiment+@incClassRubrique ,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from #groupe_secteurEtats as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and tr.Sous_Secteur = grSecteur.rubrique
  and Dateinventaire = @dateInventaire  
  and Numero_Niveau = @niveauInventaire
  where grSecteur.rubriquePrincipale = 'GOVIES_EURO' -- seulement les govies en zone Euro
  group by  grSecteur.groupe,grSecteur.ordreGroupe 
   
  
  insert 
  into  #rendement
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		grSecteur.ordreGroupe as ordreGroupe,
		  grSecteur.rubrique as rubrique,
		  grSecteur.libelle as libelle,
		  grSecteur.classement,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from #groupe_secteurEtats as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and tr.Sous_Secteur = grSecteur.rubrique
  and Dateinventaire = @dateInventaire  
  and Numero_Niveau = @niveauInventaire
  group by  grSecteur.groupe,grSecteur.ordreGroupe, grSecteur.rubrique ,grSecteur.libelle,grSecteur.classement
 
  /********************************************************************/
  -- 2.2 les agencies 
  -- liste des couples Groupe, sous secteurs Agencies par Pays  
    
  set @incClassRubrique = @incClassRubrique+ 2

  -- agencies zone Euro
  select  gr.groupe ,gr.ordreGroupe, 'AGENCIES-Euro' as 'rubriquePrincipale', secteur.libelle as 'libelle',pays.libelle as 'pays'
  into #groupe_rubriqueAgencies 
  from #groupe as gr,
  [SOUS_SECTEUR] as secteur,
  [PAYS] as pays
  left outer join [ZONE_GEOGRAPHIQUE] as zone on zone.Pays  = pays.libelle
  where   
  secteur.id_secteur = 'O AGENCIES'
  and zone.Zone = 'Zone Euro'
  
    
  -- autres pays  
  insert into #groupe_rubriqueAgencies 
  select  gr.groupe ,gr.ordreGroupe,'AGENCIES-AUTR', secteur.libelle as 'libelle', pays.libelle as 'pays'
  from #groupe as gr,
  [SOUS_SECTEUR] as secteur,
  [PAYS] as pays  
  where   
  secteur.id_secteur = 'O AGENCIES'
  and Pays.libelle not in (select Pays from [ZONE_GEOGRAPHIQUE]   where Zone = 'Zone Euro')
  
 -- select * from #groupe_rubriqueAgencies order by ordreGroupe 
  
  -- total Agencies
 insert 
  into  #rendement
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		grSecteur.ordreGroupe as ordreGroupe,
		  'AGENCIES' as rubrique,
		  'AGENCIES',
		  100* @incClassCompartiment+@incClassRubrique,
		  SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from #groupe_rubriqueAgencies as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire  
  and tr.Sous_Secteur = grSecteur.libelle
  and tr.Pays = grSecteur.pays
  where grSecteur.rubriquePrincipale =  'AGENCIES-Euro'  
   group by  grSecteur.groupe,  grSecteur.ordreGroupe
  
  -- consolidation des agencies en pays "autres"
insert 
  into  #rendement
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
    select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		 'AGENCIES-AUTRES' as rubrique,
		 'AGENCIES Autres pays' as libelle ,
		 100* @incClassCompartiment+@incClassRubrique+1 as classement,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from #groupe_rubriqueAgencies as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Sous_Secteur = grSecteur.libelle
  and tr.Pays =  grSecteur.pays
  where grSecteur.rubriquePrincipale =  'AGENCIES-AUTR'  
  group by  grSecteur.groupe,  grSecteur.ordreGroupe
  
  
  -- ventilation sur les pays zones euro
  insert 
  into  #rendement
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,		 
		  'AGENCIES-Euro-'+grSecteur.pays as rubrique,
		  '            '+grSecteur.pays as libelle,
		 100* @incClassCompartiment+@incClassRubrique+0.1 as classementRubrique,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from #groupe_rubriqueAgencies  as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
   and Dateinventaire = @dateInventaire  
  and Numero_Niveau = @niveauInventaire
  and tr.Sous_Secteur =  grSecteur.libelle
  and tr.Pays =  grSecteur.pays
  where  grSecteur.rubriquePrincipale = 'AGENCIES-Euro'
    group by  grSecteur.groupe,grSecteur.ordreGroupe,grSecteur.pays
    
   
  /********************************************************************/
  -- 2.3 2.4 2.5 : ZONE Pays  - regroupement des lignes/rubriques par groupe
  -- ZONE 1  TOUS sauf ZONE 1 
  -- ZONE 2( les sovereigns et les agencies pour les PIIGS )
  -- France (les lignes sovereigns et agencies)
    
  set @incClassRubrique = @incClassRubrique+ 2
    
    -- une table temporaire pour la definition des zones personnalisees (zones geographiques)
  -- TODO : a mettre dans le MDD avec une specification du calcul
  select gr.groupe ,gr.ordreGroupe,'ZONE 1 Etat+Agences' as zone, 100*@incClassCompartiment+@incClassRubrique  as ordre
  into #groupe_zone 
  from #groupe as gr
  
  set @incClassRubrique = @incClassRubrique+ 1
  
  insert into #groupe_zone 
  select gr.groupe ,gr.ordreGroupe,'ZONE 2 Etat+Agences' as zone, 100*@incClassCompartiment+@incClassRubrique as ordre
  from #groupe as gr
      
  set @incClassRubrique = @incClassRubrique+ 1      
      
  insert into #groupe_zone 
  select distinct gr.groupe ,gr.ordreGroupe,'France Etat+Agences' as zone, 100*@incClassCompartiment+@incClassRubrique as ordre
  from #groupe as gr
  

  --Calcul ZONE 2 : prendre les PIIGS
  create table #pays_PIIGS_emprunts ( libelle varchar(60) COLLATE DATABASE_DEFAULT  not null)
  insert into #pays_PIIGS_emprunts values ('EMPRUNT D''ETAT PORTUGAIS')
  insert into #pays_PIIGS_emprunts values ('EMPRUNT D''ETAT IRLANDAIS')
  insert into #pays_PIIGS_emprunts values ('EMPRUNT D''ETAT ITALIEN')
  insert into #pays_PIIGS_emprunts values ('EMPRUNT D''ETAT GREC')
  insert into #pays_PIIGS_emprunts values ('EMPRUNT D''ETAT ESPAGNOL')
  insert into #pays_PIIGS_emprunts 
  select 'AGENCIES-Euro-'+pays.libelle
  from [PAYS] as pays
  where pays.id IN ('PT', 'IT','IE','GR','ES')
  
  insert 
  into  #rendement
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
   select grZ.groupe as groupe,
		 grZ.ordreGroupe as ordreGroupe,
		  grZ.zone as rubrique,
		  grZ.zone as libelle,
		  grZ.ordre as classementRubrique,
         SUM( rapport.valeur ) as valeur
  from #groupe_zone  as grZ 
  left outer join #rendement as rapport 
  on rapport.groupe = grZ.groupe    
  and rapport.rubrique IN (select libelle from #pays_PIIGS_emprunts)
  where grZ.zone = 'ZONE 2 Etat+Agences'   
  group by  grZ.groupe,  grZ.ordreGroupe, grZ.zone, grZ.ordre
      
    --Calcul ZONE 1 : l ensemble des positions Sovereign et Agencies - Zone 2
 insert 
  into  #rendement
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grZ.groupe as groupe,
		 grZ.ordreGroupe as ordreGroupe,
		 grZ.zone as rubrique,
		  grZ.zone as rubrique,
		  grZ.ordre  as ordre,
         SUM( rapport.valeur ) as valeur
  from #groupe_zone  as grZ 
  left outer join #rendement as rapport 
  on rapport.groupe = grZ.groupe    
  and ( rapport.rubrique = 'AGENCIES' or rapport.rubrique like 'EMPRUNT D''ETAT%' )
  where grZ.zone = 'ZONE 1 Etat+Agences'   
  group by  grZ.groupe,  grZ.ordreGroupe, grZ.zone, grZ.ordre
    
  update rapportZone1
   set valeur = rapportZone1.valeur - rapportZone2.valeur
    from #rendement  as rapportZone1 
   left outer join #rendement  as rapportZone2 
   on rapportZone2.rubrique = 'ZONE 2 Etat+Agences'
						and rapportZone2.groupe = rapportZone1.groupe
   where rapportZone1.rubrique = 'ZONE 1 Etat+Agences'
  
  
  --Calcul ZONE France
   insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grZ.groupe as groupe,
		 grZ.ordreGroupe as ordreGroupe,
		  grZ.zone as rubrique,
		  grZ.zone as libelle,
		  grZ.ordre ,
         SUM( rapport.valeur ) as valeur
  from #groupe_zone  as grZ 
  left outer join #rendement as rapport 
  on rapport.groupe = grZ.groupe    
  and rapport.rubrique IN ('AGENCIES-Euro-France','EMPRUNT D''ETAT FRANCAIS')
  where grZ.zone = 'France Etat+Agences' 
  group by  grZ.groupe,  grZ.ordreGroupe, grZ.zone, grZ.ordre
  
  /**********************************/
  -- 2.6 EMPRUNT FONCIER ET HYPOTHECAIRE
  -- liste des couples Groupe, sous secteurs des emprunts d etat, hors américain
  set @incClassRubrique = @incClassRubrique+ 1
  
  select  gr.groupe ,gr.ordreGroupe, secteur.libelle as 'secteurLibelle', pays.libelle as 'paysLibelle'
   into #groupe_secteurFoncierHypo 
  from #groupe as gr,
  [SOUS_SECTEUR] as secteur,
  [PAYS] as pays
  left outer join [ASSOCIATION_PAYS_ZONE] as zone on zone.id_pays = pays.id
  where   
  secteur.id_secteur = 'O COVERED'
  and zone.id_zone = 'ZoneE'
  order by  gr.Groupe , secteur.libelle, pays.libelle 
    
insert into #groupe_secteurFoncierHypo
select distinct gr.groupe ,gr.ordreGroupe, 'EMPRUNT FONCIER ET HYPOTHECAIRE' as 'secteurLibelle', '' as 'paysLibelle'
    from #groupe as gr
  order by  gr.Groupe
  
  
  /** consolidation sur le covered en Zone Euro */ 
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		  'EMPRUNT FONCIER ET HYPOTHECAIRE' as rubrique,
		  'EMPRUNT FONCIER ET HYPOTHECAIRE' as libelle,
		 100*@incClassCompartiment+@incClassRubrique,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from #groupe_secteurFoncierHypo  as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire  
  and Numero_Niveau = @niveauInventaire
  and tr.Sous_Secteur = grSecteur.secteurLibelle
  and tr.Pays in (select paysLibelle from  #groupe_secteurFoncierHypo where paysLibelle<>'')
  where  grSecteur.paysLibelle = ''
  group by  grSecteur.groupe,  grSecteur.ordreGroupe
  
  /** le covered sur chacun des pays Zone Euro */
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		  grSecteur.secteurLibelle+ ' ' + grSecteur.paysLibelle as rubrique,
		  '            ' + grSecteur.paysLibelle as libelle,
		 100*@incClassCompartiment+@incClassRubrique+0.1,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from #groupe_secteurFoncierHypo  as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
   and Dateinventaire = @dateInventaire  
  and Numero_Niveau = @niveauInventaire
  and tr.Sous_Secteur =  grSecteur.secteurLibelle
  and tr.Pays =  grSecteur.paysLibelle
  where  grSecteur.paysLibelle <> '' 
    group by  grSecteur.groupe,grSecteur.ordreGroupe,grSecteur.secteurLibelle,grSecteur.paysLibelle
  
 
  /** le covered sur autres pays */
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		  'EMPRUNT FONCIER ET HYPOTHECAIRE_Autres' as rubrique,
		  'EMPRUNT Ex Euro' as libelle,
          100*@incClassCompartiment+@incClassRubrique+1,
          SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from #groupe_secteurFoncierHypo  as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
   and Dateinventaire = @dateInventaire  
  and Numero_Niveau = @niveauInventaire
  and tr.Sous_Secteur =  grSecteur.secteurLibelle
  and tr.Pays not in (select paysLibelle from  #groupe_secteurFoncierHypo where paysLibelle<>'')
  where  grSecteur.paysLibelle = '' 
    group by  grSecteur.groupe,grSecteur.ordreGroupe,grSecteur.secteurLibelle,grSecteur.paysLibelle
  
 
  /**********************************/
  -- 2.7 CORPORATES consolidés : l'ensemble des titres en secteurs "O CNF" et "O CF"
  -- nécessite les 2étapes precedentes
    
  set @incClassRubrique = @incClassRubrique+ 2
  
  select  gr.groupe ,gr.ordreGroupe, 'CORPORATES-TOTAL' as 'rubrique',
   secteur.libelle as 'codeSecteur'  
   into #groupe_secteurCorporate
  from #groupe as gr,
  [SECTEUR] as secteur
  where   
  secteur.id like 'O CF%' or secteur.id like 'O CNF%'
  
  -- select * from #groupe_secteurCorporate
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
   select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		  'CORPORATES-TOTAL' as rubrique,
 		  'CORPORATES' as libelle,
		 100*@incClassCompartiment+@incClassRubrique,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from  #groupe_secteurCorporate  as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Secteur = grSecteur.codeSecteur    
  group by  grSecteur.groupe,  grSecteur.ordreGroupe
  
  /**********************************/
  -- 2.71 CORPORATES Financieres : 3 secteurs commencant par "O CF"
  -- liste des couples Groupe, sous secteurs des coporates Fi et non Fi  
    
  select  gr.groupe ,gr.ordreGroupe, 'CORPORATES FINANCIERES' as 'secteurLibelle',
  secteur.libelle as 'codeSecteur',
  pays.libelle as 'paysLibelle'
   into #groupe_secteurCorporateFi 
  from #groupe as gr,
  [SECTEUR] as secteur,
  [PAYS] as pays
  left outer join [ZONE_GEOGRAPHIQUE] as zone on zone.Pays = pays.libelle
  where   
  secteur.id like 'O CF %'
  and zone.Zone = 'Zone Euro'
  order by  gr.Groupe , secteur.libelle, pays.libelle 
        
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		  'CORPORATES FINANCIERES ' + grSecteur.paysLibelle as rubrique,
		  '            ' + grSecteur.paysLibelle as rubrique,
		 100*@incClassCompartiment+ @incClassRubrique+0.11,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite )
  from  #groupe_secteurCorporateFi   as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Secteur = grSecteur.codeSecteur
  and tr.Pays = grSecteur.paysLibelle
  group by  grSecteur.groupe,  grSecteur.ordreGroupe, grSecteur.paysLibelle 
  
  -- pour chaque groupe, le reliquat qui est sur les pays non ventilé
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		 'CORPORATES FINANCIERES Autres pays' as rubrique,
		  '            Autres pays' as libelle,
		  100*@incClassCompartiment+ @incClassRubrique+0.12,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from  (select distinct groupe,ordreGroupe,secteurLibelle,codeSecteur from #groupe_secteurCorporateFi) as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Secteur = grSecteur.codeSecteur
  and tr.Pays not in (select distinct paysLibelle from #groupe_secteurCorporateFi  )  
  group by  grSecteur.groupe,  grSecteur.ordreGroupe, grSecteur.secteurLibelle
      
  -- consolidations des corporates fi (tous les pays)
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		  'CCORPORATES FINANCIERES' as rubrique,
          'CORPORATES FINANCIERES' as libelle,
		  100*@incClassCompartiment+ @incClassRubrique+0.1,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from  (select distinct groupe,ordreGroupe,secteurLibelle,codeSecteur from #groupe_secteurCorporateFi) as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Secteur = grSecteur.codeSecteur
  group by  grSecteur.groupe,  grSecteur.ordreGroupe
  
  
  /**********************************/
  -- 2.72 CORPORATES Non Financieres : secteur commencant par "O CNF"
  
  select  gr.groupe ,gr.ordreGroupe, 'CORPORATES NON FINANCIERES' as 'secteurLibelle',
  secteur.libelle as 'codeSecteur',
  pays.libelle as 'paysLibelle'
   into #groupe_secteurCorporateNonFi 
  from #groupe as gr,
  [SECTEUR] as secteur,
  [PAYS] as pays
  left outer join [ZONE_GEOGRAPHIQUE] as zone on zone.Pays = pays.libelle
  where   
  secteur.id like 'O CNF%'
  and zone.Zone = 'Zone Euro'
  order by  gr.Groupe , secteur.libelle, pays.libelle 
   
 -- select * from  #groupe_secteurCorporateNonFi
   
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		  'CORPORATES NON FINANCIERES ' + grSecteur.paysLibelle as rubrique,
 		  '            ' + grSecteur.paysLibelle as rubrique,
		 100*@incClassCompartiment+ @incClassRubrique+0.21,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from  #groupe_secteurCorporateNonFi  as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Secteur = grSecteur.codeSecteur
  and tr.Pays = grSecteur.paysLibelle
  group by  grSecteur.groupe,  grSecteur.ordreGroupe, grSecteur.paysLibelle 
  
  
  -- pour chaque groupe, le reliquat qui est sur les pays non ventilé
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		  'CORPORATES NON FINANCIERES Autres pays' as rubrique,
		  '            Autres pays' as rubrique,
		 100*@incClassCompartiment+ @incClassRubrique+0.22 as ordre,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from  (select distinct groupe,ordreGroupe,secteurLibelle,codeSecteur from #groupe_secteurCorporateNonFi)   as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Secteur = grSecteur.codeSecteur
  and tr.Pays not in (select distinct paysLibelle from #groupe_secteurCorporateNonFi where groupe = grSecteur.groupe )  
  group by  grSecteur.groupe,  grSecteur.ordreGroupe
  
   
  -- consolidations des corporates non fi pour tous pays
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,
		  'CORPORATES NON FINANCIERES' as libelle,
  		  'CORPORATES NON FINANCIERES' as libelle,
		 100*@incClassCompartiment+ @incClassRubrique+0.2 as ordre,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Sensibilite )
  from  (select distinct groupe,ordreGroupe,secteurLibelle,codeSecteur from #groupe_secteurCorporateNonFi)  as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Secteur = grSecteur.codeSecteur
  group by  grSecteur.groupe,  grSecteur.ordreGroupe
     

  /**********************************/
  -- DECOMPOSITION FONDS et Actions
  -- 2.8 FONDS OBLIG
  -- 2.9 Autres dans les Obligs 
  
  /*********************************/  
    
  set @incClassRubrique = @incClassRubrique+ 1
          
  create table #groupe_ActionSecteur  ( groupe varchar(25) COLLATE DATABASE_DEFAULT , ordreGroupe int null, rubriquePrincipale varchar(30) COLLATE DATABASE_DEFAULT , secteurLibelle varchar (30) COLLATE DATABASE_DEFAULT ,
  classifRubrique float, codeSecteur varchar(60) COLLATE DATABASE_DEFAULT )
  
  create table #groupe_ActionSousSecteur ( groupe varchar(25) COLLATE DATABASE_DEFAULT , ordreGroupe int null,rubriquePrincipale varchar(30) COLLATE DATABASE_DEFAULT , secteurLibelle varchar (60) COLLATE DATABASE_DEFAULT ,
  classifRubrique float, codeSousSecteur varchar(60) COLLATE DATABASE_DEFAULT )
  
  create table #groupe_ActionTypeProduit ( groupe varchar(25) COLLATE DATABASE_DEFAULT , ordreGroupe int null,rubriquePrincipale varchar(30) COLLATE DATABASE_DEFAULT , secteurLibelle varchar (30) COLLATE DATABASE_DEFAULT ,
  classifRubrique float, codeTypeProduit varchar(60) COLLATE DATABASE_DEFAULT )
  

  create table #groupe_TypeActif ( groupe varchar(25) COLLATE DATABASE_DEFAULT , ordreGroupe int null,rubriquePrincipale varchar(30) COLLATE DATABASE_DEFAULT , secteurLibelle varchar (30) COLLATE DATABASE_DEFAULT ,
  classifRubrique float, codeTypeActif varchar(60) COLLATE DATABASE_DEFAULT )  
  
  /** 2.8 Compartiment OPCVM OBLIG est cree avec le critere secteur = 'F OBLIG' */
  insert into #groupe_ActionSecteur  (groupe, ordreGroupe,rubriquePrincipale, secteurLibelle, classifRubrique, codeSecteur)
  select gr.groupe ,gr.ordreGroupe,'OPCVM OBLIG','OPCVM OBLIG', 100*@incClassCompartiment+ @incClassRubrique,  secteur.libelle as 'codeSecteur' 
  from #groupe as gr,
  [SECTEUR] as secteur
   where secteur.id = 'F OBLIG'
          
  -- 2.9 TOTAL des Autres Obligations 
  -- pour les type_actif = obligations  ou convertible , et hors les secteurs obligataires déjà sélectionnés précédemment 
  -- ce sont les dérivés comme PUT CALL sur oblig ou Swap sur taux
   set @incClassRubrique  = @incClassRubrique + 1
    
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select gr.groupe as groupe,
		 gr.ordreGroupe as ordreGroupe,
		  'OBLIGATIONS - Autres' as rubrique,
 		  'OBLIGATIONS - (dérivés)' as libelle,
		  100*@incClassCompartiment+ @incClassRubrique as ordre,
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from  #groupe  as gr 
  left outer join [PTF_TRANSPARISE] as tr   
  on tr.Groupe = gr.Groupe  
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and ( tr.Type_actif IN ( 'Obligations', 'Obligations Convertibles') 
           and ( secteur is null or secteur not in (  select libelle from #secteurObligations ) )  )
  group by  gr.groupe,  gr.ordreGroupe
    
    
   -- classification sur les secteurs
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select grSecteur.groupe as groupe,
		 grSecteur.ordreGroupe as ordreGroupe,		 
		  grSecteur.codeSecteur as rubrique,
		  grSecteur.secteurLibelle as libelle,
		 grSecteur.classifRubrique,
          SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from  #groupe_ActionSecteur  as grSecteur 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = grSecteur.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Secteur = grSecteur.codeSecteur  
  group by  grSecteur.groupe,  grSecteur.ordreGroupe, grSecteur.codeSecteur, grSecteur.secteurLibelle,grSecteur.classifRubrique
 
 /**********************************************/
 /***              FIN  DONNEES AGREGEES    ****/
/**********************************************/
 
 
 /**********************************************/
 /***              DEBUT  DONNEES NON CLASSIFIEES ****/
/**********************************************/
    set @incClassCompartiment  = @incClassCompartiment+1    
    
  -- classification sur le sous secteur de produit
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select gr.groupe as groupe,
		 gr.ordreGroupe as ordreGroupe,
		  'NON CLASSIFIES',
		  'NON CLASSIFIES',
		 @incClassCompartiment , 
         SUM( ( Valeur_Boursiere + Coupon_Couru ) * Rendement )
  from  #groupe  as gr 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Groupe = gr.Groupe
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and secteur not in ( select libelle  from [SECTEUR] 
                       where ( id  like 'A %'
                            or id = 'F ACTIONS' 
                            or id  = 'F DIVERS' 
                            -- or id = 'F TRESO'
                            or id like 'O CNF%' 
							or id like 'O CF%' 
							or id= 'F OBLIG' 
							or id =  'O GVT' 
							or id = 'O AGENCIES'
							or id = 'O COVERED' ) )  
  and (type_actif is null or type_actif not in ('Monétaire') )
  and ( duration is not null or duration <> 0 )
  group by  gr.groupe,  gr.ordreGroupe
  
  
 /**********************************************/
 /***              FIN  DONNEES AGREGEES    ****/
/**********************************************/
 
  
 
 /**********************************************/
 /***              DEBUT  DONNEES CALCULEES ****/
/**********************************************/
 
  -- specification des colonnes RETRAITE et ASSURANCE
  insert 
  into  #rendement 
  (groupe,ordreGroupe,rubrique,libelle,ordre,valeur)
  select gr.groupe as groupe,
		 gr.ordreGroupe as ordreGroupe, 
		 rapport.rubrique as rubrique,
		 rapport.libelle as libelle,
		 rapport.ordre,
         SUM( rapport.valeur ) as valeur
  from #GroupeCalcule as gr, #rendement as rapport 
   where rapport.groupe IN (select composant.groupeComposant from #GroupeCalculeComposant as composant where composant.groupeCalcule = gr.groupe)
    group by  rapport.rubrique, rapport.libelle, rapport.ordre , gr.groupe, gr.ordreGroupe
  
 /**********************************************/
 /***              FIN  DONNEES CALCULEES  ****/
/**********************************************/

 /**********************************************/
 /***         DEBUT  CALCUL PONDERATION      ***/
 /***       prendre le nominateur duration   ***/
 /***et le denominateur cle= 'MonitoringGroupe'*/
/***********************************************/
 /*
  -- specification des colonnes RETRAITE et ASSURANCE
  insert 
  into  [PTF_RAPPORT] 
  (cle,groupe, ordreGroupe,date,rubrique,libelle, classementRubrique,  valeur )
  select @cleReport as cle,
		 dur.groupe as groupe,
		 dur.ordreGroupe as ordreGroupe, 
		 @dateInventaire, 
		 dur.rubrique as rubrique,
		 dur.libelle as libelle,
		 dur.ordre  as classementRubrique,
         dur.valeur / ( NULLIF( rapport.valeur,0 ) ) as valeur
  from #rendement as dur
  left outer join [PTF_RAPPORT] as rapport on rapport.date = @dateInventaire and rapport.cle = 'MonitoringGroupe'  
   where rapport.groupe = dur.groupe and rapport.rubrique = dur.rubrique
    
  */
 /**********************************************/
 /***              FIN  DONNEES CALCULEES  ****/
/**********************************************/


insert 
into  PTF_RAPPORT
(cle,groupe,ordreGroupe,date,rubrique,libelle,classementRubrique,valeur)
select @cleReport as cle,
		 dur.groupe as groupe,
		 dur.ordreGroupe as ordreGroupe, 
		 @dateInventaire as date, 
		 dur.rubrique as rubrique,
		 dur.libelle as libelle,
		 dur.ordre  as classementRubrique,
         dur.valeur / ( NULLIF( rapport.valeur,0 ) ) as valeur
from #rendement as dur
  left outer join [PTF_RAPPORT] as rapport on rapport.date = @dateInventaire and rapport.cle = 'MonitoringGroupe'  
   where rapport.groupe = dur.groupe and rapport.rubrique = dur.rubrique
   ORDER BY ordreGroupe, classementRubrique





 --return 0
 /*** execution en dehors de la procédure */
drop table #rendement
drop table #Groupe
drop table #GroupeCalcule
drop table #GroupeCalculeComposant
drop table #secteurObligations
drop table #secteurEtatsZoneEuro
drop table #groupe_secteurEtats 
drop table #groupe_rubriqueAgencies 
drop table #pays_PIIGS_emprunts
drop table #groupe_zone
drop table #groupe_secteurFoncierHypo  
drop table #groupe_secteurCorporate 
drop table #groupe_secteurCorporateFi 
drop table #groupe_secteurCorporateNonFi 
drop table #groupe_ActionSecteur 
drop table #groupe_ActionSousSecteur
drop table #groupe_ActionTypeProduit
drop table #groupe_TypeActif


GO

