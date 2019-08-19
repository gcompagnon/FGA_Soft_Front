USE [E1DBFGA01]
GO

/****** Object:  StoredProcedure [dbo].[ReportCompteRating]    Script Date: 09/18/2012 10:05:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





 
/** configuration du rapport SupervisionGroupe:
Les regroupements/groupes des comptes clients sont déduits des positions Omega 
presentes dans les inventaires en transparence

Pour chaque groupe, des rubriques définies par libellé sont fournies avec leur valeur.(NULL pour une rubrique non définies pour un groupe) 

Chaque groupe possède un ordre prédéfini pour l'affichage (cet ordre est unique pour un groupe donné)
Dans un groupe donné, la rubrique possède un ordre en priorité de l'ordre naturel (alphabétique)

Chaque rapport est identifié par une clé et une date (se rapportant à l'inventaire) ce qui garantie que les données sont cohérentes entre elles suivant ces 2 valeurs.
**/  

--USE FGA_DEV
-- avec l'utilisateur fga 
--GO
-- execute as fga
-- DROP PROCEDURE ReportMonitoringGroupe


ALTER PROCEDURE [dbo].[ReportCompteRating] 
        @dateInventaire  datetime ,
        @niveauInventaire tinyint ,
        @cleReport char(20) OUTPUT,
        @comptes varchar(500)
AS

/** Parametres: --------------------------------------**/
/*
declare @dateInventaire as datetime
set  @dateInventaire = '31/01/2012'
declare @niveauInventaire as tinyint
set @niveauInventaire = 3
declare @cleReport as char(20)
*/
IF @cleReport IS NULL
BEGIN
  set @cleReport = 'CompteRating'
END

/** Parametres: ---FIN--------------------------------**/

-- test d existence de la date demandée pour l'inventaire
/** TODO **/
--select distinct DateInventaire from  [PTF_TRANSPARISE] order by DateInventaire
-- select distinct Groupe from  [PTF_TRANSPARISE] where DateInventaire = '2011-08-24'

-- test d existence du niveau d inventaire
/** TODO **/

-- suppression pour la date demandée mais on pourra archiver sur une autre table ces rapports 
delete from [PTF_RAPPORT]  
   where date = @dateInventaire 
   and cle = @cleReport

/**CREATION D'UNE TABLE CONTENANT LES COMPTES ET LEUR NUMERO D'ORDRE******************/
declare @ordre tinyint
set @ordre=1

CREATE TABLE #comptes (compte varchar(25) COLLATE DATABASE_DEFAULT primary key , ordreGroupe smallint null)

DECLARE @OrderID varchar(25), @Pos int
DECLARE @comptesBis varchar(500)
SET @comptesBis=@comptes

SET @comptes = LTRIM(RTRIM(@comptes))+ ','
SET @Pos = CHARINDEX(',', @comptes, 1)

IF REPLACE(@comptes, ',', '') <> ''
BEGIN
	WHILE @Pos > 0
	BEGIN
		SET @OrderID = LTRIM(RTRIM(LEFT(@comptes, @Pos - 1)))
		IF @OrderID <> ''
		BEGIN
			INSERT INTO #comptes (compte,ordreGroupe) VALUES (@OrderID,@ordre) --Use Appropriate conversion
			set @ordre=@ordre+1
		END
		SET @comptes = RIGHT(@comptes, LEN(@comptes) - @Pos)
		SET @Pos = CHARINDEX(',', @comptes, 1)

	END
END

/**************************************************************************/

-- une table rassemblant tous les secteurs de type Obligataire 
    select libelle 
    into #secteurObligations
    from [SECTEUR] 
    where id like 'O CNF%' 
       or id like 'O CF%' 
       or id= 'F OBLIG' 
       or id =  'O GVT' 
       or id = 'O AGENCIES'
       or id = 'O COVERED'


create table #rating (ordre tinyint, rating varchar(5) COLLATE DATABASE_DEFAULT)
insert into #rating values (1,'AAA')
insert into #rating values (2,'AA+')
insert into #rating values (3,'AA')
insert into #rating values (4,'AA-')
insert into #rating values (5,'A+')
insert into #rating values (6,'A')
insert into #rating values (7,'A-')
insert into #rating values (8,'BBB+')
insert into #rating values (9,'BBB')
insert into #rating values (10,'BBB-')
insert into #rating values (11,'BB+')
insert into #rating values (12,'BB')
insert into #rating values (13,'BB-')
insert into #rating values (14,'B+')
insert into #rating values (15,'B')
insert into #rating values (16,'B-')
insert into #rating values (17,'CCC+')
insert into #rating values (18,'CCC')
insert into #rating values (19,'CCC-')
insert into #rating values (20,'CC')
insert into #rating values (21,'C')
insert into #rating values (22,'D')
insert into #rating values (23,'CD')
insert into #rating values (24,'NR')

select * into #groupe_rating
from #Comptes, #rating



create table #res ( cle char(20) COLLATE DATABASE_DEFAULT, compte varchar(25) COLLATE DATABASE_DEFAULT, ordreGroupe tinyint, dateInventaire dateTime,  rubrique varchar(25) COLLATE DATABASE_DEFAULT, libelle varchar(25) COLLATE DATABASE_DEFAULT, classementRubrique tinyint, valeur float)
insert into #res
Select @cleReport as cle,
		gr.compte,
		gr.ordreGroupe,
		@dateinventaire as dateInventaire,
		gr.rating as rubrique,
		gr.rating as libelle,
		gr.ordre as classementrubrique,
		SUM(tr.valeur_boursiere+coupon_couru) as valeur
	FROM #groupe_rating as gr
	left outer join [dbo].[PTF_TRANSPARISE] as tr 
	on tr.compte = gr.compte
	and tr.Dateinventaire = @dateInventaire
	and tr.Numero_Niveau = @niveauInventaire  
	and tr.rating=gr.rating
	and tr.Secteur in (select libelle from #secteurObligations )
	GROUP BY gr.rating, gr.compte, gr.ordreGroupe, gr.ordre
	order by ordregroupe, classementRubrique
	
	
/**Calcul des valeurs par COMPTE**/		
insert into #res 
SELECT @cleReport as cle,
		r.compte,
		r.ordreGroupe,
		@dateInventaire as dateInventaire,
		'ENCOURS - Total' as rubrique,
		'ENCOURS - Total' as libelle,
		0 as classementRubrique,
		SUM(r.valeur) as valeur
	FROM #res as r
	GROUP BY r.compte, r.ordreGroupe	
	
/**Calcul des valeurs par RATING**/		
insert into #res 
SELECT @cleReport as cle,
		@comptesBis as compte,
		0 as ordreGroupe,
		@dateInventaire as dateInventaire,
		r.libelle as rubrique,
		r.libelle as libelle,
		r.classementRubrique as classementRubrique,
		SUM(r.valeur) as valeur
	FROM #res as r
	GROUP BY r.libelle, r.classementRubrique

	
	
INSERT INTO ptf_RAPPORT
Select cle,
libelle,
valeur,
 compte as groupe, 
 dateInventaire as date, 
 ordreGroupe,
  classementRubrique, 
  rubrique
from #res
	
drop table #rating
drop table #comptes
drop table #secteurObligations
drop table #groupe_rating
drop table #res







GO

