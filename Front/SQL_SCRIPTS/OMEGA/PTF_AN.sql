--Récupère l'actif net des mandats dans OMEGA 
--pour mettre le resultat dans PTF_AN


use omega


declare @dateInventaire as datetime
set  @dateInventaire = '31/12/2014'
--set  @dateInventaire = '***'

-- table temporaire contenant les Actifs nets
CREATE TABLE #PTF_AN(
	[compte] [varchar](12) NOT NULL,
	[ISIN_Ptf] [varchar](12) NULL,
	[Libelle_Ptf] [varchar](60) NULL,
	[date] [datetime] NOT NULL,
	[AN] [float] NULL,
	[TYPE] [varchar](10) NULL,
-- CONSTRAINT [pk_PTF_AN] PRIMARY KEY CLUSTERED 
--(
--	[compte] ASC,
--	[date] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


-- LISTE DES COMPTES en OPCVM et MANDATS
create table #COMPTES (Compte varchar(12), TYPE varchar(6), ISIN varchar(12) null, ISIN_PART2 varchar(12) null, LIBELLE varchar(60) NULL )

insert into #COMPTES
select v._compte as 'COMPTE','OPCVM ' as 'TYPE', 'XX0000000000' as 'ISIN',null as 'ISIN_PART2', c._LIBELLECLI as 'LIBELLE'
from fcp.vlrstion v
left outer join Fcp.CONSGRPE cg on cg._compte=v._compte
left outer join fcp.cpartfcp c on  c._compte=v._compte 
where
         v._DATEOPERATION=@dateInventaire
And   v._actif <> 0
And  cg._codegroupe = 'OP' -- les groupes OPCVM

insert into #COMPTES
select v._compte as 'COMPTE','MANDAT' as 'TYPE', null as 'ISIN',null as 'ISIN_PART2', c._LIBELLECLI as 'LIBELLE'
from fcp.vlrstion v
left outer join fcp.cpartfcp c on  c._compte=v._compte 
where
         v._DATEOPERATION=@dateInventaire	 
And   v._actif <> 0
And v._compte not in (select Compte from #COMPTES where Type ='OPCVM ')

-- MAJ de l isin pour les OPCVM
-- pas de multiparts
update #COMPTES
set ISIN = c._username
from #COMPTES as v
left outer join fcp.cpartfcp c on c._compte=v.Compte
where v.TYPE = 'OPCVM' and isnull(c._multiparts,0) = 0

-- CAS PARTICULIER multiparts non configuré dans omega
--FCP FEDERIS OBLIGATION EURO                       
update #COMPTES set ISIN_PART2 = 'FR0010258251' 
from #COMPTES as v where v.compte ='6100018' and ISIN = 'FR0010250571' 

--FCP FEDERIS OBLIGATIONS ISR                      
update #COMPTES set ISIN_PART2 = 'FR0010616979' 
from #COMPTES as v where v.compte ='6100076' and ISIN = 'FR0010622662' 

--FEDERIS CORE EURO CREDIT 2018                      
update #COMPTES set ISIN_PART2 = 'FR0011234079' 
from #COMPTES as v where v.compte ='6100092' and ISIN = 'FR0011234087' 

--FCP FEDERIS EURO ACTIONS                     
update #COMPTES set ISIN_PART2 = 'FR0010263806' 
from #COMPTES as v where v.compte ='6100030' and ISIN = 'FR0007078480' 

--   FCPE SOREA OBLIGATIONS
update #COMPTES set ISIN_PART2 = 'QS0011148895' 
from #COMPTES as v where v.compte ='6100037' and ISIN = '8506' 

--   FCPE SOREA ACTIONS ETHIQUES ET SOLIDAIRES 
update #COMPTES set ISIN_PART2 = 'QS0011148887' 
from #COMPTES as v where v.compte ='6100043' and ISIN = '8515' 

--   FCPE SOREA ACTIONS EURO  
update #COMPTES set ISIN_PART2 = 'QS0011147285' 
from #COMPTES as v where v.compte ='6100044' and ISIN = '8517' 

--   FCPE SOREA CROISSANCE 
update #COMPTES set ISIN_PART2 = 'QS0002105PD3' 
from #COMPTES as v where v.compte ='6100059' and ISIN = '8516' 

--   FCPE NAPHTACHIMIE ACTIONS
update #COMPTES set ISIN_PART2 = 'QS0011122163' 
from #COMPTES as v where v.compte ='6100012' and ISIN = '6079' 
   	        	

-- en multiparts: prendre le code ISIN de la part principale
update #COMPTES
set ISIN = m._codeProdui
from #COMPTES as v
left outer join fcp.cpartfcp c on c._compte=v.Compte
left outer join fcp.multiparts as m on m._Compte = v.Compte and ( _TypePart = 1 or _nomPart = 'M' or _nomPart = 'D' or _nomPart = 'PART M' or _nomPart = 'PART F')
where v.TYPE = 'OPCVM' and isnull(c._multiparts,0) <> 0

-- FIN de LA LISTE DES COMPTES



-- POUR LES MANDATS : prendre la valo dans fcp.vlrstion
insert into #PTF_AN
select distinct
         v._compte as compte,
         null as ISIN_Ptf,  -- pas d isin sur les mandats
         c._LIBELLECLI as Libelle_Ptf,
         v._DATEOPERATION as date,
         v._actif as AN,
         'MANDAT' as TYPE
from fcp.vlrstion v
left outer join fcp.cpartfcp c on c._compte=v._compte
left outer join Fcp.CONSGRPE cg on cg._compte=v._compte
where
         v._DATEOPERATION=@dateInventaire
And   v._actif <> 0
And  v._compte in (select Compte from #COMPTES where Type ='MANDAT')  -- les groupes Mandats


-- POUR LES OPCVM : prendre la valo dans com.prixhist

-- cas des fonds non multiparts
insert into #PTF_AN
select
         v.Compte as compte,
         v.ISIN as ISIN_Ptf,
         c._LIBELLECLI as Libelle_Ptf,
         ph._date as date,
         ph._ACTIFNET as AN,
         'MONOPART' as TYPE
from #COMPTES as v
left outer join fcp.cpartfcp c on c._compte=v.Compte
left outer join com.prixhist ph on ph._codeprodui = c._username
where
Type ='OPCVM'
And v.ISIN_PART2 is null
And ph._date=@dateInventaire
And   isnull(c._multiparts,0) = 0


-- cas des fonds multiparts: 1 compte correspond à plusieurs ISIN (et donc, plusieurs Actifs nets sont calculés)
-- On va faire la somme des actifs nets pour 1 compte
select m._Compte as Compte,comptes.ISIN,ph._date as Date, sum(ph._actifnet) as ActifNet
into #ACTIFNET_MULTIPARTS
from fcp.multiparts as m
left outer join com.prixhist as ph on ph._codeprodui = m._codeprodui 
left outer join #COMPTES as comptes on comptes.Compte = m._compte 
where 
comptes.Type ='OPCVM'
and ph._date = @dateInventaire
group by m._Compte,comptes.ISIN,ph._date

-- pour les "faux multiparts"
insert into #ACTIFNET_MULTIPARTS
select comptes.Compte as Compte,comptes.ISIN,ph._date as Date, sum(ph._actifnet) as ActifNet
from com.prixhist as ph 
left outer join #COMPTES as comptes on comptes.Type ='OPCVM' And comptes.ISIN_PART2 is not null
where 
( ph._codeprodui = ISIN or ph._codeprodui = ISIN_PART2)
and ph._date = @dateInventaire
group by comptes.Compte,comptes.ISIN,ph._date

insert into #PTF_AN
select
         v.Compte as compte,
         v.ISIN as ISIN_Ptf,
         c._LIBELLECLI as Libelle_Ptf,
         v.date as date,         
         v.ActifNet as AN,
         'MULTIPART' as TYPE
from #ACTIFNET_MULTIPARTS as v
left outer join fcp.cpartfcp c on c._compte=v.Compte


--select * from fcp.multiparts
--select * from fcp.valoParts
--select top 100 * from com.prixhist where  _date = '31/01/2013' and _codeprodui in (
--select _codeprodui from fcp.multiparts
--) 
--select * from #COMPTES

--select _ACTIFNET/_NBRPARTS,* from com.prixhist ph where ph._codeprodui in (
--'FR0011319656',
--'FR0011334408' ) and
--ph._date='31/01/2013'


-- controle entre les valorisations Omega et les actifs validés
--select v._actif,a.AN,a.* from #PTF_AN as a
--left outer join fcp.vlrstion v on v._compte  = a.compte and _DATEOPERATION = a.date
--where ABS(v._actif-a.AN) > 0.02*ABS(a.AN)/100


-- sortie
select * from #PTF_AN


drop table #COMPTES
drop table #ACTIFNET_MULTIPARTS

--alter table #PTF_AN drop constraint pk_PTF_AN
drop table #PTF_AN
