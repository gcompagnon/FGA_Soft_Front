-- requete de construction de la hiérarchie Groupe / Compte

use OMEGA
GO
select  
      G._codegroupe, d._nomgroupe,
      C._compte as C02_compte, 
       C._libellecli as C01_libellé,          
       V. _actif as C03_actif,
	V._dateoperation as C04_date  
from fcp.CPARTFCP C
left outer join fcp.VLRSTION V on C._compte = V._compte
left outer join fcp.CONSGRPE G on C._compte = G._compte
left outer join fcp.grpedeft d on d._codegroupe=g._codegroupe        
where 
         V._dateoperation='30/09/2011' 
and C._compte IN (select _compte from fcp.CONSGRPE where _codegroupe = '01' ) -- le groupe des portefeuilles actifs
and G._codegroupe <> '01'
--and  G._codegroupe IN ( 'P2', 'P4', 'EP', 'MM' )
Order by  d._nomgroupe, C._libellecli


--G._codegroupe in ('IG','IO','UM','IN','CT','CA','MM','sa','SD','QX','GA','A0','UN','MN','SM')






--select d._codegroupe,_nomgroupe, _compte, d.* from  fcp.CONSGRPE G
--left outer join           fcp.grpedeft d               on d._codegroupe=g._codegroupe
--order by d._codegroupe


