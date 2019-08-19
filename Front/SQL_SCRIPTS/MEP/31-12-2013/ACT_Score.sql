
ALTER PROCEDURE ACT_Score_Style

	@DATE As DATETIME

AS


/*
EXECUTE ACT_Score_Style '18/06/2012'
Calcul de score des isin pour les portefeuilles Growth et value en mode ABS
*/


------------------------------------------MARKET = ABS-------------------------------------------
--------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------



SELECT 
	SUM(SXXP) As 'poids',
	SUM(CAPEX_SALES_NTM_W * SXXP)/SUM(SXXP) As 'moy',
	SQRT( SUm(SXXP*SQUARE(CAPEX_SALES_NTM_W))/SUm(SXXP) - SQUARE(SUm(SXXP*CAPEX_SALES_NTM_W)/SUm(SXXP)) ) As 'vol' 
Into #norme1
FROM ACT_DATA_FACTSET
where date=@date and ICB_SUPERSECTOR NOT LIKE '8%' and CAPEX_SALES_NTM IS NOT NULL and SXXP IS NOT NULL


UPDATE ACT_DATA_FACTSET 
SET CAPEX_SALES_NTM_WM = 
	   case when a.ICB_SUPERSECTOR LIKE '8%'	or a.CAPEX_SALES_NTM IS NULL then NULL
       else (CAPEX_SALES_NTM_W - n.moy)/n.vol end
FROM ACT_DATA_FACTSET a, #norme1 n
WHERE a.date=@date  and SXXP IS NOT NULL

--SELECT * FROM #norme1
DROP TABLE #norme1

SELECT 
	SUM(SXXP) As 'poids',
	SUM(EBIT_MARGIN_NTM_W * SXXP)/SUM(SXXP) As 'moy',
	SQRT( SUm(SXXP*SQUARE(EBIT_MARGIN_NTM_W))/SUm(SXXP) - SQUARE(SUm(SXXP*EBIT_MARGIN_NTM_W)/SUm(SXXP)) ) As 'vol' 
Into #norme2
FROM ACT_DATA_FACTSET
where date=@date and ICB_SUPERSECTOR NOT LIKE '8%' and EBIT_MARGIN_NTM IS NOT NULL and SXXP IS NOT NULL

UPDATE ACT_DATA_FACTSET 
SET EBIT_MARGIN_NTM_WM = 
	   case when a.ICB_SUPERSECTOR LIKE '8%' or a.EBIT_MARGIN_NTM IS NULL then NULL
       else (EBIT_MARGIN_NTM_W - n.moy )/ n.vol end
FROM ACT_DATA_FACTSET a, #norme2 n
WHERE a.date=@date and SXXP IS NOT NULL --and a.ICB_SUPERSECTOR NOT LIKE '8%'

DROP TABLE #norme2


/* colonne winsorizer

SELECT SUM(SALES_TREND_5YR), 
		SUM(SALES_RSD ),
		SUM(SALES_GROWTH_NTM ),
		SUM(SALES_GROWTH_STM),
		SUM(EPS_TREND_5YR),
		SUM(EPS_RSD),	
		SUM(EPS_GROWTH_NTM),
		SUM(EPS_GROWTH_STM) As 'faux',
		SUM(EBIT_MARGIN_NTM) AS 'ebit',
		SUM(CAPEX_SALES_NTM),
		SUM(IGROWTH_NTM),	
		SUM(PEG_NTM_INVERSE)
FROM ACT_DATA_FACTSET where date='01/11/2011' and SXXP IS NOT NULL

SELECT SUM(SALES_TREND_5YR_W), 
		SUM(SALES_RSD_W ),
		SUM(SALES_GROWTH_NTM_W ),
		SUM(SALES_GROWTH_STM_W),
		SUM(EPS_TREND_5YR_W),
		SUM(EPS_RSD_W),	
		SUM(EPS_GROWTH_NTM_W),
		SUM(EPS_GROWTH_STM_W) As 'faux',
		SUM(EBIT_MARGIN_NTM_W) AS 'ebit',
		SUM(CAPEX_SALES_NTM_W),
		SUM(IGROWTH_NTM_W),	
		SUM(PEG_NTM_INVERSE_W)
FROM ACT_DATA_FACTSET where date='01/11/2011' and SXXP IS NOT NULL

SELECT	SUM(SALES_TREND_5YR_WM), 
		SUM(SALES_RSD_WM),
		SUM(SALES_GROWTH_NTM_WM),
		SUM(SALES_GROWTH_STM_WM),
		SUM(EPS_TREND_5YR_WM),
		SUM(EPS_RSD_WM),	
		SUM(EPS_GROWTH_NTM_WM),
		SUM(EPS_GROWTH_STM_WM),
		SUM(EBIT_MARGIN_NTM_WM) AS 'ebit',
		SUM(CAPEX_SALES_NTM_WM),
		SUM(IGROWTH_NTM_WM),	
		SUM(PEG_NTM_INVERSE_WM)
FROM ACT_DATA_FACTSET where date='01/11/2011' and SXXP IS NOT NULL
SELECT COMPANY_NAME,Price, SALES_TREND_5YR,SALES_RSD,SALES_GROWTH_NTM,SALES_GROWTH_STM,EPS_TREND_5YR,EPS_RSD,EPS_GROWTH_NTM,EPS_GROWTH_STM,EBIT_MARGIN_NTM,CAPEX_SALES_NTM,IGROWTH_NTM,PEG_NTM_INVERSE from ACT_DATA_FACTSET where date='15/11/2011' order by company_name
SELECT SUM(Growth_TOTAL_M) As 'growth' FROm ACT_DATA_FACTSET where date='01/11/2011' and SXXP IS NOT NULL
*/



-- 1) Portfolio Growth
---------------------------------------------------------------------------------------------------------------
		
create table #poids_ABS_Growth(libelle varchar(120) COLLATE DATABASE_DEFAULT,isin varchar(13) COLLATE DATABASE_DEFAULT, indicateur varchar(60) COLLATE DATABASE_DEFAULT, ponderation float)

insert into #poids_ABS_Growth
SELECT   
	a.COMPANY_NAME,
	a.isin,
    'SALES_TREND_5YR' as indicateur,
	CASE
		WHEN a.SALES_TREND_5YR_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_TREND_5YR' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_TREND_5YR' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'SALES_RSD' as indicateur,
	CASE
		WHEN a.SALES_RSD_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_RSD' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_RSD' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'SALES_GROWTH_NTM' as indicateur,
	CASE 
		WHEN a.SALES_GROWTH_NTM_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_GROWTH_NTM' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_GROWTH_NTM' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'SALES_GROWTH_STM' as indicateur,
	CASE 
		WHEN a.SALES_GROWTH_STM_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_GROWTH_STM' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_GROWTH_STM' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EPS_TREND_5YR' as indicateur,
	CASE 
		WHEN a.EPS_TREND_5YR_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_TREND_5YR' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_TREND_5YR' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EPS_RSD' as indicateur,
	CASE 
		WHEN a.EPS_RSD_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_RSD' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_RSD' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EPS_GROWTH_NTM' as indicateur,
	CASE 
		WHEN a.EPS_GROWTH_NTM_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_GROWTH_NTM' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_GROWTH_NTM' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EPS_GROWTH_STM' as indicateur,
	CASE 
		WHEN a.EPS_GROWTH_STM_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_GROWTH_STM' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_GROWTH_STM' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EBIT_MARGIN_NTM' as indicateur,
	CASE 
		WHEN a.EBIT_MARGIN_NTM_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EBIT_MARGIN_NTM' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EBIT_MARGIN_NTM' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'CAPEX_SALES_NTM' as indicateur,
	CASE 
		WHEN a.CAPEX_SALES_NTM_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='CAPEX_SALES_NTM' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='CAPEX_SALES_NTM' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'IGROWTH_NTM' as indicateur,
	CASE 
		WHEN a.IGROWTH_NTM_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='IGROWTH_NTM' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='IGROWTH_NTM' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date

insert into #poids_ABS_Growth
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PEG_NTM_INVERSE' as indicateur,
	CASE 
		WHEN a.PEG_NTM_INVERSE_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PEG_NTM_INVERSE' and portfolio='Growth')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PEG_NTM_INVERSE' and portfolio='Growth')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL
--select * from #ponderation_par_actif_sector order by libelle


SELECT  
	a.COMPANY_NAME,
	a.isin,
		( COALESCE(SALES_TREND_5YR_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'SALES_TREND_5YR' and isin = a.isin)
		+ COALESCE(SALES_RSD_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'SALES_RSD' and isin = a.isin )
		+ COALESCE(SALES_GROWTH_NTM_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'SALES_GROWTH_NTM' and isin = a.isin )
		+ COALESCE(SALES_GROWTH_STM_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'SALES_GROWTH_STM' and isin = a.isin )
		+ COALESCE(EPS_TREND_5YR_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'EPS_TREND_5YR' and isin = a.isin )
		+ COALESCE(EPS_RSD_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'EPS_RSD' and isin = a.isin )
		+ COALESCE(EPS_GROWTH_NTM_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'EPS_GROWTH_NTM' and isin = a.isin )
		+ COALESCE(EPS_GROWTH_STM_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'EPS_GROWTH_STM' and isin = a.isin )
		+ COALESCE(EBIT_MARGIN_NTM_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'EBIT_MARGIN_NTM' and isin = a.isin )
		+ COALESCE(CAPEX_SALES_NTM_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'CAPEX_SALES_NTM' and isin = a.isin )
		+ COALESCE(IGROWTH_NTM_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'IGROWTH_NTM' and isin = a.isin )
		+ COALESCE(PEG_NTM_INVERSE_WM,0) * (select ponderation from #poids_ABS_Growth where indicateur = 'PEG_NTM_INVERSE' and isin = a.isin) )
		/
		(NULLIF( (select SUM(ponderation) from #poids_ABS_Growth where isin = a.isin) , 0) )
		 AS 'total'	
into #GrowthTotalM
FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL
ORDER BY COMPANY_NAME
--SELECT * FROm #GrowthTotalM order by COMPANY_NAME

UPDATE ACT_DATA_FACTSET 
Set GROWTH_TOTAL_M = total
FROM ACT_DATA_FACTSET a, #GrowthTotalM m
WHERE a.date=@date and m.isin = a.isin and SXXP IS NOT NULL
--SELECT COMPANY_NAME, TOTAL_M FROm ACT_DATA_FACTSET where date=@date


--Supprimer table temporaire
DROP TABLE #GrowthTotalM
DROP TABLE #poids_ABS_Growth



-- 2) Portfolio Value
---------------------------------------------------------------------------------------------------------------
/*
SELECT COMPANY_NAME,ICB_SUPERSECTOR,PE_NTM_INVERSE_W,PE_ON_AVG10Y_INVERSE_W,PB_NTM_INVERSE_W,PB_ON_AVG10Y_INVERSE_W,EV_EBITDA_NTM_INVERSE_W,EV_EBITDA_ON_AVG10Y_INVERSE_W,DIV_YLD_NTM_W,ROE_NTM_W FROM ACT_DATA_FACTSET 
where date='02/11/2011' and ICB_SUPERSECTOR like '8%' 
order by company_name
SELECT SUM(PE_NTM_INVERSE),
		SUM(PE_ON_AVG10Y_INVERSE),
		SUM(PB_NTM_INVERSE),
		SUM(PB_ON_AVG10Y_INVERSE),
		SUM(EV_EBITDA_NTM_INVERSE),
		SUM(EV_EBITDA_ON_AVG10Y_INVERSE),	
		SUM(DIV_YLD_NTM),
		SUM(ROE_NTM)
FROM ACT_DATA_FACTSET where date='02/11/2011'
SELECT SUM(PE_NTM_INVERSE_W),
		SUM(PE_ON_AVG10Y_INVERSE_W),
		SUM(PB_NTM_INVERSE_W),
		SUM(PB_ON_AVG10Y_INVERSE_W),
		SUM(EV_EBITDA_NTM_INVERSE_W),
		SUM(EV_EBITDA_ON_AVG10Y_INVERSE_W),	
		SUM(DIV_YLD_NTM_W),
		SUM(ROE_NTM_W)
FROM ACT_DATA_FACTSET where date='02/11/2011'
SELECT SUM(PE_NTM_INVERSE_WM),
		SUM(PE_ON_AVG10Y_INVERSE_WM),
		SUM(PB_NTM_INVERSE_WM),
		SUM(PB_ON_AVG10Y_INVERSE_WM),
		SUM(EV_EBITDA_NTM_INVERSE_WM),
		SUM(EV_EBITDA_ON_AVG10Y_INVERSE_WM),	
		SUM(DIV_YLD_NTM_WM),
		SUM(ROE_NTM_WM)
FROM ACT_DATA_FACTSET where date='02/11/2011'
SELECT COMPANY_NAME, PE_NTM_INVERSE_WM,PE_ON_AVG10Y_INVERSE_WM,PB_NTM_INVERSE_WM,
		PB_ON_AVG10Y_INVERSE_WM,
		EV_EBITDA_NTM_INVERSE_WM,
		EV_EBITDA_ON_AVG10Y_INVERSE_WM,	
		DIV_YLD_NTM_WM,
		ROE_NTM_WM,VALUE_TOTAL_M		
FROm ACT_DATA_FACTSET where date='02/11/2011' order by company_name
SELECT SUM(VALUE_TOTAL_M) As 'ABS' FROm ACT_DATA_FACTSET where date='02/11/2011'
*/

SELECT 
	SUM(SXXP) As 'poids',
	SUM(EV_EBITDA_NTM_INVERSE_W * SXXP)/SUM(SXXP) As 'moy',
	SQRT( SUm(SXXP*SQUARE(EV_EBITDA_NTM_INVERSE_W))/SUm(SXXP) - SQUARE(SUm(SXXP*EV_EBITDA_NTM_INVERSE_W)/SUm(SXXP)) ) As 'vol' 
Into #norme11
FROM ACT_DATA_FACTSET
where date=@date and ICB_SUPERSECTOR NOT LIKE '8%' and EV_EBITDA_NTM_INVERSE IS NOT NULL and SXXP IS NOT NULL

UPDATE ACT_DATA_FACTSET 
SET EV_EBITDA_NTM_INVERSE_WM = 
	   case when a.ICB_SUPERSECTOR LIKE '8%'	or a.EV_EBITDA_NTM_INVERSE IS NULL then NULL
       else (EV_EBITDA_NTM_INVERSE_W - n.moy)/n.vol end
FROM ACT_DATA_FACTSET a, #norme11 n
WHERE a.date=@date  and SXXP IS NOT NULL
DROP TABLE #norme11

SELECT 
	SUM(SXXP) As 'poids',
	SUM(EV_EBITDA_ON_AVG10Y_INVERSE_W * SXXP)/SUM(SXXP) As 'moy',
	SQRT( SUm(SXXP*SQUARE(EV_EBITDA_ON_AVG10Y_INVERSE_W))/SUm(SXXP) - SQUARE(SUm(SXXP*EV_EBITDA_ON_AVG10Y_INVERSE_W)/SUm(SXXP)) ) As 'vol' 
Into #norme22
FROM ACT_DATA_FACTSET
where date=@date and ICB_SUPERSECTOR NOT LIKE '8%' and EV_EBITDA_ON_AVG10Y_INVERSE IS NOT NULL and SXXP IS NOT NULL

UPDATE ACT_DATA_FACTSET 
SET EV_EBITDA_ON_AVG10Y_INVERSE_WM = 
	   case when a.ICB_SUPERSECTOR LIKE '8%' or a.EV_EBITDA_ON_AVG10Y_INVERSE IS NULL then NULL
       else (EV_EBITDA_ON_AVG10Y_INVERSE_W - n.moy )/ n.vol end
FROM ACT_DATA_FACTSET a, #norme22 n
WHERE a.date=@date and SXXP IS NOT NULL
DROP TABLE #norme22


create table #poids_ABS_Value(libelle varchar(120) COLLATE DATABASE_DEFAULT,isin varchar(13) COLLATE DATABASE_DEFAULT, indicateur varchar(60) COLLATE DATABASE_DEFAULT, ponderation float)

insert into #poids_ABS_Value
SELECT   
	a.COMPANY_NAME,
	a.isin,
    'ROE_NTM' as indicateur,
	CASE
		WHEN a.ROE_NTM_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='ROE_NTM' and portfolio='Value')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='ROE_NTM' and portfolio='Value')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Value
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'DIV_YLD_NTM' as indicateur,
	CASE
		WHEN a.DIV_YLD_NTM_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='DIV_YLD_NTM' and portfolio='Value')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='DIV_YLD_NTM' and portfolio='Value')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Value
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PE_NTM_INVERSE' as indicateur,
	CASE 
		WHEN a.PE_NTM_INVERSE_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_NTM_INVERSE' and portfolio='Value')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_NTM_INVERSE' and portfolio='Value')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Value
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PE_ON_AVG10Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PE_ON_AVG10Y_INVERSE_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_ON_AVG10Y_INVERSE' and portfolio='Value')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_ON_AVG10Y_INVERSE' and portfolio='Value')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Value
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PB_NTM_INVERSE' as indicateur,
	CASE 
		WHEN a.PB_NTM_INVERSE_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_NTM_INVERSE' and portfolio='Value')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_NTM_INVERSE' and portfolio='Value')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL
 
insert into #poids_ABS_Value
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PB_ON_AVG10Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PB_ON_AVG10Y_INVERSE_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_ON_AVG10Y_INVERSE' and portfolio='Value')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_ON_AVG10Y_INVERSE' and portfolio='Value')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Value
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EV_EBITDA_NTM_INVERSE' as indicateur,
	CASE 
		WHEN a.EV_EBITDA_NTM_INVERSE_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EV_EBITDA_NTM_INVERSE' and portfolio='Value')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EV_EBITDA_NTM_INVERSE' and portfolio='Value')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_ABS_Value
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EV_EBITDA_ON_AVG10Y_INVERSE' as indicateur,
	CASE 
		WHEN a.EV_EBITDA_ON_AVG10Y_INVERSE_WM is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EV_EBITDA_ON_AVG10Y_INVERSE' and portfolio='Value')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EV_EBITDA_ON_AVG10Y_INVERSE' and portfolio='Value')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL
--select * from #poids_ABS_Value order by libelle


SELECT  
	a.COMPANY_NAME,
	a.isin,
		( COALESCE(ROE_NTM_WM,0) * (select ponderation from #poids_ABS_Value where indicateur = 'ROE_NTM' and isin = a.isin)
		+ COALESCE(DIV_YLD_NTM_WM,0) * (select ponderation from #poids_ABS_Value where indicateur = 'DIV_YLD_NTM' and isin = a.isin )
		+ COALESCE(PE_NTM_INVERSE_WM,0) * (select ponderation from #poids_ABS_Value where indicateur = 'PE_NTM_INVERSE' and isin = a.isin )
		+ COALESCE(PE_ON_AVG10Y_INVERSE_WM,0) * (select ponderation from #poids_ABS_Value where indicateur = 'PE_ON_AVG10Y_INVERSE' and isin = a.isin )
		+ COALESCE(PB_NTM_INVERSE_WM,0) * (select ponderation from #poids_ABS_Value where indicateur = 'PB_NTM_INVERSE' and isin = a.isin )
		+ COALESCE(PB_ON_AVG10Y_INVERSE_WM,0) * (select ponderation from #poids_ABS_Value where indicateur = 'PB_ON_AVG10Y_INVERSE' and isin = a.isin )
		+ COALESCE(EV_EBITDA_NTM_INVERSE_WM,0) * (select ponderation from #poids_ABS_Value where indicateur = 'EV_EBITDA_NTM_INVERSE' and isin = a.isin )
		+ COALESCE(EV_EBITDA_ON_AVG10Y_INVERSE_WM,0) * (select ponderation from #poids_ABS_Value where indicateur = 'EV_EBITDA_ON_AVG10Y_INVERSE' and isin = a.isin  ))
		/
		(NULLIF( (select SUM(ponderation) from #poids_ABS_Value where isin = a.isin) , 0) )
		 AS 'total'	
into #ValueTotalM
FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL
ORDER BY COMPANY_NAME
--SELECT * FROm #ValueTotalM order by COMPANY_NAME

UPDATE ACT_DATA_FACTSET 
Set VALUE_TOTAL_M = total
FROM ACT_DATA_FACTSET a, #ValueTotalM m
WHERE a.date=@date and m.isin = a.isin and SXXP IS NOT NULL

--Supprimer table temporaire
DROP TABLE #ValueTotalM
DROP TABLE #poids_ABS_Value






--Rank ABS pour tous les portefeuilles 
SELECT 
	a.isin,
	a.COMPANY_NAME, 
	a.GROWTH_TOTAL_M,
	a.VALUE_TOTAL_M,
	NTILE(5) OVER (ORDER BY GROWTH_TOTAL_M DESC) AS 'Growth_Quintile',
	ROW_NUMBER() OVER (ORDER BY GROWTH_TOTAL_M DESC ) AS 'Growth_Ranking',
	NTILE(5) OVER (ORDER BY VALUE_TOTAL_M DESC) AS 'Value_Quintile',
	ROW_NUMBER() OVER (ORDER BY VALUE_TOTAL_M DESC ) AS 'Value_Ranking'
into #quintile_M
FROM ACT_DATA_FACTSET a--, ACT_DATA_LIQUIDITY l 
WHERE 
	a.date=@date  and
	--a.date = l.date and
	--a.GROWTH_TOTAL_M IS NOT NULL and
	--a.isin = l.isin and
	--l.unions IS NOT NULL
	a.LIQUIDITY_TEST IN('X','F')
ORDER bY a.COMPANY_NAME


UPDATE ACT_DATA_FACTSET 
Set GROWTH_QUINTILE_M = q.Growth_Quintile, 
	GROWTH_RANKING_M = q.Growth_Ranking,
	VALUE_QUINTILE_M = q.Value_Quintile, 
	VALUE_RANKING_M = q.Value_Ranking
FROM ACT_DATA_FACTSET a, #quintile_M q
WHERE a.date=@date and q.isin = a.isin and SXXP IS NOT NULL

DROP TABLE #quintile_M

/*
SELECT 
	a.isin As 'Isin', 
	a.COMPANY_NAME As 'Libellé',
	ICB_SUPERSECTOR As 'Code',
	s.libelle As 'Secteur', 
	a.GROWTH_total_m As 'Score',
	a.GROWTH_ranking_m As 'Ranking', 
	a.GROWTH_quintile_m As 'Quintile'
FROM ACT_DATA_FACTSET a
LEFT OUTER JOIN ACT_SUPERSECTOR s ON a.ICB_SUPERSECTOR = id  
WHERE 
	date = '11/11/2011' and 
	GROWTH_quintile_m is not NULL
ORDER BY GROWTH_total_m DESC

SELECT 
	a.isin As 'Isin', 
	a.COMPANY_NAME As 'Libellé',
	ICB_SUPERSECTOR As 'Code',
	s.libelle As 'Secteur', 
	a.total_s As 'Score',
	a.ranking_s As 'Ranking',  
	a.quintile_s As 'Quintile'
FROM ACT_DATA_FACTSET a
LEFT OUTER JOIN ACT_SUPERSECTOR s ON a.ICB_SUPERSECTOR = id  
WHERE 
	date = '17/10/2011' and 
	quintile_m is not NULL
ORDER BY s.libelle,total_s DESC
*/

GO






		






















ALTER PROCEDURE [dbo].[ACT_Score_Blend]



	@DATE As DATETIME

AS




/*
EXECUTE ACT_Score_Blend '02/07/2012'
Calcul de score des isin pour le portefeuille Blend en mode BIC
*/

------------------------------------------MARKET = BIC -------------------------------------------
--------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------

/* test BIC Blend
	SELECT	COMPANY_NAME,ICB_SUPERSECTOR, 
	SALES_GROWTH_NTM,
	SALES_RSD,
	SALES_TREND_5YR,
	EPS_GROWTH_NTM,
	EPS_RSD,
	EPS_TREND_5YR,
	EBIT_MARGIN_NTM,
	EBIT_MARGIN_NTM_5_CV,
	ROE_NTM,
	DIV_YLD_NTM,
	PE_NTM_INVERSE,
	PE_ON_AVG5Y_INVERSE,
	PB_NTM_INVERSE
	PB_ON_AVG5Y_INVERSE,
	GARPN_TOTAL_S
	FROM ACT_DATA_FACTSET WHERE date='18/11/2011' order by company_name
	
	SELECT SUM(SALES_GROWTH_NTM),SUM(SALES_RSD),SUM(SALES_TREND_5YR),SUM(EPS_GROWTH_NTM),SUM(EPS_RSD),SUM(EPS_TREND_5YR),
	SUM(EBIT_MARGIN_NTM),SUM(EBIT_MARGIN_NTM_5_CV),SUM(ROE_NTM),SUM(DIV_YLD_NTM),SUM(PE_NTM_INVERSE),SUM(PE_ON_AVG5Y_INVERSE),
	SUM(PB_NTM_INVERSE),SUM(PB_ON_AVG5Y_INVERSE)
	FROM ACT_DATA_FACTSET WHERE date='18/11/2011' 
	
	SELECT SUM(SALES_GROWTH_NTM_W),SUM(SALES_RSD_W),SUM(SALES_TREND_5YR_W),SUM(EPS_GROWTH_NTM_W),SUM(EPS_RSD_W),SUM(EPS_TREND_5YR_W),
	SUM(EBIT_MARGIN_NTM_W),SUM(EBIT_MARGIN_NTM_5_CV_W),SUM(ROE_NTM_W),SUM(DIV_YLD_NTM_W),SUM(PE_NTM_INVERSE_W),SUM(PE_ON_AVG5Y_INVERSE_W),
	SUM(PB_NTM_INVERSE_W),SUM(PB_ON_AVG5Y_INVERSE_W)
	FROM ACT_DATA_FACTSET WHERE date='18/11/2011'  
	
	SELECT SUM(SALES_GROWTH_NTM_WS),SUM(SALES_RSD_WS),SUM(SALES_TREND_5YR_WS),SUM(EPS_GROWTH_NTM_WS),SUM(EPS_RSD_WS),SUM(EPS_TREND_5YR_WS),
	SUM(EBIT_MARGIN_NTM_WS),SUM(ROE_NTM_WS),SUM(DIV_YLD_NTM_WS),SUM(PE_NTM_INVERSE_WS),SUM(PE_ON_AVG5Y_INVERSE_WS),
	SUM(PB_NTM_INVERSE_WS),SUM(PB_ON_AVG5Y_INVERSE_WS)
	FROM ACT_DATA_FACTSET WHERE date='18/11/2011' 
	
	SELECT SUM(GARPN_TOTAL_S) FROM ACT_DATA_FACTSET WHERE date='18/11/2011' 
*/


--contient pour chaque actif sa composition
create table #poids_BIC_GarpN (libelle varchar(120) COLLATE DATABASE_DEFAULT,isin varchar(13) COLLATE DATABASE_DEFAULT, indicateur varchar(60) COLLATE DATABASE_DEFAULT, ponderation float)
--SELECT * FROM ACT_DATA_FACTSET_COEF
insert into #poids_BIC_GarpN
SELECT   
	a.COMPANY_NAME,
	a.isin,
    'SALES_GROWTH_NTM' as indicateur,
	CASE
		WHEN a.SALES_GROWTH_NTM_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_GROWTH_NTM' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_GROWTH_NTM' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_GROWTH_NTM' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'SALES_RSD' as indicateur,
	CASE
		WHEN a.SALES_RSD_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_RSD' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_RSD' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_RSD' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'SALES_TREND_5YR' as indicateur,
	CASE 
		WHEN a.SALES_TREND_5YR_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_TREND_5YR' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_TREND_5YR' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='SALES_TREND_5YR' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EPS_GROWTH_NTM' as indicateur,
	CASE 
		WHEN a.EPS_GROWTH_NTM_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_GROWTH_NTM' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_GROWTH_NTM' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_GROWTH_NTM' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EPS_RSD' as indicateur,
	CASE 
		WHEN a.EPS_RSD_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_RSD' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_RSD' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_RSD' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EPS_TREND_5YR' as indicateur,
	CASE 
		WHEN a.EPS_TREND_5YR_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_TREND_5YR' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_TREND_5YR' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EPS_TREND_5YR' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EBIT_MARGIN_TREND_5YR' as indicateur,
	CASE 
		WHEN a.EBIT_MARGIN_TREND_5YR_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EBIT_MARGIN_TREND_5YR' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EBIT_MARGIN_TREND_5YR' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EBIT_MARGIN_TREND_5YR' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'EBIT_MARGIN_RSD' as indicateur,
	CASE 
		WHEN a.EBIT_MARGIN_RSD_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EBIT_MARGIN_RSD' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EBIT_MARGIN_RSD' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='EBIT_MARGIN_RSD' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PBT_RWA_TREND_5YR' as indicateur,
	CASE 
		WHEN a.PBT_RWA_TREND_5YR_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PBT_RWA_TREND_5YR' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PBT_RWA_TREND_5YR' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PBT_RWA_TREND_5YR' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PBT_RWA_RSD' as indicateur,
	CASE 
		WHEN a.PBT_RWA_RSD_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PBT_RWA_RSD' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PBT_RWA_RSD' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PBT_RWA_RSD' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'ROE_NTM' as indicateur,
	CASE 
		WHEN a.ROE_NTM_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='ROE_NTM' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='ROE_NTM' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='ROE_NTM' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'DIV_YLD_NTM' as indicateur,
	CASE 
		WHEN a.DIV_YLD_NTM_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='DIV_YLD_NTM' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='DIV_YLD_NTM' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='DIV_YLD_NTM' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PE_NTM_INVERSE' as indicateur,
	CASE 
		WHEN a.PE_NTM_INVERSE_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_NTM_INVERSE' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_NTM_INVERSE' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_NTM_INVERSE' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PE_ON_AVG5Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PE_ON_AVG5Y_INVERSE_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PB_NTM_INVERSE' as indicateur,
	CASE 
		WHEN a.PB_NTM_INVERSE_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_NTM_INVERSE' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_NTM_INVERSE' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_NTM_INVERSE' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PB_ON_AVG5Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PB_ON_AVG5Y_INVERSE_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PE_VS_IND_ON_AVG5Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PE_VS_IND_ON_AVG5Y_INVERSE_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_VS_IND_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_VS_IND_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PE_VS_IND_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL

insert into #poids_BIC_GarpN
SELECT  
	a.COMPANY_NAME,
	a.isin,
    'PB_VS_IND_ON_AVG5Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PB_VS_IND_ON_AVG5Y_INVERSE_WS is NULL THEN 0
		WHEN a.ICB_SUPERSECTOR = '8300' THEN  
		 (SELECT COALESCE(bank,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_VS_IND_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
		WHEN a.ICB_SUPERSECTOR LIKE '8%' THEN  
		 (SELECT COALESCE(finance,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_VS_IND_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
		else
		 (SELECT COALESCE(others,0) FROM ACT_DATA_FACTSET_COEF WHERE date=@date and indicator='PB_VS_IND_ON_AVG5Y_INVERSE' and portfolio='BlendValeur')
	END as ponderation
	FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL


SELECT  
	a.COMPANY_NAME,
	a.isin,
		( COALESCE(SALES_GROWTH_NTM_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'SALES_GROWTH_NTM' and isin = a.isin)
		+ COALESCE(SALES_RSD_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'SALES_RSD' and isin = a.isin )
		+ COALESCE(SALES_TREND_5YR_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'SALES_TREND_5YR' and isin = a.isin )
		+ COALESCE(EPS_GROWTH_NTM_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EPS_GROWTH_NTM' and isin = a.isin )
		+ COALESCE(EPS_RSD_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EPS_RSD' and isin = a.isin )
		+ COALESCE(EPS_TREND_5YR_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EPS_TREND_5YR' and isin = a.isin )
		+ COALESCE(EBIT_MARGIN_TREND_5YR_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EBIT_MARGIN_TREND_5YR' and isin = a.isin )
		+ COALESCE(EBIT_MARGIN_RSD_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EBIT_MARGIN_RSD' and isin = a.isin )
		+ COALESCE(PBT_RWA_TREND_5YR_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PBT_RWA_TREND_5YR' and isin = a.isin )
		+ COALESCE(PBT_RWA_RSD_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PBT_RWA_RSD' and isin = a.isin )
		+ COALESCE(ROE_NTM_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'ROE_NTM' and isin = a.isin )
		+ COALESCE(DIV_YLD_NTM_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'DIV_YLD_NTM' and isin = a.isin )
		+ COALESCE(PE_NTM_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PE_NTM_INVERSE' and isin = a.isin )
		+ COALESCE(PE_ON_AVG5Y_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PE_ON_AVG5Y_INVERSE' and isin = a.isin) 
		+ COALESCE(PB_NTM_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PB_NTM_INVERSE' and isin = a.isin )
		+ COALESCE(PB_ON_AVG5Y_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PB_ON_AVG5Y_INVERSE' and isin = a.isin)
		+ COALESCE(PE_VS_IND_ON_AVG5Y_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PE_VS_IND_ON_AVG5Y_INVERSE' and isin = a.isin) 
		+ COALESCE(PB_VS_IND_ON_AVG5Y_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PB_VS_IND_ON_AVG5Y_INVERSE' and isin = a.isin)	)
		/ (NULLIF( (select SUM(ponderation) from #poids_BIC_GarpN where isin = a.isin) , 0)) AS 'total',
		
		( COALESCE(SALES_GROWTH_NTM_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'SALES_GROWTH_NTM' and isin = a.isin)
		+ COALESCE(SALES_RSD_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'SALES_RSD' and isin = a.isin )
		+ COALESCE(SALES_TREND_5YR_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'SALES_TREND_5YR' and isin = a.isin )
		+ COALESCE(EPS_GROWTH_NTM_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EPS_GROWTH_NTM' and isin = a.isin )
		+ COALESCE(EPS_RSD_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EPS_RSD' and isin = a.isin )
		+ COALESCE(EPS_TREND_5YR_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EPS_TREND_5YR' and isin = a.isin ) )
		/ (NULLIF( (select SUM(ponderation) from #poids_BIC_GarpN where isin = a.isin and indicateur in ('SALES_GROWTH_NTM','SALES_RSD','SALES_TREND_5YR','EPS_GROWTH_NTM','EPS_RSD','EPS_TREND_5YR')) , 0))  AS 'growth',
		
		( COALESCE(EBIT_MARGIN_TREND_5YR_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EBIT_MARGIN_TREND_5YR' and isin = a.isin )
		+ COALESCE(EBIT_MARGIN_RSD_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'EBIT_MARGIN_RSD' and isin = a.isin )
		+ COALESCE(PBT_RWA_TREND_5YR_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PBT_RWA_TREND_5YR' and isin = a.isin )
		+ COALESCE(PBT_RWA_RSD_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PBT_RWA_RSD' and isin = a.isin )
		+ COALESCE(ROE_NTM_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'ROE_NTM' and isin = a.isin ) 	 )
		/ (NULLIF( (select SUM(ponderation) from #poids_BIC_GarpN where isin = a.isin and indicateur IN ('EBIT_MARGIN_TREND_5YR','EBIT_MARGIN_RSD','PBT_RWA_TREND_5YR','PBT_RWA_RSD','ROE_NTM')) , 0)) AS 'yield',	
		
		( COALESCE(DIV_YLD_NTM_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'DIV_YLD_NTM' and isin = a.isin )
		+ COALESCE(PE_NTM_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PE_NTM_INVERSE' and isin = a.isin )
		+ COALESCE(PE_ON_AVG5Y_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PE_ON_AVG5Y_INVERSE' and isin = a.isin) 
		+ COALESCE(PE_VS_IND_ON_AVG5Y_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PE_VS_IND_ON_AVG5Y_INVERSE' and isin = a.isin )
		+ COALESCE(PB_NTM_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PB_NTM_INVERSE' and isin = a.isin )
		+ COALESCE(PB_ON_AVG5Y_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PB_ON_AVG5Y_INVERSE' and isin = a.isin) 
		+ COALESCE(PB_VS_IND_ON_AVG5Y_INVERSE_WS,0) * (select ponderation from #poids_BIC_GarpN where indicateur = 'PE_VS_IND_ON_AVG5Y_INVERSE' and isin = a.isin) )
		/ (NULLIF( (select SUM(ponderation) from #poids_BIC_GarpN where isin = a.isin and indicateur IN ('DIV_YLD_NTM','PE_NTM_INVERSE','PE_ON_AVG5Y_INVERSE','PE_VS_IND_ON_AVG5Y_INVERSE','PB_NTM_INVERSE','PB_ON_AVG5Y_INVERSE','PB_VS_IND_ON_AVG5Y_INVERSE_WS')), 0)) AS 'value'	
into #totalS
FROM ACT_DATA_FACTSET a 
WHERE date=@date and SXXP IS NOT NULL
ORDER BY COMPANY_NAME

UPDATE ACT_DATA_FACTSET 
Set GARPN_TOTAL_S = total,
	GARPN_GROWTH_S = growth,
	GARPN_VALUE_S = value,
	GARPN_YIELD_S = yield
FROM ACT_DATA_FACTSET a, #totalS s
WHERE a.date=@date and s.isin = a.isin and SXXP IS NOT NULL

DROP TABLE #poids_BIC_GarpN
DROP TABLE #totalS