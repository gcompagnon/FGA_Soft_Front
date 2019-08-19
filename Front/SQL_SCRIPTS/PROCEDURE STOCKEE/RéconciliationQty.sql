
CREATE PROCEDURE RéconciliationQty
       @date DATETIME,
	   @epsilon FLOAT
AS 


/*
DECLARE @date DATETIME
SET @date='29/07/2011'
DECLARE @epsilon AS FLOAT
SET @epsilon= 0.0001
*/

DELETE FROM PTF_CHORUS_RAPPORT where date=@date and type_pb= 'Ecart quantité'


-- data chorus
select 
		c.compte AS 'Compte_CHORUS',
		sum(c.quantite) as 'QUANTITE_CHORUS',
		sum(c.valeur_comptable) AS 'VC_CHORUS' ,
		avg(prix_ref)			as 'Px_Ref_Chorus',			
		c.code_titre_ref
into #toto_chorus
from PTF_CHORUS c
where c.date_inventaire=@date 
--and c.compte='5300250' 
group by c.compte, c.code_titre_ref


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
--and f.compte in ('5300250', '5300252')


SELECT 
		Compte_OMEGA,
		f.Libelle_OMEGA,  
		f.isin_titre,
		--f.libelle_titre, 
		f.type_produit,
		sum(f.quantite) AS 'Quantite_OMEGA'
INTO #toto_OMEGA
FROM #toto_fga f
group by 
        compte_OMEGA, 		
        f.Libelle_OMEGA, 
		f.isin_titre, 
		--f.libelle_titre, 
		f.type_produit


--Ecart quantité
SELECT
  		f.Compte_OMEGA,
		c.Compte_CHORUS,
		f.Libelle_OMEGA, 
		f.isin_titre, 
		'                                                                                ' as libelle_titre, 
		f.type_produit,
		c.VC_CHORUS ,
		Px_Ref_Chorus,
		c.Quantite_CHORUS ,
		f.Quantite_OMEGA, 
		CASE WHEN (f.Quantite_OMEGA - c.Quantite_CHORUS) =0 then 0
		ELSE
			CASE WHEN f.type_produit LIKE '%Obligations%' and c.px_ref_chorus > 0 THEN 
				(f.Quantite_OMEGA -(c.VC_CHORUS/c.px_ref_chorus)*100) 
			ELSE 
				(f.Quantite_OMEGA - c.Quantite_CHORUS) 
			END 
		END AS Ecart

INTO #total
FROM #toto_omega f, #toto_chorus c , PTF_CHORUS_CORRESPONDANCE cor
WHERE
f.compte_OMEGA = cor.compte_omega and c.Compte_CHORUS = cor.compte_chorus and
	f.isin_titre = c.code_titre_ref


update #total 
set libelle_titre  = f.libelle_titre
from PTF_FGA f, #total t
where  f.isin_titre= t.isin_titre

INSERT INTO PTF_CHORUS_RAPPORT 
(date,type_pb,Compte_OMEGA,Compte_CHORUS,libelle_ptf,code_titre,libelle_titre,type_produit,Quantite_CHORUS,Quantite_OMEGA,Vc_CHORUS, Px_ref_CHORUS, ecart)
SELECT 
		@date As 'date',
		'Ecart quantité' As type_pb,
		Compte_OMEGA, 
		Compte_CHORUS,
		Libelle_OMEGA As 'Libelle_ptf', 
		isin_titre As 'code_titre', 
		libelle_titre as 'Libelle_titre', 
		type_produit 'type_produit',
		quantite_CHORUS,
		quantite_OMEGA, 
		VC_CHORUS,
		Px_Ref_Chorus As 'Px_ref_CHORUS',
		(SUM(Ecart)) As 'Ecart'
FROM #total
GROUP BY 
		Compte_OMEGA , 
		Compte_CHORUS,
		Libelle_OMEGA, 
		isin_titre, 
		libelle_titre, 
		type_produit,
		Px_Ref_Chorus,
		VC_CHORUS,
		quantite_CHORUS,
		quantite_OMEGA 
HAVING abs(SUM(Ecart)) > @epsilon
ORDER BY abs(SUM(Ecart)) DESC

SELECT 
		Compte_OMEGA AS 'Compte OMEGA', 
		Compte_CHORUS AS'Compte CHORUS',
		Libelle_OMEGA As 'Libellé Portefeuille', 
		isin_titre As 'Code titre', 
		libelle_titre as 'Libellé titre', 
		type_produit 'Type produit',
		(SUM(Ecart)) As 'Ecart Quantité',
		quantite_CHORUS 'Quantité CHORUS',
		quantite_OMEGA 'Quantité OMEGA', 
	    VC_CHORUS As 'Valeur comptable CHORUS',
		Px_Ref_Chorus As 'Prix ref CHORUS'
FROM #total
GROUP BY 
		Compte_OMEGA , 
		Compte_CHORUS,
		Libelle_OMEGA, 
		isin_titre, 
		libelle_titre, 
		type_produit,
		Px_Ref_Chorus,
		VC_CHORUS,
		quantite_CHORUS,
		quantite_OMEGA 
HAVING abs(SUM(Ecart)) > @epsilon
ORDER BY abs(SUM(Ecart)) DESC

--select * from #toto_chorus
--select * from #toto_omega


DROP TABLE #total
DROP TABLE #toto_chorus
DROP TABLE #toto_OMEGA
DROP TABLE #toto_FGA


--SELECT * FROM PTF_CHORUS_RAPPORT
--SELECT * FROM PTF_FGA where dateinventaire='29/07/2011' and compte='4030005' and code_titre='FR0000492076'
--DELETE FROM PTF_FGA where dateinventaire='29/07/2011'	
--DELETE FROM PTF_CHORUS_RAPPORT