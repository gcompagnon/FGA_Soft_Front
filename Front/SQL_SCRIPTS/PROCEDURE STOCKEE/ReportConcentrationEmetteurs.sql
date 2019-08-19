 
/** configuration du rapport Concentration par emetteurs (de nature Assurance, Banque, Financiere ou Foncieres):
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


ALTER PROCEDURE ReportConcentrationEmetteurs 
        @dateInventaire  datetime ,
        @niveauInventaire tinyint ,
        @rapportCle char(20) OUTPUT
        --@NatureEmetteurs table (
        --                    natureEmetteur char(6)
        --                    )OUTPUT
AS

/** Parametres: --------------------------------------**/
/*
declare @dateInventaire as datetime 
set @dateInventaire = '30/11/2011'
declare @niveauInventaire as float
set @niveauInventaire = 3
declare @rapportCle as char(20)                            
*/
declare @NatureEmetteurs table (
                            natureEmetteur char(6) COLLATE DATABASE_DEFAULT 
                            )



-- la liste des natures emetteurs selectionnées
IF NOT EXISTS (SELECT * FROM @NatureEmetteurs )
BEGIN
insert  @NatureEmetteurs  values( 'ASS')
insert  @NatureEmetteurs values( 'BAN')
insert  @NatureEmetteurs values( 'FIN')
insert  @NatureEmetteurs values( 'FON' )
END

-- construction de la table de décomposition avec le traitement particulier des Corporates Fi:
	-- niveau CorpoFinanciere
	create table #Seniorite  (categorie varchar(15) COLLATE DATABASE_DEFAULT , sousSecteur varchar(60) COLLATE DATABASE_DEFAULT , ordre float)
	insert #Seniorite values ('SENIOR', 'EMPRUNT ASSURANCE SEN', 1)
	insert #Seniorite values ('SENIOR', 'EMPRUNT BANQUE SEN', 1)
    insert #Seniorite values ('SENIOR', 'EMPRUNT SERVICES FINANCIERS SEN',1)		
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT BANQUE LT2', 2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT BANQUE UT2',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT BANQUE T1',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT ASSURANCE LT2',2)	
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT ASSURANCE UT2',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT ASSURANCE T1',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT SERVICES FINANCIERS LT2',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT SERVICES FINANCIERS UT2',2)
	insert #Seniorite values ('SUBORDONNEE', 'EMPRUNT SERVICES FINANCIERS T1',2)
	insert #Seniorite values ('COVERED', 'EMPRUNT FONCIER ET HYPOTHECAIRE',3)


IF @rapportCle IS NULL
BEGIN
set @rapportCle = 'ConcentEmetteurs'
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

create table #Groupe  ( groupe varchar(25) COLLATE DATABASE_DEFAULT primary key, ordreGroupe smallint null)
insert #Groupe values ('MM AGIRC',1)
insert #Groupe values ('MM ARRCO',2)
insert #Groupe values ('MMP',5)
insert #Groupe values ('MM MUTUELLE',6)
insert #Groupe values ('CMAV',7)
insert #Groupe values ('QUATREM',8)
insert #Groupe values ('AUXIA',9)
insert #Groupe values ('CAPREVAL',10)
insert #Groupe values ('INPR',11)
insert #Groupe values ('SAPREM',12)
insert #Groupe values ('AUTRES',13)

/** la table des colonnes calculees / des sur-groupes  **/
create table #GroupeCalcule  ( groupe varchar(25) COLLATE DATABASE_DEFAULT primary key, ordreGroupe tinyint null)
insert #GroupeCalcule values ('RETRAITE',3)
insert #GroupeCalcule values ('POIDS Retraite',4)
insert #GroupeCalcule values ('ASSURANCE',14)
insert #GroupeCalcule values ('POIDS Assurance',15)
insert #GroupeCalcule values ('POIDS FGA',16)

create table #GroupeCalculeComposant  ( groupeCalcule varchar(25) COLLATE DATABASE_DEFAULT , groupeComposant varchar(25) COLLATE DATABASE_DEFAULT null)
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

-- le poids est calculé sur les entités Assurance ou Retraite --
insert #GroupeCalculeComposant values ('POIDS Assurance', 'ASSURANCE')
insert #GroupeCalculeComposant values ('POIDS Retraite', 'RETRAITE')
insert #GroupeCalcule values ('FGA',0)
insert #GroupeCalculeComposant values ('POIDS FGA', 'FGA')

/** on met dans le groupe FGA l ensemble des groupes */
insert #GroupeCalculeComposant 
 select distinct 'FGA',Groupe
 from [PTF_TRANSPARISE]
 where Dateinventaire = @dateInventaire
 and Numero_Niveau = @niveauInventaire


declare @maxGroup int
set @maxGroup = 16
--Les nouveaux groupes seront places arbitrairement à la suite

-- temporaire pour stocker les groupes présents dans les inventaires transparisés 
   insert into #Groupe
  select distinct Groupe, -1 as ordreGroupe   
  from [dbo].[PTF_TRANSPARISE]
  where Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and Groupe not in (select groupe from #Groupe)

insert #GroupeCalculeComposant select 'EXTERNE', Groupe from #Groupe where ordreGroupe = -1

  update #Groupe 
  set   ordreGroupe = @maxGroup,
  @maxGroup = @maxGroup +1 
  where ordreGroupe = -1
  
insert #GroupeCalcule values ('EXTERNE',@maxGroup) 
set @maxGroup = @maxGroup +1    
insert #GroupeCalcule values ('POIDS Externe',@maxGroup)
insert #GroupeCalculeComposant values ('POIDS Externe', 'EXTERNE')    
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
  on tr.Groupe = gr.Groupe
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
  on tr.Groupe = grSecteur.Groupe
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
  on tr.Groupe = grSecteur.Groupe
  and tr.Sous_Secteur = grSecteur.rubrique
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  group by  grSecteur.groupe,grSecteur.ordreGroupe, grSecteur.rubrique ,grSecteur.libelle

/********************************************************************/
  
  create table  #groupe_type (groupe varchar(25) COLLATE DATABASE_DEFAULT , ordreGroupe int, rubrique varchar(120) COLLATE DATABASE_DEFAULT, ordreRubrique float, libelle varchar(60) COLLATE DATABASE_DEFAULT, code varchar(60) COLLATE DATABASE_DEFAULT)  

 
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
  where tr.Dateinventaire = @dateInventaire
        and tr.Numero_Niveau = @niveauInventaire
        and ( Type_actif <> 'Monétaire' or Type_actif is null ) --TODO : qualifier la base pour ne plus avoir de champ à NULL)
        and   emetteur not in (select libelle from PAYS ) 
        and  emetteur not in ( 'PAYS BAS' ,'GRECE')        -- TODO : à supprimer après correction dans la base 
  group by emetteur
  
  -- on ne prends que les emtteurs d une certaine nature (passée en parametre)
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
		 @dateInventaire as date,
		 eg.emetteur as rubrique, 
         'global' as sous_rubrique, -- encours pour 1 groupe
		 'Global' as libelle,
		 @incClassCompartiment+eg.ordre as classementRubrique,
         0 as classementSousRubrique,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from (select distinct ge.emetteur,ge.ordre,gt.groupe, gt.ordreGroupe from   #groupe_type as gt  , #perimetre_emetteur as ge  ) as eg
  left outer join [PTF_TRANSPARISE] as tr on
  tr.Groupe = eg.Groupe
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
  tr.Groupe = em_sous_secteur.Groupe
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
 
  insert 
  into  PTF_RAPPORT_NIV2 
  (cle,groupe, ordreGroupe,date,rubrique,sousRubrique,libelle, classementRubrique,classementSousRubrique,  valeur )
  select @rapportCle as cle,
		 gr.groupe as groupe,
		 gr.ordreGroupe as ordreGroupe, 
		 @dateInventaire,
		 rapport.rubrique as rubrique,
	     rapport.sousRubrique As sousRubrique,
		 rapport.libelle as libelle,
		 rapport.classementRubrique,
		 rapport.classementSousRubrique,
         SUM( rapport.valeur ) as valeur
  from #GroupeCalcule as gr
  --left outer join PTF_RAPPORT_NIV2 as rapport on rapport.date = @dateInventaire and rapport.cle = @rapportCle   
  left outer join PTF_RAPPORT_NIV2 as rapport on rapport.date = @dateInventaire and rapport.cle = @rapportCle   
   and rapport.groupe IN (select composant.groupeComposant from #GroupeCalculeComposant as composant where composant.groupeCalcule = gr.groupe)
  where gr.groupe IN ('ASSURANCE', 'RETRAITE', 'FGA', 'EXTERNE')
    group by  rapport.rubrique,rapport.sousRubrique, rapport.libelle, rapport.classementRubrique ,rapport.classementSousRubrique, gr.groupe, gr.ordreGroupe
  
  
  
  --select * from #GroupeCalcule
-- calcul des colonnes POIDS RETRAITE et POIDS ASSURANCE
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
  where  gr.groupe IN ('POIDS Retraite',  'POIDS Assurance', 'POIDS FGA', 'POIDS Externe') 
  and montantRubrique.date = @dateInventaire and montantRubrique.cle = @rapportCle 
  and montantRubrique.groupe = ( select composant.groupeComposant from #GroupeCalculeComposant as composant where composant.groupeCalcule = gr.groupe) 
  
  
 /**********************************************/
 /***              FIN  DONNEES CALCULEES  ****/
/**********************************************/
 
 
drop table #Groupe
drop table #GroupeCalcule
drop table #GroupeCalculeComposant
drop table #groupe_secteurEtats 
drop table #Seniorite
drop table #groupe_type 
drop table #groupe_emetteur
drop table #perimetre_emetteur 