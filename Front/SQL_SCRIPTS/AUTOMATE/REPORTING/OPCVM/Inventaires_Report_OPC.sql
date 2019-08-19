/*
Requete de stephane pour daniel
*/

Declare @date datetime
set @date=(select max(date) from PTF_AN where date > DATEADD(month,-1, GETDATE()) and date < DATEADD(day, -DAY(GETDATE()) , GETDATE()))

SELECT [Groupe]
      ,[Dateinventaire]
      ,[Compte]
      ,[ISIN_Ptf]
      ,[Libelle_Ptf]
      ,[code_Titre]
      ,[isin_titre]
      ,[Libelle_Titre]
      ,([Valeur_Boursiere]+[Coupon_Couru]) as VB_Totale
      ,[Type_Produit]
      ,[Devise_Titre]
      ,[Secteur]
      ,[Sous_Secteur]
      ,[Pays]
      ,[Emetteur]
      ,[Rating]
      ,[Grp_Emetteur]
      ,maturite_finale=case
      when [maturite]=0 then NULL else [maturite] end
      ,TTM=case
      when [maturite]> @date then(((convert(numeric,[maturite])) -(convert(numeric,@date)))/365.25) else '0' end
      ,Categorie=case
      when [Rating] in ('BB+','NR','BB','BB-','B+','B','B-','CCC+','CCC','CCC-','CC','C','CD') and left([Sous_Secteur],7)='EMPRUNT'
 then 'HY'
      when [Rating] in ('AAA','AA+','AA','AA-','A+','A','A-','BBB+','BBB','BBB-')and left([Sous_Secteur],7)='EMPRUNT' then 'IG'
      else '' end
      ,[duration]
      ,[sensibilite]
      ,[quantite]
      ,[coupon]
      ,[rendement]
  FROM [dbo].[PTF_FGA]    
   where  [Dateinventaire]=@date and [Groupe]='OPCVM'
  
  order by [Compte]