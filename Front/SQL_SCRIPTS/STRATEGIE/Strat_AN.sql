Create table #ValeurActifNet (
			compte varchar(12),
			actifnet float
			)

insert into #ValeurActifNet
select compte, sum(valeur_boursiere+coupon_couru) from ptf_fga where dateinventaire = '01-01-2099' group by compte

CREATE TABLE #AN(
	[compte] [varchar](12) NOT NULL,
	[ISIN_Ptf] [varchar](12) NULL,
	[Libelle_Ptf] [varchar](60) NULL,
	[date] [datetime] NOT NULL,
	[AN] [float] NULL
	)
Insert into #AN
select distinct v.compte,p.isin_ptf,p.libelle_ptf ,p.dateinventaire, v.actifNet  from  #ValeurActifNet v inner join ptf_fga p on v.compte=p.compte where p.dateinventaire = '01-01-2099'

Select * from #AN

drop table #ValeurActifNet
drop table #AN