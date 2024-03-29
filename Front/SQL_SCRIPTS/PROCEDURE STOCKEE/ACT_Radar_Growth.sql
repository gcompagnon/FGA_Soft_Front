
/****** Object:  StoredProcedure [dbo].[ACT_Radar_Growth]    Script Date: 12/20/2013 17:48:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_Radar_Growth]

	@Date As DATETIME,
	@Pres As VARCHAR(20),
	@TICKER As VARCHAR(20),
	@FGA As VARCHAR(20),
	@MM As VARCHAR(4)
AS

--DECLARE @Date AS VARCHAR(20),@pres AS VARCHAR(20),@TICKER AS VARCHAR(12), @FGA As VARCHAR(20), @MM As VARCHAR(4)
--SET @date='01/12/2013'
--SET @pres='Growth'
--SET @TICKER='CDI FP'
--SET @FGA='0'
--SET @MM='AVG'

CREATE TABLE #values
(
	NAME VARCHAR(50) COLLATE DATABASE_DEFAULT,
	VALUE float
)

IF @Pres='Growth' 
BEGIN

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'EPS CHG NTM' AS NAME, (CASE WHEN @MM = '' THEN N_EPS_CHG_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'EPS TREND' AS NAME, (CASE WHEN @MM = '' THEN N_EPS_TREND ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'EPS VAR RSD' AS NAME, (CASE WHEN @MM = '' THEN N_EPS_VAR_RSD ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'SALES CHG NTM' AS NAME, (CASE WHEN @MM = '' THEN N_SALES_CHG_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'SALES TREND' AS NAME, (CASE WHEN @MM = '' THEN N_SALES_TREND ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'SALES VAR RSD' AS NAME, (CASE WHEN @MM = '' THEN N_SALES_VAR_RSD ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

END
ELSE IF @Pres='Value' 
BEGIN

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'PE NTM' AS NAME, (CASE WHEN @MM = '' THEN N_PE_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'PE ON MED5Y' AS NAME, (CASE WHEN @MM = '' THEN N_PE_ON_MED5Y ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'PE PREMIUM ON HIST' AS NAME, (CASE WHEN @MM = '' THEN N_PE_PREMIUM_ON_HIST ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
IF @FGA=401001 
BEGIN
INSERT INTO #values
SELECT 'P TBV NTM' AS NAME, (CASE WHEN @MM = '' THEN N_P_TBV_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END
ELSE
BEGIN
INSERT INTO #values
SELECT 'PB NTM' AS NAME, (CASE WHEN @MM = '' THEN N_PB_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END

-------------------------------------------------------------------
IF @FGA=401001 
BEGIN
INSERT INTO #values
SELECT 'P TBV ON MED5Y' AS NAME, (CASE WHEN @MM = '' THEN N_P_TBV_ON_MED5Y ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END
ELSE
BEGIN
INSERT INTO #values
SELECT 'PB ON MED5Y' AS NAME, (CASE WHEN @MM = '' THEN N_PB_ON_MED5Y ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'PB PREMIUM ON HIST' AS NAME, (CASE WHEN @MM = '' THEN N_PB_PREMIUM_ON_HIST ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'DIV YLD NTM' AS NAME, (CASE WHEN @MM = '' THEN N_DIV_YLD_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

END
ELSE IF @Pres='Qualite' 
BEGIN

-------------------------------------------------------------------
IF @FGA=401001 
BEGIN
INSERT INTO #values
SELECT 'PBT RWA NTM' AS NAME, (CASE WHEN @MM = '' THEN N_PBT_RWA_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END
ELSE IF @FGA=403010 
BEGIN
INSERT INTO #values
SELECT 'PBT SALES NTM' AS NAME, (CASE WHEN @MM = '' THEN N_PBT_SALES_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END
ELSE
BEGIN
INSERT INTO #values
SELECT 'EBIT MARGIN NTM' AS NAME, (CASE WHEN @MM = '' THEN N_EBIT_MARGIN_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END

-------------------------------------------------------------------
IF @FGA=401001 
BEGIN
INSERT INTO #values
SELECT 'ROTE NTM' AS NAME, (CASE WHEN @MM = '' THEN N_ROTE_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END
ELSE
BEGIN
-------------------------------------------------------------------
INSERT INTO #values
SELECT 'FCF TREND' AS NAME, (CASE WHEN @MM = '' THEN N_FCF_TREND ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'NET DEBT EBITDA NTM' AS NAME, (CASE WHEN @MM = '' THEN N_NET_DEBT_EBITDA_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'ROE NTM' AS NAME, (CASE WHEN @MM = '' THEN N_ROE_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END

-------------------------------------------------------------------
IF @FGA=401001 
BEGIN
INSERT INTO #values
SELECT 'COST INCOME NTM' AS NAME, (CASE WHEN @MM = '' THEN N_COST_INCOME_NTM ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date
END

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'ISR' AS NAME, (CASE WHEN @MM = '' THEN GARPN_ISR_S ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

END
ELSE IF @Pres='Total' 
BEGIN

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'ISR' AS NAME, (CASE WHEN @MM = '' THEN GARPN_ISR_S ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'Growth' AS NAME, (CASE WHEN @MM = '' THEN GARPN_GROWTH_S ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'Value' AS NAME, (CASE WHEN @MM = '' THEN GARPN_VALUE_S ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

-------------------------------------------------------------------
INSERT INTO #values
SELECT 'Quality' AS NAME, (CASE WHEN @MM = '' THEN GARPN_YIELD_S ELSE 5 END) AS VALUE
FROM DATA_FACTSET WHERE TICKER=@TICKER AND DATE=@Date

END

SELECT * FROM #values

DROP TABLE #values


/*
DECLARE @poids AS FLOAT
IF @FGA='FGA_EU' 
BEGIN
SET @poids = (SELECT SUM(SXXP) As 'poids' FROM ACT_DATA_FACTSET where date=@Date and is_euro is not null and SXXP IS NOT NULL)
END
ELSE 
BEGIN
SET @poids = (SELECT SUM(SXXP) As 'poids' FROM ACT_DATA_FACTSET where date=@Date and is_euro is not null and SXXP IS NOT NULL)
END

SELECT 
	isin,
	icb_supersector, 
	company_name, 
	SXXP/@poids As 'poids'
Into #euro_poids
FROM ACT_DATA_FACTSET 
WHERE date=@Date and is_euro is not null and SXXP IS NOT NULL
--SELECT * FROM #euro_poids

CREATE TABLE #cible(
	indicator Varchar(30),
	indicator_name Varchar(120),
	Portefeuille VARCHAR(20),
	Valeur_tmp Float,
	Valeur Float
)
	
	

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('SALES_TREND_5YR','Ventes tendance (5ans)','Croissance',6.51175401873785)	
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('SALES_RSD','Ventes stabilité (5ans)','Croissance',1.12025174256388)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('SALES_GROWTH_NTM','Ventes 12 prochains mois','Croissance',7.96845643165454)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('SALES_GROWTH_STM','Ventes 12 prochains mois (N+1)','Croissance',6.74851558540184)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('EPS_TREND_5YR','BPA tendance (5ans)','Croissance',16.7907304465374)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('EPS_RSD','BPA stabilité (5ans)','Croissance',1.17423773413739)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('EPS_GROWTH_NTM','BPA 12 prochains mois','Croissance',12.211809148526)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('EPS_GROWTH_STM','BPA 12 prochains mois (N+1)','Croissance',16.5576282065829)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('EBIT_MARGIN_NTM','Marge EBIT 12 prochains mois','Croissance',16.633213827549)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('CAPEX_SALES_NTM','Capex/Sales 12 prochains mois','Croissance',5.88363198037017)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('IGROWTH_NTM','Croissance interne(g) 12 prochains mois','Croissance',11.2274784277739)
INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) VALUES('PEG_NTM_INVERSE','PEG 12 prochains mois','Croissance',1.3)


INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp)
SELECT 
		'SALES_TREND_5YR' As indicator,
		'Ventes tendance (5ans)' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(SALES_TREND_5YR*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND SALES_TREND_5YR IS NOT NULL 

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp)
SELECT 
		'SALES_RSD' As indicator,
		'Ventes stabilité (5ans)' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(SALES_RSD*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND SALES_RSD IS NOT NULL

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp)
SELECT 
		'SALES_GROWTH_NTM' As indicator,
		'Ventes 12 prochains mois' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(SALES_GROWTH_NTM*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND SALES_GROWTH_NTM IS NOT NULL

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp)
SELECT 
		'SALES_GROWTH_STM' As indicator,
		'Ventes 12 prochains mois (N+1)' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(SALES_GROWTH_STM*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND SALES_GROWTH_STM IS NOT NULL

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp)
SELECT 
		'EPS_TREND_5YR' As indicator,
		'BPA tendance (5ans)' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(EPS_TREND_5YR*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND EPS_TREND_5YR IS NOT NULL

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) 
SELECT 
		'EPS_RSD' As indicator,
		'BPA stabilité (5ans)' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(EPS_RSD*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND EPS_RSD IS NOT NULL

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) 
SELECT 
		'EPS_GROWTH_NTM' As indicator,
		'BPA 12 prochains mois' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(EPS_GROWTH_NTM*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND EPS_GROWTH_NTM IS NOT NULL

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) 
SELECT 
		'EPS_GROWTH_STM' As indicator,
		'BPA 12 prochains mois (N+1)' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(EPS_GROWTH_STM*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND EPS_GROWTH_STM IS NOT NULL

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp)
SELECT 
		'EBIT_MARGIN_NTM' As indicator,
		'Marge EBIT 12 prochains mois (N+1)' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(EBIT_MARGIN_NTM*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND EBIT_MARGIN_NTM IS NOT NULL AND d.icb_supersector NOT LIKE '8%'

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp)
SELECT 
		'CAPEX_SALES_NTM' As indicator,
		'Capex/Sales 12 prochains mois' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(CAPEX_SALES_NTM*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND CAPEX_SALES_NTM IS NOT NULL AND d.icb_supersector NOT LIKE '8%'

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) 
SELECT 
		'IGROWTH_NTM' As indicator,
		'Croissance interne(g) 12 prochains mois' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(IGROWTH_NTM*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND IGROWTH_NTM IS NOT NULL

INSERT INTO #cible(indicator, indicator_name, Portefeuille, Valeur_tmp) 
SELECT 
		'PEG_NTM_INVERSE' As indicator,
		'PEG 12 prochains mois' As indicator_name,
		'Eurostoxx' As Portefeuille,
		SUM(PEG_NTM_INVERSE*poids)/SUM(poids) As Valeur_tmp
FROM ACT_DATA_FACTSET d, #euro_poids p 
WHERE date=@Date AND p.isin = d.isin AND PEG_NTM_INVERSE IS NOT NULL
--SELECT * FROM #cible order by Portefeuille,indicator_name 


SELECT 
	MAX(valeur_tmp)+1 As 'Max1',
	CASE WHEN MAX(valeur_tmp) <1.9 THEN 
		Round(MAX(valeur_tmp),0,1)+1
	ELSE
		(ROUND((MAX(valeur_tmp)+1)/10,0,1)+1)*10
	END As 'echelle',  
	indicator 
INTO #echelle
from #cible group by indicator
--SELECt * FROM #echelle order by indicator

UPDATE #cible 
SET valeur = (5*valeur_tmp)/e.echelle
FROM #cible c, #echelle e
WHERE c.indicator = e.indicator
	


IF @pres = 'growth11'
BEGIN
SELECT indicator_name, valeur FROM #cible where portefeuille = @pres 
END 

IF @pres = 'growth22' 
BEGIN
SELECT indicator_name, valeur FROM #cible where portefeuille = @pres
END 

SELECT N_EPS_TREND,
		N_EPS_VAR_RSD,		
		N_SALES_TREND,
		N_SALES_CHG_NTM,
		N_SALES_TREND,
		N_SALES_VAR_RSD,
		N_PE_NTM,
		N_PE_ON_MED5Y,
		N_PE_PREMIUM_ON_HIST,
		N_P_TBV_NTM,
		N_PB_NTM,
		N_PB_ON_MED5Y,
		N_P_TBV_ON_MED5Y,
		N_PB_PREMIUM_ON_HIST,
		N_DIV_YLD_NTM
FROM DATA_FACTSET WHERE ISIN='FR0000120966' AND DATE='01/12/2013'
--GB0007908733

DROP TABLE #cible
DROP TABLE #euro_poids
DROP TABLE #echelle
*/