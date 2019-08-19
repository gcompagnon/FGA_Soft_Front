/*
Récupère l'historique de recommandation pour un sous secteur donné
*/
       
GO

--DROP PROCEDURE GetSousSecteurRecommandationHistoGrid

CREATE PROCEDURE GetSousSecteurRecommandationHistoGrid

       @libelle VARCHAR(60)
AS 

/*
DECLARE @libelle AS VARCHAR(60)
SET @libelle = 'AGENCIES'
*/

SELECT DISTINCT 
	ss.libelle As 'Sous Secteur', 
	trss.date AS 'Last date', 
	tr.libelle AS 'Recommandation',
	tr2.libelle AS 'Isr',
	trss.commentaire AS 'Commentaire'
INTO #tmp
FROM TX_RECOMMANDATION_SOUS_SECTEUR trss , SOUS_SECTEUR ss , tx_recommandation tr, tx_recommandation tr2
WHERE  
	ss.libelle = @libelle AND 
	ss.id = trss.id_sous_secteur AND 
	tr.id = trss.id_recommandation AND
	tr2.id = trss.id_recommandation_isr

SELECT * FROM #tmp ORDER BY 'Last Date' DESC 

DROP TABLE #tmp

        