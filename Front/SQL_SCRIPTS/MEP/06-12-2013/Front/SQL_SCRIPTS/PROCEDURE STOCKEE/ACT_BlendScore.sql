USE [E2DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[ACT_BlendScore]    Script Date: 09/09/2013 18:30:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_BlendScore]

	@Date As DATETIME


AS

--EXECUTE ACT_BlendScore '25/07/2012'
--DECLARE @Date AS Datetime 
--SET @Date='05/08/2012'
--SELECT * FROM ACT_DATA_FACTSET_AGR where date='31/07/2012' and indice='SXXP' and FGA_SECTOR IS NOT NULL
/*
SELECT 
	isin,
	company_name,
	icb_supersector,
	SXXP,
	GARPN_TOTAL_S,
	GARPN_GROWTH_S,
	GARPN_VALUE_S,
	GARPN_YIELD_S,
	SALES_GROWTH_NTM,
	SALES_RSD,
	SALES_TREND_5YR,
	EPS_GROWTH_NTM,
	EPS_RSD,
	EPS_TREND_5YR,
	EBIT_MARGIN_NTM,
	PE_VS_IND_ON_AVG5Y,
	--EBIT_MARGIN_NTM_5_CV,
	ROE_NTM,
	DIV_YLD_NTM,
	PE_NTM,
	PE_ON_AVG5Y,
	PB_NTM,
	PB_ON_AVG5Y
INTO #garp
FROM ACT_DATA_FACTSET f
WHERE f.date=@Date
--SELECT * FROM #garp order by icb_supersector 

CREATE TABLE #donnee(
	indicator VARCHAR(120),
	icb_supersector VARCHAR(4),
	value FLOAT,
)

--INSERT INTO #donnee SELECT 'GARPN_TOTAL_S', icb_supersector,		SUM(GARPN_TOTAL_S*SXXP)/SUM(SXXP)			AS 'value' FROM #garp WHERE GARPN_TOTAL_S IS NOT NULL GROUP BY icb_supersector
--INSERT INTO #donnee SELECT 'GARPN_GROWTH_S', icb_supersector,		SUM(GARPN_GROWTH_S*SXXP)/SUM(SXXP)		AS 'value' FROM #garp WHERE GARPN_GROWTH_S IS NOT NULL GROUP BY icb_supersector
--INSERT INTO #donnee SELECT 'GARPN_VALUE_S', icb_supersector,		SUM(GARPN_VALUE_S*SXXP)/SUM(SXXP)			AS 'value' FROM #garp WHERE GARPN_VALUE_S IS NOT NULL GROUP BY icb_supersector
--INSERT INTO #donnee SELECT 'GARPN_YIELD_S', icb_supersector,		SUM(GARPN_YIELD_S*SXXP)/SUM(SXXP)			AS 'value' FROM #garp WHERE GARPN_YIELD_S IS NOT NULL GROUP BY icb_supersector

/*1)On agrege les données moyenne arithmétique ou géométrique */
INSERT INTO #donnee SELECT 'SXXP', icb_supersector,					SUM(SXXP)										AS 'value' FROM #garp GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'SALES_GROWTH_NTM', icb_supersector,		SUM(SALES_GROWTH_NTM*SXXP)/SUM(SXXP)		AS 'value' FROM #garp WHERE SALES_GROWTH_NTM IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'SALES_RSD', icb_supersector,			SUM(SALES_RSD*SXXP)/SUM(SXXP)			AS 'value' FROM #garp WHERE SALES_RSD IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'SALES_TREND_5YR', icb_supersector,		SUM(SALES_TREND_5YR*SXXP)/SUM(SXXP)			AS 'value' FROM #garp WHERE SALES_TREND_5YR IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'EPS_GROWTH_NTM', icb_supersector,		SUM(EPS_GROWTH_NTM*SXXP)/SUM(SXXP)		AS 'value' FROM #garp WHERE EPS_GROWTH_NTM IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'EPS_RSD', icb_supersector,				SUM(EPS_RSD*SXXP)/SUM(SXXP)				AS 'value' FROM #garp WHERE EPS_RSD IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'EPS_TREND_5YR', icb_supersector,		SUM(EPS_TREND_5YR*SXXP)/SUM(SXXP)			AS 'value' FROM #garp WHERE SALES_GROWTH_NTM IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'EBIT_MARGIN_NTM', icb_supersector,		SUM(EBIT_MARGIN_NTM*SXXP)/SUM(SXXP)		AS 'value' FROM #garp WHERE EBIT_MARGIN_NTM IS NOT NULL GROUP BY icb_supersector
--INSERT INTO #donnee SELECT 'EBIT_MARGIN_NTM_5_CV', icb_supersector,	SUM(EBIT_MARGIN_NTM_5_CV*SXXP)/SUM(SXXP)	AS 'value' FROM #garp WHERE EBIT_MARGIN_NTM_5_CV IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'DIV_YLD_NTM', icb_supersector,			SUM(DIV_YLD_NTM*SXXP)/SUM(SXXP)			AS 'value' FROM #garp WHERE DIV_YLD_NTM IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'PE_NTM', icb_supersector,				SUM(SXXP)/SUM(SXXP/PE_NTM)				AS 'value' FROM #garp WHERE PE_NTM IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'PE_ON_AVG5Y', icb_supersector,			SUM(SXXP)/SUM(SXXP/PE_ON_AVG5Y)		AS 'value' FROM #garp WHERE PE_ON_AVG5Y IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'PB_NTM', icb_supersector,				SUM(SXXP)/SUM(SXXP/PB_NTM)				AS 'value' FROM #garp WHERE PB_NTM IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'PB_ON_AVG5Y', icb_supersector,			SUM(SXXP)/SUM(SXXP/PB_ON_AVG5Y)		AS 'value' FROM #garp WHERE PB_ON_AVG5Y IS NOT NULL GROUP BY icb_supersector
INSERT INTO #donnee SELECT 'ROE_NTM', icb_supersector,				0													AS 'value' FROM #garp GROUP BY icb_supersector

/*
UPDATE d 
SET d.value= d2.value/ d3.value
FROM #donnee d
LEFT OUTER JOIN #donnee d2 ON d2.indicator='PE_NTM' and d2.icb_supersector = d.icb_supersector
LEFT OUTER JOIN #donnee d3 ON d3.indicator='PB_NTM' and d3.icb_supersector = d.icb_supersector
WHERE d.indicator='ROE_NTM' 
*/
--SELECT * FROM #donnee

/*2)On refait une translation de la matrice*/
SELECT DISTINCT
		s.libelle as 'Secteur',
		-0.0E-1 as 'GARPN_TOTAL_S', --'Score'-
		-0.0E-1 as 'GARPN_GROWTH_S', --'Croissance'
		-0.0E-1 as 'GARPN_VALUE_S', --'Valeur'
		-0.0E-1 as 'GARPN_YIELD_S', --'Rendement'
		i1.value as 'SXXP',
		i2.value as 'SALES_GROWTH_NTM', --'Cr Ventes'		
		i3.value as 'SALES_RSD', --'Ventes CV'
		i4.value as 'SALES_TREND_5YR', --'Ventes trend'
		i5.value as 'EPS_GROWTH_NTM',  --'Cr BPA'
		i6.value as 'EPS_RSD',  --'BPA CV'
		i7.value as 'EPS_TREND_5YR',  --'BPA trend'
		i8.value as 'EBIT_MARGIN_NTM',  --'Marge EBIT'
		--i9.value as 'EBIT_MARGIN_NTM_5_CV',  --'Marge EBIT CV'
		i13.value/i11.value*100 as 'ROE_NTM',  --'ROE'
		i10.value as 'DIV_YLD_NTM', --'Rdt'
		i11.value as 'PE_NTM', --'PE'
		i12.value as 'PE_ON_AVG5Y', --'PE/m(5)'
		i13.value as 'PB_NTM', --'PB' 
		i14.value as 'PB_ON_AVG5Y', --'PB/m(5)'
		0 as 'coefFinance',
		0 as 'coefOthers'
INTO #agrege
FROM #donnee i1
left outer join ACT_SUPERSECTOR s on s.id = i1.icb_supersector 
left outer join #donnee i2   on i2.icb_supersector  = i1.icb_supersector and i2.indicator  = 'SALES_GROWTH_NTM'
left outer join #donnee i3   on i3.icb_supersector  = i1.icb_supersector and i3.indicator  = 'SALES_RSD'	
left outer join #donnee i4   on i4.icb_supersector  = i1.icb_supersector and i4.indicator  = 'SALES_TREND_5YR'	
left outer join #donnee i5   on i5.icb_supersector  = i1.icb_supersector and i5.indicator  = 'EPS_GROWTH_NTM'	
left outer join #donnee i6   on i6.icb_supersector  = i1.icb_supersector and i6.indicator  = 'EPS_RSD'	
left outer join #donnee i7   on i7.icb_supersector  = i1.icb_supersector and i7.indicator  = 'EPS_TREND_5YR'	
left outer join #donnee i8   on i8.icb_supersector  = i1.icb_supersector and i8.indicator  = 'EBIT_MARGIN_NTM'	
--left outer join #donnee i9   on i9.icb_supersector  = i1.icb_supersector and i9.indicator  = 'EBIT_MARGIN_NTM_5_CV'	
left outer join #donnee i10  on i10.icb_supersector = i1.icb_supersector and i10.indicator = 'DIV_YLD_NTM'	
left outer join #donnee i11  on i11.icb_supersector = i1.icb_supersector and i11.indicator = 'PE_NTM'
left outer join #donnee i12  on i12.icb_supersector = i1.icb_supersector and i12.indicator = 'PE_ON_AVG5Y'
left outer join #donnee i13  on i13.icb_supersector = i1.icb_supersector and i13.indicator = 'PB_NTM'
left outer join #donnee i14  on i14.icb_supersector = i1.icb_supersector and i14.indicator = 'PB_ON_AVG5Y'	
where i1.indicator = 'SXXP' 
--SELECT * from #agrege


/*3)On calcul la moyenne et l'écart type de chaque indicateur*/
SELECT	
	SUM(SXXP*SALES_GROWTH_NTM)/SUM(SXXP) As 'E_SALES_GROWTH_NTM',
	SQRT( SUM(SXXP*SQUARE(SALES_GROWTH_NTM))/SUM(SXXP) - SQUARE(SUM(SXXP*SALES_GROWTH_NTM)/SUM(SXXP)) ) As 'O_SALES_GROWTH_NTM', 
	SUM(SXXP*SALES_RSD)/SUM(SXXP) As 'E_SALES_RSD',
	SQRT( SUM(SXXP*SQUARE(SALES_RSD))/SUM(SXXP) - SQUARE(SUM(SXXP*SALES_RSD)/SUM(SXXP)) ) As 'O_SALES_RSD', 
	SUM(SXXP*SALES_TREND_5YR)/SUM(SXXP) As 'E_SALES_TREND_5YR',
	SQRT( SUM(SXXP*SQUARE(SALES_TREND_5YR))/SUM(SXXP) - SQUARE(SUM(SXXP*SALES_TREND_5YR)/SUM(SXXP)) ) As 'O_SALES_TREND_5YR', 
	SUM(SXXP*EPS_GROWTH_NTM)/SUM(SXXP) As 'E_EPS_GROWTH_NTM',
	SQRT( SUM(SXXP*SQUARE(EPS_GROWTH_NTM))/SUM(SXXP) - SQUARE(SUM(SXXP*EPS_GROWTH_NTM)/SUM(SXXP)) ) As 'O_EPS_GROWTH_NTM', 
	SUM(SXXP*EPS_RSD)/SUM(SXXP) As 'E_EPS_RSD',
	SQRT( SUM(SXXP*SQUARE(EPS_RSD))/SUM(SXXP) - SQUARE(SUM(SXXP*EPS_RSD)/SUM(SXXP)) ) As 'O_EPS_RSD', 
	SUM(SXXP*EPS_TREND_5YR)/SUM(SXXP) As 'E_EPS_TREND_5YR',
	SQRT( SUM(SXXP*SQUARE(EPS_TREND_5YR))/SUM(SXXP) - SQUARE(SUM(SXXP*EPS_TREND_5YR)/SUM(SXXP)) ) As 'O_EPS_TREND_5YR', 
	SUM(SXXP*EBIT_MARGIN_NTM)/SUM(SXXP) As 'E_EBIT_MARGIN_NTM',
	SQRT( SUM(SXXP*SQUARE(EBIT_MARGIN_NTM))/SUM(SXXP) - SQUARE(SUM(SXXP*EBIT_MARGIN_NTM)/SUM(SXXP)) ) As 'O_EBIT_MARGIN_NTM', 
	--SUM(SXXP*EBIT_MARGIN_NTM_5_CV)/SUM(SXXP) As 'E_EBIT_MARGIN_NTM_5_CV',
	--SQRT( SUM(SXXP*SQUARE(EBIT_MARGIN_NTM_5_CV))/SUM(SXXP) - SQUARE(SUM(SXXP*EBIT_MARGIN_NTM_5_CV)/SUM(SXXP)) ) As 'O_EBIT_MARGIN_NTM_5_CV', 
	SUM(SXXP*ROE_NTM)/SUM(SXXP) As 'E_ROE_NTM',
	SQRT( SUM(SXXP*SQUARE(ROE_NTM))/SUM(SXXP) - SQUARE(SUM(SXXP*ROE_NTM)/SUM(SXXP)) ) As 'O_ROE_NTM', 
	SUM(SXXP*DIV_YLD_NTM)/SUM(SXXP) As 'E_DIV_YLD_NTM',
	SQRT( SUM(SXXP*SQUARE(DIV_YLD_NTM))/SUM(SXXP) - SQUARE(SUM(SXXP*DIV_YLD_NTM)/SUM(SXXP)) ) As 'O_DIV_YLD_NTM', 
	SUM(SXXP*PE_NTM)/SUM(SXXP) As 'E_PE_NTM',
	SQRT( SUM(SXXP*SQUARE(PE_NTM))/SUM(SXXP) - SQUARE(SUM(SXXP*PE_NTM)/SUM(SXXP)) ) As 'O_PE_NTM', 
	SUM(SXXP*PE_ON_AVG5Y)/SUM(SXXP) As 'E_PE_ON_AVG5Y',
	SQRT( SUM(SXXP*SQUARE(PE_ON_AVG5Y))/SUM(SXXP) - SQUARE(SUM(SXXP*PE_ON_AVG5Y)/SUM(SXXP)) ) As 'O_PE_ON_AVG5Y', 
	SUM(SXXP*PB_NTM)/SUM(SXXP) As 'E_PB_NTM',
	SQRT( SUM(SXXP*SQUARE(PB_NTM))/SUM(SXXP) - SQUARE(SUM(SXXP*PB_NTM)/SUM(SXXP)) ) As 'O_PB_NTM',
	SUM(SXXP*PB_ON_AVG5Y)/SUM(SXXP) As 'E_PB_ON_AVG5Y',
	SQRT( SUM(SXXP*SQUARE(PB_ON_AVG5Y))/SUM(SXXP) - SQUARE(SUM(SXXP*PB_ON_AVG5Y)/SUM(SXXP)) ) As 'O_PB_ON_AVG5Y'
Into #EspvsVari
FROM #agrege

/*4)On normalise les indicateurs*/
UPDATE #agrege 
SET SALES_GROWTH_NTM = (SALES_GROWTH_NTM - ev.E_SALES_GROWTH_NTM )/ ev.O_SALES_GROWTH_NTM,
	SALES_RSD = (SALES_RSD - ev.E_SALES_RSD )/ ev.O_SALES_RSD,
	SALES_TREND_5YR =	(SALES_TREND_5YR - ev.E_SALES_TREND_5YR )/ ev.O_SALES_TREND_5YR,
	EPS_GROWTH_NTM = (EPS_GROWTH_NTM - ev.E_EPS_GROWTH_NTM )/ ev.O_EPS_GROWTH_NTM,	
	EPS_RSD = (EPS_RSD - ev.E_EPS_RSD )/ ev.O_EPS_RSD,	
	EPS_TREND_5YR = (EPS_TREND_5YR - ev.E_EPS_TREND_5YR )/ ev.O_EPS_TREND_5YR,	
	EBIT_MARGIN_NTM = (EBIT_MARGIN_NTM - ev.E_EBIT_MARGIN_NTM )/ ev.O_EBIT_MARGIN_NTM,	
	--EBIT_MARGIN_NTM_5_CV = (EBIT_MARGIN_NTM_5_CV - ev.E_EBIT_MARGIN_NTM_5_CV )/ ev.O_EBIT_MARGIN_NTM_5_CV,
	ROE_NTM = (ROE_NTM - ev.E_ROE_NTM )/ ev.O_ROE_NTM,	
	DIV_YLD_NTM = (DIV_YLD_NTM - ev.E_DIV_YLD_NTM )/ ev.O_DIV_YLD_NTM,	
	PE_NTM = (PE_NTM - ev.E_PE_NTM )/ ev.O_PE_NTM, 
	PE_ON_AVG5Y = (PE_ON_AVG5Y - ev.E_PE_ON_AVG5Y )/ ev.O_PE_ON_AVG5Y,
	PB_NTM = (PB_NTM - ev.E_PB_NTM )/ ev.O_PB_NTM,
	PB_ON_AVG5Y = (PB_ON_AVG5Y - ev.E_PB_ON_AVG5Y )/ ev.O_PB_ON_AVG5Y
FROM #EspvsVari ev



/*5)On calcul les scores*/
/*UPDATE #agrege
SET 
	GARPN_TOTAL_S = 
	GARPN_GROWTH_S = 
	GARPN_VALUE_S = 
	GARPN_YIELD_S = 
FROM #ACT_DATA_FACTSET_COEF c
WHERE c.date=@date and 
*/
update #agrege
set coefFinance = 1 
from #agrege
where Secteur in ('Banques')


update #agrege
set coefOthers = 1
from #agrege
where Secteur not in ('Banques')

/*5)Calcul des scores*/
update #agrege
set GARPN_TOTAL_S = 
		( ( a.coefFinance * c1.FINANCE + a.coefOthers * c1.OTHERS) * SALES_GROWTH_NTM  + 
          ( a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS) * SALES_RSD  +
          ( a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS) * SALES_TREND_5YR +
          ( a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS) * EPS_GROWTH_NTM +
          ( a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS) * EPS_RSD +
          ( a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS) * EPS_TREND_5YR +
          ( a.coefFinance * c7.FINANCE + a.coefOthers * c7.OTHERS) * EBIT_MARGIN_NTM +
          --( a.coefFinance * c8.FINANCE + a.coefOthers * c8.OTHERS) * EBIT_MARGIN_NTM_5_CV +
          ( a.coefFinance * c9.FINANCE + a.coefOthers * c9.OTHERS) * ROE_NTM +
          ( a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS) * DIV_YLD_NTM +
          ( a.coefFinance /c11.FINANCE + a.coefOthers /c11.OTHERS) * PE_NTM +
          ( a.coefFinance /c12.FINANCE + a.coefOthers /c12.OTHERS) * PE_ON_AVG5Y +
          ( a.coefFinance /c13.FINANCE + a.coefOthers /c13.OTHERS) * PB_NTM +
          ( a.coefFinance /c14.FINANCE + a.coefOthers /c14.OTHERS) * PB_ON_AVG5Y   ) 
        / ( a.coefFinance * c1.FINANCE + a.coefOthers * c1.OTHERS + a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS + a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS + a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS + a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS + a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS + a.coefFinance * c7.FINANCE + a.coefOthers * c7.OTHERS + a.coefFinance * c8.FINANCE + a.coefOthers * c8.OTHERS + a.coefFinance * c9.FINANCE + a.coefOthers * c9.OTHERS + a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS + a.coefFinance / c11.FINANCE + a.coefOthers / c11.OTHERS + a.coefFinance / c12.FINANCE + a.coefOthers / c12.OTHERS + a.coefFinance / c13.FINANCE + a.coefOthers / c13.OTHERS + a.coefFinance / c14.FINANCE + a.coefOthers / c14.OTHERS),
    
    GARPN_GROWTH_S = 
		( ( a.coefFinance * c1.FINANCE + a.coefOthers * c1.OTHERS) * SALES_GROWTH_NTM  + 
          ( a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS) * SALES_RSD  +
          ( a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS) * SALES_TREND_5YR +
          ( a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS) * EPS_GROWTH_NTM +
          ( a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS) * EPS_RSD +
          ( a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS) * EPS_TREND_5YR  ) 
        / ( a.coefFinance * c1.FINANCE + a.coefOthers * c1.OTHERS + a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS + a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS + a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS + a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS + a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS),
    
    GARPN_VALUE_S = 
		( ( a.coefFinance * c7.FINANCE + a.coefOthers * c7.OTHERS) * EBIT_MARGIN_NTM +
          --( a.coefFinance * c8.FINANCE + a.coefOthers * c8.OTHERS) * EBIT_MARGIN_NTM_5_CV +
          ( a.coefFinance * c9.FINANCE + a.coefOthers * c9.OTHERS) * ROE_NTM   ) 
        / ( a.coefFinance * c7.FINANCE + a.coefOthers * c7.OTHERS + a.coefFinance * c8.FINANCE + a.coefOthers * c8.OTHERS + a.coefFinance * c9.FINANCE + a.coefOthers * c9.OTHERS ),
    
    GARPN_YIELD_S = 
		( ( a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS) * DIV_YLD_NTM +
          ( a.coefFinance /c11.FINANCE + a.coefOthers /c11.OTHERS) * PE_NTM +
          ( a.coefFinance /c12.FINANCE + a.coefOthers /c12.OTHERS) * PE_ON_AVG5Y +
          ( a.coefFinance /c13.FINANCE + a.coefOthers /c13.OTHERS) * PB_NTM +
          ( a.coefFinance /c14.FINANCE + a.coefOthers /c14.OTHERS) * PB_ON_AVG5Y   ) 
        / ( a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS + a.coefFinance / c11.FINANCE + a.coefOthers / c11.OTHERS + a.coefFinance / c12.FINANCE + a.coefOthers / c12.OTHERS + a.coefFinance / c13.FINANCE + a.coefOthers / c13.OTHERS + a.coefFinance / c14.FINANCE + a.coefOthers / c14.OTHERS)
    
from  #agrege as a ,
ACT_DATA_FACTSET_COEF as c1 ,ACT_DATA_FACTSET_COEF as c2 ,ACT_DATA_FACTSET_COEF as c3, ACT_DATA_FACTSET_COEF as c4 ,ACT_DATA_FACTSET_COEF as c5, ACT_DATA_FACTSET_COEF as c6 ,ACT_DATA_FACTSET_COEF as c7,ACT_DATA_FACTSET_COEF as c8 ,ACT_DATA_FACTSET_COEF as c9,ACT_DATA_FACTSET_COEF as c10 ,ACT_DATA_FACTSET_COEF as c11,ACT_DATA_FACTSET_COEF as c12 ,ACT_DATA_FACTSET_COEF as c13 ,ACT_DATA_FACTSET_COEF as c14
where c1.indicator = 'SALES_GROWTH_NTM' and  c1.date= @date and c1.portfolio='Garp New' 
	and c2.indicator = 'SALES_RSD' and  c2.date= @date and c2.portfolio='Garp New' 
	and c3.indicator = 'SALES_TREND_5YR' and  c3.date= @date and c3.portfolio='Garp New' 
	and c4.indicator = 'EPS_GROWTH_NTM' and  c4.date= @date and c4.portfolio='Garp New' 
	and c5.indicator = 'EPS_RSD' and  c5.date= @date and c5.portfolio='Garp New' 
	and c6.indicator = 'EPS_TREND_5YR' and  c6.date= @date and c6.portfolio='Garp New' 
	and c7.indicator = 'EBIT_MARGIN_NTM' and  c7.date= @date and c7.portfolio='Garp New' 
	--and c8.indicator = 'EBIT_MARGIN_NTM_5_CV' and  c8.date= @date and c8.portfolio='Garp New' 
	and c9.indicator = 'ROE_NTM' and  c9.date= @date and c9.portfolio='Garp New' 	
	and c10.indicator = 'DIV_YLD_NTM' and  c10.date= @date and c10.portfolio='Garp New' 
	and c11.indicator = 'PE_NTM_INVERSE' and  c11.date= @date and c11.portfolio='Garp New' 
	and c12.indicator = 'PE_ON_AVG5Y_INVERSE' and  c12.date= @date and c12.portfolio='Garp New' 
	and c13.indicator = 'PB_NTM_INVERSE' and  c13.date= @date and c13.portfolio='Garp New' 
	and c14.indicator = 'PB_ON_AVG5Y_INVERSE' and  c14.date= @date and c14.portfolio='Garp New' 							


select  
		secteur AS 'Secteur',
		GARPN_TOTAL_S AS 'Score',
		GARPN_GROWTH_S AS 'Croissance',
		GARPN_VALUE_S AS 'Valeur',
		GARPN_YIELD_S AS 'Rendement',
		--SXXP AS 'Poids',
		SALES_GROWTH_NTM AS 'Cr Ventes',		
		SALES_RSD AS 'Ventes CV',
		SALES_TREND_5YR AS 'Ventes trend',
		EPS_GROWTH_NTM AS 'Cr BPA',
		EPS_RSD AS 'BPA CV',
		EPS_TREND_5YR AS 'BPA trend',
		EBIT_MARGIN_NTM AS 'Marge EBIT',
		--EBIT_MARGIN_NTM_5_CV AS 'Marge EBIT CV',
		ROE_NTM AS 'ROE',
		DIV_YLD_NTM AS 'Rdt',
		PE_NTM AS 'PE',
		PE_ON_AVG5Y AS 'PE/m(5)',
		PB_NTM AS 'PB', 
		PB_ON_AVG5Y AS 'PB/m(5)' 
from #agrege

--SELECT * FROM ACT_DATA_FACTSET_COEF WHERE date= '14/02/2012' and portfolio='Garp New'

DROP TABLE #garp 
DROP TABLE #donnee
DROP TABLE #agrege
DROP TABLE #EspvsVari

*/

--Execute ACT_BlendScore '06/07/2012'
--DECLARE @Date AS Datetime 
--SET @Date='02/07/2013'

--TODO REMPLACE ICB_SECTOR par FGA_SECTOR


--1)On remet les indicateurs par ordre de grandeur (plus grand mieux c'est) 
SELECT DISTINCT
		FGA_SECTOR,
		-0.0E-1 as 'BLEND_TOTAL', --'Score'
		-0.0E-1 as 'BLEND_GROWTH', --'Croissance'
		-0.0E-1 as 'BLEND_VALUE', --'Valeur'
		-0.0E-1 as 'BLEND_YIELD', --'Rendement'
		SXXP,
		SALES_GROWTH_NTM, --'Cr Ventes'		
		SALES_RSD, --'Ventes CV'
		SALES_TREND_5YR, --'Ventes trend'
		EPS_GROWTH_NTM,  --'Cr BPA'
		EPS_RSD,  --'BPA CV'
		EPS_TREND_5YR,  --'BPA trend'
		CASE WHEN FGA_SECTOR='25' THEN PBT_RWA_TREND_5YR ELSE EBIT_MARGIN_TREND_5YR END AS EBIT_MARGIN_TREND_5YR,  --'Marge EBIT'
		CASE WHEN FGA_SECTOR='25' THEN PBT_RWA_RSD ELSE EBIT_MARGIN_RSD END AS EBIT_MARGIN_RSD,
		CASE WHEN PE_NTM <= 0 THEN NULL ELSE PB_NTM/PE_NTM END As ROE_NTM,  --'ROE'
		DIV_YLD_NTM, --'Rdt'
		CASE WHEN PE_NTM <= 0 THEN NULL ELSE 1/PE_NTM END As PE_NTM_INVERSE, --'PE'
		CASE WHEN PE_ON_AVG5Y <= 0 THEN NULL ELSE 1/PE_ON_AVG5Y END As PE_ON_AVG5Y_INVERSE, --'PE/m(5)'
		CASE WHEN PE_VS_IND_ON_AVG5Y <= 0 THEN NULL ELSE 1/PE_VS_IND_ON_AVG5Y END As PE_VS_IND_ON_AVG5Y_INVERSE,
		CASE WHEN PB_NTM <= 0 THEN NULL ELSE 1/PB_NTM END As PB_NTM_INVERSE, --'PB' 
		CASE WHEN PB_ON_AVG5Y <= 0 THEN NULL ELSE 1/PB_ON_AVG5Y END As PB_ON_AVG5Y_INVERSE, --'PB/m(5)'
		CASE WHEN PB_VS_IND_ON_AVG5Y <= 0 THEN NULL ELSE 1/PB_VS_IND_ON_AVG5Y END As PB_VS_IND_ON_AVG5Y_INVERSE
		--0 as 'coefFinance',
		--0 as 'coefOthers',
		--0 as 'coefBank'
INTO #agrege
FROM ACT_DATA_FACTSET_AGR
WHERE date=@date and ICB_SUPERSECTOR IS NOT NULL and FGA_SECTOR IS NOT NULL and indice='SXXP'
--ORDER BY FGA_SECTOR

--Drop table #agrege

--select * from #agrege
--select * from ACT_DATA_FACTSET_AGR
--WHERE fga_sector=25
-- table with SXXP/Criteria
-- DROP TABLE #SXXPEspVsVari
-- DROP TABLE #MidSXXPEspVsVari

SELECT
	FGA_SECTOR,
	CASE WHEN SALES_GROWTH_NTM IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_SALES_GROWTH_NTM',
	CASE WHEN SALES_RSD IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_SALES_RSD',
	CASE WHEN SALES_TREND_5YR IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_SALES_TREND_5YR',
	CASE WHEN EPS_GROWTH_NTM IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_EPS_GROWTH_NTM',
	CASE WHEN EPS_RSD IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_EPS_RSD',
	CASE WHEN EPS_TREND_5YR IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_EPS_TREND_5YR',
	CASE WHEN EBIT_MARGIN_TREND_5YR IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_EBIT_MARGIN_TREND_5YR',
	CASE WHEN EBIT_MARGIN_RSD IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_EBIT_MARGIN_RSD',
	--CASE WHEN PBT_RWA_TREND_5YR IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_PBT_RWA_TREND_5YR',
	--CASE WHEN PBT_RWA_RSD IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_PBT_RWA_RSD',
	CASE WHEN ROE_NTM IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_ROE_NTM',
	CASE WHEN DIV_YLD_NTM IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_DIV_YLD_NTM',
	CASE WHEN PE_NTM_INVERSE IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_PE_NTM_INVERSE',
	CASE WHEN PE_ON_AVG5Y_INVERSE IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_PE_ON_AVG5Y_INVERSE',
	CASE WHEN PE_VS_IND_ON_AVG5Y_INVERSE IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_PE_VS_IND_ON_AVG5Y_INVERSE',
	CASE WHEN PB_NTM_INVERSE IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_PB_NTM_INVERSE',
	CASE WHEN PB_ON_AVG5Y_INVERSE IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_PB_ON_AVG5Y_INVERSE',
	CASE WHEN PB_VS_IND_ON_AVG5Y_INVERSE IS NULL THEN NULL ELSE SXXP END AS 'SXXP_O_PB_VS_IND_ON_AVG5Y_INVERSE'
INTO #SXXPEspVsVari
FROM #agrege

--SELECT * FROM #agrege
--SELECT * FROM #SXXPEspVsVari

-- DROP TABLE #MidSXXPEspVsVari
	
SELECT
	agr.FGA_SECTOR,
	SALES_GROWTH_NTM - (SELECT SUM(SXXP_O_SALES_GROWTH_NTM * SALES_GROWTH_NTM) / SUM(SXXP_O_SALES_GROWTH_NTM) 
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_SALES_GROWTH_NTM',
	SALES_RSD - (SELECT SUM(SXXP_O_SALES_RSD * SALES_RSD) / SUM(SXXP_O_SALES_RSD)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_SALES_RSD',
	SALES_TREND_5YR - (SELECT SUM(SXXP_O_SALES_TREND_5YR * SALES_TREND_5YR) / SUM(SXXP_O_SALES_TREND_5YR)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_SALES_TREND_5YR',
	EPS_GROWTH_NTM - (SELECT SUM(SXXP_O_EPS_GROWTH_NTM * EPS_GROWTH_NTM) / SUM(SXXP_O_EPS_GROWTH_NTM)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_EPS_GROWTH_NTM',
	EPS_RSD - (SELECT SUM(SXXP_O_EPS_RSD * EPS_RSD) / SUM(SXXP_O_EPS_RSD)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_EPS_RSD',
	EPS_TREND_5YR - (SELECT SUM(SXXP_O_EPS_TREND_5YR * EPS_TREND_5YR) / SUM(SXXP_O_EPS_TREND_5YR)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_EPS_TREND_5YR',
	EBIT_MARGIN_TREND_5YR - (SELECT SUM(SXXP_O_EBIT_MARGIN_TREND_5YR * EBIT_MARGIN_TREND_5YR) / SUM(SXXP_O_EBIT_MARGIN_TREND_5YR)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_EBIT_MARGIN_TREND_5YR',
	EBIT_MARGIN_RSD - (SELECT SUM(SXXP_O_EBIT_MARGIN_RSD * EBIT_MARGIN_RSD) / SUM(SXXP_O_EBIT_MARGIN_RSD)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_EBIT_MARGIN_RSD',
	-- PBT_RWA_TREND_5YR - (SELECT SUM(SXXP_O_PBT_RWA_TREND_5YR * PBT_RWA_TREND_5YR) / SUM(SXXP_O_PBT_RWA_TREND_5YR)
		--				FROM #agrege agr 
		--				INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
		--				) AS 'SXXP_MID_O_PBT_RWA_TREND_5YR',
	-- PBT_RWA_RSD - (SELECT SUM(SXXP_O_PBT_RWA_RSD * PBT_RWA_RSD) / SUM(SXXP_O_PBT_RWA_RSD)
		--				FROM #agrege agr 
		--				INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
		--				) AS 'SXXP_MID_O_PBT_RWA_RSD',
	ROE_NTM - (SELECT SUM(SXXP_O_ROE_NTM * ROE_NTM) / SUM(SXXP_O_ROE_NTM)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_ROE_NTM',
	DIV_YLD_NTM - (SELECT SUM(SXXP_O_DIV_YLD_NTM * DIV_YLD_NTM) / SUM(SXXP_O_DIV_YLD_NTM)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_DIV_YLD_NTM',
	PE_NTM_INVERSE - (SELECT SUM(SXXP_O_PE_NTM_INVERSE * PE_NTM_INVERSE) / SUM(SXXP_O_PE_NTM_INVERSE)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_PE_NTM_INVERSE',
	PE_ON_AVG5Y_INVERSE - (SELECT SUM(SXXP_O_PE_ON_AVG5Y_INVERSE * PE_ON_AVG5Y_INVERSE) / SUM(SXXP_O_PE_ON_AVG5Y_INVERSE)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_PE_ON_AVG5Y_INVERSE',
	PE_VS_IND_ON_AVG5Y_INVERSE - (SELECT SUM(SXXP_O_PE_VS_IND_ON_AVG5Y_INVERSE * PE_VS_IND_ON_AVG5Y_INVERSE) / SUM(SXXP_O_PE_VS_IND_ON_AVG5Y_INVERSE)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_PE_VS_IND_ON_AVG5Y_INVERSE',
	PB_NTM_INVERSE - (SELECT SUM(SXXP_O_PB_NTM_INVERSE * PB_NTM_INVERSE) / SUM(SXXP_O_PB_NTM_INVERSE)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_PB_NTM_INVERSE',
	PB_ON_AVG5Y_INVERSE - (SELECT SUM(SXXP_O_PB_ON_AVG5Y_INVERSE * PB_ON_AVG5Y_INVERSE) / SUM(SXXP_O_PB_ON_AVG5Y_INVERSE)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_PB_ON_AVG5Y_INVERSE',
	PB_VS_IND_ON_AVG5Y_INVERSE - (SELECT SUM(SXXP_O_PB_VS_IND_ON_AVG5Y_INVERSE * PB_VS_IND_ON_AVG5Y_INVERSE) / SUM(SXXP_O_PB_VS_IND_ON_AVG5Y_INVERSE)
						FROM #agrege agr 
						INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
						) AS 'SXXP_MID_O_PB_VS_IND_ON_AVG5Y_INVERSE'
INTO
	#MidSXXPEspVsVari
FROM
	#agrege agr
	INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
	
--SELECT * FROM #MidSXXPEspVsVari




/*2)On calcul la moyenne et l'écart type de chaque indicateur*/
-- E_ = espérance || O_ = Ecart type
-- DROP TABLE #EspVsVari

SELECT
	SUM(SXXP_O_SALES_GROWTH_NTM * SALES_GROWTH_NTM)/SUM(SXXP_O_SALES_GROWTH_NTM) As 'E_SALES_GROWTH_NTM',
	SQRT(1/SUM(SXXP_O_SALES_GROWTH_NTM) * SUM(SXXP_O_SALES_GROWTH_NTM * (SQUARE(SXXP_MID_O_SALES_GROWTH_NTM)))) As 'O_SALES_GROWTH_NTM',
	SUM(SXXP_O_SALES_RSD * SALES_RSD)/SUM(SXXP_O_SALES_RSD) As 'E_SALES_RSD',
	SQRT(1/SUM(SXXP_O_SALES_RSD) * SUM(SXXP_O_SALES_RSD * (SQUARE(SXXP_MID_O_SALES_RSD)))) As 'O_SALES_RSD',
	SUM(SXXP_O_SALES_TREND_5YR * SALES_TREND_5YR)/SUM(SXXP_O_SALES_TREND_5YR) As 'E_SALES_TREND_5YR',
	SQRT(1/SUM(SXXP_O_SALES_TREND_5YR) * SUM(SXXP_O_SALES_TREND_5YR * (SQUARE(SXXP_MID_O_SALES_TREND_5YR)))) As 'O_SALES_TREND_5YR',
	SUM(SXXP_O_EPS_GROWTH_NTM * EPS_GROWTH_NTM)/SUM(SXXP_O_EPS_GROWTH_NTM) As 'E_EPS_GROWTH_NTM',
	SQRT(1/SUM(SXXP_O_EPS_GROWTH_NTM) * SUM(SXXP_O_EPS_GROWTH_NTM * (SQUARE(SXXP_MID_O_EPS_GROWTH_NTM)))) As 'O_EPS_GROWTH_NTM',
	SUM(SXXP_O_EPS_RSD * EPS_RSD)/SUM(SXXP_O_EPS_RSD) As 'E_EPS_RSD',
	SQRT(1/SUM(SXXP_O_EPS_RSD) * SUM(SXXP_O_EPS_RSD * (SQUARE(SXXP_MID_O_EPS_RSD)))) As 'O_EPS_RSD',
	SUM(SXXP_O_EPS_TREND_5YR * EPS_TREND_5YR)/SUM(SXXP_O_EPS_TREND_5YR) As 'E_EPS_TREND_5YR',
	SQRT(1/SUM(SXXP_O_EPS_TREND_5YR) * SUM(SXXP_O_EPS_TREND_5YR * (SQUARE(SXXP_MID_O_EPS_TREND_5YR)))) As 'O_EPS_TREND_5YR',
	SUM(SXXP_O_EBIT_MARGIN_TREND_5YR * EBIT_MARGIN_TREND_5YR)/SUM(SXXP_O_EBIT_MARGIN_TREND_5YR) As 'E_EBIT_MARGIN_TREND_5YR',
	SQRT(1/SUM(SXXP_O_EBIT_MARGIN_TREND_5YR) * SUM(SXXP_O_EBIT_MARGIN_TREND_5YR * (SQUARE(SXXP_MID_O_EBIT_MARGIN_TREND_5YR)))) As 'O_EBIT_MARGIN_TREND_5YR',
	SUM(SXXP_O_EBIT_MARGIN_RSD * EBIT_MARGIN_RSD)/SUM(SXXP_O_EBIT_MARGIN_RSD) As 'E_EBIT_MARGIN_RSD',
	SQRT(1/SUM(SXXP_O_EBIT_MARGIN_RSD) * SUM(SXXP_O_EBIT_MARGIN_RSD * (SQUARE(SXXP_MID_O_EBIT_MARGIN_RSD)))) As 'O_EBIT_MARGIN_RSD',
	--SUM(SXXP_O_PBT_RWA_TREND_5YR * PBT_RWA_TREND_5YR)/SUM(SXXP_O_PBT_RWA_TREND_5YR) As 'E_PBT_RWA_TREND_5YR',
	--SQRT(1/SUM(SXXP_O_PBT_RWA_TREND_5YR) * SUM(SXXP_O_PBT_RWA_TREND_5YR * (SQUARE(SXXP_MID_O_PBT_RWA_TREND_5YR)))) As 'O_PBT_RWA_TREND_5YR',
	--SUM(SXXP_O_PBT_RWA_RSD * PBT_RWA_RSD)/SUM(SXXP_O_PBT_RWA_RSD) As 'E_PBT_RWA_RSD',
	--SQRT(1/SUM(SXXP_O_PBT_RWA_RSD) * SUM(SXXP_O_PBT_RWA_RSD * (SQUARE(SXXP_MID_O_PBT_RWA_RSD)))) As 'O_PBT_RWA_RSD',
	SUM(SXXP_O_ROE_NTM * ROE_NTM)/SUM(SXXP_O_ROE_NTM) As 'E_ROE_NTM',
	SQRT(1/SUM(SXXP_O_ROE_NTM) * SUM(SXXP_O_ROE_NTM * (SQUARE(SXXP_MID_O_ROE_NTM)))) As 'O_ROE_NTM',
	SUM(SXXP_O_DIV_YLD_NTM * DIV_YLD_NTM)/SUM(SXXP_O_DIV_YLD_NTM) As 'E_DIV_YLD_NTM',
	SQRT(1/SUM(SXXP_O_DIV_YLD_NTM) * SUM(SXXP_O_DIV_YLD_NTM * (SQUARE(SXXP_MID_O_DIV_YLD_NTM)))) As 'O_DIV_YLD_NTM',
	SUM(SXXP_O_PE_NTM_INVERSE * PE_NTM_INVERSE)/SUM(SXXP_O_PE_NTM_INVERSE) As 'E_PE_NTM_INVERSE',
	SQRT(1/SUM(SXXP_O_PE_NTM_INVERSE) * SUM(SXXP_O_PE_NTM_INVERSE * (SQUARE(SXXP_MID_O_PE_NTM_INVERSE)))) As 'O_PE_NTM_INVERSE',
	SUM(SXXP_O_PE_ON_AVG5Y_INVERSE * PE_ON_AVG5Y_INVERSE)/SUM(SXXP_O_PE_ON_AVG5Y_INVERSE) As 'E_PE_ON_AVG5Y_INVERSE',
	SQRT(1/SUM(SXXP_O_PE_ON_AVG5Y_INVERSE) * SUM(SXXP_O_PE_ON_AVG5Y_INVERSE * (SQUARE(SXXP_MID_O_PE_ON_AVG5Y_INVERSE)))) As 'O_PE_ON_AVG5Y_INVERSE',
	SUM(SXXP_O_PE_VS_IND_ON_AVG5Y_INVERSE * PE_VS_IND_ON_AVG5Y_INVERSE)/SUM(SXXP_O_PE_VS_IND_ON_AVG5Y_INVERSE) As 'E_PE_VS_IND_ON_AVG5Y_INVERSE',
	SQRT(1/SUM(SXXP_O_PE_VS_IND_ON_AVG5Y_INVERSE) * SUM(SXXP_O_PE_VS_IND_ON_AVG5Y_INVERSE * (SQUARE(SXXP_MID_O_PE_VS_IND_ON_AVG5Y_INVERSE)))) As 'O_PE_VS_IND_ON_AVG5Y_INVERSE',
	SUM(SXXP_O_PB_NTM_INVERSE * PB_NTM_INVERSE)/SUM(SXXP_O_PB_NTM_INVERSE) As 'E_PB_NTM_INVERSE',
	SQRT(1/SUM(SXXP_O_PB_NTM_INVERSE) * SUM(SXXP_O_PB_NTM_INVERSE * (SQUARE(SXXP_MID_O_PB_NTM_INVERSE)))) As 'O_PB_NTM_INVERSE',
	SUM(SXXP_O_PB_ON_AVG5Y_INVERSE * PB_ON_AVG5Y_INVERSE)/SUM(SXXP_O_PB_ON_AVG5Y_INVERSE) As 'E_PB_ON_AVG5Y_INVERSE',
	SQRT(1/SUM(SXXP_O_PB_ON_AVG5Y_INVERSE) * SUM(SXXP_O_PB_ON_AVG5Y_INVERSE * (SQUARE(SXXP_MID_O_PB_ON_AVG5Y_INVERSE)))) As 'O_PB_ON_AVG5Y_INVERSE',
	SUM(SXXP_O_PB_VS_IND_ON_AVG5Y_INVERSE * PB_VS_IND_ON_AVG5Y_INVERSE)/SUM(SXXP_O_PB_VS_IND_ON_AVG5Y_INVERSE) As 'E_PB_VS_IND_ON_AVG5Y_INVERSE',
	SQRT(1/SUM(SXXP_O_PB_VS_IND_ON_AVG5Y_INVERSE) * SUM(SXXP_O_PB_VS_IND_ON_AVG5Y_INVERSE * (SQUARE(SXXP_MID_O_PB_VS_IND_ON_AVG5Y_INVERSE)))) As 'O_PB_VS_IND_ON_AVG5Y_INVERSE'
Into 
	#EspvsVari
FROM 
	#agrege agr
	INNER JOIN #SXXPEspVsVari sxxp ON agr.FGA_SECTOR = sxxp.FGA_SECTOR
	INNER JOIN #MidSXXPEspVsVari mid ON agr.FGA_SECTOR = mid.FGA_SECTOR


--select * from #EspvsVari

--SELECT * FROM #agrege

/*3) si les tendances sont négatives _TREND on supprime les stabilités _RSD*/
--UPDATE #agrege SET SALES_RSD = NULL WHERE SALES_TREND_5YR < 0 
--UPDATE #agrege SET EPS_RSD = NULL WHERE EPS_TREND_5YR < 0
--UPDATE #agrege SET EBIT_MARGIN_RSD = NULL WHERE EBIT_MARGIN_TREND_5YR < 0

/*3)On normalise les indicateurs (calcul du score centré réduit)*/
UPDATE #agrege 
SET SALES_GROWTH_NTM = CASE WHEN ev.O_SALES_GROWTH_NTM = 0 THEN NULL ELSE (SALES_GROWTH_NTM - ev.E_SALES_GROWTH_NTM )/ ev.O_SALES_GROWTH_NTM END,
	SALES_RSD = CASE WHEN ev.O_SALES_RSD = 0 THEN NULL ELSE (SALES_RSD - ev.E_SALES_RSD )/ ev.O_SALES_RSD END,
	SALES_TREND_5YR = CASE WHEN ev.O_SALES_TREND_5YR = 0 THEN NULL ELSE (SALES_TREND_5YR - ev.E_SALES_TREND_5YR )/ ev.O_SALES_TREND_5YR END,
	EPS_GROWTH_NTM = CASE WHEN ev.O_EPS_GROWTH_NTM = 0 THEN NULL ELSE (EPS_GROWTH_NTM - ev.E_EPS_GROWTH_NTM )/ ev.O_EPS_GROWTH_NTM END,	
	EPS_RSD = CASE WHEN ev.O_EPS_RSD = 0 THEN NULL ELSE (EPS_RSD - ev.E_EPS_RSD )/ ev.O_EPS_RSD END,	
	EPS_TREND_5YR = CASE WHEN ev.O_EPS_TREND_5YR = 0 THEN NULL ELSE (EPS_TREND_5YR - ev.E_EPS_TREND_5YR )/ ev.O_EPS_TREND_5YR END,	
	EBIT_MARGIN_TREND_5YR = CASE WHEN ev.O_EBIT_MARGIN_TREND_5YR = 0 THEN NULL ELSE (EBIT_MARGIN_TREND_5YR - ev.E_EBIT_MARGIN_TREND_5YR )/ ev.O_EBIT_MARGIN_TREND_5YR END,	
	EBIT_MARGIN_RSD = CASE WHEN ev.O_EBIT_MARGIN_RSD = 0 THEN NULL ELSE (EBIT_MARGIN_RSD - ev.E_EBIT_MARGIN_RSD )/ ev.O_EBIT_MARGIN_RSD END,
	--PBT_RWA_TREND_5YR = CASE WHEN ev.O_PBT_RWA_TREND_5YR = 0 THEN NULL ELSE (PBT_RWA_TREND_5YR - ev.E_PBT_RWA_TREND_5YR )/ ev.O_PBT_RWA_TREND_5YR END,	
	--PBT_RWA_RSD = CASE WHEN ev.O_PBT_RWA_RSD = 0 THEN NULL ELSE (PBT_RWA_RSD - ev.E_PBT_RWA_RSD )/ ev.O_PBT_RWA_RSD END,
	ROE_NTM = CASE WHEN ev.O_ROE_NTM = 0 THEN NULL ELSE (ROE_NTM - ev.E_ROE_NTM )/ ev.O_ROE_NTM END,	
	DIV_YLD_NTM = CASE WHEN ev.O_DIV_YLD_NTM = 0 THEN NULL ELSE (DIV_YLD_NTM - ev.E_DIV_YLD_NTM )/ ev.O_DIV_YLD_NTM END,	
	PE_NTM_INVERSE = CASE WHEN ev.O_PE_NTM_INVERSE = 0 THEN NULL ELSE (PE_NTM_INVERSE - ev.E_PE_NTM_INVERSE )/ ev.O_PE_NTM_INVERSE END, 
	PE_ON_AVG5Y_INVERSE = CASE WHEN ev.O_PE_ON_AVG5Y_INVERSE = 0 THEN NULL ELSE (PE_ON_AVG5Y_INVERSE - ev.E_PE_ON_AVG5Y_INVERSE )/ ev.O_PE_ON_AVG5Y_INVERSE END,
	PE_VS_IND_ON_AVG5Y_INVERSE = CASE WHEN ev.O_PE_VS_IND_ON_AVG5Y_INVERSE = 0 THEN NULL ELSE (PE_VS_IND_ON_AVG5Y_INVERSE - ev.E_PE_VS_IND_ON_AVG5Y_INVERSE)/ ev.O_PE_VS_IND_ON_AVG5Y_INVERSE END,
	PB_NTM_INVERSE = CASE WHEN ev.O_PB_NTM_INVERSE = 0 THEN NULL ELSE (PB_NTM_INVERSE - ev.E_PB_NTM_INVERSE )/ ev.O_PB_NTM_INVERSE END,
	PB_ON_AVG5Y_INVERSE = CASE WHEN ev.O_PB_ON_AVG5Y_INVERSE = 0 THEN NULL ELSE (PB_ON_AVG5Y_INVERSE - ev.E_PB_ON_AVG5Y_INVERSE )/ ev.O_PB_ON_AVG5Y_INVERSE END,
	PB_VS_IND_ON_AVG5Y_INVERSE = CASE WHEN ev.O_PB_VS_IND_ON_AVG5Y_INVERSE = 0 THEN NULL ELSE (PB_VS_IND_ON_AVG5Y_INVERSE - ev.E_PB_VS_IND_ON_AVG5Y_INVERSE)/ ev.O_PB_VS_IND_ON_AVG5Y_INVERSE END
FROM #EspvsVari ev

/*
/*4)On active les bons coefs des indicateurs*/
--SELECT * FROM ACT_DATA_FACTSET_COEF WHERE date= '14/02/2012' and portfolio='Garp New'
update #agrege
set coefFinance = 1 
from #agrege
where FGA_SECTOR in ('26','27','28')

update #agrege
set coefBank = 1 
from #agrege
where FGA_SECTOR = '25'

update #agrege
set coefOthers = 1
from #agrege
where FGA_SECTOR not in ('25','26','27','28')
*/

/*4)On active les bons coefs des indicateurs si NULL il compte pas pour le dénominateur et numérateur*/
--DECLARE @Date as datetime
--SET @date = '02/07/2013'

DECLARE @id_indice as INT
SET @id_indice = (SELECT id FROM ACT_INDICE WHERE libelle = 'SXXP')
--SELECT @id_indice

--SELECT * FROM ACT_COEF_CRITERE
--SELECT * FROM ACT_COEF_INDICE


create table #coef (FGA_SECTOR varchar(13), indicateur varchar(60), ponderation float)
--SELECT * FROM #agrege
--SELECT distinct(portfolio) from ACT_DATA_FACTSET_COEF
--SELECT * FROm #coef

insert into #coef
SELECT   
	a.FGA_SECTOR,
    'SALES_GROWTH_NTM' as indicateur,
	CASE
		WHEN a.SALES_GROWTH_NTM is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											
											
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='SALES_GROWTH_NTM'
					), 0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'SALES_RSD' as indicateur,
	CASE
		WHEN a.SALES_RSD is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='SALES_RSD'
					) ,0)
	END as ponderation
FROM #agrege a 


insert into #coef
SELECT  
	a.FGA_SECTOR,
    'SALES_TREND_5YR' as indicateur,
	CASE 
		WHEN a.SALES_TREND_5YR is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='SALES_TREND_5YR'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'EPS_GROWTH_NTM' as indicateur,
	CASE 
		WHEN a.EPS_GROWTH_NTM is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='EPS_GROWTH_NTM'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'EPS_RSD' as indicateur,
	CASE 
		WHEN a.EPS_RSD is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='EPS_RSD'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'EPS_TREND_5YR' as indicateur,
	CASE 
		WHEN a.EPS_TREND_5YR is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='EPS_TREND_5YR'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'EBIT_MARGIN_TREND_5YR' as indicateur,
	CASE 
		WHEN a.EBIT_MARGIN_TREND_5YR is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='EBIT_MARGIN_TREND_5YR'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'EBIT_MARGIN_RSD' as indicateur,
	CASE 
		WHEN a.EBIT_MARGIN_RSD is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='EBIT_MARGIN_RSD'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'ROE_NTM' as indicateur,
	CASE 
		WHEN a.ROE_NTM is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='ROE_NTM'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'DIV_YLD_NTM' as indicateur,
	CASE 
		WHEN a.DIV_YLD_NTM is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='DIV_YLD_NTM'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'PE_NTM_INVERSE' as indicateur,
	CASE 
		WHEN a.PE_NTM_INVERSE is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='PE_NTM_INVERSE'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'PE_ON_AVG5Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PE_ON_AVG5Y_INVERSE is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='PE_ON_AVG5Y_INVERSE'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'PB_NTM_INVERSE' as indicateur,
	CASE 
		WHEN a.PB_NTM_INVERSE is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='PB_NTM_INVERSE'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'PB_ON_AVG5Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PB_ON_AVG5Y_INVERSE is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='PB_ON_AVG5Y_INVERSE'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'PE_VS_IND_ON_AVG5Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PE_VS_IND_ON_AVG5Y_INVERSE is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='PE_VS_IND_ON_AVG5Y_INVERSE'
					) ,0)
	END as ponderation
FROM #agrege a 

insert into #coef
SELECT  
	a.FGA_SECTOR,
    'PB_VS_IND_ON_AVG5Y_INVERSE' as indicateur,
	CASE 
		WHEN a.PB_VS_IND_ON_AVG5Y_INVERSE is NULL THEN 0
		else
			COALESCE((	SELECT coef
						FROM
							ACT_COEF_INDICE ind
							INNER JOIN (SELECT MAX(ind.date) as maxdate, crit.id_critere, crit.nom
										FROM ACT_COEF_INDICE ind
										INNER JOIN ACT_COEF_CRITERE crit ON crit.id_critere = ind.id_critere
										WHERE ind.id_indice=@id_indice
											AND ind.date <= @date AND crit.is_sector = 0
										GROUP BY crit.id_critere, crit.nom
										) crit ON crit.id_critere=ind.id_critere
											AND crit.maxdate = ind.date
						WHERE crit.nom ='PB_VS_IND_ON_AVG5Y_INVERSE'
					) ,0)
	END as ponderation
FROM #agrege a 



/*
/*5)Calcul des scores*/
update #agrege
set BLEND_TOTAL = 
		( ( a.coefFinance * c1.FINANCE  + a.coefOthers * c1.OTHERS  + a.coefBank * c1.BANK  ) * COALESCE(SALES_GROWTH_NTM,0)  + 
          ( a.coefFinance * c2.FINANCE  + a.coefOthers * c2.OTHERS  + a.coefBank * c2.BANK  ) * COALESCE(SALES_RSD,0)  +
          ( a.coefFinance * c3.FINANCE  + a.coefOthers * c3.OTHERS  + a.coefBank * c3.BANK  ) * COALESCE(SALES_TREND_5YR,0) +
          ( a.coefFinance * c4.FINANCE  + a.coefOthers * c4.OTHERS  + a.coefBank * c4.BANK  ) * COALESCE(EPS_GROWTH_NTM,0) +
          ( a.coefFinance * c5.FINANCE  + a.coefOthers * c5.OTHERS  + a.coefBank * c5.BANK  ) * COALESCE(EPS_RSD,0) +
          ( a.coefFinance * c6.FINANCE  + a.coefOthers * c6.OTHERS  + a.coefBank * c6.BANK  ) * COALESCE(EPS_TREND_5YR,0) +
          ( a.coefFinance * c7.FINANCE  + a.coefOthers * c7.OTHERS  + a.coefBank * c7.BANK  ) * COALESCE(EBIT_MARGIN_RSD,0) +
          ( a.coefFinance * c8.FINANCE  + a.coefOthers * c8.OTHERS  + a.coefBank * c8.BANK  ) * COALESCE(EBIT_MARGIN_TREND_5YR,0) +
          --( a.coefFinance * c77.FINANCE + a.coefOthers * c77.OTHERS + a.coefBank * c77.BANK ) * COALESCE(PBT_RWA_RSD,0) +
          --( a.coefFinance * c88.FINANCE + a.coefOthers * c88.OTHERS + a.coefBank * c88.BANK ) * COALESCE(PBT_RWA_TREND_5YR,0) +
          ( a.coefFinance * c9.FINANCE  + a.coefOthers * c9.OTHERS  + a.coefBank * c9.BANK  ) * COALESCE(ROE_NTM,0) +
          ( a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS + a.coefBank * c10.BANK ) * COALESCE(DIV_YLD_NTM,0) +
          ( a.coefFinance * c11.FINANCE + a.coefOthers * c11.OTHERS + a.coefBank * c11.BANK ) * COALESCE(PE_NTM_INVERSE,0) +
          ( a.coefFinance * c12.FINANCE + a.coefOthers * c12.OTHERS + a.coefBank * c12.BANK ) * COALESCE(PE_ON_AVG5Y_INVERSE,0) +
          ( a.coefFinance * c13.FINANCE + a.coefOthers * c13.OTHERS + a.coefBank * c13.BANK ) * COALESCE(PE_VS_IND_ON_AVG5Y_INVERSE,0)  +
          ( a.coefFinance * c14.FINANCE + a.coefOthers * c14.OTHERS + a.coefBank * c14.BANK ) * COALESCE(PB_NTM_INVERSE,0) +
          ( a.coefFinance * c15.FINANCE + a.coefOthers * c15.OTHERS + a.coefBank * c15.BANK ) * COALESCE(PB_ON_AVG5Y_INVERSE,0) +
          ( a.coefFinance * c16.FINANCE + a.coefOthers * c16.OTHERS + a.coefBank * c16.BANK ) * COALESCE(a.PB_VS_IND_ON_AVG5Y_INVERSE,0)  ) 
        / ( a.coefFinance * c1.FINANCE  + a.coefOthers * c1.OTHERS  + a.coefBank * c1.BANK + a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS  + a.coefBank * c2.BANK +  a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS  + a.coefBank * c3.BANK + a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS + a.coefBank * c4.BANK + a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS  + a.coefBank * c5.BANK + a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS  + a.coefBank * c6.BANK + a.coefFinance * c7.FINANCE + a.coefOthers * c7.OTHERS  + a.coefBank * c7.BANK + a.coefFinance * c8.FINANCE + a.coefOthers * c8.OTHERS  + a.coefBank * c8.BANK + a.coefFinance * c9.FINANCE + a.coefOthers * c9.OTHERS  + a.coefBank * c9.BANK + a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS  + a.coefBank * c10.BANK + a.coefFinance * c11.FINANCE + a.coefOthers * c11.OTHERS  + a.coefBank * c11.BANK + a.coefFinance * c12.FINANCE + a.coefOthers * c12.OTHERS  + a.coefBank * c12.BANK + a.coefFinance * c13.FINANCE + a.coefOthers * c13.OTHERS  + a.coefBank * c13.BANK + a.coefFinance * c14.FINANCE + a.coefOthers * c14.OTHERS  + a.coefBank * c4.BANK + a.coefFinance * c15.FINANCE + a.coefOthers * c15.OTHERS  + a.coefBank * c15.BANK + a.coefFinance * c16.FINANCE + a.coefOthers * c16.OTHERS  + a.coefBank * c16.BANK),
    
    BLEND_GROWTH = 
		( ( a.coefFinance * c1.FINANCE + a.coefOthers * c1.OTHERS + a.coefBank * c1.BANK ) * COALESCE(SALES_GROWTH_NTM,0)  + 
          ( a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS + a.coefBank * c2.BANK ) * COALESCE(SALES_RSD,0)  +
          ( a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS + a.coefBank * c3.BANK) * COALESCE(SALES_TREND_5YR,0) +
          ( a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS + a.coefBank * c4.BANK) * COALESCE(EPS_GROWTH_NTM,0) +
          ( a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS + a.coefBank * c5.BANK) * COALESCE(EPS_RSD,0) +
          ( a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS + a.coefBank * c6.BANK) * COALESCE(EPS_TREND_5YR,0)  ) 
        / ( a.coefFinance * c1.FINANCE + a.coefOthers * c1.OTHERS + a.coefBank * c1.BANK+ a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS  + a.coefBank * c2.BANK + a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS  + a.coefBank * c3.BANK + a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS  + a.coefBank * c4.BANK + a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS  + a.coefBank * c5.BANK + a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS  + a.coefBank * c6.BANK),
    
    BLEND_YIELD = 
		( ( a.coefFinance * c7.FINANCE  + a.coefOthers * c7.OTHERS  + a.coefBank * c7.BANK ) * COALESCE(EBIT_MARGIN_RSD,0) +
          ( a.coefFinance * c8.FINANCE  + a.coefOthers * c8.OTHERS  + a.coefBank * c8.BANK) * COALESCE(EBIT_MARGIN_TREND_5YR,0) +
          --( a.coefFinance * c77.FINANCE + a.coefOthers * c77.OTHERS + a.coefBank * c77.BANK) * COALESCE(PBT_RWA_RSD,0) +
          --( a.coefFinance * c88.FINANCE + a.coefOthers * c88.OTHERS + a.coefBank * c88.BANK) * COALESCE(PBT_RWA_TREND_5YR,0) +
          ( a.coefFinance * c9.FINANCE  + a.coefOthers * c9.OTHERS  + a.coefBank * c9.BANK) * COALESCE(ROE_NTM,0)   ) 
        / ( a.coefFinance * c7.FINANCE  + a.coefOthers * c7.OTHERS  + a.coefBank * c7.BANK + a.coefFinance * c8.FINANCE + a.coefOthers * c8.OTHERS  + a.coefBank * c8.BANK +  a.coefFinance * c9.FINANCE + a.coefOthers * c9.OTHERS  + a.coefBank * c9.BANK ),
    
    BLEND_VALUE = 
		( ( a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS + a.coefBank * c10.BANK ) * COALESCE(DIV_YLD_NTM,0) +
          ( a.coefFinance * c11.FINANCE + a.coefOthers * c11.OTHERS + a.coefBank * c11.BANK) * COALESCE(PE_NTM_INVERSE,0) +
          ( a.coefFinance * c12.FINANCE + a.coefOthers * c12.OTHERS + a.coefBank * c12.BANK ) * COALESCE(PE_ON_AVG5Y_INVERSE,0) +
          ( a.coefFinance * c13.FINANCE + a.coefOthers * c13.OTHERS + a.coefBank * c13.BANK ) * COALESCE(PE_VS_IND_ON_AVG5Y_INVERSE,0)  +
          ( a.coefFinance * c14.FINANCE + a.coefOthers * c14.OTHERS + a.coefBank * c14.BANK ) * COALESCE(PB_NTM_INVERSE,0) +
          ( a.coefFinance * c15.FINANCE + a.coefOthers * c15.OTHERS + a.coefBank * c15.BANK ) * COALESCE(PB_ON_AVG5Y_INVERSE,0)+
          ( a.coefFinance * c16.FINANCE + a.coefOthers * c16.OTHERS + a.coefBank * c16.BANK ) * COALESCE(PB_VS_IND_ON_AVG5Y_INVERSE,0)  ) 
        / ( a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS + a.coefBank * c10.BANK + a.coefFinance * c11.FINANCE + a.coefOthers * c11.OTHERS + a.coefBank * c11.BANK + a.coefFinance * c12.FINANCE + a.coefOthers * c12.OTHERS  + a.coefBank * c12.BANK + a.coefFinance * c13.FINANCE + a.coefOthers * c13.OTHERS  + a.coefBank * c13.BANK + a.coefFinance * c14.FINANCE + a.coefOthers * c14.OTHERS  + a.coefBank * c14.BANK + a.coefFinance * c15.FINANCE + a.coefOthers * c15.OTHERS  + a.coefBank * c15.BANK + a.coefFinance * c16.FINANCE + a.coefOthers * c16.OTHERS  + a.coefBank * c16.BANK)
    
from  #agrege as a ,
ACT_DATA_FACTSET_COEF as c1 ,ACT_DATA_FACTSET_COEF as c2 ,ACT_DATA_FACTSET_COEF as c3, ACT_DATA_FACTSET_COEF as c4 ,ACT_DATA_FACTSET_COEF as c5, ACT_DATA_FACTSET_COEF as c6 , ACT_DATA_FACTSET_COEF as c7, ACT_DATA_FACTSET_COEF as c8, ACT_DATA_FACTSET_COEF as c9,ACT_DATA_FACTSET_COEF as c10 ,ACT_DATA_FACTSET_COEF as c11,ACT_DATA_FACTSET_COEF as c12,ACT_DATA_FACTSET_COEF as c13 ,ACT_DATA_FACTSET_COEF as c14 ,ACT_DATA_FACTSET_COEF as c15 ,ACT_DATA_FACTSET_COEF as c16
where   c1.indicator = 'SALES_GROWTH_NTM' and  c1.date= @date and c1.portfolio='BlendSecteur' 
	and c2.indicator = 'SALES_RSD' and  c2.date= @date and c2.portfolio='BlendSecteur' 
	and c3.indicator = 'SALES_TREND_5YR' and  c3.date= @date and c3.portfolio='BlendSecteur' 
	and c4.indicator = 'EPS_GROWTH_NTM' and  c4.date= @date and c4.portfolio='BlendSecteur' 
	and c5.indicator = 'EPS_RSD' and  c5.date= @date and c5.portfolio='BlendSecteur' 
	and c6.indicator = 'EPS_TREND_5YR' and  c6.date= @date and c6.portfolio='BlendSecteur' 
	and c7.indicator = 'EBIT_MARGIN_RSD' and  c7.date= @date and c7.portfolio='BlendSecteur' 
	and c8.indicator = 'EBIT_MARGIN_TREND_5YR' and  c8.date= @date and c8.portfolio='BlendSecteur' 
	--and c77.indicator = 'PBT_RWA_RSD' and  c77.date= @date and c77.portfolio='BlendSecteur' 
	--and c88.indicator = 'PBT_RWA_TREND_5YR' and  c88.date= @date and c88.portfolio='BlendSecteur' 
	and c9.indicator = 'ROE_NTM' and  c9.date= @date and c9.portfolio='BlendSecteur' 	
	and c10.indicator = 'DIV_YLD_NTM' and  c10.date= @date and c10.portfolio='BlendSecteur' 
	and c11.indicator = 'PE_NTM_INVERSE' and  c11.date= @date and c11.portfolio='BlendSecteur' 
	and c12.indicator = 'PE_ON_AVG5Y_INVERSE' and  c12.date= @date and c12.portfolio='BlendSecteur'
	and c13.indicator = 'PE_VS_IND_ON_AVG5Y_INVERSE' and c13.date=@date and c13.portfolio='BlendSecteur'  
	and c14.indicator = 'PB_NTM_INVERSE' and  c14.date= @date and c14.portfolio='BlendSecteur' 
	and c15.indicator = 'PB_ON_AVG5Y_INVERSE' and  c15.date= @date and c15.portfolio='BlendSecteur' 							
	and c16.indicator = 'PB_VS_IND_ON_AVG5Y_INVERSE' and  c16.date= @date and c16.portfolio='BlendSecteur' 
--Select * from #agrege
--Select * from ACT_DATA_FACTSET_AGR
*/

UPDATE #agrege  
SET BLEND_TOTAL = 
		COALESCE(( COALESCE(SALES_GROWTH_NTM,0) * (select ponderation from #coef where indicateur = 'SALES_GROWTH_NTM' and FGA_SECTOR = a.FGA_SECTOR)
		+ COALESCE(SALES_RSD,0) * (select ponderation from #coef where indicateur = 'SALES_RSD' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(SALES_TREND_5YR,0) * (select ponderation from #coef where indicateur = 'SALES_TREND_5YR' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(EPS_GROWTH_NTM,0) * (select ponderation from #coef where indicateur = 'EPS_GROWTH_NTM' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(EPS_RSD,0) * (select ponderation from #coef where indicateur = 'EPS_RSD' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(EPS_TREND_5YR,0) * (select ponderation from #coef where indicateur = 'EPS_TREND_5YR' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(EBIT_MARGIN_TREND_5YR,0) * (select ponderation from #coef where indicateur = 'EBIT_MARGIN_TREND_5YR' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(EBIT_MARGIN_RSD,0) * (select ponderation from #coef where indicateur = 'EBIT_MARGIN_RSD' and FGA_SECTOR = a.FGA_SECTOR )
		--+ COALESCE(PBT_RWA_TREND_5YR,0) * (select ponderation from #coef where indicateur = 'PBT_RWA_TREND_5YR' and FGA_SECTOR = a.FGA_SECTOR )
		--+ COALESCE(PBT_RWA_RSD,0) * (select ponderation from #coef where indicateur = 'PBT_RWA_RSD' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(ROE_NTM,0) * (select ponderation from #coef where indicateur = 'ROE_NTM' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(DIV_YLD_NTM,0) * (select ponderation from #coef where indicateur = 'DIV_YLD_NTM' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(PE_NTM_INVERSE,0) * (select ponderation from #coef where indicateur = 'PE_NTM_INVERSE' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(PE_ON_AVG5Y_INVERSE,0) * (select ponderation from #coef where indicateur = 'PE_ON_AVG5Y_INVERSE' and FGA_SECTOR = a.FGA_SECTOR) 
		+ COALESCE(PB_NTM_INVERSE,0) * (select ponderation from #coef where indicateur = 'PB_NTM_INVERSE' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(PB_ON_AVG5Y_INVERSE,0) * (select ponderation from #coef where indicateur = 'PB_ON_AVG5Y_INVERSE' and FGA_SECTOR = a.FGA_SECTOR)
		+ COALESCE(PE_VS_IND_ON_AVG5Y_INVERSE,0) * (select ponderation from #coef where indicateur = 'PE_VS_IND_ON_AVG5Y_INVERSE' and FGA_SECTOR = a.FGA_SECTOR) 
		+ COALESCE(PB_VS_IND_ON_AVG5Y_INVERSE,0) * (select ponderation from #coef where indicateur = 'PB_VS_IND_ON_AVG5Y_INVERSE' and FGA_SECTOR = a.FGA_SECTOR)	)
		/ (NULLIF((select SUM(ponderation) from #coef where FGA_SECTOR = a.FGA_SECTOR) , 0))
		,-100), -- le total vaut -100 si le resultat est nul
		
BLEND_GROWTH = 
		COALESCE(( COALESCE(SALES_GROWTH_NTM,0) * (select ponderation from #coef where indicateur = 'SALES_GROWTH_NTM' and FGA_SECTOR = a.FGA_SECTOR)
		+ COALESCE(SALES_RSD,0) * (select ponderation from #coef where indicateur = 'SALES_RSD' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(SALES_TREND_5YR,0) * (select ponderation from #coef where indicateur = 'SALES_TREND_5YR' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(EPS_GROWTH_NTM,0) * (select ponderation from #coef where indicateur = 'EPS_GROWTH_NTM' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(EPS_RSD,0) * (select ponderation from #coef where indicateur = 'EPS_RSD' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(EPS_TREND_5YR,0) * (select ponderation from #coef where indicateur = 'EPS_TREND_5YR' and FGA_SECTOR = a.FGA_SECTOR ) )
		/ (NULLIF( (select SUM(ponderation) from #coef where FGA_SECTOR = a.FGA_SECTOR and indicateur in ('SALES_GROWTH_NTM','SALES_RSD','SALES_TREND_5YR','EPS_GROWTH_NTM','EPS_RSD','EPS_TREND_5YR')) , 0))
		,-100), -- le total vaut -100 si le resultat est nul
		
BLEND_YIELD = 
		COALESCE(( COALESCE(EBIT_MARGIN_TREND_5YR,0) * (select ponderation from #coef where indicateur = 'EBIT_MARGIN_TREND_5YR' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(EBIT_MARGIN_RSD,0) * (select ponderation from #coef where indicateur = 'EBIT_MARGIN_RSD' and FGA_SECTOR = a.FGA_SECTOR )
		--+ COALESCE(PBT_RWA_TREND_5YR,0) * (select ponderation from #coef where indicateur = 'PBT_RWA_TREND_5YR' and FGA_SECTOR = a.FGA_SECTOR )
		--+ COALESCE(PBT_RWA_RSD,0) * (select ponderation from #coef where indicateur = 'PBT_RWA_RSD' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(ROE_NTM,0) * (select ponderation from #coef where indicateur = 'ROE_NTM' and FGA_SECTOR = a.FGA_SECTOR ) 	 )
		/ (NULLIF( (select SUM(ponderation) from #coef where FGA_SECTOR = a.FGA_SECTOR and indicateur IN ('EBIT_MARGIN_TREND_5YR','EBIT_MARGIN_RSD','ROE_NTM')) , 0))	
		,-100), -- le total vaut -100 si le resultat est nul
		
BLEND_VALUE = 		
		COALESCE(( COALESCE(DIV_YLD_NTM,0) * (select ponderation from #coef where indicateur = 'DIV_YLD_NTM' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(PE_NTM_INVERSE,0) * (select ponderation from #coef where indicateur = 'PE_NTM_INVERSE' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(PE_ON_AVG5Y_INVERSE,0) * (select ponderation from #coef where indicateur = 'PE_ON_AVG5Y_INVERSE' and FGA_SECTOR = a.FGA_SECTOR) 
		+ COALESCE(PE_VS_IND_ON_AVG5Y_INVERSE,0) * (select ponderation from #coef where indicateur = 'PE_VS_IND_ON_AVG5Y_INVERSE' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(PB_NTM_INVERSE,0) * (select ponderation from #coef where indicateur = 'PB_NTM_INVERSE' and FGA_SECTOR = a.FGA_SECTOR )
		+ COALESCE(PB_ON_AVG5Y_INVERSE,0) * (select ponderation from #coef where indicateur = 'PB_ON_AVG5Y_INVERSE' and FGA_SECTOR = a.FGA_SECTOR) 
		+ COALESCE(PB_VS_IND_ON_AVG5Y_INVERSE,0) * (select ponderation from #coef where indicateur = 'PE_VS_IND_ON_AVG5Y_INVERSE' and FGA_SECTOR = a.FGA_SECTOR) )
		/ (NULLIF( (select SUM(ponderation) from #coef where FGA_SECTOR = a.FGA_SECTOR and indicateur IN ('DIV_YLD_NTM','PE_NTM_INVERSE','PE_ON_AVG5Y_INVERSE','PE_VS_IND_ON_AVG5Y_INVERSE','PB_NTM_INVERSE','PB_ON_AVG5Y_INVERSE','PB_VS_IND_ON_AVG5Y_INVERSE')), 0))	
		,-100) -- le total vaut -100 si le resultat est nul
FROM #agrege a

--SELECT * FROM #agrege a

UPDATE DATA_FACTSET
SET BLEND_TOTAL  = tmp.BLEND_TOTAL,		
    BLEND_GROWTH = tmp.BLEND_GROWTH,
    BLEND_VALUE  = tmp.BLEND_VALUE,
    BLEND_YIELD  = tmp.BLEND_YIELD
FROM #agrege tmp, ACT_DATA_FACTSET_AGR a
WHERE a.date=@date and a.fga_sector=tmp.fga_sector and indice='SXXP'

/*
select  
		FGA_Sector AS 'FGA Sector',
		BLEND_TOTAL AS 'Score',
		BLEND_GROWTH AS 'Croissance',
		BLEND_VALUE AS 'Valeur',
		BLEND_YIELD AS 'Rendement',
		--SXXP AS 'Poids',
		SALES_GROWTH_NTM AS 'Cr Ventes',		
		SALES_RSD AS 'Ventes CV',
		SALES_TREND_5YR AS 'Ventes trend',
		EPS_GROWTH_NTM AS 'Cr BPA',
		EPS_RSD AS 'BPA CV',
		EPS_TREND_5YR AS 'BPA trend',
		EBIT_MARGIN_NTM AS 'Marge EBIT',
		--EBIT_MARGIN_NTM_5_CV AS 'Marge EBIT CV',
		PB_NTM/PE_NTM AS 'ROE',
		DIV_YLD_NTM AS 'Rdt',
		PE_NTM AS 'PE',
		PE_ON_AVG5Y AS 'PE/m(5)',
		PB_NTM AS 'PB', 
		PB_ON_AVG5Y AS 'PB/m(5)' 
from ACT_DATA_FACTSET_AGR where date=@date and FGA_SECTOR IS NOT NULL and ICB_SUPERSECTOR IS NOT NULL and INDICE='SXXP'
*/

--SELECt * FROm #agrege order by BLEND_TOTAL

DROP TABLE #agrege
DROP TABLE #EspvsVari
DROP TABLE #SXXPEspVsVari
DROP TABLE #MidSXXPEspVsVari
DROP TABLE #coef
































/*
DECLARE @Date AS Datetime 
SET @Date='25/06/2012'
Select 
		( ( a.coefFinance * c1.FINANCE  + a.coefOthers * c1.OTHERS  + a.coefBank * c1.BANK  ) * COALESCE(SALES_GROWTH_NTM,0)  + 
          ( a.coefFinance * c2.FINANCE  + a.coefOthers * c2.OTHERS  + a.coefBank * c2.BANK  ) * COALESCE(SALES_RSD,0)  +
          ( a.coefFinance * c3.FINANCE  + a.coefOthers * c3.OTHERS  + a.coefBank * c3.BANK  ) * COALESCE(SALES_TREND_5YR,0) +
          ( a.coefFinance * c4.FINANCE  + a.coefOthers * c4.OTHERS  + a.coefBank * c4.BANK  ) * COALESCE(EPS_GROWTH_NTM,0) +
          ( a.coefFinance * c5.FINANCE  + a.coefOthers * c5.OTHERS  + a.coefBank * c5.BANK  ) * COALESCE(EPS_RSD,0) +
          ( a.coefFinance * c6.FINANCE  + a.coefOthers * c6.OTHERS  + a.coefBank * c6.BANK  ) * COALESCE(EPS_TREND_5YR,0) +
          ( a.coefFinance * c7.FINANCE  + a.coefOthers * c7.OTHERS  + a.coefBank * c7.BANK  ) * COALESCE(EBIT_MARGIN_RSD,0) +
          ( a.coefFinance * c8.FINANCE  + a.coefOthers * c8.OTHERS  + a.coefBank * c8.BANK  ) * COALESCE(EBIT_MARGIN_TREND_5YR,0) +
          ( a.coefFinance * c77.FINANCE + a.coefOthers * c77.OTHERS + a.coefBank * c77.BANK ) * COALESCE(PBT_RWA_RSD,0) +
          ( a.coefFinance * c88.FINANCE + a.coefOthers * c88.OTHERS + a.coefBank * c88.BANK ) * COALESCE(PBT_RWA_TREND_5YR,0) +
          ( a.coefFinance * c9.FINANCE  + a.coefOthers * c9.OTHERS  + a.coefBank * c9.BANK  ) * COALESCE(ROE_NTM,0) +
          ( a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS + a.coefBank * c10.BANK ) * COALESCE(DIV_YLD_NTM,0) +
          ( a.coefFinance * c11.FINANCE + a.coefOthers * c11.OTHERS + a.coefBank * c11.BANK ) * COALESCE(PE_NTM_INVERSE,0) +
          ( a.coefFinance * c12.FINANCE + a.coefOthers * c12.OTHERS + a.coefBank * c12.BANK ) * COALESCE(PE_ON_AVG5Y_INVERSE,0) +
          ( a.coefFinance * c13.FINANCE + a.coefOthers * c13.OTHERS + a.coefBank * c13.BANK ) * COALESCE(PE_VS_IND_ON_AVG5Y_INVERSE,0)  +
          ( a.coefFinance * c14.FINANCE + a.coefOthers * c14.OTHERS + a.coefBank * c14.BANK ) * COALESCE(PB_NTM_INVERSE,0) +
          ( a.coefFinance * c15.FINANCE + a.coefOthers * c15.OTHERS + a.coefBank * c15.BANK ) * COALESCE(PB_ON_AVG5Y_INVERSE,0) +
          ( a.coefFinance * c16.FINANCE + a.coefOthers * c16.OTHERS + a.coefBank * c16.BANK ) * COALESCE(a.PB_VS_IND_ON_AVG5Y_INVERSE,0)  ) 
        / ( a.coefFinance * c1.FINANCE  + a.coefOthers * c1.OTHERS  + a.coefBank * c1.BANK + a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS  + a.coefBank * c2.BANK +  a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS  + a.coefBank * c3.BANK + a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS + a.coefBank * c4.BANK + a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS  + a.coefBank * c5.BANK + a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS  + a.coefBank * c6.BANK + a.coefFinance * c7.FINANCE + a.coefOthers * c7.OTHERS  + a.coefBank * c7.BANK + a.coefFinance * c8.FINANCE + a.coefOthers * c8.OTHERS  + a.coefBank * c8.BANK + a.coefFinance * c77.FINANCE + a.coefOthers * c77.OTHERS  + a.coefBank * c77.BANK + a.coefFinance * c88.FINANCE + a.coefOthers * c88.OTHERS  + a.coefBank * c88.BANK + a.coefFinance * c9.FINANCE + a.coefOthers * c9.OTHERS  + a.coefBank * c9.BANK + a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS  + a.coefBank * c10.BANK + a.coefFinance * c11.FINANCE + a.coefOthers * c11.OTHERS  + a.coefBank * c11.BANK + a.coefFinance * c12.FINANCE + a.coefOthers * c12.OTHERS  + a.coefBank * c12.BANK + a.coefFinance * c13.FINANCE + a.coefOthers * c13.OTHERS  + a.coefBank * c13.BANK + a.coefFinance * c14.FINANCE + a.coefOthers * c14.OTHERS  + a.coefBank * c4.BANK + a.coefFinance * c15.FINANCE + a.coefOthers * c15.OTHERS  + a.coefBank * c15.BANK + a.coefFinance * c16.FINANCE + a.coefOthers * c16.OTHERS  + a.coefBank * c16.BANK) As '1',
    
 
		--( ( a.coefFinance * c1.FINANCE + a.coefOthers * c1.OTHERS + a.coefBank * c1.BANK ) * COALESCE(SALES_GROWTH_NTM,0)  + 
        --  ( a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS + a.coefBank * c2.BANK ) * COALESCE(SALES_RSD,0)  +
        --  ( a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS + a.coefBank * c3.BANK) * COALESCE(SALES_TREND_5YR,0) +
        --  ( a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS + a.coefBank * c4.BANK) * COALESCE(EPS_GROWTH_NTM,0) +
        --  ( a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS + a.coefBank * c5.BANK) * COALESCE(EPS_RSD,0) +
        --  ( a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS + a.coefBank * c6.BANK) * COALESCE(EPS_TREND_5YR,0)  ) 
        -- / 
        ( a.coefFinance * c1.FINANCE  + a.coefOthers * c1.OTHERS  + a.coefBank * c1.BANK + a.coefFinance * c2.FINANCE + a.coefOthers * c2.OTHERS  + a.coefBank * c2.BANK +  a.coefFinance * c3.FINANCE + a.coefOthers * c3.OTHERS  + a.coefBank * c3.BANK + a.coefFinance * c4.FINANCE + a.coefOthers * c4.OTHERS + a.coefBank * c4.BANK + a.coefFinance * c5.FINANCE + a.coefOthers * c5.OTHERS  + a.coefBank * c5.BANK + a.coefFinance * c6.FINANCE + a.coefOthers * c6.OTHERS  + a.coefBank * c6.BANK) As '2'
    /*

		( ( a.coefFinance * c7.FINANCE  + a.coefOthers * c7.OTHERS  + a.coefBank * c7.BANK ) * COALESCE(EBIT_MARGIN_RSD,0) +
          ( a.coefFinance * c8.FINANCE  + a.coefOthers * c8.OTHERS  + a.coefBank * c8.BANK) * COALESCE(EBIT_MARGIN_TREND_5YR,0) +
          ( a.coefFinance * c77.FINANCE + a.coefOthers * c77.OTHERS + a.coefBank * c77.BANK) * COALESCE(PBT_RWA_RSD,0) +
          ( a.coefFinance * c88.FINANCE + a.coefOthers * c88.OTHERS + a.coefBank * c88.BANK) * COALESCE(PBT_RWA_TREND_5YR,0) +
          ( a.coefFinance * c9.FINANCE  + a.coefOthers * c9.OTHERS  + a.coefBank * c9.BANK) * COALESCE(ROE_NTM,0)   ) 
        / ( a.coefFinance * c7.FINANCE  + a.coefOthers * c7.OTHERS  + a.coefBank * c7.BANK + a.coefFinance * c8.FINANCE + a.coefOthers * c8.OTHERS  + a.coefBank * c8.BANK + a.coefFinance * c77.FINANCE + a.coefOthers * c77.OTHERS  + a.coefBank * c77.BANK + a.coefFinance * c88.FINANCE + a.coefOthers * c88.OTHERS  + a.coefBank * c88.BANK + a.coefFinance * c9.FINANCE + a.coefOthers * c9.OTHERS  + a.coefBank * c9.BANK ) As '3'
    
 
		( ( a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS + a.coefBank * c10.BANK ) * COALESCE(DIV_YLD_NTM,0) +
          ( a.coefFinance * c11.FINANCE + a.coefOthers * c11.OTHERS + a.coefBank * c11.BANK) * COALESCE(PE_NTM_INVERSE,0) +
          ( a.coefFinance * c12.FINANCE + a.coefOthers * c12.OTHERS + a.coefBank * c12.BANK ) * COALESCE(PE_ON_AVG5Y_INVERSE,0) +
          ( a.coefFinance * c13.FINANCE + a.coefOthers * c13.OTHERS + a.coefBank * c13.BANK ) * COALESCE(PE_VS_IND_ON_AVG5Y_INVERSE,0)  +
          ( a.coefFinance * c14.FINANCE + a.coefOthers * c14.OTHERS + a.coefBank * c14.BANK ) * COALESCE(PB_NTM_INVERSE,0) +
          ( a.coefFinance * c15.FINANCE + a.coefOthers * c15.OTHERS + a.coefBank * c15.BANK ) * COALESCE(PB_ON_AVG5Y_INVERSE,0)+
          ( a.coefFinance * c16.FINANCE + a.coefOthers * c16.OTHERS + a.coefBank * c16.BANK ) * COALESCE(PB_VS_IND_ON_AVG5Y_INVERSE,0)  ) 
        / ( a.coefFinance * c10.FINANCE + a.coefOthers * c10.OTHERS + a.coefBank * c10.BANK + a.coefFinance * c11.FINANCE + a.coefOthers * c11.OTHERS + a.coefBank * c11.BANK + a.coefFinance * c12.FINANCE + a.coefOthers * c12.OTHERS  + a.coefBank * c12.BANK + a.coefFinance * c13.FINANCE + a.coefOthers * c13.OTHERS  + a.coefBank * c13.BANK + a.coefFinance * c14.FINANCE + a.coefOthers * c14.OTHERS  + a.coefBank * c14.BANK + a.coefFinance * c15.FINANCE + a.coefOthers * c15.OTHERS  + a.coefBank * c15.BANK + a.coefFinance * c16.FINANCE + a.coefOthers * c16.OTHERS  + a.coefBank * c16.BANK) As '4'
        */
    
from  #agrege as a ,
ACT_DATA_FACTSET_COEF as c1 ,ACT_DATA_FACTSET_COEF as c2 ,ACT_DATA_FACTSET_COEF as c3, ACT_DATA_FACTSET_COEF as c4 ,ACT_DATA_FACTSET_COEF as c5, ACT_DATA_FACTSET_COEF as c6 , ACT_DATA_FACTSET_COEF as c7, ACT_DATA_FACTSET_COEF as c8, ACT_DATA_FACTSET_COEF as c77, ACT_DATA_FACTSET_COEF as c88 ,ACT_DATA_FACTSET_COEF as c9,ACT_DATA_FACTSET_COEF as c10 ,ACT_DATA_FACTSET_COEF as c11,ACT_DATA_FACTSET_COEF as c12,ACT_DATA_FACTSET_COEF as c13 ,ACT_DATA_FACTSET_COEF as c14 ,ACT_DATA_FACTSET_COEF as c15 ,ACT_DATA_FACTSET_COEF as c16
where   c1.indicator = 'SALES_GROWTH_NTM' and  c1.date= @date and c1.portfolio='BlendSecteur' 
	and c2.indicator = 'SALES_RSD' and  c2.date= @date and c2.portfolio='BlendSecteur' 
	and c3.indicator = 'SALES_TREND_5YR' and  c3.date= @date and c3.portfolio='BlendSecteur' 
	and c4.indicator = 'EPS_GROWTH_NTM' and  c4.date= @date and c4.portfolio='BlendSecteur' 
	and c5.indicator = 'EPS_RSD' and  c5.date= @date and c5.portfolio='BlendSecteur' 
	and c6.indicator = 'EPS_TREND_5YR' and  c6.date= @date and c6.portfolio='BlendSecteur' 
	and c7.indicator = 'EBIT_MARGIN_RSD' and  c7.date= @date and c7.portfolio='BlendSecteur' 
	and c8.indicator = 'EBIT_MARGIN_TREND_5YR' and  c8.date= @date and c8.portfolio='BlendSecteur' 
	and c77.indicator = 'PBT_RWA_RSD' and  c77.date= @date and c77.portfolio='BlendSecteur' 
	and c88.indicator = 'PBT_RWA_TREND_5YR' and  c88.date= @date and c88.portfolio='BlendSecteur' 
	and c9.indicator = 'ROE_NTM' and  c9.date= @date and c9.portfolio='BlendSecteur' 	
	and c10.indicator = 'DIV_YLD_NTM' and  c10.date= @date and c10.portfolio='BlendSecteur' 
	and c11.indicator = 'PE_NTM_INVERSE' and  c11.date= @date and c11.portfolio='BlendSecteur' 
	and c12.indicator = 'PE_ON_AVG5Y_INVERSE' and  c12.date= @date and c12.portfolio='BlendSecteur'
	and c13.indicator = 'PE_VS_IND_ON_AVG5Y_INVERSE' and c13.date=@date and c13.portfolio='BlendSecteur'  
	and c14.indicator = 'PB_NTM_INVERSE' and  c14.date= @date and c14.portfolio='BlendSecteur' 
	and c15.indicator = 'PB_ON_AVG5Y_INVERSE' and  c15.date= @date and c15.portfolio='BlendSecteur' 							
	and c16.indicator = 'PB_VS_IND_ON_AVG5Y_INVERSE' and  c16.date= @date and c16.portfolio='BlendSecteur' 
	--select * from #agrege

*/


