-- ATTENTION: sauver le fichier en encoding UTF-8 (avec Notepad++: Encoding) car il y a des caractères accentués

-- sortie pour l'exposition , données pour le reporting
-- renvoie l ensemble du contenu des mandats en transparence (par defaut niveau 3) 

declare @niveau_Trans tinyint
set @niveau_Trans = 4

declare @date_inventaire datetime
set @date_inventaire = '31/12/2012'

select  
convert(char,DateInventaire,103) as C01_Dateinventaire,
case 
when p.Compte like '0000%' then substring(p.Compte,5,len( p.Compte )-4)
when p.Compte like '000%' then substring(p.Compte,4,len( p.Compte )-3)
when p.Compte like '00%' then substring(p.Compte,3,len( p.Compte )-2)
when p.Compte like '0%' then substring(p.Compte,2,len( p.Compte)-1)
else p.Compte end as C02_Compte,
Libelle_Ptf as C04_Libelle_Ptf,
code_Titre as C05_code_Titre,
Libelle_titre as C07_Libelle_Titre,
Sum(isnull(Valeur_Boursiere,0)) as C12_Valeur_Boursiere,
Sum(isnull(Coupon_couru,0)) as C13_Coupon_Couru,
Type_produit as C18_Type_Produit,
Secteur as C23_Secteur,
sous_secteur as C24_Sous_Secteur,
isnull(pays,'') as C25_Pays,
ptf.Type_mandat as 'Type_de_mandat',
isnull(Zone_Géo,'') as 'Zone_Geo',
type_actif as 'Type_actif',
isnull(groupe_rating,'') as 'groupe_rating',
isnull(tranche_de_maturite,'') as 'Tranche_de_Maturite',
Sum( isnull(Valeur_Boursiere,0)+isnull(Coupon_couru,0) ) as 'VB_CC'
into #REPORTING_EXTRACTION
from PTF_TRANSPARISE as p
left outer join [ref].[portfolio] as ptf on ptf.Compte = p.Compte
where DateInventaire = @date_inventaire and numero_Niveau = @niveau_Trans
and Groupe <> 'OPCVM'
group by DateInventaire,p.Compte,Libelle_Ptf,code_Titre,Libelle_titre,Type_produit,
Secteur,sous_secteur,
ptf.Type_mandat,pays,Zone_Géo, type_actif,groupe_rating,tranche_de_maturite
order by C02_Compte, code_Titre

-- Modification , transco
-------------------------

-- suite à la modification du traitement des lignes de liquidité (cash et opcvm monétaire), 
-- le type_actif est mis comme le mandat (AS_PTF) au lieu de Monétaire
-- voir les tables  PTF_TYPE_ACTIF et PTF_CARAC_OPCVM  avec AS_PTF
update #REPORTING_EXTRACTION
set type_Actif = 'Monétaire'
from #REPORTING_EXTRACTION as e
left outer join PTF_CARAC_OPCVM as opcvm on opcvm.Libelle = e.C24_Sous_Secteur
where opcvm.Types = 'AS_PTF'
and e.C24_Sous_Secteur like '%TRESORERIE%'

update #REPORTING_EXTRACTION
set type_Actif = 'Monétaire'
from #REPORTING_EXTRACTION as e
left outer join PTF_TYPE_ACTIF as actif on actif.Produit = e.C18_Type_Produit
where actif.Types = 'AS_PTF'
and ( e.C18_Type_Produit like '%Monétaire%' or e.C18_Type_Produit = 'Cash' 
or e.C18_Type_Produit like 'FUTURES DEVISE %' )

-- fusion du compte 5300252 U.N.M.I OBLIGATIONS COURT TERME dans U.N.M.I OBLIGATIONS
update #REPORTING_EXTRACTION
set C02_Compte = '5300250'
where C02_Compte = '5300252'
-- le titre future sur dividendes eurostoxx de la fin année courante est traité comme une obligations en terme d expo
update #REPORTING_EXTRACTION
set 
Type_Actif = 'Obligations',
C23_Secteur = 'FONDS OBLIGATIONS',
C24_Sous_Secteur = 'OPCVM OBLIGATIONS EURO'
where  C05_code_Titre = 'DEDZ3'

update #REPORTING_EXTRACTION
set 
Zone_Geo = 'Amerique du nord'
where Zone_Geo = 'Amérique du nord'

select * from #REPORTING_EXTRACTION


