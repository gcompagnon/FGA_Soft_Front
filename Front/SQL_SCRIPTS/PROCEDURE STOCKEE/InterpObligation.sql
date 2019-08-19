USE [E0DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[TX_InterpObligation]    Script Date: 12/10/2014 17:40:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---- ==============================================================
---- Author:		<Najla Amdouni alias Framboise>
---- Create date: <09/30/2014>
---- Description:	<BLABLA description de la procedure stockée>
---- ==============================================================



ALTER PROCEDURE [dbo].[TX_InterpObligation]
	(@InputDate As DATETIME,
	@InputDateF As datetime,
	@InputPays As CHAR(2),
	@InputMaturity As INT)
AS

BEGIN

--Création d'une table temporaire 
	CREATE TABLE #TMP(
	[Date] Datetime not null,
	[Maturity1] int not null,
	[Rate] float) 

--Création d'une variable qui va contenir les dates du curseur
DECLARE @CurDate AS Datetime
--
DECLARE @cursordate AS DATETIME
DECLARE curdate1 SCROLL CURSOR FOR
(SELECT distinct date from ref_security.PRICE WHERE DATE BETWEEN @InputDate AND @InputDateF)

OPEN curdate1
FETCH NEXT FROM curdate1 into @cursorDate
WHILE (@@FETCH_STATUS = 0)
	BEGIN
	
	PRINT @cursorDate
	
	SELECT * into #linear_interpolation
	FROM (
		SELECT a.MaturityDate as DateM, pr.Date, pr.Debt_YTM_Rate as DBR, pr.ISINId from ref_security.PRICE as pr
		LEFT OUTER JOIN ref_security.ASSET as a on a.Id = pr.SecurityId
		LEFT OUTER JOIN ref_common.IDENTIFICATION as id on id.Id = a.IdentificationId
		LEFT OUTER JOIN ref_issuer.ROLE as r on r.AssetId = a.Id
		WHERE pr.Price_Source = 'BARCLAYS' and r.Country like @InputPays and Date = @cursordate) A order by Date, DateM
		


---


-- Création d'un curseur
DECLARE Cur SCROLL CURSOR FOR
--SELECT distinct Date
SELECT distinct DATEADD(year, @InputMaturity, Date) as Date
FROM #linear_interpolation 
WHERE Date >= @InputDate and Date <= @InputDateF 
ORDER BY Date; 


 OPEN Cur;
 FETCH NEXT FROM Cur INTO @CurDate
 WHILE (@@FETCH_STATUS = 0)
 BEGIN

-- RESULTAT : le Select va retourner dans l'appli un tableau avec les résultats demandés

INSERT INTO #TMP
SELECT @CurDate , @InputMaturity,
(	SELECT CASE 
	WHEN next.DateM IS NULL THEN prev.DBR
	WHEN prev.DateM IS NULL THEN next.DBR
	WHEN next.DateM = prev.DateM THEN prev.DBR 
	ELSE ( DATEDIFF(d, prev.DateM, @CurDate) * next.DBR + DATEDIFF(d, @CurDate, next.DateM) * prev.DBR ) / DATEDIFF(d, prev.DateM, next.DateM) END AS Rate 
		FROM (
			SELECT TOP 1 DateM, DBR 
			FROM #linear_interpolation 
			WHERE DateM <= @CurDate
			ORDER BY DateM DESC
		) AS prev 
		CROSS JOIN (
			SELECT TOP 1 DateM, DBR 
			FROM #linear_interpolation 
			WHERE DateM >= @CurDate
			ORDER BY DateM ASC
		) AS next
) As Rate 

	FETCH NEXT FROM Cur INTO @CurDate;
	END
	
	CLOSE Cur; 
	DEALLOCATE Cur;

--SELECT * FROM #linear_interpolation
--SELECT * FROM #LI_date
--SELECT * FROM #TMP

-- FREE MEMORY : Suppression des bases temporaires 
DROP Table #linear_interpolation
FETCH NEXT FROM curdate1 into @cursorDate

END

END


CLOSE curdate1
DEALLOCATE curdate1

SELECT distinct * FROM #TMP
DROP TABLE #TMP


--TEST


--DECLARE @InputPays  CHAR(2) set @InputPays = 'FR'
--DECLARE @InputMaturity int set @InputMaturity = 5
--DECLARE @InputDate  VARCHAR(150)  SET @InputDate = '03/09/2012' 
--DECLARE @InputDateF VARCHAR(150) SET @InputDateF = '25/12/2012'
--EXECUTE TX_InterpObligation @InputDate, @InputDateF, @InputPays, @InputMaturity
