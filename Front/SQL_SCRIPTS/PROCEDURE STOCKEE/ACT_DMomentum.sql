ALTER PROCEDURE ACT_DMomentum

	@date As DATETIME

AS

--EXECUTE ACT_DMomentum '02/03/2012'


SELECT 
	ss.libelle As 'ICB Sector', 
	COMPANY_NAME As 'Company Name', 
	currency As 'Crncy', 
	t.ticker_bloomberg As 'Ticker', 
	MARKET_CAP_EUR As 'Mkt Cap m€',
	s.libelle As ' ICB SuperSector', 
	PERF_1M As 'Perf 1M', PERF_3M  As 'Perf 3M', PERF_6M As 'Perf 6M', PERF_1YR  As 'Perf 1Y', PERF_MTD  As 'Perf MTD', PERF_YTD  As 'Perf YTD', 
	VOL_1M  As 'Vol 1M', VOL_3M  As 'Vol 3M', VOL_1YR  As 'Vol 1Y', 
	BETA_1YR   As 'Beta 3Y', 
	PRICE_PCTIL_1M As 'Price 1M', PRICE_PCTIL_3M As 'Price 3M', PRICE_PCTIL_1YR As 'Price 1Y', PRICE_PCTIL_5YR As 'Price 5Y', PRICE As 'Price', PRICE_52_HIGH As 'Price 52 high', PRICE_52_LOW As 'Price 52 low', 
	TARGET As 'Target', TARGET_FB As 'Target FB', TARGET_UPDOWN_FB As 'Target UpDown FB',
	EPS_CHG_1M As 'EPS Rev 1M', EPS_CHG_3M As 'EPS Rev 3M', EPS_CHG_6M As 'EPS Rev 6M', EPS_CHG_1YR As 'EPS Rev 1Y', EPS_CHG_YTD As 'EPS Rev YTD',
	RATING_POS_PCT As ' Rating pos', RATING_POS_PCT_FB As 'Rating pos FB', RATING_TOT_FB As 'Rating total FB'
FROM ACT_DATA_FACTSET f
	LEFT OUTER JOIN ACT_SUPERSECTOR s ON f.ICB_SUPERSECTOR = s.id 
	LEFT OUTER JOIN ACT_SECTOR ss ON f.ICB_SECTOR = ss.id  
	LEFT OUTER JOIN ACT_VALEUR t ON f.isin = t.isin
WHERE f.date=@date
ORDER BY 'Mkt Cap m€' DESC
