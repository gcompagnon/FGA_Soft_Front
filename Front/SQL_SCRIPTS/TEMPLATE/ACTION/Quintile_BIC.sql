-- *** = date
-- #sector = sector_icb present cette date

SELECT 
	a.isin,
	a.COMPANY_NAME, 
	a.ICB_SECTOR,
	a.GARPN_TOTAL_S,
	a.GARPO_TOTAL_S,
	ROW_NUMBER() OVER (ORDER BY GARPO_TOTAL_S DESC ) AS 'O_Ranking',
	NTILE(5) OVER (ORDER BY GARPO_TOTAL_S DESC ) AS 'O_Quintile',
	ROW_NUMBER() OVER (ORDER BY GARPN_TOTAL_S DESC ) AS 'N_Ranking',
	NTILE(5) OVER (ORDER BY GARPN_TOTAL_S DESC ) AS 'N_Quintile',
	a.LIQUIDITY_TEST
INTO #quintile_S
FROM ACT_DATA_FACTSET a 
--left outer join ACT_DATA_LIQUIDITY l ON a.date = l.date and	a.isin = l.isin 	 
WHERE 
	a.date='***'   and
	--a.GARPO_TOTAL_S IS NOT NULL  and 
    --l.unions IS NOT NULL and 
    a.LIQUIDITY_TEST IN('1','F')  and 
    ICB_SECTOR = '#sector'

UPDATE ACT_DATA_FACTSET 
SET GARPO_QUINTILE_S = q.O_Quintile, 
	GARPO_RANKING_S = q.O_Ranking,
	GARPN_QUINTILE_S = q.N_Quintile, 
	GARPN_RANKING_S = q.N_Ranking
FROM ACT_DATA_FACTSET a, #quintile_S q
WHERE a.date='***' and q.isin = a.isin

DROP TABLE #quintile_S 