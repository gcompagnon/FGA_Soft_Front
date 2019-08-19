
/****** Object:  StoredProcedure [dbo].[ACT_DataGridBlendValeur]    Script Date: 11/25/2013 14:08:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_DataGridBlendValeur]

	@date As DATETIME,
	@id_fga As INTEGER,
	@FGA As VARCHAR(20)

AS

--Execute ACT_DataGridBlendValeur '17/12/2012', '20'
--SELECT * FROM ACT_COEF_CRITERE

		--DECLARE @date  VARCHAR(150) 
		--DECLARE @id_fga INTEGER 
		--DECLARE @FGA  VARCHAR(150) 
		--SET @date = '14/11/2013'
		--SET @id_fga = 501001
		--SET @FGA = 'Europe'


-- Get sectors' criterias.
SELECT Max(date) AS maxdate, id_critere, id_fga
INTO #secteurs
FROM ACT_COEF_SECTEUR
WHERE date <= @date
GROUP BY id_critere, id_fga
--SELECT * FROM #secteurs
--select *  from ACT_COEF_CRITERE

-- Get columns from sector ordered by group then position.
DECLARE @columns AS NVARCHAR(MAX)
SET @columns =
    stuff((	select N', ' + nom + N' AS "' + description + N'"'
			FROM (
					SELECT
						CAST(crit.nom AS NVARCHAR) AS nom
						, CAST(COALESCE(crit.description, crit.nom) AS NVARCHAR) AS "description"
						, COALESCE(crit.groupe, 1) AS groupe
						, crit.position + 10 * (parent.position + 10 * root.position) As pos
					FROM ACT_COEF_CRITERE crit
					INNER JOIN ACT_COEF_CRITERE parent ON parent.id_critere = crit.id_parent
					INNER JOIN ACT_COEF_CRITERE root ON root.id_critere = parent.id_parent
					INNER JOIN #secteurs sect ON sect.id_critere=crit.id_critere
					WHERE
						-- easiest way must exists...
						(sect.id_fga = @id_fga
							OR (NOT EXISTS(SELECT 1 FROM #secteurs WHERE id_fga=@id_fga) AND 
							id_fga IS NULL))
						AND crit.is_sector = 1
				) c
			ORDER BY c.groupe, c.pos
			for xml path(N'')
		  ), 1, 1, N'');
--SELECT @columns 
DROP TABLE #secteurs

DECLARE @Poids AS VARCHAR(50) = 'MXEU'
IF @FGA = 'USA'
BEGIN
SET @Poids = 'MXUSLC'
END

DECLARE @sql AS NVARCHAR(MAX)
SET @sql = N'select '
+N'		(SELECT TOP 1 TICKER FROM DATA_FACTSET WHERE ISIN = a.ISIN AND DATE=a.date) As "Ticker",
		a.company_name As "Company Name",
		a.currency As "Crncy",
		a.' + @Poids + ' AS "Poids",
		a.liquidity_test As "liquidity",'
+N'		FLOOR(r.rank_quant / ((	
						SELECT CAST(COUNT(*) + 1 AS float)
						FROM DATA_FACTSET d
						inner join  ref_security.SECTOR s1 on s1.code=d.SECTOR
						inner join ref_security.SECTOR_TRANSCO tr1 on tr1.id_sector1=s1.id
						inner join ref_security.SECTOR fga1 on fga1.id=tr1.id_sector2
						WHERE'
IF @FGA = 'Europe'
BEGIN
SET @sql = @sql + ' fga1.class_name=''FGA_EU'' AND d.MXEU is not NULL'
END/*1/((11)/5)+1 d.sector = a.sector*/
ELSE
BEGIN
SET @sql = @sql + ' fga1.class_name=''FGA_US'' AND d.MXUSLC is not NULL'
END
SET @sql = @sql +		' and d.ISIN is not null and d.date=a.date
							AND fga1.code=fga.code
						) / 5) + 1) As "Quint Quant",
		a.GARPN_TOTAL_S AS "Note",
		a.GARPN_GROWTH_S AS "Growth",
		a.GARPN_YIELD_S AS "Profit",
		a.GARPN_VALUE_S AS "Value",
		a.GARPN_ISR_S AS "ISR",
		isr.[Note Actions] AS "note ISR", '
SET @sql = @sql + @columns
+N' FROM DATA_FACTSET a
INNER JOIN (
				SELECT ISIN, ROW_NUMBER() OVER(ORDER BY GARPN_TOTAL_S DESC) AS rank_quant
				FROM DATA_FACTSET f
				inner join  ref_security.SECTOR s2 on s2.code=f.SECTOR
				inner join ref_security.SECTOR_TRANSCO tr2 on tr2.id_sector1=s2.id
				inner join ref_security.SECTOR fga2 on fga2.id=tr2.id_sector2
				WHERE'
IF @FGA = 'Europe'
BEGIN
SET @sql = @sql + ' fga2.class_name=''FGA_EU'' AND f.MXEU is not NULL'
END
ELSE
BEGIN
SET @sql = @sql + ' fga2.class_name=''FGA_US'' AND f.MXUSLC is not NULL'
END
SET @sql = @sql +		' and f.ISIN is not null and f.date=''' + CAST(@date AS NVARCHAR) + N''' AND  fga2.code='+ CAST(@id_fga AS NVARCHAR) + N'
			) r ON r.ISIN = a.ISIN
LEFT OUTER JOIN (
					SELECT
						isr2.isin
						, [Note Actions] AS ''Note Actions''
					FROM ISR_NOTE isr2
					INNER JOIN  (
									SELECT ISIN, max(date) AS maxdate
									FROM ISR_NOTE
									WHERE Date <= ''' + CAST(@date AS NVARCHAR) + N''' 
									GROUP BY ISIN
								) last ON isr2.ISIN = last.ISIN AND isr2.date = maxdate
				) isr ON a.ISIN = isr.ISIN
inner join  ref_security.SECTOR s on s.code=a.SECTOR
inner join ref_security.SECTOR_TRANSCO tr on tr.id_sector1=s.id
inner join ref_security.SECTOR fga on fga.id=tr.id_sector2
where'
IF @FGA = 'Europe'
BEGIN
SET @sql = @sql + ' fga.class_name=''FGA_EU'' AND a.MXEU is not NULL'
END
ELSE
BEGIN
SET @sql = @sql + ' fga.class_name=''FGA_US'' AND a.MXUSLC is not NULL'
END
SET @sql = @sql +		' and a.ISIN is not null and a.date=''' + CAST(@date AS NVARCHAR) + N''' and fga.code='+ CAST(@id_fga AS NVARCHAR) + N'
order by ''Quint quant'', a.GARPN_TOTAL_S DESC'
--SELECT @sql
EXEC (@sql)


