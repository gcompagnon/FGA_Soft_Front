/*
Récupère l'historique de LA DERNIERE recommandation pays par pays
*/
       
GO

DROP PROCEDURE GetPaysRecommandationGrid 

CREATE PROCEDURE GetPaysRecommandationGrid

AS 


SELECT Max(date) AS date, id_pays Into #tmp FROM TX_RECOMMANDATION_PAYS GROUP BY id_pays

SELECT 
	p.libelle As 'Pays', 
	tmp.date AS 'Last date', 
	tr.libelle AS 'Recommandation',
	tr2.libelle AS 'Isr', 
	trp.id_utilisateur AS 'Utilisateur',
	trp.commentaire AS 'Commentaire'
INTO #res FROM TX_RECOMMANDATION_PAYS trp, #tmp tmp, Pays p,TX_RECOMMANDATION tr, TX_RECOMMANDATION tr2
WHERE 
	tmp.date = trp.date And 
	tmp.id_pays = trp.id_pays And 
	p.id = trp.id_pays And 
	tr.id = trp.id_recommandation AND 
	tr2.id = trp.id_recommandation_isr 

SELECT * FROM #res 

DROP TABLE #tmp 
DROP TABLE #res
