
/****** Object:  StoredProcedure [dbo].[ACT_Add_Reco_Secteur]    Script Date: 12/05/2013 14:45:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Aurélien Brandicourt
-- Create date: 30/08/2012
-- Description:	Ajoute une recommandation à une valeur et la lie à la recommandation du supersecteur
-- =============================================
 
ALTER PROCEDURE [dbo].[ACT_Add_Reco_Secteur]
	@id AS int,
	@type AS VARCHAR(32),
	@recoMXEU AS VARCHAR(4),
	@recoMXEUM AS VARCHAR(4),
	@recoMXEM AS VARCHAR(4),
	@recoMXUSLC AS VARCHAR(4),
	@login as VARCHAR(32)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/*
	DECLARE @id AS int,	@type AS VARCHAR(32), @reco AS VARCHAR(4)
	
	Set @id='10'
	Set @type='ICB'
	Set @reco='+'
*/
	-------------
	DECLARE @idICB AS int
	DECLARE @idcomment AS int
	DECLARE @idchange AS int
	DECLARE @date as datetime

	
	BEGIN TRANSACTION
	SET @date=CAST(Floor(CAST(GETDATE() As float)) AS datetime)

    -- Insert statements for procedure here
    -- Récupère l'identifiant du supersecteur
	IF @type LIKE 'ICB'
		SET @idICB=@id
	ELSE
		SET @idICB=(SELECT id_federis FROM ACT_SUPERSECTOR sup
					INNER JOIN ACT_SECTOR sec on sec.id_supersector = sup.id
					INNER JOIN ACT_SUBSECTOR sub on sub.id_sector=sec.id
					INNER JOIN ACT_FGA_SECTOR fga on fga.id = sub.id_fga_sector
					WHERE fga.id=@id
					GROUP BY id_federis)

	-- Création de la table temporaire contenant les derniers commentaires 
	-- généraux du supersecteur et de ses secteurs FGA
	SELECT MAX(date) as maxdate, id_secteur, type
	INTO #comments
	FROM ACT_RECO_SECTOR
	WHERE type='ICB' AND id_secteur=@idICB
	GROUP BY id_secteur, type
	UNION
	SELECT MAX(date) as maxdate, id_secteur, type
	FROM ACT_SUPERSECTOR sup
	INNER JOIN ACT_SECTOR sec on sec.id_supersector = sup.id
	INNER JOIN ACT_SUBSECTOR sub on sub.id_sector=sec.id
	INNER JOIN ACT_RECO_SECTOR T3 on T3.id_secteur=sub.id_fga_sector
	WHERE id_federis=@idICB AND T3.type='FGA'
	GROUP BY id_secteur, type
	
	-- Ajout du nouveau commentaire de changement
	IF NOT EXISTS(SELECT date, id_secteur, type FROM ACT_RECO_SECTOR WHERE date=@date AND id_secteur=@id AND type=@type)
	BEGIN
		INSERT INTO ACT_RECO_COMMENT (comment)
		VALUES ('');
		SET @idchange = SCOPE_IDENTITY()
	END

	-- Ajout de la nouvelle recommandation
	IF NOT EXISTS(SELECT date, id_secteur, type FROM ACT_RECO_SECTOR WHERE date=@date AND id_secteur=@id AND type=@type)
	BEGIN
		IF @idcomment IS NULL
			BEGIN
			INSERT INTO ACT_RECO_COMMENT (comment)
				VALUES ('')
			SET @idcomment=SCOPE_IDENTITY()
			END
		ELSE
			IF NOT @date = CAST(Floor(CAST((select date FROM ACT_RECO_SECTOR WHERE id_comment = @idcomment AND type='ICB') As float)) AS datetime)
				BEGIN
					-- Nouveau commentaire général contenant la base de l'ancien
					INSERT INTO ACT_RECO_COMMENT (comment)
						(SELECT comment FROM ACT_RECO_COMMENT
							WHERE id=@idcomment);
					SET @idcomment=SCOPE_IDENTITY()
				END
	END
	
	IF EXISTS(SELECT date, id_secteur, type FROM ACT_RECO_SECTOR WHERE date=@date AND id_secteur=@id AND type=@type)
	BEGIN
		UPDATE ACT_RECO_SECTOR SET
			recommandation=@recoMXEU,
			reco_MXEUM=@recoMXEUM,
			reco_MXEM=@recoMXEM,
			reco_MXUSLC=@recoMXUSLC
			
		WHERE date=@date AND id_secteur=@id AND type=@type
	END
	ELSE
	BEGIN
		INSERT INTO ACT_RECO_SECTOR (date, id_secteur, type, recommandation, id_comment, id_comment_change,reco_MXEUM, reco_MXEM, reco_MXUSLC, loginModif)
		VALUES (@date, @id, @type, @recoMXEU, @idcomment, @idchange, @recoMXEUM, @recoMXEM, @recoMXUSLC, @login)
	END

	-- suppression de la table temporaire
	DROP TABLE #comments

	COMMIT TRANSACTION;
END