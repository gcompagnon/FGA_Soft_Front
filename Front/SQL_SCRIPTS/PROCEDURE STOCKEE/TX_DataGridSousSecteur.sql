
-- Remplie la grille DataGridSousSecteur
--DROP PROCEDURE DataGridSousSecteur
CREATE PROCEDURE DataGridSousSecteur
AS
BEGIN
	
SELECT Max(date) AS date, id_sous_secteur Into #tmp FROM TX_RECOMMANDATION_SOUS_SECTEUR GROUP BY id_sous_secteur 
--SELECT * FROM TX_RECOMMANDATION_SOUS_SECTEUR
SELECT 
	s.libelle AS 'Secteur', 
	ss.libelle As 'Sous Secteur', 
	CASE WHEN tr.id = 'Pos' THEN '       X' ELSE ' ' END AS 'Positif', 
	CASE WHEN tr.id = 'Ne'  THEN '       X' ELSE ' ' END AS 'Neutre', 
	CASE WHEN tr.id = 'Ne-' THEN '       X' ELSE ' ' END AS 'Neutre-', 
	CASE WHEN tr.id = 'Neg' THEN '       X' ELSE ' ' END AS 'Négatif', 
	CASE WHEN tr.id = 'Rev' THEN '       X' ELSE ' ' END AS 'Review',
	CASE WHEN tr.id = 'Na'  THEN '       X' ELSE ' ' END AS 'Na',  
	' ' AS '     ',  
	CASE WHEN tr.id = 'Pos' and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Positif', 
	CASE WHEN tr.id = 'Ne'  and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Neutre', 
	CASE WHEN tr.id = 'Ne-' and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Neutre-', 
	CASE WHEN tr.id = 'Neg' or  tr2.id =  'Neg' THEN '       X' ELSE ' ' END AS 'Négatif', 
	CASE WHEN tr.id = 'Rev' and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Review',
	CASE WHEN tr.id = 'Na'  and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Na' 
FROM TX_RECOMMANDATION_SOUS_SECTEUR trss, #tmp tmp, SOUS_SECTEUR ss, SECTEUR s, TX_RECOMMANDATION tr, TX_RECOMMANDATION tr2  
WHERE 
tmp.date = trss.date And tmp.id_sous_secteur = trss.id_sous_secteur And ss.id = trss.id_sous_secteur And tr.id = trss.id_recommandation  And tr2.id = trss.id_recommandation_isr And ss.id_secteur = s.id 
ORDER BY s.libelle, ss.libelle 
DROP TABLE #tmp

END

