USE [E2DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[STRAT_Traitement_Proxy]    Script Date: 04/11/2013 10:27:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[STRAT_Traitement_Proxy]

AS

BEGIN 

Declare @titre varchar(20)
Declare @poids float
Declare @date DateTime
Declare @grille varchar(15)

/*Création d'une table contenant tous les produits de PTF_FGA, qui n'ont pas de grille
mais qui ont un proxy. On pourra ainsi récupérer la décomposition du titre via son proxy*/
Create table #tmp (
	code_titre varchar(20),
	[date] datetime,
	type_produit varchar(60), 
	secteur varchar(100),
	Sous_Secteur varchar(100)
)

create table #titreAjour(
	code_titre varchar(15),
	[date] datetime,
)

insert into #titreAjour
	select distinct p.code_proxy,max(p.[date]) 
	from ptf_proxy p 
		inner join strat_titre_grille st on  st.isin_titre = p.code_proxy and st.[date] = p.[date] 
	group by p.code_proxy

Create table #LastFGA  (	
						code_titre varchar(15),
						[date] datetime
						)
insert into #LastFGA 
	SELECT [code_titre],Max(Dateinventaire) as Madate 
	FROM [PTF_FGA] Group by code_titre

insert into  #tmp 
	Select distinct PTF.[code_titre],
					PTF.Dateinventaire,
					PTF.[Type_Produit],
					PTF.[Secteur],
					PTF.[Sous_Secteur]
	From 
	#LastFGA Rqt
		INNER JOIN [PTF_FGA] Ptf on Rqt.code_Titre = PTF.code_titre and Rqt.[Date] = PTF.dateinventaire 
	Where Rqt.code_titre not in (
		select distinct ISIN_titre from [STRAT_TITRE_GRILLE]
		)
	and Rqt.code_titre in (
		select distinct code_proxy from PTF_proxy
		)

						
Insert into #tmp
	Select distinct PTF.[code_titre],
					PTF.Dateinventaire,
					PTF.[Type_Produit],
					PTF.[Secteur],
					PTF.[Sous_Secteur]
	From 
	#LastFGA Rqt
		INNER JOIN [PTF_FGA] Ptf on Rqt.code_Titre = PTF.code_titre and Rqt.[date] = PTF.dateinventaire 
	Where Rqt.code_titre not in (
		select distinct code_titre from #tmp
		)
	and Rqt.code_titre in (
		select distinct code_proxy from PTF_proxy where code_proxy 
		not in (
			select code_proxy from #titreAjour
		)
		)
	and Rqt.code_titre in (
		select distinct isin_titre from Strat_titre_grille where poids <> 1 
		)


--Création d'une table contenant les titres les plus récents qui ont déja une grille
Create table #Titrerecent (
	code_titre varchar(20),
	[date] datetime
)
	
insert into #Titrerecent 
	Select isin_titre,
	Max([Date]) 
	from strat_titre_grille 
	group by Isin_titre

--Création d'une table contenant les proxys les plus récents
Create table #LastProxy (
	code_proxy varchar(20), 
	[date] datetime
)
	
Insert into #LastProxy 
	select  code_proxy,
			max([date]) 
	from ptf_proxy 
	group by code_proxy


--Création d'une table qui contient pour chaque proxy, le poids des différents titres
Create table #ProxyGrille (
	code_proxy varchar(20), 
	poids float, 
	grille varchar(15)
)

insert into #ProxyGrille 
	select distinct
		p.code_proxy ,
		sum(p.poids_VB),
		id_grille
	from #tmp t 
	inner JOIN PTF_proxy p on p.code_proxy = t.code_titre 
	left join strat_titre_grille s on  s.ISIN_titre = p.code_titre 
	inner join #Titrerecent ti on ti.code_titre = p.code_titre and ti.[date] = s.[date]
	inner join #LastProxy l on l.code_proxy = p.code_proxy and l.[date] = P.[date]
	group by p.code_proxy,
			 id_grille
	order by p.code_proxy


/*Création d'une table contenant le poids totale des proxys
Si le poids est de 1, c'est qu'on peut définir sa décomosition selon sa grille
Sinon, il nous manque de l'informations et on ne peut rien faire sur ce titre*/
Create table #ProxyPoids (
	code_proxy varchar(20),
	poids float
)
Insert into #ProxyPoids
	select code_proxy, 
	sum(poids) 
	from #ProxyGrille 
	group by code_proxy 
	order by code_proxy

--Table temporaire pour le traitement des différents titres. 
Create table #stockagetemporaire (
	code_proxy varchar(20), 
	poids float, 
	grille varchar(15)
)


Begin TRANSACTION
  -- Boucle tant qu'il nous reste des titres à traiter
  WHILE EXISTS(SELECT TOP 1 * FROM #ProxyPoids )
  BEGIN
    SET @titre = (select TOP 1 code_proxy FROM #ProxyPoids)
    SET @poids = (select TOP 1 poids FROM #ProxyPoids)
    /*Selon les arrondis, on peut avoir une valeur au alentour de 1
    On considère le titre comme étant OK*/
    IF (@poids between 0.999999 and 1.00000000001)
    BEGIN
      Insert into #stockagetemporaire select * from #ProxyGrille where code_proxy = @titre
	  --On affecte tous les titres du proxy dans Strat_titre_Grille
	  WHILE EXISTS(SELECT TOP 1 * FROM #stockagetemporaire )
	  BEGIN
	    Set @Date = (Select top 1 l.date from #stockagetemporaire s inner join #LastProxy l on s.code_proxy = l.code_proxy)
	    Set @grille = (Select top 1 grille from #stockagetemporaire )
	    Set @poids = (Select top 1 poids from #stockagetemporaire )
	    Insert into strat_titre_grille values (@titre, @grille,@Date,@poids, 'Auto')
	    Delete top(1) from #stockagetemporaire
	  END
    END
    Delete Top (1) from #ProxyPoids
  END

COMMIT TRANSACTION 


drop table #tmp 
drop table #Titrerecent 
drop table #LastProxy
drop table #ProxyPoids
drop table #stockagetemporaire
drop table #proxygrille
drop table #titreAjour
Drop table #LastFGA



END

