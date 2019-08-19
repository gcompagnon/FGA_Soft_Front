
--------------------------------------------------------------------------------------------------
-------           Execution des procedures stock�es pour le calcul des donn�es de monitoring
--------------------------------------------------------------------------------------------------

---Par defaut la date utilis�e est la derniere disponible 
-- et pas de forcage
declare @dateInventaire as datetime
declare @forcage as char(1)
set @forcage = 'N'
set  @dateInventaire = (Select max (dateInventaire) from PTF_FGA where Compte = '7201106')


declare @nb int


set @nb = (Select count(*) from PTF_RAPPORT where date = @dateInventaire and Groupe ='FGA' and cle = 'MonitoringRatingRe' )
IF @nb = 0 or ( ( @forcage <> 'N' OR @forcage <> 'n' ) and  @forcage <> '')
BEGIN
print 'MonitoringRatingRe'
execute ReportMonitoringRatingRe   @dateInventaire , 4 , 'MonitoringRatingRe'
END
