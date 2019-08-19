--DateFin correspond  à la date à laquelle on veut comparer l'ecart par rapport au bench
Declare @dateFin as datetime
--DateDeb correspond  à la date à laquelle on veut remonter pour calculer cet ecart 
Declare @dateDeb as datetime
Declare @debutdeMois as bit
Declare @FindeMois as bit

Set @dateDeb = '#paramdeb'
Set @dateFin = '#paramfin'

--set @datedeb = '30/07/2013'
--set @dateFin = '31/07/2013'

--Select DatePArt(weekday,@datefin)

--Si la date de fin est samedi, on effectue le calcul entre le vendredi et le lundi
If DatePArt(weekday,@DateFin) = 6 
begin 
  set @datefin = CAST(@datefin AS DateTime)+2 
end

--On vérifie si on est en début de mois
If Month(@dateDeb) != Month(@dateFin) 
begin
  set @debutdeMois = 0 
end 
--On vérifie qu'on ne passe pas en fin de mois pendant le week end
else if DatePArt(weekday,@DateFin) = 5  and (Month(@DateDeb) != Month (CAST(@datefin AS DateTime)+3))
  begin 
    set @debutdeMois = 0 
  end
else
set @debutdeMois = 1

-- On vérifie si on est en fin de mois
If Month(@DateFin) != Month(CAST (@dateFin AS Datetime)+1) 
begin
  set @Findemois = 0 
end 
--On vérifie qu'on ne passe pas en début de mois pendant le week end
else if DatePArt(weekday,CAST(@datefin AS DateTime)+1) = 5  and (Month(CAST(@datefin AS DateTime) +1) != Month (CAST(@datefin AS DateTime)+4))
  begin 
    set @Findemois = 0 
  end
else
set @Findemois = 1


--Calcul des perfs des indices de références  
-- A la date de fin
select code_omega,date,cours_close into #RefFin from strat_valeur_indice where code_omega in (
'BAEURGVTI110',
'BT11TREU',
'DE0006301161',
'DJEUROSTOXX',
'DJSTOXXEXEUROP',
'MSCIEMERNETUSD',
'SP500NET',
'EONIATMP',
'MSCIACAPFNETEUR'
)
and date = @dateFin 

-- A la date de Debut
select code_omega,date,cours_close into #RefDebut from strat_valeur_indice where code_omega in (
'BAEURGVTI110',
'BT11TREU',
'DE0006301161',
'DJEUROSTOXX',
'DJSTOXXEXEUROP',
'MSCIEMERNETUSD',
'SP500NET',
'EONIATMP',
'MSCIACAPFNETEUR'
)
and date = @dateDeb

select d.code_omega, (f.cours_close-d.cours_close)/f.cours_close as ecart,i.grille ,i.poids into #ReferenceEcart from #RefDebut d inner join #RefFin f on d.code_omega = f.code_omega inner join strat_grille_indice i on d.code_omega = i.code_Indice

select max(date) as Date, code_composite into #DatelastCompo from strat_composition_indice group by code_composite
select s.*  into #LastCompo from strat_composition_indice s inner join #DatelastCompo d on d.date = s.date and d.code_composite = s.code_composite

Create table #TitreBench (
	code_composite varchar(15),
	date datetime,
	Composition varchar(15),
	poids float
)

--Table contenant tous les titres qui n'ont pas de composite. C'est à dire que leur indice est pur
Insert into #TitreBench select * from #LastCompo where composition not in (select code_composite from strat_composition_indice)

--On met dans une table temporaire (#Atraiter) les titres qui ne sont pas des indices purs 
select * into #Atraiter  from #LastCompo where composition in (select code_composite from strat_composition_indice)

declare @continue as BIT 
Set @continue = 0
While (@continue = 0 ) 
begin
	if Exists(select * from #ATraiter) 
	begin
		select a.code_composite,a.date,s.composition,a.poids*s.poids/100 as poids into #tmp from #ATraiter a inner join #LastCompo s on a.composition = s.code_composite
		insert into #TitreBench select * from #tmp where composition not in (select code_composite from strat_composition_indice)
		--On a fini le traitement et on repart pour un autre tour de boucle
		Delete from #Atraiter
		insert into #Atraiter select * from #tmp where composition in (select code_composite from strat_composition_indice)
		--On supprime cette table temporaire qui ne nous est plus utile
		drop table #tmp
	End
    Else SET @continue = 1
END

Create table #ValeurFin (
		identifiant int not null identity,
		code_composite varchar (15) not null,
		date Datetime not null,
		composition varchar (15) not null,
		poids float not null,
		cours_close float not null
)

Create table #ValeurDeb (
		identifiant int not null identity,
		code_composite varchar (15) not null,
		date Datetime not null,
		composition varchar (15) not null,
		poids float not null,
		cours_close float not null
)

--Calcul de la valeur des indices à la date souhaitée
Insert into #ValeurFin (code_composite,date, composition,poids, cours_close ) select code_composite,v.date, composition,poids, cours_close from #TitreBench t inner join STRAT_INDICE i on i.code_indice = t.composition inner join strat_valeur_indice v on v.code_omega= i.code_omega where v.date = @dateFin

--Calcul de la valeur des indices à partir la date souhaitée
Insert into #ValeurDeb  (code_composite,date, composition,poids, cours_close ) select code_composite,v.date, composition,poids, cours_close from #TitreBench t inner join STRAT_INDICE i on i.code_indice = t.composition inner join strat_valeur_indice v on v.code_omega= i.code_omega where v.date = @dateDeb

select f.code_composite,f.composition, f.poids, ((f.cours_close-d.cours_close)/d.cours_close)*100 as ecart  into #EcartIndice from #ValeurDeb d inner join #ValeurFin f on f.identifiant = d.identifiant

select code_composite,composition,((e.poids*g.poids)/100) as poids, ecart,code_omega, grille into #EcartIndiceGrille from #EcartIndice e inner join strat_indice i on i.code_indice = e.composition inner join STRAT_GRILLE_INDICE g on g.code_indice = i.code_omega order by code_composite

--Allocation cible sur l'ensemble des mandats en fonctions des grilles
select s.groupe,sum(i.poids) as poids,i.grille 
into #AllocationGrille 
from strat_AllocationCible s inner join #EcartIndiceGrille  i on indiceReference = code_composite 
group by s.date,s.groupe,i.grille 
order by groupe

--Allocation par groupe et indice en fonction du poids total de la grille
select s.groupe,ag.poids as poids_Total ,sum((i.poids * ecart)/ag.poids) as PerfGrille ,i.grille
into #AllocationBench
from strat_AllocationCible s inner join #EcartIndiceGrille  i on indiceReference = code_composite inner join #AllocationGrille Ag on ag.grille= i.grille and ag.groupe = s.groupe
group by s.date,s.groupe,i.grille,ag.poids
order by groupe


--Table de résultat des performances pour la date Fin
Create table #PerfFin(
		date Datetime not null,
		groupe varchar(25) not null,
		valeur_boursiere float not null,
		PartPTF float not null,
		BenchIndice float not null,
		PartBench float not null,
		EcartBench float not null,
		id_grille varchar(10) not null,
		PerfGrille float
)


--Dans table #GrilleMandat on a toutes les poches  (Action_Euro,ActionExEuro...) qui sont dans chaque ptf
select date, groupe,id_grille into #GrilleMandat from strat_allocation where date = @dateDeb

--On va récupérer la deniere valeur du dernier calcul de performance
Declare @DateLastPerf as datetime

if datepart(weekday,@datedeb)=1 begin set @DateLastPerf = CAST(@dateDeb AS DateTime)-3 end else set @DateLastPerf = @DateDeb

select DateFin,Groupe , id_grille, case when @debutdeMois = 0 then Bench else BenchReel end as Bench into #HistoPerf from strat_valorisation_mandat  where DateFin = @DateLastPerf

Insert into #PerfFin
select Date,allo.groupe, valeur_boursiere,PourcentGrille as PartPTF,poids_total, h.Bench*(1+(PerfGrille/100)) , PourcentGrille- (h.Bench*(1+(PerfGrille/100))), h.id_grille, PerfGrille from strat_allocation allo inner  join #AllocationBench b on b.groupe = allo.Groupe and b.grille = allo.id_grille inner join #HistoPerf h on h.groupe = allo.groupe and h.id_grille= allo.id_grille where allo.date = @dateDeb
Union
select allo.date, g.groupe,allo.valeur_boursiere, PourcentGrille as PartPTF,0,0 as Bench, PourcentGrille , r.grille,r.ecart as perf from #GrilleMandat g inner join #ReferenceEcart r on r.grille =g.id_grille inner join strat_allocation allo on allo.groupe = g.groupe and allo.id_grille = r.grille inner join #HistoPerf h on h.groupe = allo.groupe and h.id_grille= allo.id_grille where not exists (select groupe, grille from #AllocationBench a where a.groupe=g.groupe and a.grille=g.id_grille )  and allo.date = @dateDeb
order by allo.groupe

select p.Groupe,p.id_grille ,case when Benchtotal = 0 then 0 else round ((PartBench*100 )/BenchTotal,2) end as BenchReel into #BenchActualise from #Perffin p inner join (select groupe, sum(partbench) as benchtotal from #perffin group by groupe ) RQT on rqt.groupe= p.groupe


--select *,(valeur_ecart * perfGrille) /100 as Plue_value from #PerfFin
Insert into STRAT_VALORISATION_MANDAT
select @Datedeb, @datefin, p.Groupe,p.Id_grille, Round(valeur_boursiere,2) as Valeur_boursiere,Round(PartPTF,2) as PartPTF,BenchIndice,case when @Findemois = 0 then BenchIndice else BenchReel end as PartBench,round(PartPTF-BenchReel,2) as EcartBench, round(PerfGrille,2) as PerfGrille, round((((EcartBench/100)*(PerfGrille/100))*100),5) as Plue_Value from #PerfFin p inner join strat_grille g on g.Id = p.Id_grille inner join #BenchActualise b on b.groupe = p.groupe and b.id_grille = p.id_grille order by groupe, ordre asc

Drop table #DatelastCompo
Drop table #LastCompo 
Drop table #TitreBench 
drop table #Atraiter
drop table #AllocationBench
Drop table #AllocationGrille
Drop table #ValeurDeb
Drop table #ValeurFin
drop table #EcartIndice
drop table #EcartIndiceGrille
drop table #RefDebut
drop table #RefFin
drop table #ReferenceEcart 
drop table #PerfFin
drop table #GrilleMandat
drop table #HistoPerf
drop table #BenchActualise

