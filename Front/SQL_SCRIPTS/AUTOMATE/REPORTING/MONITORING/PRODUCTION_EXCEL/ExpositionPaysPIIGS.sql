--USE FGA_Data
--go
--------------------------------------------------------------------------------------------------
-------           Sortie de l'état donnant les expositions sur les pays (PIIGS, etc..)
-------            seulement la Zone 2 et pour l'assurance
--------------------------------------------------------------------------------------------------

declare @date as datetime
SET @date = (Select max (dateInventaire) from PTF_FGA where Compte = '7201106')

declare @rapportCle as char(20)
set @rapportCle='MonitorExpoPays'


declare @cle_Montant as char(20)	
set @cle_Montant='MonitorExpoPays'

declare @rubriqueEncours as char(20)
set @rubriqueEncours='TOTAL' 

declare @sousRubriqueEncours as char(20)
set @sousRubriqueEncours='TOTAL_HOLDING' 

declare @rapportKey as char(20)
set @rapportKey = 'MonitorExpoPaysDur'


select  
case when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)=0 then 1 
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000) then 0
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100) then 2
                when gr0.classementRubrique/10=CONVERT(INT,gr0.classementRubrique/10) then 3
                else 4
		end as 'style',
	case when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100) then 'graph1'
	else ''
	end as 'graph',
   100*gr0.classementRubrique +10*gr0.classementSousRubrique
   as 'ordre',  
  gr0.classementRubrique, gr0.classementSousRubrique, 
  gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
   gr4.valeur as 'MMP',
   gr5.valeur as 'INPR',
   gr6.valeur as 'CAPREVAL',
   gr7.valeur as 'CMAV',
   gr8.valeur as 'MM MUTUELLE',
--   gr9.valeur as 'SAPREM',   
   gr10.valeur as 'AUXIA',
   gr11.valeur as 'QUATREM',
   gr12.valeur as 'AUTRES',
   gr13.valeur as 'ASSURANCE',
   (gr13_amount.valeur/NULLIF(gr13_all.valeur,0)) as ' '
  from PTF_RAPPORT_NIV2 as gr0
  left outer join PTF_RAPPORT_NIV2 as gr4 on gr4.rubrique = gr0.rubrique and gr4.sousRubrique = gr0.sousRubrique 
  and gr4.Groupe= 'MMP' and gr4.cle = @rapportCle and gr4.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr5 on gr5.rubrique = gr0.rubrique and gr5.sousRubrique = gr0.sousRubrique 
  and gr5.Groupe= 'INPR' and gr5.cle = @rapportCle and gr5.date = @date
    left outer join PTF_RAPPORT_NIV2 as gr6 on gr6.rubrique = gr0.rubrique and gr6.sousRubrique = gr0.sousRubrique 
  and gr6.Groupe= 'CAPREVAL' and gr6.cle = @rapportCle and gr6.date = @date  
  left outer join PTF_RAPPORT_NIV2 as gr7 on gr7.rubrique = gr0.rubrique and gr7.sousRubrique = gr0.sousRubrique 
  and gr7.Groupe= 'CMAV' and gr7.cle = @rapportCle and gr7.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr8 on gr8.rubrique = gr0.rubrique and gr8.sousRubrique = gr0.sousRubrique 
  and gr8.Groupe= 'MM MUTUELLE' and gr8.cle = @rapportCle and gr8.date = @date
  --left outer join PTF_RAPPORT_NIV2 as gr9 on gr9.rubrique = gr0.rubrique and gr9.sousRubrique = gr0.sousRubrique 
  --and gr9.Groupe= 'SAPREM' and gr9.cle = @rapportCle and gr9.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr10 on gr10.rubrique = gr0.rubrique and gr10.sousRubrique = gr0.sousRubrique 
  and gr10.Groupe= 'AUXIA' and gr10.cle = @rapportCle and gr10.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr11 on gr11.rubrique = gr0.rubrique and gr11.sousRubrique = gr0.sousRubrique 
  and gr11.Groupe= 'QUATREM' and gr11.cle = @rapportCle and gr11.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr12 on gr12.rubrique = gr0.rubrique and gr12.sousRubrique = gr0.sousRubrique 
  and gr12.Groupe= 'AUTRES' and gr12.cle = @rapportCle and gr12.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr13 on gr13.rubrique = gr0.rubrique and gr13.sousRubrique = gr0.sousRubrique 
   and gr13.Groupe= 'ASSURANCE' and gr13.cle = @rapportCle and gr13.date = @date  

  left outer join PTF_RAPPORT_NIV2 as gr13_amount on gr13_amount.rubrique = gr0.rubrique and gr13_amount.sousRubrique = gr0.sousRubrique 
   and gr13_amount.Groupe= gr13.Groupe and gr13_amount.cle = @cle_Montant and gr13_amount.date = @date         
  left outer join PTF_RAPPORT_NIV2 as gr13_all on gr13_all.rubrique = @rubriqueEncours and gr13_all.sousRubrique = @sousRubriqueEncours 
   and gr13_all.Groupe= gr13.Groupe and gr13_all.cle = @cle_Montant and gr13_all.date = @date      
   
      where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date 
      and (gr0.classementRubrique = 0 or FLOOR( gr0.classementRubrique / 100 ) = 3)
      and gr0.classementSousRubrique < 8
  order by gr0.classementRubrique, gr0.classementSousRubrique



select  
case when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)=0 then 6 
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000) then 5
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100) then 7
                when gr0.classementRubrique/10=CONVERT(INT,gr0.classementRubrique/10) then 8
                else 9
		end as 'style',
	case when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100) then 'graph1'
	else ''
	end as 'graph',
    100*gr0.classementRubrique +10*gr0.classementSousRubrique
   as 'ordre',  
  gr0.classementRubrique, gr0.classementSousRubrique, 
  gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
   gr4.valeur as 'MMP',
   gr5.valeur as 'INPR',
   gr6.valeur as 'CAPREVAL',
   gr7.valeur as 'CMAV',
   gr8.valeur as 'MM MUTUELLE',
--   gr9.valeur as 'SAPREM',   
   gr10.valeur as 'AUXIA',
   gr11.valeur as 'QUATREM',
   gr12.valeur as 'AUTRES',
   gr13.valeur as 'ASSURANCE',
   (gr13_amount.valeur/NULLIF(gr13_all.valeur,0)) as ' '
  from PTF_RAPPORT_NIV2 as gr0
  left outer join PTF_RAPPORT_NIV2 as gr4 on gr4.rubrique = gr0.rubrique and gr4.sousRubrique = gr0.sousRubrique 
  and gr4.Groupe= 'MMP' and gr4.cle = @rapportKey and gr4.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr5 on gr5.rubrique = gr0.rubrique and gr5.sousRubrique = gr0.sousRubrique 
  and gr5.Groupe= 'INPR' and gr5.cle = @rapportKey and gr5.date = @date
    left outer join PTF_RAPPORT_NIV2 as gr6 on gr6.rubrique = gr0.rubrique and gr6.sousRubrique = gr0.sousRubrique 
  and gr6.Groupe= 'CAPREVAL' and gr6.cle = @rapportKey and gr6.date = @date  
  left outer join PTF_RAPPORT_NIV2 as gr7 on gr7.rubrique = gr0.rubrique and gr7.sousRubrique = gr0.sousRubrique 
  and gr7.Groupe= 'CMAV' and gr7.cle = @rapportKey and gr7.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr8 on gr8.rubrique = gr0.rubrique and gr8.sousRubrique = gr0.sousRubrique 
  and gr8.Groupe= 'MM MUTUELLE' and gr8.cle = @rapportKey and gr8.date = @date
  --left outer join PTF_RAPPORT_NIV2 as gr9 on gr9.rubrique = gr0.rubrique and gr9.sousRubrique = gr0.sousRubrique 
  --and gr9.Groupe= 'SAPREM' and gr9.cle = @rapportKey and gr9.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr10 on gr10.rubrique = gr0.rubrique and gr10.sousRubrique = gr0.sousRubrique 
  and gr10.Groupe= 'AUXIA' and gr10.cle = @rapportKey and gr10.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr11 on gr11.rubrique = gr0.rubrique and gr11.sousRubrique = gr0.sousRubrique 
  and gr11.Groupe= 'QUATREM' and gr11.cle = @rapportKey and gr11.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr12 on gr12.rubrique = gr0.rubrique and gr12.sousRubrique = gr0.sousRubrique 
  and gr12.Groupe= 'AUTRES' and gr12.cle = @rapportKey and gr12.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr13 on gr13.rubrique = gr0.rubrique and gr13.sousRubrique = gr0.sousRubrique 
   and gr13.Groupe= 'ASSURANCE' and gr13.cle = @rapportKey and gr13.date = @date  

  left outer join PTF_RAPPORT_NIV2 as gr13_amount on gr13_amount.rubrique = gr0.rubrique and gr13_amount.sousRubrique = gr0.sousRubrique 
   and gr13_amount.Groupe= gr13.Groupe and gr13_amount.cle = @cle_Montant and gr13_amount.date = @date         
  left outer join PTF_RAPPORT_NIV2 as gr13_all on gr13_all.rubrique = @rubriqueEncours and gr13_all.sousRubrique = @sousRubriqueEncours 
   and gr13_all.Groupe= gr13.Groupe and gr13_all.cle = @cle_Montant and gr13_all.date = @date      
   
      where gr0.Groupe= 'FGA' and gr0.cle = @rapportKey and gr0.date = @date 
      and (gr0.classementRubrique = 0 or FLOOR( gr0.classementRubrique / 100 ) = 3)
      and gr0.classementSousRubrique < 8
  order by gr0.classementRubrique, gr0.classementSousRubrique
