
--select * from dbo.TX_YTM_Indicateurs('20/10/2010','20/12/2014', 'FR0010670737',NULL,'BARCLAYS')


ALTER FUNCTION dbo.TX_YTM_Indicateurs(@dateBegin date, @dateEnd date, @id1 [nvarchar](20), @id2 [nvarchar](20),@pricingSourceType [varchar](20) )
RETURNS @Listing TABLE 
(
	[date] date NOT NULL,
	id1 [nvarchar](20) NOT NULL,
	id2 [nvarchar](20) NULL,
	pricingSourceType [nvarchar](20) NOT NULL,
	YTM_close float,
	MobileAvg3M float,
	MobileAvg6M float,
	HistAvg float,
	PeriodAvg float, -- sur la periode @dateBegin @dateEnd
	MobileVol3M  float, -- Sur un echantillonnage de 3 Mois
	MobileVol6M  float, -- Sur un echantillonnage de 6 Mois
	PeriodVol float,
	MobileZscore3M  float,
	MobileZscore6M  float,
	Zscore float,
	YTM_max float,
	YTM_min float,
	YTM_max5pc float,
	YTM_max5pcDate datetime,
	YTM_min5pc float,
	YTM_min5pcDate datetime
 )
 AS
BEGIN

	
----------------------------------------------------------------
-- DEBUG
----------------------------------------------------------------
/*
declare @Listing TABLE ( [date] date NOT NULL,
	id1 [nvarchar](20) NOT NULL,
	id2 [nvarchar](20) NULL,
	pricingSourceType [nvarchar](20) NOT NULL,
	YTM_close float,
	MobileAvg3M float,
	MobileAvg6M float,
	HistAvg float,
	PeriodAvg float, -- sur la periode @dateBegin @dateEnd
	MobileVol3M  float, -- Sur un echantillonnage de 3 Mois
	MobileVol6M  float, -- Sur un echantillonnage de 6 Mois
	PeriodVol float,
	MobileZscore3M  float,
	MobileZscore6M  float,
	Zscore float,
	YTM_max float,
	YTM_min float,
	YTM_max5pc float,
	YTM_max5pcDate datetime,
	YTM_min5pc float,
	YTM_min5pcDate datetime ) 
declare @dateBegin date  SET @dateBegin = '20/10/2010' 
declare @dateEnd date  SET @dateEnd = '22/12/2014' 
declare	@id1 [nvarchar](20)
declare @id2 [nvarchar](20)
declare @pricingSourceType [varchar](20)

-- Liste des OAT
	--SELECT a.*
	--from ref_security.ASSET as a 
	--LEFT OUTER JOIN ref_issuer.ROLE as r on r.AssetId = a.Id
	--WHERE r.IssuerName = 'FRANCE'


set @id1='FR0010670737'
set @id2=NULL
SET @pricingSourceType='BARCLAYS'
--SET @pricingSourceType = 'IBOXX_EUR'

*/


declare @key [char](5)
set @key='YTM_D'

IF (@id2 is NULL)
BEGIN

DECLARE @timeDepthInMonths int
SET @timeDepthInMonths = 12

declare @YTM_close float
declare @MobileAvg3M float
declare @MobileAvg6M float
declare @HistAvg float
declare @PeriodAvg float
declare @MobileVol3M float
declare @MobileVol6M float
declare @PeriodVol float
declare @MobileZscore3M  float
declare @MobileZscore6M  float
declare @Zscore float
declare @YTM_max float
declare @YTM_min float
declare @YTM_max5pc float
declare @YTM_max5pcDate datetime
declare @YTM_min5pc float
declare @YTM_min5pcDate datetime
	
/* MOYENNE HISTORIQUE */
SET @timeDepthInMonths = 12 * 10
set @HistAvg = (select AVG(Value) from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between DATEADD( mm, -@timeDepthInMonths ,@dateEnd) and @dateEnd)
/* Moyenne sur la periode */
set @PeriodAvg = (select AVG(Value) from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between @dateBegin and @dateEnd )
set @PeriodVol= (select STDEVP(Value)*SQRT(252) from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between @dateBegin and @dateEnd )
/* min et max */
set @YTM_max=(select Max(Value) from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between @dateBegin and @dateEnd )
set @YTM_min=(select Min(Value) from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between @dateBegin and @dateEnd )

declare @5percentcandidates Table( date datetime,value float )

insert into @5percentcandidates
select top 5 percent Date,Value 
from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between @dateBegin and @dateEnd order by Value
set @YTM_min5pc = (select top 1 Value from @5percentcandidates order by Value desc)
set @YTM_min5pcDate = (select top 1 Date from @5percentcandidates order by Value desc)
delete from @5percentcandidates

insert into @5percentcandidates
select top 5 percent Date,Value 
from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between @dateBegin and @dateEnd order by Value desc
set @YTM_max5pc = (select top 1 Value from @5percentcandidates order by Value)
set @YTM_max5pcDate = (select top 1 Date from @5percentcandidates order by Value)
/*** Fin valeurs communes*/
-- boucle pour chaque date

declare @date datetime set @date = @dateBegin 
WHILE (@date < @dateEnd)  
BEGIN
IF( DATEPART(dw,@date) not in (6,7) )
BEGIN

set @YTM_close = (select top 1 Value from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] =@date)



If( @YTM_close is NULL)
BEGIN
insert into @Listing (Date,ID1,ID2,pricingSourceType) VALUES (@date,@id1,@id2,@pricingSourceType)
END
ELSE 
BEGIN
/* MOYENNE MOBILE SIMPLE 3MOIS*/
SET @timeDepthInMonths = 3
set @MobileAvg3M = (select AVG(Value) from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date )
set @MobileVol3M= (select STDEVP(Value)*SQRT(252) from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date )

SET @timeDepthInMonths = 6
set @MobileAvg6M = (select AVG(Value) from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date )
set @MobileVol6M= (select STDEVP(Value)*SQRT(252) from TX_AGGREGATE_DATA where key1 = @key and key2= @id1 and key5 = @pricingSourceType and [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date )

-- ZSCORE: X-PeriodAvg/ PeriodVol/SQRT 252
if( @PeriodVol is null OR @PeriodVol =0)
BEGIN
set @Zscore = NULL
END
ELSE
BEGIN
set @Zscore = (select  @YTM_close - @PeriodAvg)/ @PeriodVol / SQRT(252) 
END
if( @MobileVol3M is null OR @MobileVol3M =0)
BEGIN
set @MobileZscore3M = NULL
END
ELSE
BEGIN
set @MobileZscore3M = (select  @YTM_close - @MobileAvg3M)/ @MobileVol3M / SQRT(252) 
END
if( @MobileVol6M is null  OR @MobileVol6M =0)
BEGIN
set @MobileZscore6M = NULL
END
ELSE
BEGIN
set @MobileZscore6M = (select  @YTM_close - @MobileAvg6M)/ @MobileVol6M / SQRT(252) 
END

insert into @Listing
select @date,@id1,@id2,@pricingSourceType, @YTM_close, @MobileAvg3M,@MobileAvg6M, @HistAvg,@PeriodAvg, @MobileVol3M ,@MobileVol6M,@PeriodVol,
@MobileZscore3M,@MobileZscore6M,@Zscore,@YTM_max,@YTM_min,
@YTM_max5pc,@YTM_max5pcDate,@YTM_min5pc,@YTM_min5pcDate

END -- FIN IF 1 
END -- FIN IF 2
		set @date=DATEADD(d,1,@date)
END -- FIN WHILE

END




RETURN;

END 