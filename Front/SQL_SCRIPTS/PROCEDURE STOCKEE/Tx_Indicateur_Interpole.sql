USE [E0DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[Indicateur_Interpole]    Script Date: 12/10/2014 18:18:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---- ==============================================================
---- Author:		<Najla Amdouni alias Framboise>
---- Create date: <09/30/2014>
---- Description:	<Indicateur pour un taux interpolé>
---- ==============================================================



ALTER PROCEDURE [dbo].[Indicateur_Interpole]
	(@InputDate As DATETIME,
	@InputDateF As datetime,
	@InputPays As CHAR(2),
	@InputMaturity As INT)
AS

BEGIN

-- Pour debug
DECLARE @InputDate datetime  SET @InputDate = '10/12/2014' 

DECLARE @pricingSourceType [varchar](20)
SET @pricingSourceType = 'BARCLAYS'
--SET @pricingSourceType = 'IBOXX_EUR'

/* MOYENNE MOBILE SIMPLE 6MOIS*/
-- Liste des valeurs 
select * from TX_AGGREGATE_DATA where key1 = 'YTM_I' and key5 = @pricingSourceType and Date between DATEADD( mm, -6 ,@InputDate) and @InputDate





select distinct key2,key3,key4, 
AVG(Value) OVER ( PARTITION BY  key2,key3,key4 ) 
from TX_AGGREGATE_DATA where key1 = 'YTM_I' and key5 = @pricingSourceType and Date between DATEADD( mm, -6 ,@InputDate) and @InputDate




select AVG( ALL value ) 

select distinct Date,key5 from TX_AGGREGATE_DATA order by Date





/* Insert dans une table qui est dans la base du résultat de ma procedure sur le taux interpolé */

DROP TABLE Test

CREATE TABLE Test
(
	Date DATETIME,
	Maturity INT,
	Rate FLOAT
)

Insert into Test
EXECUTE TX_InterpObligation @InputDate, @InputDateF, @InputPays, @InputMaturity 


/* MOYENNE MOBILE SIMPLE */
DECLARE @rownumber INT = 1;
DECLARE @rownumber1 INT = 1;
DECLARE @NUMlig int;

/* Pour avoir le nombre de ligne de la table */ 
set @NUMlig = (select COUNT(distinct Date) from Test) 

/**TABLE TEMPORAIRE POUR LA CONSOLIDATION DES DONNEES **/ 
DECLARE @moy TABLE(
    [Date] [date],
	[Valeur] [float]
)


SELECT distinct f.Date,
			f.Rate, 
			row_Number() OVER (ORDER BY date DESC) AS rnum
			into #testmoy
			FROM Test f
			where Date BETWEEN @InputDate and @InputDateF 
			order by Date
/**BOUCLE POUR LE CALCUL DES MOYENNES MOBILES **/
WHILE (@rownumber < @NUMlig)   /*LE NOMBRE DE LIGNE 665 = @NUMlig*/

BEGIN



INSERT INTO @moy SELECT
max(Date) ,
(SELECT AVG(Rate) FROM #testmoy WHERE rnum BETWEEN @rownumber AND @rownumber + 59) AS Valeur
FROM #testmoy
WHERE rnum BETWEEN @rownumber AND @rownumber + 59 /*3 mois = 60jrs */ 


/*INCREMENTATION DE LA LIGNE DE DEPART POUR CHAQUE ENSEMBLE DE DONNEES*/
SET @rownumber = @rownumber + 1;

END


--Moyenne Historique 
SELECT @InputDateF as Date,
		AVG(f.Rate) as MoyH
INTO #MH
FROM Test f
ORDER BY Date DESC


--Moyenne 
SELECT @InputDateF as Date, AVG(Rate) as Valeur
into #moyenne 
from Test
where Date Between @InputDate and @InputDateF
order by Date DESC


--ECART TYPE 
SELECT date, 
	(SELECT STDEVP(Rate) FROM Test WHERE Date BETWEEN @InputDate and @InputDateF ) as Volality 
INTO #VOLALITY
FROM Test 
WHERE Date BETWEEN @InputDate and @InputDateF 
ORDER  BY Date DESC

--L'ECART TYPE DE 3 MOIS GLISSANT
/**TABLE TEMPORAIRE POUR LA CONSOLIDATION DES DONNEES**/ 
DECLARE @VOLB TABLE(
    [Date] [date],
	[VOL] [float]
)
SELECT f.Date,
			f.Rate
			,row_Number() OVER (ORDER BY Date DESC) AS rnum
			into #testVol
			FROM Test f
			WHERE Date BETWEEN @InputDate and @InputDateF
			
/**BOUCLE POUR LE CALCUL de L'ECART TYPE  **/
WHILE (@rownumber1 < @NUMlig)   /*LE NOMBRE DE LIGNE 665 = @NUMlig*/

BEGIN

INSERT INTO @VOLB SELECT
max(Date) As Date,
(SELECT STDEV(Rate) FROM Test) AS VOL
FROM #testvol
WHERE rnum BETWEEN @rownumber1 AND @rownumber1 + 59 /*3 mois */ 

/*INCREMENTATION DE LA LIGNE DE DEPART POUR CHAQUE ENSEMBLE DE DONNEES*/
SET @rownumber1 = @rownumber1 + 1;

END




 --ZSCORE 
 SELECT B.Date,
	((B.Rate - A.MoyH) / C.volality) as Zscore
	
INTO #ZSCORE
FROM Test as B 
INNER JOIN #MH as A on A.Date = B.Date
INNER JOIN #Volality as C on C.Date = B.Date
WHERE B.Date BETWEEN @InputDate and @InputDateF
ORDER BY Date DESC 




--ZSCORE 3 MOIS GLISSANT 
SELECT B.Date,
	((B.Rate - A.Valeur) / C.VOL) as Zscoremob	
INTO #ZSCOREMOB
FROM Test as B 
INNER JOIN @moy as A on A.Date = B.Date
INNER JOIN @VOLB as C on C.Date = B.Date
--WHERE B.Date BETWEEN @InputDate and @InputDateF
ORDER BY B.Date DESC 
--select * from Test
--select * from @moy
--select * from @VOLB
--select * from #ZSCOREMOB


 --MIN 5%
select 	
		f.Date,
		f.Rate,
		row_Number() OVER (ORDER BY Rate ) AS rnum
into #mintest
from Test f
where Date BETWEEN @InputDate and @InputDateF 

 -- select * from #test 
(select top 5 percent *
into #mintest2 
from #mintest)


--Max 5%
select 
		f.Date,
		f.Rate,
		row_Number() OVER (ORDER BY Rate DESC) AS rnum
into #maxtest
from Test f
where f.Date BETWEEN @InputDate and @InputDateF 

select top 5 percent * into #maxtest2 from #maxtest 


DECLARE @result TABLE(
	[Indicateur] [nchar](32),
    [Date] [date],
	[Valeur] [float]
)

INSERT INTO @result
SELECT 'Closing Price' as Indicateur , dte as Date,  round(clo,3) as Valeur FROM KIM WHERE dte = @InputDateF

INSERT INTO @result
SELECT 'Moyenne Mobile' as Indicateur, Date,  round(Valeur,2) FROM @moy WHERE DATE = @InputDateF

insert into @result
select 'Moyenne' as Indicateur , Date, ROUND(Valeur,3) from #moyenne where Date = @InputDateF

INSERT INTO @result
SELECT 'Moyenne Historique' as Indicateur, Date,  round(MoyH,2) as Valeur FROM #MH WHERE DATE = @InputDateF

INSERT INTO @result
SELECT 'Ecart Type' as Indicateur, Date, round(Volality,2) FROM #VOLALITY WHERE DATE = @InputDateF

INSERT INTO @result
SELECT 'Ecart Type Mobile' as Indicateur, Date,  round(VOL,2) as Valeur FROM @VOLB WHERE DATE = @InputDateF

INSERT INTO @result
SELECT 'ZScore' as Indicateur, Date,  round(Zscore,2) as Valeur FROM #ZSCORE WHERE DATE = @InputDateF

INSERT INTO @result
SELECT 'ZScore Mobile' as Indicateur, Date,  ROUND(Zscoremob,2) FROM #ZSCOREMOB WHERE DATE = @InputDateF

INSERT INTO @result
select top(1) 'Max' as Indicateur,  Date, ROUND(Rate,2) as Valeur from #maxtest 

INSERT INTO @result
SELECT top(1) 'Max 5%' as Indicateur , Date, round(Rate,2) as Valeur from #maxtest2 order by Rate

INSERT INTO @result
select top(1) 'Min' as Indicateur, Date, round(Rate,2) as Valeur from #mintest

INSERT INTO @result
SELECT  top(1)  'Min 5%' as Indicateur,  Date, round(Rate,2) as Valeur from #mintest2 order by Rate DESC



SELECT  * FROM @result


--Drop Table 
drop table #MH
drop table #VOLALITY
drop table #ZSCORE
drop table #ZSCOREMOB
--drop table #Max
--drop table #Min
drop table #mintest
drop table #mintest2
drop table #maxtest
drop table #maxtest2
drop table #testmoy
drop table #testVol
drop table #moyenne
END



----TEST
--DECLARE @InputPays  CHAR(2) set @InputPays = 'BE'
--DECLARE @InputMaturity int set @InputMaturity = 2
--DECLARE @InputDate  VARCHAR(150)  SET @InputDate = '03/09/2012' 
--DECLARE @InputDateF VARCHAR(150) SET @InputDateF = '30/11/2012'
--EXECUTE Indicateur_Interpole @InputDate, @InputDateF, @InputPays, @InputMaturity
