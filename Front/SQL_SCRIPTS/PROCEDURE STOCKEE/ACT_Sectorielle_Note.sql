--Execute ACT_Sectorielle_Note

DROP PROCEDURE ACT_Sectorielle_Note

GO

CREATE PROCEDURE ACT_Sectorielle_Note

AS

DECLARE @nbr_indic AS FLOAT
SET @nbr_indic = (SELECT COUNT(id) FROM ACT_THEME where valide='1' and niveau='secteur')

CREATE TABLE #note(
	date DATETIME,
	id_secteur  VARCHAR(4) COLLATE DATABASE_DEFAULT,
	id_theme  VARCHAR(5) COLLATE DATABASE_DEFAULT,
	id_note VARCHAR(3) COLLATE DATABASE_DEFAULT,
	note  FLOAT
)

INSERT INTO #note
SELECT Max(date) as date, id_secteur, id_theme, '    ' as id_note, 0.01 as note 
FROM ACT_FGA_SECTOR_NOTE n, ACT_THEME t
WHERE t.id=n.id_theme and t.valide='1' and t.niveau='secteur'
GROUP BY id_secteur, id_theme
--SELECT * FROM ACT_THEME

--On met les notes 
update #note
set id_note = r.id_note,
	note = no.poids
from #note as n
LEFT OUTER JOIN ACT_FGA_SECTOR_NOTE r ON r.date=n.date and r.id_secteur = n.id_secteur and r.id_theme=n.id_theme
LEFT OUTER JOIN ACT_NOTE no ON r.id_note = no.id
--SELECT * FROM #note

--Calcul de la note global ./10 par secteur
INSERT INTO #note
SELECT NULL, id_secteur, 'tot', '', SUM(note)/@nbr_indic FROM #note GROUP BY id_secteur

--Correspondance des secteurs
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

/**
--Grille de restitution par secteur
SELECT
	sec.libelle_supersector As 'SUPERSECTEURS OFFICIEL',
	s.libelle  As 'Secteur FGA',
	r0.note As 'Note ./10',
	sam.id_note As 'Sensibilité à amélioration macro',
	exEm.id_note As 'CA exposé aux émergents',
	exU.id_note As 'CA exposé aux US',
	hcr.id_note As 'Hausse du coût de refinancement',
	af2.id_note As 'Augmentation fiscalité',
	sdd.id_note As 'Sécurité du dividende',
	beuro.id_note As 'Baisse de l''€ favorable',
	heuro.id_note As 'Hausse de l''€ favorable',
	she.id_note As 'Sensibilité à la hausse du prix de l''énergie',
	shmp.id_note As 'Sensibilité à la hausse des coûts matières',
	exEu.id_note As 'CA exposé en Europe',
	share.id_note As '"Shareholder friendly"'
FROM ( select distinct id_secteur from #note) as r
LEFT OUTER JOIN ACT_FGA_SECTOR s ON s.id=r.id_secteur
LEFT OUTER JOIN #sectors sec ON r.id_secteur = sec.id_fga
LEFT OUTER JOIN #note as r0 ON r0.id_secteur = r.id_secteur and r0.id_theme='tot'
LEFT OUTER JOIN #note as sam ON sam.id_secteur = r.id_secteur and sam.id_theme='sam'
LEFT OUTER JOIN #note as exEm ON exEm.id_secteur = r.id_secteur and exEm.id_theme='exEm' 
LEFT OUTER JOIN #note as exU ON exU.id_secteur = r.id_secteur and exU.id_theme='exU' 
LEFT OUTER JOIN #note as hcr ON hcr.id_secteur = r.id_secteur and hcr.id_theme='hcr'
LEFT OUTER JOIN #note as af2 ON af2.id_secteur = r.id_secteur and af2.id_theme='af2'
LEFT OUTER JOIN #note as sdd ON sdd.id_secteur = r.id_secteur and sdd.id_theme='sdd'
LEFT OUTER JOIN #note as beuro ON beuro.id_secteur = r.id_secteur and beuro.id_theme='beuro'
LEFT OUTER JOIN #note as heuro ON heuro.id_secteur = r.id_secteur and heuro.id_theme='heuro'
LEFT OUTER JOIN #note as she ON she.id_secteur = r.id_secteur and she.id_theme='she'
LEFT OUTER JOIN #note as shmp ON shmp.id_secteur = r.id_secteur and shmp.id_theme='shmp' 
LEFT OUTER JOIN #note as exEu ON exEu.id_secteur = r.id_secteur and exEu.id_theme='exEu' 
LEFT OUTER JOIN #note as share ON share.id_secteur = r.id_secteur and share.id_theme='share'
ORDER BY sec.id_supersector
**/


--Grille de restitution par secteur
SELECT
	CONVERT(int,s.id_fga) As ' ',
	--s.libelle_supersector As 'Super Sector ICB',
	s.libelle_fga  As 'Secteur FGA',
	r0.note As 'Note ./10',
	sam.id_note As 'Sensibilité à amélioration macro',
	exEm.id_note As 'CA exposé aux émergents',
	exU.id_note As 'CA exposé aux US',
	hcr.id_note As 'Hausse du coût de refinancement',
	af2.id_note As 'Augmentation fiscalité',
	sdd.id_note As 'Sécurité du dividende',
	beuro.id_note As 'Baisse de l''€ favorable',
	heuro.id_note As 'Hausse de l''€ favorable',
	she.id_note As 'Sensibilité à la hausse du prix de l''énergie',
	shmp.id_note As 'Sensibilité à la hausse des coûts matières',
	exEu.id_note As 'CA exposé en Europe',
	share.id_note As '"Shareholder friendly"'
FROM #sectors s
LEFT OUTER JOIN #note as r0 ON r0.id_secteur = s.id_fga and r0.id_theme='tot'
LEFT OUTER JOIN #note as sam ON sam.id_secteur = s.id_fga and sam.id_theme='sam'
LEFT OUTER JOIN #note as exEm ON exEm.id_secteur = s.id_fga and exEm.id_theme='exEm' 
LEFT OUTER JOIN #note as exU ON exU.id_secteur = s.id_fga and exU.id_theme='exU' 
LEFT OUTER JOIN #note as hcr ON hcr.id_secteur = s.id_fga and hcr.id_theme='hcr'
LEFT OUTER JOIN #note as af2 ON af2.id_secteur = s.id_fga and af2.id_theme='af2'
LEFT OUTER JOIN #note as sdd ON sdd.id_secteur = s.id_fga and sdd.id_theme='sdd'
LEFT OUTER JOIN #note as beuro ON beuro.id_secteur = s.id_fga and beuro.id_theme='beuro'
LEFT OUTER JOIN #note as heuro ON heuro.id_secteur = s.id_fga and heuro.id_theme='heuro'
LEFT OUTER JOIN #note as she ON she.id_secteur = s.id_fga and she.id_theme='she'
LEFT OUTER JOIN #note as shmp ON shmp.id_secteur = s.id_fga and shmp.id_theme='shmp' 
LEFT OUTER JOIN #note as exEu ON exEu.id_secteur = s.id_fga and exEu.id_theme='exEu' 
LEFT OUTER JOIN #note as share ON share.id_secteur = s.id_fga and share.id_theme='share'
ORDER BY CONVERT(int,s.id_fga)


DROP TABLE #note
DROP TABLE #sectors




--SELECT MAX(date) FROm ACT_FGA_SECTOR_RECOMMANDATION
--SELECT * FROM #sectors
--(SELECT CONVERT(char(10), Max(date), 103) FROm ACT_FGA_SECTOR_RECOMMANDATION


