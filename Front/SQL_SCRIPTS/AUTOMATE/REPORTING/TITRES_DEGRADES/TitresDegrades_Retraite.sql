DECLARE @date datetime
SET @date=(Select GETDATE())

SELECT 
      convert( varchar(10),[Dateinventaire],103) as DateInventaire
      ,[Secteur]
      ,[Grp_Emetteur]
      ,[Rating]
      ,sum([Valeur_Boursiere]+[Coupon_Couru]) as Encours
      ,sum([Vie_residuelle]*([Valeur_Boursiere]+[Coupon_Couru]))/sum([Valeur_Boursiere]+[Coupon_Couru]) as Vie_Moyenne     
FROM [dbo].[PTF_TRANSPARISE]
WHERE GROUPE in ('MM AGIRC', 'MM ARRCO')
AND DATEINVENTAIRE=@date
AND NUMERO_NIVEAU=5
AND [Groupe_rating] in ('BBB', 'BB')
AND [Secteur]<>'EMPRUNTS D''ETAT'
GROUP BY [Dateinventaire]
      ,[Secteur]
      ,[Grp_Emetteur]
      ,[Rating]
      ,[Groupe_rating]
order by  Secteur,  Grp_Emetteur
