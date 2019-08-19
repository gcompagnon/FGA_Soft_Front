USE [E0DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[Indicateurs_Tableau]    Script Date: 12/10/2014 18:23:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---- ====================================================================================
---- Author:		<Najla Amdouni>
---- Create date: <19/11/2014>
---- Description:	<Calcule du zscore etc.. pour tableau>
---- =====================================================================================

ALTER  PROCEDURE [dbo].[Indicateurs_Tableau]
	(@Isin As Varchar(12),
	@Date As Datetime)

AS

BEGIN

--MOYENNE MOBILE SIMPLE 

DECLARE @rownumber INT = 1;
DECLARE @rownumber1 INT = 1;
DECLARE @NUMlig int;
-- Pour avoir le nombre de ligne de la table 
set @NUMlig = (SELECT COUNT(*) from COTATION1)

/**TABLE TEMPORAIRE POUR LA CONSOLIDATION DES DONNEES**/ 
DECLARE @moy TABLE(
    [Date] [date],
	[Valeur] [float],
	[Indicateur] [char]
)
SELECT f.dte,
			f.clo
			,row_Number() OVER (PARTITION BY isin ORDER BY dte DESC) AS rnum
			into #testmoy
			FROM COTATION1 f
			where isin =  'AT0000383864' 
/**BOUCLE POUR LE CALCUL DES MOYENNES MOBILES **/
WHILE (@rownumber < @NUMlig)   /*LE NOMBRE DE LIGNE 665 = @NUMlig*/

BEGIN

INSERT INTO @moy SELECT
max(a.dte) As Date,AVG(clo)AS Valeur, 'Moyenne Mobile' As Indicateur
FROM ( SELECT f.dte,
			f.clo
			,row_Number() OVER (PARTITION BY isin ORDER BY dte DESC) AS rnum
	   FROM COTATION1 f 
	   where isin =  'AT0000383864'
	 ) AS a
WHERE rnum BETWEEN @rownumber AND @rownumber + 59 /*3 mois = 60jrs */ 

/*INCREMENTATION DE LA LIGNE DE DEPART POUR CHAQUE ENSEMBLE DE DONNEES*/
SET @rownumber = @rownumber + 1;

END

SELECT * from @moy 



--Moyenne Historique 
SELECT  dte as Date,
		AVG(clo) over (partition by isin ) as MoyH,
		'Moyenne historique' as Indicateur
		
INTO #MH
FROM COTATION1
WHERE isin = 'AT0000383864' and dte = '17/10/2014'
ORDER BY Date DESC


select * from #MH order by Date DESC



 --ECART TYPE 
SELECT dte as Date,
		STDEVP(clo) over (partition by isin ) as Volality,
		'Volality' as Indicateur 
INTO #VOLALITY
FROM COTATION1
WHERE isin = 'AT0000383864' and dte = '17/10/2014'
ORDER  BY Date DESC

select * from #VOLALITY order by Date DESC;



--L'ECART TYPE DE 3 MOIS GLISSANT
/**TABLE TEMPORAIRE POUR LA CONSOLIDATION DES DONNEES**/ 
DECLARE @VOLB TABLE(
    [Date] [date],
	[VOL] [float]
)
SELECT f.dte,
			f.clo
			,row_Number() OVER (PARTITION BY isin ORDER BY dte DESC) AS rnum
			into #testVol
			FROM COTATION1 f
			where isin = 'AT0000383864'
/**BOUCLE POUR LE CALCUL de L'ECART TYPE  **/
WHILE (@rownumber1 < @NUMlig)   /*LE NOMBRE DE LIGNE 665 = @NUMlig*/

BEGIN

INSERT INTO @VOLB SELECT
max(a.dte) As Date,STDEV(clo)AS VOL
FROM ( SELECT f.dte,
			f.clo
			,row_Number() OVER (PARTITION BY isin ORDER BY dte DESC) AS rnum
	   FROM COTATION1 f 
	   where isin =  'AT0000383864'
	 ) AS a
WHERE rnum BETWEEN @rownumber1 AND @rownumber1 + 59 /*3 mois */ 

/*INCREMENTATION DE LA LIGNE DE DEPART POUR CHAQUE ENSEMBLE DE DONNEES*/
SET @rownumber1 = @rownumber1 + 1;

END

select * from @VOLB


 --ZSCORE 
 SELECT dte as Date,
	((B.clo - A.MoyH) / C.volality) as Zscore,
	'Zscore' as Indicateur
INTO #ZSCORE
FROM COTATION1 as B 
INNER JOIN #MH as A on A.Date = B.dte 
INNER JOIN #Volality as C on C.Date = B.dte
WHERE B.isin = 'AT0000383864' and dte = '17/10/2014'
ORDER BY Date DESC 
select * from #ZSCORE order by Date DESC


--ZSCORE 3 MOIS GLISSANT 
SELECT dte as Date,
	((B.clo - A.Valeur) / C.VOL) as Zscoremob,
	'Zscore Mobile' as Indicateur
INTO #ZSCOREMOB
FROM COTATION1 as B 
INNER JOIN @moy as A on A.Date = B.dte 
INNER JOIN @VOLB as C on C.Date = B.dte
WHERE B.isin = 'AT0000383864' and dte = '17/10/2014'
ORDER BY Date DESC 

select * from #ZSCOREMOB order by Date DESC


--MAX
SELECT dte as Date,
		MAX(clo) over (partition by isin ) as Valeur,
		'Max' as Indicateur
INTO #Max
FROM COTATION1
WHERE isin = 'AT0000383864' and dte = '17/10/2014'
ORDER  BY Date DESC

select * from #Max 


--MIN
SELECT dte as Date,
	MIN(clo) over (partition by isin ) as Valeur,
	'Min' as Indicateur
INTO #Min
FROM COTATION1
WHERE isin = 'AT0000383864' and dte = '17/10/2014'
ORDER BY Date DESC

select * from #Min 

 --MIN 5%
select 	
		f.isin,
		f.dte,
		f.clo,
		row_Number() OVER (PARTITION BY isin ORDER BY clo ) AS rnum
into #mintest
from COTATION1 f
where isin = 'AT0000383864'

 -- select * from #test 
(select top 5 percent *
into #mintest2 
from #mintest)

select top(1) isin, dte as Date, clo as Valeur, 'Min 5%' as Indicateur  from #mintest2 order by clo DESC



--Max 5%
select 	
		f.isin,
		f.dte,
		f.clo,
		row_Number() OVER (PARTITION BY isin ORDER BY clo DESC) AS rnum
into #maxtest
from COTATION1 f
where isin = 'AT0000383864'

select top 5 percent * into #maxtest2 from #maxtest 

select  top (1) isin, dte as Date, clo as Valeur, 'Max 5%' as Indicateur from #maxtest2 order by clo




--Drop Table 
drop table #MH
drop table #VOLALITY
drop table #ZSCORE
drop table #ZSCOREMOB
drop table #Max
drop table #Min
drop table #mintest
drop table #mintest2
drop table #maxtest
drop table #maxtest2
drop table #testmoy
drop table #testVol

END



----TEST
--DECLARE @Isin Varchar(12) set @Isin = 'AT0000383864'
--DECLARE @Date Datetime set @Date = '17/10/2014'
--EXECUTE Indicateurs_Tableau @Isin, @Date


