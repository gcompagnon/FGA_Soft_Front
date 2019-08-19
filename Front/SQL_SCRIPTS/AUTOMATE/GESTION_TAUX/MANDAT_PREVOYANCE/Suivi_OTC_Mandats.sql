use omega

declare @date as datetime
SET @date = (Select GETDATE())
SET @date = '15/04/2014'



(select 
   convert(varchar(10),PR._date,103) as Date_valorisation,
   C._libellecli as Libelle_ptf,
   P._libelle1prod as Libelle_Produit,
   O._sensoperation as sens,
   Contrepartie = case
   when O._codecourt='SGO' then 'SG'
   when O._codecourt='BARO' then 'BARCLAYS'
   when O._codecourt='NATIO' then 'NATIXIS'
   when O._codecourt='HSBCO' then 'HSBC'
   when O._codecourt='BNPPO' then 'BNP'
  when O._codecourt='DBO' then 'DB'
   else '' end,
   convert(varchar(10),O._dateoperation,103) as Date_operation,
    convert(varchar(10),O._datevaleur,103) as Date_forward,
    O._nominal as Nominal,
   ((PR._coursclose*O._nominal)/100)-(O._montantbrut) as Exposition,
 O._txactuariel as TRA,
 convert(varchar(10),P._echeance,103) as echeance,
   'cash' as Type_Collateral
into #TEMP0
from fcp.operat  O
left outer join fcp.cpartfcp C on o._compte=c._compte
left outer join com.produit P on O._codeprodui=P._codeprodui
inner join com.prixhist PR on O._codeprodui=PR._codeprodui and PR._date=(select max(_date) from com.prixhist PR where PR._date<=@date and O._codeprodui=PR._codeprodui)
where left(O._codeprodui,2)='FW')

insert into #TEMP0
select
convert(varchar,V._dateoperation,103),
C._libellecli,
p._libelle1prod,
o._codeoperation,
case
   when B._codecourt='SGO' then 'SG'
   when B._codecourt='BARO' then 'BARCLAYS'
   when B._codecourt='NATIO' then 'NATIXIS'
   when B._codecourt='HSBCO' then 'HSBC'
   when B._codecourt='BNPPO' then 'BNP'
  when B._codecourt='DBO' then 'DB'
   else '' end,
convert(varchar(10),o._dateoperation,103),
convert(varchar(10),o._datevaleur,103),
(p._nominal), 
(v._netamountD),
 '0',
convert(varchar(10),p._echeance,103),
'cash'
from (SELECT  _compte as max_compte,  max(_dateoperation) as max_date
	FROM     fcp.valoprdt 
	WHERE  _dateoperation <=@date 
	GROUP BY  _compte) md,   
        fcp.valoprdt  v
left outer join com.produit p on  p._codeprodui=v._assettype
left outer join fcp.cpartfcp C on v._compte=c._compte
left outer join fcp.operat O on P._codeprodui=O._codeprodui
left outer join com.courtier B on B._codecourt=O._contrepartie
where           
v._dateoperation=md.max_date and v._compte =md.max_compte
and p._codedutype='SWLG' and o._codeoperation='A' and left(v._compte,2) not in ('61','AV')

select  *,row_number() OVER(PARTITION BY Libelle_Ptf,Contrepartie ORDER BY Libelle_Produit  ) as id 
into #LIST_PRODUIT
from #TEMP0 



select 
_numeroint,
Compte=case
when _compte='COLL08115' then 'CMAV OBLIGATIONS'
when _compte='COLL630012' then 'QUATREM RETRAITE OBLIGATIONS'
when _compte='COL4030005' then 'MMP PREV OBLIGATIONS' end,

Contrepartie=case	
   when _codecourt='SGO' then 'SG'	
   when _codecourt='BARO' then 'BARCLAYS'	
   when _codecourt='NATIO' then 'NATIXIS'	
   when _codecourt='HSBCO' then 'HSBC'	
   when _codecourt='BNPPO' then 'BNP'	
  when _codecourt='DBO' then 'DB'	
   else 'NA' end,	
 Montant=case
   when _codemvt='D' then (-1)*(_montant) else _montant end
 into #TEMP1 

from fcp.mouvemt where left(_compte,3)='COL' and _dateoperation<=@date


select
Compte,
Contrepartie,
sum(Montant) as collateral_detenu
into #TEMP2
from #TEMP1
Group by Compte,Contrepartie


insert into #TEMP2 
select
Compte,
'' as contrepartie,
sum(Montant) as collateral_detenu
from #TEMP1
Group by Compte

------------------------------
select 5 as 'style', t.*,isnull(p.collateral_detenu,0) as collateral_detenu from #LIST_PRODUIT as t
left outer join #TEMP2 as p on p.Compte = t.Libelle_ptf and p.Contrepartie = t.Contrepartie and t.id = 1
order by t.Libelle_ptf, t.Contrepartie,id

------------------------------
select 5 as 'style',* from #TEMP2
order by Compte, Contrepartie
------------------------------
select 5 as 'style',* from #TEMP1

Drop table #TEMP0
Drop table #LIST_PRODUIT
Drop table #TEMP1
Drop table #TEMP2