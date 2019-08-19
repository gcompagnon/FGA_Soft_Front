/*
Récupère la DERNIERE recommendation de tous les sous secteurs
*/
       
GO

--DROP PROCEDURE GetSousSecteurRecommandationGrid

CREATE PROCEDURE GetSousSecteurRecommandationGrid

AS 

SELECT Max(date) AS date, id_sous_secteur Into #tmp FROM TX_RECOMMANDATION_SOUS_SECTEUR GROUP BY id_sous_secteur

SELECT 
	s.libelle AS 'Secteur', 
	ss.libelle As 'Sous Secteur', 
	tmp.date AS 'Last date', 
	tr.libelle AS 'Recommandation',
	tr2.libelle As 'Isr',
	trss.commentaire AS 'Commentaire'
INTO #res
FROM TX_RECOMMANDATION_SOUS_SECTEUR trss, #tmp tmp, SOUS_SECTEUR ss, SECTEUR s, TX_RECOMMANDATION tr, TX_RECOMMANDATION tr2 
WHERE
	tmp.date = trss.date AND 
	tmp.id_sous_secteur = trss.id_sous_secteur AND 
	ss.id = trss.id_sous_secteur AND 
	tr.id = trss.id_recommandation AND 
	tr2.id = trss.id_recommandation_isr AND 
	ss.id_secteur = s.id

SELECT * FROM #res ORDER BY 'SECTEUR', 'Sous Secteur'

DROP TABLE #tmp
DROP TABLE #res
        