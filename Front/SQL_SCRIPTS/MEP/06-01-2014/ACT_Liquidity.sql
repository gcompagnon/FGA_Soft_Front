
/****** Object:  StoredProcedure [dbo].[ACT_Liquidty]    Script Date: 10/24/2011 17:27:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_Liquidity]

	@date As DATETIME

AS

/*
SELECT 	
	l.isin As 'Isin', 
	l.libelle As 'Libellé', 
	l.defaut As 'Défaut', 
	l.forcer As 'Forcer',
	s.libelle As 'Secteur'
into #res
FROM ACT_DATA_LIQUIDITY l
LEFT OUTER JOIN ACT_DATA_FACTSET d ON d.isin = l.isin and d.date= (SELECT DISTINCT TOP 1 date FROM ACT_DATA_FACTSET where date<=@date order by date DESC)
LEFT OUTER JOIN ACT_SUPERSECTOR s ON s.id = d.icb_sector 
WHERE l.date = @date 
ORDER BY l.libelle 


SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) As 'Compteur', * FROM #res

DROP TABLE #res
*/

--EXECUTE ACT_Liquidity '17/11/2011'
--SELECT isin from ACT_DATA_LIQUIDITY where date ='17/10/2011' EXCEPT SELECT isin from ACT_DATA_LIQUIDITY where date='31/10/2011'

SELECT 	
	isin As 'Isin', 
	company_name As 'Company Name'
FROM ACT_DATA_FACTSET
WHERE
	date= (SELECT DISTINCT TOP 1 date FROM ACT_DATA_FACTSET WHERE date<=@date order by date DESC) and
	liquidity_test = 'F'