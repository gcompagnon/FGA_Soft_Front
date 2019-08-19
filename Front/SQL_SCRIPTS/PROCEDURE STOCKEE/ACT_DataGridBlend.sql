USE [E2DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[ACT_DataGridBlend]    Script Date: 11/05/2013 17:33:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_DataGridBlend]

	@date As DATETIME

AS

select  
		convert(int,s.code) As 'Code',
		s.label AS 'FGA Sector',
		--agr.BLEND_TOTAL As 'Old Score',
		a.BLEND_TOTAL AS 'Score',
		a.BLEND_GROWTH AS 'Growth',
		a.BLEND_YIELD AS 'Profit.',
		a.BLEND_VALUE AS 'Value',
		--SXXP AS 'Poids',
		a.SALES_GROWTH_NTM AS 'Cr Sales %',		
		a.EPS_GROWTH_NTM AS 'Cr EPS %',
		--Yield
		CASE WHEN a.PE_NTM = 0 THEN NULL ELSE a.PB_NTM/a.PE_NTM * 100 END AS 'ROE %',
		--Value
		a.DIV_YLD_NTM AS 'Dvd Yld %',
		a.PE_NTM AS 'PE x',
		a.PE_ON_AVG5Y AS 'PE / Avg5Y',
		a.PE_VS_IND_ON_AVG5Y As 'PE rel / Avg5Y',
		a.PB_NTM AS 'PB x', 
		a.PB_ON_AVG5Y AS 'PB / Avg5Y',
		a.PB_VS_IND_ON_AVG5Y As 'PB rel / Avg5Y',
		--Growth
		a.SALES_TREND_5YR AS 'Sales trend',
		a.SALES_RSD AS 'Sales CV',
		a.EPS_TREND_5YR AS 'EPS trend',
		a.EPS_RSD AS 'EPS CV',
		CASE WHEN s.id=401001 THEN a.PBT_RWA_TREND_5YR ELSE a.EBIT_MARGIN_TREND_5YR  END AS 'EBIT Margin trend',
		CASE WHEN s.id=401001 THEN a.PBT_RWA_RSD ELSE a.EBIT_MARGIN_RSD END As 'EBIT Margin CV'
from DATA_FACTSET a
LEFT OUTER JOIN ref_security.SECTOR s ON s.code= fga_sector
--Voir si les heures posent un pb
LEFT OUTER JOIN DATA_FACTSET agr ON CONVERT(char(10),agr.date,103)=CONVERT(char(10),(SELECT TOP 1 date FROM DATA_FACTSET WHERE date<= (SELECT Max(date) FROm ACT_FGA_SECTOR_RECOMMANDATION)),103) 
 /*and agr.MXEU='X'*/ and agr.fga_sector IS NOT NULL and agr.fga_sector=s.code and agr.ISIN is null

where a.date=@date and a.FGA_SECTOR IS NOT NULL and a.GICS_SECTOR is null /*and a.MXEU='X'*/
order by a.BLEND_TOTAL DESC


--select distinct(date) from ACT_DATA_FACTSET_AGR
-- execute ACT_DataGridBlend '06/07/2012'

