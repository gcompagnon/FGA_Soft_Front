
/****** Object:  StoredProcedure [dbo].[ACT_Agregats_Secteur]    Script Date: 11/25/2013 11:16:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---- =============================================
---- Author:		<Author,,Name>
---- Create date: <Create Date,,>
---- Description:	<Description,,>
---- =============================================

CREATE PROCEDURE [dbo].[ACT_Agregats_Secteur_Moyenne]

	@Date As DATETIME
AS

--DECLARE @Date AS Datetime 
--SET @Date='21/11/2013'

-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ --
--      DECLARATION DES COLONNES POUR LE CALCUL DES AGREGATS SECTORIELS     --
-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ --
DECLARE @cname AS	 varchar(50) = ''
DECLARE @sec AS	 varchar(50)
DECLARE @sql AS	VARCHAR(max)

DECLARE Colsname SCROLL CURSOR FOR
(select 'EBIT_MARGIN_PPTM'
union select 'EBIT_MARGIN_PTM'
union select 'EBIT_MARGIN_LTM'
union select 'EBIT_MARGIN_NTM'
union select 'EBIT_MARGIN_STM'
union select 'FCF_YLD_PPTM'
union select 'FCF_YLD_PTM'
union select 'FCF_YLD_LTM'
union select 'FCF_YLD_NTM'
union select 'FCF_YLD_STM'
--union select 'CASH_CONVERSION_PPTM'
--union select 'CASH_CONVERSION_PTM'
--union select 'CASH_CONVERSION_LTM'
--union select 'CASH_CONVERSION_NTM'
--union select 'CASH_CONVERSION_STM'
union select 'CAPEX_SALES_LTM'
union select 'CAPEX_SALES_NTM'
union select 'CAPEX_SALES_STM'
union select 'GEARING_LTM'
union select 'GEARING_NTM'
union select 'GEARING_STM'
union select 'NET_DEBT_EBITDA_LTM'
union select 'NET_DEBT_EBITDA_NTM'
union select 'NET_DEBT_EBITDA_STM'
union select 'ROE_LTM'
union select 'ROE_NTM'
union select 'ROE_STM'
union select 'ROTE_LTM'
union select 'ROTE_NTM'
union select 'ROTE_STM'
union select 'PAYOUT_LTM'
union select 'PAYOUT_NTM'
union select 'PAYOUT_STM'
union select 'DIV_YLD_LTM'
union select 'DIV_YLD_NTM'
union select 'DIV_YLD_STM'
union select 'COST_INCOME_LTM'
union select 'COST_INCOME_NTM'
union select 'COST_INCOME_STM'
union select 'TIER_1_LTM'
union select 'TIER_1_NTM'
union select 'TIER_1_STM'
union select 'PBT_RWA_LTM'
union select 'PBT_RWA_NTM'
union select 'PBT_RWA_STM'
union select 'PBT_SALES_LTM'
union select 'PBT_SALES_NTM'
union select 'PBT_SALES_STM'
union select 'RORWA_LTM'
union select 'RORWA_NTM'
union select 'RORWA_STM'
union select 'COMBINED_RATIO_LTM'
union select 'COMBINED_RATIO_NTM'
union select 'COMBINED_RATIO_STM'
union select 'LOSS_RATIO_LTM'
union select 'LOSS_RATIO_NTM'
union select 'LOSS_RATIO_STM'
union select 'EXPENSE_RATIO_LTM'
union select 'EXPENSE_RATIO_NTM'
union select 'EXPENSE_RATIO_STM'
union select 'EBIT_MARGIN_NTM_MED5Y'
union select 'PE_ON_MED5Y'
union select 'PB_ON_MED5Y'
union select 'P_TBV_ON_MED5Y'
union select 'EV_SALES_ON_MED5Y'
union select 'EV_EBITDA_ON_MED5Y'
union select 'EV_EBIT_ON_MED5Y'
union select 'EBIT_MARGIN_ON_MED5Y'
union select 'IGROWTH_NTM'
union select 'PEG_NTM'
union select 'EV_EBITDA_TO_G_NTM'
union select 'EV_EBITA_TO_G_NTM'
union select 'EV_EBIT_TO_G_NTM'
union select 'SALES_CHG_NTM'
union select 'SALES_CHG_STM'
union select 'CAPEX_CHG_NTM'
union select 'CAPEX_CHG_STM'
union select 'EBIT_CHG_NTM'
union select 'EBIT_CHG_STM'
union select 'FCF_CHG_NTM'
union select 'FCF_CHG_STM'
union select 'EPS_CHG_NTM'
union select 'EPS_CHG_STM'
union select 'EBIT_MARGIN_DIFF_NTM'
union select 'EBIT_MARGIN_DIFF_STM'
union select 'FCF_YLD_DIFF_NTM'
union select 'FCF_YLD_DIFF_STM'
union select 'PBT_SALES_DIFF_NTM'
union select 'PBT_SALES_DIFF_STM'
union select 'PBT_RWA_DIFF_NTM'
union select 'PBT_RWA_DIFF_STM'
union select 'PE_PCTIL'
union select 'PB_PCTIL'
union select 'EV_SALES_PCTIL'
union select 'EV_EBITDA_PCTIL'
union select 'EV_EBIT_PCTIL'
union select 'EBIT_MARGIN_PCTIL'
union select 'FCF_YLD_PCTIL'
union select 'PERF_1M'
union select 'PERF_1M_EUR'
union select 'PERF_3M'
union select 'PERF_6M'
union select 'PERF_1YR'
union select 'PERF_1YR_EUR'
union select 'PERF_MTD'
union select 'PERF_MTD_EUR'
union select 'PERF_YTD'
union select 'PERF_YTD_EUR'
union select 'EPS_CHG_1M'
union select 'EPS_CHG_3M'
union select 'EPS_CHG_6M'
union select 'EPS_CHG_1YR'
union select 'EPS_CHG_YTD')

OPEN Colsname

/*################################
			FGA EUROPE
################################*/
select 'MXEU'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR 
FROM DATA_FACTSET 
WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null 
	and GICS_SECTOR is null and MXEU is not null and MXUSLC is null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR 
FROM DATA_FACTSET 
WHERE DATE=@Date and GICS_SECTOR is not null 
	and GICS_SUBINDUSTRY is null and MXEU is not null and MXUSLC is null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

	FETCH NEXT FROM Colsname INTO @cname

	WHILE ( @@FETCH_STATUS = 0)
	BEGIN

	SET @sql=
		  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
		+ N' into ##values FROM DATA_FACTSET fac1'
		+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEU is not null'
	EXEC (@sql)

	SET @sql=
		  N'UPDATE DATA_FACTSET'
		+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
		+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEU is not null and MXUSLC is null and FGA_SECTOR=' + @sec
	EXEC(@sql)

	DROP TABLE ##values

	IF @cname = 'TIER_1_STM'
	BEGIN
		FETCH FIRST FROM Colsname INTO @cname ;
		BREAK
	END

	FETCH NEXT FROM Colsname INTO @cname ;
	END

	SET @sql=
		  N'UPDATE fac SET'
		+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
		+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
		+ N' fac.MXEU=(CASE WHEN A.MXEU is not null THEN A.MXEU ELSE 0 END),'
		+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
		+ N' FROM DATA_FACTSET fac'
		+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
		+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
		+ N'					from DATA_FACTSET fac1'
		+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEU is not null) B'
		+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
		+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEU is not null and fac.MXUSLC is null and fac.FGA_SECTOR=' + @sec
	EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;
END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

	FETCH FIRST FROM Colsname INTO @cname

	WHILE ( @@FETCH_STATUS = 0)
	BEGIN

	SET @sql=
		  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
		+ N' into ##values FROM DATA_FACTSET fac1'
		+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEU is not null and fac2.MXUSLC is null'
	EXEC(@sql)

	SET @sql=
		  N'UPDATE DATA_FACTSET'
		+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
		+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEU is not null and MXUSLC is null and GICS_SECTOR=' + @sec
	EXEC(@sql)

	DROP TABLE ##values

	IF @cname = 'TIER_1_STM'
	BEGIN
		FETCH FIRST FROM Colsname INTO @cname ;
		BREAK
	END

	FETCH NEXT FROM Colsname INTO @cname ;
	END

	SET @sql=
		  N'UPDATE fac SET'
		+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
		+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
		+ N' fac.MXEU=(CASE WHEN A.MXEU is not null THEN A.MXEU ELSE 0 END),'
		+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
		+ N' FROM DATA_FACTSET fac'
		+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
		+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
		+ N'					from DATA_FACTSET fac1'
		+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEU is not null) B'
		+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
		+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEU is not null and fac.MXUSLC is null and fac.GICS_SECTOR=' + @sec
	EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			FGA USA
################################*/
select 'MXUSLC'

DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and MXEU is null and MXUSLC is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and MXEU is null and MXUSLC is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

	IF @cname <> 'CAPEX_CHG_NTM'
	BEGIN
	FETCH NEXT FROM Colsname INTO @cname
	END

	WHILE ( @@FETCH_STATUS = 0)
	BEGIN

	SET @sql=
		  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
		+ N' into ##values FROM DATA_FACTSET fac1'
		+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXUSLC is not null'
	EXEC (@sql)

	SET @sql=
		  N'UPDATE DATA_FACTSET'
		+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
		+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEU is null and MXUSLC is not null and FGA_SECTOR=' + @sec
	EXEC(@sql)

	DROP TABLE ##values

	IF @cname = 'TIER_1_STM'
	BEGIN
	FETCH FIRST FROM Colsname INTO @cname ;
	BREAK
	END

	FETCH NEXT FROM Colsname INTO @cname ;
	END

	SET @sql=
		  N'UPDATE fac SET'
		+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
		+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
		+ N' fac.MXUSLC=(CASE WHEN A.MXUSLC is not null THEN a.MXUSLC ELSE 0 END),'
		+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
		+ N' FROM DATA_FACTSET fac'
		+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
		+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
		+ N'					from DATA_FACTSET fac1'
		+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXUSLC is not null) B'
		+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
		+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEU is null and fac.MXUSLC is not null and fac.FGA_SECTOR=' + @sec
	EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;
END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

	FETCH FIRST FROM Colsname INTO @cname

	WHILE ( @@FETCH_STATUS = 0)
	BEGIN

	SET @sql=
		  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
		+ N' into ##values FROM DATA_FACTSET fac1'
		+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEU is null and fac2.MXUSLC is not null'
	EXEC(@sql)

	SET @sql=
		  N'UPDATE DATA_FACTSET'
		+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
		+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEU is null and MXUSLC is not null and GICS_SECTOR=' + @sec
	EXEC(@sql)

	DROP TABLE ##values

	IF @cname = 'TIER_1_STM'
	BEGIN
	FETCH FIRST FROM Colsname INTO @cname ;
	BREAK
	END

	FETCH NEXT FROM Colsname INTO @cname ;
	END

	SET @sql=
		  N'UPDATE fac SET'
		+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
		+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
		+ N' fac.MXUSLC=(CASE WHEN A.MXUSLC is not null THEN a.MXUSLC ELSE 0 END),'
		+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
		+ N' FROM DATA_FACTSET fac'
		+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
		+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
		+ N'					from DATA_FACTSET fac1'
		+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXUSLC is not null) B'
		+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
		+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEU is null and fac.MXUSLC is not null and fac.GICS_SECTOR=' + @sec
	EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			FGA ALL
################################*/
select 'ALL'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and MXEU is not null and MXUSLC is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and MXEU is not null and MXUSLC is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

	IF @cname <> 'CAPEX_CHG_NTM'
	BEGIN
	FETCH NEXT FROM Colsname INTO @cname
	END

	WHILE ( @@FETCH_STATUS = 0)
	BEGIN

	SET @sql=
		  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
		+ N' into ##values FROM DATA_FACTSET fac1'
		+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null'
	EXEC (@sql)

	SET @sql=
		  N'UPDATE DATA_FACTSET'
		+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
		+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEU is not null and MXUSLC is not null and FGA_SECTOR=' + @sec
	EXEC(@sql)

	DROP TABLE ##values

	IF @cname = 'TIER_1_STM'
	BEGIN
	FETCH FIRST FROM Colsname INTO @cname ;
	BREAK
	END

	FETCH NEXT FROM Colsname INTO @cname ;
	END

	SET @sql=
		  N'UPDATE fac SET'
		+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
		+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
		+ N' fac.MXEU=(CASE WHEN A.MXEU is not null THEN A.MXEU ELSE 0 END),'
		+ N' fac.MXUSLC=(CASE WHEN A.MXUSLC is not null THEN a.MXUSLC ELSE 0 END),'
		+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
		+ N' FROM DATA_FACTSET fac'
		+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
		+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
		+ N'					from DATA_FACTSET fac1'
		+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
		+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null) B'
		+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
		+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEU is not null and fac.MXUSLC is not null and fac.FGA_SECTOR=' + @sec
	EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;
END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' + @cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEU is not null and MXUSLC is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.MXEU=(CASE WHEN A.MXEU is not null THEN A.MXEU ELSE 0 END),'
+ N' fac.MXUSLC=(CASE WHEN A.MXUSLC is not null THEN a.MXUSLC ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEU is not null and fac.MXUSLC is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			MXEM
################################*/
select 'MXEM'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and MXEM is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and MXEM is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEM is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEM is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.MXEM=(CASE WHEN A.MXEM is not null THEN a.MXEM ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEM is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEM is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEM is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEM is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.MXEM=(CASE WHEN A.MXEM is not null THEN a.MXEM ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEM is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEM is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils
/*################################
			MXEUM
################################*/
select 'MXEUM'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and MXEUM is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and MXEUM is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEUM is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEUM is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.MXEUM=(CASE WHEN A.MXEUM is not null THEN a.MXEUM ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEUM is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEUM is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEUM is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXEUM is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.MXEUM=(CASE WHEN A.MXEUM is not null THEN a.MXEUM ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXEUM is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXEUM is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils
/*################################
			MXFR
################################*/
select 'MXFR'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and MXFR is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and MXFR is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXFR is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXFR is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.MXFR=(CASE WHEN A.MXFR is not null THEN a.MXFR ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXFR is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXFR is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXFR is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and MXFR is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.MXFR=(CASE WHEN A.MXFR is not null THEN a.MXFR ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.MXEM) AS ''MXEM'', SUM(B.MXEU) AS ''MXEU'', SUM(B.MXEUM) AS ''MXEUM'', SUM(B.MXFR) AS ''MXFR'', SUM(B.MXUSLC) AS ''MXUSLC'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.MXEM, fac2.MXEU, fac2.MXEUM, fac2.MXFR, fac2.MXUSLC, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.MXFR is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.MXFR is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils
/*################################
			6100001
################################*/
select '01'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and [6100001] is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and [6100001] is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100001] is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100001] is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100001]=(CASE WHEN A.[6100001] is not null THEN a.[6100001] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100001]) AS ''6100001'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100001], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100001] is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100001] is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100001] is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100001] is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100001]=(CASE WHEN A.[6100001] is not null THEN a.[6100001] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100001]) AS ''6100001'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100001], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100001] is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100001] is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			6100002
################################*/
select '02'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and [6100002] is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and [6100002] is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100002] is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100002] is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100002]=(CASE WHEN A.[6100002] is not null THEN a.[6100002] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100002]) AS ''6100002'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100002], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100002] is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100002] is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN


SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100002] is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100002] is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100002]=(CASE WHEN A.[6100002] is not null THEN a.[6100002] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100002]) AS ''6100002'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100002], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100002] is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100002] is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			6100004
################################*/
select '04'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and [6100004] is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and [6100004] is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100004] is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100004] is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100004]=(CASE WHEN A.[6100004] is not null THEN a.[6100004] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100004]) AS ''6100004'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100004], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100004] is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100004] is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100004] is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100004] is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100004]=(CASE WHEN A.[6100004] is not null THEN a.[6100004] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100004]) AS ''6100004'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100004], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100004] is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100004] is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			6100024
################################*/
select '24'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and [6100024] is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and [6100024] is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100024] is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100024] is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100024]=(CASE WHEN A.[6100024] is not null THEN a.[6100024] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100024]) AS ''6100024'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100024], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100024] is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100024] is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100024] is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100024] is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100024]=(CASE WHEN A.[6100024] is not null THEN a.[6100024] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100024]) AS ''6100024'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100024], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100024] is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100024] is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			6100026
################################*/
select '26'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and [6100026] is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and [6100026] is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100026] is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100026] is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100026]=(CASE WHEN A.[6100026] is not null THEN a.[6100026] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100026]) AS ''6100026'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100026], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100026] is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100026] is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100026] is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100026] is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100026]=(CASE WHEN A.[6100026] is not null THEN a.[6100026] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100026]) AS ''6100026'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100026], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100026] is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100026] is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			6100030
################################*/
select '30'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and [6100030] is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and [6100030] is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100030] is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100030] is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100030]=(CASE WHEN A.[6100030] is not null THEN a.[6100030] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100030]) AS ''6100030'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100030], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100030] is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100030] is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100030] is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100030] is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100030]=(CASE WHEN A.[6100030] is not null THEN a.[6100030] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100030]) AS ''6100030'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100030], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100030] is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100030] is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			6100033
################################*/
select '33'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and [6100033] is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and [6100033] is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100033] is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100033] is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100033]=(CASE WHEN A.[6100033] is not null THEN a.[6100033] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100033]) AS ''6100033'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100033], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100033] is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100033] is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100033] is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100033] is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100033]=(CASE WHEN A.[6100033] is not null THEN a.[6100033] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100033]) AS ''6100033'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100033], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100033] is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100033] is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			6100062
################################*/
select '62'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and [6100062] is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and [6100062] is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100062] is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100062] is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100062]=(CASE WHEN A.[6100062] is not null THEN a.[6100062] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100062]) AS ''6100062'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100062], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100062] is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100062] is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100062] is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100062] is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100062]=(CASE WHEN A.[6100062] is not null THEN a.[6100062] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100062]) AS ''6100062'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100062], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100062] is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100062] is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			6100063
################################*/
select '63'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and [6100063] is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and [6100063] is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100063] is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100063] is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100063]=(CASE WHEN A.[6100063] is not null THEN a.[6100063] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100063]) AS ''6100063'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100063], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100063] is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100063] is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100063] is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and [6100063] is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.[6100063]=(CASE WHEN A.[6100063] is not null THEN a.[6100063] ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.[6100063]) AS ''6100063'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.[6100063], fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.[6100063] is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.[6100063] is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			AVEURO
################################*/
select 'AVEURO'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and AVEURO is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and AVEURO is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.AVEURO is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and AVEURO is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.AVEURO=(CASE WHEN A.AVEURO is not null THEN a.AVEURO ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.AVEURO) AS ''AVEURO'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.AVEURO, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.AVEURO is not null) B'
+ N'			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.AVEURO is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.AVEURO is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and AVEURO is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.AVEURO=(CASE WHEN A.AVEURO is not null THEN a.AVEURO ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.AVEURO) AS ''AVEURO'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.AVEURO, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.AVEURO is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.AVEURO is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

FETCH FIRST FROM Colsname INTO @cname ;
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils

/*################################
			AVEUROPE
################################*/
select 'AVEUROPE'
DECLARE SecteurFils SCROLL CURSOR FOR
SELECT FGA_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and FGA_SECTOR is not null and ISIN is null and GICS_SECTOR is null and AVEUROPE is not null

DECLARE SecteurPere SCROLL CURSOR FOR
SELECT GICS_SECTOR FROM DATA_FACTSET WHERE DATE=@Date and GICS_SECTOR is not null and GICS_SUBINDUSTRY is null and AVEUROPE is not null

OPEN SecteurFils
OPEN SecteurPere

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Fils (FGA)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurFils INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

IF @cname <> 'CAPEX_CHG_NTM'
BEGIN
FETCH NEXT FROM Colsname INTO @cname
END

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.AVEUROPE is not null'
EXEC (@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and AVEUROPE is not null and FGA_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.AVEUROPE=(CASE WHEN A.AVEUROPE is not null THEN a.AVEUROPE ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.fga_sector) AS ''fga_sector'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.AVEUROPE) AS ''AVEUROPE'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.fga_sector, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.AVEUROPE, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.fga_sector=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.AVEUROPE is not null) B'
+ N' 			) A ON A.fga_sector=fac.FGA_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.AVEUROPE is not null and fac.FGA_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurFils INTO @sec ;

END

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DEBUT Des Secteurs Pères
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
FETCH NEXT FROM SecteurPere INTO @sec

WHILE ( @@FETCH_STATUS = 0 )
BEGIN

FETCH FIRST FROM Colsname INTO @cname

WHILE ( @@FETCH_STATUS = 0)
BEGIN

SET @sql=
  N'SELECT AVG(fac2.' + @cname + ') as ' +@cname
+ N' into ##values FROM DATA_FACTSET fac1'
+ N'	 inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'	 where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.AVEUROPE is not null'
EXEC(@sql)

SET @sql=
  N'UPDATE DATA_FACTSET'
+ N' SET ' + @cname + '=(SELECT * FROM ##values)'
+ N' WHERE DATE=''' + cast(@date as varchar) + ''' and GICS_SUBINDUSTRY is null and AVEUROPE is not null and GICS_SECTOR=' + @sec
EXEC(@sql)

DROP TABLE ##values

IF @cname = 'TIER_1_STM'
BEGIN
FETCH FIRST FROM Colsname INTO @cname ;
BREAK
END

FETCH NEXT FROM Colsname INTO @cname ;
END

SET @sql=
  N'UPDATE fac SET'
+ N' fac.EV_NTM_EUR=(CASE WHEN A.EV_NTM_EUR is not null THEN A.EV_NTM_EUR ELSE 0 END),'
+ N' fac.MARKET_CAP_EUR=(CASE WHEN A.MARKET_CAP_EUR is not null THEN A.MARKET_CAP_EUR ELSE 0 END),'
+ N' fac.AVEUROPE=(CASE WHEN A.AVEUROPE is not null THEN a.AVEUROPE ELSE 0 END),'
+ N' fac.SALES_NTM_EUR=(CASE WHEN A.SALES_NTM_EUR is not null THEN A.SALES_NTM_EUR ELSE 0 END)'
+ N' FROM DATA_FACTSET fac'
+ N' INNER JOIN (SELECT MAX(B.GICS_SECTOR) AS ''GICS_SECTOR'', SUM(B.EV_NTM_EUR) AS ''EV_NTM_EUR'', SUM(B.MARKET_CAP_EUR) AS ''MARKET_CAP_EUR'', SUM(B.AVEUROPE) AS ''AVEUROPE'', SUM(B.SALES_NTM_EUR) AS ''SALES_NTM_EUR'''
+ N'			from (select fac1.GICS_SECTOR, fac2.EV_NTM_EUR, fac2.MARKET_CAP_EUR, fac2.AVEUROPE, fac2.SALES_NTM_EUR'
+ N'					from DATA_FACTSET fac1'
+ N'					inner join DATA_FACTSET fac2 on fac2.SECTOR=fac1.GICS_SUBINDUSTRY'
+ N'					where fac1.DATE=''' + cast(@date as varchar) + ''' and fac2.DATE=''' + cast(@date as varchar) + ''' and fac1.GICS_SECTOR=' + @sec + ' and fac1.GICS_SUBINDUSTRY is not null and fac2.AVEUROPE is not null) B'
+ N'			) A ON A.GICS_SECTOR=fac.GICS_SECTOR'
+ N' WHERE fac.DATE=''' + cast(@date as varchar) + ''' and fac.GICS_SUBINDUSTRY is null and fac.AVEUROPE is not null and fac.GICS_SECTOR=' + @sec
EXEC(@sql)

FETCH NEXT FROM SecteurPere INTO @sec ;
END

CLOSE Colsname
DEALLOCATE Colsname
CLOSE SecteurPere
DEALLOCATE SecteurPere
CLOSE SecteurFils
DEALLOCATE SecteurFils