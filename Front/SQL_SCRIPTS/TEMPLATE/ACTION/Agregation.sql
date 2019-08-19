--Création de la ligne global pour un poids et un secteur donné
DECLARE @id_sector AS VARCHAR(5)
SET @id_sector = 'mon_secteur'
DECLARE @type AS VARCHAR(10)
SET @type = 'mon_type'
DECLARE @date AS DATETIME
SET @date = 'ma_date'
DECLARE @libelle_sector AS VARCHAR(60)
SET @libelle_sector =(SELECT libelle FROM ACT_SECTEUR where id=@id_sector)

SELECT d.*, s.libelle AS sector, t.TICKER_BLOOMBERG As ticker INTO #factset FROM ACT_DATA_FACTSET d
LEFT OUTER JOIN ACT_SECTEUR s ON d.icb_sector = s.id  
LEFT OUTER JOIN ACT_TICKER t ON d.isin = t.isin and t.date=(SELECT MAX(date) FROM ACT_TICKER where date <= '14/02/2012')
WHERE d.date= @date
--SELECT * FROM ACT_SECTEUR

CREATE TABLE #ref(
	indicator VARCHAR(120),
	isin VARCHAR(12),
	compagny_name VARCHAR(160),
	icb_sector VARCHAR(4),
	sector VARCHAR(120),
	stoxx_600 FLOAT,
	eurostoxx FLOAT,
	stoxx_600_ex_euro FLOAT,
	value FLOAT,
)
	
INSERT INTO #ref SELECT 'MARKET_CAP_EURO', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, MARKET_CAP_EURO FROM #factset			
INSERT INTO #ref SELECT 'SALES_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, SALES_YEAR FROM #factset
INSERT INTO #ref SELECT 'SALES_YEAR_GROWTH', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, SALES_YEAR_GROWTH FROM #factset
INSERT INTO #ref SELECT 'SALES_NEXT_YEAR_GROWTH', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, SALES_NEXT_YEAR_GROWTH FROM #factset
INSERT INTO #ref SELECT 'EPS_YEAR_GROWTH', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, EPS_YEAR_GROWTH FROM #factset
INSERT INTO #ref SELECT 'EPS_NEXT_YEAR_GROWTH', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, EPS_NEXT_YEAR_GROWTH FROM #factset
INSERT INTO #ref SELECT 'EBIT_MARGIN_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, EBIT_MARGIN_YEAR FROM #factset
INSERT INTO #ref SELECT 'EBIT_MARGIN_NEXT_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, EBIT_MARGIN_NEXT_YEAR FROM #factset
INSERT INTO #ref SELECT 'ROE_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, ROE_YEAR FROM #factset
INSERT INTO #ref SELECT 'ROE_NEXT_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, ROE_NEXT_YEAR FROM #factset
INSERT INTO #ref SELECT 'DIV_YIELD_NTM', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, DIV_YIELD_NTM FROM #factset
INSERT INTO #ref SELECT 'PE_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, PE_YEAR FROM #factset
INSERT INTO #ref SELECT 'PE_NEXT_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, PE_NEXT_YEAR FROM #factset
INSERT INTO #ref SELECT 'PB_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, PB_YEAR FROM #factset
INSERT INTO #ref SELECT 'PB_NEXT_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, PB_NEXT_YEAR FROM #factset
INSERT INTO #ref SELECT 'EV_EBITDA_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, EV_EBITDA_YEAR FROM #factset
INSERT INTO #ref SELECT 'EV_EBITDA_NEXT_YEAR', isin, company_name, icb_sector, sector, stoxx_600, eurostoxx, stoxx_600_ex_euro, EV_EBITDA_NEXT_YEAR FROM #factset
--SELECT * FROM #ref		






--Affichage
IF @type = 'secteur' 
BEGIN
	SELECT	
			'* mon_indice *' As 'Company Name', 
			--(SELECT SUM(mon_indice) FROM #factset WHERE date=@date) As 'Poids',
			NULL As 'Ticker',
			'X' As 'Liquidité',
			NULL As 'Pays',
			(SELECT SUM(value) FROM #ref WHERE indicator='MARKET_CAP_EURO'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Market Cap Mds €',
			(SELECT SUM(value) FROM #ref WHERE indicator='SALES_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'Ventes FY Mds €', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='SALES_YEAR_GROWTH'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr Ventes FY %', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='SALES_NEXT_YEAR_GROWTH'	and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr Ventes FY+1 %', 				
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EPS_YEAR_GROWTH'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr BPA FY %', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EPS_NEXT_YEAR_GROWTH'		and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr BPA FY+1 %', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EBIT_MARGIN_YEAR'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Marge EBIT FY', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EBIT_MARGIN_NEXT_YEAR'		and value IS NOT NULL and mon_indice IS NOT NULL) As 'Marge EBIT FY+1', 
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_YEAR'					and value IS NOT NULL and mon_indice IS NOT NULL)/ (SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_YEAR'      and value IS NOT NULL and mon_indice IS NOT NULL)*100 As 'ROE FY',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_NEXT_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL)/ (SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_NEXT_YEAR' and value IS NOT NULL and mon_indice IS NOT NULL)*100 As 'ROE FY+1',
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='DIV_YIELD_NTM'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'Rdt', 
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_YEAR'					and value IS NOT NULL and mon_indice IS NOT NULL) As 'PE FY x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_NEXT_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'PE FY+1 x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_YEAR'					and value IS NOT NULL and mon_indice IS NOT NULL) As 'PB FY x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_NEXT_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'PB FY+1 x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='EV_EBITDA_YEAR'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'EV/EBITDA FY x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='EV_EBITDA_NEXT_YEAR'		and value IS NOT NULL and mon_indice IS NOT NULL) As 'EV/EBITDA FY+1 x',
			NULL As 'Note Isr'
	UNION
	SELECT	
			'**'+ @libelle_sector +'**' As 'Company Name', 
			--(SELECT SUM(mon_indice) FROM #factset WHERE date=@date and icb_sector=@id_sector) As 'Poids',
			NULL AS 'Ticker',
			'X' As 'Liquidité',
			NULL As 'Pays',
			(SELECT SUM(value) FROM #ref WHERE indicator='MARKET_CAP_EURO'			and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'Market Cap Mds €',
			(SELECT SUM(value) FROM #ref WHERE indicator='SALES_YEAR'				and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'Ventes FY Mds €', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='SALES_YEAR_GROWTH'			and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'Cr Ventes FY %', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='SALES_NEXT_YEAR_GROWTH'	and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'Cr Ventes FY+1 %', 				
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EPS_YEAR_GROWTH'			and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'Cr BPA FY %', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EPS_NEXT_YEAR_GROWTH'		and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'Cr BPA FY+1 %', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EBIT_MARGIN_YEAR'			and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'Marge EBIT FY', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EBIT_MARGIN_NEXT_YEAR'		and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'Marge EBIT FY+1', 
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_YEAR'					and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL)/ (SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_YEAR'      and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL)*100 As 'ROE FY',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_NEXT_YEAR'				and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL)/ (SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_NEXT_YEAR' and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL)*100 As 'ROE FY+1',
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='DIV_YIELD_NTM'				and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'Rdt', 
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_YEAR'					and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'PE FY x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_NEXT_YEAR'				and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'PE FY+1 x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_YEAR'					and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'PB FY x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_NEXT_YEAR'				and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'PB FY+1 x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='EV_EBITDA_YEAR'			and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'EV/EBITDA FY x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='EV_EBITDA_NEXT_YEAR'		and value IS NOT NULL and icb_sector=@id_sector and mon_indice IS NOT NULL) As 'EV/EBITDA FY+1 x',
			NULL As 'Note Isr'
	UNION
	SELECT	
			f.company_name, 
			t.TICKER_BLOOMBERG, 
			CASE 
				WHEN f.LIQUIDITY_TEST = '1' THEN 'X'
				WHEN f.LIQUIDITY_TEST = 'F'  THEN 'F'
				ELSE NULL 
			END As 'Liquidité',
			f.country As 'Pays',  
			f.MARKET_CAP_EURO As 'Market Cap Mds €',
			SALES_YEAR,
			SALES_YEAR_GROWTH,
			SALES_NEXT_YEAR_GROWTH,
			EPS_YEAR_GROWTH,
			EPS_NEXT_YEAR_GROWTH,
			EBIT_MARGIN_YEAR,
			EBIT_MARGIN_NEXT_YEAR,
			ROE_YEAR,
			ROE_NEXT_YEAR,
			DIV_YIELD_NTM,
			PE_YEAR,
			PE_NEXT_YEAR,
			PB_YEAR,
			PB_NEXT_YEAR,
			EV_EBITDA_YEAR,
			EV_EBITDA_NEXT_YEAR,
			CASE 
				WHEN 'mon_indice' = 'STOXX_600' THEN i.europe
				WHEN 'mon_indice' = 'EUROSTOXX' THEN i.euro
				ELSE i.exEuro 
			END As 'Note'
			--CASE WHEN f.eurozone IS NULL  THEN i.europe ELSE i.euro END AS 'note'  
	FROM #factset f 
	LEFT OUTER JOIN ACT_TICKER t ON f.isin = t.isin AND t.date= (SELECT TOP 1 date FROM ACT_TICKER where date <= @date) 
	LEFT OUTER JOIN ISR_NOTE i on i.sedol=f.sedol and i.date=@date
	WHERE f.icb_sector=@id_sector and f.date=@date and f.mon_indice IS NOT NULL ORDER BY 'Market Cap Mds €' DESC 
END 

	
	
	

IF @type = 'isin' 
BEGIN
		SELECT	
			'* mon_indice *' As 'Company Name', 
			NULL AS 'Ticker',
			'X' As 'Liquidité',
			NULL As 'Pays',
			(SELECT SUM(value) FROM #ref WHERE indicator='MARKET_CAP_EURO'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Market Cap Mds €',
			(SELECT SUM(value) FROM #ref WHERE indicator='SALES_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'Ventes FY Mds €', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='SALES_YEAR_GROWTH'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr Ventes FY %', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='SALES_NEXT_YEAR_GROWTH'	and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr Ventes FY+1 %', 				
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EPS_YEAR_GROWTH'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr BPA FY %', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EPS_NEXT_YEAR_GROWTH'		and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr BPA FY+1 %', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EBIT_MARGIN_YEAR'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Marge EBIT FY', 
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EBIT_MARGIN_NEXT_YEAR'		and value IS NOT NULL and mon_indice IS NOT NULL) As 'Marge EBIT FY+1', 
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_YEAR'					and value IS NOT NULL and mon_indice IS NOT NULL)/ (SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_YEAR'      and value IS NOT NULL and mon_indice IS NOT NULL)*100 As 'ROE FY',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_NEXT_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL)/ (SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_NEXT_YEAR' and value IS NOT NULL and mon_indice IS NOT NULL)*100 As 'ROE FY+1',
			(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='DIV_YIELD_NTM'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'Rdt', 
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_YEAR'					and value IS NOT NULL and mon_indice IS NOT NULL) As 'PE FY x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_NEXT_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'PE FY+1 x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_YEAR'					and value IS NOT NULL and mon_indice IS NOT NULL) As 'PB FY x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_NEXT_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'PB FY+1 x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='EV_EBITDA_YEAR'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'EV/EBITDA FY x',
			(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='EV_EBITDA_NEXT_YEAR'		and value IS NOT NULL and mon_indice IS NOT NULL) As 'EV/EBITDA FY+1 x',
			NULL As 'Note Isr'
	UNION
	SELECT	
			f.company_name, 
			t.TICKER_BLOOMBERG,
			CASE 
				WHEN LIQUIDITY_TEST = '1' THEN 'X'
				WHEN LIQUIDITY_TEST = 'F'  THEN 'F'
				ELSE NULL 
			END As 'Liquidité',
			f.country As 'Pays',  
			f.MARKET_CAP_EURO As 'Market Cap Mds €',
			SALES_YEAR,
			SALES_YEAR_GROWTH,
			SALES_NEXT_YEAR_GROWTH,
			EPS_YEAR_GROWTH,
			EPS_NEXT_YEAR_GROWTH,
			EBIT_MARGIN_YEAR,
			EBIT_MARGIN_NEXT_YEAR,
			ROE_YEAR,
			ROE_NEXT_YEAR,
			DIV_YIELD_NTM,
			PE_YEAR,
			PE_NEXT_YEAR,
			PB_YEAR,
			PB_NEXT_YEAR,
			EV_EBITDA_YEAR,
			EV_EBITDA_NEXT_YEAR,
			CASE 
				WHEN 'mon_indice' = 'STOXX_600' THEN i.europe
				WHEN 'mon_indice' = 'EUROSTOXX' THEN i.euro
				ELSE i.exEuro 
			END As 'Note'  
	FROM #factset f 
	LEFT OUTER JOIN ACT_TICKER t ON f.isin = t.isin AND t.date= (SELECT TOP 1 date FROM ACT_TICKER where date <= @date)
	LEFT OUTER JOIN ISR_NOTE i on i.sedol=f.sedol and i.date=@date
	WHERE f.date=@date and f.mon_indice IS NOT NULL ORDER BY 'Market Cap Mds €' DESC 
END


IF @type = 'secteurs' 
BEGIN

	SELECT indicator, icb_sector, sector , SUM(value) as Value, mon_indice	
	INTO #tempCalc
	FROM 
	(
		SELECT indicator, icb_sector, sector,
		CASE  
			WHEN indicator IN ('MARKET_CAP_EURO','SALES_YEAR') THEN value
			WHEN indicator IN ('CAPEX_ON_SALES_NTM','INTERNAL_GROWTH_NTM','GROWTH_ANNUALIZED','DIV_YIELD_NTM','EBIT_MARGIN_NTM','EBIT_MARGIN_YEAR','EBIT_MARGIN_NEXT_YEAR','EPS_YEAR_GROWTH','EPS_NEXT_YEAR_GROWTH','SALES_YEAR_GROWTH','SALES_NEXT_YEAR_GROWTH') THEN (mon_indice*value)
			WHEN indicator IN ('PE_NTM','PE_NTM_AVG_5','PE_NTM_AVG_10','PE_YEAR','PE_NEXT_YEAR','PB_NTM','PB_NTM_AVG_5','PB_NTM_AVG_10','PB_YEAR','PB_NEXT_YEAR','EV_EBITDA_NTM','EV_EBITDA_NTM_AVG_5','EV_EBITDA_NTM_AVG_10','EV_EBITDA_YEAR','EV_EBITDA_NEXT_YEAR','SALES_YEAR') THEN (mon_indice/value)
			ELSE 0
		END AS 'value',
		mon_indice
	FROM #ref WHERE value IS NOT NULL and mon_indice IS NOT NULL 	
	) AS t GROUP BY indicator,icb_sector,sector,mon_indice ORDER BY sector


	SELECT indicator, icb_sector, sector, SUM(mon_indice) AS poids,
	CASE
		WHEN indicator IN ('MARKET_CAP_EURO','SALES_YEAR') THEN SUM(value)
		WHEN indicator IN ('CAPEX_ON_SALES_NTM','INTERNAL_GROWTH_NTM','GROWTH_ANNUALIZED','DIV_YIELD_NTM','EBIT_MARGIN_NTM','EBIT_MARGIN_YEAR','EBIT_MARGIN_NEXT_YEAR','EPS_YEAR_GROWTH','EPS_NEXT_YEAR_GROWTH','SALES_YEAR_GROWTH','SALES_NEXT_YEAR_GROWTH') THEN SUM(Value)/SUM(mon_indice)
		WHEN indicator IN ('PE_NTM','PE_NTM_AVG_5','PE_NTM_AVG_10','PE_YEAR','PE_NEXT_YEAR','PB_NTM','PB_NTM_AVG_5','PB_NTM_AVG_10','PB_YEAR','PB_NEXT_YEAR','EV_EBITDA_NTM','EV_EBITDA_NTM_AVG_5','EV_EBITDA_NTM_AVG_10','EV_EBITDA_YEAR','EV_EBITDA_NEXT_YEAR') THEN SUM(mon_indice) / SUM(Value)
		ELSE 0
	END AS 'value'
	INTO #tempCalc2
	FROM #tempCalc group by indicator,icb_sector,sector ORDER BY sector
	--SELECT * FROM #tempCalc2

	SELECT	
				'* mon_indice *' As 'Company Name', 
				(SELECT SUM(value) FROM #ref WHERE indicator='MARKET_CAP_EURO'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Market Cap Mds €',
				(SELECT SUM(value) FROM #ref WHERE indicator='SALES_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'Ventes FY Mds €', 
				(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='SALES_YEAR_GROWTH'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr Ventes FY %', 
				(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='SALES_NEXT_YEAR_GROWTH'	and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr Ventes FY+1 %', 				
				(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EPS_YEAR_GROWTH'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr BPA FY %', 
				(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EPS_NEXT_YEAR_GROWTH'		and value IS NOT NULL and mon_indice IS NOT NULL) As 'Cr BPA FY+1 %', 
				(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EBIT_MARGIN_YEAR'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'Marge EBIT FY', 
				(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='EBIT_MARGIN_NEXT_YEAR'		and value IS NOT NULL and mon_indice IS NOT NULL) As 'Marge EBIT FY+1', 
				(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_YEAR'					and value IS NOT NULL and mon_indice IS NOT NULL)/ (SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_YEAR'      and value IS NOT NULL and mon_indice IS NOT NULL)*100 As 'ROE FY',
				(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_NEXT_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL)/ (SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_NEXT_YEAR' and value IS NOT NULL and mon_indice IS NOT NULL)*100 As 'ROE FY+1',
				(SELECT SUM(mon_indice*value)/SUM(mon_indice) FROM #ref WHERE indicator='DIV_YIELD_NTM'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'Rdt', 
				(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_YEAR'					and value IS NOT NULL and mon_indice IS NOT NULL) As 'PE FY x',
				(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PE_NEXT_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'PE FY+1 x',
				(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_YEAR'					and value IS NOT NULL and mon_indice IS NOT NULL) As 'PB FY x',
				(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='PB_NEXT_YEAR'				and value IS NOT NULL and mon_indice IS NOT NULL) As 'PB FY+1 x',
				(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='EV_EBITDA_YEAR'			and value IS NOT NULL and mon_indice IS NOT NULL) As 'EV/EBITDA FY x',
				(SELECT SUM(mon_indice)/SUM(mon_indice/value) FROM #ref WHERE indicator='EV_EBITDA_NEXT_YEAR'		and value IS NOT NULL and mon_indice IS NOT NULL) As 'EV/EBITDA FY+1 x'
	UNION
	SELECT 
		'**'+i1.sector+'**' AS 'Company Name', 
		--(SELECT SUM(mon_indice) FROM #factset WHERE icb_sector=i1.icb_sector) As 'Poids' , 
		i1.value as 'MARKET_CAP_EURO',
		i2.value as 'SALES_YEAR',		
		i3.value as 'SALES_YEAR_GROWTH',
		i4.value as 'SALES_NEXT_YEAR_GROWTH',
		i5.value as 'EPS_YEAR_GROWTH',
		i6.value as 'EPS_NEXT_YEAR_GROWTH',
		i7.value as 'EBIT_MARGIN_YEAR',
		i8.value as 'EBIT_MARGIN_NEXT_YEAR',
		i14.value/i12.value*100 as 'ROE_YEAR',
		i15.value/i13.value*100 as 'ROE_NEXT_YEAR',
		i11.value as 'DIV_YIELD_NTM',
		i12.value as 'PE_YEAR',		
		i13.value as 'PE_NEXT_YEAR',
		i14.value as 'PB_YEAR',
		i15.value as 'PB_NEXT_YEAR',
		i16.value as 'EV_EBITDA_YEAR',		
		i17.value as 'EV_EBITDA_NEXT_YEAR'
	FROM #tempCalc2 as i1
	left outer join #tempCalc2 as i2  on i2.sector = i1.sector and i2.indicator = 'SALES_YEAR'	
	left outer join #tempCalc2 as i3  on i3.sector = i1.sector and i3.indicator = 'SALES_YEAR_GROWTH'
	left outer join #tempCalc2 as i4  on i4.sector = i1.sector and i4.indicator = 'SALES_NEXT_YEAR_GROWTH'
	left outer join #tempCalc2 as i5  on i5.sector = i1.sector and i5.indicator = 'EPS_YEAR_GROWTH'
	left outer join #tempCalc2 as i6  on i6.sector = i1.sector and i6.indicator = 'EPS_NEXT_YEAR_GROWTH'
	left outer join #tempCalc2 as i7  on i7.sector = i1.sector and i7.indicator = 'EBIT_MARGIN_YEAR'
	left outer join #tempCalc2 as i8  on i8.sector = i1.sector and i8.indicator = 'EBIT_MARGIN_NEXT_YEAR'
	left outer join #tempCalc2 as i11 on i11.sector = i1.sector and i11.indicator = 'DIV_YIELD_NTM'
	left outer join #tempCalc2 as i12 on i12.sector = i1.sector and i12.indicator = 'PE_YEAR'
	left outer join #tempCalc2 as i13 on i13.sector = i1.sector and i13.indicator = 'PE_NEXT_YEAR'
	left outer join #tempCalc2 as i14 on i14.sector = i1.sector and i14.indicator = 'PB_YEAR'
	left outer join #tempCalc2 as i15 on i15.sector = i1.sector and i15.indicator = 'PB_NEXT_YEAR'	
	left outer join #tempCalc2 as i16 on i16.sector = i1.sector and i16.indicator = 'EV_EBITDA_YEAR'
	left outer join #tempCalc2 as i17 on i17.sector = i1.sector and i17.indicator = 'EV_EBITDA_NEXT_YEAR'
	where i1.indicator = 'MARKET_CAP_EURO' 

	DROP TABLE #tempCalc
	DROP TABLE #tempCalc2
END


DROP TABLE #factset
DROP TABLE #ref
