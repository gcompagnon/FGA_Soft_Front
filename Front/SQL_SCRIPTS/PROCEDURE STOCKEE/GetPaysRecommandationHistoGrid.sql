/*
Recupère l'historique de recommandation pour un pays donnée
*/
       
GO

DROP PROCEDURE GetPaysRecommandationHistoGrid 

CREATE PROCEDURE GetPaysRecommandationHistoGrid

       @libelle VARCHAR(30)
AS 

SELECT DISTINCT 
	p.libelle As 'Pays', 
	trp.date AS 'Last date', 
	tr.libelle AS 'Recommandation',
	tr2.libelle AS 'Isr',
	trp.id_utilisateur AS 'Utilisateur', 
	trp.commentaire AS 'Commentaire'
INTO #tmp FROM TX_RECOMMANDATION_PAYS trp , Pays p , tx_recommandation tr, tx_recommandation tr2 
Where  
	p.libelle = @libelle AND 
	p.id = trp.id_pays AND 
	tr.id = trp.id_recommandation AND
	tr2.id = trp.id_recommandation_isr

SELECT * FROM #tmp ORDER BY 'Last Date' DESC 

DROP TABLE #tmp