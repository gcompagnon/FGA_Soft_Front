
-- Remplie la grille des pays 

CREATE PROCEDURE DataGridPays
AS
BEGIN
	
SELECT Max(date) AS date, id_pays Into #tmp FROM TX_RECOMMANDATION_PAYS GROUP BY id_pays 
SELECT 
	p.libelle As 'Pays',  
	CASE WHEN tr.id = 'Pos' THEN '       X' ELSE ' ' END AS 'Positif', 
	CASE WHEN tr.id = 'Ne'  THEN '       X' ELSE ' ' END AS 'Neutre', 
	CASE WHEN tr.id = 'Ne-' THEN '       X' ELSE ' ' END AS 'Neutre-', 
	CASE WHEN tr.id = 'Neg' THEN '       X' ELSE ' ' END AS 'Négatif', 
	CASE WHEN tr.id = 'Rev' THEN '       X' ELSE ' ' END AS 'Review',
	CASE WHEN tr.id = 'Na'  THEN '       X' ELSE ' ' END AS 'Na   ',
	'    ' AS '     ',  
	CASE WHEN tr.id = 'Pos' and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Positif', 
	CASE WHEN tr.id = 'Ne'  and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Neutre', 
	CASE WHEN tr.id = 'Ne-' and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Neutre-', 
	CASE WHEN tr.id = 'Neg' or  tr2.id =  'Neg' THEN '       X' ELSE ' ' END AS 'Négatif', 
	CASE WHEN tr.id = 'Rev' and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Review',
	CASE WHEN tr.id = 'Na'  and tr2.id <> 'Neg' THEN '       X' ELSE ' ' END AS 'Na'
FROM TX_RECOMMANDATION_PAYS trp, #tmp tmp, Pays p,TX_RECOMMANDATION tr, TX_RECOMMANDATION tr2
WHERE tmp.date = trp.date And tmp.id_pays = trp.id_pays And p.id = trp.id_pays And tr.id = trp.id_recommandation  ANd tr2.id = trp.id_recommandation_isr 
ORDER BY p.libelle
DROP TABLE #tmp

END
