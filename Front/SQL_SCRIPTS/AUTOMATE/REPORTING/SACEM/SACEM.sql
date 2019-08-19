DECLARE @aaa float

DECLARE @date datetime
SET @date='29/06/2012'

select 'date', CAST(datepart(day,@date) as varchar)+'/'+ CAST(datepart(month,@date) as varchar)+'/'+CAST(datepart(year,@date) as varchar)

SET @aaa= (select sum(valeur_boursiere+coupon_couru) from ptf_transparise as tr
where tr.dateinventaire=@date
	and tr.compte in ('6300120','6300121','6300122')
	and numero_niveau=4
	and type_actif not like '%action%'
	)
	

select tr.groupe_rating, sum(valeur_boursiere+coupon_couru),sum(valeur_boursiere+coupon_couru)/@aaa from ptf_transparise as tr
left outer join [SECTEUR] as secteur on secteur.libelle=tr.Secteur
left outer join [SOUS_SECTEUR] as ssecteur on ssecteur.libelle=tr.sous_Secteur
where tr.dateinventaire=@date
	and tr.compte in ('6300120','6300121','6300122')
	and numero_niveau=4
	and type_actif not like '%action%'
	and (secteur.id in ( 'O CF ASSU','O CF BANQ','O CF SFIN','O CNF', 'O COVERED')
		or ssecteur.id='O AGENC NG')
group by tr.groupe_rating





/*** 2eme methode **/
/****Repartition Sectorielle emetteurs privés*****/
select tr.secteur, sum(valeur_boursiere+coupon_couru),sum(valeur_boursiere+coupon_couru)/@aaa from ptf_transparise as tr
left outer join [SECTEUR] as secteur on secteur.libelle=tr.Secteur
left outer join [SOUS_SECTEUR] as ssecteur on ssecteur.libelle=tr.sous_Secteur
where tr.dateinventaire=@date
	and tr.compte in ('6300120','6300121','6300122')
	and numero_niveau=4
	and type_actif not like '%action%'
	and (secteur.id in ( 'O AGENCIES','F OBLIG','O CF ASSU','O CF BANQ','O CF SFIN','O CNF', 'O COVERED')
		or ssecteur.id='O AGENC NG')
group by tr.secteur

select * from secteur


--/!\ Bon résultats mais trop de pays /!\
/****Repartition géographique dette souveraine et agences garantes d'état*****/
select tr.pays, sum(valeur_boursiere+coupon_couru),sum(valeur_boursiere+coupon_couru)/@aaa as pct_det from ptf_transparise as tr
left outer join [SOUS_SECTEUR] as ssecteur on ssecteur.libelle=tr.Sous_Secteur
where tr.dateinventaire=@date
	and tr.compte in ('6300120','6300121','6300122')
	and numero_niveau=4
	and type_actif not like '%action%'
	and ( ssecteur.id_secteur in ( 'O GVT') -- dette souveraine
	or ssecteur.id in ('O AGENCIES') )	 -- ou agencies garanties états
group by tr.pays
order by pct_det desc 


/**Actifs actions repartition geographique********************************************/
SET @aaa= (SELECT SUM(valeur_boursiere+coupon_couru) FROM ptf_transparise 
				where libelle_ptf like 'QUATREM RETRAITE ACTIONS' 
				and dateinventaire=@date
				and numero_niveau=4
				and zone_géo is not null)
				

select t.zone_géo, 
		sum(t.valeur_boursiere+coupon_couru)/@aaa
from ptf_transparise as t
where t.libelle_ptf like 'QUATREM RETRAITE ACTIONS' and t.dateinventaire=@date
	and numero_niveau=4
	and zone_géo is not null
group by t.zone_géo
order by t.zone_géo desc
/*************************************************************************************/



/**Actifs actions repartition sectorielle********************************************/
SET @aaa= (SELECT SUM(valeur_boursiere+coupon_couru) FROM ptf_transparise 
				where libelle_ptf like 'QUATREM RETRAITE ACTIONS' 
				and dateinventaire=@date
				and numero_niveau=4
				and secteur in (select libelle from SECTEUR where id like 'A %'))

select t.secteur, 
		sum(t.valeur_boursiere+coupon_couru)/@aaa as pct_det
from ptf_transparise as t
where t.libelle_ptf like 'QUATREM RETRAITE ACTIONS' and t.dateinventaire=@date
	and numero_niveau=4
    and t.secteur in (select libelle from SECTEUR where id like 'A %')
group by t.secteur
order by pct_det desc
/*********************************************************************************************/

