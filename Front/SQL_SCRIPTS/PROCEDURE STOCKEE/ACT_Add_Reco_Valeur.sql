
/****** Object:  StoredProcedure [dbo].[ACT_Add_Reco_Valeur]    Script Date: 12/03/2013 18:55:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Aurélien Brandicourt
-- Create date: 04/05/2012
-- Description:	Ajout d'une recommandation de valeur
-- =============================================
ALTER PROCEDURE [dbo].[ACT_Add_Reco_Valeur] 
	-- Add the parameters for the stored procedure here
	@isin AS VARCHAR(12),
	--@quintil_quant AS integer = -1,
	--@quintil_qual AS integer  = -1,
	@reco_SXXE AS VARCHAR(4) = ' ',
	@reco_SXXA AS VARCHAR(4) = ' ',
	@reco_SXXP AS VARCHAR(4) = ' ',
	@reco_MXUSLC AS VARCHAR(4) = ' ',
	@login as VARCHAR(32) = ' '
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

/*
	DECLARE @isin AS VARCHAR(12),
	@reco_SXXP AS VARCHAR(4),
	@reco_SXXE AS VARCHAR(4),
	@reco_SXXA AS VARCHAR(4)
	
	Set @isin='FR0000120628'
	Set @reco_SXXE=''
	Set @reco_SXXA='@@'
	Set @reco_SXXP=''
*/
	-------------	
	DECLARE @idcomment AS int
	DECLARE @idchange AS int
	DECLARE @date as datetime

	BEGIN TRANSACTION
	SET @date=CAST(Floor(CAST(GETDATE() As float)) AS datetime)

	-- Récupère l'identifiant du commentaire général
	SET @idcomment=(SELECT id_comment
					FROM ACT_RECO_VALEUR
					WHERE date=(SELECT MAX(date) FROM ACT_RECO_VALEUR WHERE ISIN=@isin)
						AND ISIN=@isin)
	BEGIN TRY	
		IF @date = CAST(Floor(CAST((select MAX(date) FROM ACT_RECO_VALEUR WHERE ISIN=@isin) As float)) AS datetime)
			-- Mise à jour de la dernière recommandation
			BEGIN
			SET @idchange = (SELECT id_comment_change FROM ACT_RECO_VALEUR
							WHERE ISIN=@isin AND date=@date)
			UPDATE ACT_RECO_VALEUR
			SET --quintil_quant=@quintil_quant,
				--quintil_qual=@quintil_qual,
				reco_SXXE=@reco_SXXE,
				reco_SXXA=@reco_SXXA,
				reco_SXXP=@reco_SXXP,
				reco_MXUSLC=@reco_MXUSLC
			WHERE ISIN=@isin AND date=@date
			END
		ELSE
			-- Nouvelle recommandation
			BEGIN
			INSERT INTO ACT_RECO_COMMENT (comment)
					VALUES ('');
			SET @idchange = SCOPE_IDENTITY()
			
			IF @idcomment IS NULL
				BEGIN
				-- Création de l'identifiant du commentaire général
				INSERT INTO ACT_RECO_COMMENT (comment)
					VALUES ('')
				END
			ELSE
				BEGIN
				-- Création de l'identifiant du commentaire général sur la base de l'ancien
				INSERT INTO ACT_RECO_COMMENT (comment)
					(SELECT comment FROM ACT_RECO_COMMENT WHERE id=@idcomment)
				END
			SET @idcomment=SCOPE_IDENTITY()
			
			INSERT INTO ACT_RECO_VALEUR (date, ISIN, reco_SXXP, reco_SXXE, reco_SXXA, reco_MXUSLC, id_comment, id_comment_change, loginModif)
				VALUES (@date, @isin, @reco_SXXP, @reco_SXXE, @reco_SXXA, @reco_MXUSLC, @idcomment, @idchange, @login)
			END
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION
	END CATCH
		
	IF @@TRANCOUNT > 0
		COMMIT TRANSACTION
		
	--select * FROM ACT_RECO_VALEUR
END
