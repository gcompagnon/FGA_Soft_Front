--------------------------------------------------------------------------------------------------
-------           Sortie de l'tat donnant l encours pour chaque groupe �metteurs
------              Il est possible de param�trer la nature des groupes emetteurs 
------                                            et son emprise en % (ne prendre les emtteurs >= � x%
------              les familles, natures et pays des groupes �metteurs sont bas�s sur les tables de r�f�rence: 
------                GROUPE_EMETTEUR_OMEGA et NATURE_EMETTEUR_OMEGA
--------------------------------------------------------------------------------------------------

declare @date as datetime 
set @date =  (Select max (dateInventaire) from PTF_FGA where Compte = '7201106')
declare @rapportCle as char(20)
set @rapportCle = 'ConcentGrpEmetteurs'

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
create table #GROUPE_EMETTEUR_GOV ( libelleGroupeEmetteur varchar(50)  COLLATE DATABASE_DEFAULT not null primary key,pays varchar(50) COLLATE DATABASE_DEFAULT, natureGroupeEmetteur varchar(50) COLLATE DATABASE_DEFAULT,familleNatureEmetteur varchar(50) COLLATE DATABASE_DEFAULT)
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



-- Pour l'ordre , faire un classement  l'affichage : classer les + gros groupes pour la retraite 
  select gr3.rubrique as emetteur, gr3.valeur as encours,ROW_NUMBER() OVER (ORDER BY gr3.valeur desc) as ordre
  into #perimetre_emetteur_retraite 
  from PTF_RAPPORT_NIV2 as gr3 
  left outer join GROUPE_EMETTEUR_OMEGA as emetteur on emetteur.libelleGroupeEmetteur = gr3.rubrique
  where gr3.Groupe= 'Poids RETRAITE' and gr3.cle = @rapportCle and gr3.date = @date and 
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
  gr1.valeur as 'MM AGIRC' ,
  gr2.valeur as 'MM ARRCO',  
  gr3.valeur as 'RETRAITE',
  gr4.valeur as '% Retraite'
  from PTF_RAPPORT_NIV2 as gr0
  left outer join GROUPE_EMETTEUR_OMEGA as emetteur on emetteur.libelleGroupeEmetteur = gr0.rubrique   
  left outer join NATURE_EMETTEUR_OMEGA as nature on nature.codeNatureEmetteur = emetteur.codeNatureGroupeEmetteur
  left outer join #GROUPE_EMETTEUR_GOV as gov on gov.libelleGroupeEmetteur = gr0.sousRubrique
  left outer join #perimetre_emetteur_retraite as emetteurs on emetteurs.emetteur = gr0.rubrique
  left outer join PTF_RAPPORT_NIV2 as gr1 on gr1.rubrique = gr0.rubrique and gr1.sousRubrique = gr0.sousRubrique 
   and gr1.groupe= 'MM AGIRC' and gr1.cle = @rapportCle and gr1.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr2 on gr2.rubrique = gr0.rubrique and gr2.sousRubrique = gr0.sousRubrique 
   and gr2.Groupe= 'MM ARRCO' and gr2.cle = @rapportCle and gr2.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr3 on gr3.rubrique = gr0.rubrique and gr3.sousRubrique = gr0.sousRubrique 
   and gr3.Groupe= 'RETRAITE' and gr3.cle = @rapportCle and gr3.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr4 on gr4.rubrique = gr0.rubrique and gr4.sousRubrique = gr0.sousRubrique 
  and gr4.Groupe= 'POIDS Retraite' and gr4.cle = @rapportCle and gr4.date = @date   
   where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date and  emetteurs.emetteur is not null
  order by ordre, gr0.classementSousRubrique


-- Pour l'ordre , faire un classement � l'affichage : classer les + gros groupes pour l assurance 
  select gr3.rubrique as emetteur, gr3.valeur as encours,ROW_NUMBER() OVER (ORDER BY gr3.valeur desc) as ordre
  into #perimetre_emetteur_assurance
  from PTF_RAPPORT_NIV2 as gr3 
  left outer join GROUPE_EMETTEUR_OMEGA as emetteur on emetteur.libelleGroupeEmetteur = gr3.rubrique  
  where gr3.Groupe= 'Poids ASSURANCE' and gr3.cle = @rapportCle and gr3.date = @date and 
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
		when (emetteurs.ordre=2 AND gr0.classementSousRubrique=1)then 'graph1'
		when gr0.classementSousRubrique=0 then 'graph1'
		else ''
		end as 'graph', 
  emetteurs.ordre,
  gr0.classementRubrique, gr0.classementSousRubrique, 
  COALESCE(nature.familleNatureEmetteur, gov.familleNatureEmetteur) as FamilleNature,
  COALESCE(emetteur.natureGroupeEmetteur, gov.natureGroupeEmetteur) as Nature,
  COALESCE(emetteur.paysGroupeEmetteur, gov.pays) as Pays,
  gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
  gr5.valeur as 'MMP',
  gr6.valeur as 'INPR',
  gr7.valeur as 'CAPREVAL',
  gr8.valeur as 'CMAV',
  gr9.valeur as 'MM MUTUELLE',
  --gr10.valeur as 'SAPREM',   
  gr11.valeur as 'AUXIA',
  gr12.valeur as 'QUATREM',
  gr13.valeur as 'AUTRES',
  gr14.valeur as 'ASSURANCE',
  gr15.valeur as '% Assurance'
  from PTF_RAPPORT_NIV2 as gr0
  left outer join GROUPE_EMETTEUR_OMEGA as emetteur on emetteur.libelleGroupeEmetteur = gr0.rubrique   
  left outer join NATURE_EMETTEUR_OMEGA as nature on nature.codeNatureEmetteur = emetteur.codeNatureGroupeEmetteur  
  left outer join #GROUPE_EMETTEUR_GOV as gov on gov.libelleGroupeEmetteur = gr0.sousRubrique    
  left outer join #perimetre_emetteur_assurance as emetteurs on emetteurs.emetteur = gr0.rubrique
  left outer join PTF_RAPPORT_NIV2 as gr5 on gr5.rubrique = gr0.rubrique and gr5.sousRubrique = gr0.sousRubrique 
  and gr5.Groupe= 'MMP' and gr5.cle = @rapportCle and gr5.date = @date
    left outer join PTF_RAPPORT_NIV2 as gr6 on gr6.rubrique = gr0.rubrique and gr6.sousRubrique = gr0.sousRubrique 
  and gr6.Groupe= 'INPR' and gr6.cle = @rapportCle and gr6.date = @date  
  left outer join PTF_RAPPORT_NIV2 as gr7 on gr7.rubrique = gr0.rubrique and gr7.sousRubrique = gr0.sousRubrique 
  and gr7.Groupe= 'CAPREVAL' and gr7.cle = @rapportCle and gr7.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr8 on gr8.rubrique = gr0.rubrique and gr8.sousRubrique = gr0.sousRubrique 
  and gr8.Groupe= 'CMAV' and gr8.cle = @rapportCle and gr8.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr9 on gr9.rubrique = gr0.rubrique and gr9.sousRubrique = gr0.sousRubrique 
  and gr9.Groupe= 'MM MUTUELLE' and gr9.cle = @rapportCle and gr9.date = @date
  --left outer join PTF_RAPPORT_NIV2 as gr10 on gr10.rubrique = gr0.rubrique and gr10.sousRubrique = gr0.sousRubrique 
  --and gr10.Groupe= 'SAPREM' and gr10.cle = @rapportCle and gr10.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr11 on gr11.rubrique = gr0.rubrique and gr11.sousRubrique = gr0.sousRubrique 
  and gr11.Groupe= 'AUXIA' and gr11.cle = @rapportCle and gr11.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr12 on gr12.rubrique = gr0.rubrique and gr12.sousRubrique = gr0.sousRubrique 
  and gr12.Groupe= 'QUATREM' and gr12.cle = @rapportCle and gr12.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr13 on gr13.rubrique = gr0.rubrique and gr13.sousRubrique = gr0.sousRubrique 
  and gr13.Groupe= 'AUTRES' and gr13.cle = @rapportCle and gr13.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr14 on gr14.rubrique = gr0.rubrique and gr14.sousRubrique = gr0.sousRubrique 
  and gr14.Groupe= 'ASSURANCE' and gr14.cle = @rapportCle and gr14.date = @date  
  left outer join PTF_RAPPORT_NIV2 as gr15 on gr15.rubrique = gr0.rubrique and gr15.sousRubrique = gr0.sousRubrique 
  and gr15.Groupe= 'POIDS Assurance' and gr15.cle = @rapportCle and gr15.date = @date
  where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date
   and emetteurs.emetteur is not null      
--   and (emetteur.natureGroupeEmetteur is null or emetteur.natureGroupeEmetteur <> 'SGP')
    order by ordre, gr0.classementSousRubrique



-- Pour l'ordre , faire un classement � l'affichage : classer les + gros groupes pour l assurance 
  select gr3.rubrique as emetteur, gr3.valeur as encours,ROW_NUMBER() OVER (ORDER BY gr3.valeur desc) as ordre
  into #perimetre_emetteur_externe
  from PTF_RAPPORT_NIV2 as gr3 
  left outer join GROUPE_EMETTEUR_OMEGA as emetteur on emetteur.libelleGroupeEmetteur = gr3.rubrique
  where gr3.Groupe= 'Poids EXTERNE' and gr3.cle = @rapportCle and gr3.date = @date and 
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
		when (emetteurs.ordre=2 AND gr0.classementSousRubrique=1) then 'graph1'
		when gr0.classementSousRubrique=0 then 'graph1'
		else ''
		end as 'graph',
  emetteurs.ordre,
  gr0.classementRubrique, gr0.classementSousRubrique, 
  COALESCE(nature.familleNatureEmetteur, gov.familleNatureEmetteur) as FamilleNature,  
  COALESCE(emetteur.natureGroupeEmetteur, gov.natureGroupeEmetteur) as Nature,
  COALESCE(emetteur.paysGroupeEmetteur, gov.pays) as Pays,
   gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
   gr14.valeur as 'ARCELOR MITTAL France',
   gr15.valeur as 'IDENTITES MUTUELLE',
   gr16.valeur as 'IRCEM MUTUELLE',
   gr17.valeur as 'IRCEM PREVOYANCE',
   gr18.valeur as 'IRCEM RETRAITE',
   gr19.valeur as 'UNMI',
   gr20.valeur as 'MMIS',
   gr21.valeur as 'TOTAL EXTERNE',
   gr22.valeur as '% EXTERNE'
  from PTF_RAPPORT_NIV2 as gr0
  left outer join GROUPE_EMETTEUR_OMEGA as emetteur on emetteur.libelleGroupeEmetteur = gr0.rubrique   
  left outer join NATURE_EMETTEUR_OMEGA as nature on nature.codeNatureEmetteur = emetteur.codeNatureGroupeEmetteur  
  left outer join #GROUPE_EMETTEUR_GOV as gov on gov.libelleGroupeEmetteur = gr0.sousRubrique  
  left outer join #perimetre_emetteur_externe as emetteurs on emetteurs.emetteur = gr0.rubrique and emetteurs.emetteur is not null  
  left outer join PTF_RAPPORT_NIV2 as gr14 on gr14.rubrique = gr0.rubrique and gr14.sousRubrique = gr0.sousRubrique 
  and gr14.Groupe= 'ARCELOR MITTAL France' and gr14.cle = @rapportCle and gr14.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr15 on gr15.rubrique = gr0.rubrique and gr15.sousRubrique = gr0.sousRubrique 
  and gr15.Groupe= 'IDENTITES MUTUELLE' and gr15.cle = @rapportCle and gr15.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr16 on gr16.rubrique = gr0.rubrique and gr16.sousRubrique = gr0.sousRubrique 
  and gr16.Groupe= 'IRCEM MUTUELLE' and gr16.cle = @rapportCle and gr16.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr17 on gr17.rubrique = gr0.rubrique and gr17.sousRubrique = gr0.sousRubrique 
  and gr17.Groupe= 'IRCEM PREVOYANCE' and gr17.cle = @rapportCle and gr17.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr18 on gr18.rubrique = gr0.rubrique and gr18.sousRubrique = gr0.sousRubrique 
  and gr18.Groupe= 'IRCEM RETRAITE' and gr18.cle = @rapportCle and gr18.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr19 on gr19.rubrique = gr0.rubrique and gr19.sousRubrique = gr0.sousRubrique 
  and gr19.Groupe= 'UNMI' and gr19.cle = @rapportCle and gr19.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr20 on gr20.rubrique = gr0.rubrique and gr20.sousRubrique = gr0.sousRubrique 
  and gr20.Groupe= 'MMIS' and gr20.cle = @rapportCle and gr20.date = @date      
  left outer join PTF_RAPPORT_NIV2 as gr21 on gr21.rubrique = gr0.rubrique and gr21.sousRubrique = gr0.sousRubrique 
  and gr21.Groupe= 'EXTERNE' and gr21.cle = @rapportCle and gr21.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr22 on gr22.rubrique = gr0.rubrique and gr22.sousRubrique = gr0.sousRubrique 
  and gr22.Groupe= 'POIDS Externe' and gr22.cle = @rapportCle and gr22.date = @date    
      where gr0.ordreGroupe= 0 and gr0.cle = @rapportCle and gr0.date = @date
       and emetteurs.emetteur is not null
  order by ordre, gr0.classementSousRubrique
  


    select  
	 case when gr0.classementRubrique=1 then 1 
		when gr0.classementRubrique=2 AND gr0.classementSousRubrique=1 then 0
		when gr0.classementSousRubrique=0 then 0
		else 2
		end as 'style',
case when gr0.classementRubrique=1 then ''
		when gr0.classementRubrique=2 AND gr0.classementSousRubrique=1 then 'graph1'
		when gr0.classementSousRubrique=0 then 'graph1'
		else ''
		end as 'graph',
  gr0.classementRubrique,
  gr0.classementRubrique, gr0.classementSousRubrique, 
  COALESCE(nature.familleNatureEmetteur, gov.familleNatureEmetteur) as FamilleNature,  
  COALESCE(emetteur.natureGroupeEmetteur, gov.natureGroupeEmetteur) as Nature,
  COALESCE(emetteur.paysGroupeEmetteur, gov.pays) as pays,
  gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
  gr0.valeur as 'FGA(tous les groupes)',
  gr22.valeur as '%'
  from PTF_RAPPORT_NIV2 as gr0
  left outer join GROUPE_EMETTEUR_OMEGA as emetteur on emetteur.libelleGroupeEmetteur = gr0.rubrique   
  left outer join NATURE_EMETTEUR_OMEGA as nature on nature.codeNatureEmetteur = emetteur.codeNatureGroupeEmetteur  
  left outer join #GROUPE_EMETTEUR_GOV as gov on gov.libelleGroupeEmetteur = gr0.sousRubrique
  left outer join PTF_RAPPORT_NIV2 as gr22 on gr22.rubrique = gr0.rubrique and gr22.sousRubrique = gr0.sousRubrique 
  and gr22.Groupe= 'POIDS FGA' and gr22.cle = @rapportCle and gr22.date = @date      
      where gr0.groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date
  order by gr0.classementRubrique, gr0.classementSousRubrique
  

drop table #perimetre_emetteur_retraite 
drop table #perimetre_emetteur_assurance
drop table #perimetre_emetteur_externe
drop table #GROUPE_EMETTEUR_GOV