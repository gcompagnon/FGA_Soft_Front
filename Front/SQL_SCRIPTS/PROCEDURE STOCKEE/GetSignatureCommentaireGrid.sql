/*
R�cup�re l'historique de commentaire pour une signature donn�e
*/
       
GO

CREATE PROCEDURE GetSignatureCommentaireGrid

       @code VARCHAR(30)
AS 



SELECT date AS 'Date',id_utilisateur AS 'Utilisateur' ,libelle As 'Commentaire' from TX_COMMENTAIRE where code_signature = @code ORDER BY date DEsc
        