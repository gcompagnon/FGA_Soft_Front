USE [E0DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[Indicateurs]    Script Date: 12/10/2014 18:22:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---- ====================================================================================
---- Calcule du zscore, volatilité, moyenne mobile, moyenne historique ...
---- pour une date. et sur le YTM de :
----  @input1 : ISIN et @input2 : NULL
----  @input1 : Pays et @input2 : maturité cible : 1 à 10 15 20 25 30
----  @pricingSourceType : pricing source : BARCLAYS ou IBOXX_EUR
----  retourne une ligne avec les calculs
---- =====================================================================================





CREATE FUNCTION dbo.TX_Indicateur (@dateInput datetime, @input1 [nvarchar](20), @input2 [nvarchar](20),@pricingSourceType [varchar](20) )
RETURNS @Listing TABLE 
(
	[date] datetime NOT NULL,
	id1 [nvarchar](20) NOT NULL,
	id2 [nvarchar](20) NULL,
	pricingSourceType [nvarchar](20) NOT NULL,
	YTM_close float,
	YTM_MobileAv_6M float,
	YTM_Vol3M  float, -- Sur un echantillonnage de 
	YTM_Vol12M  float,
	
 )
AS
BEGIN
-----------------------------------------------------------
-----------------------------------------------------------
-- Pour debug
DECLARE @InputDate datetime  SET @InputDate = '10/12/2014' 

DECLARE @pricingSourceType [varchar](20)
SET @pricingSourceType = 'BARCLAYS'
--SET @pricingSourceType = 'IBOXX_EUR'

DECLARE @timeDepthInMonths int
SET @timeDepthInMonths = 12

/* MOYENNE MOBILE SIMPLE 12MOIS*/
SET @timeDepthInMonths = 6
select  distinct key2,key3,key4, 
AVG(Value) OVER ( PARTITION BY  key2,key3,key4 ) 
from TX_AGGREGATE_DATA where key1 = 'YTM_I' and key5 = @pricingSourceType and Date between DATEADD( mm, -@timeDepthInMonths ,@InputDate) and @InputDate

/* MOYENNE MOBILE SIMPLE 12MOIS*/
SET @timeDepthInMonths = 12
select distinct key2,key3,key4, 
AVG(Value) OVER ( PARTITION BY  key2,key3,key4 ) 
from TX_AGGREGATE_DATA where key1 = 'YTM_I' and key5 = @pricingSourceType and Date between DATEADD( mm, -@timeDepthInMonths ,@InputDate) and @InputDate

/* MOYENNE HISTORIQUE */
SET @timeDepthInMonths = 12 * 10
select distinct key2,key3,key4, 
AVG(Value) OVER ( PARTITION BY  key2,key3,key4 ) 
from TX_AGGREGATE_DATA where key1 = 'YTM_I' and key5 = @pricingSourceType and Date between DATEADD( mm, -@timeDepthInMonths ,@InputDate) and @InputDate



-- VOLATILITE HISTORIQUE (Standard Deviation / sqrt ( durée) ) 

-- VOLATILITE 
----------------- TEST -------------------------------------------
create table #TABLE1 ( ytm tinyint )

insert into #TABLE1 VALUES( 2)
insert into #TABLE1 VALUES( 4)
insert into #TABLE1 VALUES( 4)
insert into #TABLE1 VALUES( 4)
insert into #TABLE1 VALUES( 5)
insert into #TABLE1 VALUES( 5)
insert into #TABLE1 VALUES( 7)
insert into #TABLE1 VALUES( 9)

select AVG(ytm) from #TABLE1 
select STDEV(ALL ytm) from #TABLE1  -- sample based
select STDEVP(ytm) from #TABLE1 -- complete standard dev / population based
--------------------------------------------------------------------
--Z-Score / standard score





 --MOYENNE MOBILE SIMPLE 

DECLARE @rownumber INT = 1;
DECLARE @rownumber1 INT = 1;
DECLARE @NUMlig int;
-- Pour avoir le nombre de ligne de la table 
set @NUMlig = (SELECT COUNT(*) from COTATION1)

/**TABLE TEMPORAIRE POUR LA CONSOLIDATION DES DONNEES**/ 
DECLARE @moy TABLE(
    [Date] [date],
	[Valeur] [float]
)
SELECT f.dte,
			f.clo
			,row_Number() OVER (PARTITION BY isin ORDER BY dte DESC) AS rnum
			into #testmoy
			FROM COTATION1 f
			where isin = @Isin  and dte BETWEEN @DateD and @DateF 
/**BOUCLE POUR LE CALCUL DES MOYENNES MOBILES **/
WHILE (@rownumber < @NUMlig)   /*LE NOMBRE DE LIGNE 665 = @NUMlig*/

BEGIN

INSERT INTO @moy SELECT
max(dte) As Date, AVG(clo) AS Valeur
from #testmoy 
WHERE rnum BETWEEN @rownumber AND @rownumber + 59 /*3 mois = 60jrs */ 

/*INCREMENTATION DE LA LIGNE DE DEPART POUR CHAQUE ENSEMBLE DE DONNEES*/
SET @rownumber = @rownumber + 1;

END



--MOYENNE HISTORIQUE/STATIQUE

SELECT  dte as Date,
		AVG(clo) over (partition by isin ) as Valeur
INTO #MH
FROM COTATION1
WHERE isin = @Isin
ORDER BY Date DESC


--Moyenne 
Select dte as Date,
	AVG(clo) over (partition by isin) as Valeur
into #moyenne
from KIM
where isin = @Isin and dte Between @DateD and @DateF
order by Date DESC
 
 
 --ECART TYPE 
SELECT dte as Date,
		STDEVP(clo) over (partition by isin ) as Volality
INTO #VOLALITY
FROM COTATION1
WHERE isin = @Isin and dte BETWEEN @DateD and @DateF 
ORDER  BY Date DESC


--L'ECART TYPE DE 3 MOIS GLISSANT
/**TABLE TEMPORAIRE POUR LA CONSOLIDATION DES DONNEES**/ 

-- Pour avoir le nombre de ligne de la table 
set @NUMlig = (SELECT COUNT(*) from COTATION1)

DECLARE @VOLB TABLE(
    [Date] [date],
	[Valeur] [float]
)
SELECT f.dte,
			f.clo
			,row_Number() OVER (PARTITION BY isin ORDER BY dte DESC) AS rnum
			into #testVol
			FROM COTATION1 f
			where isin = @Isin  and dte BETWEEN @DateD and @DateF 
/**BOUCLE POUR LE CALCUL de L'ECART TYPE  **/
WHILE (@rownumber1 < @NUMlig)   /*LE NOMBRE DE LIGNE 665 = @NUMlig*/

BEGIN

INSERT INTO @VOLB SELECT
max(dte) As Date, STDEV(clo)AS Valeur
FROM #testVol
WHERE rnum BETWEEN @rownumber1 AND @rownumber1 + 59 /*3 mois */ 

/*INCREMENTATION DE LA LIGNE DE DEPART POUR CHAQUE ENSEMBLE DE DONNEES*/
SET @rownumber1 = @rownumber1 + 1;

END


 --ZSCORE 
 SELECT dte as Date,
	((B.clo - A.Valeur) / C.volality) as Zscore
INTO #ZSCORE
FROM COTATION1 as B 
INNER JOIN #MH as A on A.Date = B.dte 
INNER JOIN #Volality as C on C.Date = B.dte
WHERE B.isin = @Isin  and dte BETWEEN @DateD and @DateF 
ORDER BY Date DESC 



--ZSCORE 3 MOIS GLISSANT 
SELECT dte as Date,
	((B.clo - A.Valeur) / C.Valeur) as Valeur 
INTO #ZSCOREMOB
FROM COTATION1 as B 
INNER JOIN @moy as A on A.Date = B.dte 
INNER JOIN @VOLB as C on C.Date = B.dte
WHERE B.isin = @Isin  and dte BETWEEN @DateD and @DateF 
ORDER BY Date DESC 



--MAX
SELECT dte as Date,
		MAX(clo) over (partition by isin ) as Valeur
INTO #Max
FROM COTATION1
WHERE isin = @Isin  and dte BETWEEN @DateD and @DateF 
ORDER  BY Date DESC

--MIN
SELECT dte as Date,
	MIN(clo) over (partition by isin ) as Valeur
INTO #Min
FROM COTATION1
WHERE isin = @Isin  and dte BETWEEN @DateD and @DateF 
ORDER BY Date DESC

 --MIN 5%
select 	
		f.isin,
		f.dte,
		f.clo,
		row_Number() OVER (PARTITION BY isin ORDER BY clo ) AS rnum
into #mintest
from COTATION1 f
where isin = @Isin  and dte BETWEEN @DateD and @DateF 

(select top 5 percent * into #mintest2 from #mintest)




--Max 5%
select 	
		f.isin,
		f.dte,
		f.clo,
		row_Number() OVER (PARTITION BY isin ORDER BY clo DESC) AS rnum
into #maxtest
from COTATION1 f
where isin = @Isin  and dte BETWEEN @DateD and @DateF 

select top 5 percent * into #maxtest2 from #maxtest 



-- TABLE AVEC L'ECART TYPE DE 3 MOIS GLISSANT colonne VOL
if ( @str = 'Mobile Volatility')
	SELECT Date, ROUND(Valeur,3) as Valeur from @VOLB
	where Date is not null and Valeur is not null 
	order by Date

else if (@str = 'Volatility')
	select Date, ROUND(Volality,3) as Valeur from #VOLALITY order by Date 

-- TABLE AVEC LA MOYENNE HISTO colonne MH
else if (@str = 'Historical Average')
	select Date, ROUND(Valeur,3) as Valeur from #MH where Date BETWEEN @DateD and @DateF order by Date

-- TABLE AVEC LA MOYENNE MOBILE colonne Moy
else if (@str = 'Mobile Average')
	SELECT Date, ROUND(Valeur, 3) as Valeur from @moy
	where Date is not null and Valeur is not null 
	order by Date

else if (@str = 'Mobile Z Score') 
select Date, ROUND(Valeur,3) as Valeur from #ZSCOREMOB
where Date is not null and Valeur is not null 
 order by Date 

else if (@str = 'Z Score')
select Date, ROUND(Zscore,3)as Valeur  from #ZSCORE order by Date

else if (@str = 'Maximum')
select Date, ROUND(Valeur,3) as Valeur from #Max order by Date

else if (@str = 'Minimum')
select Date, round(Valeur,3) as Valeur from #Min order by Date

else if (@str = 'Maximum 5%')
select  top (1) dte as Date, ROUND(clo, 3) as Valeur from #maxtest2  where dte Between @DateD and @DateF order by clo, dte

else if (@str = 'Minimum 5%')
select  top(1) dte as Date, ROUND(clo, 3) as Valeur from #mintest2 order by clo Desc

else if (@str = 'Closing Price')
select dte as Date, ROUND(clo, 3) as Valeur from COTATION1 where isin = @Isin  and dte BETWEEN @DateD and @DateF order by Date

else if (@str = 'Average')
select Date, ROUND(Valeur, 3) as Valeur from #moyenne order by Date


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
drop table #moyenne
drop table #MH
END



----TEST
--DECLARE @Isin Varchar(12) set @Isin = 'AT0000383864'
--DECLARE @DateD datetime  set @DateD = '20/02/2013'
--DECLARE @DateF datetime set @DateF = '16/04/2014'
--DECLARE @str nVarchar(30) set @str = 'Maximum 5%'
--EXECUTE Indicateurs @Isin, @DateD, @DateF, @str 

