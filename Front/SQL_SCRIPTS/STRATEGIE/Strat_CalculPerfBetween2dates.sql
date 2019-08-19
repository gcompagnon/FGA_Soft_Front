Declare @DateDeb as DateTime
Declare @DateFin as DateTime
Declare @NbBoucle as Integer

Set @DateDeb = '#ParamDeb'
Set @DateFin = '#ParamFin'

--Set @DateDeb ='11-07-2013'
--Set @DateFin ='17-07-2013'


select * into #WorkTable from strat_valorisation_mandat where datedeb between @DateDeb and @DateFin

select max(Datedeb)as datedeb, max(dateFin) as dateFin into #LastDate from #WorkTable

select Groupe,Id_grille, sum(plue_value) as plue_value into #PlueValue from #Worktable group by groupe,id_grille

select distinct dateDeb into #TmpDeb from #Worktable order by dateDeb
select distinct dateFin into #TmpFin from #Worktable order by dateFin

Create table #CalculPerf (
		groupe varchar(25),
		id_grille varchar(10),
		PerfPoche float)

Declare @Continue as bit -- Si 0 si on continue 1 sinon
Set @Continue=0

--Initialisation, on garde les premières perfs 
Insert into #CalculPerf Select groupe,id_grille,1+(PerfPoche/100) from #WorkTable where DateDeb = (select top 1 datedeb from #TmpDeb) and DateFin =(select top 1 dateFin from #TmpFin)

--Initialisation faites : On a plus besoin des 2 premieres dates
Delete top (1) from #TmpDeb
Delete top (1) from #TmpFin

While (@Continue = 0 )
begin
	If EXISTS (select * from #TmpDeb) 
	begin
		
		--Si on a des valeurs, il reste des dates à prendre en compte
		Update #CalculPerf set Perfpoche = c.PerfPoche * (1+(matable.PerfPoche/100)) 
			from 
				(select groupe,id_grille,PerfPoche 
				from #WorkTable 
				where DateDeb = (select top 1 datedeb from #TmpDeb) 
				and DateFin =(select top 1 dateFin from #TmpFin)
				) matable inner join #CalculPerf c
				on c.groupe = matable.groupe and c.id_grille = matable.id_grille
		-- On a traité ces deux dates donc on les supprime
		Delete top (1) from #TmpDeb
		Delete top (1) from #TmpFin
		--Set @DateDeb= CAST(@DateDeb AS DateTime)+1
		
	end
	else Set @continue = 1
end
Update #CalculPerf set PerfPoche = (PerfPoche -1 )*100
--select * from #CalculPerf

Select w.groupe,w.id_grille,valeur_boursiere, Part_PTF,BenchReel,EcartBench, c.PerfPoche, p.Plue_Value  from #CalculPerf c 
inner join #Pluevalue p on c.Groupe = p.groupe and c.Id_grille= p.id_grille
inner join #WorkTable w on w.Groupe = p.groupe and w.Id_grille= p.id_grille
where w.datedeb = (select Datedeb from #LastDate) and w.dateFin= (select DateFin from #LastDate) 
order by groupe,id_grille



Drop table #WorkTable 
Drop table #Pluevalue
Drop table #TmpDeb
Drop table #TmpFin
Drop table #CalculPerf
Drop table #LastDate 

