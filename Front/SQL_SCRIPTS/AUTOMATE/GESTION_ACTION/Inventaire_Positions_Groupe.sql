-----------------------------------------------------
-- requete de sortie des positions en temps réel
--  sur la base omega
-----------------------------------------------------
-- parametres: 
--   la liste des groupes
-- 1ere version: 1 group
-----------------------------------------------------

--declare @group table
--(
--  Groupe varchar(25)
--)
--insert into @group(Groupe)  Values ('M')

declare @groupe varchar(25)
set @groupe='OP'


--  la date de valorisation est à la date du jour
declare @value_date DATETIME
set @value_date = ( select CAST( FLOOR( CAST( GETDATE() AS FLOAT ) )AS DATETIME) )
/*
Paramètres en entrée
@ai_CODEGROUPE	Varchar(10)	Code groupe de fonds.
@ai_Date	Varchar(10)	Date de calcul format dd/mm/yyyy.
@ai_NbreJourArriere	Smallint	Nombre de jours arrière.
@ai_ClotOuv	Bit	0-Cours clôture 1-Cours ouverture.
@ai_Bfcp	Bit	0-Version gestion de portefeuille 1-Version gestion de fonds commun de placement.
@ai_DateOpVal	Bit	0-Date opération 1-Date Valeur.
Résultat en sortie
_COMPTE	Varchar(10)	Code du fonds
_LIBELLECLI	Varchar(50)	Libellé du fonds
_CODEPRODUI	Varchar(15)	Code du Produit.
_LIBELLE1PROD	Varchar(100)	Libellé1 du produit
_LIBELLE2PROD	Varchar(100)	Libellé2 du produit.
_Position	Decimal(23,6)	Position.
_Position_Nominal	Decimal(27,6)	Position/Nominal
_PRIXMARCHE	Decimal(23,6)	Prix du marché.
_Valorisation	Decimal(23,6)	Valorisation
_ISIN	Char(12)	Code ISIN du produit.
_CODEDEVISE	Varchar(3)	Code devises.
_DATEDERNIERCOURS	Datetime	Date dernier cours.
*/


SELECT * 
into #POSITIONS
FROM App.omg_fnt_FEPoOuGrpe(@groupe,@value_date,15, 0, 1, 0) order by _compte



-- requete de sortie des positions en obligations 
-- secteurs: corp agencies, emprunt


select t.name as 'table',c.name as 'column',y.name as 'type', c.max_length
from sys.columns as c
left outer join sys.tables as t on t.object_id = c.object_id -- and t.type ='U' -- les tables user table
left outer join sys.types as y on y.system_type_id = c.system_type_id 
where c.name like '%strategie%'




select max(vvv._dateoperation) as 'date',
 vvv._compte as 'compte'
into #DERNIERE_VALO_COMPTES
from fcp.valoprdt vvv  
where vvv._dateoperation<=getdate() and vvv._dateoperation>=(getdate()-50)
and vvv._compte<>('6300150')
group by vvv._compte

--Code fonds	Libéllé Fonds	Code ISIN	Libellé Produit	_Strategie	A/V	Quantité	Devise	Prix de revient	Prix de revient CC	Prix du Marché	Prix du Marché CC	Montant Net	Montant Net CC	+/- Value	+/- Value CC	Libéllé Type	Libellé Pays	Libellé secteur	Libellé sous-secteur
sp_columns [valoprdt]

SELECT     
--    g._NOMGROUPE                       as C00_Groupe,    
--    convert(char,v._dateoperation,103) as C01_Date,    
    v._compte                          as "Code fonds",         
--    isnull(c._username,'')             as C03_ISIN_Ptf,    
    c._LIBELLECLI                      as "Libéllé Fonds",
--    v._assettype                       as C05_code_Titre,    
    p._isin                            as "Code ISIN",
    p._libelle1prod                    as "Libellé Produit",
    v._STRATINV
    isnull(v._netamountD,0)            as C12_Valeur_Boursiere,         
    isnull(v._netamountDcc,0)          as C13_Coupon_Couru,     
    isnull(v._netentrypriceD,0)        as C14_Valeur_Comptable,      
    isnull(v._netentrypriceDcc,0)      as C15_Coupon_Couru_Comptable,       
    isnull(v._unlossD,0)               as C16_PMV,       
    isnull(v._unlossDcc,0)             as C17_PMV_CC,     
    v._libelletypeproduit              as C18_Type_Produit,      
    v._deviseS                         as C19_Devise_Titre,
    s._LIBELLESECTEUR                  as C23_Secteur,
    ss._LIBELLESOUSSECTEUR             as C24_Sous_Secteur,    
    isnull(pi._LIBELLEPAYS,'')         as C25_Pays,      
    e._NOMEMETTEUR                     as C26_Emetteur,    
    GR._libellegrper                   as C27_Grp_Emetteur,    
    convert(char,p._echeance,103)      as C28_maturite,    
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
left outer join           #DERNIERE_VALO_COMPTES lastOpe    on lastOpe.compte = c._compte
WHERE       
    v._dateoperation= lastOpe.date
--and g._codegroupe in ('M','OP')
and g._codegroupe in ('OP')
--and left(s._LIBELLESECTEUR,4) in ('CORP','AGEN','EMPR') 
--and v._compte<>('6300150')    
--and s._LIBELLESECTEUR <> 'EMPRUNTS D''ETAT'
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
select i.*, r.C32_Rating,  convert(char,r.C33_DateRating,103)  as C33_DateRating
from #INVENTAIRE as i
left outer join #RATING as r on r.C05_code_Titre = i.C05_code_Titre

