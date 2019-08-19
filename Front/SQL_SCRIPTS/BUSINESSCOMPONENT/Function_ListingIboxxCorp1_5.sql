

-- COMPOSITION DE L INDICE IBOXX CORPORATE 1-5
-- pour etre utiliser dans les feuilles excel de credit ISR

IF OBJECT_ID(N'ref_holding.INDEX_LISTING_CORP1_5', N'TF') IS NOT NULL
    DROP FUNCTION ref_holding.INDEX_LISTING_CORP1_5;
GO

CREATE FUNCTION ref_holding.INDEX_LISTING_CORP1_5 (@dateIndex datetime)
RETURNS @Listing1_5 TABLE 
(
ISIN [nvarchar](12) NOT NULL,
C1 char(1) NULL,C2 char(1) NULL,C3 char(1) NULL,
Poids float NULL,
C5 char(1) NULL,C6 char(1) NULL,C7 char(1) NULL,C8 char(1) NULL,
[LT Rating] [nvarchar](max) NULL,
C10 char(1) NULL,C11 char(1) NULL,C12 char(1) NULL,C13 char(1) NULL,C14 char(1) NULL,C15 char(1) NULL,
C16 char(1) NULL,C17 char(1) NULL,C18 char(1) NULL,C19 char(1) NULL,C20 char(1) NULL,C21 char(1) NULL,
C22 char(1) NULL,C23 char(1) NULL,C24 char(1) NULL,C25 char(1) NULL,C26 char(1) NULL,C27 char(1) NULL,
C28 char(1) NULL,C29 char(1) NULL,C30 char(1) NULL,C31 char(1) NULL,C32 char(1) NULL,C33 char(1) NULL,
C34 char(1) NULL,C35 char(1) NULL,C36 char(1) NULL,C37 char(1) NULL,C38 char(1) NULL,C39 char(1) NULL,
C40 char(1) NULL,C41 char(1) NULL,C42 char(1) NULL,C43 char(1) NULL,C44 char(1) NULL,C45 char(1) NULL,
C46 char(1) NULL,C47 char(1) NULL,C48 char(1) NULL,
[Debt & Tier] char(13), 
Sector [nvarchar](max)
)
AS BEGIN


	declare @ListIndex Table( iBoxxSubIndex [nvarchar](12) )
	---- iBoxx EUR Corporates 1-5
	insert into @ListIndex values ( 'DE0006301187')--	iBoxx EUR Corporates 1-3
	insert into @ListIndex values ( 'DE0006301518')--	iBoxx EUR Corporates 3-5

	declare @Listing TABLE (IndexDate datetime NOT NULL,IndexCode [nvarchar](12) NOT NULL,ISIN [nvarchar](12) NOT NULL,
		[AssetId] [bigint] NOT NULL,MarketValue float,MarketValue_Cur [nchar](4),MarketValueTotal float,MarketValueTotal_Cur [nchar](4),IndexTotal int,
		FaceAmount float,FaceAmount_Cur [nchar](4),BookValue float,BookValue_Cur [nchar](4),[Quantity] [real] NULL,
		[Weight] [float] NULL)

	declare @indexI [nvarchar](12)

	DECLARE indexes_cursor CURSOR FOR
	select iBoxxSubIndex from @ListIndex;

	OPEN indexes_cursor;
	FETCH NEXT FROM indexes_cursor INTO @indexI;

	WHILE @@FETCH_STATUS = 0
	   BEGIN
		  insert @Listing 
		  select * from ref_holding.INDEX_LISTING( @indexI, @dateIndex,'IBOXX_EUR')
		  FETCH NEXT FROM indexes_cursor INTO @indexI;
	   END;

	CLOSE indexes_cursor;
	DEALLOCATE indexes_cursor;

	declare @IndexMV float
	set @IndexMV = (select sum(MarketValue) from  @Listing)

	insert into @Listing1_5 
	select l.ISIN,'','','',100*MarketValue/@IndexMV as 'Poids','','','','',rValide.Value as 'LT Rating','','','','','','',
	'','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','',d.FinancialInfos_Seniority_SeniorityLevel as 'Debt & Tier',c.Classification5 as 'Sector'
	from @Listing as l
	left outer join ref_security.ASSET_CLASSIFICATION as c on c.AssetId = l.AssetId and c.Source ='IBOXX_EUR'
	left outer join ( select AssetId, MAX(ValueDate) as ValueDate from ref_rating.RATING where RatingScheme ='IBOXX_EUR' and AssetId in (select AssetId from @Listing) and ValueDate <= @dateIndex group by AssetId ) as r on r.AssetId = l.AssetId 
	left outer join ref_rating.RATING as rValide on rValide.RatingScheme ='IBOXX_EUR' and rValide.AssetId = l.AssetId  and rValide.ValueDate = r.ValueDate
	left outer join ref_security.DEBT as d on d.Id = l.AssetId
	
	insert into @Listing1_5 
	select 'ZZ-FIN' as ISIN,'','','',0 as 'Weight','','','','','' as 'Rating','','','','','','',
	'','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','' as 'Debt & Tier','' as 'Sector'

	--select 'ZZ-FIN' as ISIN,'','','',0 as 'Weight','','','','','' as 'Rating','','','','','','',
	--'','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','' as 'Debt & Tier','' as 'Sector'
	RETURN;
END
