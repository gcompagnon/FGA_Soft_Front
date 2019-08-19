DECLARE @aaa float

DECLARE @date datetime
SET @date='31/07/2012'

DECLARE @numero_niveau tinyint
SET @numero_niveau=4

select 'date', CAST(datepart(day,@date) as varchar)+'/'+ CAST(datepart(month,@date) as varchar)+'/'+CAST(datepart(year,@date) as varchar)

SET @aaa= (select sum(valeur_boursiere+coupon_couru) from ptf_transparise as tr
where tr.dateinventaire=@date
and tr.compte in ('6300110','6300111','6300112','6300120','6300121','6300122','6300130','6300131','6300132','6300140','6300141')
	and numero_niveau=@numero_niveau
	and type_actif not like '%action%'
	)
	

select tr.groupe_rating, sum(valeur_boursiere+coupon_couru),sum(valeur_boursiere+coupon_couru)/@aaa from ptf_transparise as tr
left outer join [SECTEUR] as secteur on secteur.libelle=tr.Secteur
left outer join [SOUS_SECTEUR] as ssecteur on ssecteur.libelle=tr.sous_Secteur
where tr.dateinventaire=@date
	and tr.compte in ('6300110','6300111','6300112','6300120','6300121','6300122','6300130','6300131','6300132','6300140','6300141')
	and numero_niveau=@numero_niveau
	and type_actif not like '%action%'
	and (secteur.id in ( 'O CF ASSU','O CF BANQ','O CF SFIN','O CNF', 'O COVERED')
		or ssecteur.id='O AGENC NG')
group by tr.groupe_rating



/****Repartition Sectorielle emetteurs privés*****/
select tr.secteur, sum(valeur_boursiere+coupon_couru),sum(valeur_boursiere+coupon_couru)/@aaa from ptf_transparise as tr
left outer join [SECTEUR] as secteur on secteur.libelle=tr.Secteur
left outer join [SOUS_SECTEUR] as ssecteur on ssecteur.libelle=tr.sous_Secteur
where tr.dateinventaire=@date
and tr.compte in ('6300110','6300111','6300112','6300120','6300121','6300122','6300130','6300131','6300132','6300140','6300141')
	and numero_niveau=@numero_niveau
	and type_actif not like '%action%'
	and (secteur.id in ( 'O CF ASSU','O CF BANQ','O CF SFIN','O CNF', 'O COVERED')
		or ssecteur.id in ('O AGENC NG'))
group by tr.secteur



--/!\ Bon résultats mais trop de pays /!\
/****Repartition géographique dette souveraine et agences garantes d'état*****/
select tr.pays, sum(valeur_boursiere+coupon_couru),sum(valeur_boursiere+coupon_couru)/@aaa as pct_det from ptf_transparise as tr
left outer join [SOUS_SECTEUR] as ssecteur on ssecteur.libelle=tr.Sous_Secteur
where tr.dateinventaire=@date
and tr.compte in ('6300110','6300111','6300112','6300120','6300121','6300122','6300130','6300131','6300132','6300140','6300141')
	and numero_niveau=@numero_niveau
	and type_actif not like '%action%'
	and ( ssecteur.id_secteur in ( 'O GVT') -- dette souveraine
	or ssecteur.id in ('O AGENCIES') )	 -- ou agencies garanties états
group by tr.pays
order by pct_det desc 


/**Actifs actions repartition geographique********************************************/
SET @aaa= (SELECT SUM(valeur_boursiere+coupon_couru) FROM ptf_transparise 
				where compte in ('6300111','6300121','6300131','6300141')
					and type_actif='actions'
				--libelle_ptf like 'QUATREM RETRAITE ACTIONS' 
				and dateinventaire=@date
				and numero_niveau=@numero_niveau
				and zone_géo is not null)
				

select t.zone_géo, 
		sum(t.valeur_boursiere+coupon_couru)/@aaa
from ptf_transparise as t
where compte in ('6300010','6300111','6300121','6300131','6300141')
	and type_actif='actions'
	--t.libelle_ptf like 'QUATREM RETRAITE ACTIONS' 
	and t.dateinventaire=@date
	and numero_niveau=@numero_niveau
	and zone_géo is not null
group by t.zone_géo
order by t.zone_géo desc
/*************************************************************************************/



/**Actifs actions repartition sectorielle********************************************/
SET @aaa= (SELECT SUM(valeur_boursiere+coupon_couru) FROM ptf_transparise 
				where compte in ('6300111','6300121','6300131','6300141')
					and type_actif='actions'
				--libelle_ptf like 'QUATREM RETRAITE ACTIONS' 
				and dateinventaire=@date
				and numero_niveau=@numero_niveau
				and secteur in (select libelle from SECTEUR where id like 'A %'))

select t.secteur, 
		sum(t.valeur_boursiere+coupon_couru)/@aaa as pct_det
from ptf_transparise as t
where compte in ('6300111','6300121','6300131','6300141')
	and type_actif='actions'
	--t.libelle_ptf like 'QUATREM RETRAITE ACTIONS' 
	and t.dateinventaire=@date
	and numero_niveau=@numero_niveau
    and t.secteur in (select libelle from SECTEUR where id like 'A %')
group by t.secteur
order by pct_det desc
/************************************************************************************/

/** Exposition en fonds externe ********************************************/
SET @aaa= (SELECT SUM(valeur_boursiere+coupon_couru) FROM ptf_fga
				where groupe='QUATREM'
				and dateinventaire=@date
				and secteur in (select libelle from SECTEUR where id in ('F ACTIONS','F DIVERS','F OBLIG') ) )
				

select t.isin_titre, t.libelle_titre,
        sum(Valeur_Boursiere+coupon_couru)/1000000 as VM,
        sum(PMV)/1000000 as PMV,
		sum(t.valeur_boursiere)/@aaa as pct_det
from ptf_fga as t
where t.groupe='QUATREM' and t.dateinventaire=@date
    and t.secteur in (select libelle from SECTEUR where id in ('F ACTIONS','F DIVERS','F OBLIG') )
group by t.isin_titre, t.libelle_titre
order by pct_det desc
/*********************************************************************************************/



/** Principales Lignes Obligataires************************************************************************/
SET @aaa= (select sum(valeur_boursiere+coupon_couru) from ptf_fga as tr
where tr.dateinventaire=@date
	and tr.groupe='QUATREM'
	)
	
select top 15 emetteur, sum(valeur_boursiere+coupon_couru),sum(valeur_boursiere+coupon_couru)/@aaa from ptf_fga
where dateinventaire=@date
and groupe='QUATREM'
group by emetteur
order by sum(valeur_boursiere+coupon_couru) desc
/*******************************************************************************/



select left(pays,3),emetteur,sum(valeur_boursiere),sum(valeur_boursiere)/@aaa from ptf_fga
where dateinventaire=@date
and groupe='QUATREM'
and pays in ('ESPAGNE','PORTUGAL')
group by pays,emetteur
order by pays, emetteur