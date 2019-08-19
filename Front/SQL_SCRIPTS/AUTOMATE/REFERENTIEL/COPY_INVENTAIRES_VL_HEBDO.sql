
--------------------------------------------------------------------------------
--  Copie de l inventaire des fonds FGA qui ne sont pas en VL hebdo :
--           Core euro credit 2018
--           Core euro credit 2019
-- FR0000984668	MEDERIC MODERE 4DEC
-- FR0000984643	MEDERIC MEDIAN 4DEC
--------------------------------------------------------------------------------
--------------------------------------------------------------------------------
-- date d inventaire
declare @dateTransparence datetime
set @dateTransparence = '30/01/2015'
-- la liste des fonds dont l inventaire est à recopier 
declare @fonds table(
compte varchar(60)
)

insert into @fonds (compte) VALUES ('6100092') --	FEDERIS CORE EURO CREDIT 2018
insert into @fonds (compte) VALUES ('6100096') --	FEDERIS CORE EURO CREDIT 2019
insert into @fonds (compte) VALUES ('6100103') --	FEDERIS CORE EURO CREDIT 2022
insert into @fonds (compte) VALUES ('6100027') --	FR0000984668	FCP MEDERIC MODERE
insert into @fonds (compte) VALUES ('6100028') --	FR0000984643	FCP MEDERIC MEDIAN 
--insert into @fonds (compte) VALUES ('6100035') --   6100035	FCP FEDERIS ISR TRESORERIE

--insert into @fonds (compte) VALUES ('6100016') --   6100016	FCPE NAPHTACHIMIE EPARGNE
 

---------------------------------------------------------------------------------

---------------------------------------------------------------------------------
-- une requete pour lister les fcp FGA qui ne sont pas présent en inventaire et ni en proxy
 
--select * from PTF_FGA where Groupe <> 'OPCVM' and DateInventaire = @dateTransparence
--and Emetteur ='FEDERIS GA' and code_titre not in ( 
--select distinct Isin_ptf from PTF_FGA where Groupe = 'OPCVM'  and DateInventaire =@dateTransparence)
--and 
--code_titre not in ( select isin_titre from ptf_param_proxy where Date =@dateTransparence and source ='OPCVM')
---------------------------------------------------------------------------------

-- la derniere date d inventaire dispo avec comme contrainte :
--               date demandée - 1.5 mois <  d  < date demandée
select a.compte,max (Date) as Date
into #Compte_Dates
from PTF_AN as a 
left outer join PTF_FGA as f on f.compte = a.compte and f.Dateinventaire = a.date 
where a.compte in ( select compte from @fonds )
and a.date  >= DATEADD ( month, -1.5, @dateTransparence)
and a.date <=@dateTransparence
and f.Compte is not null
group by a.compte


select i.*
into #INVENTAIRE
from PTF_FGA as i
left outer join #Compte_Dates as d on d.compte = i.compte
where i.Groupe = 'OPCVM' 
and d.compte is not null
and i.dateinventaire = d.date
and i.Dateinventaire <> @dateTransparence


update #INVENTAIRE
set DateInventaire =@dateTransparence


select a.*
into #ACTIF_NET
from PTF_AN as a
left outer join #Compte_Dates as d on d.compte = a.compte
where 
d.compte is not null
and a.date = d.date
and a.date <> @dateTransparence

update #ACTIF_NET
set Date =@dateTransparence


insert into PTF_AN 
select * from #ACTIF_NET

insert into PTF_FGA 
select * from #INVENTAIRE



drop table #Compte_Dates 
drop table #INVENTAIRE
drop table #ACTIF_NET