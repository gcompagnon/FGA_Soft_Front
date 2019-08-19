-- PASSAGE DU MODE DE CALCUL ACT_Agregats_Secteur de Moyenne à Mediane et inversement



ACT_Agregats_Secteur_Moyenne
ACT_Agregats_Secteur_Mediane

exec sp_rename 'ACT_Agregats_Secteur', 'ACT_Agregats_Secteur_Mediane'
exec sp_rename 'ACT_Agregats_Secteur_Moyenne', 'ACT_Agregats_Secteur'



exec sp_rename 'ACT_Agregats_Secteur', 'ACT_Agregats_Secteur_Moyenne_Opti'
exec sp_rename 'ACT_Agregats_Secteur_Mediane', 'ACT_Agregats_Secteur'


exec sp_rename 'ACT_Agregats_Secteur', 'ACT_Agregats_Secteur_Mediane'
exec sp_rename 'ACT_Agregats_Secteur_Moyenne_Opti', 'ACT_Agregats_Secteur'


select * from DATA_FACTSET where date = '13/09/2014'

---

CREATE PROCEDURE [dbo].[ACT_Agregats_Secteur]

	@Date As DATETIME
AS
execute [dbo].[ACT_Agregats_Secteur_Harmonique_Opti] @date
execute [dbo].[ACT_Agregats_Secteur_Moyenne_Opti] @date
GO



update DATA_FACTSET
set DATE ='13/09/2014'
where date = '14/09/2014'

