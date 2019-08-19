-- >test du bon fonctionnement des transparisation:

-- controle sur la "NAV"
--Avant 
select Groupe,sum(Valeur_boursiere + Coupon_Couru) from PTF_FGA 
where DateInventaire = '30/09/2011' and Groupe <> 'OPCVM'
 group by Groupe
  order by Groupe

--Après
SELECT Groupe, SUM(Valeur_Boursiere+Coupon_Couru) from PTF_TRANSPARISE
where DateInventaire = '30/09/2011' and Numero_Niveau = 1
 group by Groupe
order by Groupe

SELECT Groupe, SUM(Valeur_Boursiere+Coupon_Couru) from PTF_TRANSPARISE
where DateInventaire = '30/09/2011' and Numero_Niveau = 2
 group by Groupe
 order by Groupe

 
-- les fonds OPCVM de FGA
-- table temporaire pour faicliter les jointures
select distinct fga.Compte, fga.Isin_Ptf,fga.Libelle_Ptf, an.AN
into #FGA_OPCVM
from  PTF_FGA as fga
left outer join PTF_AN as an on an.Date =   fga.DateInventaire and an.ISIN_PTF = fga.Isin_Ptf
where fga.DateInventaire = '30/09/2011' and fga.Groupe = 'OPCVM'


-- les lignes qui ont été touchées
SELECT pct_det_Niv_0,Isin_proxy_Niv_1,pp.libellé_proxy ,
Isin_origine_Niv_1, fga.Libelle_Ptf,fga.AN,
pct_det_Niv_1,
Isin_proxy_Niv_2,Isin_origine_Niv_2,pct_det_Niv_2,
* from PTF_TRANSPARISE as ptf
left outer join PTF_PARAM_PROXY as pp on pp.code_proxy = Isin_proxy_Niv_1
left outer join #FGA_OPCVM as fga on fga.Isin_Ptf = Isin_origine_Niv_1
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 2
and ( ptf.Isin_proxy_Niv_1 is not null or ptf.Isin_origine_Niv_1 <> '')
order by ptf.Isin_Proxy_Niv_1, ptf.Isin_origine_Niv_1


SELECT sum(pct_det_Niv_0),Isin_origine_Niv_1 from PTF_TRANSPARISE
where DateInventaire = '30/09/2011' and Numero_Niveau = 1 
group by Isin_origine_Niv_1
and ( Isin_proxy_Niv_1 is not null or Isin_origine_Niv_1 is not null)

---------------------------------------------------------------------
-- ce qui reste en fonds OPCVM FGA au Niveau2
SELECT pct_det_Niv_0,Isin_proxy_Niv_1,pp.libellé_proxy ,
Isin_origine_Niv_1, fga.Libelle_Ptf,fga.AN,
pct_det_Niv_1,
Isin_proxy_Niv_2,Isin_origine_Niv_2,pct_det_Niv_2,ptf.isin_titre,* 
from PTF_TRANSPARISE as ptf
left outer join PTF_PARAM_PROXY as pp on pp.code_proxy = Isin_proxy_Niv_1
left outer join #FGA_OPCVM as fga on fga.Isin_Ptf = Isin_origine_Niv_1
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 2
and ptf.pct_det_Niv_2>0
-----------------------------------------TOTAL en fonds FGA 
SELECT SUM(Valeur_Boursiere + Coupon_Couru)
from PTF_TRANSPARISE as ptf
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 2
and ptf.pct_det_Niv_2>0

-- lister les lignes de fonds FGA Niveau2
SELECT pct_det_Niv_0,Isin_proxy_Niv_1,pp.libellé_proxy ,
Isin_origine_Niv_1, 
pct_det_Niv_1,
Isin_proxy_Niv_2,Isin_origine_Niv_2,pct_det_Niv_2,ptf.isin_titre,* 
from PTF_TRANSPARISE as ptf
left outer join PTF_PARAM_PROXY as pp on pp.code_proxy = Isin_proxy_Niv_1
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 2
and ptf.isin_titre in ( select Isin_Ptf from #FGA_OPCVM)

------------------------------------------------------------------
-- ce qui reste en fonds "PROXY" au Niveau2
SELECT pct_det_Niv_0,Isin_proxy_Niv_1,
Isin_origine_Niv_1, pct_det_Niv_1,
Isin_proxy_Niv_2,Isin_origine_Niv_2,pct_det_Niv_2,ptf.isin_titre,* 
from PTF_TRANSPARISE as ptf
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 2
and ptf.isin_titre in ( select isin_titre from PTF_PARAM_PROXY)
and ptf.isin_titre <> 'DE0006289309'

SELECT SUM(Valeur_Boursiere + Coupon_Couru)
from PTF_TRANSPARISE as ptf
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 2
and ptf.isin_titre in ( select isin_titre from PTF_PARAM_PROXY)
and ptf.isin_titre <> 'DE0006289309'
---------------------------------------------------------------------
SELECT pct_det_Niv_0,Isin_proxy_Niv_1,
Isin_origine_Niv_1, pct_det_Niv_1,
Isin_proxy_Niv_2,Isin_origine_Niv_2,pct_det_Niv_2,ptf.isin_titre,* 
from PTF_TRANSPARISE as ptf
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 2
and ( ptf.Isin_proxy_Niv_1 is not null
or ptf.Isin_proxy_Niv_2 is not null )
---------------------------------------------------------------------
-- ce qui reste en fonds OPCVM FGA au Niveau3
SELECT pct_det_Niv_0,Isin_proxy_Niv_1,
Isin_origine_Niv_1, pct_det_Niv_1,
Isin_proxy_Niv_2,pp.libellé_proxy,Isin_origine_Niv_2,fga.Libelle_Ptf,fga.AN,pct_det_Niv_2,
Isin_proxy_Niv_3,Isin_origine_Niv_3,pct_det_Niv_3,
ptf.isin_titre,* 
from PTF_TRANSPARISE_NIV3 as ptf
left outer join PTF_PARAM_PROXY as pp on pp.code_proxy = Isin_proxy_Niv_2
left outer join #FGA_OPCVM as fga on fga.Isin_Ptf = Isin_origine_Niv_2
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 3
and ptf.pct_det_Niv_3>0
-----------------------------------------TOTAL en fonds FGA Niveau 3
SELECT SUM(Valeur_Boursiere + Coupon_Couru)
from PTF_TRANSPARISE_NIV3 as ptf
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 3
and ptf.pct_det_Niv_3>0

-- lister les lignes de fonds FGA Niveau3
SELECT pct_det_Niv_0,Isin_proxy_Niv_1,
Isin_origine_Niv_1, pct_det_Niv_1,
Isin_proxy_Niv_2,pp.libellé_proxy,Isin_origine_Niv_2,pct_det_Niv_2,
Isin_proxy_Niv_3,Isin_origine_Niv_3,pct_det_Niv_3,
ptf.isin_titre,* 
from PTF_TRANSPARISE_NIV3 as ptf
left outer join PTF_PARAM_PROXY as pp on pp.code_proxy = Isin_proxy_Niv_2
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 3
and ptf.isin_titre in ( select Isin_Ptf from #FGA_OPCVM)

------------------------------------------------------------------
-- ce qui reste en fonds "PROXY" au Niveau3
SELECT pct_det_Niv_0,Isin_proxy_Niv_1,
Isin_origine_Niv_1, pct_det_Niv_1,
Isin_proxy_Niv_2,Isin_origine_Niv_2,pct_det_Niv_2,
Isin_proxy_Niv_3,Isin_origine_Niv_3,pct_det_Niv_3,
ptf.isin_titre,* 
from PTF_TRANSPARISE_NIV3 as ptf
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 3
and ptf.isin_titre in ( select isin_titre from PTF_PARAM_PROXY)
and ptf.isin_titre <> 'DE0006289309'

SELECT SUM(Valeur_Boursiere + Coupon_Couru)
from PTF_TRANSPARISE_NIV3 as ptf
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 3
and ptf.isin_titre in ( select isin_titre from PTF_PARAM_PROXY)
and ptf.isin_titre <> 'DE0006289309'

------------------------------------------------------------------
-- ce qui reste en fonds EXTERNES au Niveau3
SELECT SUM(Valeur_boursiere+coupon_couru) as position,
ptf.isin_titre,ptf.libelle_Titre, Pays,Emetteur , Grp_Emetteur, Secteur
from PTF_TRANSPARISE_NIV3 as ptf
where ptf.DateInventaire = '30/09/2011' and ptf.Numero_Niveau = 3
and ptf.Secteur like 'FONDS %'
group by ptf.isin_titre,ptf.libelle_Titre, Pays,Emetteur , Grp_Emetteur,Secteur
order by position desc

SELECT count(*)
from PTF_TRANSPARISE_NIV3 as ptf 
where DateInventaire = '30/09/2011' and Numero_Niveau = 3

SELECT count(*)
from PTF_TRANSPARISE as ptf 
where DateInventaire = '30/09/2011' and Numero_Niveau = 2

SELECT count(*)
from PTF_TRANSPARISE_NIV3 as ptf 
where DateInventaire = '30/09/2011' and Numero_Niveau = 3

