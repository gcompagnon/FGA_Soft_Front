
CREATE PROCEDURE RéconciliationInOmega
       @date DATETIME
AS 


--DECLARE @date DATETIME
--SET @date='29/07/2011'

DELETE FROM PTF_CHORUS_RAPPORT where date=@date and type_pb= 'Existe dans OMEGA pas dans CHORUS'


-- DANS OMEGA mais pas dans CHORUS
SELECT   
		f.compte As 'Compte_OMEGA', 
		c.compte As 'Compte_CHORUS',
		f.Libelle_Ptf, 
		f.isin_titre As 'Code_titre', 
		f.libelle_titre, 
		f.type_produit,
		c.quantite AS 'Quantite_CHORUS',
		f.quantite AS 'Quantite_OMEGA',
		f.coursclose As 'cours_OMEGA',
		f.coupon_couru As 'Cc_OMEGA',
		f.valeur_comptable AS 'Vc_OMEGA'
INTO #total
FROM PTF_FGA f
	right outer JOIN PTF_CHORUS_CORRESPONDANCE cor ON (f.compte = cor.compte_omega or (f.compte = '5300252' and cor.compte_omega='5300250'))
	LEFT OUTER JOIN PTF_CHORUS c ON  c.compte = cor.compte_chorus and  c.date_inventaire= @date and f.isin_titre = c.code_titre_ref
WHERE
	 f.dateinventaire=@date and f.groupe <> 'OPCVM' and f.code_titre NOT LIKE '%Cash%'

INSERT INTO PTF_CHORUS_RAPPORT 
(date,type_pb,Compte_OMEGA,Compte_CHORUS,libelle_ptf,code_titre,libelle_titre,type_produit,Quantite_CHORUS,Quantite_OMEGA,cours_OMEGA,Cc_OMEGA,Vc_OMEGA)
SELECT @date As 'date','Existe dans OMEGA pas dans CHORUS' As 'type_pb', * FROM #total 
WHERE compte_chorus is null

SELECT  
		compte_OMEGA As 'Compte OMEGA', 
		Libelle_Ptf As 'Libellé ptf', 
		code_titre As 'Code titre', 
		libelle_titre as 'Libellé titre', 
		type_produit as 'Type produit',
		Quantite_OMEGA as 'Quantité OMEGA'
FROM #total 
WHERE compte_chorus is null

DROP TABLE #total

--SELECT * FROM PTF_FGA where dateinventaire='31/05/2011' and compte='5300240'
--SELECT * FROM PTF_CHORUS_RAPPORT