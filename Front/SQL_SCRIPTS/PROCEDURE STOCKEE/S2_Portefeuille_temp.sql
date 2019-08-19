--Execute S2_Portefeuille '11/04/2012'

DROP PROCEDURE S2_Portefeuille_temp

GO

CREATE PROCEDURE S2_Portefeuille_temp

		@date As DATETIME

AS


--DECLARE @date As DATETIME
--SET @date='01/05/2012'
--EXECUTE S2_Portefeuille '01/05/2012'

--SELECT * FROM S2_PORT_TOTAL where date='25/04/2012' order by portfolio
--SELECT * FROM S2_PORT_COMPO_TEMP where date='25/04/2012'
--SELECT * FROM S2_PORT_COMPO where date='25/04/2012'


/*
  TABLE TMP
*/

BEGIN TRY

BEGIN TRANSACTION

--DECLARE @date As DATETIME
--SET @date='01/05/2012'


--0)On prend la liste des nouveaux portefeuilles
(SELECT portfolio into #plein FROM S2_PORT_COMPO_TEMP where date=@date group by portfolio HAVING (SUM(weight)) > 99.9 AND (SUM(weight)) < 100.1)

--1)On fait un transfert de base pour la rapidité
INSERT INTO S2_PORT_COMPO(date,portfolio,isin,rating,issuerTicker,weight)
SELECT  date, 
        t.portfolio,
        t.isin,
        t.rating,
        t.issuerTicker,
        t.weight 
FROM S2_PORT_COMPO_TEMP t, #plein p where date=@date and t.portfolio= p.portfolio

/*
    CALCUL DES AGREGATIONS
*/


DECLARE @sens1 As FLOAT
SET @sens1=0.95

--2) Pré-calcul des agrégation des portefeuilles pb à cause des case
SELECT s.date, s.portfolio, 
    SUM(s.weight*i.street_duration)/100 As duration,
    SUM(s.weight*i.Annual_Modified_Duration)/100 As sensi, 
    SUM(s.weight*i.years_to_maturity)/100 As life,
    SUM(s.weight*i.Annual_Yield)/100 As yield,
    CASE WHEN i.sector_level1='Sovereigns' THEN 
		0
	ELSE 
		CASE WHEN r.sensi < i.Annual_Modified_Duration THEN 
			SUM(s.weight*r.scr*r.sensi)/100 
		ELSE 
			SUM(s.weight*r.scr*i.Annual_Modified_Duration)/100 
		END 
	END	As scr,
    CASE WHEN i.iboxx_rating = 'AAA' THEN SUM(s.weight) END As AAA,
    CASE WHEN i.iboxx_rating = 'AA' THEN SUM(s.weight) END As AA,
    CASE WHEN i.iboxx_rating = 'A' THEN SUM(s.weight) END As A,
    CASE WHEN i.iboxx_rating = 'BBB' THEN SUM(s.weight) END As BBB,
    CASE WHEN i.iboxx_rating<>'AAA' and i.iboxx_rating<>'AA'  and i.iboxx_rating<>'A'  and i.iboxx_rating<>'BBB' and i.iboxx_rating IS NOT NULL THEN SUM(s.weight) END As HY,
    SUM(s.weight*d.defaut)/100 As defaut,
    SUM(s.weight*t1.proba)/100 As transition_1yr,
    SUM(s.weight*t5.proba)/100 As transition_5yr,
    CASE WHEN i.sector_level1='Sovereigns' THEN 
		0
	ELSE		
		CASE WHEN r.sensi < i.Annual_Modified_Duration THEN 
			( SUM(s.weight*t1AAA.proba*rAAA.scr*(rAAA.sensi-@sens1)) + SUM(s.weight*t1AA.proba*rAA.scr*(rAA.sensi-@sens1)) + SUM(s.weight*t1A.proba*rA.scr*(rA.sensi-@sens1)) + SUM(s.weight*t1BBB.proba*rBBB.scr*(rBBB.sensi-@sens1)) + SUM(s.weight*t1BB.proba*rBB.scr*(rBB.sensi-@sens1)) + SUM(s.weight*t1B.proba*rB.scr*(rB.sensi-@sens1)) + SUM(s.weight*t1CCC.proba*rCCC.scr*(rCCC.sensi-@sens1)) + SUM(s.weight*t1D.proba*rD.scr*(rD.sensi-@sens1)) + SUM(s.weight*t1NULL.proba*rNULL.scr*(rNULL.sensi-@sens1))   )/10000 
		ELSE 
			( SUM(s.weight*t1AAA.proba*rAAA.scr*(i.Annual_Modified_Duration-@sens1)) +  SUM(s.weight*t1AA.proba*rAA.scr*(i.Annual_Modified_Duration-@sens1)) + SUM(s.weight*t1A.proba*rA.scr*(i.Annual_Modified_Duration-@sens1)) + SUM(s.weight*t1BBB.proba*rBBB.scr*(i.Annual_Modified_Duration-@sens1)) + SUM(s.weight*t1BB.proba*rBB.scr*(i.Annual_Modified_Duration-@sens1)) + SUM(s.weight*t1B.proba*rB.scr*(i.Annual_Modified_Duration-@sens1)) + SUM(s.weight*t1CCC.proba*rCCC.scr*(i.Annual_Modified_Duration-@sens1)) + SUM(s.weight*t1D.proba*rD.scr*(i.Annual_Modified_Duration-@sens1)) + SUM(s.weight*t1NULL.proba*rNULL.scr*(i.Annual_Modified_Duration-@sens1))   )/10000 
		END 
	END As scr_1yr
    
INTO #PORTFOLIO_CARACT_TMP    
FROM S2_PORT_COMPO_TEMP s
LEFT OUTER JOIN #plein p ON s.portfolio=p.portfolio
LEFT OUTER JOIN S2_UNIVERS i ON i.report_date=@date and s.isin=i.ISIN
LEFT OUTER JOIN S2_RATING_PARAM r ON r.rating=i.iboxx_rating
LEFT OUTER JOIN S2_DEFAULT d ON d.rating=i.iboxx_rating and d.year= (CASE WHEN ROUND(i.years_to_maturity,0) < 15 THEN ROUND(i.years_to_maturity,0) ELSE 15 END)
--proba de dégradation
LEFT OUTER JOIN S2_TRANSITION t1 ON t1.de=i.iboxx_rating and t1.a=i.iboxx_rating and t1.year=1 
LEFT OUTER JOIN S2_TRANSITION t5 ON t5.de=i.iboxx_rating and t5.a=i.iboxx_rating and t5.year=5 
--scr dans 1ans
LEFT OUTER JOIN S2_TRANSITION t1AAA ON t1AAA.year=1 and t1AAA.de=i.iboxx_rating and t1AAA.a='AAA'
LEFT OUTER JOIN S2_RATING_PARAM rAAA ON rAAA.rating='AAA'
LEFT OUTER JOIN S2_TRANSITION t1AA ON t1AA.year=1 and t1AA.de=i.iboxx_rating and t1AA.a='AA'
LEFT OUTER JOIN S2_RATING_PARAM rAA ON rAA.rating='AA'
LEFT OUTER JOIN S2_TRANSITION t1A ON t1A.year=1 and t1A.de=i.iboxx_rating and t1A.a='A'
LEFT OUTER JOIN S2_RATING_PARAM rA ON rA.rating='A'
LEFT OUTER JOIN S2_TRANSITION t1BBB ON t1BBB.year=1 and t1BBB.de=i.iboxx_rating and t1BBB.a='BBB'
LEFT OUTER JOIN S2_RATING_PARAM rBBB ON rBBB.rating='BBB'
LEFT OUTER JOIN S2_TRANSITION t1BB ON t1BB.year=1 and t1BB.de=i.iboxx_rating and t1BB.a='BB'
LEFT OUTER JOIN S2_RATING_PARAM rBB ON rBB.rating='BB'
LEFT OUTER JOIN S2_TRANSITION t1B ON t1B.year=1 and t1B.de=i.iboxx_rating and t1B.a='B'
LEFT OUTER JOIN S2_RATING_PARAM rB ON rB.rating='B'
LEFT OUTER JOIN S2_TRANSITION t1CCC ON t1CCC.year=1 and t1CCC.de=i.iboxx_rating and t1CCC.a='CCC'
LEFT OUTER JOIN S2_RATING_PARAM rCCC ON rCCC.rating='CCC'
LEFT OUTER JOIN S2_TRANSITION t1D ON t1D.year=1 and t1D.de=i.iboxx_rating and t1D.a='D'
LEFT OUTER JOIN S2_RATING_PARAM rD ON rD.rating='D'
LEFT OUTER JOIN S2_TRANSITION t1NULL ON t1NULL.year=1 and t1NULL.de=i.iboxx_rating and t1NULL.a IS NULL
LEFT OUTER JOIN S2_RATING_PARAM rNULL ON rNULL.rating IS NULL
WHERE s.date=@date 
GROUP BY s.date, s.portfolio,r.sensi ,i.Annual_Modified_Duration, i.iboxx_rating, i.sector_level1

/*(SELECT * FROM S2_TRANSITION where year=1 and de='AAA' and a='AAA')
SELECT * FROM S2_RATING_PARAM
SELECT SUM(s.weight*r.scr*r.sensi)/100
SELECT SUM(COALESCE(SALES_RSD,0)) FROM ACT_DATA_FACTSET where isin ='AT0000606306'
*/

SELECT date, portfolio, 
    sum(duration) As duration, 
    sum(sensi) as sensi, 
    sum(life) As life, 
    sum(yield) As yield , 
    sum(scr) as scr, 
    SUM(AAA) as AAA, 
    SUM(AA) as AA, 
    SUM(A) as A, 
    SUM(BBB) as BBB,
    SUM(HY) as HY, 
    SUM(defaut) as defaut,
    SUM(transition_1yr) As transition_1yr,
    SUM(transition_5yr) As transition_5yr,
    SUM(scr_1yr) As scr_1yr  
Into #PORTFOLIO_CARACT
FROM #PORTFOLIO_CARACT_TMP 
GROUP BY date, portfolio

--SELECT * FROm #PORTFOLIO_CARACT 
--SELECT * FROm  S2_PORT_TOTAL
UPDATE S2_PORT_TOTAL 
SET duration= p.duration, 
    sensi= p.sensi, 
    life= p.life, 
    yield= p.yield , 
    scr= p.scr, 
    AAA= p.AAA, 
    AA= p.AA, 
    A= p.A, 
    BBB= p.BBB,
    HY=p.HY, 
    defaut= p.defaut,
    transition_1yr = p.transition_1yr,
    transition_5yr = p.transition_5yr ,
    scr_1yr = p.scr_1yr 
FROM #PORTFOLIO_CARACT p, S2_PORT_TOTAL t where t.date=@date and p.portfolio=t.portfolio


DELETE FROM S2_PORT_COMPO_TEMP where date=@date and portfolio IN (SELECT * FROM #plein)

DROP TABLE #PORTFOLIO_CARACT_TMP
DROP TABLE #PORTFOLIO_CARACT
DROP table #plein

COMMIT TRANSACTION

END TRY
BEGIN CATCH
       select 'CATCH';
       ROLLBACK TRANSACTION
END CATCH;