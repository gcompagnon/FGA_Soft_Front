USE [E2DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[ACT_TickerConvert]    Script Date: 11/22/2013 17:16:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ACT_TickerConvert]

	@Date As DATETIME

AS

		--DECLARE @date  DATETIME 
		--SET @date = '01/12/2013'

--|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|--
--|																											|--
--|											TICKER CONVERSION												|--
--|																											|--
--|	Principe : Convertit les ticker U.S. en ticker Bloomberg en fonction des règles de conversion (infra)	|--
--|			   et de la table de correspondance dans ACT_TICKER_CONVERSION									|--
--|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|--



SELECT ISIN, TICKER AS 'FACTSET', (SUBSTRING(TICKER, 0, CHARINDEX('-', TICKER))) AS 'TICKER', (SUBSTRING(TICKER, CHARINDEX('-', TICKER) + 1,20)) AS 'EXCH', '********************' AS 'BBG'
INTO #tick
FROM DATA_FACTSET
WHERE DATE=@date AND ISIN is not NULL
--SELECT * FROM #tick

UPDATE #tick
SET TICKER = REPLACE(TICKER, '.', '/') WHERE EXCH='GB'

UPDATE #tick
SET TICKER = TICKER + '/' WHERE EXCH='GB' AND LEN(TICKER) < 3

UPDATE #tick
SET TICKER = REPLACE(TICKER, '.', '')

UPDATE t
SET EXCH = c.EXCH_B
FROM #tick t
INNER JOIN ACT_TICKER_CONVERSION c ON c.EXCH_F = t.EXCH

UPDATE #tick
SET BBG = TICKER + ' ' + EXCH

UPDATE #tick
SET BBG = TICKER WHERE EXCH NOT IN (SELECT EXCH_F FROM ACT_TICKER_CONVERSION)

UPDATE t
SET BBG = c.BBG
FROM #tick t
INNER JOIN ACT_TICKER_CONVERSION c ON c.ISIN = t.ISIN

UPDATE fac
SET fac.TICKER = t.BBG
FROM DATA_FACTSET fac
INNER JOIN #tick t ON t.ISIN=fac.ISIN AND fac.DATE=@date

--UPDATE DATA_FACTSET
--SET TICKER = (CASE WHEN SUBSTRING(TICKER, 0, 1) = ' ' THEN SUBSTRING(TICKER, 1, LEN(TICKER) - 1) ELSE TICKER END)
--WHERE DATE=@date

--SELECT * FROM #tick
DROP TABLE #tick


