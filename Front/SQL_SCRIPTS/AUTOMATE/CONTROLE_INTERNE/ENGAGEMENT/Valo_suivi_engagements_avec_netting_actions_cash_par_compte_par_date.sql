declare @date datetime	
set @date=(Select GETDATE())
(select 	
			5 as 'style',
            g._NOMGROUPE                      as Groupe,	
            convert(varchar,v._dateoperation,103)                   as Dateinventaire,	
            v._compte                                  as Compte, 	
            c._LIBELLECLI                          as Libelle_Ptf,	
            v._assettype                              as Code_Titre,            	
            p._libelle1prod                           as Libelle_Titre,	
            P2._libelle1prod                          as Libelle_sous_jacent,                       	
            v._libelletypeproduit                   as Type_Produit,
            v._deviseS                                as Devise_Titre,
            v._BS  as sens_position,
            case 
             when p._echeance is null Then ''
             when p._echeance < '01/01/1901' Then ''
             else convert( varchar(10),p._echeance,103) end as Maturite,
           'engagement en euro'=CASE	
           when v._libelletypeproduit like 'FUTURES %' then sum( (case when (v._BS = 'V') then -1 else 1 end) *(p._multiple*v._marketpriceD*v._quantite)) 	
           else sum(isnull((case v._BS when 'A' then 1 else -1 end)*(case when (v._devises<>'EUR') then 1/d._coursclose else 1 end)*v._quantite*p._multiple*h._delta*h2._coursclose,0)) end,
           'engagement en poids'=CASE	
           when v._libelletypeproduit like 'FUTURES %' then sum( (case when (v._BS = 'V') then -1 else 1 end)*(p._multiple*v._marketpriceD*v._quantite))/o._actif 	
           else sum(isnull((case v._BS when 'A' then 1 else -1 end)*(case when (v._devises<>'EUR') then 1/d._coursclose else 1 end)*v._quantite*p._multiple*h._delta*h2._coursclose,0))/o._actif end	
from 	
          fcp.valoprdt  v	
left outer join           com.produit p                                   on  p._codeprodui=v._assettype	
left outer join           com.produit p2                 on p2._codeprodui=p._sousjacent	
left outer join           com.prixhist  h               on v._dateoperation=h._date and p._codeprodui=h._codeprodui	
left outer join           com.prixhist  h2              on v._dateoperation=h2._date and p._sousjacent=h2._codeprodui	
left outer join           com.devhist d                on d._contredevise=v._devises and d._date=v._dateoperation	
left outer join           fcp.vlrstion o on v._compte=o._compte and v._dateoperation=o._dateoperation	
left outer join           fcp.cpartfcp c                                   on  c._compte=v._compte	
left outer join           Fcp.CONSGRPE cg                      on cg._compte=v._compte	
left outer join           fcp.grpedeft g                                   on g._codegroupe=cg._codegroupe 	
 where 	
 g._codegroupe in ('op')	
 and	
 v._dateoperation=@date	
 and 	
(v._marche)in ('F','O')	
  group by g._NOMGROUPE, v._dateoperation,  v._compte,c._LIBELLECLI,v._assettype, p._libelle1prod, P2._libelle1prod, v._libelletypeproduit,v._deviseS, v._BS, p._echeance, o._actif)	
union	
(select 	
			5 as 'style',
            g._NOMGROUPE                      as Groupe,	
            convert(varchar,v._dateoperation,103)                   as Dateinventaire,	
            v._compte                                  as Compte, 	
            c._LIBELLECLI                          as Libelle_Ptf,	
            v._assettype                              as Code_Titre,            	
            p._libelle1prod                           as Libelle_Titre,	
            p._libelle1prod,                     	
            v._libelletypeproduit                   as Type_Produit,  	
            v._deviseS                                as Devise_Titre,	
            v._BS,     	
           '',    	
           sum(v._netamountd),	
           sum(v._netamountd)/o._actif	
from 	
          fcp.valoprdt  v	
left outer join           com.produit p                                   on  p._codeprodui=v._assettype	
left outer join           fcp.vlrstion o on v._compte=o._compte and v._dateoperation=o._dateoperation	
left outer join           fcp.cpartfcp c                                   on  c._compte=v._compte	
left outer join           Fcp.CONSGRPE cg                      on cg._compte=v._compte	
left outer join           fcp.grpedeft g                                   on g._codegroupe=cg._codegroupe 	
 where 	
 g._codegroupe in ('op')	
 and	
 v._dateoperation=@date	
 and	
 v._assettype in (select distinct p1._sousjacent from fcp.valoprdt v1 left outer join com.produit p1 on v1._assettype=p1._codeprodui	
 left outer join fcp.cpartfcp c on  c._compte=v1._compte 
 left outer join Fcp.CONSGRPE cg on cg._compte=v1._compte 
 left outer join fcp.grpedeft g on g._codegroupe=cg._codegroupe	
 where  g._codegroupe in ('op') and v1._dateoperation=@date and v1._marche in ('F','O')) 	
 and v._marche='A'	
 group by g._NOMGROUPE, v._dateoperation,  v._compte,c._LIBELLECLI,v._assettype, p._libelle1prod, v._libelletypeproduit,v._deviseS, v._BS, o._actif)
 union
 (select 	
			5 as 'style',
            g._NOMGROUPE                      as Groupe,	
            convert(varchar,v._dateoperation,103)                   as Dateinventaire,	
            v._compte                                  as Compte, 	
            c._LIBELLECLI                          as Libelle_Ptf,	
            'Monetaire',            	
            '',   	
            'Cash',                      	
            'Cash', 	
            'EUR',
            '',   	
            '',    	
            sum(v._netamountd),
            sum(v._netamountd)/o._actif	
from 	
          fcp.valoprdt v	
left outer join           com.produit p                                   on  p._codeprodui=v._assettype	
left outer join           fcp.vlrstion o on v._compte=o._compte and v._dateoperation=o._dateoperation	
left outer join           fcp.cpartfcp c                                   on  c._compte=v._compte	
left outer join           Fcp.CONSGRPE cg                      on cg._compte=v._compte	
left outer join           fcp.grpedeft g                                   on g._codegroupe=cg._codegroupe
 where 	
 g._codegroupe in ('op')	
 and	
 v._dateoperation=@date
 and
 v._assettype in (select distinct p._codeprodui from fcp.valoprdt v left outer join com.produit p on v._assettype=p._codeprodui	
 left outer join fcp.cpartfcp c on  c._compte=v._compte 
 left outer join Fcp.CONSGRPE cg on cg._compte=v._compte 
 left outer join fcp.grpedeft g on g._codegroupe=cg._codegroupe
  where  g._codegroupe in ('op') and v._dateoperation=@date and ((v._codesoussecteur='FT TRESO') or (p._codedutype in ('BT','CD')))) 
  
 group by g._NOMGROUPE, v._dateoperation,  v._compte, c._LIBELLECLI, o._actif)
 
 union
 (select 	
			5 as 'style',
            g._NOMGROUPE                      as Groupe,	
            convert(varchar,vc._dateoperation,103)                   as Dateinventaire,	
            vc._compte                                  as Compte, 	
            c._LIBELLECLI                          as Libelle_Ptf,	
            'Cash',            	
            '',   	
            'Cash',                      	
            'Cash', 	
            'EUR',
            '',   	
            '',     	
            sum(vc._netamounts),
            sum(vc._pourcents/100)
from 	
          fcp.valocash vc	
left outer join           fcp.cpartfcp c                                   on  c._compte=vc._compte	
left outer join           Fcp.CONSGRPE cg                      on cg._compte=vc._compte	
left outer join           fcp.grpedeft g                                   on g._codegroupe=cg._codegroupe
 where 	
 g._codegroupe in ('op')	
 and	
 vc._dateoperation=@date	
   
 group by g._NOMGROUPE, vc._dateoperation,  vc._compte, c._LIBELLECLI)
 
 
 
  