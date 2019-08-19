-- =============================================
-- Author:		Coquard Benjamin
-- Create date: 22/10/2012
-- Last Update: 10/12/2012
-- Description:	Ajoute un coefficient qualitatif à un secteur
-- =============================================
ALTER PROCEDURE [dbo].[ACT_Add_Coef_Secteur_Simulation] 
	-- Add the parameters for the stored procedure here
	@id_critere int,
	@id_fga int,
	@coef float,
	@date Datetime = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from 
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	 --DECLARE @id_critere  FLOAT  
	 --DECLARE @id_fga  FLOAT  
	 --DECLARE @coef  VARCHAR(150)  
	 --DECLARE @date As Datetime

	 --SET @id_critere = 77
	 --SET @id_fga = 0
	 --SET @coef = '5' 
	 --SET @date = '10/12/2012'
	
	IF @id_fga = 0 
		SET @id_fga = NULL

	IF @date IS NULL
		SELECT @date=MAX(date)
		FROM DATA_FACTSET_SIMULATION

	--DECLARE @id_critere AS int, @id_fga AS int, @coef AS int
	--SET @id_critere = 1
	--SET @id_fga = NULL
	--SET @coef = 4

	BEGIN TRANSACTION
	BEGIN TRY
		if @id_fga IS NULL
			BEGIN
			IF NOT EXISTS(	SELECT * 
							FROM ACT_COEF_SECTEUR_SIMULATION
							WHERE id_critere=@id_critere AND id_fga IS NULL AND date=@date
						 )
				BEGIN
				-- Création d'un coefficient
				INSERT INTO ACT_COEF_SECTEUR_SIMULATION (id_critere, id_fga, date, coef)
					VALUES (@id_critere, @id_fga, @date, @coef);
				END
			ELSE
				BEGIN
				-- Mise à jour d'un coefficient
				UPDATE ACT_COEF_SECTEUR_SIMULATION
				SET coef=@coef
				WHERE id_critere=@id_critere AND id_fga IS NULL AND date=@date
				END
			END
		ELSE
			BEGIN
			IF NOT EXISTS(	SELECT * 
						FROM ACT_COEF_SECTEUR_SIMULATION
						WHERE id_critere=@id_critere AND id_fga=@id_fga AND date=@date
					 )
				BEGIN
				-- Création d'un coefficient
				INSERT INTO ACT_COEF_SECTEUR_SIMULATION (id_critere, id_fga, date, coef)
					VALUES (@id_critere, @id_fga, @date, @coef);
				END
			ELSE
				BEGIN
				-- Mise à jour d'un coefficient
				UPDATE ACT_COEF_SECTEUR_SIMULATION
				SET coef=@coef
				WHERE id_critere=@id_critere AND id_fga=@id_fga AND date=@date
				END
			END
			
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		WHILE @@TRANCOUNT > 0
			ROLLBACK TRANSACTION
	END CATCH
		
	IF @@TRANCOUNT > 0
		COMMIT TRANSACTION
		
	--SELECT * fROM ACT_COEF_CRITERE
	--SELECT * FROM ACT_COEF_SECTEUR
END
