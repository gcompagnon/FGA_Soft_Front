/****** Script for SelectTopNRows command from SSMS  ******/
Declare @date datetime
set @date = '30/09/2014'

Declare @compte varchar(12)
set @compte = '6100101'



SELECT [Groupe]
      ,[Dateinventaire]
      ,[Compte]
      ,[ISIN_Ptf]
      ,[Libelle_Ptf]
      ,[code_Titre]
      ,[isin_titre]
      ,[Libelle_Titre]
      ,([Valeur_Boursiere]+[Coupon_Couru]) as Exposition_Totale
      ,Valeur_Boursiere=case
      when LEFT(Type_produit,3) in ('FUT') then PMV
      when LEFT(Libelle_Titre,13) in ('Liquidité(Fut') then '0'
      else ([Valeur_Boursiere]+[Coupon_Couru]) end
      ,Type_Produit=case
      when LEFT(Type_produit,3)='FUT' then 'Futures'
      when LEFT(Type_produit,3)='BEN' then 'Options'
      when LEFT(Type_produit,3)='Act' and code_Titre<>isin_titre then 'Options'
      when LEFT(Libelle_Titre,13)='Liquidité(Opt' then 'Options'
      when Type_Produit='Cash' or Secteur='FONDS TRESORERIE' then 'Trésorerie'
      when Left(Type_Produit,5)in ('Fonds','Sicav') then Secteur
      when LEFT (Type_Produit,5)='Oblig' and LEFT(Secteur,10) in ('CORPORATES','EMPRUNTS F','AGENCIES N') then 'Obligations crédit'
      when LEFT (Type_Produit,5)='Oblig' and LEFT(Secteur,10) in ('EMPRUNTS D') then 'Obligations état' end    
           
      ,[Devise_Titre]
      ,Secteur=case
      when LEFT(Type_produit,3)='FUT' and Libelle_titre like '%dividende%' then 'Dérivés Dividendes'
      when LEFT(Type_produit,3) in ('FUT','BEN') and Secteur like '%Action%' then 'Dérivés Actions'
      when LEFT(Type_produit,3) in ('FUT','BEN') and left(Secteur,3) in ('Bie','Con','Ene','Fin','Ind','Mat','San','Ser','Tec','Tel')  then 'Dérivés Actions'
      when LEFT(Type_produit,3)='Act' and code_Titre<>isin_titre then 'Dérivés Actions' else Secteur end
      
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
  FROM [e0dbfga01].[dbo].[PTF_FGA]  
  
   where  [Dateinventaire]=@date and [Groupe]='OPCVM' and Compte=@compte
  
  order by [Compte]