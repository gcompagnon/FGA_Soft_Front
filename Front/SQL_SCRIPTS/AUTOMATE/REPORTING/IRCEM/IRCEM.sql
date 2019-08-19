declare @date datetime
set @date = '31/07/2012'

--fonds dédiés diversifiés
Select * from ptf_fga
	where dateinventaire=@date
	and compte in ('3020010','3020011','3020012')
	and isin_titre in ('FR0007056742','FR0010291211','FR0010317784','FR0010561787','FR0010509992','FR0010605907','FR0010510008')
	
--Fonds actions
Select 'Fonds Actions', SUM(valeur_boursiere) from ptf_fga
	where dateinventaire=@date
	and compte in ('3020010','3020011','3020012')
	and type_produit NOT LIKE 'Obligations structur_es'
	and secteur = 'FONDS ACTIONS'
	
--Obligations et fonds taux
Select 'Obligations', SUM(valeur_boursiere+coupon_couru) from ptf_fga 
	where dateinventaire=@date
	and compte in ('3020010','3020011','3020012')
	and ((secteur ='FONDS OBLIGATIONS' OR type_produit LIKE 'Oblig%') AND type_produit NOT LIKE 'Obligations Structur_es')
	

--fonds structures
Select 'Fonds Structures', SUM(valeur_boursiere + coupon_couru) from ptf_fga 
	where dateinventaire=@date
	and compte in ('3020010','3020011','3020012')
	and type_produit LIKE 'Obligations structur_es'
	
--Total mandat FGA
Select 'Total Mandat' ,SUM(AN) from ptf_an
	where date=@date
	and compte in ('3020010','3020011','3020012')
	
--monetaire
Select 'Monetaire' ,SUM(valeur_boursiere) from ptf_fga
	where dateinventaire=@date
	and compte in ('3020010','3020011','3020012')
	and secteur ='FONDS TRESORERIE'
	
--décomposition en transparence selon Action/oblig/monetaire
--dans une table temporaire pour aciliter la recherche dans le fichier excel par la suite
create table #temp (Isin_origine_Niv_1 varchar(25), type_actif varchar(25), valeur float)
insert into #temp
Select Isin_origine_Niv_1, type_actif, SUM(valeur_boursiere+coupon_couru) from ptf_transparise
	where dateinventaire=@date
	and numero_niveau=4
	and compte in ('3020010','3020011','3020012')
	and Isin_origine_Niv_1 IN ('FR0007056742','FR0010291211')
	group by type_actif, Isin_origine_Niv_1

select Isin_origine_Niv_1+' '+type_actif, valeur from #temp

drop table #temp
