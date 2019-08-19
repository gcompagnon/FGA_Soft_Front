----------------------------------------------------------
-- Retourne pour tous les parts de fonds actifs (table com.produit avec emetteur like FEDERIS%
-- et le compte associé (groupe 'OP') :
-- les données venant du dépositaire: l actif net avec compte, libellé et isin, ... pour chaque NOURRICIER
-- les données calculée d'Omega : l actif net et nb de parts du fonds MAITRE
----------------------------------------------------------
-- Paramètres: @date_start et @date_end (par défaut:aujourd hui): entre les 2 dates ,  pour lister les Actifs Nets de Parts
--             @niveau_Controle: 1 ou 2(défaut)  à 1: rapprochement MAITRE/NOURRICIER avec detection des ecarts AN et nbParts, à 2 : liste des VLs et actifs nets diffusés et comptabilisés
--             @seuil_ecart_parts et seuil_ecart_AN : par défaut 0.5% , écart maximum entre les données Custody et le BackOffice afin d avoir un NOK en controle (NA si les données ne sont pas disponibles, par exemple, pas de VL comptable un jour)
--             @result_type de 1 à 3 : donne les differentes sorties : 1 pour les lignes a investiguer, 2 pour toutes les lignes 3 pour la configuration Maitre/nourricier
declare @niveau_Controle tinyint
set @niveau_Controle = 2

declare @date_end datetime
set @date_end = GETDATE()

declare @date_start datetime
set @date_start = DATEADD(day, -30, @date_end)

-- Pour le parametrage du controle de niveau 1
declare @seuil_ecart_parts float
set @seuil_ecart_parts = 0.005

declare @seuil_ecart_AN float
set @seuil_ecart_AN = 0.005

declare @result_type tinyint
set @result_type = 1

-- faire une table temporaire pour avoir le lien entre le compte du fonds Maitre et les code ISIN des fonds nourriciers
-- TODO faire la même table avec le _codeprodui et _username de fcp.cpartfcp qui doivent contenir les isin des fds nourriciers
-- TODO: faire un controle qui exploite la base FGA_Ref et OMEGA afin d avoir cette table en live
create table #MASTER_FEEDER ( code varchar(15) primary key, account char(10) not null,typePart char(1) null, quotityPart tinyint not null, logUser char(3), logDate datetime )

insert into #MASTER_FEEDER VALUES ('CIPCRFOACT','1010011',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0007012182','6100001',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0007021936','6100002',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0007045950','6100004',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0011208081','6100004',NULL,1,'TQA','08/03/2012')
-- Dans omega: le fonds maitre n est pas renseigne en parts C ou D pour 6100012
--insert into ##MASTER_FEEDER VALUES ('6079','6100012', 'C',1,'TQA','20/02/2012')
--insert into ##MASTER_FEEDER VALUES ('QS0011122163','6100012', 'D',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('6079','6100012',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0011122163','6100012',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('0411','6100015',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('1130','6100016',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('0410','6100017',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0010250571','6100018',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010258251','6100018',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0000989303','6100019',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0000989287','6100020',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0000989295','6100021',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0000989279','6100022',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0000989261','6100023',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0007057674','6100024',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0007454889','6100025',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0007022801','6100026',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0000984668','6100027',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0000984643','6100028',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0000984650','6100029',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0007078480','6100030',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010263806','6100030',NULL,100,'TQA','20/02/2012') -- ce fonds a une particularité: le nb de parts est à diviser par 100
insert into #MASTER_FEEDER VALUES ('FR0007079330','6100031',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010030031','6100033',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010027458','6100034',NULL,1,NULL,NULL)
-- Dans omega: le fonds maitre n est pas renseigne en parts I ou R pour 6100035
--insert into ##MASTER_FEEDER VALUES  ('FR0010250597','6100035','I',1,NULL,NULL)
--insert into ##MASTER_FEEDER VALUES ('FR0010256156','6100035', 'R',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0010250597','6100035',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010256156','6100035',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('8506','6100037','F',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0011148895','6100037','E',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('8515','6100043','F',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0011148887','6100043','E',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('8517','6100044','F',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0011147285','6100044','E',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('8525','6100047','C',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('6111','6100054','C',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0011122155','6100054','D',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('8516','6100059','F',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0002105PD3','6100059','E',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0007044532','6100060',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('8681','6100061',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0007022793','6100062',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0007022967','6100063',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010140244','6100064',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010140251','6100065',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010193235','6100066',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010236372','6100068',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0010250720','6100069',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0010251082','6100070',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010346148','6100072',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0010404731','6100073',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('990000094039','6100074',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0010444836','6100075',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0010616979','6100076','I',1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010622662','6100076','R',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0010738179','6100077',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0011128020','6100078',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0011128038','6100079',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0011128046','6100080',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('QS0011128053','6100081',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0010775437','6100082',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010790030','6100083',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010828673','6100084',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010827444','6100085',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010822122','6100086',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0011049709','6100088',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0011105279','6100089',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0011133438','6100090',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0011152925','6100091',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0007031646','AVCAN',NULL,1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0000098253','AVEPAR','D',1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0010777912','AVEPAR','C',1,'TQA','20/02/2012')
insert into #MASTER_FEEDER VALUES ('FR0007031653','AVEURO',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0007074166','AVEUROPE',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0011234079','6100092',NULL,1,NULL,NULL)
insert into #MASTER_FEEDER VALUES ('FR0011234087','6100092',NULL,1,NULL,NULL)


-----------------------------------------------------------------------------------------------------
---- Affichage des comptes qui n ont pas de lien avec un nourricier
--select c.* from fcp.cpartfcp c 
--left outer join Fcp.CONSGRPE cg on cg._compte=c._compte 
--where cg._CODEGROUPE = 'OP'
--and c._compte not in (select account from #MASTER_FEEDER)
--and _libellecli not like '%plus utili%'
--order by c._compte
---- Affichage des fcp Federis sans lien avec un maitre
--select * from  com.produit as p 
--where _emetteur like 'FEDERIS%' and
--p._codeprodui not in (select code from #MASTER_FEEDER)
--and _libelle1prod not like '%plus utili%'
--order by _codeprodui
-------------------------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------
-- CONTROLE NIVEAU 2
-- liste de toutes les VL diffusées par le custody sur chaque part (fonds nourricier)
--------------------------------------------------------------------------------------
IF @niveau_Controle = 2
BEGIN

	select 
	         convert(varchar, ph._date, 103) as C01_date,
			 ph._codeprodui  as C02_code, 
			 p._codedutype  as C03_type,
			 p._libelle1prod as C04_libelle,			 
			 ph._coursclose as C05_VL, 
			 ph._actifnet as C06_ActifNet, 
			 isnull(n.typePart,'') as C07_typeParts, 
			 ph._nbrparts/isnull(n.quotityPart,1) as C08_nbParts,
			 isnull(n.account,'NA') as B01_CompteFondsMaitre,
			 isnull(c._LIBELLECLI,'A renseigner dans la base FGA_Ref') as B02_LibelleFondsMaitre	         
	from          
		com.produit p          
	left outer join com.prixhist as ph on ph._codeprodui =p._codeprodui and ph._nbrparts<> 0
	left outer join #MASTER_FEEDER as n on n.code = p._codeprodui 
	left outer join fcp.cpartfcp as c on c._compte=n.account
	where 
	ph._date <=@date_end
	and ph._date >=@date_start
	and p._emetteur like 'FEDERIS%'
	order by C03_type,C04_libelle, ph._date

END


--------------------------------------------------------------------------------------
-- CONTROLE NIVEAU 1
-- Pour les comptes : faire la somme et mettre en parallèle avec le calcul Omega
--------------------------------------------------------------------------------------
IF @niveau_Controle = 1
BEGIN

	-- ETAPE 1: 
	-- liste des parts (fonds nourricier) sans lien avec un fonds maitre (compte dans Omega) => il faut renseigner la table #MASTER_FEEDER
	select 
			 ph._codeprodui  as C01_code, 
			 p._codedutype  as C02_type,
			 p._libelle1prod as C03_libelle,
			 ph._date as C04_date,         
			 ph._coursclose as C05_VL, 
			 ph._actifnet as C06_ActifNet, 
			 ph._nbrparts as C07_nbparts
	into #NO_COMPTE_MAITRE         
	from          
		com.produit p          
	left outer join com.prixhist as ph on ph._codeprodui =p._codeprodui and ph._nbrparts<> 0
	where 
	ph._date <=@date_end
	and ph._date >=@date_start
	and p._emetteur like 'FEDERIS%'
	and p._codeprodui not in (select m.code from #MASTER_FEEDER as m)
	--order by C02_type,C03_libelle, ph._date
	  
	-- ETAPE 2: 
	-- lister pour compte et date: les nourriciers afin d obtenir l'actif net et le nb parts consolidés en vue de comparaison BackOffice/Custody
	-- calcul pour l actif net sans distinguo pour les types de parts
	select          
			 ph._date as C01_date,
			 m.account  as B01_CompteFondsMaitre,
			 c._LIBELLECLI as B02_LibelleFondsMaitre,   
			 c._depositaire as B03_CustodyFondsMaitre,
			 courtier._libellecourt as B04_CustodyFondsMaitre,
			 count(p._codeprodui) as C02_nbNourriciers,
			 isnull(v._actif,0) as B04_ActifNetCalculOmega,
			 SUM(ph._actifnet) as C03_ActifNetTotalNourriciers
	into #CONTROLE_NIVEAU1_AN         
	from              
		com.produit p          
	left outer join com.prixhist as ph on ph._codeprodui =p._codeprodui and ph._nbrparts<> 0
	left outer join #MASTER_FEEDER as m on m.code = p._codeprodui 
	left outer join fcp.vlrstion as v on v._actif <>0 and v._DATEOPERATION=ph._DATE and  v._compte = m.account
	left outer join fcp.cpartfcp as c on c._compte=m.account
	left outer join com.courtier as courtier on courtier._codecourt=c._depositaire
	where 
	ph._date <=@date_end
	and ph._date >=@date_start
	and p._codeprodui not in(select distinct C01_code from #NO_COMPTE_MAITRE)
	and p._emetteur like 'FEDERIS%'
	group by m.account,ph._date,v._actif, c._LIBELLECLI,c._depositaire,courtier._libellecourt

	-- calcul pour le nb de parts en distinguant le type de parts
	-- cas particulier : le nb de parts de FR0010263806 (dont le maitre est 6100030) est divisé par 100
	
	select          
			 ph._date as C01_date,
			 m.account  as B01_CompteFondsMaitre,
			 c._LIBELLECLI as B02_LibelleFondsMaitre,   
			 c._depositaire as B03_CustodyFondsMaitre,
			 courtier._libellecourt as B04_CustodyFondsMaitre,
			 isnull(m.typePart,'') as C04_typeParts,
			 case
			 when m.typePart IN ('D', 'R','F') then v._NBREPARTSD 
			 when m.typePart IN ('C', 'I','E') then v._NBREPARTSC
			 else v._NBREPARTS 
			 end as B05_nbpartsMaitre,			 
			 SUM(ph._nbrparts/m.quotityPart) as C04_nbpartsNourriciers
			 --isnull(v._NBREPARTS,0)as B03_nbpartsTotalMaitre,
			 --isnull(v._NBREPARTSD,0)as B04_nbpartsDistribOmega,
			 --isnull(v._NBREPARTSC,0)as B05_nbpartsCapitaliseeOmega,               
	into #CONTROLE_NIVEAU1_PARTS         
	from              
		com.produit p          
	left outer join com.prixhist as ph on ph._codeprodui =p._codeprodui and ph._nbrparts<> 0
	left outer join #MASTER_FEEDER as m on m.code = p._codeprodui 
	left outer join fcp.vlrstion as v on v._actif <>0 and v._DATEOPERATION=ph._DATE and  v._compte = m.account
	left outer join fcp.cpartfcp as c on c._compte=m.account
	left outer join com.courtier as courtier on courtier._codecourt=c._depositaire
	where 
	ph._date <=@date_end
	and ph._date >=@date_start
	and p._codeprodui not in(select distinct C01_code from #NO_COMPTE_MAITRE)
	and p._emetteur like 'FEDERIS%'
	group by m.account,ph._date, m.typePart,v._NBREPARTS, v._NBREPARTSD , v._NBREPARTSC ,c._LIBELLECLI,c._depositaire,courtier._libellecourt

	-- ETAPE 3: 
	-- table des resultats
	create table #CONTROLE_NIVEAU1 ( C01_date char(10) not null, B01_CompteFondsMaitre char(15) not null,
									 B02_LibelleFondsMaitre varchar(120) null, B03_CustodyFondsMaitre char(5), B04_CustodyFondsMaitre varchar(100),
									 C02_nbNourriciers int, B04_ActifNetCalculOmega float null,
									 C03_ActifNetTotalNourriciers float null, C04_typeParts char(1),
									 B05_nbpartsMaitre int null, C04_nbpartsNourriciers int,
									 ControlAN char(3) null, ControlParts char(3) null)

	-- ajouter les lignes ou le compte du fonds maitre n est pas renseigne dans la table Referentiel
	insert into #CONTROLE_NIVEAU1 
	select 
			 convert(varchar, n.C04_date, 103) as C01_date,
			 'ZZ_'+n.C01_code as B01_CompteFondsMaitre,
			 'Compte Non dispo pour '+n.C03_libelle as B02_LibelleFondsMaitre,
			 '' as B03_CustodyFondsMaitre,
			 '' as B04_CustodyFondsMaitre,
			 1 as C02_nbNourriciers,
			 0 as B04_ActifNetCalculOmega,
			 n.C06_ActifNet as C03_ActifNetTotalNourriciers,
			 '' as C04_typeParts,
			 0 as B05_nbpartsMaitre,
			 n.C07_nbparts as C04_nbpartsNourriciers,         
			 'NA' as ControlAN,
			 'NA' as ControlParts                 
	from #NO_COMPTE_MAITRE as n 
	order by B01_CompteFondsMaitre, n.C04_date

	-- ETAPE 4: 
	-- Retourner le controle avec 2 colonnes donnant la coherence entre l actif net et le nb de parts Custody/Back-office
	insert into #CONTROLE_NIVEAU1
	select 
		convert(varchar, c1.C01_date, 103) as C01_date,c1.B01_CompteFondsMaitre,c1.B02_LibelleFondsMaitre,c1.B03_CustodyFondsMaitre,c1.B04_CustodyFondsMaitre,c1.C02_nbNourriciers,
		c1.B04_ActifNetCalculOmega,c1.C03_ActifNetTotalNourriciers,
		c2.C04_typeParts,
		isnull(c2.B05_nbpartsMaitre,0),c2.C04_nbpartsNourriciers,
		case 
		when B04_ActifNetCalculOmega is null or B04_ActifNetCalculOmega =0 then 'NA'
		when ( ABS(B04_ActifNetCalculOmega-C03_ActifNetTotalNourriciers)/B04_ActifNetCalculOmega )>=@seuil_ecart_AN then 'NOK'
		else ''
		end as ControlAN,
		case 
		when c2.B05_nbpartsMaitre is null or c2.B05_nbpartsMaitre =0 then 'NA'
		when ( ABS(c2.B05_nbpartsMaitre-c2.C04_nbpartsNourriciers)/c2.B05_nbpartsMaitre )>=@seuil_ecart_parts then 'NOK'
		else ''
		end as ControlParts
	from #CONTROLE_NIVEAU1_PARTS as c2,
		 #CONTROLE_NIVEAU1_AN as c1 
	where c1.C01_date = c2.C01_date and c1.B01_CompteFondsMaitre = c2.B01_CompteFondsMaitre 
	order by c1.B01_CompteFondsMaitre, c1.C01_date

	-- ETAPE 5:
	-- Affichage des résultats
	IF @result_type = 1
	   select ControlParts,ControlAN,C01_date,B01_CompteFondsMaitre,B02_LibelleFondsMaitre,B03_CustodyFondsMaitre,C02_nbNourriciers,B04_ActifNetCalculOmega,C03_ActifNetTotalNourriciers,C04_typeParts,B05_nbpartsMaitre,C04_nbpartsNourriciers from #CONTROLE_NIVEAU1 where ControlParts <> '' OR ControlAN <> ''
	ELSE 
	   BEGIN
	      IF @result_type = 2	      
	          select * from #CONTROLE_NIVEAU1
	      ELSE
	         BEGIN
	             IF @result_type = 3
	                -------------------------------------------------------------------------------------------------------
                    ---- Affichage des liens fonds maitre (compte) avec son ou ses fonds nourriciers (produit FGA)
                    select m.account, c._LIBELLECLI, m.code, p._libelle1prod, isnull(m.typePart,'')as typePart,m.quotityPart, isnull(m.loguser,'')as loguser 
					from #MASTER_FEEDER as m
                    left outer join com.produit p on p._codeprodui = m.code
                    left outer join fcp.cpartfcp as c on c._compte=m.account
                    order by m.account
	         END
	   END
	
    drop table #CONTROLE_NIVEAU1_PARTS
    drop table #CONTROLE_NIVEAU1_AN
    drop table #CONTROLE_NIVEAU1
    drop table #NO_COMPTE_MAITRE
END

drop table #MASTER_FEEDER