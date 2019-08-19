DROP PROCEDURE STRAT_Affecter_Titre_Grille

GO

CREATE PROCEDURE STRAT_Affecter_Titre_Grille

AS

--Select * from STRAT_TITRE_GRILLE
BEGIN 

Declare @champ varchar(15)--Colonne de la table
Declare @valeur varchar(60)--Valeur prise par la colonne
Declare @grille varchar(10)-- Grille que l'on associe
Declare @valeurRqt varchar(60) -- Pour gérer les ' en BDD 
DECLARE @sqlCommand varchar(1000)



--Table contenant tous les titres qui n'ont pas de Grille
Create Table #TitreAAffecter (
	[code_Titre] [varchar](20) NOT NULL,
	[Dateinventaire] [datetime] NOT NULL,
	[Type_Produit] [varchar](60) NULL,
	[Secteur] [varchar](60) NULL,
	[Sous_Secteur] [varchar](60) NULL
	)
	
--Table contenant tous les titres qui nécessite d'etre updaté
Create Table #UpdateTitre (
	[code_Titre] [varchar](20) NOT NULL,
	[Dateinventaire] [datetime] NOT NULL,
	[Type_Produit] [varchar](60) NULL,
	[Secteur] [varchar](60) NULL,
	[Sous_Secteur] [varchar](60) NULL
	)

--Table contenant les grilles que l'on veut affecter (Par exemple toutes les grilles Actions Europe)
Create table #Grille (	
	[Champ] [varchar](15) NOT NULL,
	[Valeur] [varchar] (50) NOT NULL,
	[Grille] [varchar] (10) NOT NULL 
)


-- Table pour effectuer tous les traitements
Create table #TmpGrille (	
	[Champ] [varchar](15) NOT NULL,
	[Valeur] [varchar] (50) NOT NULL,
	[Grille] [varchar] (10) NOT NULL 
)
	

Create Table #TmpTitre(
	[code_Titre] [varchar](20) NOT NULL,
	[Dateinventaire] [datetime] NOT NULL,
)
 
Create Table #Tmp(
	[code_Titre] [varchar](20) NOT NULL
)
  
Create Table #Tmpproxy(
	[code_Titre] [varchar](20) NOT NULL,
	[Date] [datetime] NOT NULL,
)

Create Table #MAJProxy(
	[code_Titre] [varchar](20) NOT NULL,
	[Date] [datetime] NOT NULL,
)


Insert into #Tmpproxy
	SELECT distinct [code_titre],
					max([date]) as madate
	FROM [E2DBFGA01].[dbo].[PTF_PROXY]
	where code_titre <> 'Liquidité(OPCVM)'
	group by code_titre
  
  
Insert into #TmpTitre 
	SELECT  [code_titre],
		    Max(Dateinventaire) as Madate
			FROM [PTF_FGA]
			Group by code_titre

--Selectionne tous les titres qui n'ont pas de grille
--On prend les titres les plus récents (un titre peut etre historisé et il y  a donc plusieurs lignes pour ce meme titre)
Insert into #TitreAAffecter 
	Select distinct PTF.[code_titre],
					PTF.Dateinventaire,
					PTF.[Type_Produit],
					PTF.[Secteur],
					PTF.[Sous_Secteur] 
					from #TmpTitre Rqt 
					INNER JOIN [PTF_FGA] Ptf on Rqt.code_Titre = PTF.code_titre and Rqt.[Dateinventaire] = PTF.dateinventaire 
					Where Rqt.code_titre not in (
						select ISIN_titre 
						from [STRAT_TITRE_GRILLE]
						) 

--On prend les titres proxy qui ne sont pas encore transparise et qui ne sont ni dans PTF_FGA
--ni dans STRAT_TITRE_GRILLE
Insert into #TitreAAffecter
	Select distinct prox.code_titre,
		prox.[date],
		Prox.[Type_Produit],
		Prox.[Secteur],
		Prox.[Sous_Secteur]
	from #Tmpproxy tmp 
	inner join ptf_proxy prox on prox.code_titre = tmp.code_titre and prox.date= tmp.date -- where prox.type_produit != 'NULL'
	Where tmp.code_titre not in (
		select ISIN_titre 
		from [STRAT_TITRE_GRILLE]
	)
	and tmp.code_titre not in (
		select code_titre 
		from [PTF_FGA]
		)


--Selection des titres dont la date est plus récente que notre grille 
Insert into #UpdateTitre 
Select distinct PTF.[code_titre],
				PTF.Dateinventaire,
				PTF.[Type_Produit],
				PTF.[Secteur],
				PTF.[Sous_Secteur]
				from #TmpTitre Rqt 
				INNER JOIN [PTF_FGA] Ptf on Rqt.code_Titre = PTF.code_titre and Rqt.[Dateinventaire] = PTF.dateinventaire 
				Where RQT.[code_titre] in (
					Select Code_titre 
					from #TmpTitre tmp 
					inner join (Select  ISIN_TItre,
										Max(Date) as Madate 
								from [STRAT_TITRE_GRILLE] 
								group by ISIN_titre
								) upd on tmp.[code_titre] = upd.Isin_titre 
								where DateInventaire >  Madate
				)
				order by PTF.code_titre
				
Insert into #UpdateTitre 				
Select distinct PTF.[code_titre],
				PTF.[Date],
				PTF.[Type_Produit],
				PTF.[Secteur],
				PTF.[Sous_Secteur]
				from #TmpProxy RQT
				INNER JOIN [PTF_PROXY] Ptf on Rqt.code_Titre = PTF.code_titre and Rqt.[Date] = PTF.[date]
				Where RQt.[code_titre] in (
					Select Code_titre 
					from #TmpProxy tmp 
					inner join (Select  ISIN_TItre,
										Max(Date) as Madate 
								from [STRAT_TITRE_GRILLE] 
								group by ISIN_titre
								) upd on tmp.[code_titre] = upd.Isin_titre 
								where [Date] >  Madate
				)
				and RQT.[code_titre] not in (
					select code_titre 
					from #UpdateTitre
					)
/*
					
Insert into #UpdateTitre 
	select distinct PTF.[code_titre],
					PTF.[Date],
					PTF.[Type_Produit],
					PTF.[Secteur],
					PTF.[Sous_Secteur]
					from #MAJProxy RQT
					LEFT JOIN [PTF_PROXY] Ptf on Rqt.code_Titre = PTF.code_titre and Rqt.[Date] = PTF.[date]
					order by PTF.[code_titre]
					
*/
Insert into #TitreAAffecter select * from #UpdateTitre


--On récupère la table de correspondance qui nous permet d'affecter les titres et les grilles
insert into #Grille 
	select * 
	from [STRAT_CORRESPONDANCE_TITRE_GRILLE]

--Traitement des SWAPS
insert into #TmpGrille 
	select * 
	from #Grille 
	where Valeur='SWAP'

BEGIN TRANSACTION
	SET @champ = (SELECT TOP 1 champ FROM #TmpGrille)
	SET @valeur =UPPER((SELECT TOP 1 Valeur FROM #TmpGrille))
	SET @grille = (SELECT TOP 1 Grille FROM #TmpGrille)

	SET @sqlCommand = 'Select distinct TitGri.code_titre,'''+ @grille+''',TitGri.[Dateinventaire],1,''Auto'' from (SELECT code_titre,PATINDEX(''%'+ @valeur+'%'',[Code_titre])as swap from #TitreAAffecter) Rqt INNER JOIN #TitreAAffecter TitGri on Rqt.code_Titre = TitGri.code_titre where RQt.swap = 1'
	Insert into [STRAT_TITRE_GRILLE]([ISIN_titre] ,[Id_Grille] ,[Date],[Poids],[Source])  EXEC (@sqlCommand)

	SET @sqlCommand = 'Select distinct TitGri.code_titre from (SELECT code_titre,PATINDEX(''%'+ @valeur+'%'',[Code_titre])as swap from #TitreAAffecter) Rqt INNER JOIN #TitreAAffecter TitGri on Rqt.code_Titre = TitGri.code_titre where RQt.swap = 1'
	insert into #tmp EXEC (@sqlCommand)

	Delete from #TitreAAffecter where code_Titre in (Select * from #tmp)

	Delete from #TmpGrille where valeur = @valeur and champ =@champ
COMMIT TRANSACTION

Delete From #Grille where valeur ='SWAP'
Delete from #tmp


--On traite les oblig Inflation (via order by DESC) puis les obligation d'etat 	
insert into #TmpGrille select * from #Grille where Grille='Obl_inf' or Grille='Obl_Etat' order by Grille desc --where Grille = 'Act_Euro'

Begin TRANSACTION
  -- Boucle tant qu'il nous reste des titres de type Obl_inf ou Obl_Etat à traiter
  WHILE EXISTS(SELECT TOP 1 * FROM #TmpGrille)
  BEGIN
    SET @champ = (SELECT TOP 1 champ FROM #TmpGrille)
    SET @valeur =UPPER((SELECT TOP 1 Valeur FROM #TmpGrille))
    SET @valeurRqt = @valeur
    Set @valeurRqt = Replace(substring(@valeurRqt,0,Len(@valeurRqt)+1),'''','''''')
    SET @grille = (SELECT TOP 1 Grille FROM #TmpGrille)
    
	SET @sqlCommand = 'Select [Code_titre], ''' + @grille + ''',[Dateinventaire],1,''Auto'' from #TitreAAffecter where '+ @champ + '= ''' + @valeurRqt+''''
	Insert into [STRAT_TITRE_GRILLE]([ISIN_titre] ,[Id_Grille] ,[Date],[Poids],[Source])  EXEC (@sqlCommand)
    
    SET @sqlCommand = 'Select [Code_titre] from #TitreAAffecter where '+ @champ + '= ''' + @valeurRqt+''''
    insert into #tmp EXEC (@sqlCommand)
    
    SET @sqlCommand = 'Delete from #TitreAAffecter where '+ @champ + '= ''' + @valeurRqt+ ''''
    EXEC (@sqlCommand)
    
    Delete from #TmpGrille where valeur = @valeur and champ =@champ
  END
Commit TRANSACTION
Delete from #Grille where Grille='Obl_inf' or Grille='Obl_Etat'
Delete from #tmp

--Traitement de tous les autres titres
insert into #TmpGrille select * from #Grille where Grille!='Obl_inf' and Grille!='Obl_Etat' --where Grille = 'Act_Euro'

Begin TRANSACTION
  WHILE EXISTS(SELECT TOP 1 * FROM #TmpGrille)
  BEGIN
    SET @champ = (SELECT TOP 1 champ FROM #TmpGrille)
    SET @valeur = UPPER((SELECT TOP 1 Valeur FROM #TmpGrille))
    SET @grille = (SELECT TOP 1 Grille FROM #TmpGrille)
    
  
	SET @sqlCommand = 'Select [Code_titre], ''' + @grille + ''',[Dateinventaire],1,''Auto'' from #TitreAAffecter where '+ @champ + '= ''' + @valeur+''''
	Insert into [STRAT_TITRE_GRILLE]([ISIN_titre] ,[Id_Grille] ,[Date],[Poids],[Source])  EXEC (@sqlCommand)
    
    SET @sqlCommand = 'Delete from #TitreAAffecter where '+ @champ + '= ''' + @valeur+ ''''
    EXEC (@sqlCommand)
    
    Delete from #TmpGrille where valeur = @valeur and champ =@champ
  END
Commit TRANSACTION


--Traitement des droits d'attributions
--Les droits d'attributions sont des titres actions, il faut alors rechercher à quelle zone Géographique ils appartiennent
insert into #TmpGrille select * from #Grille where Grille='Act_ExEuro' or Grille='Act_Euro' or Grille='Act_Emerg' or Grille='Act_US' or Grille='Act_Asie' --where Grille = 'Act_Euro'

Begin TRANSACTION
  WHILE EXISTS(SELECT TOP 1 * FROM #TmpGrille)
  BEGIN
    SET @champ = (SELECT TOP 1 champ FROM #TmpGrille)
    SET @valeur = UPPER((SELECT TOP 1 Valeur FROM #TmpGrille))
    SET @grille = (SELECT TOP 1 Grille FROM #TmpGrille)
    
	SET @sqlCommand = 'Select tit.code_titre,'''+ @grille+ ''', Dateinventaire, 1, ''Auto'' from #TitreAAffecter tit inner join ref.country p on Substring(Code_titre,1,2) = p.iso2 inner join #tmpGrille on '''+ @valeur+''' = ''Actions '' + french where (tit.Type_produit = ''Droits d''''attribution'' or tit.Type_produit = ''Actions'' or tit.Type_produit = ''Action'' ) and valeur = '''+@valeur+''''
	Insert into [STRAT_TITRE_GRILLE]([ISIN_titre] ,[Id_Grille] ,[Date],[Poids],[Source])  EXEC (@sqlCommand)
 
    SET @sqlCommand = 'Select tit.code_titre from #TitreAAffecter tit inner join ref.country p on Substring(Code_titre,1,2) = p.iso2 inner join #tmpGrille on '''+ @valeur+''' = ''Actions '' + french where (tit.Type_produit = ''Droits d''''attribution'' or tit.Type_produit = ''Actions'' or tit.Type_produit = ''Action'' ) and valeur = '''+@valeur+''''
    insert into #tmp ([Code_titre]) EXEC (@sqlCommand)
   
    Delete from #TmpGrille where valeur = @valeur and champ =@champ
    
  END
  /*Delete from #TitreAAffecter where Type_produit ='Droits d''attribution' or Type_produit = 'Actions' or Type_produit = 'Action' */
  Delete from #TitreAAffecter where code_titre in (select code_titre from #tmp)
Commit TRANSACTION

-- A voir problème dans le premier tour sur l'affectation des titres 'actions' or 'action'
-- changer table pays par ref.country
-- gérer update titre proxy

Drop table #TitreAAffecter 
Drop table #Grille
Drop table #TmpGrille
Drop table #tmpTitre 
drop table #tmpproxy
drop table #UpdateTitre
Drop table #TMP
Drop table #MAJProxy

END
