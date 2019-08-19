/*
 Obtient les plus grands décalages de ZSpread entre 2 dates
*/
       
GO

CREATE PROCEDURE IboxxZSpread

       @debut DATETIME,
	   @fin   DATETIME
AS 

/*
SELECT isin, issuerName, Zspread INTO #old FROM TX_IBOXX WHERE date = @debut
SELECT isin, issuerName, Zspread INTO #new FROM TX_IBOXX WHERE date = @fin

SELECT 
	n.isin As 'ISIN', 
	n.issuerName AS 'Libellé', 
	FLOOR(n.Zspread - o.Zspread) AS 'ZSpread Variation', 
	ABS(ROUND((n.Zspread - o.Zspread)/o.Zspread,4))*100 AS '|ZSpread|%',
	n.Zspread As 'ZSpread t',	
	o.Zspread As 'ZSpread t-1'
INTO #test 
FROM  #old o, #new n
WHERE n.isin = o.isin 
ORDER BY ABS(FLOOR(n.Zspread - o.Zspread)) DESC

SELECT TOP 10 * FROM #test 

DROP TABLE #old
DROP TABLE #new
DROP TABLE #test
*/

SELECT 
	n.isin As 'ISIN', 
	n.issuerName AS 'Libellé',
	n.level4 As 'Level 4',
	n.country As 'Pays',
	n.rating As 'Rating',
	n.MaturityDate As 'Maturité',
	CASE WHEN n.tier='*' THEN 'Senior' Else n.tier End As 'Dette' ,
	CONVERT(decimal(38,2),o.IndexWeight) As 'Poids',
	CONVERT(decimal(38,2),o.AnnualYield) As 'Taux', 
	FLOOR(n.Zspread - o.Zspread) AS 'ZSpread -', 
	CONVERT(decimal(38,2), ABS((n.Zspread - o.Zspread)/o.Zspread*100) ) AS '|ZSpread|%',
	FLOOR(n.Zspread) As 'ZSpread t',	
	FLOOR(o.Zspread) As 'ZSpread t-1'
FROM  TX_IBOXX o, TX_IBOXX n
WHERE n.isin = o.isin  and n.date=@fin and o.date=@debut
ORDER BY FLOOR(n.Zspread - o.Zspread) DESC
