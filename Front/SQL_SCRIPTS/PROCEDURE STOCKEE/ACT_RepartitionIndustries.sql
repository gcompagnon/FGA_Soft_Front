
-- =============================================
-- Author:		Coquard Benjamin
-- Create date: 24/11/2014
-- Description:	Utilisation du script : 
-- Pour la tableau : 
--			EXECUTE ACT_RepartitionIndustries '03/10/2014', '-1', '', ''
-- Pour la gaph ptf : 
--			EXECUTE ACT_RepartitionIndustries '03/10/2014', '10', '[6100002]', 'null'
-- Pour la graph gaps : 
--			EXECUTE ACT_RepartitionIndustries '03/10/2014', '10', '[6100002]', 'MXFR'
-- =============================================

CREATE PROCEDURE [dbo].[ACT_RepartitionIndustries]
	@date AS DATETIME,
	@idSector As VARCHAR(8),
	@ptf as VARCHAR(32),
	@bench as VARCHAR(32)
AS
BEGIN

	if @idSector = -1
	-- On recupere toutes les industries à une date donnée.
	-- Partie servant a creer le tableau repartition.
	BEGIN
		CREATE TABLE #MERGED
		(
			INDUSTRY VARCHAR(256),
			MXFR FLOAT,
			[6100002] FLOAT,
			MXEM FLOAT,
			[6100030] FLOAT,
			AVEURO FLOAT,
			[6100004] FLOAT,
			[6100063] FLOAT,
			AVEUROPE FLOAT,
			[6100001] FLOAT,
			[6100033] FLOAT,
			MXEUM FLOAT,
			[6100062] FLOAT,
			MXEU FLOAT,
			[6100026] FLOAT,
			MXUSLC FLOAT,
			[6100024] FLOAT,
			[6100066] FLOAT			
		)

		DECLARE @sec AS	 varchar(50)

		DECLARE SecteurPere SCROLL CURSOR FOR
		SELECT GICS_SECTOR 
		FROM DATA_FACTSET 
		WHERE DATE='03/10/2014' and GICS_SECTOR is not null 
			and GICS_SUBINDUSTRY is null and MXEU is not null and MXUSLC is null
				
		OPEN SecteurPere

		FETCH NEXT FROM SecteurPere INTO @sec
		WHILE ( @@FETCH_STATUS = 0 )
		BEGIN
			SELECT distinct fga.LABEL AS Libelle,
			fac.MXFR as MXFR,
			fac.[6100002]  as [6100002], 
			fac.MXEM as MXEM,
			fac.[6100030] as [6100030], 
			fac.AVEURO as AVEURO, 
			fac.[6100004] as [6100004], 
			fac.[6100063] as [6100063], 
			fac.AVEUROPE as AVEUROPE, 
			fac.[6100001] as [6100001], 
			fac.[6100033] as [6100033],
			fac.MXEUM as MXEUM,
			fac.[6100062] as [6100062],
			fac.MXEU as MXEU,
			fac.[6100026] as [6100026],
			fac.MXUSLC as MXUSLC,
			fac.[6100024] as [6100024],
			fac.[6100066] as [6100066] 
			INTO #tmp
			FROM ref_security.SECTOR s
			INNER JOIN ref_security.SECTOR_TRANSCO st on st.id_sector1 = s.id
			INNER JOIN ref_security.SECTOR fga on fga.id = st.id_sector2
			inner join DATA_FACTSET fac on fac.SECTOR = s.code
			WHERE s.class_name = 'GICS' 
			and s.id_parent = (select id from ref_security.SECTOR where code = @sec) 
			AND fac.DATE = @date

			INSERT INTO #MERGED
			SELECT libelle as  INDUSTRY,
			(SUM(MXFR)) as MXFR,
			convert(decimal(10, 2),(SUM([6100002]) * 100))  as [6100002], 
			(SUM(MXEM)) as MXEM,
			convert(decimal(10, 2),(SUM([6100030]) * 100))  as [6100030], 
			convert(decimal(10, 2),(SUM(AVEURO) * 100))  as AVEURO, 
			convert(decimal(10, 2),(SUM([6100004]) * 100))  as [6100004], 
			convert(decimal(10, 2),(SUM([6100063]) * 100))  as [6100063], 
			convert(decimal(10, 2),(SUM(AVEUROPE) * 100))  as AVEUROPE, 
			convert(decimal(10, 2),(SUM([6100001]) * 100))  as [6100001], 
			convert(decimal(10, 2),(SUM([6100033]) * 100))  as [6100033],
			(SUM(MXEUM)) as MXEUM,
			convert(decimal(10, 2),(SUM([6100062]) * 100))  as [6100062],
			(SUM(MXEU)) as MXEU,
			convert(decimal(10, 2),(SUM([6100026]) * 100))  as [6100026],
			(SUM(MXUSLC)) as MXUSLC,
			convert(decimal(10, 2),(SUM([6100024]) * 100))  as [6100024],
			convert(decimal(10, 2),(SUM([6100066]) * 100))  as [6100066]
			FROM #TMP GROUP BY Libelle

			DROP TABLE #tmp

		FETCH NEXT FROM SecteurPere INTO @sec
		END

		CLOSE SecteurPere
		DEALLOCATE SecteurPere

		SELECT *,
		convert(decimal(10, 2),[6100002] - MXFR) as Ecart_6100002,
		convert(decimal(10, 2),[6100030] - MXEM) as Ecart_6100030,
		convert(decimal(10, 2),AVEURO - MXEM) as Ecart_AVEURO,
		convert(decimal(10, 2),[6100004] - MXEM) as Ecart_6100004,
		convert(decimal(10, 2),[6100063] - MXEM) as Ecart_6100063,
		convert(decimal(10, 2),AVEUROPE - MXEM) as Ecart_AVEUROPE,
		convert(decimal(10, 2),[6100001] - MXEM) as Ecart_6100001,
		convert(decimal(10, 2),[6100033] - MXEM) as Ecart_6100033,
		convert(decimal(10, 2),[6100062] - MXEUM) as Ecart_6100062,
		convert(decimal(10, 2),[6100026] - MXEU) as Ecart_6100026,
		convert(decimal(10, 2),[6100024] - MXUSLC) as Ecart_6100024,
		convert(decimal(10, 2),[6100066] - MXUSLC) as Ecart_6100066
		FROM #MERGED

		DROP TABLE #MERGED

	END
	ELSE
	-- ICI on creer le graph on utilise que secteur et ptf la date ne sert pas	
	BEGIN
		
		CREATE TABLE #MERGED2
		(
			DATE DATETIME,
			INDUSTRY VARCHAR(256),
			PTF FLOAT
		)

		DECLARE @sector AS varchar(8)

		DECLARE @sql as VARCHAR(MAX)

		DECLARE SecteurPere SCROLL CURSOR FOR
		SELECT GICS_SECTOR 
		FROM DATA_FACTSET 
		WHERE DATE=(SELECT MAX(DATE) FROM DATA_FACTSET) and GICS_SECTOR is not null 
			and GICS_SUBINDUSTRY is null and MXEU is not null and MXUSLC is null
				
		OPEN SecteurPere

		FETCH NEXT FROM SecteurPere INTO @sector
		WHILE ( @@FETCH_STATUS = 0 )
		BEGIN
			
			IF @sector = @idSector
			BEGIN
				IF @bench <> 'null'
				BEGIN
					SET @sql = N' SELECT distinct fac.date, fga.LABEL AS Libelle, ' +
						N' fac.'+ @bench + ' as BENCH, fac.' + @ptf + ' as PTF ' +
						N' INTO #tmp2 ' +
						N' FROM ref_security.SECTOR s ' +
						N' INNER JOIN ref_security.SECTOR_TRANSCO st on st.id_sector1 = s.id ' +
						N' INNER JOIN ref_security.SECTOR fga on fga.id = st.id_sector2 ' +
						N' inner join DATA_FACTSET fac on fac.SECTOR = s.code ' +
						N' WHERE s.class_name = ''GICS''  ' +
						N' and s.id_parent = (select id from ref_security.SECTOR where code = ' + @idSector + ') ' +
						N' INSERT INTO #MERGED2 ' +
						N' SELECT date, libelle as  INDUSTRY, ' +
						N' (SUM(PTF) * 100 - SUM(BENCH)) as PTF ' +
						N' FROM #tmp2 GROUP BY date, Libelle ORDER BY Libelle, date'+
						N' DROP TABLE #tmp2 '
					
					EXECUTE (@sql)
					BREAK
				END
				ELSE
				BEGIN
					SET @sql = N' SELECT distinct fac.date, fga.LABEL AS Libelle, ' +
						N' fac.' + @ptf + ' as PTF ' +
						N' INTO #tmp2 ' +
						N' FROM ref_security.SECTOR s ' +
						N' INNER JOIN ref_security.SECTOR_TRANSCO st on st.id_sector1 = s.id ' +
						N' INNER JOIN ref_security.SECTOR fga on fga.id = st.id_sector2 ' +
						N' inner join DATA_FACTSET fac on fac.SECTOR = s.code ' +
						N' WHERE s.class_name = ''GICS''  ' +
						N' and s.id_parent = (select id from ref_security.SECTOR where code = ' + @idSector + ') ' +
						N' INSERT INTO #MERGED2 ' +
						N' SELECT date, libelle as  INDUSTRY, ' +
						N' (SUM(PTF) * 100) as PTF ' +
						N' FROM #tmp2 GROUP BY date, Libelle ORDER BY Libelle, date'+
						N' DROP TABLE #tmp2 '
					
					EXECUTE (@sql)
					BREAK
				END
			END
			
		FETCH NEXT FROM SecteurPere INTO @sector
		
		END

		CLOSE SecteurPere
		DEALLOCATE SecteurPere

		SELECT * FROM #MERGED2

		DROP TABLE #MERGED2
		
	END		

END
