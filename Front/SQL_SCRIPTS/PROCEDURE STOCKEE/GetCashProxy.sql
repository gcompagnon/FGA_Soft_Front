/*
 Ajout les lignes de cash dans la table PTF_PROXY lors de l'ajout d'un nouveau fichier excel dans la table
*/
       
GO

CREATE PROCEDURE GetCashProxy

       @date DATETIME
AS 

SELECT DISTINCT code_proxy INTO #code FROM PTF_PARAM_PROXY 
WHERE Source <> 'OPCVM' AND date = @date 
ORDER BY code_proxy 

SELECT c.code_proxy, 1 - SUM(p.Poids_VB + p.Poids_CC) AS Somme 
INTO #tmp_som_ligne FROM PTF_PROXY p 
	RIGHT JOIN #code c ON c.code_proxy = p.code_Proxy 
WHERE date= @date 
GROUP BY c.code_proxy ORDER BY c.code_proxy 

SELECT * FROM #tmp_som_ligne 

DROP TABLE #code 
DROP TABLE #tmp_som_ligne
 
