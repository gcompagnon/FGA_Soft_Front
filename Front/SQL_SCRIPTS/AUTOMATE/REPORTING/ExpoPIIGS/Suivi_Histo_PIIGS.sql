
--create table TMP_RECAP_OP(grp_emetteur varchar(30),
--mois_operation tinyint,
-- annee_operation int,
--  type_operation char(1),
--   montant float)


/****** Script for SelectTopNRows command from SSMS  ******/
--'30/12/2011','31/01/2012','29/02/2012','30/03/2012','30/04/2012','31/05/2012','29/06/2012'


-- prendre 1 année en arrière : et de 01/01/AAAA jusqu a aujourd hui
Declare @dateStart datetime
set @dateStart = '01/01/'+Cast( year(getdate())-1 as varchar)

create table #DATES(numero tinyint,dateInventaire datetime, mois_operation tinyint, annee_operation int)

declare @dateEnd datetime
set @dateEnd =  DATEADD(month,1,getdate())


/******Creation dans @res de la liste des fins de mois*****************************/
declare @res varchar(500)
set @res=''''
declare @dateIterative datetime
declare @i tinyint
set @i = 0

while @datestart<=@dateEnd
BEGIN	
	set @dateIterative='01/'+CAST(month(@datestart) as varchar)+'/'+CAST(year(@datestart) as varchar)	
	set @dateIterative=DATEADD(d,-1,@dateIterative)
	--cas particulier fin de mois spéciale
	IF @dateIterative='31/03/2013'
	BEGIN
			set @dateIterative = '28/03/2013'
	END
	ELSE
	BEGIN					
		WHILE DATEPART(weekday,@dateIterative)>5
		BEGIN
				set @dateIterative=DATEADD(d,-1,@dateIterative)
		END
	END
	set @res = @res +CONVERT(varchar,@dateIterative,103)
	insert into #DATES 	select @i,@dateIterative,month(@dateIterative),year(@dateIterative)
	
	
	set @datestart = DATEADD(month, 1,@datestart )		
	set @i = @i+1
	
	IF @datestart<@dateEnd
		set @res=@res+''', '''
	ELSE
		set @res=@res+''''
    

END
   
select @res
select @datestart
/*************************************************************************/

declare @query varchar(max)    
set @query =N' SELECT
	UPPER(Grp_Emetteur) as Grp_Emetteur, 
	Dateinventaire as dateInventaire,
	case
		when [pct_det_Niv_0] is null then ''direct''
		else ''transparisé''
	end as Niveau,
	SUM([Valeur_Boursiere]+[Coupon_Couru]) as VB_TOTALE
  INTO ##temp2 FROM [dbo].[PTF_TRANSPARISE]
  where [Numero_Niveau]=''3''
  and [Dateinventaire] in ('+@res+') 
  and [Type_actif]=''Obligations''
  and [Grp_Emetteur] in (''Italie'',''espagne'')
  and [Groupe] not in (''OPCVM'',''MM AGIRC'',''MM ARRCO'',''IRCEM RETRAITE'',''IRCEM PREVOYANCE'',''IRCEM MUTUELLE'',''UNMI'',''IDENTITES MUTUELLE'',''ARCELOR MITTAL France'')
   GROUP BY 
      Grp_Emetteur,
      Dateinventaire,
      case
		when [pct_det_Niv_0] is null then ''direct''
		else ''transparisé''
	end'
PRINT @query
EXECUTE(@query)
    
--creation de la table des variations achats/ventes par mois
select grp_emetteur, mois_operation,annee_operation, SUM(montant) as montant
into #var from TMP_RECAP_OP
group by grp_emetteur, mois_operation,annee_operation

--DEBUG
-- select * from #var
-- select * from ##temp2 order by grp_emetteur, dateInventaire,Niveau
--select * from TMP_RECAP_OP
select
		case when t1.grp_emetteur='ESPAGNE' then 'serie1;serie2;serie3;serie4'
			when t1.grp_emetteur='ITALIE' then 'serie7;serie8;serie9;serie10'
			else ''
		end as graph,
		t1.grp_emetteur,
		Msuivant.dateinventaire as dateInventaire,
		opA.montant as Achats_direct,
		opV.montant as Ventes_direct,
		tmpDir.VB_totale - t1Bis.VB_totale - v.montant as variation_MTM_Direct,
		tmpTrans.VB_totale - t1.VB_totale as Variation_stock_Transparisé		
FROM ##temp2 as t1
join #DATES as Mcourant on Mcourant.dateInventaire = t1.dateInventaire
join #DATES as Msuivant on Msuivant.numero = ( Mcourant.numero+1)
JOIN ##temp2 as tmpTrans on t1.grp_emetteur=tmpTrans.grp_emetteur
	and tmpTrans.dateinventaire = Msuivant.dateInventaire
	and t1.niveau=tmpTrans.niveau
JOIN ##temp2 as tmpDir on t1.grp_emetteur=tmpDir.grp_emetteur
	and tmpDir.dateinventaire = Msuivant.dateInventaire
	and tmpDir.niveau='direct'
JOIN ##temp2 as t1Bis on t1.grp_emetteur=t1Bis.grp_emetteur
	and t1.dateinventaire=t1Bis.dateinventaire
	and t1Bis.niveau='direct'
JOIN #var as v on v.grp_emetteur=t1.grp_emetteur
    and v.mois_operation  = Msuivant.mois_operation
    and v.annee_operation  = Msuivant.annee_operation
JOIN TMP_RECAP_OP as opA on opA.grp_emetteur=t1.grp_emetteur
    and opA.mois_operation  = Msuivant.mois_operation
    and opA.annee_operation  = Msuivant.annee_operation
	and opA.type_operation='A'
JOIN TMP_RECAP_OP as opV on opv.grp_emetteur=t1.grp_emetteur
    and opV.mois_operation  = Msuivant.mois_operation
    and opV.annee_operation  = Msuivant.annee_operation
	and opV.type_operation='V'
where t1.niveau='transparisé'
order by t1.grp_emetteur, t1.dateInventaire
 
SELECT
		case when t1.grp_emetteur='ESPAGNE' then 'serie5;serie6'
			when t1.grp_emetteur='ITALIE' then 'serie11;serie12'
			else ''
		end as graph,
		t1.grp_emetteur,
		t1.dateInventaire,
		t1.VB_Totale as VB_Totale_Direct,
		t2.VB_Totale as VB_Totale_Transparisé
FROM ##temp2 as t1
JOIN ##temp2 as t2 on t1.grp_emetteur=t2.grp_emetteur
	and t1.dateInventaire=t2.dateInventaire
	and t2.niveau='transparisé'
WHERE t1.niveau='direct'
and t1.dateInventaire >= (select dateInventaire from #DATES where numero=1)
order by t1.grp_emetteur, t1.dateInventaire


--Drop table #TEMP
--drop table #operations

drop table ##temp2
drop table #var
drop table #DATES

--delete TMP_RECAP_OP