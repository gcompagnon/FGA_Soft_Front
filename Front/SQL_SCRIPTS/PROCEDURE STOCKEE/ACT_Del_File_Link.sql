-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Aurélien Brandicourt
-- Create date: 17/09/2012
-- Description:	Supprime un lien vers un fichier. Le fichier est supprimé
-- s'il est dans l'arborescence des fichiers copiés et qu'il n'y a plus
-- aucune référence à lui
-- =============================================
ALTER PROCEDURE ACT_Del_File_Link 
	-- Add the parameters for the stored procedure here
	@id_file int,
	@id_sector_icb int = -1, 
	@id_sector_fga int = -1,
	@id_value int = -1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	BEGIN TRANSACTION
	BEGIN TRY
	    -- Insert statements for procedure here
		-- Remove file link
		DELETE FROM ACT_FILE_LINK
			WHERE	id_file = @id_file
				AND id_value = @id_value
				AND id_sector_icb = @id_sector_icb
				AND id_sector_fga = @id_sector_fga
	    
		-- TODO delete file in Directory if no more references
		IF NOT EXISTS(SELECT * FROM ACT_FILE_LINK
						WHERE id_file = @id_file)
			BEGIN
			-- Reference not found
			DELETE FROM ACT_FILE
				WHERE	id = @id_file
			END
	END TRY
	BEGIN CATCH
		WHILE @@TRANCOUNT > 0
			ROLLBACK TRANSACTION
	END CATCH
			
	IF @@TRANCOUNT > 0
		COMMIT TRANSACTION
END
GO
