SELECT  DISTINCT
			--p._isin,	
            e._emetteur As code,
            e._NOMEMETTEUR As libelle,
            GR._codegrper As id_groupe,
            GR._libellegrper AS libelle_groupe,

            CASE when s._CODESECTEUR = 'O COVERED' THEN 'O CF BANQ' ELSE s._CODESECTEUR END As 'id_secteur',
            CASE when s._CODESECTEUR = 'O COVERED' THEN (select distinct _LIBELLESECTEUR from com.secteurs where _CODESECTEUR = 'O CF BANQ') ELSE s._LIBELLESECTEUR END As 'libelle_secteur',
 
            CASE when ss._CODESOUSSECTEUR IN ('O COVERED', 'O CF LT2', 'O CF UT2', 'O CF T1', 'O CFB SENI') THEN 'O CFB SENI' 
				 when ss._CODESOUSSECTEUR IN ('O CF PERP', 'O CF SUB', 'O CFA UT2', 'O CFA SENI') THEN 'O CFA SENI'
				 when ss._CODESOUSSECTEUR IN ('O CF SFIN', 'O SFIN LT2', 'O SFIN UT2', 'O CF SFPER') THEN 'O CF SFIN'
				 when ss._CODESOUSSECTEUR IN ('O CNF ALIM', 'O ALIM SUB') THEN 'O CNF ALIM'
				 when ss._CODESOUSSECTEUR IN ('O CNF AUTO', 'O AUTO SUB') THEN 'O CNF AUTO'
				 when ss._CODESOUSSECTEUR IN ('O CNF BSCO', 'O BSCO SUB') THEN 'O CNF BSCO'
				 when ss._CODESOUSSECTEUR IN ('O CNF INDU', 'O INDU SUB') THEN 'O CNF INDU'
				 when ss._CODESOUSSECTEUR IN ('O CNF CHIM', 'O CHIM SUB') THEN 'O CNF CHIM'
				 when ss._CODESOUSSECTEUR IN ('O CNF CONS', 'O CONS SUB') THEN 'O CNF CONS'
				 when ss._CODESOUSSECTEUR IN ('O CNF DIST', 'O DIST SUB') THEN 'O CNF DIST'
				 when ss._CODESOUSSECTEUR IN ('O CNF ENER', 'O ENER SUB') THEN 'O CNF ENER'
				 when ss._CODESOUSSECTEUR IN ('O CNF TECH', 'O TECH SUB') THEN 'O CNF TECH'
				 when ss._CODESOUSSECTEUR IN ('O CNF TELE', 'O TELE SUB') THEN 'O CNF TELE'
				 when ss._CODESOUSSECTEUR IN ('O CNF VOYA', 'O VOYA SUB') THEN 'O CNF VOYA'
				 when ss._CODESOUSSECTEUR IN ('O CNF COLL', 'O COLL SUB') THEN 'O CNF COLL'
				 when ss._CODESOUSSECTEUR IN ('O CNF MEDI', 'O MEDI SUB') THEN 'O CNF MEDI'
				 when ss._CODESOUSSECTEUR IN ('O CNF SANT', 'O SANT SUB') THEN 'O CNF SANT'
				 when ss._CODESOUSSECTEUR IN ('O CNF BASE', 'O BASE SUB') THEN 'O CNF BASE'
			ELSE ss._CODESOUSSECTEUR
			END As id_sous_secteur,

            CASE when ss._CODESOUSSECTEUR IN ('O COVERED', 'O CF LT2', 'O CF UT2', 'O CF T1', 'O CFB SENI') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CFB SENI') 
				 when ss._CODESOUSSECTEUR IN ('O CF PERP','O CF SUB', 'O CFA UT2', 'O CFA SENI') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CFA SENI') 
				 when ss._CODESOUSSECTEUR IN ('O CF SFIN', 'O SFIN LT2', 'O SFIN UT2', 'O CF SFPER') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CF SFIN')
				 when ss._CODESOUSSECTEUR IN ('O CNF ALIM', 'O ALIM SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF ALIM')
				 when ss._CODESOUSSECTEUR IN ('O CNF AUTO', 'O AUTO SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF AUTO')
				 when ss._CODESOUSSECTEUR IN ('O CNF BSCO', 'O BSCO SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF BSCO')
				 when ss._CODESOUSSECTEUR IN ('O CNF INDU', 'O INDU SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF INDU')
				 when ss._CODESOUSSECTEUR IN ('O CNF CHIM', 'O CHIM SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF CHIM')
				 when ss._CODESOUSSECTEUR IN ('O CNF CONS', 'O CONS SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF CONS')
				 when ss._CODESOUSSECTEUR IN ('O CNF DIST', 'O DIST SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF DIST')
				 when ss._CODESOUSSECTEUR IN ('O CNF ENER', 'O ENER SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF ENER')
				 when ss._CODESOUSSECTEUR IN ('O CNF TECH', 'O TECH SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF TECH')
				 when ss._CODESOUSSECTEUR IN ('O CNF TELE', 'O TELE SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF TELE')
				 when ss._CODESOUSSECTEUR IN ('O CNF VOYA', 'O VOYA SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF VOYA')
				 when ss._CODESOUSSECTEUR IN ('O CNF COLL', 'O COLL SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF COLL')
				 when ss._CODESOUSSECTEUR IN ('O CNF MEDI', 'O MEDI SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF MEDI')
				 when ss._CODESOUSSECTEUR IN ('O CNF SANT', 'O SANT SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF SANT')
				 when ss._CODESOUSSECTEUR IN ('O CNF BASE', 'O BASE SUB') THEN (select distinct _LIBELLESOUSSECTEUR from com.soussect where _CODESOUSSECTEUR = 'O CNF BASE')
			ELSE ss._LIBELLESOUSSECTEUR 
			END As libelle_sous_secteur,

            pi._CODEPAYS As id_pays,
            pi._LIBELLEPAYS As libelle_pays, 
            'NA' As interneCT,
			'NA' AS interneLT
			--r._signature As interneLT
--Into #test
FROM com.produit p
right outer join	com.prdtclasspys ps         on p._codeprodui=ps._codeprodui and ps._classification=0
right outer join    com.soussect ss             on ss._CODESOUSSECTEUR=ps._codesoussecteur and  left(ss._CODESOUSSECTEUR,1)<>'A' and  left(ss._CODESOUSSECTEUR,2)<>'NA'
right outer join	com.ssectclass ss_s         on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
right outer join    com.secteurs s              on s._CODESECTEUR=ss_s._codesecteur and s._CODESECTEUR <>'NULL'
right outer join    com.rating r				on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where  r._codeprodui=p._codeprodui and  r._codeagence='INT' )
right outer join    com.emetteur e              on e._emetteur=p._emetteur and e._emetteur NOT IN('DEXIA CRED','EXAFIN','BNPP ARBIT','CDC FINANC')
right outer join    com.grpemetteursratios GR   on  e._grper=GR._codegrper
right outer join    com.pays pi                 on pi._CODEPAYS= gr._codepays
WHERE  
		p._echeance > '31/12/1995' and (left(p._codedutype,1)='O' or p._codedutype in ('CD','BTAN')) and _signature<>''
order by e._emetteur

--remplacé right par left 
--SELECT distinct code FROM #test 
--SELECT * from #test where code IN('CALYON', 'CREDIT LOG', 'DEXIA CRED', 'EXAFIN', 'GOLDMAN SA', 'MORGAN STA', 'S2P', 'VINCI S.A.')
--SELECT code,count(code) from #test  group by code having count(code) >1



