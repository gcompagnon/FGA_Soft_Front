
CREATE PROCEDURE RéconciliationInChorus
       @date DATETIME
AS 


--DECLARE @date DATETIME
--SET @date='29/07/2011'

DELETE FROM PTF_CHORUS_RAPPORT where date=@date and type_pb= 'Existe dans CHORUS pas dans OMEGA'

-- DANS CHORUS mais pas dans OMEGA
SELECT   
		f.compte As 'Compte_OMEGA', 
		c.compte As 'Compte_CHORUS',
		c.Libelle_Ptf, 
		c.code_titre_ref AS 'code_titre', 
		c.libelle_titre, 
		c.type_produit,
		c.quantite AS 'Quantite_CHORUS',
		f.quantite AS 'Quantite_OMEGA',
		c.cours As 'Cours_CHORUS',
		c.valeur_comptable As 'Vc_CHORUS',
		c.coupon_couru As 'Cc_CHORUS',
		c.prix_ref As 'Px_ref_CHORUS'
INTO #total
FROM PTF_CHORUS c
	right OUTER JOIN PTF_CHORUS_CORRESPONDANCE cor ON c.compte = cor.compte_chorus
	LEFT OUTER JOIN PTF_FGA f ON  (f.compte = cor.compte_omega or (f.compte = '5300252' and cor.compte_omega='5300250')) and f.isin_titre = c.code_titre_ref and f.dateinventaire=@date and f.groupe <> 'OPCVM' and f.code_titre NOT LIKE '%Cash%'
WHERE
	 c.date_inventaire=@date



INSERT INTO PTF_CHORUS_RAPPORT 
(date,type_pb,Compte_OMEGA,Compte_CHORUS,libelle_ptf,code_titre,libelle_titre,type_produit,Quantite_CHORUS,Quantite_OMEGA,cours_CHORUS,Cc_CHORUS,Vc_CHORUS, Px_ref_CHORUS)
SELECT @date As 'date','Existe dans CHORUS pas dans OMEGA' As 'type_pb', * FROM #total 
where compte_omega is null

Select 
		Compte_CHORUS As 'Compte CHORUS',
		libelle_Ptf As 'Libellé ptf', 
		code_titre As 'Code titre', 
		libelle_titre As 'Libelle titre', 
		type_produit As 'Type produit',
		Quantite_CHORUS As 'Quantité CHORUS'
from #total 
where compte_omega is null

DROP TABLE #total

--SELECT * FROM PTF_FGA where dateinventaire='29/07/2011' and libelle_ptf like '%UNMI%'
--SELECT * FROM PTF_FGA where dateinventaire='29/07/2011' and compte IN ('5300252','5300250')
--SELECT * FROM PTF_CHORUS where date_inventaire='29/07/2011' and compte='5300250'
--SELECT * FROM PTF_CHORUS_RAPPORT