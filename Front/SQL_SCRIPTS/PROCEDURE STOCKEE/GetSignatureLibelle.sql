/*
R�cup�re les libelles des signatures sauvegard�es dans TX_SIGNATURE
*/
       
GO

CREATE PROCEDURE GetSignatureLibelle

       @text Varchar(150)
AS 

SELECT DISTINCT libelle from TX_SIGNATURE WHERE libelle LIKE @text
        