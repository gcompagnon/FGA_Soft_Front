--Liste de tous les comptes à utiliser
-- le nom du parametre est "Compte" / le nom de la colonne en retour

declare @dateInventaire as datetime
set  @dateInventaire = '31/05/2013'

select distinct Compte from PTF_FGA where dateinventaire = @dateInventaire
