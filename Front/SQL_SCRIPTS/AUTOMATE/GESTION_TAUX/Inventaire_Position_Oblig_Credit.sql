-- requete de sortie des positions en obligations 
-- secteurs: corp agencies, emprunt

--drop table #DERNIERE_OPE_COMPTES
--drop table #RATINGDATE
--drop table #INVENTAIRE
--drop table #RATING

select max(vvv._dateoperation) as 'date',
 vvv._compte as 'compte'
into #DERNIERE_OPE_COMPTES
from fcp.valoprdt vvv  
where vvv._dateoperation<=getdate() and vvv._dateoperation>=(getdate()-50)
and vvv._compte<>('6300150')
group by vvv._compte


SELECT     
    rtrim(g._NOMGROUPE)                as C00_Groupe,    
    rtrim(convert(char,v._dateoperation,103)) as C01_Date,    
    rtrim(v._compte)                   as C02_Compte,     
    rtrim(isnull(c._username,''))      as C03_ISIN_Ptf,    
    rtrim(c._LIBELLECLI)               as C04_Libelle_Ptf,    
    rtrim(v._assettype)                as C05_code_Titre,    
    rtrim(p._isin)                     as C06_isin_titre,       
    rtrim(p._libelle1prod)             as C07_Libelle_Titre,     
    isnull(v._netamountD,0)            as C12_Valeur_Boursiere,         
    isnull(v._netamountDcc,0)          as C13_Coupon_Couru,     
    isnull(v._netentrypriceD,0)        as C14_Valeur_Comptable,      
    isnull(v._netentrypriceDcc,0)      as C15_Coupon_Couru_Comptable,       
    isnull(v._unlossD,0)               as C16_PMV,       
    isnull(v._unlossDcc,0)             as C17_PMV_CC,     
    rtrim(v._libelletypeproduit)       as C18_Type_Produit,      
    v._deviseS                         as C19_Devise_Titre,
    rtrim(s._LIBELLESECTEUR)           as C23_Secteur,
    rtrim(ss._LIBELLESOUSSECTEUR)      as C24_Sous_Secteur,    
    rtrim(isnull(pi._LIBELLEPAYS,''))  as C25_Pays,      
    rtrim(e._NOMEMETTEUR)              as C26_Emetteur,    
    rtrim(GR._libellegrper)            as C27_Grp_Emetteur,    
    rtrim(convert(char,p._echeance,103))as C28_maturite,    
    isnull(h._duration,0)              as C29_duration,    
    isnull(h._coursclose,0)            as C30_Cours_Close,    
    v._quantite*p._nominal             as C31_QuantiteNominal
    --r._signature                       as C27_Rating,    
into #INVENTAIRE    
FROM     
    fcp.valoprdt  v    
left outer join           com.produit p         on p._codeprodui=v._assettype    
left outer join    com.prdtclasspys ps         on p._codeprodui=ps._codeprodui and ps._classification=0    
left outer join           com.soussect ss       on ss._CODESOUSSECTEUR=ps._codesoussecteur    
left outer join    com.ssectclass ss_s           on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0    
left outer join           com.secteurs s        on s._CODESECTEUR=ss_s._codesecteur    
--left outer join           com.rating r   on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where r._date<=v._dateoperation and r._codeprodui=p._codeprodui)    
left outer join           com.prixhist  h       on v._datecours=h._date and p._codeprodui=h._codeprodui    
left outer join           com.emetteur e        on e._emetteur=p._emetteur    
left outer join           com.grpemetteursratios GR     on  e._grper=GR._codegrper    
left outer join           com.pays pi           on pi._CODEPAYS= gr._codepays    
left outer join           fcp.cpartfcp c        on  c._compte=v._compte    
left outer join           Fcp.CONSGRPE cg       on cg._compte=v._compte    
left outer join           fcp.grpedeft g        on g._codegroupe=cg._codegroupe    
left outer join           #DERNIERE_OPE_COMPTES lastOpe    on lastOpe.compte = c._compte
WHERE       
    v._dateoperation= lastOpe.date
and g._codegroupe in ('M','OP')
and left(s._LIBELLESECTEUR,4) in ('CORP','AGEN','EMPR') 
and v._compte<>('6300150')    
and s._LIBELLESECTEUR <> 'EMPRUNTS D''ETAT'
order by      
    g._NOMGROUPE ,  v._compte,  p._isin

-- RATING ensuite
select 
    max(r._date) as C33_DateRating,
    r._codeprodui as C05_code_Titre
into #RATINGDATE
from #INVENTAIRE as i
left outer join com.rating as r on r._codeprodui=i.C05_code_Titre and r._codeagence='INT' and r._date<=i.C01_Date
group by  r._codeprodui

select 
    r._signature as C32_Rating,
    r._date as C33_DateRating,
    r._codeprodui as C05_code_Titre
into #RATING
from #RATINGDATE as i
left outer join com.rating as r on r._codeprodui=i.C05_code_Titre and r._codeagence='INT' and r._date=i.C33_DateRating

-- SORTIE
select i.*, rtrim(r.C32_Rating)as C32_Rating,  rtrim(convert(char,r.C33_DateRating,103))  as C33_DateRating
from #INVENTAIRE as i
left outer join #RATING as r on r.C05_code_Titre = i.C05_code_Titre


--SELECT     
--    g._NOMGROUPE              as C00_Groupe,    
--    convert(char,v._dateoperation,101) as C01_Date,    
--    v._compte                  as C02_Compte,     
--    c._username              as C03_ISIN_Ptf,    
--    c._LIBELLECLI          as C04_Libelle_Ptf,    
--    v._assettype              as C05_code_Titre,    
--    p._isin                   as C06_isin_titre,       
--    p._libelle1prod           as C07_Libelle_Titre,     
--    v._netamountM          as C12_Valeur_Boursiere,         
--    v._netamountMcc              as C13_Coupon_Couru,     
--    v._netentrypriceM              as C14_Valeur_Comptable,      
--           v._netentrypriceMcc           as C15_Coupon_Couru_Comptable,       
--    v._unlossM                as C16_PMV,       
--    v._unlossMcc            as C17_PMV_CC,     
--    v._libelletypeproduit           as C18_Type_Produit,      
--    v._deviseS                as C19_Devise_Titre,      
--    s._LIBELLESECTEUR               as C23_Secteur,      
--    ss._LIBELLESOUSSECTEUR         as C24_Sous_Secteur,     
--    pi._LIBELLEPAYS        as C25_Pays,      
--    e._NOMEMETTEUR          as C26_Emetteur,    
--    r._signature             as C27_Rating,    
--          GR._libellegrper as C29_Grp_Emetteur,    
--          convert(char,p._echeance,101) as C29_maturite,    
--          h._duration as duration,    
--           h._coursclose,    
--           v._quantite*p._nominal AS QUANTITE    
--FROM     
--          fcp.valoprdt  v    
--left outer join           com.produit p         on p._codeprodui=v._assettype    
--left outer join    com.prdtclasspys ps         on p._codeprodui=ps._codeprodui and ps._classification=0    
--left outer join           com.soussect ss       on ss._CODESOUSSECTEUR=ps._codesoussecteur    
--left outer join    com.ssectclass ss_s           on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0    
--left outer join           com.secteurs s        on s._CODESECTEUR=ss_s._codesecteur    
--left outer join           com.rating r   on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where r._date<=v._dateoperation and r._codeprodui=p._codeprodui)    
--left outer join           com.prixhist  h       on v._datecours=h._date and p._codeprodui=h._codeprodui    
--left outer join           com.emetteur e        on e._emetteur=p._emetteur    
--left outer join           com.grpemetteursratios GR     on  e._grper=GR._codegrper    
--left outer join           com.pays pi           on pi._CODEPAYS= gr._codepays    
--left outer join           fcp.cpartfcp c        on  c._compte=v._compte    
--left outer join           Fcp.CONSGRPE cg       on cg._compte=v._compte    
--left outer join           fcp.grpedeft g        on g._codegroupe=cg._codegroupe    
--WHERE       
--         v._dateoperation= 
--and      g._codegroupe in ('M','OP')
--and left(s._LIBELLESECTEUR,4) in ('CORP','AGEN','EMPR')    
--and     v._compte<>('6300150')    
--order by      
--         g._NOMGROUPE ,  v._compte,  p._isin