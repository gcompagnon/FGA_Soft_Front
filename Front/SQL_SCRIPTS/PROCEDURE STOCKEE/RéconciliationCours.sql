
CREATE PROCEDURE RéconciliationCours
       @date DATETIME,
	   @epsilon FLOAT
AS 


/*
DECLARE @date DATETIME
SET @date='29/07/2011'
DECLARE @epsilon AS FLOAT
SET @epsilon= 0.0001
*/

DELETE FROM PTF_CHORUS_RAPPORT where date=@date and type_pb = 'Ecart cours'


SELECT 		
        'Compte_OMEGA' = case when f.compte='5300252' then '5300250'
							else f.compte
					     end, 
        'Libelle_OMEGA' = case when f.compte='5300252' then 'U.N.M.I OBLIGATIONS'
							else f.Libelle_PTF
					     end, 
        * 
into #toto_FGA 
FROM PTF_FGA f 
where
	f.groupe <> 'OPCVM' and f.isin_titre NOT LIKE '%Cash%' and
	f.dateinventaire=@date


SELECT
  		f.Compte AS 'Compte OMEGA',
		c.Compte AS 'Compte CHORUS',
		f.Libelle_ptf As 'Libellé ptf', 
		f.isin_titre AS 'Code titre', 
		f.libelle_titre As 'Libellé titre', 
		f.type_produit As 'Type produit',
		f.coursclose - c.cours AS Ecart,
		f.coursclose As 'Cours OMEGA',
		c.cours As 'Cours CHORUS'
FROM #toto_fga f, PTF_CHORUS c , PTF_CHORUS_CORRESPONDANCE cor
WHERE
	f.compte_OMEGA = cor.compte_omega and c.compte = cor.compte_chorus and
	f.isin_titre = c.code_titre_ref and c.date_inventaire = @date
GROUP BY 
		f.Compte,
		c.Compte,
		f.Libelle_ptf, 
		f.isin_titre, 
		f.libelle_titre, 
		f.type_produit,
		f.coursclose,
		c.cours
HAVING ABS(f.coursclose - c.cours)> @epsilon
ORDER by ABS(f.coursclose - c.cours) DESC



INSERT INTO PTF_CHORUS_RAPPORT 
(date,type_pb,Compte_OMEGA,Compte_CHORUS,libelle_ptf,code_titre,libelle_titre,type_produit,cours_OMEGA,cours_CHORUS, ecart)
SELECT
		@date As date,
		'Ecart cours' As type_pb,
  		f.Compte AS 'Compte_OMEGA',
		c.Compte AS 'Compte_CHORUS',
		f.Libelle_ptf, 
		f.isin_titre AS 'code_titre', 
		f.libelle_titre, 
		f.type_produit,
		f.coursclose As 'cours_OMEGA',
		c.cours As 'cours_CHORUS',
		f.coursclose - c.cours AS Ecart
FROM #toto_fga f, PTF_CHORUS c , PTF_CHORUS_CORRESPONDANCE cor
WHERE
	f.compte_OMEGA = cor.compte_omega and c.compte = cor.compte_chorus and
	f.isin_titre = c.code_titre_ref and c.date_inventaire = @date
GROUP BY 
		f.Compte,
		c.Compte,
		f.Libelle_ptf, 
		f.isin_titre, 
		f.libelle_titre, 
		f.type_produit,
		f.coursclose,
		c.cours
HAVING ABS(f.coursclose - c.cours)> @epsilon
ORDER by ABS(f.coursclose - c.cours) DESC


DROP TABLE #toto_fga

--
--DELETE FROM PTF_CHORUS_RAPPORT where date='29/07/2011'
--SELECT * FROM PTF_CHORUS_RAPPORT

/*
--Ecart cours close en pourcentage
SELECT f.compte As 'Compte', f.code_titre As 'Code titre', f.libelle_titre As 'Libellé titre',f.type_produit,f.coursclose AS 'Cours OMEGA',c.cours AS 'Cours CHORUS' ,f.valeur_boursiere As 'Valeur boursiere OMEGA',c.valeur_boursiere As 'Valeur boursiere CHORUS', SUM(ABS(f.coursclose - c.cours)/c.cours)*100 As 'Ecart Cours close %'  
FROM PTF_FGA f, PTF_CHORUS c 
WHERE
	f.compte = c.compte and
	f.code_titre = c.code_titre and
	f.dateinventaire = c.date_inventaire and f.dateinventaire='31/05/2011'
GROUP BY f.compte, f.code_titre,f.libelle_titre,f.type_produit,f.coursclose,c.cours,f.valeur_boursiere,c.valeur_boursiere
HAVING SUM(ABS(f.coursclose - c.cours)/c.cours) > @epsilon
ORDER BY 'Ecart Cours close %' DESC
*/