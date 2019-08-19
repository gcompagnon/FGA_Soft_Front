
/****** Object:  StoredProcedure [dbo].[ACT_DataGridValue]    Script Date: 12/11/2012 15:17:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_DataGridValue]
		@date DATETIME
AS 

SELECT 
	--a.isin As 'Isin',
	t.TICKER_BLOOMBERG As 'Ticker',  
	a.company_name As 'Libellé',
	CASE 
	   WHEN LIQUIDITY_TEST = 'X' THEN 'X'
       WHEN LIQUIDITY_TEST = 'F' THEN 'F'
       ELSE NULL 
	END As 'Liquidité',
	--l.unions As 'Liquidity',
	s.libelle As 'Secteur FGA', 
	--Round(a.value_total_m,3) As 'Score',
	--a.value_ranking_m As 'Ranking',  
	--a.value_quintile_m As 'Quintile',
	a.is_euro As 'Euro Zone',
	PE_NTM As 'PE' ,
	PE_ON_AVG10Y AS 'PE/m(10)',
	PB_NTM AS 'PB',
	PB_ON_AVG10Y As 'PB/m(10)' ,
	EV_EBITDA_NTM As 'EV/EBITDA',
	EV_EBITDA_ON_AVG10Y As 'EV/EBITDA / m(10)',	
	DIV_YLD_NTM As 'Rdt',
	ROE_NTM As 'ROE' 
FROM ACT_DATA_FACTSET a
LEFT OUTER JOIN ACT_FGA_SECTOR s ON a.FGA_SECTOR = s.id 
--LEFT OUTER JOIN ACT_DATA_LIQUIDITY l ON l.date = @date and a.isin = l.isin 
LEFT OUTER JOIN ACT_VALEUR t ON a.isin = t.isin
WHERE 
	a.date = @date and SXXP IS NOT NULL--and Value_quintile_m is not NULL
ORDER BY s.libelle
--ORDER BY value_total_m DESC