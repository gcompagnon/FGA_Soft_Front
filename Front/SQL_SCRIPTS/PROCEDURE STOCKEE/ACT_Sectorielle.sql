
DROP PROCEDURE ACT_Sectorielle

GO

CREATE PROCEDURE ACT_Sectorielle

AS

SELECT id_secteur, MAX(date) As date into #recommandation_fga FROM ACT_FGA_SECTOR_RECOMMANDATION GROUP BY id_secteur


Select distinct 
	i.id As id_industry, i.libelle As libelle_industry, 
	sus.id As id_supersector, sus.libelle As libelle_supersector, 
	f.id AS id_fga ,f.libelle As libelle_fga
into #sectors
from ACT_SUBSECTOR ss 
LEFT OUTER JOIN ACT_SECTOR s on s.id = ss.id_sector
LEFT OUTER JOIN ACT_SUPERSECTOR sus ON sus.id=s.id_supersector
LEFT OUTER JOIN ACT_FGA_SECTOR f ON f.id = ss.id_fga_sector
LEFT OUTER JOIN ACT_INDUSTRY i on i.id = sus.id_industry
order by id_fga


DECLARE @indice As VARCHAR(4)
Set @indice='SXXP'

DECLARE @date As DATETIME
Set @date = (SELECT MAX(date) FROM ACT_DATA_FACTSET_AGR)

SELECT id_secteur, MAX(date) As date into #recommandation_icb FROM ACT_ICB_SECTOR_RECOMMANDATION GROUP BY id_secteur

SELECT
	s.id  As 'Id ICB', s.libelle  As 'secteur ICB',
	CONVERT(VARCHAR, rfgad.date, 103)As 'Date',
	rfga.id_recommandation As 'FGA Recommandation',	
	a.SXXP As 'Poids Indice', /* CHANGER INDICE POUR LES ZONES GEO*/
	1/9 As 'Poids FGA',
	1/9-a.SXXP As 'Diff', /* CHANGER INDICE POUR LES ZONES GEO*/
	'' As 'New Reco'
FROM ACT_SUPERSECTOR s
LEFT OUTER JOIN ACT_DATA_FACTSET_AGR a ON a.date=@date and a.indice= @indice and a.ICB_SUPERSECTOR IS NOT NULL and a.FGA_SECTOR IS NULL and a.icb_supersector=s.id
LEFT OUTER JOIN #recommandation_fga rfgad ON rfgad.id_secteur=s.id
LEFT OUTER JOIN ACT_ICB_SECTOR_RECOMMANDATION rfga ON rfga.id_secteur=s.id and rfga.date=rfgad.date
ORDER BY s.id

DROP TABLE #recommandation_icb


SELECT
	s.id_supersector  As 'Id ICB', s.libelle_supersector  As 'secteur ICB',
	s.id_fga As 'Id FGA',s.libelle_fga As 'Secteur FGA',
	CONVERT(VARCHAR, rfgad.date, 103)As 'Date',
	rfga.id_recommandation As 'FGA Recommandation',	
	a.SXXP As 'Poids Indice', /* CHANGER INDICE POUR LES ZONES GEO*/
	1/9 As 'Poids FGA',
	1/9-a.SXXP As 'Diff', /* CHANGER INDICE POUR LES ZONES GEO*/
	'' As 'New Reco',
	'' As 'New FGA'
FROM #sectors s
LEFT OUTER JOIN ACT_DATA_FACTSET_AGR a ON a.date=@date and a.indice= @indice and a.ICB_SUPERSECTOR IS NOT NULL and a.FGA_SECTOR IS NOT NULL and a.fga_sector=s.id_fga
LEFT OUTER JOIN #recommandation_fga rfgad ON rfgad.id_secteur=s.id_fga
LEFT OUTER JOIN ACT_FGA_SECTOR_RECOMMANDATION rfga ON rfga.id_secteur=s.id_fga and rfga.date=rfgad.date
ORDER BY CONVERT(int,s.id_fga)


DROP TABLE #recommandation_fga
DROP TABLE #sectors

--EXECUTE ACT_Sectorielle
