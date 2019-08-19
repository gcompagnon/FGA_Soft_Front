--USE FGA_Data
--go
--------------------------------------------------------------------------------------------------
-------           Sortie de l'état donnant la répartition des groupe sur les secteurs
--------------------------------------------------------------------------------------------------
declare @date as datetime
SET @date = (Select max (dateInventaire) from PTF_FGA where Compte = '7201106')

declare @rapportCle as char(20)
set @rapportCle = 'MonitoringGroupe'
--set @rapportCle = 'MonitoringGroupeDuration'
--set @rapportCle = 'MonitorGroupeCompta'

declare @cle_Montant as char(20)	
set @cle_Montant='MonitoringGroupe'


declare @rubriqueEncours as char(20)
set @rubriqueEncours='Encours' 
--set @rubriqueEncours='OBLIGATIONS - Total' 



select  
case when gr0.classementRubrique=1 then 2 
		when gr0.classementRubrique<10 then 0
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) AND gr0.classementRubrique<300 then 1
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) then 3
                else 4
		end as 'style',
case when gr0.classementRubrique=1 then ''
		when gr0.classementRubrique=201.1 then 'graph1'
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) AND gr0.classementRubrique<300 then 'serie1'
		when gr0.classementRubrique=504.1 then 'serie5'
		else ''
		end as 'graph',
   gr0.classementRubrique,  gr0.rubrique, gr0.libelle, 
   gr1.valeur as 'MM AGIRC' ,
   gr2.valeur as 'MM ARRCO',  
   gr3.valeur as 'RETRAITE',
  (gr3_amount.valeur/NULLIF(gr3_all.valeur,0)) as ' '
  from [PTF_RAPPORT] as gr0
  left outer join [PTF_RAPPORT] as gr1 on gr1.rubrique = gr0.rubrique
   and gr1.groupe= 'MM AGIRC' and gr1.cle = @rapportCle and gr1.date = @date
  left outer join [PTF_RAPPORT] as gr2 on gr2.rubrique = gr0.rubrique
   and gr2.groupe= 'MM ARRCO'  and gr2.cle = @rapportCle and gr2.date = @date
  left outer join [PTF_RAPPORT] as gr3 on gr3.rubrique = gr0.rubrique
   and gr3.groupe= 'RETRAITE' and gr3.cle = @rapportCle and gr3.date = @date
  left outer join [PTF_RAPPORT] as gr3_amount on gr3_amount.rubrique = gr0.rubrique
   and gr3_amount.groupe= gr3.groupe and gr3_amount.cle = @cle_Montant and gr3_amount.date = @date
  left outer join [PTF_RAPPORT] as gr3_all on gr3_all.rubrique = @rubriqueEncours
   and gr3_all.groupe= gr3.groupe and gr3_all.cle = @cle_Montant and gr3_all.date = @date
   
    where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date    
  order by
  case  when gr0.classementRubrique < 100 then 100*gr0.classementRubrique
  else gr0.classementRubrique
  end,gr0.libelle
   

 select 
case when gr0.classementRubrique=1 then 2 
		when gr0.classementRubrique<10 then 0
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) AND gr0.classementRubrique<300 then 1
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) then 3
                else 4
		end as 'style',
case when gr0.classementRubrique=1 then ''
		when gr0.classementRubrique=201.1 then 'graph1'
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) AND gr0.classementRubrique<300 then 'serie2'
		when gr0.classementRubrique=504.1 then 'serie6'
		else ''
		end as 'graph',
  gr0.classementRubrique,gr0.rubrique, gr0.libelle, 
   gr4.valeur as 'MMP',
   gr5.valeur as 'INPR',
   gr6.valeur as 'CAPREVAL',   
   gr7.valeur as 'CMAV',
   gr8.valeur as 'MM MUTUELLE',
   gr9.valeur as 'SAPREM',
   gr10.valeur as 'AUXIA',
   gr11.valeur as 'QUATREM',
   gr12.valeur as 'AUTRES',
   gr13.valeur as 'ASSURANCE',
  (gr13_amount.valeur/NULLIF(gr13_all.valeur,0)) as ' '
  from [PTF_RAPPORT] as gr0
  left outer join [PTF_RAPPORT] as gr4 on gr4.rubrique = gr0.rubrique
  and gr4.Groupe= 'MMP' and gr4.cle = @rapportCle and gr4.date = @date
  left outer join [PTF_RAPPORT] as gr5 on gr5.rubrique = gr0.rubrique
  and gr5.Groupe= 'INPR' and gr5.cle = @rapportCle and gr5.date = @date
    left outer join [PTF_RAPPORT] as gr6 on gr6.rubrique = gr0.rubrique
  and gr6.Groupe= 'CAPREVAL' and gr6.cle = @rapportCle and gr6.date = @date  
  left outer join [PTF_RAPPORT] as gr7 on gr7.rubrique = gr0.rubrique
  and gr7.Groupe= 'CMAV' and gr7.cle = @rapportCle and gr7.date = @date
  left outer join [PTF_RAPPORT] as gr8 on gr8.rubrique = gr0.rubrique
  and gr8.Groupe= 'MM MUTUELLE' and gr8.cle = @rapportCle and gr8.date = @date
  left outer join [PTF_RAPPORT] as gr9 on gr9.rubrique = gr0.rubrique
  and gr9.Groupe= 'SAPREM' and gr9.cle = @rapportCle and gr9.date = @date
  left outer join [PTF_RAPPORT] as gr10 on gr10.rubrique = gr0.rubrique
  and gr10.Groupe= 'AUXIA' and gr10.cle = @rapportCle and gr10.date = @date
  left outer join [PTF_RAPPORT] as gr11 on gr11.rubrique = gr0.rubrique
  and gr11.Groupe= 'QUATREM' and gr11.cle = @rapportCle and gr11.date = @date
  left outer join [PTF_RAPPORT] as gr12 on gr12.rubrique = gr0.rubrique
  and gr12.Groupe= 'AUTRES' and gr12.cle = @rapportCle and gr12.date = @date
  left outer join [PTF_RAPPORT] as gr13 on gr13.rubrique = gr0.rubrique
  and gr13.Groupe= 'ASSURANCE' and gr13.cle = @rapportCle and gr13.date = @date  
  left outer join [PTF_RAPPORT] as gr13_amount on gr13_amount.rubrique = gr0.rubrique
   and gr13_amount.groupe= gr13.groupe and gr13_amount.cle = @cle_Montant and gr13_amount.date = @date
  left outer join [PTF_RAPPORT] as gr13_all on gr13_all.rubrique = @rubriqueEncours
   and gr13_all.groupe= gr13.groupe and gr13_all.cle = @cle_Montant and gr13_all.date = @date
  
    where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date    
  order by
  case  when gr0.classementRubrique < 100 then 100*gr0.classementRubrique
  else gr0.classementRubrique
  end,gr0.libelle

select  
case when gr0.classementRubrique=1 then 2 
		when gr0.classementRubrique<10 then 0
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) AND gr0.classementRubrique<300 then 1
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) then 3
                else 4
		end as 'style',
case when gr0.classementRubrique=1 then ''
		when gr0.classementRubrique=201.1 then 'graph2'
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) AND gr0.classementRubrique<300 then 'serie3'
		when gr0.classementRubrique=504.1 then 'serie7'
		else ''
		end as 'graph',
  gr0.classementRubrique, gr0.rubrique,gr0.libelle, 
   gr14.valeur as 'ARCELOR MITTAL France',
   gr15.valeur as 'IDENTITES MUTUELLE',
   gr16.valeur as 'IRCEM MUTUELLE',
   gr17.valeur as 'IRCEM PREVOYANCE',
   gr18.valeur as 'IRCEM RETRAITE',
   gr19.valeur as 'UNMI',
   gr20.valeur as 'TOTAL EXTERNE',
  (gr20_amount.valeur/NULLIF(gr20_all.valeur,0)) as ' '   
  from [PTF_RAPPORT] as gr0
  left outer join [PTF_RAPPORT] as gr14 on gr14.rubrique = gr0.rubrique
  and gr14.Groupe= 'ARCELOR MITTAL France' and gr14.cle = @rapportCle and gr14.date = @date    
  left outer join [PTF_RAPPORT] as gr15 on gr15.rubrique = gr0.rubrique
  and gr15.Groupe= 'IDENTITES MUTUELLE' and gr15.cle = @rapportCle and gr15.date = @date    
  left outer join [PTF_RAPPORT] as gr16 on gr16.rubrique = gr0.rubrique
  and gr16.Groupe= 'IRCEM MUTUELLE' and gr16.cle = @rapportCle and gr16.date = @date    
  left outer join [PTF_RAPPORT] as gr17 on gr17.rubrique = gr0.rubrique
  and gr17.Groupe= 'IRCEM PREVOYANCE' and gr17.cle = @rapportCle and gr17.date = @date    
  left outer join [PTF_RAPPORT] as gr18 on gr18.rubrique = gr0.rubrique
  and gr18.Groupe= 'IRCEM RETRAITE' and gr18.cle = @rapportCle and gr18.date = @date    
  left outer join [PTF_RAPPORT] as gr19 on gr19.rubrique = gr0.rubrique
  and gr19.Groupe= 'UNMI' and gr19.cle = @rapportCle and gr19.date = @date    
  left outer join [PTF_RAPPORT] as gr20 on gr20.rubrique = gr0.rubrique
  and gr20.Groupe= 'EXTERNE' and gr20.cle = @rapportCle and gr20.date = @date    
  left outer join [PTF_RAPPORT] as gr20_amount on gr20_amount.rubrique = gr0.rubrique
   and gr20_amount.groupe= gr20.groupe and gr20_amount.cle = @cle_Montant and gr20_amount.date = @date
  left outer join [PTF_RAPPORT] as gr20_all on gr20_all.rubrique = @rubriqueEncours
   and gr20_all.groupe= gr20.groupe and gr20_all.cle = @cle_Montant and gr20_all.date = @date
  
    where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date    
  order by
  case  when gr0.classementRubrique < 100 then 100*gr0.classementRubrique
  else gr0.classementRubrique
  end,gr0.libelle

select
case when gr0.classementRubrique=1 then 2 
		when gr0.classementRubrique<10 then 0
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) AND gr0.classementRubrique<300 then 1
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) then 3
                else 4
		end as 'style',
case when gr0.classementRubrique=1 then ''
		when gr0.classementRubrique=201.1 then 'graph2'
		when gr0.classementRubrique=CONVERT(INT,gr0.classementRubrique) AND gr0.classementRubrique<300 then 'serie4'
		when gr0.classementRubrique=504.1 then 'serie8'
		else ''
		end as 'graph',
gr0.classementRubrique,gr0.rubrique, gr0.libelle, 
 gr0.valeur as 'FGA',
 (gr0_amount.valeur/NULLIF(gr0_all.valeur,0)) as ' ' 
from [PTF_RAPPORT] as gr0
  left outer join [PTF_RAPPORT] as gr0_amount on gr0_amount.rubrique = gr0.rubrique
   and gr0_amount.groupe= gr0.groupe and gr0_amount.cle = @cle_Montant and gr0_amount.date = @date
  left outer join [PTF_RAPPORT] as gr0_all on gr0_all.rubrique = @rubriqueEncours
   and gr0_all.groupe= gr0.groupe and gr0_all.cle = @cle_Montant and gr0_all.date = @date

  where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date    
order by
case  when gr0.classementRubrique < 100 then 100*gr0.classementRubrique
else gr0.classementRubrique
end,gr0.libelle
