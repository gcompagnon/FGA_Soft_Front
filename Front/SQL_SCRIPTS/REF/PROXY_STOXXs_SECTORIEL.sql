
declare @dateInventaire datetime
set @dateInventaire = '29/08/2014'



select
  p.Date,
  p.Code_Proxy +'_' + replace(left(p.sous_secteur, 15), ' ','_') as code_proxy,
  Libelle_Proxy+' - ' + p.sous_secteur as Libelle_proxy,
  code_titre,
  Libelle_Titre,
  Poids_VB/ts.somme_pds_sect as Poids_VB,
  Poids_CC,
  Type_Produit,
  Devise_Titre,
  Secteur,
  p.Sous_Secteur,
  Pays,
  Emetteur,
  Rating,
  Groupe_Emet,
  Maturité,
  Duration,
  Sensibilite
  into #SXXP_SECT
from
  dbo.PTF_PROXY p,
  (select sous_secteur, sum(poids_VB) as somme_pds_sect, code_proxy  from dbo.PTF_PROXY where date=@dateInventaire and code_proxy in('SXXE', 'SXXP')  group by sous_secteur, code_proxy ) ts
where
    date=@dateInventaire  
and p.code_proxy = ts.code_proxy --DJS 600 Pr
and ts.sous_secteur=p.sous_secteur
order by
  p.sous_secteur
  
  
  update #SXXP_SECT
  set code_proxy ='SXXP_AUTOM'
  where Libelle_proxy ='DJS 600 Pr - AUTOMOBILE'

  update #SXXP_SECT
  set code_proxy ='SXXP_BANQU'
  where Libelle_proxy ='DJS 600 Pr - BANQUES'
  
  update #SXXP_SECT
  set code_proxy ='SXXP_PETRO'
  where Libelle_proxy ='DJS 600 Pr - PETROLE ET GAZ'
	
  update #SXXP_SECT
  set code_proxy ='SXXP_PRODU'
  where Libelle_proxy ='DJS 600 Pr - PRODUITS DE BASE'

	
  update #SXXP_SECT
  set code_proxy ='SXXP_SANTE'
  where Libelle_proxy ='DJS 600 Pr - SANTE'	
	
  update #SXXP_SECT
  set code_proxy ='SXXP_SERVI'
  where Libelle_proxy ='DJS 600 Pr - SERVICES AUX COLLECTIVITES'		
	
  update #SXXP_SECT
  set code_proxy ='SXXP_TELEC'
  where Libelle_proxy ='DJS 600 Pr - TELECOMMUNICATIONS'		
	
	
  update #SXXP_SECT
  set code_proxy ='SXXP_ASSUR'
  where Libelle_proxy ='DJS 600 Pr - ASSURANCE'		
  	
	
  update #SXXP_SECT
  set code_proxy ='SXXP_VOYAG'
  where Libelle_proxy ='DJS 600 Pr - VOYAGES ET LOISIRS'			


  update #SXXP_SECT
  set code_proxy ='SXXP_ALIME'
  where Libelle_proxy ='DJS 600 Pr - ALIMENTAIRE - BOISSON'			


  update #SXXP_SECT
  set code_proxy ='SXXP_CONSO'
  where Libelle_proxy ='DJS 600 Pr - B&S DE CONSOMMATION'			

update #SXXP_SECT
  set code_proxy ='SXXP_INDUS'
  where Libelle_proxy ='DJS 600 Pr - B&S INDUSTRIELS'

update #SXXP_SECT
  set code_proxy ='SXXP_CONSTR'
  where Libelle_proxy ='DJS 600 Pr - CONSTRUCTION'

update #SXXP_SECT
  set code_proxy ='SXXP_FINANC'
  where Libelle_proxy ='DJS 600 Pr - SERVICES FINANCIERS'


  update #SXXP_SECT
  set code_proxy ='SXXE_AUTOM'
  where Libelle_proxy ='DJES Pr - AUTOMOBILE'

  update #SXXP_SECT
  set code_proxy ='SXXE_BANQU'
  where Libelle_proxy ='DJES Pr - BANQUES'
  
  update #SXXP_SECT
  set code_proxy ='SXXE_PETRO'
  where Libelle_proxy ='DJES Pr - PETROLE ET GAZ'
	
  update #SXXP_SECT
  set code_proxy ='SXXE_PRODU'
  where Libelle_proxy ='DJES Pr - PRODUITS DE BASE'

	
  update #SXXP_SECT
  set code_proxy ='SXXE_SANTE'
  where Libelle_proxy ='DJES Pr - SANTE'	
	
  update #SXXP_SECT
  set code_proxy ='SXXE_SERVI'
  where Libelle_proxy ='DJES Pr - SERVICES AUX COLLECTIVITES'		
	
  update #SXXP_SECT
  set code_proxy ='SXXE_TELEC'
  where Libelle_proxy ='DJES Pr - TELECOMMUNICATIONS'		
	
	
  update #SXXP_SECT
  set code_proxy ='SXXE_ASSUR'
  where Libelle_proxy ='DJES Pr - ASSURANCE'		
  	
	
  update #SXXP_SECT
  set code_proxy ='SXXE_VOYAG'
  where Libelle_proxy ='DJES Pr - VOYAGES ET LOISIRS'			


  update #SXXP_SECT
  set code_proxy ='SXXE_ALIME'
  where Libelle_proxy ='DJES Pr - ALIMENTAIRE - BOISSON'			


  update #SXXP_SECT
  set code_proxy ='SXXE_CONSO'
  where Libelle_proxy ='DJES Pr - B&S DE CONSOMMATION'			

update #SXXP_SECT
  set code_proxy ='SXXE_INDUS'
  where Libelle_proxy ='DJES Pr - B&S INDUSTRIELS'

update #SXXP_SECT
  set code_proxy ='SXXE_CONSTR'
  where Libelle_proxy ='DJES Pr - CONSTRUCTION'

update #SXXP_SECT
  set code_proxy ='SXXE_FINANC'
  where Libelle_proxy ='DJES Pr - SERVICES FINANCIERS'

insert into dbo.PTF_PROXY  
  select * from #SXXP_SECT


--insert into dbo.PTF_PROXY  
--  select * from #SXXP_SECT where code_proxy not in ('SXXE_BANQU')
  
--  select * from dbo.PTF_PROXY 
--  where Code_Proxy in (select Code_Proxy from #SXXP_SECT) and DATE = '31/07/2014' 
drop table #SXXP_SECT