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
-- Create date: 22/11/2012
-- Description:	Ajout d'un nouveau coefficient sur un critère indiciel
-- =============================================
ALTER PROCEDURE ACT_Add_Coef_Indice 
	-- Add the parameters for the stored procedure here
	@id_critere int,
	@id_indice int,
	@coef float = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
 --DECLARE @id_critere  FLOAT  
 --DECLARE @id_sector  FLOAT  
 --DECLARE @coef  VARCHAR(150)  
 
 --SET @id_critere = 4 
 --SET @id_sector = 10
 --SET @coef = '5' 
 
	IF @id_indice = 0 
		SET @id_indice = NULL

	DECLARE @date As Datetime
	SET @date=CAST(Floor(CAST(GETDATE() As float)) AS datetime)

	--DECLARE @id_critere AS int, @id_sector AS int, @coef AS int
	--SET @id_critere = 1
	--SET @id_sector = NULL
	--SET @coef = 4

	BEGIN TRANSACTION
	BEGIN TRY
		
		SET @date=CAST(Floor(CAST(GETDATE() As float)) AS datetime)

		if @id_indice IS NULL
			BEGIN
			IF NOT EXISTS(	SELECT * 
							FROM ACT_COEF_INDICE
							WHERE id_critere=@id_critere AND id_indice IS NULL AND date=@date
						 )
				BEGIN
				-- Création d'un coefficient
				INSERT INTO ACT_COEF_INDICE (id_critere, id_indice, date, coef)
					VALUES (@id_critere, @id_indice, @date, @coef);
				END
			ELSE
				BEGIN
				-- Mise à jour d'un coefficient
				UPDATE ACT_COEF_INDICE
				SET coef=@coef
				WHERE id_critere=@id_critere AND id_indice IS NULL AND date=@date
				END
			END
		ELSE
			BEGIN
			IF NOT EXISTS(	SELECT * 
						FROM ACT_COEF_INDICE
						WHERE id_critere=@id_critere AND id_indice=@id_indice AND date=@date
					 )
				BEGIN
				-- Création d'un coefficient
				INSERT INTO ACT_COEF_INDICE (id_critere, id_indice, date, coef)
					VALUES (@id_critere, @id_indice, @date, @coef);
				END
			ELSE
				BEGIN
				-- Mise à jour d'un coefficient
				UPDATE ACT_COEF_INDICE
				SET coef=@coef
				WHERE id_critere=@id_critere AND id_indice=@id_indice AND date=@date
				END
			END
	END TRY
	BEGIN CATCH
		WHILE @@TRANCOUNT > 0
			ROLLBACK TRANSACTION
	END CATCH
		
	IF @@TRANCOUNT > 0
		COMMIT TRANSACTION
		
	--SELECT * fROM ACT_COEF_CRITERE
	--SELECT * FROM ACT_COEF_INDICE
END
GO