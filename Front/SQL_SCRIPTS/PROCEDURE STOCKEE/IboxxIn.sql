/*
Récupere les nouveaux titres de l'iboxx qui sont rentrés
*/
       
GO

CREATE PROCEDURE IboxxIn

       @debut DATETIME,
	   @fin   DATETIME
AS 


--DECLARE @debut DATETIME
--SET @debut='31/08/2011'
--DECLARE @fin DATETIME
--SET @fin='02/09/2011'

SELECT isin into #in FROM TX_IBOXX where date = @fin
EXCEPT
SELECT isin FROM TX_IBOXX where date = @debut
SELECT ib.isin, ib.issuerName FROM #in i, TX_IBOXX ib
where ib.date = @fin and i.isin = ib.isin 
Drop TABLE #in
        