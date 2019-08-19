--------------------------------------------------------------
-- sortie des titres en direct sur les fonds FGA
-- en quantité et prix/valorisation

-- (demande equipe Strategie 12/2014)
--------------------------------------------------------------
--App.omg_fnt_FEPoOuGrpe : GET Position par Groupe 
--fonction basée sur la fonction App.omg_fnt_PoOuGrpe mais retourne le compte et le libellé aussi

-- List Of Parameters
-- @ai_CODEGROUPE Code de Groupe
-- @ai_DateDate de calcul format dd/mm/yyyy.
-- @ai_NbreJourArriere Nombre de jours arrière.
-- @ai_ClotOuv 0-Cours clôture 1-Cours ouverture.
-- @ai_Bfcp 0-Version gestion de portefeuille 1-Version gestion de fonds commun de placement.
-- @ai_DateOpVal 0-Date opération 1-Date Valeur.
--------------------------------------------------------------
use omega
--Recherche de la derniere date d inventaire
declare @compteReference varchar(10)
set @compteReference = '6100001' -- FCP FEDERIS ACTIONS FRANCE

declare @dateDemandee as char(10)
set @dateDemandee = '01/12/2014'
-- Si la date demandee n existe pas, prendre la dernier disponible
declare @parametreDateForcage as  datetime
set @parametreDateForcage = (
select max(v._dateoperation) from fcp.valoprdt v where v._compte=@compteReference and v._dateoperation >'25/12/2010'
and v._dateoperation <= @dateDemandee
)


declare @parametreDate as char(10)
set @parametreDate = CONVERT(char(10),@parametreDateForcage,103) 


-- les devise / forex : la derniere disponible
select com.* 
into #devises
from Com.DEVHIST com  
inner join ( select _contredevise as devise ,max(_date) as DerniereDate  from Com.DEVHIST where _date <= @parametreDate group by _contredevise) tmp on tmp.derniereDate = com._date and com._contredevise=tmp.devise

-- recherche les comptes avec leur VLs, soit l'officiel, soit calculé
select v._compte as 'COMPTE','OPCVM ' as 'TYPE', 'XX0000000000' as 'ISIN',null as 'ISIN_PART2', c._LIBELLECLI as 'LIBELLE'
,ph._ACTIFNET as 'OFFICIEL',
v._ACTIF
into #comptes
from fcp.vlrstion v
left outer join Fcp.CONSGRPE cg on cg._compte=v._compte
left outer join fcp.cpartfcp c on  c._compte=v._compte 
left outer join com.prixhist ph on ph._codeprodui = c._username and  ph._date= v._DATEOPERATION
where
         v._DATEOPERATION=@parametreDate
And   v._actif <> 0
And  cg._codegroupe = 'OP' -- les groupes OPCVM

--and v._compte in ( select _compte from Fcp.CONSGRPE where _codegroupe in ( 'HC', 'NR', 'AK', 'RV','TL'   ))

select 
convert(char(10),pos._datederniercours,103) as _datederniercours,
pos._compte,
pos._libellecli,
--left(p._codedutype,3) as _codedutype,
p._codedutype,
p._codereuter as TICKER ,
pos._codeprodui,
pos._isin,
pos._libelle1prod,
p._sousjacent,
pos._position,
pos._prixmarche,
pos._valorisation,
pos._codedevise,
case pos._codedevise
when 'EUR' then 1
else d._coursclose
end as 'forexEURXXX',
case pos._codedevise
when 'EUR' then @parametreDate
else d._date 
end as 'dateForex',
isnull(c.OFFICIEL, c._ACTIF) as 'ActifNet'
into #positions
from App.omg_fnt_FEPoOuGrpe('OP',@parametreDate,50, 0, 1, 0) pos
left outer join #comptes as c on c.compte = pos._compte
left outer join #devises as d on d._contredevise = pos._codedevise
left outer join  com.produit p on p._codeprodui = pos._codeprodui and p._deviseactif = pos._codedevise
where pos._compte in (select compte from #comptes)


select _compte,_libellecli ,
sum(_valorisation/forexEURXXX) as Total
into #Total
from #positions
group by _compte,_libellecli


select _CODEDUTYPE, max (_NBRMODIFICATION) as nb
into #Types
from com.TTYPEPROD 
where _LIBELLE1TYPE not like '%NE PLUS UTILISER%'
group by _CODEDUTYPE


select
p._datederniercours,p._compte,p._libellecli,
t2._LIBELLE1TYPE,
p.TICKER,
p._codeprodui,p._isin,p._libelle1prod,
isnull(ssjacent._codereuter,'') as 'sousjacent_TICKER',
isnull(ssjacent._codeprodui,'') as 'sousjacent_codeprodui',
isnull(ssjacent._libelle1prod,'') as 'sousjacent_libelle',
p._position,p._prixmarche,p._valorisation,p._codedevise,p.forexEURXXX,p.dateForex,p.ActifNet,
t.Total,
_valorisation/forexEURXXX/t.Total as Poids
from #positions as p
left outer join #Total as t on t._compte = p._compte
left outer join #types as typ on typ._CODEDUTYPE = p._codedutype
left outer join com.TTYPEPROD as t2 on t2._CODEDUTYPE = p._codedutype and t2._NBRMODIFICATION = typ.nb
left outer join  com.produit as ssjacent on ssjacent._codeprodui = p._sousjacent
order by _compte desc


drop table #positions
drop table #comptes
drop table #devises
drop table #total
drop table #types

