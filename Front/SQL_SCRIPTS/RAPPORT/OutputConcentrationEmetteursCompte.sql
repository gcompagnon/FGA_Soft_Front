 
/** configuration du rapport Concentration par emetteurs:
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
-- DROP PROCEDURE ReportConcentrationEmetteurs

/*
CREATE PROCEDURE ReportConcentrationEmetteurs 
        @dateInventaire  datetime ,
        @niveauInventaire tinyint ,
        @rapportCle char(20) OUTPUT,
        @NatureEmetteurs table (
                            natureEmetteur char(6)
                            )OUTPUT
AS
*/
/** Parametres: --------------------------------------**/

declare @dateInventaire as datetime 
set @dateInventaire = '12/10/2011'
declare @niveauInventaire as float
set @niveauInventaire = 2
declare @NatureEmetteurs table (
                            natureEmetteur char(6)
                            )
declare @rapportCle as char(20)                            

drop table #Groupe
drop table #GroupeCalcule
drop table #GroupeCalculeComposant
drop table #groupe_secteurEtats 
drop table #Seniorite
drop table #groupe_type 
drop table #groupe_emetteur
drop table #perimetre_emetteur 


-- la liste des natures emetteurs selectionnées
IF NOT EXISTS (SELECT * FROM @NatureEmetteurs )
BEGIN
insert into @NatureEmetteurs 
 select codeNatureEmetteur from NATURE_EMETTEUR_OMEGA
END



-- construction de la table de décomposition avec le traitement particulier des Corporates Fi:
	-- niveau CorpoFinanciere
	create table #Seniorite  (categorie varchar(15), sousSecteur varchar(60), ordre float)
	insert #Seniorite values ('SENIOR', 'EMPRUNT SENIOR ASSURANCE', 1)
	insert #Seniorite values ('SENIOR', 'EMPRUNT SENIOR BANQUE', 1)
    insert #Seniorite values ('SENIOR', 'EMPRUNT SERVICES FINANCIERS',1)		
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT LOWER TIER 2 BANQUE', 2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT UPPER TIER 2 BANQUE',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT TIER 1 BANQUE',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT SUBORDONNE ASSURANCE',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT PERPETUEL ASSURANCE',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT PERPETUEL SERV FINANCIERS',2)
	insert #Seniorite values ('COVERED', 'EMPRUNT FONCIER ET HYPOTHECAIRE',3)


IF @rapportCle IS NULL
BEGIN
set @rapportCle = 'ConcentEmetteurs_PEREObligations'
END




-- test d existence de la date demandée pour l'inventaire
/** TODO **/
--select distinct DateInventaire from  [E2FGATP25].[dbo].[PTF_TRANSPARISE] order by DateInventaire
-- select distinct Groupe from  [E2FGATP25].[dbo].[PTF_TRANSPARISE] where DateInventaire = '2011-08-24'

-- suppression pour la date demandée mais on pourra archiver sur une autre table ces rapports 
delete from PTF_RAPPORT_NIV2  
   where date = @dateInventaire 
   and cle = @rapportCle

declare @incClassCompartiment as float /** 1 pour encours, 2 pour les états ... **/
set @incClassCompartiment = 1

-- utilisation de la requete pour les comptes
create table #Groupe  ( groupe varchar(15) primary key , ordreGroupe smallint null)
insert #Groupe values ('4010015',0)

/** la table des colonnes calculees / des sur-groupes  **/
create table #GroupeCalcule  ( groupe varchar(15) primary key , ordreGroupe tinyint null)
insert #GroupeCalcule values ('POIDS 4010015',1)

-- le poids est calculé sur les entités Assurance ou Retraite --
create table #GroupeCalculeComposant  ( groupeCalcule varchar(15), groupeComposant varchar(15) null)
insert #GroupeCalculeComposant values ('POIDS 4010015', '4010015')

    
 /********************************************************************/
 -- 1.0 ENCOURS
 -- les encours: somme de toutes les lignes 
 
 insert 
  into  PTF_RAPPORT_NIV2 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @rapportCle as cle,
		 gr.Groupe as groupe,		 
		 gr.ordreGroupe , 
		 @dateInventaire,  
		 'ENCOURS' as rubrique,		 
		 'Encours' as sousRubrique,
		 'Encours' as libelle,
		  @incClassCompartiment ,
		  0,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeurf
  from #Groupe as gr 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Compte = gr.Groupe
  and tr.Dateinventaire =  @dateInventaire
  and tr.Numero_Niveau = @niveauInventaire  
  group by  gr.Groupe,gr.ordreGroupe
/********************************************************************/
 -- 2.1 les emprunts etats  / Sovereigns / Govies @incClassCompartiment=1  @incClassRubrique=0
  -- liste des couples Groupe, sous secteurs des emprunts d etat avec la clé id_secteur='O GVT'
  --- liste des pays émetteurs de dettes en zone euro ( à supprimer lorsque que l'on aura une classification plus granulaire)  
  set @incClassCompartiment = @incClassCompartiment + 1
  
  select  gr.groupe ,gr.ordreGroupe,
  sousSecteur.libelle as 'rubrique',
  REPLACE ( sousSecteur.libelle , 'EMPRUNT D''ETAT' , '            ' ) as 'libelle'
  into #groupe_secteurEtats 
  from #groupe as gr,
  [dbo].[SOUS_SECTEUR] as sousSecteur
  where sousSecteur.id_secteur = 'O GVT'   
  
  --  consolidation des Sovereign --
insert 
  into  PTF_RAPPORT_NIV2 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @rapportCle as cle,
		grSecteur.groupe as groupe,
		grSecteur.ordreGroupe as ordreGroupe,
		 @dateInventaire,
		  'PAYS_GOVIES' as rubrique,
		  'TOTAL' as sousRubrique,		  
		  'EMPRUNT D''ETAT' as libelle,
		 @incClassCompartiment,
		 1,
         SUM( Valeur_Boursiere + Coupon_Couru )
  from #groupe_secteurEtats as grSecteur 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Compte = grSecteur.Groupe
  and tr.Sous_Secteur = grSecteur.rubrique
  and Dateinventaire = @dateInventaire  
  and Numero_Niveau = @niveauInventaire 
  group by  grSecteur.groupe,grSecteur.ordreGroupe 

 
  -- total par pays et par groupe --
  insert 
  into  PTF_RAPPORT_NIV2
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @rapportCle as cle,
		grSecteur.groupe as groupe,
		grSecteur.ordreGroupe as ordreGroupe,
		 @dateInventaire as date,
		 'PAYS_GOVIES' as rubrique,
		 'PAYS_'+grSecteur.rubrique as sousRubrique,
		  grSecteur.libelle as libelle,
		  @incClassCompartiment as classementRubrique,
		  2 as classementSousRubrique,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from #groupe_secteurEtats as grSecteur 
  left outer join [dbo].[PTF_TRANSPARISE] as tr 
  on tr.Compte = grSecteur.Groupe
  and tr.Sous_Secteur = grSecteur.rubrique
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  group by  grSecteur.groupe,grSecteur.ordreGroupe, grSecteur.rubrique ,grSecteur.libelle

/********************************************************************/
  
  create table  #groupe_type (groupe varchar(20), ordreGroupe int, rubrique varchar(120), ordreRubrique float, libelle varchar(60), code varchar(60))  

 
  insert into #groupe_type (groupe, ordreGroupe, rubrique, ordreRubrique, libelle, code)  
  select gr.groupe ,gr.ordreGroupe, 'ACTIONS' ,1,  'ACTIONS (direct et fonds)' , sousSecteur.libelle
  from #groupe as gr,
  SOUS_SECTEUR sousSecteur
  where sousSecteur.id_secteur = 'F ACTIONS' or sousSecteur.id_secteur like 'A %'
  
  insert into #groupe_type (groupe, ordreGroupe, rubrique, ordreRubrique, libelle, code)  
  select gr.groupe ,gr.ordreGroupe, 'OBLIG-ALL' ,2,  'OBLIGATIONS' , sousSecteur.libelle
  from #groupe as gr,
  SOUS_SECTEUR sousSecteur
  where sousSecteur.id_secteur like 'O %' or sousSecteur.id_secteur= 'F OBLIG'  

  insert into #groupe_type (groupe, ordreGroupe, rubrique, ordreRubrique, libelle, code)  
  select  gr.groupe ,gr.ordreGroupe,
  sen.categorie as 'rubrique',
  2+0.1*sen.ordre as 'ordreRubrique',
  '        '+sen.categorie as 'libelle',
  sen.sousSecteur as 'code'
  from #groupe as gr,
  #Seniorite sen
 
  insert into #groupe_type (groupe, ordreGroupe, rubrique, ordreRubrique, libelle, code)  
  select gr.groupe ,gr.ordreGroupe, 'OBLIG-NON-FI' ,2.5,  '        OBLIGATIONS NON FINANCIERES' , sousSecteur.libelle
  from #groupe as gr,
  SOUS_SECTEUR sousSecteur
  where sousSecteur.id_secteur like 'O CNF%'
 
 insert into #groupe_type (groupe, ordreGroupe, rubrique, ordreRubrique, libelle, code)  
  select gr.groupe ,gr.ordreGroupe, 'FONDS-OBL' ,2.6,  '        FONDS obligataire' , sousSecteur.libelle
  from #groupe as gr,
  SOUS_SECTEUR sousSecteur
  where sousSecteur.id_secteur = 'F OBLIG' 

 insert into #groupe_type (groupe, ordreGroupe, rubrique, ordreRubrique, libelle, code)  
  select gr.groupe ,gr.ordreGroupe, 'AGENCIES' ,2.7,  '        AGENCIES' , sousSecteur.libelle
  from #groupe as gr,
  SOUS_SECTEUR sousSecteur
  where sousSecteur.id_secteur IN ( 'O GVT' , 'O AGENCIES') 

insert into #groupe_type (groupe, ordreGroupe, rubrique, ordreRubrique, libelle, code)  
  select gr.groupe ,gr.ordreGroupe, 'FONDS-DIV' ,3,  'FONDS diversifié' , sousSecteur.libelle
  from #groupe as gr,
  SOUS_SECTEUR sousSecteur
  where sousSecteur.id_secteur = 'F DIVERS' 

  -- la liste des encours par emetteur: on peut donc classer par encours des grp emetteurs
  select  tr.emetteur as 'emetteur' , SUM( Valeur_Boursiere + Coupon_Couru ) as encours 
  into #groupe_emetteur 
  from [dbo].[PTF_TRANSPARISE] as tr 
  where tr.Compte = '4010015'
        and tr.Dateinventaire = @dateInventaire
        and tr.Numero_Niveau = @niveauInventaire
        --and Type_actif <> 'Monétaire'
        and   emetteur not in (select libelle from PAYS ) 
        and  emetteur not in ( 'PAYS BAS' ,'GRECE')        -- TODO : à supprimer après correction dans la base 
  group by emetteur
  
  
  select emetteur, encours,ROW_NUMBER() OVER (ORDER BY encours desc) as ordre , omega.paysEmetteur, omega.libelleNatureEmetteur
  into #perimetre_emetteur 
  from #groupe_emetteur  as e
  left outer join EMETTEUR_OMEGA as omega on omega.nomEmetteur = e.emetteur
  where omega.natureEmetteur IN ( select natureEmetteur from @NatureEmetteurs )
  and encours is not null
  
  -- select * from #perimetre_emetteur order by emetteur
  --  select * from EMETTEUR_OMEGA order by nomEmetteur
  -- select * from #groupe_emetteur    order by emetteur
    
-- ligne global par groupe    
  insert 
  into  PTF_RAPPORT_NIV2 
  (cle,groupe,ordreGroupe,date,rubrique,sousRubrique,libelle,classementRubrique,classementSousRubrique,valeur)
  select @rapportCle as cle,
		 eg.groupe as groupe,
		 eg.ordreGroupe as ordreGroupe,
		 @dateInventaire,--@dateInventaire as date,
		 eg.emetteur as rubrique, 
         'global' as sous_rubrique, -- encours pour 1 groupe
		 'Global' as libelle,
		 @incClassCompartiment+eg.ordre as classementRubrique,
         0 as classementSousRubrique,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from (select distinct ge.emetteur,ge.ordre,gt.groupe, gt.ordreGroupe from   #groupe_type as gt  , #perimetre_emetteur as ge  ) as eg
  left outer join [PTF_TRANSPARISE] as tr on
  tr.Compte = eg.Groupe
  and tr.Dateinventaire = @dateInventaire  
  and tr.Numero_Niveau = @niveauInventaire      
  and tr.emetteur = eg.emetteur
  group by  eg.groupe,eg.ordreGroupe, eg.emetteur, eg.ordre
  order by classementRubrique, classementSousRubrique, ordreGroupe
  
  -- lignes pour chaque rubrique (ensemble de sous secteurs)
  insert 
  into  PTF_RAPPORT_NIV2 
  (cle,groupe, ordreGroupe,date,rubrique,sousRubrique,libelle, classementRubrique,classementSousRubrique,  valeur )
  select @rapportCle as cle,
		 em_sous_secteur.groupe as groupe,
		 em_sous_secteur.ordreGroupe as ordreGroupe,
		 @dateInventaire,--@dateInventaire as date,
		 em_sous_secteur.emetteur as rubrique, 
         em_sous_secteur.rubrique as sous_rubrique, 
		 em_sous_secteur.libelle as libelle,
		 @incClassCompartiment+em_sous_secteur.ordre as classementRubrique,
         em_sous_secteur.ordreRubrique as classementSousRubrique,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from  (select gt.groupe,gt.ordreGroupe, gt.rubrique, gt.ordreRubrique, gt.libelle, gt.code,  ge.emetteur, ge.ordre from #groupe_type as gt  , #perimetre_emetteur as ge ) as em_sous_secteur
  left outer join [PTF_TRANSPARISE] as tr on
  tr.Compte = em_sous_secteur.Groupe
  and tr.Dateinventaire = @dateInventaire  
  and tr.Numero_Niveau = @niveauInventaire      
  and tr.emetteur = em_sous_secteur.emetteur 
  and tr.Sous_secteur = em_sous_secteur.code  
  group by  em_sous_secteur.groupe,em_sous_secteur.ordreGroupe, em_sous_secteur.emetteur, em_sous_secteur.ordre, em_sous_secteur.rubrique,em_sous_secteur.libelle,em_sous_secteur.ordreRubrique
    
 /**********************************************/
 /***              FIN  DONNEES AGREGEES  ****/
/**********************************************/

 /**********************************************/
 /***              DEBUT  DONNEES CALCULEES ****/
/**********************************************/
  -- calcul des colonnes RETRAITE et ASSURANCE

  
-- calcul des colonnes POIDS 
  insert 
  into  PTF_RAPPORT_NIV2 
  (cle,groupe, ordreGroupe,date,rubrique,sousRubrique,libelle, classementRubrique,classementSousRubrique,  valeur )
  select @rapportCle as cle,
		 gr.groupe as groupe,
		 gr.ordreGroupe as ordreGroupe, 
		  @dateInventaire,
		 montantRubrique.rubrique as rubrique,
	     montantRubrique.sousRubrique As sousRubrique,
		 montantRubrique.libelle as libelle,
		 montantRubrique.classementRubrique,
		 montantRubrique.classementSousRubrique,
         (montantRubrique.valeur / NULLIF(encoursTotal.valeur,0) )*100 as valeur
  from #GroupeCalcule as gr,
  PTF_RAPPORT_NIV2  as montantRubrique 
   left outer join PTF_RAPPORT_NIV2  as encoursTotal on encoursTotal.date = @dateInventaire and encoursTotal.cle = @rapportCle and 
    encoursTotal.rubrique = 'ENCOURS' and encoursTotal.sousRubrique = 'Encours' and encoursTotal.groupe =  montantRubrique.groupe
  where  gr.groupe IN ( 'POIDS 4010015') 
  and montantRubrique.date = @dateInventaire and montantRubrique.cle = @rapportCle 
  and montantRubrique.groupe = ( select composant.groupeComposant from #GroupeCalculeComposant as composant where composant.groupeCalcule = gr.groupe) 
  
  
 /**********************************************/
 /***              FIN  DONNEES CALCULEES  ****/
/**********************************************/ 

/** SORTIE **/

declare @date as datetime 
set @date = '12/10/2011'
declare @rapportCle as char(20)
set @rapportCle = 'ConcentEmetteurs_PEREObligations'
  select  
  case  when gr0.classementRubrique < 100 then 100*gr0.classementRubrique+10*gr0.classementSousRubrique
  else gr0.classementRubrique
  end  as 'ordre',
  gr0.classementRubrique,gr0.classementSousRubrique,
  emetteur.libelleNatureEmetteur, emetteur.paysEmetteur,
   gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
   gr0.valeur as '4010015',
   gr1.valeur as 'POIDS 4010015'
  from PTF_RAPPORT_NIV2 as gr0
  left outer join EMETTEUR_OMEGA as emetteur on emetteur.nomEmetteur = gr0.rubrique   
  left outer join PTF_RAPPORT_NIV2 as gr1 on gr1.rubrique = gr0.rubrique and gr1.sousRubrique = gr0.sousRubrique 
   and gr1.Groupe='POIDS 4010015'  and gr1.cle = @rapportCle and gr1.date = @date   
  where gr0.Groupe= '4010015' and gr0.cle = @rapportCle and gr0.date = @date
and ( gr0.sousRubrique =  'global' or gr0.sousRubrique like 'PAYS_EMPRUNT D''ETAT%')
  order by gr0.classementRubrique, gr0.classementSousRubrique

/*
select * from PTF_RAPPORT_NIV2 where cle = 'ConcentEmetteurs    ' and date = '31/08/2011'
 order by classementRubrique, classementSousRubrique 
*/