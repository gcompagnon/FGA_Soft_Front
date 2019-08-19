/*
Récupère la DERNIERE recommendation de tous les sous secteurs
*/
       
GO

CREATE PROCEDURE GetSignatureLibelleOmega

       @text Varchar(150)
AS 

SELECT DISTINCT libelle from TMP_SIGNATURE_OMEGA  WHERE libelle LIKE @text
EXCEPT 
SELECT libelle from TX_SIGNATURE



