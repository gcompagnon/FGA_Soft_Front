
-- =============================================
-- Author:		Coquard Benjamin
-- Create date: 24/11/2014
-- Description:	
-- =============================================

CREATE PROCEDURE [dbo].[ACT_RepartitionPays]
	@date AS DATETIME
AS
BEGIN

SELECT 
fac.COUNTRY,
convert(decimal(10, 2),SUM(fac.MXFR)) as MXFR,
convert(decimal(10, 2),SUM(fac.[6100002]) * 100)  as [6100002], 
convert(decimal(10, 2),SUM(fac.MXEM)) as MXEM,
convert(decimal(10, 2),SUM(fac.[6100030]) * 100)  as [6100030], 
convert(decimal(10, 2),SUM(fac.AVEURO) * 100)  as AVEURO, 
convert(decimal(10, 2),SUM(fac.[6100004]) * 100)  as [6100004], 
convert(decimal(10, 2),SUM(fac.[6100063]) * 100)  as [6100063], 
convert(decimal(10, 2),SUM(fac.AVEUROPE) * 100)  as AVEUROPE, 
convert(decimal(10, 2),SUM(fac.[6100001]) * 100)  as [6100001], 
convert(decimal(10, 2),SUM(fac.[6100033]) * 100)  as [6100033],
convert(decimal(10, 2),SUM(fac.MXEUM)) as MXEUM,
convert(decimal(10, 2),SUM(fac.[6100062]) * 100)  as [6100062],
convert(decimal(10, 2),SUM(fac.MXEU)) as MXEU,
convert(decimal(10, 2),SUM(fac.[6100026]) * 100)  as [6100026],
convert(decimal(10, 2),SUM(fac.MXUSLC)) as MXUSLC,
convert(decimal(10, 2),SUM(fac.[6100024]) * 100)  as [6100024],
convert(decimal(10, 2),SUM(fac.[6100066]) * 100)  as [6100066]
INTO #values
FROM DATA_FACTSET as fac 
WHERE fac.DATE = @date AND COUNTRY IS NOT NULL
GROUP BY fac.COUNTRY
ORDER BY fac.COUNTRY


SELECT *,
convert(decimal(10, 2),[6100002] - MXFR) as Ecart_6100002,
convert(decimal(10, 2),[6100030] - MXEM) as Ecart_6100030,
convert(decimal(10, 2),AVEURO - MXEM) as Ecart_AVEURO,
convert(decimal(10, 2),[6100004] - MXEM) as Ecart_6100004,
convert(decimal(10, 2),[6100063] - MXEM) as Ecart_6100063,
convert(decimal(10, 2),AVEUROPE - MXEM) as Ecart_AVEUROPE,
convert(decimal(10, 2),[6100001] - MXEM) as Ecart_6100001,
convert(decimal(10, 2),[6100033] - MXEM) as Ecart_6100033,
convert(decimal(10, 2),[6100062] - MXEUM) as Ecart_6100062,
convert(decimal(10, 2),[6100026] - MXEU) as Ecart_6100026,
convert(decimal(10, 2),[6100024] - MXUSLC) as Ecart_6100024,
convert(decimal(10, 2),[6100066] - MXUSLC) as Ecart_6100066
FROM #values

DROP TABLE #values

END