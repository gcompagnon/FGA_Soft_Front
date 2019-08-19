USE [E0DBFGA01]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


---- ==============================================================
---- Description: Calcul des Taux YTM pour les années/ maturité cible  1 à 10 15 20 25 30 Yrs 	
--   prendre les gov par type d indice : BARCLAYS ou IBOXX_EUR et en calculer l interpolation YTM sur chaque pilier
---- ==============================================================

 
CREATE PROCEDURE [dbo].[TX_YTM_Interpolation]
	(@InputDate As DATETIME,
	@pricingSourceType [varchar](20))
AS

BEGIN

-- Pour debug
--DECLARE @InputDate  datetime  SET @InputDate = '10/12/2014' 

--DECLARE @pricingSourceType [varchar](20)
--SET @pricingSourceType = 'BARCLAYS'
--SET @pricingSourceType = 'IBOXX_EUR'

 
-- 	liste des emprunts pour chaque emetteur 
	SELECT distinct pr.Date, pr.Price_Source, r.Country, r.IssuerName, a.MaturityDate as DateM, pr.Debt_YTM_Rate as YTM, pr.ISINId,pr.SecurityId
	into #linear_interpolation
	from ref_security.PRICE as pr
	LEFT OUTER JOIN ref_security.ASSET as a on a.Id = pr.SecurityId
	LEFT OUTER JOIN ref_issuer.ROLE as r on r.AssetId = a.Id
	WHERE pr.Price_Source = @pricingSourceType and pr.Date = @InputDate

-- Liste des emetteurs
	SELECT distinct Country, IssuerName, IssuerName as transcoIssuer
	into #issuers
	from #linear_interpolation as l	
	
	--select * from #issuers 
	
-- transcodification "auto" pour les pays
select i.transcoIssuer,UPPER(c.name) as 'Issuer'
into #TRANCO_ISSUER
from #issuers as i
left outer join ref.COUNTRY as c on c.iso2 = i.country
where i.transcoIssuer = c.name or i.transcoIssuer = c.name+' (REPUBLIC OF)' or i.transcoIssuer = 'Republic of '+c.name 
or i.transcoIssuer = 'Kingdom of '+c.name 
-- maj de la liste des emetteurs

update #issuers
set IssuerName = t.Issuer
from #issuers as i
left outer join #TRANCO_ISSUER as t on t.transcoIssuer = i.transcoIssuer
where t.transcoIssuer is not null

-- Ne conserver que les emetteurs Pays
delete from #issuers where IssuerName not in (select Name from ref.COUNTRY) or Country is null

-- faire la transco sur le PAYS et moyenner si il y a plusieurs titres sur la même echeance
select Date, Price_Source, issuer.Country, issuer.IssuerName , DateM , AVG(YTM) as YTM 
into #linear_interpolationYTM
from #linear_interpolation as i
left outer join #issuers as issuer on issuer.transcoIssuer = i.IssuerName  and issuer.Country = i.Country 
group by Date, Price_Source, issuer.Country,issuer.IssuerName, DateM  

DROP Table #linear_interpolation
	
CREATE TABLE #MatDATES (Y int not null, Date datetime not null)
insert #MatDATES SELECT 1 as Y, DATEADD(year, 1, @InputDate) as Date
insert #MatDATES SELECT 2, DATEADD(year, 2, @InputDate) as Date
insert #MatDATES SELECT 3, DATEADD(year, 3, @InputDate) as Date
insert #MatDATES SELECT 4, DATEADD(year, 4, @InputDate) as Date
insert #MatDATES SELECT 5, DATEADD(year, 5, @InputDate) as Date
insert #MatDATES SELECT 6, DATEADD(year, 6, @InputDate) as Date
insert #MatDATES SELECT 7, DATEADD(year, 7, @InputDate) as Date
insert #MatDATES SELECT 8, DATEADD(year, 8, @InputDate) as Date
insert #MatDATES SELECT 9, DATEADD(year, 9, @InputDate) as Date
insert #MatDATES SELECT 10, DATEADD(year, 10, @InputDate) as Date
insert #MatDATES SELECT 15, DATEADD(year, 15, @InputDate) as Date
insert #MatDATES SELECT 20, DATEADD(year, 20, @InputDate) as Date
insert #MatDATES SELECT 30, DATEADD(year, 30, @InputDate) as Date

-- selection des titres / maturité qui correspondent aux années "cibles"
-- pour la date juste avant
SELECT l.IssuerName, l.Y, max(prev.dateM) as DateM
into #MAT_DATE_PREV
from  ( select distinct i.IssuerName,i.Country, md.* from #issuers as i, #MatDATES as md ) as l
LEFT outer join  #linear_interpolationYTM as prev on prev.Country = l.Country and prev.IssuerName = l.IssuerName
where prev.DateM <= l.Date
group by l.IssuerName,l.Y

-- pour la date juste apres
SELECT l.IssuerName, l.Y, min(nxt.dateM) as DateM
into #MAT_DATE_NEXT
from  ( select distinct i.IssuerName,i.Country,md.* from #issuers as i, #MatDATES as md ) as l
LEFT outer join  #linear_interpolationYTM as nxt on nxt.Country = l.Country and nxt.IssuerName = l.IssuerName 
where nxt.DateM >= l.Date
group by l.IssuerName,l.Y


-- Affichage pour chaque emetteur et chaque Annee Cible, le YTM interpolé.
--Calcul de YTM interpole :
-- pour x, date cible, prendre a et b a<x<b
-- f(a) et f(b) sont YTM des 2 obligations en maturité a et b
-- f(x) = f(a) + ( f(b) -- f(a) ) * (x-a) / (b-a)
-- f(x) = ( f(a)*(b-x) + f(b)*(x-a) ) / (b-a)

select i.IssuerName,i.Country,i.Y, prev.DateM as 'a',nxt.DateM as 'b',prevDebt.YTM as 'YTM_a', 
nextDebt.YTM as 'YTM_b',
(	SELECT CASE 
	WHEN nxt.DateM IS NULL THEN prevDebt.YTM
	WHEN prev.DateM IS NULL THEN nextDebt.YTM
	WHEN nxt.DateM = prev.DateM THEN prevDebt.YTM 
	ELSE ( DATEDIFF(d, prev.DateM, i.Date ) * nextDebt.YTM + DATEDIFF(d, i.Date, nxt.DateM) * prevDebt.YTM ) / DATEDIFF(d, prev.DateM, nxt.DateM) END ) AS Rate 
into #TMP
from (  select distinct i.IssuerName,i.country,md.* from #issuers as i, #MatDATES as md ) as i
LEFT outer join #MAT_DATE_PREV as prev on prev.IssuerName = i.IssuerName and prev.Y = i.Y
LEFT outer join #MAT_DATE_NEXT as nxt on nxt.IssuerName = i.IssuerName and nxt.Y = i.Y
LEFT outer join #linear_interpolationYTM as prevDebt on prevDebt.IssuerName in (select transcoIssuer from #issuers where IssuerName = i.IssuerName) and prevDebt.Country = i.Country and prevDebt.DateM = prev.DateM
LEFT outer join #linear_interpolationYTM as nextDebt on nextDebt.IssuerName in (select transcoIssuer from #issuers where IssuerName = i.IssuerName) and nextDebt.Country = i.Country and nextDebt.DateM = nxt.DateM


-- Historisation des resultats
delete from [dbo].[TX_AGGREGATE_DATA] where DATE = @InputDate and key1 = 'YTM_I' and key5 = @pricingSourceType

insert into [dbo].[TX_AGGREGATE_DATA]
select @InputDate as 'Date', 'YTM_I' as key1, IssuerName as Key2, 
 Country as Key3, Y as key4, @pricingSourceType as key5, Rate as 'Value' from #TMP where Rate is not null 
 

-- FREE MEMORY : Suppression des bases temporaires 
DROP TABLE #TMP

DROP Table #linear_interpolationYTM
DROP Table #MatDATES
DROP Table #issuers
DROP Table #MAT_DATE_PREV
DROP Table #MAT_DATE_NEXT
DROP Table #TRANCO_ISSUER
END
GO


