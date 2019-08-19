IF OBJECT_ID(N'ref_holding.INDEX_LISTING', N'TF') IS NOT NULL
    DROP FUNCTION ref_holding.INDEX_LISTING;
GO

----------------------------------------------------------------------------------------------------
-- Fonction fournissant la décomposition d un indice dans son univers (STAT/RETURN pour Barclays ou DAY/NEXT_DAY pour MSCI) 
-- parametres :
--   @index : code de l indice : voir ref_holding.INDEX pour les libellés
--   les principaux sont DE0009682716 (Iboxx Corp) 
--                       BT11TREU (Euro Treasury) pour BARCLAYS_RETURN 
--                    et BT11TREUSTAT pour BARCLAYS_STAT
--                       MXEM (MSCI) ..
--   @dateIndex : date
--   @indexHoldingType: type de composition : valeurs possibles: BARCLAYS_RETURN BARCLAYS_STAT IBOXX_EUR MSCI_SI MSCI_SI_ND (ND pour NextDay)
----------------------------------------------------------------------------------------------------
-- Retour : 
----------------------------------------------------------------------------------------------------
--  IndexDate : reprise de la date parametree
--	IndexCode : reprise du code indice
--	ISIN : ISIN de la valeur
--	[AssetId] : clé interne de la valeur
--	MarketValue : montant de la valeur dans l indice
--	MarketValue_Cur : devise du montant
--	MarketValueTotal : montant global de  l indice -> le poids de la valeur est donc MarketValue/MarketValueTotal (si la devise est identique)
--	MarketValueTotal_Cur : devise du montant total 
--	IndexTotal : nombre de valeurs dans l indice (provient des donnees sources)
--  FaceAmount : valeur faciale pour les debts , si disponible
--	FaceAmount_Cur :
--  BookValue : montant d acquisition (pour le calcul de perf) si disponible
--	BookValue_Cur 
--	[Quantity] : quantité de titres pour la valeur dans le super indice, si disponible
--	[Weight] : 	poids de la valeur dans le superindice, si disponible !! Attention , ce n'est pas le poids de la valeur dans l indice demandé
----------------------------------------------------------------------------------------------------


CREATE FUNCTION ref_holding.INDEX_LISTING ( @index [nvarchar](12), @dateIndex datetime, @indexHoldingType [varchar](20))
RETURNS @Listing TABLE 
(
	IndexDate datetime NOT NULL,
	IndexCode [nvarchar](12) NOT NULL,
	ISIN [nvarchar](12) NOT NULL,
	[AssetId] [bigint] NOT NULL,
	MarketValue float,
	MarketValue_Cur [nchar](4),
	MarketValueTotal float,
	MarketValueTotal_Cur [nchar](4),
	IndexTotal int,
    FaceAmount float,
	FaceAmount_Cur [nchar](4),
    BookValue float,
	BookValue_Cur [nchar](4),
	[Quantity] [real] NULL,
	[Weight] [float] NULL
 )
AS
BEGIN

--DEBUG
/*
declare @index [nvarchar](12)
declare @dateIndex datetime
declare @indexHoldingType [varchar](20)
--set @index='BTS3TREU'
set @index='BT11TREU'
set @index='DE0006301161'
set @dateIndex = '01/06/2015'
--set @indexHoldingType='BARCLAYS_RETURN'
set @indexHoldingType='IBOXX_EUR'
*/
--DEBUG


-- Identification of parent id code for the index Code ( if the index is a superindex, this is the index)
--declare @indexAssetHolding [nvarchar](12)
declare @indexAssetHoldingId bigint
declare @superIndexAssetHoldingId bigint

select 
@superIndexAssetHoldingId = ParentId
,@indexAssetHoldingId = Id
from ref_holding.[COMPONENT] where ISIN =@index and DATE='31/12/9999'

-- The key of selection into the superindex for the subindex
declare @familyKey [nvarchar](max)
set @familyKey = (select familyKey from  ref_holding.[INDEX] where ISIN =@index )

-- Test si il il y a une zone geographique à prendre en compte / geographical area mapping or not
declare @CountryMatrix bit
set @CountryMatrix = 1
IF NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'CTRY') 
BEGIN
set @CountryMatrix = 0
END

--select @familyKey,@CountryMatrix,@indexAssetHoldingId

declare @IndexMV float
declare @IndexDivisor float
declare @IndexNumberOfSecurities int
declare @IndexMVCurrency nchar(4)

select @IndexMV= v.MarketValue , @IndexMVCurrency= v.MarketValue_Cur, @IndexNumberOfSecurities = v.IndexNumberOfSecurities, @IndexDivisor = v.IndexDivisor from  ref_Holding.VALUATION as v 
where v.Date = @dateIndex and v.ISINId = @index and v.ValuationSource = @indexHoldingType
--set @IndexMV = @IndexMV * 1E6 -- si la MV est en MUSD

INSERT @Listing
select h.Date as 'IndexDate',@index as 'IndexCode', h.AssetISIN as 'ISIN', h.AssetId, h.MarketValue, h.MarketValue_Cur, @IndexMV as 'MarketValueTotal' ,@IndexMVCurrency 'MarketValueTotal_Cur', @IndexNumberOfSecurities as 'IndexTotal', 
h.FaceAmount, h.FaceAmount_Cur, h.BookValue, h.BookValue_Cur, h.Quantity, h.[Weight]
--, p.Price,p.Price_Cur, fx.FX
--p.Yield_ChangePrice_1D_Value, p.Yield_ChangePrice_MTD_Value, p.Yield_ChangePrice_YTD_Value
from ref_holding.COMPONENT as c
left outer join ref_holding.ASSET_HOLDING as h on h.Id = c.Id
left outer join ref_security.ASSET as a on a.Id = h.AssetId
left outer join ref_common.IDENTIFICATION as id on id.id = a.IdentificationId  
--left outer join ref_security.PRICE as p on p.SecurityId = h.AssetId and p.Date = @datePrice and p.Price_Source = @indexHoldingType
--left outer join ref_rating.RATING as r on r.AssetId = e.id and r.RatingScheme =@indexHoldingType and r.ValueDate < @datePrice
--left outer join ref_security.FX_RATE as fx on fx.Date= @datePrice and fx.UnitCurrency = 'USD' and fx.QuotedCurrency = p.Price_Cur and fx.ValuationSource = 'MSCI_SI'
where
c.Date = @dateIndex and ( c.ParentId = @indexAssetHoldingId or c.ParentId = @superIndexAssetHoldingId) and
c.Id is not null and
h.FamilyKey like  @familyKey   
and ( id.Country in ( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'CTRY')
 OR NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'CTRY') ) 
and ( id.Country in ( select c.ValueCode from ref_holding.INDEX_HASHTABLE as c
                      left outer join ref_holding.INDEX_HASHTABLE as m on m.[KEY] = 'MTRX' and m.ISIN = @index 
                      where c.ISIN = m.ValueCode and c.[KEY]= 'CTRY')
 OR NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'MTRX') ) 
RETURN;
END
