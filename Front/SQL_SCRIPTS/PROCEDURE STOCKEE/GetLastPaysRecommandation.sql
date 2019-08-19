--DROP PROCEDURE [GetLastPaysRecommandation]

CREATE PROCEDURE [dbo].[GetLastPaysRecommandation]
	
		@libelle VARCHAR(30)

AS 

SELECT Max(date) AS date, id_pays Into #tmp FROM TX_RECOMMANDATION_PAYS GROUP BY id_pays

SELECT 
	p.libelle As 'Pays', 
	tmp.date AS 'Last date', 
	tr.libelle AS 'Recommandation', 
	trp.id_utilisateur AS 'Utilisateur',
	CASE WHEN trp.commentaire IS NULL THEN ' ' ELSE trp.commentaire  END AS 'Commentaire',
	tr2.libelle AS 'Isr' 
INTO #res
FROM TX_RECOMMANDATION_PAYS trp, #tmp tmp, Pays p,TX_RECOMMANDATION tr, TX_RECOMMANDATION tr2 
WHERE
	tmp.date = trp.date AND 
	tmp.id_pays = trp.id_pays AND 
	p.id = trp.id_pays AND 
	tr.id = trp.id_recommandation AND 
	tr2.id = trp.id_recommandation_isr AND 
	p.libelle = @libelle

SELECT * FROM #res

DROP TABLE #tmp
DROP TABLE #res