
/****** Object:  StoredProcedure [dbo].[ACT_Add_File_Link]    Script Date: 09/18/2012 14:36:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Aurélien Brandicourt
-- Create date: 11/09/2012
-- Description:	Ajoute un lien entre un fichier et une valeur/secteur, si le fichier n'est pas référencé, une nouvelle ligne est ajoutée dans ACT_FILE
-- =============================================
ALTER PROCEDURE [dbo].[ACT_Add_File_Link] 
	-- Add the parameters for the stored procedure here
	@id_sector_icb int = -1, 
	@id_sector_fga int = -1,
	@id_value int = -1,
	@url varchar(255), 
	@fname varchar(100),
	@desc text = '',
	@onglet int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @id_file As int
	DECLARE @date As Datetime
		
	--DECLARE @url as varchar(255)
	--DECLARE @fname as varchar(100)
	--DECLARE @desc as varchar(255)
	--Declare @onglet as int
	--DECLARE @id_sector_icb as int
	--DECLARE @id_sector_fga as int
	--DECLARE @id_value as int
	
	--SET @desc = ''
	--SET @id_sector_icb = -1
	--SET @id_sector_fga = -1
	
	--SET @fname = 'toto'
	--SET @url = 'tata'
	--SET @id_value = 811
	
	BEGIN TRY
		-- Récupération de l'id du fichier
		Set @id_file = (SELECT id
						FROM ACT_FILE
						WHERE fname=@fname AND url=@url);
		SET @date=CAST(Floor(CAST(GETDATE() As float)) AS datetime)

		IF @id_file is NULL
			BEGIN
			-- Création d'un id pour le fichier
			INSERT INTO ACT_FILE (fname, url, description)
				VALUES (@fname, @url, @desc);
			Set @id_file = SCOPE_IDENTITY();
			END
		
		-- Ajout le lien fichier-valeur/secteur
		INSERT INTO ACT_FILE_LINK (date, id_file, id_sector_icb, id_sector_fga, id_value, onglet)
			VALUES (@date, @id_file, @id_sector_icb, @id_sector_fga, @id_value, @onglet);

	END TRY
	BEGIN CATCH

	END CATCH
		
	--SELECT * FROM ACT_FILE_LINK
	--SELECT * FROM ACT_FILE
		
	
END
