-- ensemble des scripts pour calculer les interpolations sur les YTM que l on ne dispose pas  forcement
---Lancement du calcul pour l interpolation sur les YTM inexistants dans la base
-- le résultat est que l on obtient un YTM en quotidien "DAILY"
--  clé ='YTM_D'

--soit pour un ISIN , soit pour les 
----  @input1 : ISIN et @input2 : NULL
----  @input1 : Pays et @input2 : maturité cible : 1 à 10 15 20 25 30
----  @pricingSourceType : pricing source : BARCLAYS ou IBOXX_EUR



ALTER PROCEDURE [dbo].[TX_YTM_ExtractInterpolate]
	(@id1 [nvarchar](20),@id2 [nvarchar](20),
	@pricingSourceType [varchar](20))
AS

BEGIN

--declare	@id1 [nvarchar](20)
--declare @id2 [nvarchar](20)
--declare @pricingSourceType [varchar](20)
-- Liste des OAT
	--SELECT a.*
	--from ref_security.ASSET as a 
	--LEFT OUTER JOIN ref_issuer.ROLE as r on r.AssetId = a.Id
	--WHERE r.IssuerName = 'FRANCE'

--set @id1='FR0010670737'
--set @id2=NULL
--SET @pricingSourceType='BARCLAYS'
----SET @pricingSourceType = 'IBOXX_EUR'



--------------------------------------------------------
IF (@id2 is NULL) --- Le calcul concerne un ISIN / un seul titre
--------------------------------------------------------
BEGIN
	DECLARE @assetId bigint
	SET @assetId = (select ID from ref_security.ASSET where ISIN = @id1)
	
-- INSERT available Market Data YTM
-- BUG sur les Iboxx_eur, le YTM avant le 02/10/2014 est 100x
	
	select [Date] as 'Date' , 'YTM_D' as 'key1', @id1 as 'key2' , 'MARKET_DATA' as 'key3' ,NULL as 'key4' , @pricingSourceType as 'key5', 
	case 
	when Date between '02/07/2014' and '25/07/2014' and @pricingSourceType ='IBOXX_EUR' then Debt_YTM_Rate
	when Date <= '02/10/2014' and @pricingSourceType ='IBOXX_EUR' then Debt_YTM_Rate/100
	else Debt_YTM_Rate end as 'Value'
	into #MARKETDATA_YTM
	from ref_security.PRICE as pr
	where 
	pr.SecurityId = @assetId and
	Price_Source = @pricingSourceType
	and Date not in ( select Date from [dbo].[TX_AGGREGATE_DATA] where key1 = 'YTM_D' and key2=@id1 and key5 =@pricingSourceType and key3='MARKET_DATA')	
	
	-- MERGE AVEC LES RESULTATS EXISTANTS	
	MERGE INTO [dbo].[TX_AGGREGATE_DATA] AS D1 -- Target
	USING #MARKETDATA_YTM AS S1
	ON ( D1.Date = S1.Date and D1.key1 = S1.key1 and D1.key2 = S1.key2 and D1.key5 = S1.key5 )
	WHEN MATCHED THEN -- Il y avait un taux interpolé calculé
		Update set key3='MARKET_DATA', Value = S1.Value
	when NOT MATCHED by Target THEN
		Insert (Date,key1,key2,key3,key4,key5,Value) Values (S1.Date,S1.key1,S1.key2,S1.key3,S1.key4,S1.key5,S1.Value)
	;
		
	-- trouver toutes les dates entre la date min et max des marketdata
	declare	@today datetime
	set @today=(select Convert(date, getdate()))
	declare	@dateBegin datetime
	declare @dateEnd datetime
	set @dateBegin=(select MIN([Date]) from ref_security.PRICE as pr where pr.SecurityId = @assetId and	Price_Source = @pricingSourceType)
	set @dateEnd=(select MAX([Date]) from ref_security.PRICE as pr where pr.SecurityId = @assetId and	Price_Source = @pricingSourceType)

	-- Liste de l'ensemble des dates
	select a.Date, 
	case 
	when md.Date is null then 0
	else 1 end as 'MD'
	into #DATES
	from (
		select DATEADD( DAY , - a.a - (10 * b.a) - (100 * c.a) - (1000 * d.a) ,@today) as 'Date'
		from (select 0 as a union all select 1 union all select 2 union all select 3 union all select 4 union all select 5 union all select 6 union all select 7 union all select 8 union all select 9) as a
		cross join (select 0 as a union all select 1 union all select 2 union all select 3 union all select 4 union all select 5 union all select 6 union all select 7 union all select 8 union all select 9) as b
		cross join (select 0 as a union all select 1 union all select 2 union all select 3 union all select 4 union all select 5 union all select 6 union all select 7 union all select 8 union all select 9) as c
		cross join (select 0 as a union all select 1 union all select 2 union all select 3 union all select 4 union all select 5 union all select 6 union all select 7 union all select 8 union all select 9) as d
	) a
	full outer join [dbo].[TX_AGGREGATE_DATA] as md on md.key1='YTM_D' and md.key2=@id1 and md.key3='MARKET_DATA' and md.key5=@pricingSourceType and md.Date = a.Date
	where
	DATEPART(dw,a.date) not in (6,7) --Week end exclus
	and 
	a.Date between @dateBegin and @dateEnd

	-- INTERPOLATION
	select i.Date, 'YTM_D' as 'key1', @id1 as 'key2', 'INTERPOLATED' as 'key3', NULL as 'key4',@pricingSourceType as 'key5', 
	( DATEDIFF(d, prevDebt.[Date], i.[Date] ) * nextDebt.Value + DATEDIFF(d, i.[Date], nextDebt.[Date]) * prevDebt.Value ) / DATEDIFF(d, prevDebt.[Date], nextDebt.[Date])  AS 'Value'  
	into #INTERPOLATED_YTM
	from #DATES as i
	left outer join [dbo].[TX_AGGREGATE_DATA] as nextDebt on nextDebt.key1='YTM_D' and nextDebt.key2 =@id1 and nextDebt.key3='MARKET_DATA' and nextDebt.key5=@pricingSourceType and nextDebt.Date = (select Min( Date) from #DATES where MD=1 and Date>i.[Date])
	left outer join [dbo].[TX_AGGREGATE_DATA] as prevDebt on prevDebt.key1='YTM_D' and prevDebt.key2 =@id1 and prevDebt.key3='MARKET_DATA' and prevDebt.key5=@pricingSourceType and prevDebt.Date = (select Max( Date) from #DATES where MD=1 and Date<i.[Date])
	where i.MD=0
	order by Date
	
	-- MERGE AVEC LES RESULTATS EXISTANTS 	
	-- calcul d interpolation
	MERGE INTO [dbo].[TX_AGGREGATE_DATA] AS D -- Target
	USING #INTERPOLATED_YTM As S
	on ( D.Date = S.Date and D.key1 = S.key1 and D.key2 = S.key2 and D.key5 = S.key5 )
	WHEN MATCHED THEN --maj des calculs
		Update set key3='INTERPOLATED', Value = S.Value
	when NOT MATCHED by Target Then -- nouvelle dates à interpoler
		Insert (Date,key1,key2,key3,key4,key5,Value) Values (S.Date,S.key1,S.key2,S.key3,S.key4,S.key5,S.Value)		
	;
		
	drop table #MARKETDATA_YTM	
	drop table #INTERPOLATED_YTM	
	drop table #DATES
	
	

END
--------------------------------------------------------
ELSE -- si le calcul concerne les PAYS / maturité cible
--------------------------------------------------------
BEGIN

-- A FAIRE
select * from [dbo].[TX_AGGREGATE_DATA] where key1='YTM_D' and key2 = 'FR0010670737'
and key5 ='BARCLAYS' order by Date
  
END

END -- PROCEDURE