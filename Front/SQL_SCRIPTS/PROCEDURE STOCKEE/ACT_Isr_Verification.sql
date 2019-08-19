-- écrit les isins, et libellés de factset qui ne sont pas dans le fichier isr de paul

DROP PROCEDURE ACT_Isr_Verification

GO

CREATE PROCEDURE ACT_Isr_Verification
		@date DATETIME
AS 

--DECLARE @date As DATETIME
--SET @date='30/12/2011'
--EXECUTE ACT_Isr_Verification '30/12/2011'

SELECT isin INTO #tmp FROM ACT_DATA_FACTSET WHERE date=(SELECT DISTINCT TOP 1 date FROM ACT_DATA_FACTSET where date <= @date order by date DESC) 
EXCEPT 
SELECT isin FROM ISR_NOTE WHERE date=@date 
--SELECT * from #tmp

SELECT DISTINCT 
	d.isin AS 'isin', 
	d.company_name AS 'COMPANY NAME' 
into #tmp2 
FROM #tmp t, ACT_DATA_FACTSET d
WHERE 
	t.isin = d.isin and date=(SELECT DISTINCT TOP 1 date FROM ACT_DATA_FACTSET where date <= @date order by date DESC)
order by d.company_name

SELECT  * FROM #tmp2

DROP TABLE #tmp
DROP TABLE #tmp2

/*
SELECT DISTINCT * FROM ISR_NOTE where name like '%glencore%' and date ='08/07/2012'
SELECT * FROM  ACT_DATA_FACTSET where date='06/07/2012' and company_name like '%glencore%'
SELECT MAX(date) FROm ACT_DATA_FACTSET
*/