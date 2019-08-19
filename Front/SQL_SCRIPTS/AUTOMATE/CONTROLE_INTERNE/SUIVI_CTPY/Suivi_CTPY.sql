DECLARE @date datetime
SET @date=(Select GETDATE())
(select 
   5 as 'style',
   convert(varchar,PR._date,103) as Date_valorisation, 
   'Forwards' as type_produit,
   B._libellecourt as contrepartie,  
   sum(O._nominal) as Nominal,
   sum(((PR._coursclose*O._nominal)/100)-(O._montantbrut)) as Exposition
 
from fcp.operat  O
left outer join fcp.cpartfcp C on o._compte=c._compte
left outer join com.produit P on O._codeprodui=P._codeprodui
left outer join com.courtier B on B._codecourt=O._codecourt
inner join com.prixhist PR on O._codeprodui=PR._codeprodui and PR._date=(select max(_date) from com.prixhist PR where PR._date<=@date and O._codeprodui=PR._codeprodui)
where left(O._codeprodui,2)='FW'
group by PR._date, B._libellecourt)
union
(select
5 as 'style',
convert(varchar,V._dateoperation,103),
'Obligations structurees',
GR._libellegrper,
sum(v._quantite*p._nominal),
sum(v._netamountD+v._netamountDcc)
from fcp.valoprdt v
left outer join com.produit p  on  p._codeprodui=v._assettype
left outer join com.emetteur e on e._emetteur=p._emetteur
left outer join com.grpemetteursratios GR on  e._grper=GR._codegrper
where P._codedutype='OSTRU' and v._dateoperation=@date
group by V._dateoperation, GR._libellegrper)
 union
 
(select
5 as 'style',
convert(varchar,V._dateoperation,103),
'Swaps',
B._libellecourt,
sum(p._nominal), 
sum(v._netamountD)
from (SELECT  _compte as max_compte,  max(_dateoperation) as max_date   
	FROM     fcp.valoprdt 
	WHERE  _dateoperation <=@date
	GROUP BY  _compte) md,
          fcp.valoprdt  v
left outer join com.produit p on  p._codeprodui=v._assettype
left outer join fcp.operat O on P._codeprodui=O._codeprodui
left outer join com.courtier B on B._codecourt=O._codecourt
where           
v._dateoperation=md.max_date and v._compte =md.max_compte
and p._codedutype='SWLG' and o._codeoperation='A'
Group by V._dateoperation, B._libellecourt)


---------------------------------------------------------------------
-- DETAIL
---------------------------------------------------------------------

(select 
   5 as 'style',
   convert(varchar,PR._date,103) as Date_valorisation, 
   C._libellecli as libelle_compte,
   'Forwards' as type_produit,
   O._codeprodui as code_produit,
   p._libelle1prod as libelle_produit,
   B._libellecourt as contrepartie,  
   O._nominal as Nominal,
   ((PR._coursclose*O._nominal)/100)-(O._montantbrut) as Exposition
 
from fcp.operat  O
left outer join fcp.cpartfcp C on o._compte=c._compte
left outer join com.produit P on O._codeprodui=P._codeprodui
left outer join com.courtier B on B._codecourt=O._codecourt
inner join com.prixhist PR on O._codeprodui=PR._codeprodui and PR._date=(select max(_date) from com.prixhist PR where PR._date<=@date and O._codeprodui=PR._codeprodui)
where left(O._codeprodui,2)='FW')
--order by PR._date, C._libellecli, O._codeprodui, p._libelle1prod, B._libellecourt)
union
(select
5 as 'style',
convert(varchar,V._dateoperation,103),
C._libellecli,
'Obligations structurees',
p._codeprodui,
p._libelle1prod,
GR._libellegrper,
(v._quantite*p._nominal),
(v._netamountD+v._netamountDcc)
from fcp.valoprdt v
left outer join com.produit p  on  p._codeprodui=v._assettype
left outer join fcp.cpartfcp C on v._compte=c._compte
left outer join com.emetteur e on e._emetteur=p._emetteur
left outer join com.grpemetteursratios GR on  e._grper=GR._codegrper
where P._codedutype='OSTRU' and v._dateoperation=@date)
--order by V._dateoperation,C._libellecli,O._codeprodui,p._libelle1prod,GR._libellegrper)
 union
 
(select
5 as 'style',
convert(varchar,V._dateoperation,103),
C._libellecli,
'Swaps',
O._codeprodui,
p._libelle1prod,
B._libellecourt,
(p._nominal), 
(v._netamountD)
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
and p._codedutype='SWLG' and o._codeoperation='A')
--Order by V._dateoperation,C._libellecli,O._codeprodui,p._libelle1prod, B._libellecourt)