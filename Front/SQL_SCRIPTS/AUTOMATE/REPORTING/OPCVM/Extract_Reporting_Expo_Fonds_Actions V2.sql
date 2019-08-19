/*
Requete de stephane pour daniel
*/
Declare @date datetime
set @date=(select max(_dateoperation) from fcp.valoprdt where _dateoperation> DATEADD(month,-1, GETDATE()) and _dateoperation < DATEADD(day, -DAY(GETDATE()) , GETDATE()))

select 
           
            v._dateoperation                   as Dateinventaire,
            v._compte                                  as Compte, 
            c._LIBELLECLI                          as Libelle_Ptf,
            s._libellesecteur   as Secteur,
            type=case
            when (left(P._codedutype,1) in ('A','D'))or (left(P2._codedutype,1)='A') and (P._codedutype)<>'OCV' then 'Actions' 
            when left(P._codedutype,2) in ('OT','OI','OP','OC','OI','OU','CD') then 'Taux'
            when Left(P._codedutype,3) in ('CAC','CAL','FAE','FAG','FAU','PUT') and left(s._libellesecteur,5) not in ('FONDS') then 'Actions SE' --traiter en sectoriel les certicats actions sectoriels
            when (P._codeprodui) in ('GXM4','STM4','STU4') then 'Actions PY' --traiter en pays des dérivés spécifiques pays déterminés
            else '' end,
            p._isin                                   as Code_isin,   
            p._libelle1prod                           as Libelle,             
            'exposition'= case when v._libelletypeproduit like 'FUTURES %' then (case when (v._BS = 'V') then -1 else 1 end) *(p._multiple*v._marketpriceD*v._quantite)	
            when left (v._libelletypeproduit,3) in ('CAL','PUT') then (case v._BS when 'A' then 1 else -1 end)*(case when (v._devises<>'EUR') then 1/d._coursclose else 1 end)*v._quantite*p._multiple*h._delta*h2._coursclose
            else(v._netamountD+v._netamountDcc)end,
            pi._LIBELLEPAYS as Pays           
into #TEMP1             
from 
          fcp.valoprdt  v
 
left outer join           com.produit p                                   on  p._codeprodui=v._assettype
left outer join           com.produit p2                 on p2._codeprodui=p._sousjacent	
left outer join           com.prixhist  h               on v._dateoperation=h._date and p._codeprodui=h._codeprodui	
left outer join           com.prixhist  h2              on v._dateoperation=h2._date and p._sousjacent=h2._codeprodui	
left outer join           com.devhist d                on d._contredevise=v._devises and d._date=v._dateoperation
left outer join 	  com.prdtclasspys ps	        on p._codeprodui=ps._codeprodui and ps._classification=0
left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
left outer join 	  com.ssectclass ss_s           on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
left outer join           com.pays pi                                     on pi._CODEPAYS= ps._codepays
left outer join           fcp.cpartfcp c                                   on  c._compte=v._compte
left outer join           Fcp.CONSGRPE cg                      on cg._compte=v._compte
left outer join           fcp.grpedeft g                                   on g._codegroupe=cg._codegroupe
where           
         v._dateoperation=@date
and g._codegroupe in ('OP')

select
T.Dateinventaire,
T.Compte, 
T.Libelle_Ptf,
T.Secteur,
sum(T.exposition) as exposition_totale,
sum(T.exposition)/md.Total as Repart_secteur_Ptf
from 
(select compte, sum(exposition)as Total from #TEMP1
where type in ('Actions','Actions SE') group by compte) md,
#TEMP1 T
where T.Compte in ('6100001','6100002','6100004','6100024','6100026','6100030','6100062','6100063','6100066','6100089','6100093','6100097','AVEURO','AVEUROPE')
and T.type in ('Actions','Actions SE') and T.compte=md.compte

group by
T.Dateinventaire,
T.Compte, 
T.Libelle_Ptf,
T.Secteur,
md.Total


order by T.Compte

select
TE.Dateinventaire,
TE.Compte, 
TE.Libelle_Ptf,
TE.Pays,
sum(TE.exposition) as exposition_totale,
sum(TE.exposition)/me.Total as Repart_Pays_Ptf

from 
(select compte, sum(exposition)as Total from #TEMP1
where type in ('Actions','Actions PY') group by compte) me,
#TEMP1 TE 
where TE.Compte in ('6100001','6100002','6100004','6100024','6100026','6100030','6100062','6100063','6100066','6100089','6100093','6100097','AVEURO','AVEUROPE')
and TE.type in ('Actions','Actions PY') and TE.compte=me.compte

group by
TE.Dateinventaire,
TE.Compte, 
TE.Libelle_Ptf,
TE.Pays,
me.Total

select * from #TEMP1

Drop table #TEMP1   