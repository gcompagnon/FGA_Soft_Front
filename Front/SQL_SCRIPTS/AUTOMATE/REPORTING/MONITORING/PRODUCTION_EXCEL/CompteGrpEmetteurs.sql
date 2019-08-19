--------------------------------------------------------------------------------------------------
-------           Sortie de l'tat donnant l encours pour chaque groupe �metteurs
------              Il est possible de param�trer la nature des groupes emetteurs 
------                                            et son emprise en % (ne prendre les emtteurs >= � x%
------              les familles, natures et pays des groupes �metteurs sont bas�s sur les tables de r�f�rence: 
------                GROUPE_EMETTEUR_OMEGA et NATURE_EMETTEUR_OMEGA
--------------------------------------------------------------------------------------------------

declare @date as datetime 
--set @date =  (Select max (dateInventaire) from PTF_FGA where Compte = '7201106')
set @date =  '31/08/2012'
declare @rapportCle as char(20)
set @rapportCle = 'CompteGrpEmetteurs'

-- on ne prends que les emtteurs � encours > 0.5%
declare @limiteEncoursGrpEmetteurs as float
set @limiteEncoursGrpEmetteurs = 0.005

-- liste des code Nature grp emetteur que l on souhaite
declare @natureGrpEmetteur table 
(
  CodeNature char(6)
)
insert into @natureGrpEmetteur(CodeNature) select codeNatureEmetteur from NATURE_EMETTEUR_OMEGA where codeNatureEmetteur <> 'SGP'


-- les govies ne sont pas des groupes emetteurs. pour avoir une coh�rence, on met une nature "SOUVERAIN" grace � une table temporaire
create table #GROUPE_EMETTEUR_GOV ( libelleGroupeEmetteur varchar(50) not null primary key,pays varchar(50), natureGroupeEmetteur varchar(50),familleNatureEmetteur varchar(50))
insert into #GROUPE_EMETTEUR_GOV values( 'Encours','TOTAL','TOTAL','TOTAL')
insert into #GROUPE_EMETTEUR_GOV values( 'TOTAL','TOTAL','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT EURO','Zone Euro','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT FRANCAIS','France','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT BELGE','Belgique','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT PORTUGAIS','Portugal','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT AUTRICHIEN','Autriche','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT ESPAGNOL','Espagne','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT IRLANDAIS','Irlande','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT ALLEMAND','Allemagne','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT SUEDOIS','Suède','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT FINLANDAIS','Finlande','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT HOLLANDAIS','Pays-Bas','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT GREC','Grèce','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT AMERICAIN','Etats-Unis','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT HONGROIS','Hongrie','SOUVERAIN','SOUVERAIN')
insert into #GROUPE_EMETTEUR_GOV values( 'PAYS_EMPRUNT D''ETAT ITALIEN','Italie','SOUVERAIN','SOUVERAIN')


-- Pour l'ordre , faire un classement � l'affichage : classer les + gros groupes pour l assurance 
  select gr3.rubrique as emetteur, gr3.valeur as encours,ROW_NUMBER() OVER (ORDER BY gr3.valeur desc) as ordre
  into #perimetre_emetteur_compte
  from PTF_RAPPORT_NIV2 as gr3 
  left outer join GROUPE_EMETTEUR_OMEGA as emetteur on emetteur.libelleGroupeEmetteur = gr3.rubrique  
  where gr3.Groupe= '4010015' and gr3.cle = @rapportCle and gr3.date = @date and 
  gr3.sousRubrique in ( 'global', 'Encours', 'TOTAL')
  and gr3.valeur is not null
  and gr3.valeur > @limiteEncoursGrpEmetteurs 
  and ( emetteur.codeNatureGroupeEmetteur is null or emetteur.codeNatureGroupeEmetteur in (select CodeNature from @natureGrpEmetteur) )


  select  
  case when emetteurs.ordre=1 then 1 
		when emetteurs.ordre=2 AND gr0.classementSousRubrique=1 then 0
		when gr0.classementSousRubrique=0 then 0
		else 2
		end as 'style',
case when emetteurs.ordre=1 then ''
		when emetteurs.ordre=2 AND gr0.classementSousRubrique=1 then 'graph1'
		when gr0.classementSousRubrique=0 then 'graph1'
		else ''
		end as 'graph',
  emetteurs.ordre,
  gr0.classementRubrique, gr0.classementSousRubrique, 
  COALESCE(nature.familleNatureEmetteur, gov.familleNatureEmetteur) as FamilleNature,  
  COALESCE(emetteur.natureGroupeEmetteur, gov.natureGroupeEmetteur) as Nature,
  COALESCE(emetteur.paysGroupeEmetteur, gov.pays) as Pays,
  gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
  gr0.valeur as 'Compte1' 
  from PTF_RAPPORT_NIV2 as gr0
  left outer join GROUPE_EMETTEUR_OMEGA as emetteur on emetteur.libelleGroupeEmetteur = gr0.rubrique   
  left outer join NATURE_EMETTEUR_OMEGA as nature on nature.codeNatureEmetteur = emetteur.codeNatureGroupeEmetteur
  left outer join #GROUPE_EMETTEUR_GOV as gov on gov.libelleGroupeEmetteur = gr0.sousRubrique
  left outer join #perimetre_emetteur_compte as emetteurs on emetteurs.emetteur = gr0.rubrique
   where gr0.Groupe= '4010015' and gr0.cle = @rapportCle and gr0.date = @date and  emetteurs.emetteur is not null
  order by ordre, gr0.classementSousRubrique



drop table #perimetre_emetteur_compte
drop table #GROUPE_EMETTEUR_GOV