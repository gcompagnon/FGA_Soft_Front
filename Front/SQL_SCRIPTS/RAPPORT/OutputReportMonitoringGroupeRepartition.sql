--USE FGA_DEV
--go
--------------------------------------------------------------------------------------------------
-------           Sortie de l'état donnant la répartition des groupes sur les secteurs avec leur répartition (poids sur l'encours total)
--------------------------------------------------------------------------------------------------

declare @date as datetime
SET @date = '19/10/2011'

declare @rapportCle as char(20)
set @rapportCle = 'MonitoringGroupe'

declare @rubriqueEncours as char(20)
set @rubriqueEncours = 'encours'

declare @niveauInventaire as tinyint
--set @niveauInventaire = <A PARAMETRER>
set @niveauInventaire = 2

declare @returnCode int
declare @nb int

set @nb = (Select count(*) from PTF_RAPPORT where cle = @rapportCle and date = @date)
print @nb
IF @nb = 0 
BEGIN
--Retour de 0 si OK
exec @returnCode = ReportMonitoringGroupe @date,@niveauInventaire, @rapportCle OUTPUT
END 


declare @sdate1 as char(8)
declare @sdate2 as char(8)
SET @sdate1 = convert(char(8),@date,3)
SET @sdate2 = convert(char(8),@date,3)
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
  gr01.classementRubrique as 'classement', gr01.libelle as 'libelle' ,    
 gr01.valeur as 'MM AGIRC', 100*(gr01.valeur/NULLIF(gr02.valeur,0)) as 'MM AGIRC-poids',
gr11.valeur as 'MM ARRCO', 100*(gr11.valeur/NULLIF(gr12.valeur,0)) as 'MM ARRCO-poids',
gr21.valeur as 'RETRAITE', 100*(gr21.valeur/NULLIF(gr22.valeur,0)) as 'RETRAITE-poids',
gr31.valeur as 'MMP', 100*(gr31.valeur/NULLIF(gr32.valeur,0)) as 'MMP-poids',
gr41.valeur as 'INPR' , 100*(gr41.valeur/NULLIF(gr42.valeur,0)) as 'MUT2M-poids',
gr51.valeur as 'CAPREVAL', 100*(gr51.valeur/NULLIF(gr52.valeur,0)) as 'CMAV-poids',
gr61.valeur as 'CMAV', 100*(gr61.valeur/NULLIF(gr62.valeur,0)) as 'QUATREM-poids',
gr71.valeur as 'MM MUTUELLE', 100*(gr71.valeur/NULLIF(gr72.valeur,0)) as 'AUXIA-poids',
gr81.valeur as 'SAPREM', 100*(gr81.valeur/NULLIF(gr82.valeur,0)) as 'CAPREVAL-poids',
gr91.valeur as 'AUXIA', 100*(gr91.valeur/NULLIF(gr92.valeur,0)) as 'INPR-poids',
gr101.valeur as 'QUATREM', 100*(gr101.valeur/NULLIF(gr102.valeur,0)) as 'SAPREM-poids',
gr111.valeur as 'AUTRES', 100*(gr111.valeur/NULLIF(gr112.valeur,0)) as 'AUTRES-poids',
gr121.valeur as 'ASSURANCE', 100*(gr121.valeur/NULLIF(gr122.valeur,0)) as 'ASSURANCE-poids',
gr131.valeur as 'ARCELOR MITTAL France', 100*(gr131.valeur/NULLIF(gr132.valeur,0)) as 'ARCELOR MITTAL France-poids',
gr141.valeur as 'IDENTITES MUTUELLE', 100*(gr141.valeur/NULLIF(gr142.valeur,0)) as 'IDENTITES MUTUELLE-poids',
gr151.valeur as 'IRCEM MUTUELLE', 100*(gr151.valeur/NULLIF(gr152.valeur,0)) as 'IRCEM MUTUELLE-poids',
gr161.valeur as 'IRCEM PREVOYANCE', 100*(gr161.valeur/NULLIF(gr162.valeur,0)) as 'IRCEM PREVOYANCE-poids',
gr171.valeur as 'IRCEM RETRAITE', 100*(gr171.valeur/NULLIF(gr172.valeur,0)) as 'IRCEM RETRAITE-poids',
gr181.valeur as 'UNMI', 100*(gr181.valeur/NULLIF(gr182.valeur,0)) as 'UNMI-poids',
gr191.valeur as 'TOTAL EXTERNE', 100*(gr191.valeur/NULLIF(gr192.valeur,0)) as 'TOTAL EXTERNE-poids',   
gr201.valeur as 'FGA', 100*(gr201.valeur/NULLIF(gr202.valeur,0)) as 'FGA-poids'
  from [PTF_RAPPORT] as gr01
 left outer join [PTF_RAPPORT] as gr11 on gr11.rubrique = gr01.rubrique
   and gr11.Groupe= 'MM ARRCO' and gr11.cle = @rapportCle and gr11.date = @date
 left outer join [PTF_RAPPORT] as gr21 on gr21.rubrique = gr01.rubrique
   and gr21.Groupe= 'RETRAITE' and gr21.cle = @rapportCle and gr21.date = @date
 left outer join [PTF_RAPPORT] as gr31 on gr31.rubrique = gr01.rubrique
   and gr31.Groupe= 'MMP' and gr31.cle = @rapportCle and gr31.date = @date
 left outer join [PTF_RAPPORT] as gr41 on gr41.rubrique = gr01.rubrique
   and gr41.Groupe= 'INPR' and gr41.cle = @rapportCle and gr41.date = @date
 left outer join [PTF_RAPPORT] as gr51 on gr51.rubrique = gr01.rubrique
   and gr51.Groupe= 'CAPREVAL' and gr51.cle = @rapportCle and gr51.date = @date
 left outer join [PTF_RAPPORT] as gr61 on gr61.rubrique = gr01.rubrique
   and gr61.Groupe= 'CMAV' and gr61.cle = @rapportCle and gr61.date = @date
 left outer join [PTF_RAPPORT] as gr71 on gr71.rubrique = gr01.rubrique
   and gr71.Groupe= 'MUT2M' and gr71.cle = @rapportCle and gr71.date = @date
 left outer join [PTF_RAPPORT] as gr81 on gr81.rubrique = gr01.rubrique
   and gr81.Groupe= 'SAPREM' and gr81.cle = @rapportCle and gr81.date = @date
 left outer join [PTF_RAPPORT] as gr91 on gr91.rubrique = gr01.rubrique
   and gr91.Groupe= 'AUXIA' and gr91.cle = @rapportCle and gr91.date = @date
 left outer join [PTF_RAPPORT] as gr101 on gr101.rubrique = gr01.rubrique
   and gr101.Groupe= 'QUATREM' and gr101.cle = @rapportCle and gr101.date = @date
 left outer join [PTF_RAPPORT] as gr111 on gr111.rubrique = gr01.rubrique
   and gr111.Groupe= 'AUTRES' and gr111.cle = @rapportCle and gr111.date = @date
 left outer join [PTF_RAPPORT] as gr121 on gr121.rubrique = gr01.rubrique
   and gr121.Groupe= 'ASSURANCE' and gr121.cle = @rapportCle and gr121.date = @date
 left outer join [PTF_RAPPORT] as gr131 on gr131.rubrique = gr01.rubrique
   and gr131.Groupe= 'ARCELOR MITTAL France' and gr131.cle = @rapportCle and gr131.date = @date   
 left outer join [PTF_RAPPORT] as gr141 on gr141.rubrique = gr01.rubrique
   and gr141.Groupe= 'IDENTITES MUTUELLE' and gr141.cle = @rapportCle and gr141.date = @date
 left outer join [PTF_RAPPORT] as gr151 on gr151.rubrique = gr01.rubrique
   and gr151.Groupe= 'IRCEM MUTUELLE' and gr151.cle = @rapportCle and gr151.date = @date
 left outer join [PTF_RAPPORT] as gr161 on gr161.rubrique = gr01.rubrique
   and gr161.Groupe= 'IRCEM PREVOYANCE' and gr161.cle = @rapportCle and gr161.date = @date
 left outer join [PTF_RAPPORT] as gr171 on gr171.rubrique = gr01.rubrique
   and gr171.Groupe= 'IRCEM RETRAITE' and gr171.cle = @rapportCle and gr171.date = @date
 left outer join [PTF_RAPPORT] as gr181 on gr181.rubrique = gr01.rubrique
   and gr181.Groupe= 'UNMI' and gr181.cle = @rapportCle and gr181.date = @date
 left outer join [PTF_RAPPORT] as gr191 on gr191.rubrique = gr01.rubrique
   and gr191.Groupe= 'EXTERNE' and gr191.cle = @rapportCle and gr191.date = @date
 left outer join [PTF_RAPPORT] as gr201 on gr201.rubrique = gr01.rubrique
   and gr201.Groupe= 'FGA' and gr201.cle = @rapportCle and gr201.date = @date   
   left outer join [PTF_RAPPORT] as gr02 on gr02.rubrique = @rubriqueEncours
   and gr02.Groupe= 'MM AGIRC' and gr02.cle = @rapportCle and gr02.date = @date   
 left outer join [PTF_RAPPORT] as gr12 on gr12.rubrique = @rubriqueEncours
   and gr12.Groupe= 'MM ARRCO' and gr12.cle = @rapportCle and gr12.date = @date
 left outer join [PTF_RAPPORT] as gr22 on gr22.rubrique = @rubriqueEncours
   and gr22.Groupe= 'RETRAITE' and gr22.cle = @rapportCle and gr22.date = @date
 left outer join [PTF_RAPPORT] as gr32 on gr32.rubrique = @rubriqueEncours
   and gr32.Groupe= 'MMP' and gr32.cle = @rapportCle and gr32.date = @date
 left outer join [PTF_RAPPORT] as gr42 on gr42.rubrique = @rubriqueEncours
   and gr42.Groupe= 'INPR' and gr42.cle = @rapportCle and gr42.date = @date
 left outer join [PTF_RAPPORT] as gr52 on gr52.rubrique = @rubriqueEncours
   and gr52.Groupe= 'CAPREVAL' and gr52.cle = @rapportCle and gr52.date = @date
 left outer join [PTF_RAPPORT] as gr62 on gr62.rubrique = @rubriqueEncours
   and gr62.Groupe= 'CMAV' and gr62.cle = @rapportCle and gr62.date = @date
 left outer join [PTF_RAPPORT] as gr72 on gr72.rubrique = @rubriqueEncours
   and gr72.Groupe= 'MM MUTUELLE' and gr72.cle = @rapportCle and gr72.date = @date
 left outer join [PTF_RAPPORT] as gr82 on gr82.rubrique = @rubriqueEncours
   and gr82.Groupe= 'SAPREM' and gr82.cle = @rapportCle and gr82.date = @date
 left outer join [PTF_RAPPORT] as gr92 on gr92.rubrique = @rubriqueEncours
   and gr92.Groupe= 'AUXIA' and gr92.cle = @rapportCle and gr92.date = @date
 left outer join [PTF_RAPPORT] as gr102 on gr102.rubrique = @rubriqueEncours
   and gr102.Groupe= 'QUATREM' and gr102.cle = @rapportCle and gr102.date = @date
 left outer join [PTF_RAPPORT] as gr112 on gr112.rubrique = @rubriqueEncours
   and gr112.Groupe= 'AUTRES' and gr112.cle = @rapportCle and gr112.date = @date
 left outer join [PTF_RAPPORT] as gr122 on gr122.rubrique = @rubriqueEncours
   and gr122.Groupe= 'ASSURANCE' and gr122.cle = @rapportCle and gr122.date = @date
left outer join [PTF_RAPPORT] as gr132 on gr132.rubrique = @rubriqueEncours
   and gr132.Groupe= 'ARCELOR MITTAL France' and gr132.cle = @rapportCle and gr132.date = @date   
 left outer join [PTF_RAPPORT] as gr142 on gr142.rubrique = @rubriqueEncours
   and gr142.Groupe= 'IDENTITES MUTUELLE' and gr142.cle = @rapportCle and gr142.date = @date
 left outer join [PTF_RAPPORT] as gr152 on gr152.rubrique = @rubriqueEncours
   and gr152.Groupe= 'IRCEM MUTUELLE' and gr152.cle = @rapportCle and gr152.date = @date
 left outer join [PTF_RAPPORT] as gr162 on gr162.rubrique = @rubriqueEncours
   and gr162.Groupe= 'IRCEM PREVOYANCE' and gr162.cle = @rapportCle and gr162.date = @date
 left outer join [PTF_RAPPORT] as gr172 on gr172.rubrique = @rubriqueEncours
   and gr172.Groupe= 'IRCEM RETRAITE' and gr172.cle = @rapportCle and gr172.date = @date
 left outer join [PTF_RAPPORT] as gr182 on gr182.rubrique = @rubriqueEncours
   and gr182.Groupe= 'UNMI' and gr182.cle = @rapportCle and gr182.date = @date
 left outer join [PTF_RAPPORT] as gr192 on gr192.rubrique = @rubriqueEncours
   and gr192.Groupe= 'EXTERNE' and gr192.cle = @rapportCle and gr192.date = @date
  left outer join [PTF_RAPPORT] as gr202 on gr202.rubrique = @rubriqueEncours
   and gr202.Groupe= 'FGA' and gr202.cle = @rapportCle and gr202.date = @date   
    where gr01.Groupe= 'MM AGIRC' and gr01.cle = @rapportCle and gr01.date = @date
  order by  
  case  when gr01.classementRubrique < 100 then 100*gr01.classementRubrique
  else gr01.classementRubrique
  end  ,
  gr01.libelle 
  
  
