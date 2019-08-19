/* Procedure used to generate Score String */
ALTER PROCEDURE [dbo].[ACT_Get_Score_Sql]
	@root AS VARCHAR(100),
	@sql AS VARCHAR(MAX) OUTPUT
AS
	--DECLARE @root AS varchar(100)
	--SET @root = 'CROISSANCE'
	
	SELECT nom, root
	INTO #tmp
	FROM #criteres cr
	INNER JOIN #coefs co ON co.id_critere=cr.id_critere
	
	DELETE FROM #tmp
	WHERE NOT root=@root

	--DECLARE @sql VARCHAR(MAX)
	SELECT @sql = COALESCE(@sql + '+ ', '')
		+ 'COALESCE('
		+	nom + ' *	('
		+ '					SELECT coef'
		+ '					FROM #criteres cr'
		+ '					INNER JOIN #coefs co ON co.id_critere = cr.id_critere'
		+ '					WHERE co.id_fga = val.id_fga'
		+ '						AND cr.nom=''' + nom + ''''
		+ '				)'
		+ ', 0)'
	FROM #tmp
	GROUP BY nom
	
	SELECT @sql = 
		COALESCE('COALESCE( (' + @sql + ')'
				+ '			/NULLIF(('
				+ '				SELECT SUM(coef)'
				+ '				FROM #criteres cr'
				+ '				INNER JOIN #coefs co ON co.id_critere = cr.id_critere'
				+ '				WHERE co.id_fga = val.id_fga'
				+ '					AND root=''' + @root + ''''
				+ '			), 0)'
				+ '	, -100) '
		, 'NULL')

	DROP TABLE #tmp

GO