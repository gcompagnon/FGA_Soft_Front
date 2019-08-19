
/****** Object:  StoredProcedure [dbo].[ACT_DataGridGrowth]    Script Date: 12/11/2012 15:14:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_DataGridGrowth]
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
	--l.unions As 'Liquidité',
	s.libelle As 'Secteur FGA', 
	--Round(a.growth_total_m,3) As 'Score',
	--a.growth_ranking_m As 'Ranking', 
	--a.growth_quintile_m As 'Quintile',
	a.is_euro As 'Euro Zone',
	SALES_TREND_5YR As 'Ventes Trend', 
	SALES_RSD As 'Ventes CV',
	SALES_GROWTH_NTM AS 'Cr Ventes',
	SALES_GROWTH_STM As 'Cr Ventes STM',
	EPS_TREND_5YR As 'BPA trend',
	EPS_RSD As 'BPA CV',	
	EPS_GROWTH_NTM As 'Cr BPA',
	EPS_GROWTH_STM As 'Cr BPA STM',
	EBIT_MARGIN_NTM As 'Marge EBIT',
	CAPEX_SALES_NTM As 'Capex/Ventes',
	IGROWTH_NTM As 'g',	
	PEG_NTM As 'PEg'
FROM ACT_DATA_FACTSET a
LEFT OUTER JOIN ACT_FGA_SECTOR s ON a.FGA_SECTOR = s.id  
--LEFT OUTER JOIN ACT_DATA_LIQUIDITY l ON a.isin = l.isin and l.date=@date
LEFT OUTER JOIN ACT_VALEUR t ON t.isin = a.isin
WHERE 
	a.date = @date and SXXP IS NOT NULL --and	growth_quintile_m is not NULL
ORDER BY s.libelle
--ORDER BY growth_total_m DESC

