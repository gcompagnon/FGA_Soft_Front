--------------------------------------------------------------
-- sortie des titres en direct sur des fonds Actions
-- Calcul de la pondération de chaque titre en EUR
-- (demande rodolphe 11/2013)
-- @DATASORE OMEGA
--------------------------------------------------------------
use omega
--Recherche de la derniere date d inventaire
declare @compteReference varchar(10)
set @compteReference = '6100001' -- FCP FEDERIS ACTIONS FRANCE

declare @dateDemandee as char(10)
set @dateDemandee = '28/11/2014'
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
And v._compte in ( '6100001','6100002','6100004', '6100024', '6100026','6100030','6100033',
'6100061',
'6100062',   
'6100063',
'6100066',
'6100094',
'AVEURO',
'AVEUROPE')
--and v._compte in ( select _compte from Fcp.CONSGRPE where _codegroupe in ( 'HC', 'NR', 'AK', 'RV','TL'   ))

select 
convert(char(10),pos._datederniercours,103) as _datederniercours,
pos._compte,
pos._libellecli,
left(p._codedutype,3) as _codedutype,
p._codereuter as TICKER ,
pos._codeprodui,
pos._isin,
pos._libelle1prod,
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
and p._codedutype like 'A%'
and p._codedutype <> 'AFRET'

select _compte,_libellecli ,
sum(_valorisation/forexEURXXX) as Total
into #Total
from #positions
group by _compte,_libellecli

select p.* ,t.total,
_valorisation/forexEURXXX/t.Total as Poids
from #positions as p
left outer join #Total as t on t._compte = p._compte
order by Poids desc

drop table #positions
drop table #comptes
drop table #devises
drop table #total

--select * from Fcp.CONSGRPE c where _compte= '6100093   '

--select * from fcp.cpartfcp  where _libellecli like '%ENTREP%'
--select * from fcp.GRPEDEFT 
--where _codegroupe in 
--(select _codegroupe from Fcp.CONSGRPE c where _compte= '6100093   ')

