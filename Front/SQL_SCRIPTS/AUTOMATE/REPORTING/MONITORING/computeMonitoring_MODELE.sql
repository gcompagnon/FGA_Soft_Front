USE E2DBFGA01

declare @derniereDateInventaire as datetime
set  @derniereDateInventaire = (Select max (dateInventaire) from PTF_FGA where Compte = '7201106')


declare @dateInventaire as datetime
declare @forcage as char(1)
set @forcage = NULL
--set @forcage='F'
set  @dateInventaire = '20/06/2012'


declare @nb int

set @nb = (Select count(*) from PTF_RAPPORT_NIV2 where date = @dateInventaire and Groupe ='FGA' and cle = 'ConcentGrpEmetteurs' )
IF @nb = 0 OR NOT (@forcage is NULL)
BEGIN
print 'ConcentGrpEmetteurs'
execute ReportConcentrationGrpEmetteurs   @dateInventaire , 4 , 0 , 'ConcentGrpEmetteurs'
END

set @nb = (Select count(*) from PTF_RAPPORT_NIV2 where date = @dateInventaire and Groupe ='FGA' and cle = 'MonitorExpoPays' )
IF @nb = 0 OR NOT (@forcage is NULL)
BEGIN
print 'MonitorExpoPays'
execute ReportMonitoringExpositionPays    @dateInventaire , 4 , 'MonitorExpoPays'
END

set @nb = (Select count(*) from PTF_RAPPORT_NIV2 where date = @dateInventaire and Groupe ='FGA' and cle = 'MonitorExpoPaysDur' )
IF @nb = 0 OR NOT (@forcage is NULL)
BEGIN
print 'MonitorExpoPaysDur'
execute ReportMonitoringExpositionPaysDur @dateInventaire , 4 , 'MonitorExpoPaysDur'
END

set @nb = (Select count(*) from PTF_RAPPORT_NIV2 where date = @dateInventaire and Groupe ='FGA' and cle = 'MonitorExpoPaysCompt' )
IF @nb = 0 OR NOT (@forcage is NULL)
BEGIN
print 'MonitorExpoPaysCompt'
execute ReportMonitoringExpositionPaysVNC @dateInventaire , 4 , 'MonitorExpoPaysCompt'
END

set @nb = (Select count(*) from PTF_RAPPORT where date = @dateInventaire and Groupe ='FGA' and cle = 'MonitoringGroupe' )
IF @nb = 0 OR NOT (@forcage is NULL)
BEGIN
print 'MonitoringGroupe'
execute ReportMonitoringGroupe            @dateInventaire , 4 , 'MonitoringGroupe'
END

set @nb = (Select count(*) from PTF_RAPPORT where date = @dateInventaire and Groupe ='FGA' and cle = 'MonitoringGroupeDura' )
IF @nb = 0 OR NOT (@forcage is NULL)
BEGIN
print 'MonitoringGroupeDuration'
execute ReportMonitoringGroupeDuration    @dateInventaire , 4 , 'MonitoringGroupeDura'
END

