
-- Replie la grille 
CREATE PROCEDURE DataGridSignature
AS
BEGIN

SELECT	libelle AS 'Signature', 
CASE WHEN id_recommandation = 'Pos' THEN '       X' ELSE ' ' END AS 'Positif', 
CASE WHEN id_recommandation = 'Ne' THEN  '       X' ELSE ' ' END AS 'Neutre', 
CASE WHEN id_recommandation = 'Ne-' THEN '       X' ELSE ' ' END AS 'Neutre-', 
CASE WHEN id_recommandation = 'Neg' THEN '       X' ELSE ' ' END AS 'Négatif', 
CASE WHEN id_recommandation = 'Rev' THEN '       X' ELSE ' ' END AS 'Review', 
CASE WHEN id_recommandation = 'Na' THEN '       X' ELSE ' ' END AS 'Na',
'         ' AS ' ', 
CASE WHEN id_recommandation = 'Pos' and note_isr >10 THEN '       X' ELSE ' ' END AS 'Positif2', 
CASE WHEN id_recommandation = 'Ne'  and note_isr >10 THEN  '       X' ELSE ' ' END AS 'Neutre2', 
CASE WHEN id_recommandation = 'Ne-' and note_isr >10 THEN '       X' ELSE ' ' END AS 'Neutre-2', 
CASE WHEN id_recommandation = 'Neg' or  (note_isr <=10 and id_recommandation<>'Na') THEN '       X' ELSE ' ' END AS 'Négatif2', 
CASE WHEN id_recommandation = 'Rev' and note_isr >10 THEN '       X' ELSE ' ' END AS 'Rev2',
CASE WHEN id_recommandation = 'Na' THEN '       X' ELSE ' ' END AS 'Na2',
note_isr As 'Note'
FROM TX_SIGNATURE
	
END

