
CREATE PROCEDURE RéconciliationCouponCouru
       @date DATETIME,
	   @epsilon FLOAT
AS

/*
DECLARE @date DATETIME
SET @date='29/07/2011'
DECLARE @epsilon AS FLOAT
SET @epsilon= 0.0001
*/

DELETE FROM PTF_CHORUS_RAPPORT where date=@date and type_pb = 'Ecart coupon couru'


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
		f.coupon_couru - c.coupon_couru AS Ecart,
		f.coupon_couru As 'Coupon couru OMEGA',
		c.coupon_couru As 'Coupon couru CHORUS'
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
		f.coupon_couru,
		c.coupon_couru
HAVING ABS(f.coupon_couru - c.coupon_couru)> @epsilon
ORDER by ABS(f.coupon_couru - c.coupon_couru) DESC



INSERT INTO PTF_CHORUS_RAPPORT 
(date,type_pb,Compte_OMEGA,Compte_CHORUS,libelle_ptf,code_titre,libelle_titre,type_produit,Cc_OMEGA,Cc_CHORUS, ecart)
SELECT
		@date As date,
		'Ecart coupon couru' As type_pb,
  		f.Compte AS 'Compte_OMEGA',
		c.Compte AS 'Compte_CHORUS',
		f.Libelle_ptf, 
		f.isin_titre AS 'code_titre', 
		f.libelle_titre, 
		f.type_produit,
		f.coupon_couru As 'Cc_OMEGA',
		c.coupon_couru As 'Cc_CHORUS',
		f.coupon_couru - c.coupon_couru AS Ecart
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
		f.coupon_couru,
		c.coupon_couru
HAVING ABS(f.coupon_couru - c.coupon_couru)> @epsilon
ORDER by ABS(f.coupon_couru - c.coupon_couru) DESC


DROP TABLE #toto_fga


--DELETE FROM PTF_CHORUS_RAPPORT where date='29/07/2011'
--SELECT * FROM PTF_CHORUS_RAPPORT