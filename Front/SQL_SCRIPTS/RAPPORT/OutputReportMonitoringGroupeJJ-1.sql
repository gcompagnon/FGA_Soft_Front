--USE FGA_DEV
--go
--------------------------------------------------------------------------------------------------
-------           Sortie de l'état donnant la répartition des groupes sur 2 dates et l'évolution sur les 2 dates
--------------------------------------------------------------------------------------------------

declare @date1 as datetime
declare @date2 as datetime
SET @date1 = '28/09/2011'
SET @date2 = '05/10/2011'
declare @rapportCle as char(20)
set @rapportCle = 'MonitoringGroupe'


declare @niveauInventaire as tinyint
--set @niveauInventaire = <A PARAMETRER>
set @niveauInventaire = 2

declare @returnCode int
declare @nb int

set @nb = (Select count(*) from PTF_RAPPORT where cle = @rapportCle and date = @date1)
print @nb
IF @nb = 0 
BEGIN
--Retour de 0 si OK
exec @returnCode = ReportMonitoringGroupe @date1,@niveauInventaire, @rapportCle OUTPUT
END 

set @nb = (Select count(*) from PTF_RAPPORT where cle = @rapportCle and date = @date2)
print @nb
IF @nb = 0 
BEGIN
--Retour de 0 si OK
exec @returnCode = ReportMonitoringGroupe @date2,@niveauInventaire, @rapportCle OUTPUT
END 



declare @sdate1 as char(8)
declare @sdate2 as char(8)
SET @sdate1 = convert(char(8),@date1,3)
SET @sdate2 = convert(char(8),@date2,3)
/*
select 
    0.0 ,     'Evolutions J J-1' as 'libelle' ,
     'MM AGIRC '+@sdate1 ,'MM AGIRC '+@sdate2 , 'Delta' ,
       'MM ARRCO '+@sdate1, 'MM ARRCO '+@sdate2, 'Delta', 
   'RETRAITE '+@sdate1, 'RETRAITE '+@sdate2, 'Delta',
   'MMP '+@sdate1,    'MMP '+@sdate2, 'Delta',
   'MUT2M '+@sdate1,    'MUT2M '+@sdate2, 'Delta',
   'CMAV '+@sdate1,    'CMAV '+@sdate2, 'Delta',
   'QUATREM '+@sdate1,    'QUATREM '+@sdate2, 'Delta',
   'AUXIA '+@sdate1,    'AUXIA '+@sdate2, 'Delta',
   'CAPREVAL ' +@sdate1,    'CAPREVAL ' +@sdate2, 'Delta',
   'INPR '+@sdate1,    'INPR '+@sdate2, 'Delta',
   'SAPREM '+@sdate1,    'SAPREM '+@sdate2, 'Delta',
   'AUTRES '+@sdate1,    'AUTRES '+@sdate2, 'Delta',
   'ASSURANCE '+@sdate1,    'ASSURANCE '+@sdate2, 'Delta',
   'IRCEM '+@sdate1,    'IRCEM '+@sdate2, 'Delta'

union */
  select
  case when gr01.classementRubrique < 100 then 100*gr01.classementRubrique
  else gr01.classementRubrique
  end as classement1,  
  gr01.classementRubrique as 'classement', gr01.libelle as 'libelle' ,    
   gr01.valeur as 'MM AGIRC-1', gr02.valeur as 'MM AGIRC-2', 100*(gr02.valeur-gr01.valeur)/NULLIF(gr01.valeur,0) as 'MM AGIRC-evol',
gr11.valeur as 'MM ARRCO-1', gr12.valeur as 'MM ARRCO-2', 100*(gr12.valeur-gr11.valeur)/NULLIF(gr11.valeur,0) as 'MM ARRCO-evol',
gr21.valeur as 'RETRAITE-1', gr22.valeur as 'RETRAITE-2', 100*(gr22.valeur-gr21.valeur)/NULLIF(gr21.valeur,0) as 'RETRAITE-evol',
gr31.valeur as 'MMP-1', gr32.valeur as 'MMP-2', 100*(gr32.valeur-gr31.valeur)/NULLIF(gr31.valeur,0) as 'MMP-evol',
gr41.valeur as 'INPR-1', gr42.valeur as 'INPR-2', 100*(gr42.valeur-gr41.valeur)/NULLIF(gr41.valeur,0) as 'INPR-evol',
gr51.valeur as 'CAPREVAL-1', gr52.valeur as 'CAPREVAL-2', 100*(gr52.valeur-gr51.valeur)/NULLIF(gr51.valeur,0) as 'CAPREVAL-evol',
gr61.valeur as 'CMAV-1', gr62.valeur as 'CMAV-2', 100*(gr62.valeur-gr61.valeur)/NULLIF(gr61.valeur,0) as 'CMAV-evol',
gr71.valeur as 'MUT2M-1', gr72.valeur as 'MUT2M-2', 100*(gr72.valeur-gr71.valeur)/NULLIF(gr71.valeur,0) as 'MUT2M-evol',
gr81.valeur as 'SAPREM-1', gr82.valeur as 'SAPREM-2', 100*(gr82.valeur-gr81.valeur)/NULLIF(gr81.valeur,0) as 'SAPREM-evol',
gr91.valeur as 'AUXIA-1', gr92.valeur as 'AUXIA-2', 100*(gr92.valeur-gr91.valeur)/NULLIF(gr91.valeur,0) as 'AUXIA-evol',
gr101.valeur as 'QUATREM-1', gr102.valeur as 'QUATREM-2', 100*(gr102.valeur-gr101.valeur)/NULLIF(gr101.valeur,0) as 'QUATREM-evol',
gr111.valeur as 'AUTRES-1', gr112.valeur as 'AUTRES-2', 100*(gr112.valeur-gr111.valeur)/NULLIF(gr111.valeur,0) as 'AUTRES-evol',
gr121.valeur as 'ASSURANCE-1', gr122.valeur as 'ASSURANCE-2', 100*(gr122.valeur-gr121.valeur)/NULLIF(gr121.valeur,0) as 'ASSURANCE-evol',
gr131.valeur as 'IRCEM-1', gr132.valeur as 'IRCEM-2', 100*(gr132.valeur-gr131.valeur)/NULLIF(gr131.valeur,0) as 'IRCEM-evol',
gr141.valeur as 'FGA-1', gr142.valeur as 'FGA-2', 100*(gr142.valeur-gr141.valeur)/NULLIF(gr141.valeur,0) as 'FGA-evol'

  from [PTF_RAPPORT] as gr01
    left outer join [PTF_RAPPORT] as gr11 on gr11.rubrique = gr01.rubrique
   and gr11.Groupe= 'MM ARRCO' and gr11.cle = @rapportCle and gr11.date = @date1
 left outer join [PTF_RAPPORT] as gr21 on gr21.rubrique = gr01.rubrique
   and gr21.Groupe= 'RETRAITE' and gr21.cle = @rapportCle and gr21.date = @date1
 left outer join [PTF_RAPPORT] as gr31 on gr31.rubrique = gr01.rubrique
   and gr31.Groupe= 'MMP' and gr31.cle = @rapportCle and gr31.date = @date1
 left outer join [PTF_RAPPORT] as gr41 on gr41.rubrique = gr01.rubrique
   and gr41.Groupe= 'INPR' and gr41.cle = @rapportCle and gr41.date = @date1
 left outer join [PTF_RAPPORT] as gr51 on gr51.rubrique = gr01.rubrique
   and gr51.Groupe= 'CAPREVAL' and gr51.cle = @rapportCle and gr51.date = @date1
 left outer join [PTF_RAPPORT] as gr61 on gr61.rubrique = gr01.rubrique
   and gr61.Groupe= 'CMAV' and gr61.cle = @rapportCle and gr61.date = @date1
 left outer join [PTF_RAPPORT] as gr71 on gr71.rubrique = gr01.rubrique
   and gr71.Groupe= 'MUT2M' and gr71.cle = @rapportCle and gr71.date = @date1
 left outer join [PTF_RAPPORT] as gr81 on gr81.rubrique = gr01.rubrique
   and gr81.Groupe= 'SAPREM' and gr81.cle = @rapportCle and gr81.date = @date1
 left outer join [PTF_RAPPORT] as gr91 on gr91.rubrique = gr01.rubrique
   and gr91.Groupe= 'AUXIA' and gr91.cle = @rapportCle and gr91.date = @date1
 left outer join [PTF_RAPPORT] as gr101 on gr101.rubrique = gr01.rubrique
   and gr101.Groupe= 'QUATREM' and gr101.cle = @rapportCle and gr101.date = @date1
 left outer join [PTF_RAPPORT] as gr111 on gr111.rubrique = gr01.rubrique
   and gr111.Groupe= 'AUTRES' and gr111.cle = @rapportCle and gr111.date = @date1
 left outer join [PTF_RAPPORT] as gr121 on gr121.rubrique = gr01.rubrique
   and gr121.Groupe= 'ASSURANCE' and gr121.cle = @rapportCle and gr121.date = @date1
 left outer join [PTF_RAPPORT] as gr131 on gr131.rubrique = gr01.rubrique
   and gr131.Groupe= 'IRCEM' and gr131.cle = @rapportCle and gr131.date = @date1
 left outer join [PTF_RAPPORT] as gr141 on gr141.rubrique = gr01.rubrique
   and gr141.Groupe= 'FGA' and gr141.cle = @rapportCle and gr141.date = @date1

   left outer join [PTF_RAPPORT] as gr02 on gr02.rubrique = gr01.rubrique
   and gr02.Groupe= 'MM AGIRC' and gr02.cle = @rapportCle and gr02.date = @date2   
 left outer join [PTF_RAPPORT] as gr12 on gr12.rubrique = gr01.rubrique
   and gr12.Groupe= 'MM ARRCO' and gr12.cle = @rapportCle and gr12.date = @date2
 left outer join [PTF_RAPPORT] as gr22 on gr22.rubrique = gr01.rubrique
   and gr22.Groupe= 'RETRAITE' and gr22.cle = @rapportCle and gr22.date = @date2
 left outer join [PTF_RAPPORT] as gr32 on gr32.rubrique = gr01.rubrique
   and gr32.Groupe= 'MMP' and gr32.cle = @rapportCle and gr32.date = @date2
 left outer join [PTF_RAPPORT] as gr42 on gr42.rubrique = gr01.rubrique
   and gr42.Groupe= 'INPR' and gr42.cle = @rapportCle and gr42.date = @date2
 left outer join [PTF_RAPPORT] as gr52 on gr52.rubrique = gr01.rubrique
   and gr52.Groupe= 'CAPREVAL' and gr52.cle = @rapportCle and gr52.date = @date2
 left outer join [PTF_RAPPORT] as gr62 on gr62.rubrique = gr01.rubrique
   and gr62.Groupe= 'CMAV' and gr62.cle = @rapportCle and gr62.date = @date2
 left outer join [PTF_RAPPORT] as gr72 on gr72.rubrique = gr01.rubrique
   and gr72.Groupe= 'MUT2M' and gr72.cle = @rapportCle and gr72.date = @date2
 left outer join [PTF_RAPPORT] as gr82 on gr82.rubrique = gr01.rubrique
   and gr82.Groupe= 'SAPREM' and gr82.cle = @rapportCle and gr82.date = @date2
 left outer join [PTF_RAPPORT] as gr92 on gr92.rubrique = gr01.rubrique
   and gr92.Groupe= 'AUXIA' and gr92.cle = @rapportCle and gr92.date = @date2
 left outer join [PTF_RAPPORT] as gr102 on gr102.rubrique = gr01.rubrique
   and gr102.Groupe= 'QUATREM' and gr102.cle = @rapportCle and gr102.date = @date2
 left outer join [PTF_RAPPORT] as gr112 on gr112.rubrique = gr01.rubrique
   and gr112.Groupe= 'AUTRES' and gr112.cle = @rapportCle and gr112.date = @date2
 left outer join [PTF_RAPPORT] as gr122 on gr122.rubrique = gr01.rubrique
   and gr122.Groupe= 'ASSURANCE' and gr122.cle = @rapportCle and gr122.date = @date2
 left outer join [PTF_RAPPORT] as gr132 on gr132.rubrique = gr01.rubrique
   and gr132.Groupe= 'IRCEM' and gr132.cle = @rapportCle and gr132.date = @date2
   left outer join [PTF_RAPPORT] as gr142 on gr142.rubrique = gr01.rubrique
   and gr142.Groupe= 'FGA' and gr142.cle = @rapportCle and gr142.date = @date2
    where gr01.Groupe= 'MM AGIRC' and gr01.cle = @rapportCle and gr01.date = @date1
  
  order by  classement1, gr01.libelle
  /*
  case when gr01.classementRubrique < 100 then 100*gr01.classementRubrique
  else gr01.classementRubrique
  end  ,
  gr01.libelle  */
  
  