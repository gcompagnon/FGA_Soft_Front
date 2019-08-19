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
-- Create date: 07/01/2013
-- Description:	Change les références d'une ancienne valeur par une nouvelle
-- =============================================
ALTER PROCEDURE ACT_Update_Valeur_Id 
	-- Add the parameters for the stored procedure here
	@newid int, 
	@oldid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--DECLARE @newid AS integer
	--DECLARE @oldid AS integer
	--SELECT @newid = 1, @oldid = -1

    -- Insert statements for procedure here
	UPDATE ACT_NOTE_RECORD
	SET id_valeur = @newid
	WHERE id_valeur = @oldid
	
	UPDATE ACT_VALEUR_RANK
	SET id_value = @newid
	WHERE id_value = @oldid
END
GO
