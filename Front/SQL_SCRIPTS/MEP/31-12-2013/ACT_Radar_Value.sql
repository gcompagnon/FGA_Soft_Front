
/****** Object:  StoredProcedure [dbo].[ACT_Radar_Value]    Script Date: 12/20/2013 17:48:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_Radar_Value]

	@Date As VARCHAR(20),
	@TICKER1 As VARCHAR(20),
	@TICKER2 As VARCHAR(20),
	@FGA As VARCHAR(20)
AS

--DECLARE @Date AS VARCHAR(20), @TICKER1 AS VARCHAR(12), @TICKER2 AS VARCHAR(12), @FGA As VARCHAR(20)
--SET @Date = '01/12/2013'
--SET @TICKER1 = 'CFR VX'
--SET @TICKER2 = 'MC FP'
--SET @FGA = '252030'

DECLARE @SQL AS VARCHAR(MAX)
DECLARE @Metric1 AS VARCHAR(10), @Score1 AS VARCHAR(10),
		@Metric2 AS VARCHAR(10), @Score2 AS VARCHAR(10),
		@Metric3 AS VARCHAR(10), @Score3 AS VARCHAR(10),
		@Metric4 AS VARCHAR(10), @Score4 AS VARCHAR(10),
		@Metric5 AS VARCHAR(10), @Score5 AS VARCHAR(10),
		@Metric6 AS VARCHAR(10), @Score6 AS VARCHAR(10)
SET @Metric1 = SUBSTRING(@TICKER1, 0, CHARINDEX(' ', @TICKER1)) + '_GM'
SET @Score1 = SUBSTRING(@TICKER1, 0, CHARINDEX(' ', @TICKER1)) + '_GS'
SET @Metric2 = SUBSTRING(@TICKER2, 0, CHARINDEX(' ', @TICKER2)) + '_GM'
SET @Score2 = SUBSTRING(@TICKER2, 0, CHARINDEX(' ', @TICKER2)) + '_GS'
SET @Metric3 = SUBSTRING(@TICKER1, 0, CHARINDEX(' ', @TICKER1)) + '_VM'
SET @Score3 = SUBSTRING(@TICKER1, 0, CHARINDEX(' ', @TICKER1)) + '_VS'
SET @Metric4 = SUBSTRING(@TICKER2, 0, CHARINDEX(' ', @TICKER2)) + '_VM'
SET @Score4 = SUBSTRING(@TICKER2, 0, CHARINDEX(' ', @TICKER2)) + '_VS'
SET @Metric5 = SUBSTRING(@TICKER1, 0, CHARINDEX(' ', @TICKER1)) + '_PM'
SET @Score5 = SUBSTRING(@TICKER1, 0, CHARINDEX(' ', @TICKER1)) + '_PS'
SET @Metric6 = SUBSTRING(@TICKER2, 0, CHARINDEX(' ', @TICKER2)) + '_PM'
SET @Score6 = SUBSTRING(@TICKER2, 0, CHARINDEX(' ', @TICKER2)) + '_PS'

SET @SQL ='
 CREATE TABLE ##values
 (
	Num INTEGER,
	GROWTH VARCHAR(50) COLLATE DATABASE_DEFAULT,
	[' + @Metric1 + '] float,
	[' + @Score1 + '] float,
	[' + @Metric2 + '] float,
	[' + @Score2 + '] float
 )
 CREATE TABLE ##values2
 (
	Num INTEGER,
	VALUE VARCHAR(50) COLLATE DATABASE_DEFAULT,
	[' + @Metric3 + '] float,
	[' + @Score3 + '] float,
	[' + @Metric4 + '] float,
	[' + @Score4 + '] float
 )
 CREATE TABLE ##values3
 (
	Num INTEGER,
	PROFIT VARCHAR(50) COLLATE DATABASE_DEFAULT,
	[' + @Metric5 + '] float,
	[' + @Score5 + '] float,
	[' + @Metric6 + '] float,
	[' + @Score6 + '] float
 )
 INSERT INTO ##values(Num) VALUES(0)
 INSERT INTO ##values2(Num) VALUES(0)
 INSERT INTO ##values3(Num) VALUES(0)
'
EXEC(@SQL)

SELECT *
INTO #values
FROM ##values
SELECT *
INTO #values2
FROM ##values2
SELECT *
INTO #values3
FROM ##values3
DROP TABLE ##values
DROP TABLE ##values2
DROP TABLE ##values3

SET @SQL ='
 INSERT INTO #values
 SELECT (select max(Num)+1 from #values) AS Num, ''EPS CHG NTM'' AS GROWTH, f1.EPS_CHG_NTM AS [' + @Metric1 + '], f1.N_EPS_CHG_NTM AS [' + @Score1 + '], f2.EPS_CHG_NTM AS [' + @Metric2 + '], f2.N_EPS_CHG_NTM AS [' + @Score2 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''

 INSERT INTO #values
 SELECT (select max(Num)+1 from #values) AS Num, ''EPS TREND'' AS GROWTH, f1.EPS_TREND AS [' + @Metric1 + '], f1.N_EPS_TREND AS [' + @Score1 + '], f2.EPS_TREND AS [' + @Metric2 + '], f2.N_EPS_TREND AS [' + @Score2 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''

 INSERT INTO #values
 SELECT (select max(Num)+1 from #values) AS Num, ''EPS VAR RSD'' AS GROWTH, f1.EPS_VAR_RSD AS [' + @Metric1 + '], f1.N_EPS_VAR_RSD AS [' + @Score1 + '], f2.EPS_VAR_RSD AS [' + @Metric2 + '], f2.N_EPS_VAR_RSD AS [' + @Score2 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''

 INSERT INTO #values
 SELECT (select max(Num)+1 from #values) AS Num, ''SALES CHG NTM'' AS GROWTH, f1.SALES_CHG_NTM AS [' + @Metric1 + '], f1.N_SALES_CHG_NTM AS [' + @Score1 + '], f2.SALES_CHG_NTM AS [' + @Metric2 + '], f2.N_SALES_CHG_NTM AS [' + @Score2 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 
 INSERT INTO #values
 SELECT (select max(Num)+1 from #values) AS Num, ''SALES TREND'' AS GROWTH, f1.SALES_TREND AS [' + @Metric1 + '], f1.N_SALES_TREND AS [' + @Score1 + '], f2.SALES_TREND AS [' + @Metric2 + '], f2.N_SALES_TREND AS [' + @Score2 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 
 INSERT INTO #values
 SELECT (select max(Num)+1 from #values) AS Num, ''SALES VAR RSD'' AS GROWTH, f1.SALES_VAR_RSD AS [' + @Metric1 + '], f1.N_SALES_VAR_RSD AS [' + @Score1 + '], f2.SALES_VAR_RSD AS [' + @Metric2 + '], f2.N_SALES_VAR_RSD AS [' + @Score2 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
'
EXEC(@SQL)
--------------------------------------------------------------------------------------------------------------------------
SET @SQL ='
 
 INSERT INTO #values2
 SELECT (select max(Num)+1 from #values2) AS Num, ''PE NTM'' AS VALUE, f1.PE_NTM AS [' + @Metric3 + '], f1.N_PE_NTM AS [' + @Score3 + '], f2.PE_NTM AS [' + @Metric4 + '], f2.N_PE_NTM AS [' + @Score4 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''

 INSERT INTO #values2
 SELECT (select max(Num)+1 from #values2) AS Num, ''PE ON MED5Y'' AS VALUE, f1.PE_ON_MED5Y AS [' + @Metric3 + '], f1.N_PE_ON_MED5Y AS [' + @Score3 + '], f2.PE_ON_MED5Y AS [' + @Metric4 + '], f2.N_PE_ON_MED5Y AS [' + @Score4 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''

 INSERT INTO #values2
 SELECT (select max(Num)+1 from #values2) AS Num, ''PE PREMIUM ON HIST'' AS VALUE, f1.PE_PREMIUM_ON_HIST AS [' + @Metric3 + '], f1.N_PE_PREMIUM_ON_HIST AS [' + @Score3 + '], f2.PE_PREMIUM_ON_HIST AS [' + @Metric4 + '], f2.N_PE_PREMIUM_ON_HIST AS [' + @Score4 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''

 IF ''' + @FGA + '''=''401001''
 BEGIN
 INSERT INTO #values2
 SELECT (select max(Num)+1 from #values2) AS Num, ''P TBV NTM'' AS VALUE, f1.P_TBV_NTM AS [' + @Metric3 + '], f1.N_P_TBV_NTM AS [' + @Score3 + '], f2.P_TBV_NTM AS [' + @Metric4 + '], f2.N_P_TBV_NTM AS [' + @Score4 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END
 ELSE
 BEGIN
 INSERT INTO #values2
 SELECT (select max(Num)+1 from #values2) AS Num, ''PB NTM'' AS VALUE, f1.PB_NTM AS [' + @Metric3 + '], f1.N_PB_NTM AS [' + @Score3 + '], f2.PB_NTM AS [' + @Metric4 + '], f2.N_PB_NTM AS [' + @Score4 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END

 IF ''' + @FGA + '''=''401001'' 
 BEGIN
 INSERT INTO #values2
 SELECT (select max(Num)+1 from #values2) AS Num, ''P TBV ON MED5Y'' AS VALUE, f1.P_TBV_ON_MED5Y AS [' + @Metric3 + '], f1.N_P_TBV_ON_MED5Y AS [' + @Score3 + '], f2.P_TBV_ON_MED5Y AS [' + @Metric4 + '], f2.N_P_TBV_ON_MED5Y AS [' + @Score4 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END
 ELSE
 BEGIN
 INSERT INTO #values2
 SELECT (select max(Num)+1 from #values2) AS Num, ''PB ON MED5Y'' AS VALUE, f1.PB_ON_MED5Y AS [' + @Metric3 + '], f1.N_PB_ON_MED5Y AS [' + @Score3 + '], f2.PB_ON_MED5Y AS [' + @Metric4 + '], f2.N_PB_ON_MED5Y AS [' + @Score4 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END

 INSERT INTO #values2
 SELECT (select max(Num)+1 from #values2) AS Num, ''PB PREMIUM ON HIST'' AS VALUE, f1.PB_PREMIUM_ON_HIST AS [' + @Metric3 + '], f1.N_PB_PREMIUM_ON_HIST AS [' + @Score3 + '], f2.PB_PREMIUM_ON_HIST AS [' + @Metric4 + '], f2.N_PB_PREMIUM_ON_HIST AS [' + @Score4 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''

 INSERT INTO #values2
 SELECT (select max(Num)+1 from #values2) AS Num, ''DIV YLD NTM'' AS VALUE, f1.DIV_YLD_NTM AS [' + @Metric3 + '], f1.N_DIV_YLD_NTM AS [' + @Score3 + '], f2.DIV_YLD_NTM AS [' + @Metric4 + '], f2.N_DIV_YLD_NTM AS [' + @Score4 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
'
EXEC(@SQL)
--------------------------------------------------------------------------------------------------------------------------
SET @SQL ='
 IF ''' + @FGA + '''=''401001'' 
 BEGIN
 INSERT INTO #values3
 SELECT (select max(Num)+1 from #values3) AS Num, ''PBT RWA NTM'' AS PROFIT, f1.PBT_RWA_NTM AS [' + @Metric5 + '], f1.N_PBT_RWA_NTM AS [' + @Score5 + '], f2.PBT_RWA_NTM AS [' + @Metric6 + '], f2.N_PBT_RWA_NTM AS [' + @Score6 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END
 ELSE IF ''' + @FGA + '''=''403010'' 
 BEGIN
 INSERT INTO #values3
 SELECT (select max(Num)+1 from #values3) AS Num, ''PBT SALES NTM'' AS PROFIT, f1.PBT_SALES_NTM AS [' + @Metric5 + '], f1.N_PBT_SALES_NTM AS [' + @Score5 + '], f2.PBT_SALES_NTM AS [' + @Metric6 + '], f2.N_PBT_SALES_NTM AS [' + @Score6 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END
 ELSE
 BEGIN
 INSERT INTO #values3
 SELECT (select max(Num)+1 from #values3) AS Num, ''EBIT MARGIN NTM'' AS PROFIT, f1.EBIT_MARGIN_NTM AS [' + @Metric5 + '], f1.N_EBIT_MARGIN_NTM AS [' + @Score5 + '], f2.EBIT_MARGIN_NTM AS [' + @Metric6 + '], f2.N_EBIT_MARGIN_NTM AS [' + @Score6 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END
 
 INSERT INTO #values3
 SELECT (select max(Num)+1 from #values3) AS Num, ''FCF TREND'' AS PROFIT, f1.FCF_TREND AS [' + @Metric5 + '], f1.N_FCF_TREND AS [' + @Score5 + '], f2.FCF_TREND AS [' + @Metric6 + '], f2.N_FCF_TREND AS [' + @Score6 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''

 INSERT INTO #values3
 SELECT (select max(Num)+1 from #values3) AS Num, ''NET DEBT EBITDA NTM'' AS PROFIT, f1.NET_DEBT_EBITDA_NTM AS [' + @Metric5 + '], f1.N_NET_DEBT_EBITDA_NTM AS [' + @Score5 + '], f2.NET_DEBT_EBITDA_NTM AS [' + @Metric6 + '], f2.N_NET_DEBT_EBITDA_NTM AS [' + @Score6 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 
 IF ''' + @FGA + '''=''401001'' 
 BEGIN
 INSERT INTO #values3
 SELECT (select max(Num)+1 from #values3) AS Num, ''ROTE NTM'' AS PROFIT, f1.ROTE_NTM AS [' + @Metric5 + '], f1.N_ROTE_NTM AS [' + @Score5 + '], f2.ROTE_NTM AS [' + @Metric6 + '], f2.N_ROTE_NTM AS [' + @Score6 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END
 ELSE
 BEGIN
 INSERT INTO #values3
 SELECT (select max(Num)+1 from #values3) AS Num, ''ROE NTM'' AS PROFIT, f1.ROE_NTM AS [' + @Metric5 + '], f1.N_ROE_NTM AS [' + @Score5 + '], f2.ROE_NTM AS [' + @Metric6 + '], f2.N_ROE_NTM AS [' + @Score6 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END

 IF ''' + @FGA + '''=''401001'' 
 BEGIN
 INSERT INTO #values3
 SELECT (select max(Num)+1 from #values3) AS Num, ''COST INCOME NTM'' AS PROFIT, f1.COST_INCOME_NTM AS [' + @Metric5 + '], f1.N_COST_INCOME_NTM AS [' + @Score5 + '], f2.COST_INCOME_NTM AS [' + @Metric6 + '], f2.N_COST_INCOME_NTM AS [' + @Score6 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
 END
 
 INSERT INTO #values3
 SELECT (select max(Num)+1 from #values3) AS Num, ''ISR'' AS PROFIT, f1.ESG AS [' + @Metric5 + '], f1.GARPN_ISR_S AS [' + @Score5 + '], f2.ESG AS [' + @Metric6 + '], f2.GARPN_ISR_S AS [' + @Score6 + ']
 FROM DATA_FACTSET f1 INNER JOIN DATA_FACTSET f2 ON f2.TICKER=''' + @TICKER2 + ''' AND f2.DATE=''' + @Date + ''' WHERE f1.TICKER=''' + @TICKER1 + ''' AND f1.DATE=''' + @Date + '''
'
EXEC(@SQL)
--------------------------------------------------------------------------------------------------------------------------
SET @SQL ='
 WHILE ((select max(Num) from #values) < 7)
 BEGIN
 INSERT INTO #values(Num) SELECT max(Num)+1 FROM #values
 END
 WHILE ((select max(Num) from #values2) < 7)
 BEGIN
 INSERT INTO #values2(Num) SELECT max(Num)+1 FROM #values2
 END
 WHILE ((select max(Num) from #values3) < 7)
 BEGIN
 INSERT INTO #values3(Num) SELECT max(Num)+1 FROM #values3
 END
 
 DELETE FROM #values WHERE Num=0
 DELETE FROM #values2 WHERE Num=0
 DELETE FROM #values3 WHERE Num=0
 
 SELECT v1.GROWTH, v1.[' + @Metric1 + '], v1.[' + @Score1 + '], v1.[' + @Metric2 + '], v1.[' + @Score2 + '],
	   v2.VALUE, v2.[' + @Metric3 + '], v2.[' + @Score3 + '], v2.[' + @Metric4 + '], v2.[' + @Score4 + '],
	   v3.PROFIT, v3.[' + @Metric5 + '], v3.[' + @Score5 + '], v3.[' + @Metric6 + '], v3.[' + @Score6 + ']
 FROM #values v1
 INNER JOIN #values2 v2 ON v2.Num=v1.Num
 INNER JOIN #values3 v3 ON v3.Num=v1.Num
 
 DROP TABLE #values
 DROP TABLE #values2
 DROP TABLE #values3
 '

EXEC(@SQL)