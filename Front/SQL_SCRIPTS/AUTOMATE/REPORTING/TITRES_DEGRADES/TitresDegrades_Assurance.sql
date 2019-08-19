DECLARE @date datetime
SET @date=(Select GETDATE())

SELECT [Groupe]
      ,[Rating]
      ,[Grp_Emetteur]
      ,sum(([Valeur_Boursiere]+[Coupon_Couru])*datediff(day,[Dateinventaire],[maturite])/365.25)/sum([Valeur_Boursiere]+[Coupon_Couru]) as vie_residuelle_moyenne
      ,sum([Valeur_Boursiere]+[Coupon_Couru])/1e6 as 'Encours Meuros'
  FROM [dbo].[PTF_FGA]
  WHERE (GROUPE in ('SAPREM',  'MMP','CAPREVAL','INPR','MM MUTUELLE', 'CMAV','AUXIA', 'QUATREM') 
       OR COMPTE in ('6400001','6400002','8010015') ) 
  AND RATING IN ('BB+','BB','BB-','B+','B','B-','CCC+','CCC','CCC-','CC+','CC','CC-','C+','C','C-','CD')
  AND DATEINVENTAIRE = @date
  AND SECTEUR <>'EMPRUNTS D''ETAT'
  AND SECTEUR <>'AGENCIES'
GROUP BY  [Groupe]
         ,[Rating]
         ,[Grp_Emetteur]
ORDER BY [Groupe], sum([Valeur_Boursiere]+[Coupon_Couru])/1e6 desc