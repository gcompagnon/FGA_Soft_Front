
DECLARE @date datetime
SET @date='28/09/2012'

DECLARE @numero_niveau tinyint
SET @numero_niveau=4

DECLARE @aaa float

SELECT [Groupe]
      ,[Dateinventaire]
      ,[Compte]
      ,[ISIN_Ptf]
      ,[Libelle_Ptf]
      --,[code_Titre]
      ,[isin_titre]
      ,[Libelle_Titre]     
      ,([Valeur_Boursiere]+[Coupon_Couru]) as VB_totale      
      ,[Type_Produit]
      --,[Devise_Titre]
      ,[Secteur]
      ,[Sous_Secteur]
      ,[Pays]
      ,[Emetteur]
      ,[Rating]
      ,[Grp_Emetteur]
      --,[maturite]
      --,[duration]
      --,[sensibilite]
      --,[coursclose]
      --,[quantite]
      --,[pct_det_Niv_0]
      ,[Zone_Géo]
      ,[Type_actif]
      ,[Groupe_rating]     
      --,[Vie_residuelle]
      ,[Tranche_de_maturite]
       ,Segment=case
      when [Secteur]='EMPRUNTS D''ETAT' or [Sous_Secteur]='AGENCIES GARANTIES' then 'Souverain'
      when [Sous_Secteur]='AGENCIES NON GARANTIES' then 'Agences'
      when Left([Secteur],10) in ('CORPORATES','EMPRUNTS F','FONDS OBLI') then 'crédit'
      when [Type_actif]='Monétaire' and [Libelle_Ptf] not like '%action%' then 'Monétaire' else 'Actions' end,
      Type_taux=case
      when [Type_Produit] like '%Fixe%' then 'Fixe'
      when [Type_Produit] like '%Variable%' or [Type_Produit] like '%structuré%' then 'Variable'
      when [Type_Produit] like '%Indexé%' then 'Indexé'
      when [Type_actif]='Monétaire' and [Libelle_Ptf] not like '%action%' then 'Monétaire' else '' end
      
  into #temp FROM [E2DBFGA01].[dbo].[PTF_TRANSPARISE] WHERE [Groupe]='quatrem' and [Numero_Niveau]='4' and [Dateinventaire]='31/07/2012' and [Compte] in  ('6300120','6300121','6300122')


select 'date', CAST(datepart(day,@date) as varchar)+'/'+ CAST(datepart(month,@date) as varchar)+'/'+CAST(datepart(year,@date) as varchar)


declare @tot float
set @tot = (select sum(vb_totale) from #temp
where segment not in ('Actions'))


/**  Repartition par emetteur  ************************/
select segment, sum(VB_totale),sum(VB_totale)/@tot
from #temp
group by segment


/**  Repartition par type  ****************************************/
select type_taux, sum(VB_totale),sum(VB_totale)/(select sum(vb_totale)from #temp where type_taux not like '')
from #temp
where type_taux not like ''
group by type_taux

/**  Repartition rating emetteurs privés et agences non garanties  **********/
select groupe_rating, sum(vb_totale),sum(vb_totale)/@tot
from #temp
where segment in ('crédit', 'Agences')
group by groupe_rating


/**  Repartition sectorielle emetteurs privés et agences non garanties ************/
select secteur, sum(vb_totale),sum(vb_totale)/@tot
from #temp
where segment in ('crédit', 'Agences')
group by secteur

/**  Reparition Geo dette souveraine et garanties d'états  ***/
select pays, sum(vb_totale),sum(vb_totale)/@tot
from #temp
where segment='souverain'
group by pays

drop table #temp



/**Actifs actions repartition geographique********************************************/
SET @aaa= (SELECT SUM(valeur_boursiere+coupon_couru) FROM ptf_transparise 
				where compte in ('6300121')
					and type_actif='actions'
				and dateinventaire=@date
				and numero_niveau=@numero_niveau
				and zone_géo is not null)

select t.zone_géo, 
		sum(t.valeur_boursiere+coupon_couru)/@aaa
from ptf_transparise as t
where compte in ('6300121')
	and type_actif='actions'

	and t.dateinventaire=@date
	and numero_niveau=@numero_niveau
	and zone_géo is not null
group by t.zone_géo
order by t.zone_géo desc
/*************************************************************************************/



/**Actifs actions repartition sectorielle********************************************/
SET @aaa= (SELECT SUM(valeur_boursiere+coupon_couru) FROM ptf_transparise 
				where compte in ('6300121')
					and type_actif='actions'
				and dateinventaire=@date
				and numero_niveau=@numero_niveau
				and secteur in (select libelle from SECTEUR where id like 'A %'))

select t.secteur, 
		sum(t.valeur_boursiere+coupon_couru)/@aaa as pct_det
from ptf_transparise as t
where compte in ('6300121')
	and type_actif='actions'
	and t.dateinventaire=@date
	and numero_niveau=@numero_niveau
    and t.secteur in (select libelle from SECTEUR where id like 'A %')
group by t.secteur
order by pct_det desc
/************************************************************************************/




