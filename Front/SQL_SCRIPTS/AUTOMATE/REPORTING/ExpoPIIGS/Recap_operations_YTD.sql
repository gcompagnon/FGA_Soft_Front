-- Requete sur Omega pour lister les operations sur des emetteurs particuliers: Italie et espagne

-- prendre 2 années en arrière : et le 01/12/AAAA pour commencer les données sur un 01/01/AAAA
Declare @dateStart datetime
set @dateStart = DATEADD(year, -2, getdate())


Declare @date1 datetime
set @date1 = '01/12/'+Cast(year(@dateStart) as varchar)

select
      GR._libellegrper as grp_emetteur, 
      Month(O._dateoperation) as mois_operation,
      Year(O._dateoperation) as annee_operation,
      O._codeoperation as type_operation,
      case when O._codeoperation='V'
		then -sum(o._mtnet)
		else sum(o._mtnet)
	  end as montant
into #tmp from
    fcp.operat O
left outer join com.produit P on O._codeprodui=P._codeprodui
left outer join  fcp.cpartfcp C  on O._compte=C._compte
left outer join fcp.CONSGRPE cg on c._compte=cg._compte
left outer join fcp.grpedeft g on g._codegroupe=cg._codegroupe
left outer join com.emetteur E on P._emetteur=E._emetteur
left outer join  com.grpemetteursratios GR on E._grper=GR._codegrper   
where
	g._codegroupe in ('UM','IN','CA','CT','MM','sa','MG','QX','GA')
	and O._codeoperation not in ('AR','ES','ET','EE','RB')
	and GR._libellegrper in ('ITALIE','ESPAGNE')    
	and O._dateoperation >= @date1
group by GR._libellegrper, Month(O._dateoperation),Year(O._dateoperation), O._codeoperation


/***table contenant les deux types d'opérations possible***/
create table #operations (op varchar(2))
insert into #operations values('A')
insert into #operations values('V')

/****Variable pour remplir la table #mois***/
declare @mois tinyint
declare @dateEnd datetime
set @dateEnd =  getdate()

set @datestart=@date1

/****Table contenant tout les mois du début de l'année à l'actuel***/
create table #mois (mois tinyint,annee int)
while @datestart<=@dateEnd
begin
set @mois= DATEPART(month, @datestart)
insert into #mois values (@mois,Year(@datestart))
set @datestart = DATEADD(month, 1,@datestart )
end



select grp_emetteur,mois,annee,op into #pays_operations
from #tmp,#mois, #operations 
group by grp_emetteur,mois,annee,op
order by grp_emetteur, op, annee,mois


SELECT po.grp_emetteur, po.mois as mois_operation,  po.annee as annee_operation,po.op as type_operation,
		case when montant is null then 0
		else tmp.montant
		end as montant
FROM #pays_operations as po
left outer join #tmp as tmp on 
		tmp.grp_emetteur=po.grp_emetteur
		and tmp.mois_operation=po.mois
		and tmp.annee_operation=po.annee
		and tmp.type_operation=po.op
	
drop table #tmp
drop table #operations
drop table #pays_operations
drop table #mois
