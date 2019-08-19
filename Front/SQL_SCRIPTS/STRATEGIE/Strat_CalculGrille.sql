Delete from STRAT_ALLOCATION where date = CAST(FLOOR(CAST(GETDATE() AS FLOAT)) AS DateTime)

Create table #lastTitre (
				Isin_titre varchar(20) not null,
				Id_grille varchar(10) not null,
				Date datetime not null,
				poids float,
				source varchar(20) null
)

insert into  #lastTitre 
select st.* 
from (
	Select isin_titre , max([date]) as madate
	from strat_titre_grille 
	group by isin_titre ) rqt 
	inner join strat_titre_grille st on rqt.isin_titre = st.isin_titre and rqt.madate = st.[date]


create table  #LastMandat  (
	groupe [varchar](25) NOT NULL,
	Date datetime not null
	)
	
insert into #LastMandat
select groupe ,max(dateinventaire) from [PTF_TRANSPARISE] where numero_niveau =5 group by groupe -- and Dateinventaire = '28/02/2013' where and groupe ='MM ARRCO'

create table #SuiviStrat (
	groupe [varchar](25) NULL,
	Date datetime not null,
	compte varchar(60) not null,	
	libelle_ptf varchar(60) not null,
	code_titre varchar(15)not null,
	libelle_titre varchar(60) null,
	valeur_boursiere float null,
	coupon_couru float null,
	Id_grille varchar(10) null,
	poids float null
)

insert into #SuiviStrat
select	t.groupe,
		t.dateinventaire,
		t.compte,
		t.libelle_ptf,
		t.code_titre,
		t.libelle_titre,
		t.valeur_boursiere,
		t.coupon_couru,
		lt.id_grille,
		lt.poids
		from (ptf_transparise t 
		inner join #LastMandat lm on t.groupe = lm.groupe and t.Dateinventaire = lm.Date 
		left join #LastTitre lt on lt.isin_titre = t.code_titre ) 
where t.numero_niveau = 5 and valeur_boursiere is not null

--Les liquidités sont des lignes de type monétaire
Update #SuiviStrat set id_grille = 'Monetaire', poids = 1 where libelle_titre like '%Liquidité%'  

--Les autres titres n'ont pas de grille, on les range dans une case non affecté : 
Update #SuiviStrat set id_grille = 'NonAffecte', poids = 1 where id_grille is null and poids is null

--Les autres titres n'ont pas de grille, on les range dans une case non affecté : 
--Update #SuiviStrat set coupon_couru = 0 where coupon_couru is null

Create table #MandatGrille (
							Groupe varchar(25),
							valeur_boursiere float,
							PourcentGrille float,
							id_grille varchar(10)
							)

insert into #MandatGrille (Groupe,valeur_boursiere,id_grille) select groupe, sum(valeur_boursiere) + sum(coupon_couru)as valeur_boursiere, 'Autres' from #SuiviStrat where id_grille is null group by groupe order by groupe
						
Insert into #MandatGrille (	Groupe,valeur_boursiere,id_grille) select groupe, sum(valeur_boursiere* poids) + sum(coupon_couru*poids) as valeur_boursiere,id_grille  from #SuiviStrat group by groupe, id_grille order by groupe
--delete from #MandatGrille where id_grille is null 



Create table #Tmp  (
					Groupe varchar(25),
					valeur_boursiere float,
					coupon_couru float
					)
insert into #Tmp select groupe, sum(valeur_boursiere* poids)as valeur_boursiere,sum(coupon_couru*poids) as coupon_couru from #SuiviStrat group by groupe order by groupe


Declare @groupe varchar(25)
Declare @valeurboursiereTotale float

WHILE EXISTS(SELECT TOP 1 * FROM #tmp )
	BEGIN
	SET @groupe = (select TOP 1 groupe FROM #tmp)
    SET @valeurBoursiereTotale = (select TOP 1 valeur_boursiere+ coupon_couru FROM #tmp)
    --Set @valeurBoursiereTotale = @valeurBoursiereTotale --+ (select sum(valeur_boursiere)as valeur_boursiere from #SuiviStrat where id_grille is null and groupe = @groupe group by groupe)
	Update #MandatGrille set pourcentGrille = ((valeur_boursiere)/@valeurboursiereTotale)*100 where groupe = @groupe
    Delete Top (1) from #tmp
END

select CAST(FLOOR(CAST(GETDATE() AS FLOAT)) AS DateTime), * from #MandatGrille

drop table #SuiviStrat
drop table #LastMandat  
Drop table #LastTitre
drop table #tmp
drop table #MandatGrille


