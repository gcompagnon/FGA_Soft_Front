/*
Récupère la derniere recommandation pour un sous_secteur donné
*/
       
--DROP PROCEDURE GetLastSousSecteurRecommandation

CREATE PROCEDURE GetLastSousSecteurRecommandation

       @libelle VARCHAR(60)
AS 

/*
DECLARE @libelle As Varchar(60)
--SET @libelle = 'EMPRUNT SENIOR BANQUE'
SET @libelle = 'AGENCIES'
*/

SELECT Max(date) AS date, id_sous_secteur Into #tmp FROM TX_RECOMMANDATION_SOUS_SECTEUR GROUP BY id_sous_secteur

SELECT 
	s.libelle AS 'Secteur', 
	ss.libelle As 'Sous Secteur', 
	tmp.date AS 'Last date', 
	tr.libelle AS 'Recommandation', 
	trss.id_utilisateur AS 'Utilisateur',
	CASE WHEN trss.commentaire IS NULL THEN '' ELSE trss.commentaire END AS 'Commentaire',
	tr2.libelle
INTO #res
FROM TX_RECOMMANDATION_SOUS_SECTEUR trss, #tmp tmp, SOUS_SECTEUR ss, SECTEUR s, TX_RECOMMANDATION tr, TX_RECOMMANDATION tr2 
WHERE
	tmp.date = trss.date AND tmp.id_sous_secteur = trss.id_sous_secteur AND
	ss.id = trss.id_sous_secteur AND 
	tr.id = trss.id_recommandation AND tr2.id = trss.id_recommandation_isr AND 
	ss.id_secteur = s.id AND ss.libelle = @libelle

SELECT * FROM #res

DROP TABLE #tmp
DROP TABLE #res
        