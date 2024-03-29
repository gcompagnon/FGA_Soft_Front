/****** Object:  StoredProcedure [dbo].[ACT_DailyImport]    Script Date: 12/27/2013 13:58:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--USE [FGA_JMOINS1]
--GO
--/****** Object:  StoredProcedure [dbo].[ACT_DailyImport]    Script Date: 11/22/2013 16:26:29 ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
---- =============================================
---- Author:		<Author,,Name>
---- Create date: <Create Date,,>
---- Description:	<Description,,>
---- =============================================
ALTER PROCEDURE [dbo].[ACT_DailyImport] 

	--@Date As DATETIME

AS

DECLARE @Date AS Datetime 
SET @Date=(SELECT MAX(DATE)FROM DATA_FACTSET)
--SET @Date='21/11/2013'

-- =============================================
--				checkForMissingSectors
--select 'checkForMissingSectors'
-- =============================================
-----------------------------------------------------------------------------------------------------------------------------------
            -- Ajout des secteurs FGA
INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name)
SELECT FGA_SECTOR, NULL, 0, SECTOR_LABEL, 'FGA_ALL'
FROM DATA_FACTSET
WHERE GICS_SECTOR IS NULL AND MXEU is not null AND MXUSLC is not null AND FGA_SECTOR NOT IN (SELECT code FROM ref_security.SECTOR WHERE class_name='FGA_ALL')
AND ISIN is NULL and date = @Date

INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name)
SELECT FGA_SECTOR, NULL, 0, SECTOR_LABEL, 'FGA_EU'
FROM DATA_FACTSET
WHERE GICS_SECTOR IS NULL AND MXEU is not null AND MXUSLC is null AND FGA_SECTOR NOT IN (SELECT code FROM ref_security.SECTOR WHERE class_name='FGA_EU')
AND ISIN is NULL and date = @Date

INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name)
SELECT FGA_SECTOR, NULL, 0, SECTOR_LABEL, 'FGA_US'
FROM DATA_FACTSET
WHERE GICS_SECTOR IS NULL AND MXEU is null AND MXUSLC is not null AND FGA_SECTOR NOT IN (SELECT code FROM ref_security.SECTOR WHERE class_name='FGA_US')
AND ISIN is NULL and date = @Date

-----------------------------------------------------------------------------------------------------------------------------------
            -- Ajout des industrygrp en priorité pour référence dans les industries
INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name)
SELECT GICS_SECTOR, NULL, 0, SECTOR_LABEL, 'GICS'
FROM DATA_FACTSET
WHERE GICS_SECTOR IS NOT NULL AND GICS_SUBINDUSTRY IS NULL AND GICS_SECTOR NOT IN (SELECT code FROM ref_security.SECTOR)
AND ISIN is NULL AND MXFR is not null and date = @Date

-----------------------------------------------------------------------------------------------------------------------------------
            -- Ajout des subindustry
INSERT into ref_security.SECTOR(code, id_parent, [level], label, class_name)
SELECT fac.GICS_SUBINDUSTRY, sec.id, COALESCE(sec.level + 1, 0), fac.SECTOR_LABEL, 'GICS'
FROM DATA_FACTSET fac
LEFT OUTER JOIN ref_security.SECTOR sec ON sec.code = fac.GICS_SECTOR AND sec.class_name = 'GICS'
WHERE GICS_SUBINDUSTRY IS NOT NULL AND GICS_SUBINDUSTRY NOT IN (SELECT code FROM ref_security.SECTOR)
AND fac.ISIN is NULL and fac.date = @Date

-----------------------------------------------------------------------------------------------------------------------------------
            -- Liaison entre secteurs FGA GICS
INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)
SELECT sec1.ID, sec2.ID, 'GICS'
FROM DATA_FACTSET fac
INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.FGA_SECTOR
INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.GICS_SUBINDUSTRY
WHERE sec1.class_name='FGA_ALL' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'GICS')
and fac.ISIN is null and date = @Date

INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)
SELECT sec1.ID, sec2.ID, 'FGA_ALL'
FROM DATA_FACTSET fac
INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.GICS_SUBINDUSTRY
INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.FGA_SECTOR
WHERE sec2.class_name='FGA_ALL' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'FGA_ALL')
and fac.ISIN is null and date = @Date

INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)
SELECT sec1.ID, sec2.ID, 'GICS'
FROM DATA_FACTSET fac
INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.FGA_SECTOR
INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.GICS_SUBINDUSTRY
WHERE sec1.class_name='FGA_EU' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'GICS')
and fac.ISIN is null and date = @Date

INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)
SELECT sec1.ID, sec2.ID, 'FGA_EU'
FROM DATA_FACTSET fac
INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.GICS_SUBINDUSTRY
INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.FGA_SECTOR
WHERE sec2.class_name='FGA_EU' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'FGA_EU')
and fac.ISIN is null and date = @Date

INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)
SELECT sec1.ID, sec2.ID, 'GICS'
FROM DATA_FACTSET fac
INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.FGA_SECTOR
INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.GICS_SUBINDUSTRY
WHERE sec1.class_name='FGA_US' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'GICS')
and fac.ISIN is null and date = @Date

INSERT INTO ref_security.SECTOR_TRANSCO(ID_SECTOR1, ID_SECTOR2, CLASS_NAME)
SELECT sec1.ID, sec2.ID, 'FGA_US'
FROM DATA_FACTSET fac
INNER JOIN ref_security.SECTOR sec1 ON sec1.CODE = fac.GICS_SUBINDUSTRY
INNER JOIN ref_security.SECTOR sec2 ON sec2.CODE = fac.FGA_SECTOR
WHERE sec2.class_name='FGA_US' AND fac.GICS_SUBINDUSTRY IS NOT NULL AND sec1.ID not in (SELECT ID_SECTOR1 FROM ref_security.SECTOR_TRANSCO WHERE CLASS_NAME = 'FGA_US')
and fac.ISIN is null and date = @Date

-- =============================================
--				UPDATE Ticker
--select 'Ticker'
-- =============================================
EXECUTE ACT_TickerConvert @Date

-- =============================================
--				checkForMissingEquities
--select 'checkForMissingEquities'
-- =============================================
			---------------------------------------
            -- Ajout des identifications d'assets
            ---------------------------------------
INSERT into ref_common.IDENTIFICATION(ISIN, Name, Bloomberg, Country)
SELECT fac.ISIN, fac.COMPANY_NAME, fac.TICKER, 'XX'
FROM DATA_FACTSET fac
WHERE fac.ISIN IS NOT NULL AND fac.ISIN NOT IN (SELECT ISIN FROM ref_common.IDENTIFICATION WHERE Bloomberg=fac.TICKER)
and fac.date = @Date

            ---------------------------------------
            -- Ajout des equities manquantes
            ---------------------------------------
INSERT into ref_security.ASSET(ISIN, FinancialInstrumentName, MaturityDate, IdentificationId, Discriminator)
SELECT fac.ISIN, fac.COMPANY_NAME, fac.date, iden.Id, 'Equity'
FROM DATA_FACTSET fac
LEFT OUTER JOIN ref_common.IDENTIFICATION iden ON iden.ISIN = fac.ISIN
WHERE fac.ISIN IS NOT NULL AND fac.ISIN NOT IN (SELECT ISIN FROM ref_security.ASSET where MaturityDate = @Date)
and fac.date = @Date

            ---------------------------------------
            -- Lien des equities avec les secteurs 
            ---------------------------------------
INSERT INTO ref_security.ASSET_TO_SECTOR(ID_ASSET, ID_SECTOR, CLASS_NAME, CODE_SECTOR, SOURCE, DATE)
SELECT ass.ID, sec.ID, sec.CLASS_NAME, sec.CODE, 'Factset', fac.date
FROM DATA_FACTSET fac
INNER JOIN ref_security.ASSET ass ON ass.ISIN = fac.ISIN and ass.MaturityDate = fac.DATE
INNER JOIN ref_security.SECTOR sec ON sec.CODE = fac.SECTOR
WHERE fac.ISIN IS NOT NULL and ass.ID not in (SELECT id_asset FROM ref_security.ASSET_TO_SECTOR where date = @Date and id_sector=sec.id)
and fac.SECTOR IS NOT NULL and date = @Date

-- =============================================
--				UPDATE PTF
--select 'PTF'
-- =============================================
UPDATE fac SET fac.[6100001]=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='6100001' AND fac.DATE=@Date
UPDATE fac SET fac.[6100002]=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='6100002' AND fac.DATE=@Date
UPDATE fac SET fac.[6100004]=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='6100004' AND fac.DATE=@Date
UPDATE fac SET fac.[6100024]=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='6100024' AND fac.DATE=@Date
UPDATE fac SET fac.[6100026]=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='6100026' AND fac.DATE=@Date
UPDATE fac SET fac.[6100030]=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='6100030' AND fac.DATE=@Date
UPDATE fac SET fac.[6100033]=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='6100033' AND fac.DATE=@Date
UPDATE fac SET fac.[6100062]=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='6100062' AND fac.DATE=@Date
UPDATE fac SET fac.[6100063]=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='6100063' AND fac.DATE=@Date
UPDATE fac SET fac.AVEURO=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='AVEURO' AND fac.DATE=@Date
UPDATE fac SET fac.AVEUROPE=ptf.Poids
FROM DATA_FACTSET fac
INNER JOIN ACT_PTF ptf ON ptf._codeprodui=fac.ISIN
WHERE fac.ISIN is not null AND ptf._compte='AVEUROPE' AND fac.DATE=@Date

-- =============================================
--				CALCUL Agregats
--select 'Agregats'
-- =============================================
EXECUTE ACT_Agregats_Secteur @Date

-- =============================================
--				CALCUL Scores
--select 'Scores'
-- =============================================
EXECUTE ACT_BlendScore_Valeur @Date, 90, 'FGA_EU'
EXECUTE ACT_BlendScore_Valeur @Date, 90, 'FGA_US'


/* TEST IMPORT EXCEL IN BASE
go
sp_configure 'show advanced options',1  
reconfigure with override  
go  
sp_configure 'Ad Hoc Distributed Queries',1  
reconfigure with override  
go

SELECT *
FROM OPENROWSET('Microsoft.Jet.OLEDB.4.0',
                'Excel 8.0;Database=C:\DEV\FGA_Soft_Front\Front\INPUT\ACTION\FACTSET\DATA\2013-11-21_BASE NEW.xlsx;IMEX=1',
                'SELECT * FROM [Feuil1$]')
                
SELECT *
FROM OPENROWSET 
('Microsoft.Jet.OLEDB.4.0', 'Excel 8.0;Database=C:\DEV\FGA_Soft_Front\Front\INPUT\ACTION\FACTSET\DATA\2013-11-21_BASE NEW.xlsx;HDR=YES', 'select * from [Feuil1$]')

SELECT *
FROM OPENROWSET 
('Microsoft.Jet.OLEDB.4.0', 'Excel 8.0;Database=C:\DEV\FGA_Soft_Front\Front\INPUT\ACTION\FACTSET\DATA\2013-07-01_Factset.xls;HDR=YES', 'select * from [Feuil1$]')

SELECT * FROM OPENROWSET('Microsoft.Jet.OLEDB.4.0',
'Excel 8.0;Database=C:\DEV\FGA_Soft_Front\Front\INPUT\ACTION\FACTSET\DATA\2013-07-01_Factset2.xls', [2013-07-01_Factset$])

SELECT * FROM OPENROWSET('Microsoft.Jet.OLEDB.4.0',
'Excel 8.0;Database=C:\DEV\FGA_Soft_Front\Front\INPUT\ACTION\FACTSET\DATA\2013-07-01_Factset2.xls', 'SELECT * FROM [2013-07-01_Factset$]')

EXEC sp_addlinkedserver EXCEL,
     'Jet 4.0',
     'Microsoft.Jet.OLEDB.4.0',
     'C:\DEV\FGA_Soft_Front\Front\INPUT\ACTION\FACTSET\DATA\2013-11-21_BASE NEW.xlsx', 
      NULL,
     'Excel 5.0;'

GO
SELECT *
FROM EXCEL...SalesData
GO*/