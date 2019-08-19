/*
Récupère les derniers fichiers ajouter entre la dernier connexion et maintenant
*/

DROP PROCEDURE [GetDataGridFileRefresh]

CREATE PROCEDURE [dbo].[GetDataGridFileRefresh]
	
		@login VARCHAR(5)

AS 

SELECT sig.libelle AS 'Signature', fi.date AS 'Date', fi.Nom As 'Nom fichier', ef.libelle AS 'Emetteur fichier', fi.note AS 'Note', fi.id_utilisateur As 'Utilisateur'  
INTO #fichier 
FROM TX_FICHIER fi, UTILISATEUR util, TX_SIGNATURE sig, TX_EMETTEUR_FICHIER ef
WHERE 
util.id = @login
AND fi.date > util.derniere_deconnexion 
AND fi.code_signature = sig.code
AND ef.id = fi.id_emetteur_fichier

SELECT * FROM #fichier ORDER BY 'Date' DESC

DROP TABLE #fichier
