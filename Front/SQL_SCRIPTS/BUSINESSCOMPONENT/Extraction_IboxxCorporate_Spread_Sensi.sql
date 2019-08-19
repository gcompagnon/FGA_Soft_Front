--- EXTRACTION IBOXX CORP 
-- GLOBAL ou 1-5

--pour le calcul des contri au sensi et spread


declare @date datetime
set @date = '24/05/2016'
drop table #LISTING
drop table #ISSUEROLE
drop table #ISSUERS
drop table #RATINGS
--GLOBAL
select * 
into #LISTING
from ref_holding.INDEX_LISTING('DE0006301161', @date,'IBOXX_EUR') as l

 --	iBoxx EUR Corporates 1-3
--select * 
--into #LISTING
--from ref_holding.INDEX_LISTING('DE0006301187', @date,'IBOXX_EUR') as l
----	iBoxx EUR Corporates 3-5
--insert into #LISTING
--select * from ref_holding.INDEX_LISTING('DE0006301518', @date,'IBOXX_EUR') as l
	

select distinct AssetId, IssuerName , Country, ROW_NUMBER() OVER(partition by AssetId order by AssetId) AS Row 
INTO #ISSUEROLE
from ref_issuer.ROLE as issuer
where issuer.AssetId in (select AssetId from #LISTING) and issuer.Discriminator ='IssuerRole'

select * 
into #ISSUERS
from #ISSUEROLE 
where Row = 1
order by AssetId 

select rdate.AssetId,r2.Value 
into #RATINGS
from (
select AssetId,max( valueDate) as ValueDate
from ref_rating.RATING as r 
where r.AssetId in (select AssetId from #LISTING) and r.RatingScheme ='IBOXX_EUR'
and ValueDate <= @date
group by AssetId
) as rdate
left outer join ref_rating.RATING as r2  on rdate.AssetId = r2.AssetId and r2.ValueDate = rdate.ValueDate 
and r2.RatingScheme ='IBOXX_EUR'


select l.AssetId, l.ISIN,i.CUSIP,l.IndexDate,'' as RebalancingDate,issuer.IssuerName,c1.name as 'IssuerCountry',i.Ticker,c2.name as 'DebtCountry',
d.NextInterest_Rate,'' as IndexFamily,ac.Classification1, ac.Classification2,ac.Classification3,ac.Classification4,ac.Classification5,ac.Classification6,
ac.Classification7,'','',
d.FinancialInfos_Seniority_SeniorityLevel as 'Seniority' ,SUBSTRING(d.FinancialInfos_Seniority_SeniorityLevel,1,3) as 'Debt',
r.Value as 'Rating',
  a.MaturityDate, l.MarketValue / (l.MarketValueTotal) as 'Weight', 
 p.DebtSpread_AssetSwapSpread, '',l.MarketValue ,p.Price_Ask, p.Price_Bid , p.Debt_DirtyP ,p.Price,
 p.Debt_YTM_Rate, p.Debt_Sensitivity, p.DebtSpread_GovSpread,'' as Benchmark,
 l.MarketValue / (l.MarketValueTotal) *  p.Debt_Sensitivity * p.DebtSpread_AssetSwapSpread as 'ContribSpread_Sensi',
 l.MarketValue / (l.MarketValueTotal) *  p.Debt_Sensitivity  as 'ContribSensi' ,
 case
 when ( DATEDIFF( day, l.IndexDate,a.MaturityDate  )/365 )<=1 then '0-1 an'
 when ( DATEDIFF( day, l.IndexDate,a.MaturityDate  )/365 )<=3 then '1-3 ans'
 when ( DATEDIFF( day, l.IndexDate,a.MaturityDate  )/365 )<=5 then '3-5 ans'
 when ( DATEDIFF( day, l.IndexDate,a.MaturityDate  )/365 )<=7 then '5-7 ans'
 when ( DATEDIFF( day, l.IndexDate,a.MaturityDate  )/365 )<=10 then '7-10 ans'
 when ( DATEDIFF( day, l.IndexDate,a.MaturityDate  )/365 )<=15 then '10-15 ans'
 when ( DATEDIFF( day, l.IndexDate,a.MaturityDate  )/365 )<=20 then '15-20 ans'
 else '+20ans'
 end as 'Range_Maturity'
 ,p.*
from #LISTING as l
left outer join ref_security.ASSET as a on a.Id = l.AssetId
left outer join ref_security.DEBT as d on d.Id = l.AssetId
left outer join #RATINGS as r on r.AssetId = l.AssetId 
left outer join ref_security.PRICE as p on p.SecurityId = l.AssetId and p.Date = l.IndexDate and p.Price_Source = 'IBOXX_EUR'
left outer join ref_common.IDENTIFICATION as i on i.Id = a.IdentificationId
left outer join ref_security.ASSET_CLASSIFICATION as ac on  ac.AssetId = l.AssetId and ac.Source ='IBOXX_EUR'
left outer join #ISSUERS as issuer on  issuer.AssetId = l.AssetId 
left outer join ref.COUNTRY as c1 on c1.iso2 = issuer.Country
left outer join ref.COUNTRY as c2 on c2.iso2 = i.Country
order by IssuerName 

