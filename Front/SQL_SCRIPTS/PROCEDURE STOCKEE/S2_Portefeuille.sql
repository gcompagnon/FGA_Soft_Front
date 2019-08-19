--Execute S2_Portefeuille '11/04/2012'

DROP PROCEDURE S2_Portefeuille

GO

CREATE PROCEDURE S2_Portefeuille

		@date As DATETIME

AS



--DECLARE @date As DATETIME
--SET @date='25/04/2012'

--SELECT *,ROUND(life,0) FROM TX_IBOXX where date='11/04/2012' order by life
--EXECUTE S2_Portefeuille '25/04/2012'
--SELECT * FROM S2_PORT_TOTAL where date='25/04/2012' order by portfolio
--SELECT * FROM S2_PORT_COMPO_TEMP where date='25/04/2012'
--SELECT * FROM S2_PORT_COMPO where date='25/04/2012'

/*
	CALCUL DES AGREGATIONS
*/

--2) Pré-calcul des agrégation des portefeuilles pb à cause des case
SELECT s.date, s.portfolio, 
	SUM(s.weight*i.duration)/100 As duration,
	SUM(s.weight*i.AnnualModDuration)/100 As sensi, 
	SUM(s.weight*i.life)/100 As life,
	SUM(s.weight*i.AnnualYield)/100 As yield,
	CASE WHEN r.sensi < i.AnnualModDuration THEN SUM(s.weight*r.scr*r.sensi)/100 ELSE SUM(s.weight*r.scr*i.AnnualModDuration)/100 END As scr,
	CASE WHEN i.rating = 'AAA' THEN SUM(s.weight) END As AAA,
	CASE WHEN i.rating = 'AA' THEN SUM(s.weight) END As AA,
	CASE WHEN i.rating = 'A' THEN SUM(s.weight) END As A,
	CASE WHEN i.rating = 'BBB' THEN SUM(s.weight) END As BBB,
	SUM(s.weight*d.defaut)/100 As defaut
INTO #PORTFOLIO_CARACT    
FROM S2_PORT_COMPO s 
LEFT OUTER JOIN TX_IBOXX i ON i.date=@date and s.isin=i.isin
LEFT OUTER JOIN S2_RATING_PARAM r ON r.rating=i.rating
LEFT OUTER JOIN S2_DEFAULT d ON d.rating=i.rating and d.year= (CASE WHEN ROUND(i.life,0) < 15 THEN ROUND(i.life,0) ELSE 15 END)
WHERE s.date=@date and s.portfolio NOT IN (SELECT t.portfolio FROM S2_PORT_TOTAL t where t.date=@date)
GROUP BY s.date, s.portfolio,r.sensi ,i.AnnualModDuration, i.rating


INSERT INTO S2_PORT_TOTAL(date,portfolio,duration, sensi, life,yield,scr,AAA,AA,A,BBB,defaut) 
SELECT date, portfolio, sum(duration), sum(sensi), sum(life), sum(yield) , sum(scr), SUM(AAA), SUM(AA), SUM(A), SUM(BBB), SUM(defaut) 
FROM #PORTFOLIO_CARACT 
GROUP BY date, portfolio
DROP TABLE #PORTFOLIO_CARACT


/*
SELECT * FROm #PORTFOLIO_CARACT
SELECT * FROm S2_PORT_COMPO where date='25/04/2012'


SELECT CASE WHEN r.life < i.life THEN SUM(i.indexWeight*r.scr*r.life)/100 ELSE SUM(i.indexWeight*r.scr*i.life)/100 END As scr
into #test
FROm TX_IBOXX i 
LEFT OUTER JOIN S2_RATING_PARAM r ON r.rating=i.rating
where date='30/12/2011'
GROUP BY r.life ,i.life

SELECT SUM(scr) FROM #test




DROP TABLE #test
SELECT *, 
DATEDIFF(Day,dateinventaire,maturite)/365.25 AS life, 
(valeur_boursiere + coupon_couru)*100/ (Select SUm(valeur_boursiere + coupon_couru)FROm PTF_FGA where dateinventaire='30/12/2011' and groupe ='CMAV' and compte='08115')
 As indexWeight
 into #test
FROm PTF_FGA where dateinventaire='30/12/2011' and groupe ='CMAV' and compte='08115'

DROP TABLE #scr
SELECT CASE WHEN r.life < i.life THEN SUM(i.indexWeight*r.scr*r.life)/100 ELSE SUM(i.indexWeight*r.scr*i.life)/100 END As scr
into #scr
FROm  #test i 
LEFT OUTER JOIN S2_RATING_PARAM r ON r.rating=i.rating
where dateinventaire='30/12/2011'
GROUP BY r.life ,i.life

SELECT SUM(scr) FROM #scr





DROP TABLE #test
SELECT *, 
DATEDIFF(Day,dateinventaire,maturite)/365.25 AS life, 
(valeur_boursiere + coupon_couru)*100/ (Select SUm(valeur_boursiere + coupon_couru)FROm PTF_FGA where dateinventaire='30/12/2011' and groupe ='QUATREM' and libelle_ptf like '%Obligations%')
 As indexWeight
 into #test
FROm PTF_FGA where dateinventaire='30/12/2011' and groupe ='QUATREM' and libelle_ptf like '%Obligations%'

DROP TABLE #scr
SELECT CASE WHEN r.life < i.life THEN SUM(i.indexWeight*r.scr*r.life)/100 ELSE SUM(i.indexWeight*r.scr*i.life)/100 END As scr
into #scr
FROm  #test i 
LEFT OUTER JOIN S2_RATING_PARAM r ON r.rating=i.rating
where dateinventaire='30/12/2011'
GROUP BY r.life ,i.life

SELECT SUM(scr) FROM #scr




SELECT * FROm PTF_FGA where dateinventaire='30/12/2011' and groupe ='QUATREM' and libelle_ptf like '%Obligations%'

SELECT * FROM S2_RATING_PARAM
SELECT rating, scr , life into #test FROM S2_RATING_PARAM

INSERT INTO S2_RATING_PARAM
SELECT * FROM #test



SELECT * FROM S2_PORT_COMPO where date='25/04/2012' 
SELECT * FROM TX_IBOXX where date='11/04/2012' 

*/