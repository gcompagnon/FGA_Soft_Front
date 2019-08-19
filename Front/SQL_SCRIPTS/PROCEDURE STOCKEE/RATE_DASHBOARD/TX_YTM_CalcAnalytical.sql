
--select * from dbo.TX_YTM_Indicateurs('20/10/2010','20/12/2014', 'FR0010670737',NULL,'BARCLAYS')

-----------------------------------------
-- Calcul des indicateurs entre 2 dates ou à une Date precise 
-----------------------------------------
ALTER PROCEDURE dbo.TX_YTM_CalcAnalytical(@date date, @id1 [nvarchar](20), @id2 [nvarchar](20),@pricingSourceType [varchar](20) )
 AS
BEGIN

CREATE TABLE #COUNTRY (cntry nvarchar(60), iso char(2))
insert #COUNTRY VALUES ('AUSTRIA','AT')
insert #COUNTRY VALUES ('BELGIUM','BE')
insert #COUNTRY VALUES ('FINLAND','FI')
insert #COUNTRY VALUES ('FRANCE','FR')
insert #COUNTRY VALUES ('GERMANY','DE')
insert #COUNTRY VALUES ('IRELAND','IE')
insert #COUNTRY VALUES ('ITALY','IT')
insert #COUNTRY VALUES ('NETHERLANDS','NL')
insert #COUNTRY VALUES ('SPAIN','ES')
insert #COUNTRY VALUES ('UNITED KINGDOM','GB')
declare @iso [char](2)
set @iso = (select iso from #COUNTRY where cntry = @id1)

if @iso is null
begin
  set @iso = @id1
  set @id1 = (select cntry from #COUNTRY where iso = @id1)
  if @id1 is null
  begin
     set @id1 = @iso
  end
end

----------------------------------------------------------------
-- DEBUG
----------------------------------------------------------------
/*
declare @date date  SET @date = '19/04/2015' 
declare	@id1 [nvarchar](20)
declare @id2 [nvarchar](20)
declare @pricingSourceType [varchar](20)

set @id1='FR0010670737'
set @id2=NULL --pour le calcul PAYS/PILIER DE MATURITE
set @id1='FRANCE'
set @id2='5'
--SET @pricingSourceType='BARCLAYS'
SET @pricingSourceType = 'IBOXX_EUR'

*/


DECLARE @timeDepthInMonths int
SET @timeDepthInMonths = 6

declare @YTM_close float

declare @MobileAvg3M float
declare @MobileAvg6M float

declare @MobileVol3M float
declare @MobileVol6M float

declare @MobileZscore3M  float
declare @MobileZscore6M  float
	
DECLARE @assetId bigint
SET @assetId = (select ID from ref_security.ASSET where ISIN = @id1)
create table #MARKETDATA_YTM ( Date datetime, Value float NULL)

IF (@id2 is NULL)
BEGIN
	insert into #MARKETDATA_YTM
	select [Date] as 'Date' , 
	case 
	when Date between '02/07/2014' and '25/07/2014' and @pricingSourceType ='IBOXX_EUR' then Debt_YTM_Rate
	when Date <= '02/10/2014' and @pricingSourceType ='IBOXX_EUR' then Debt_YTM_Rate/100
	else Debt_YTM_Rate end as 'Value'
	from ref_security.PRICE as pr
	where 
	pr.SecurityId = @assetId and
	Price_Source = @pricingSourceType
	and [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date 
END
ELSE
BEGIN
	insert into #MARKETDATA_YTM
	select [Date] as 'Date' , YTMI.Value 
	from [dbo].[TX_AGGREGATE_DATA] as YTMI
	where 
	key1 = 'YTM_I' and (key2= @id1 or key3= @id1 ) and key4=@id2 and key5=@pricingSourceType
	and [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date 
END

/* MOYENNE MOBILE SIMPLE 3MOIS*/
SET @timeDepthInMonths = 3
set @MobileAvg3M = (select AVG(Value) from #MARKETDATA_YTM where [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date )
set @MobileVol3M= (select STDEVP(Value)*SQRT(252) from #MARKETDATA_YTM where [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date )

SET @timeDepthInMonths = 6
set @MobileAvg6M = (select AVG(Value) from #MARKETDATA_YTM where [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date )
set @MobileVol6M= (select STDEVP(Value)*SQRT(252) from #MARKETDATA_YTM where [Date] between DATEADD( mm, -@timeDepthInMonths ,@date ) and @date )

-- ZSCORE: X-PeriodAvg/ PeriodVol/SQRT 252

set @YTM_close = (select top 1 Value from #MARKETDATA_YTM where [Date] =@date)

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



------------------------------------
-- ARCHIVAGE DANS TX_AGGREGATE_DATA
------------------------------------
-- key1: ISIN ou Pays
-- key2: null ou pilier de maturité
-- key3 : IBOXX_EUR ou  BARCLAYS
------------------------------------
insert into [dbo].[TX_AGGREGATE_DATA](Date,key1,key2,key3,key4,key5,Value) VALUES ( @date , 'VOL3M',@id1,@iso,@id2, @pricingSourceType,@MobileVol3M)
insert into [dbo].[TX_AGGREGATE_DATA](Date,key1,key2,key3,key4,key5,Value) VALUES ( @date , 'VOL6M',@id1,@iso,@id2, @pricingSourceType,@MobileVol6M)

insert into [dbo].[TX_AGGREGATE_DATA](Date,key1,key2,key3,key4,key5,Value) VALUES ( @date , 'AVG3M',@id1,@iso,@id2, @pricingSourceType,@MobileAvg3M)
insert into [dbo].[TX_AGGREGATE_DATA](Date,key1,key2,key3,key4,key5,Value) VALUES ( @date , 'AVG6M',@id1,@iso,@id2, @pricingSourceType,@MobileAvg6M)

insert into [dbo].[TX_AGGREGATE_DATA](Date,key1,key2,key3,key4,key5,Value) VALUES ( @date , 'ZSC3M',@id1,@iso,@id2, @pricingSourceType,@MobileZscore3M)
insert into [dbo].[TX_AGGREGATE_DATA](Date,key1,key2,key3,key4,key5,Value) VALUES ( @date , 'ZSC6M',@id1,@iso,@id2, @pricingSourceType,@MobileZscore6M)

drop table #MARKETDATA_YTM


END 

/*
-- exec dbo.TX_YTM_CalcAnalytical '16/04/2015','FRANCE','5','IBOXX_EUR'

--exec dbo.TX_YTM_CalcAnalytical('18/05/2015','FR000',null,'IBOXX_EUR')
delete from [dbo].[TX_AGGREGATE_DATA] where key1 in ('VOL3M','VOL6M','AVG3M','AVG6M','ZSC3M','ZSC6M')
*/



