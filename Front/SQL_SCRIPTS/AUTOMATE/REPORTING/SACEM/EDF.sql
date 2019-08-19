
DECLARE @date datetime
SET @date='28/09/2012'

DECLARE @numero_niveau tinyint
SET @numero_niveau=4

DECLARE @aaa float

SELECT [Groupe]
      ,[Dateinventaire]
      ,[Compte]
      --,[ISIN_Ptf]
      --,[Libelle_Ptf]
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
      --,[Rating]
      ,[Grp_Emetteur]
      --,[maturite],[duration],[sensibilite],[coursclose],[quantite],[pct_det_Niv_0]
      ,[Zone_Géo]
      ,[Type_actif]
      ,[Groupe_rating]     
      --,[Vie_residuelle]
      ,[Tranche_de_maturite]
       ,Segment=case
      when [Secteur]='EMPRUNTS D''ETAT' or [Sous_Secteur]='AGENCIES GARANTIES' then 'Souverain'
      when Left([Secteur],10) in ('CORPORATES','EMPRUNTS F','FONDS OBLI') or [Sous_Secteur] in ('AGENCIES NON GARANTIES')then 'crédit'
      when [Type_actif]='Monétaire' and [Libelle_Ptf] not like '%action%' then 'Monétaire' else 'Actions' end,
      Type_taux=case
      when [Type_Produit] like '%Fixe%' then 'Fixe'
      when [Type_Produit] like '%Variable%' or [Type_Produit] like '%structuré%' then 'Variable'
      when [Type_Produit] like '%Indexé%' then 'Indexé'
      when [Type_actif]='Monétaire' and [Libelle_Ptf] not like '%action%' then 'Monétaire' else '' end
      
  into #temp FROM [E2DBFGA01].[dbo].[PTF_TRANSPARISE] WHERE [Groupe]='quatrem' and [Numero_Niveau]=@numero_niveau and [Dateinventaire]=@date and [Compte] not in ('6300010','6300009') 


select 'date', CAST(datepart(day,@date) as varchar)+'/'+ CAST(datepart(month,@date) as varchar)+'/'+CAST(datepart(year,@date) as varchar)


declare @tot float
set @tot = (select sum(vb_totale) 
from #temp
where segment not in ('Actions'))


/**  Repartition par emetteur  ************************/
select segment+' '+col1,sum(VB),sum(pourcent)
from (select segment,
				case when segment='crédit' and secteur='AGENCIES' then 'AGENCIES NG'
					when segment='crédit' then 'prive'
					else ' '
				end as col1, 
				sum(VB_totale) as VB,
				sum(VB_totale)/@tot as pourcent
		from #temp
		group by segment,secteur) as tab
group by segment,col1




/**  Repartition par type  ****************************************/
select rtrim(type_taux), sum(VB_totale),sum(VB_totale)/(select sum(vb_totale)from #temp where type_taux not like '')
from #temp
where type_taux not like ''
group by type_taux

/**  Repartition rating emetteurs privés et agences non garanties  **********/
select groupe_rating, sum(vb_totale),sum(vb_totale)/@tot
from #temp
where segment='crédit'
group by groupe_rating


/**  Repartition sectorielle emetteurs privés et agences non garanties ************/
select rtrim(secteur), sum(vb_totale),sum(vb_totale)/@tot
from #temp
where segment='crédit'
group by secteur

/**  Reparition Geo dette souveraine et garanties d'états  ***/
select rtrim(pays), sum(vb_totale),sum(vb_totale)/@tot
from #temp
where segment='souverain'
group by pays



/**Actifs actions repartition geographique********************************************/
SET @tot= (SELECT SUM(VB_totale) FROM #temp
				where segment='Actions'
				and zone_géo is not null)

select t.zone_géo, 
		sum(VB_totale)/@tot
from #temp as t
where segment='Actions'
	and zone_géo is not null
group by t.zone_géo
order by t.zone_géo desc
/*************************************************************************************/



/**Actifs actions repartition sectorielle********************************************/
select t.secteur, 
		sum(VB_totale)/@tot
from #temp as t
where segment='Actions'
and secteur in ('FONDS ACTIONS','FONDS TRESORERIE','Liquidité')
group by t.secteur


SET @tot= (SELECT SUM(VB_totale) FROM #temp
				where segment='Actions'
				and secteur not in ('FONDS ACTIONS','FONDS TRESORERIE','Liquidité')
				and zone_géo is not null)
				
select t.secteur, 
		sum(VB_totale)/@tot
from #temp as t
where segment='Actions'
and secteur not in ('FONDS ACTIONS','FONDS TRESORERIE','Liquidité')
group by t.secteur
/************************************************************************************/







/** Exposition en fonds externe (lignes principales)********************************************/
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
/********************************************************************************/



/** Principales Lignes Obligataires**************************************************/
set @tot = (select sum(vb_totale) 
from #temp
where segment not in ('Actions'))

select top 15 emetteur, sum(valeur_boursiere+coupon_couru),sum(valeur_boursiere+coupon_couru)/@tot from ptf_fga
where dateinventaire=@date
and groupe='QUATREM'
and grp_emetteur not in (  select groupeEmetteur
  FROM [E2DBFGA01].[dbo].[EMETTEUR_OMEGA]
  where natureEmetteur='SGP')
group by emetteur
order by sum(valeur_boursiere+coupon_couru) desc
/*******************************************************************************/


/**  Exposition Espagne et Portugal   *****************/
select left(pays,3),emetteur,sum(valeur_boursiere),sum(valeur_boursiere)/@tot from ptf_fga
where dateinventaire=@date
and groupe='QUATREM'
and pays in ('ESPAGNE','PORTUGAL')
group by pays,emetteur
order by pays, emetteur


drop table #temp
