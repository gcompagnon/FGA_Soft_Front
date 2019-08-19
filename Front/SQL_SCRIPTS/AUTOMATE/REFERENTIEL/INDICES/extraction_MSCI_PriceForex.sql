
-- Extraction des prix et forex pour chaque composents de l'indice
-- 
--declare @dateIndex datetime
--set @dateIndex = '31/03/2014'

--declare @indexHoldingType varchar(20)
--set @indexHoldingType = 'MSCI_SI'

select p.SecurityId, p.ISINId,@dateIndex as 'Date',@indexHoldingType as 'Index',p.Price,p.Price_Cur, fx.FX 
from  ref_holding.ASSET_HOLDING as h
left outer join ref_security.PRICE as p on p.Date = @dateIndex and p.SecurityId = h.AssetId
left outer join ref_security.FX_RATE as fx on fx.Date = @dateIndex and fx.[ValuationSource] = 'MSCI_SI' and fx.[QuotedCurrency] = p.Price_Cur 
where h.Date = @dateIndex and h.ParentISIN = @indexHoldingType 
