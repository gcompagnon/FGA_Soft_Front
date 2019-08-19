/*
Récupere les listes des titres sortant de l'iboxx entre 2 dates 
*/
       
GO

CREATE PROCEDURE IboxxOut

       @debut DATETIME,
	   @fin   DATETIME
AS 

--DECLARE @debut DATETIME
--SET @debut='31/08/2011'
--DECLARE @fin DATETIME
--SET @fin='02/09/2011'

SELECT isin into #out FROM TX_IBOXX where date = @debut
EXCEPT
SELECT isin FROM TX_IBOXX where date = @fin
SELECT ib.isin, ib.issuerName FROM #out o, TX_IBOXX ib
where ib.date = @debut and o.isin = ib.isin 
Drop TABLE #out
        