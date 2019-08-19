--Publication des inventaires pour une date et un comptes/ptf
declare @dateInventaire as datetime
set  @dateInventaire = '31/12/2012'

declare @Compte as varchar(60)
set @Compte = '3303106'

-- pour le cas général (non cash et non fonds monétaires)
select 
  Dateinventaire as [date], 
  Libelle_Ptf as name,
  'FEDERIS' as [portfolio source],
  Compte as [portfolio code],
  0 as [bank code],	
  0 as [branch code],
  Compte as [account],
  p.devise_titre as [base currency],
  rtrim(j.type_jump) as tmp_type,
  rtrim(j.type_jump) as [type],
  case p.type_produit
  when 'Cash' then 'EUR'
  else Libelle_Titre end as [asset],
  Devise_Titre as [currency],
  'no_place' as [market place],
  '' as [telekurs code],
  case p.type_produit
  when 'Cash' then ''
  else isin_titre end as [Isin code],
  '' as [cusip code],
  '' as	[Bloomberg code],
  '' as	[Beauchamp code],
  '' as [Local Code],    
  ROUND(quantite,2) as position,
  Valeur_Boursiere + Coupon_Couru as [base amount],  
  CASE 
  when ISNULL(quantite,0) = 0 then 0
  else  
  ROUND((ISNULL(Valeur_Comptable,0) + ISNULL(Coupon_Couru_Comptable,0))/quantite,3)   
  end as [average absorption cost],
  0 as [base PL],
  1 as [change rate],
  0 as [line PL],
  Valeur_Boursiere + Coupon_Couru as [line amount]
   
 FROM PTF_FGA p, JUMP_OMEGA_TYPE j where dateinventaire=@dateInventaire  and p.type_produit=j.type_produit
  and p.Compte = @compte
  and  p.type_produit <> 'Cash'
  and  p.sous_secteur <> 'OPCVM TRESORERIE'     
  
 UNION
 -- pour les lignes de cash: des pseudo asset
 select 
  Dateinventaire as [date], 
  Libelle_Ptf as name,
  'FEDERIS' as [portfolio source],
  p.Compte as [portfolio code],
  0 as [bank code],	
  0 as [branch code],
  p.Compte as [account],
  'EUR' as [base currency],
'OTHER' as tmp_type,
  'OTHER' as [type],
  case 
  when mandat.type_mandat = 'A' then 'CASH_STOCK'
  when mandat.type_mandat = 'D' then 'CASH_BOND'
  when mandat.type_mandat = 'O' then 'CASH_BOND'
  else 'CASH_BOND' end as [asset],
    Devise_Titre as [currency],
  'no_place' as [market place],
  '' as [telekurs code],
  case 
  when mandat.type_mandat = 'A' then 'CASH_STOCK'
  when mandat.type_mandat = 'D' then 'CASH_BOND'
  when mandat.type_mandat = 'O' then 'CASH_BOND'  
  else 'CASH_BOND' end as [Isin code],  
  '' as [cusip code],
  '' as	[Bloomberg code],
  '' as	[Beauchamp code],
  '' as [Local Code],    
    ROUND(Valeur_Boursiere + Coupon_Couru,2) as position,
  Valeur_Boursiere + Coupon_Couru as [base amount],
  CASE 
  when ISNULL(quantite,0) = 0 then 0
  else  
  ROUND((ISNULL(Valeur_Comptable,0) + ISNULL(Coupon_Couru_Comptable,0))/quantite,3)  
  end as [average absorption cost],
  0 as [base PL],
  1 as [change rate],
  0 as [line PL],
  Valeur_Boursiere + Coupon_Couru as [line amount]     
 FROM PTF_FGA p
 LEFT OUTER JOIN ref.PORTFOLIO mandat ON mandat.Compte = p.Compte -- Pour récupérer le type du mandat
  where dateinventaire=@dateInventaire 
  and p.Compte = @compte
  and  p.type_produit = 'Cash'
  UNION
 -- pour les lignes de OPCVM TRESORERIE: 2 ISIN possible: l'un en allocation Obligataire, l'autre en allocation Actions
 select 
  Dateinventaire as [date], 
  Libelle_Ptf as name,
  'FEDERIS' as [portfolio source],
  p.Compte as [portfolio code],
  0 as [bank code],	
  0 as [branch code],
  p.Compte as [account],
  'EUR' as [base currency],
  'FUND' as tmp_type,
  'FUND' as [type],  
  case 
  when mandat.type_mandat = 'A' then Libelle_Titre+'_STOCK'
  when mandat.type_mandat = 'D' then Libelle_Titre
  when mandat.type_mandat = 'O' then Libelle_Titre
  else Libelle_Titre end as [asset],
  Devise_Titre as [currency],
  'no_place' as [market place],
  '' as [telekurs code],
  case 
  when mandat.type_mandat = 'A' then isin_titre+'_S'
  when mandat.type_mandat = 'D' then isin_titre
  when mandat.type_mandat = 'O' then isin_titre
  else isin_titre end as [Isin code],
  '' as [cusip code],
  '' as	[Bloomberg code],
  '' as	[Beauchamp code],
  '' as [Local Code],
  ROUND(Valeur_Boursiere + Coupon_Couru,2) as position,
  Valeur_Boursiere + Coupon_Couru as [base amount],
  CASE 
  when ISNULL(quantite,0) = 0 then 0
  else  
  ROUND((ISNULL(Valeur_Comptable,0) + ISNULL(Coupon_Couru_Comptable,0))/quantite,3)  
  end as [average absorption cost],
  0 as [base PL],
  1 as [change rate],
  0 as [line PL],
  Valeur_Boursiere + Coupon_Couru as [line amount]   
 FROM PTF_FGA p
  LEFT OUTER JOIN ref.PORTFOLIO mandat ON mandat.Compte = p.Compte -- Pour récupérer le type du mandat
 where dateinventaire=@dateInventaire
 and p.Compte = @compte
 and  p.Sous_Secteur = 'OPCVM TRESORERIE'
 and  p.type_produit <> 'Cash'
    
 /*******************************************************************************************
 select distinct p.type_produit into #tmp_toto from ptf_transparise p
 
 select * from #tmp_toto where type_produit in (select type_produit from JUMP_OMEGA_TYPE)
 delete from #tmp_toto where type_produit in (select type_produit from JUMP_OMEGA_TYPE) 
 
 
 *****************************************************/
 
 