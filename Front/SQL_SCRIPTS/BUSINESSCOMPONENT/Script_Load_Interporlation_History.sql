-- ensemble des scripts pour calculer les interpolations en historique

---Lancement du calcul pour l interpolation à Maturité Cible / PAYS  clé ='YTM_I'
exec dbo.[TX_InterpObligation] '26/11/2014', 'IBOXX_EUR'
exec dbo.[TX_InterpObligation] '27/11/2014', 'IBOXX_EUR'
exec dbo.[TX_InterpObligation] '28/11/2014', 'IBOXX_EUR'
exec dbo.[TX_InterpObligation] '1/12/2014', 'IBOXX_EUR'
exec dbo.[TX_InterpObligation] '2/12/2014', 'IBOXX_EUR'
exec dbo.[TX_InterpObligation] '3/12/2014', 'IBOXX_EUR'
exec dbo.[TX_InterpObligation] '4/12/2014', 'IBOXX_EUR'
exec dbo.[TX_InterpObligation] '5/12/2014', 'IBOXX_EUR'
exec dbo.[TX_InterpObligation] '11/12/2014', 'IBOXX_EUR'
exec dbo.[TX_InterpObligation] '01/10/2014', 'IBOXX_EUR'

exec dbo.[TX_InterpObligation] '12/12/2014', 'BARCLAYS'



select * from  [dbo].[TX_AGGREGATE_DATA] 
where key1='YTM_I' 
and key5 = 'IBOXX_EUR' and DATE <'03/10/2014'
order by Date
exec dbo.[TX_InterpObligation] '31/12/2014', 'IBOXX_EUR'
-- ETAPE 2 --Indicateurs  Pour un ISIN


declare	@id1 [nvarchar](20)
set @id1='FR0010466938'

execute [dbo].[TX_YTM_ExtractInterpolate]  @id1,NULL,'BARCLAYS'
select * from dbo.TX_YTM_Indicateurs('20/10/2010','20/12/2014', @id1,NULL,'BARCLAYS')
 execute [dbo].[TX_YTM_ExtractInterpolate]  @id1,NULL,'IBOXX_EUR'
 select * from dbo.TX_YTM_Indicateurs('20/10/2010','20/12/2014', @id1,NULL,'IBOXX_EUR')

 select COUNT(*) from [dbo].[TX_AGGREGATE_DATA] where key1='YTM_D' and key2 = @id1

select DATE,* from [dbo].[TX_AGGREGATE_DATA] as d where key1='YTM_I'
and Value > 100
 order by d.date desc


	SELECT distinct pr.Date, pr.Price_Source, pr.Debt_YTM_Rate as YTMIBOXX,pr2.Debt_YTM_Rate as YTMBarclays, pr.ISINId,pr.SecurityId
	from ref_security.PRICE as pr
	LEFT OUTER JOIN ref_security.PRICE as pr2 on pr2.SecurityId = pr.SecurityId and pr2.Date = pr.Date and pr2.Price_Source = 'BARCLAYS'
	WHERE pr.Price_Source = 'IBOXX_EUR' and pr.Date  between '01/10/2014' and '31/12/2014'	
	order by pr.ISINId ,pr.Date



declare	@id1 [nvarchar](20)
set @id1='FR0010466938'
select * from dbo.TX_YTM_Indicateurs('20/10/2014','20/12/2014', @id1,NULL,'IBOXX_EUR')
 
 
 
 


-- CONTROLES
select distinct Date,ISINId  from ref_holding.VALUATION where ValuationSource ='IBOXX_EUR' and ISINId ='DE0009682716'
order by Date 

select distinct Date,ISINId,ValuationSource  from ref_holding.VALUATION where ValuationSource ='BARCLAYS_RETURN' and ISINId ='BT11TREU'
order by Date 

select * from [dbo].[TX_AGGREGATE_DATA] where key1='YTM_D' and key2 = 'FR0010670737'
and key5 ='BARCLAYS' order by Date

select * from [dbo].[TX_AGGREGATE_DATA] where key1='YTM_D' and key2 = 'FR0010466938'
and key5 ='IBOXX_EUR' order by Date
 
 
exec dbo.[TX_InterpObligation] '31/12/2013', 'IBOXX_EUR'

select distinct DATE,key5, COUNT(*) from [TX_AGGREGATE_DATA] group by DATE,key5  order by DATE
select top 100 * from [TX_AGGREGATE_DATA]

sp_columns [TX_AGGREGATE_DATA]

select top 100 * from [dbo].[TX_AGGREGATE_DATA] where key1='YTM_I' 

select distinct date from [dbo].[TX_AGGREGATE_DATA] where key1='YTM_I'  order by date

select Date as 'Date',key2 as 'Gov',CONVERT(int, key4)as 'mat' , Value,* from [dbo].[TX_AGGREGATE_DATA] as d where key1='YTM_I' and key2 ='FRANCE' and key5 = 'IBOXX_EUR' order by d.Date,mat

select distinct key2 from [dbo].[TX_AGGREGATE_DATA] where key1='YTM_I' 
and key5 ='BARCLAYS' order by Date


select * from [dbo].[TX_AGGREGATE_DATA] where DATE='31/08/200' key1='YTM_I' 
