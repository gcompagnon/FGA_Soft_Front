
-- =============================================
-- Author:		Coquard Benjamin
-- Create date: 01/12/2014
-- Description:	
-- =============================================

ALTER PROCEDURE [dbo].[ACT_RepartitionValeurPosition]
	@date AS DATETIME,
	@ticker AS VARCHAR(20),
	@isin AS VARCHAR(12)
AS
BEGIN
SELECT 
@ticker as Ticker,
(SELECT distinct COMPANY_NAME FROM DATA_FACTSET WHERE TICKER = @ticker AND DATE = @date) as Company,
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100002' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin)
	as "6100002",
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100030' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as  "6100030",
(SELECT distinct _position FROM ACT_PTF WHERE _compte = 'AVEURO' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as AVEURO,
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100004' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as  "6100004",
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100063' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as "6100063",
(SELECT distinct _position FROM ACT_PTF WHERE _compte = 'AVEUROPE' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as  AVEUROPE,
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100001' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as "6100001",
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100033' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as  "6100033",
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100062' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as "6100062",
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100026' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as  "6100026",
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100024' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as "6100024",
(SELECT distinct _position FROM ACT_PTF WHERE _compte = '6100066' 
	AND _datederniercours = (SELECT MAX(_datederniercours) FROM ACT_PTF 
	WHERE _datederniercours <= @date) AND _codeprodui = @isin) 
	as "6100066"
	
END
