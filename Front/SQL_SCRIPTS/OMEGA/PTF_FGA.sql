-- Fichier en encodage UTF-8 (ANSI ou ASCII fonctionne si aucun caractères accentués)

-- REQUETE DE SYNCHRONISATION entre OMEGA et la base FGA Soft
--  exploite les inventaires validés suivant les différents groupes
--  Récupère les mandats et opcvm pour une date donnée
--  Le traitement est différent suivant: 
--        Les futures
--        Les Callables, Perpetuelles
-- La ligne --<CLAUSE_COMPLEMENTAIRE> permet de configurer une clause sur des comptes (pour faire des extractions d inventaires
--      sur certains comptes
--------------------------------------------------------------------------------------------------------------------------------

--- A voir/TODO: la table fcp.OPERAT qui contient les opérations du jour liées au carnet d ordre fcp.CARNETOR par _numeroordre
---  le gérant sauvegarde une opé dans le carnet d ordre(statut sauvegardé MO et transféré MO) _TRANSFERERMO =1 
---  le MO valide l'opé pour envoyer un swift , et mettre dans la prochaine valo  
---  le MO met dans l'état ValiderBO pour attendre de fixer une VL (ordre sur OPCVM)
--         ou dans l'état retour conservateur si le prix est déjà connu
-- select top 100 c._CODEPRODUI,c._compte,c._codeoperation,c._codeoperateur,o._NOMINAL,o._QUANTITE,o._DATEVL,o._TRANSFERERMO, o._VALIDMO,o._EXPORTED, o._CONFIRM, o._VALORISEE, o._RECONCILIATED,
--  c.*,o.* from fcp.carnetor as c
--  left outer join fcp.OPERAT as o on o._NUMEROORDRE = c._NUMEROORDRE
--  order by c._datedebut desc
  

--- A voir/TODO: mettre la création des lignes Cash contrebalancant les positions Futures dans la proc stock GetCashFgaX.sql


-- A voir / todo : synchroniser sur le code Produit:  jointure com.produit et com.TYPEPROD
-- select type.*, p.* from com.produit as p
-- left outer join com.TYPEPROD as type on type._CODEDUTYPE = p._CODEDUTYPE
-- where _codeprodui = 'IT0004612179'


--------------------------------------------------------------------------------------------------------------------------------
--- Description de la table fcp.VALOPRDT  (table des fonds valorisés à end-of-day):
---                     'S' comme Stock  : _netAmountS , _netEntryPriceS et  _unlossS , le cours en devise Produit (alimentation SIX telekurs)
---                     'D' comme Devise : _netAmountD , _netEntryPriceD et  _unlossD , le cours en devise Fond (euro) , cette ligne est reconciliée avec les inventaires validés (où le forex provient de BP2S)
---                     'M' comme Market : _netAmountM , _netEntryPriceM et  _unlossM , le cours en devise Fond (euro) est calculé par Omega avec le forex stocké (cours à l'entree de la ligne en ptf)
---                     'I' comme Indicatif : non utilisé 

---         les données d analyse (duration , YTM/rendement, sensibilite/ModifiedDuration ..)  sont avec les prix dans com.prixhist 
---         sur com.produit  methodedecot est un code (voir com.METHODECOT_CORR ) (null pour les actions)
--                           methodedecot = 1 (en piece), 3 en % Pdecimal en 32ième
---                          _codedutype lié à com.typeprod
---                           _codeinterets à com.frequenceint

use omega
declare @dateInventaire as datetime
set  @dateInventaire = '05/06/2015'
--set  @dateInventaire = '***'

-- Liste des codes Groupes utilisés comme 
declare @groupList as table (gr char(2))
insert into @groupList (gr) VALUES ('IG') insert into @groupList (gr) VALUES ('IO') insert into @groupList (gr) VALUES ('UM')
insert into @groupList (gr) VALUES ('IN') insert into @groupList (gr) VALUES ('CA') insert into @groupList (gr) VALUES ('CT')
insert into @groupList (gr) VALUES ('MM') insert into @groupList (gr) VALUES ('SA') insert into @groupList (gr) VALUES ('SD')
insert into @groupList (gr) VALUES ('QX') insert into @groupList (gr) VALUES ('GA') insert into @groupList (gr) VALUES ('OP')
insert into @groupList (gr) VALUES ('A6') insert into @groupList (gr) VALUES ('A4') insert into @groupList (gr) VALUES ('A5')
insert into @groupList (gr) VALUES ('UN') insert into @groupList (gr) VALUES ('SM') insert into @groupList (gr) VALUES ('MN')
insert into @groupList (gr) VALUES ('MH') insert into @groupList (gr) VALUES ('PR')

-- select * from fcp.grpedeft g  where g._codegroupe in (select gr from @groupList )
--------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------ETAPE 0 : Lister les comptes : mandats , opcvm ou sicav (mono parts ou multipars)
-- en multi parts: prendre l isin de la part principale

-- LISTE DES COMPTES en OPCVM et MANDATS
create table #COMPTES (Compte varchar(60), TYPE varchar(15), ISIN varchar(12) null, LIBELLE varchar(60) NULL )

insert into #COMPTES
select v._compte as 'COMPTE','OPCVM ' as 'TYPE', 'XX0000000000' as 'ISIN', c._LIBELLECLI as 'LIBELLE'
from fcp.vlrstion v
left outer join Fcp.CONSGRPE cg on cg._compte=v._compte
left outer join fcp.cpartfcp c on  c._compte=v._compte 
where
         v._DATEOPERATION=@dateInventaire
And   v._actif <> 0
And  cg._codegroupe = 'OP' -- les groupes OPCVM

insert into #COMPTES
select v._compte as 'COMPTE','MANDAT' as 'TYPE', null as 'ISIN', c._LIBELLECLI as 'LIBELLE'
from fcp.vlrstion v
left outer join fcp.cpartfcp c on  c._compte=v._compte 
where
         v._DATEOPERATION=@dateInventaire	 
And   v._actif <> 0
And v._compte not in (select Compte from #COMPTES where Type ='OPCVM ')

-- MAJ de l isin pour les OPCVM
-- pas de multiparts
update #COMPTES
set ISIN = c._username,
TYPE = 'OPCVM MONOPART'
from #COMPTES as v
left outer join fcp.cpartfcp c on c._compte=v.Compte
where v.TYPE = 'OPCVM' and isnull(c._multiparts,0) = 0
-- en multiparts: prendre le code ISIN de la part principale
update #COMPTES
set ISIN = m._codeProdui,
TYPE = 'OPCVM MULTIPART'
from #COMPTES as v
left outer join fcp.cpartfcp c on c._compte=v.Compte
left outer join fcp.multiparts as m on m._Compte = v.Compte and ( _TypePart = 1 or _nomPart = 'M' or _nomPart = 'D' or _nomPart = 'PART M' or _nomPart = 'PART F')
where v.TYPE = 'OPCVM' and isnull(c._multiparts,0) <> 0

select * from #COMPTES
-- FIN de LA LISTE DES COMPTES


--------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------ETAPE 1 : Inventaire

-- Partie 1: tous actifs autres que les Futures ( and v._libelletypeproduit like 'FUTURES %' )
--           le type produit est enrichi avec TAUX pour les options sur emprunts d etat(govies)
--           pas de date de maturité pour les options, certificats Actions et Futures Actions
SELECT
            g._NOMGROUPE							as Groupe,
            v._dateoperation						as Dateinventaire,
            v._compte								as Compte,
            c.ISIN									as ISIN_Ptf,
            c.LIBELLE								as Libelle_Ptf,
            v._assettype							as code_Titre,
            p._isin									as isin_titre,
            p._libelle1prod							as Libelle_Titre,
            sum(v._netamountD)						as Valeur_Boursiere,
            sum(v._netamountDcc )					as Coupon_Couru,
            sum(v._netentrypriceD  )				as Valeur_Comptable,
            sum(v._netentrypriceDcc )				as Coupon_Couru_Comptable,
            sum(v._unlossD    )						as PMV,
            sum(v._unlossDcc)						as PMV_CC,
            rtrim(v._libelletypeproduit) + (case when (( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)='EMP') then ' TAUX' 		else '' end) as Type_Produit, 
            v._deviseS								as Devise_Titre,
            s._LIBELLESECTEUR						as Secteur,
            ss._LIBELLESOUSSECTEUR					as Sous_Secteur,
            pi._LIBELLEPAYS							as Pays,
            e._NOMEMETTEUR							as Emetteur,
--            r._signature							as Rating,
            GR._libellegrper						as Grp_Emetteur,
            case when ((( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)<>'EMP') OR 		(left(v._libelletypeproduit,12)='Certificat A') OR (left(v._libelletypeproduit,9)='FUTURES A' ) ) then NULL else p._echeance end as maturite,
            h._duration								as duration,
            case when h._sensibilite=0 then h._duration/(1+(h._rendement)/100) else h._sensibilite end as sensibilite,
            h._coursclose							as coursclose,
            case when p._methodecot=3 then	sum( v._quantite*p._nominal) else sum( v._quantite) end as quantite,
            sum(p._nominal) as nominal,
			p._coupon As coupon,
			case when h._sensibilite=0 then h._rendement else (h._duration/h._sensibilite -1 ) *100 end as rendement
INTO #INVENTAIRE		
FROM
          fcp.valoprdt  v
left outer join           com.produit p                 on p._codeprodui=v._assettype
left outer join			  com.prdtclasspys ps			on p._codeprodui=ps._codeprodui and ps._classification=0
left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
left outer join			  com.ssectclass ss_s			on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
--left outer join           com.rating r					on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where r._date<=v._dateoperation and r._codeprodui=p._codeprodui)
left outer join           com.prixhist  h               on v._datecours=h._date and p._codeprodui=h._codeprodui
left outer join           com.emetteur e                on e._emetteur=p._emetteur
left outer join           com.grpemetteursratios GR     on  e._grper=GR._codegrper
left outer join           com.pays pi                   on pi._CODEPAYS= gr._codepays
left outer join           #COMPTES c                    on  c.compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
WHERE          
         v._dateoperation=@dateInventaire and
--<CLAUSE_COMPLEMENTAIRE>
	     g._codegroupe in (select gr from @groupList) and
	     v._libelletypeproduit not like 'FUTURES %' and
	     (v._typeproduit not like 'CALL%' and  v._typeproduit not like 'PUT%' ) 
group by
			g._NOMGROUPE,
            v._dateoperation ,
            v._compte,
            c.ISIN,
            c.LIBELLE,
            v._assettype,
            p._isin,  
            p._libelle1prod,
            v._libelletypeproduit, 
            v._deviseS, 
            s._LIBELLESECTEUR, 
            ss._LIBELLESOUSSECTEUR,
            pi._LIBELLEPAYS, 
            e._NOMEMETTEUR,
--            r._signature,
			GR._libellegrper,
			p._echeance,
			h._duration,
			h._sensibilite,
			h._rendement,
			h._coursclose,
			p._methodecot,
			p._coupon,
			h._rendement
--ORDER BY g._NOMGROUPE ,  v._compte,  p._isin


-- Partie 2.1: les Futures : doubler les lignes: 
--               1 ligne de type future : quantite * quotite/taille du contrat * prix
--               2 ligne de type cash pour avoir somme(Valeur Boursiere) = PMV et somme(Valeur Comptable) = 0
SELECT
            g._NOMGROUPE							as Groupe,
            v._dateoperation						as Dateinventaire,
            v._compte								as Compte,
            c.ISIN									as ISIN_Ptf,
            c.LIBELLE								as Libelle_Ptf,
            v._assettype							as code_Titre,
            p._isin 								as isin_titre,  
            p._libelle1prod							as Libelle_Titre,
            sum( (case when (v._BS = 'V') then -1 else 1 end) *p._multiple*v._marketpriceD*v._quantite ) as Valeur_Boursiere,
            sum(v._netamountDcc )					as Coupon_Couru,
			sum( (case when (v._BS = 'V') then -1 else 1 end) *p._multiple*v._entrypriceD*v._quantite ) as Valeur_Comptable,
            sum(v._netentrypriceDcc )				as Coupon_Couru_Comptable,  
            sum(v._unlossD)						as PMV,  
            sum(v._unlossDcc)						as PMV_CC,
            rtrim(v._libelletypeproduit) as Type_Produit, 
            v._deviseS								as Devise_Titre, 
            s._LIBELLESECTEUR						as Secteur, 
            ss._LIBELLESOUSSECTEUR					as Sous_Secteur,
            pi._LIBELLEPAYS							as Pays, 
            e._NOMEMETTEUR							as Emetteur,
--            r._signature							as Rating,
            GR._libellegrper						as Grp_Emetteur,   
            case when (left(v._libelletypeproduit,9)='FUTURES A' ) then NULL else p._echeance end as maturite,
            h._duration								as duration,
            case when h._sensibilite=0 then h._duration/(1+(h._rendement)/100) else h._sensibilite end as sensibilite,
            h._coursclose							as coursclose,
            case when p._methodecot=3 then	sum( v._quantite*p._nominal) else sum( v._quantite) end as quantite,
            sum(p._nominal) as nominal,
			p._coupon As coupon,
			case when h._sensibilite=0 then h._rendement else (h._duration/h._sensibilite -1 ) *100 end as rendement			
into #INVENTAIRE_FUTURES
FROM
          fcp.valoprdt  v
left outer join           com.produit p                 on p._codeprodui=v._assettype
left outer join			  com.prdtclasspys ps			on p._codeprodui=ps._codeprodui and ps._classification=0
left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
left outer join			  com.ssectclass ss_s			on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
--left outer join           com.rating r					on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where r._date<=v._dateoperation and r._codeprodui=p._codeprodui)
left outer join           com.prixhist  h               on v._datecours=h._date and p._codeprodui=h._codeprodui
left outer join           com.emetteur e                on e._emetteur=p._emetteur
left outer join           com.grpemetteursratios GR     on  e._grper=GR._codegrper
left outer join           com.pays pi                   on pi._CODEPAYS= gr._codepays
left outer join           #COMPTES c                    on  c.compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
WHERE          
         v._dateoperation=@dateInventaire and
--<CLAUSE_COMPLEMENTAIRE>
	     g._codegroupe in (select gr from @groupList) and
	     v._libelletypeproduit like 'FUTURES %'	     
group by
			g._NOMGROUPE,
            v._dateoperation ,
            v._compte,
            c.ISIN,
            c.LIBELLE,
            v._assettype,
            p._isin,  
            p._libelle1prod,
            v._libelletypeproduit, 
            v._deviseS, 
            s._LIBELLESECTEUR, 
            ss._LIBELLESOUSSECTEUR,
            pi._LIBELLEPAYS, 
            e._NOMEMETTEUR,
--            r._signature,
			GR._libellegrper,
			p._echeance,
			h._duration,
			h._sensibilite,
			h._rendement,
			h._coursclose,
			p._methodecot,
			p._coupon,
			h._rendement
--ORDER BY g._NOMGROUPE ,  v._compte,  p._isin

-- Utiliser la date de maturité du sous jacent pour les futures Taux
update #INVENTAIRE_FUTURES
set maturite = ssjacent._echeance
-- le lien sur le sousjacent : pour recuperer la date de maturité
from #INVENTAIRE_FUTURES as f
left outer join  com.produit as future on future._codeprodui = f.code_Titre
left outer join  com.produit as ssjacent on ssjacent._codeprodui = future._sousjacent
where f.Type_Produit like 'FUTURES T%' and ssjacent._echeance is not null



---- Partie 2.2: les Futures : doubler les lignes: 
----               1 ligne de type future
----               2 ligne de type cash pour avoir somme(Valeur Boursiere) = PMV et somme(Valeur Comptable) = 0

insert into #INVENTAIRE_FUTURES
SELECT
            g._NOMGROUPE							as Groupe,
            v._dateoperation						as Dateinventaire,
            v._compte								as Compte,
            c.ISIN									as ISIN_Ptf,
            c.LIBELLE								as Libelle_Ptf,
            rtrim(v._assettype) + '_cash'					as code_Titre,
            rtrim(p._isin) + '_cash'  								as isin_titre,  
            'Liquidité(Future)'						as Libelle_Titre,
            sum( (case when (v._BS = 'A') then -1 else 1 end) *p._multiple*v._entrypriceD*v._quantite ) as Valeur_Boursiere,
            0										as Coupon_Couru,
			sum( (case when (v._BS = 'A') then -1 else 1 end) *p._multiple*v._entrypriceD*v._quantite ) as Valeur_Comptable,
            0										as Coupon_Couru_Comptable,  
            0										as PMV,  
            0										as PMV_CC,
            'Cash'									as Type_Produit, 
            'EUR'									as Devise_Titre, 
            'Liquidité'								as Secteur, 
            'Liquidité'								as Sous_Secteur,
            'France'									as Pays,  -- pas d expo sur le Pays France, mais sur l'EURO . voir pour un faux pays
            NULL									as Emetteur,
--            NULL									as Rating,
            NULL									as Grp_Emetteur,   
            NULL									as maturite,
            NULL									as duration,
			0										as sensibilite,
            NULL									as coursclose,
            0										as quantite,
            0										as nominal,
			0										as coupon,
			0										as rendement
FROM
          fcp.valoprdt  v
left outer join           com.produit p                 on p._codeprodui=v._assettype
left outer join           #COMPTES c                    on  c.compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
WHERE          
         v._dateoperation=@dateInventaire and
--<CLAUSE_COMPLEMENTAIRE>
	     g._codegroupe in (select gr from @groupList) and
	     v._libelletypeproduit like 'FUTURES %'	     
group by
			g._NOMGROUPE,
            v._dateoperation ,
            v._compte,
            c.ISIN,
            c.LIBELLE,
            v._assettype,
            p._isin
            
--ORDER BY g._NOMGROUPE ,  v._compte,  p._isin

---- Partie 2.3: les Futures de Change : mettre la patte cash des futures de change en expo devise et
--                             la patte fut en devise EUR
select f.Compte, f.Code_titre,f.Devise_titre, c.Code_titre as 'cash'
into #INV_FUTURES_CHANGE
from #INVENTAIRE_FUTURES as f
left outer join #INVENTAIRE_FUTURES as c on c.Compte = f.Compte and c.code_titre = rtrim(f.code_titre)+'_cash'
where f.Type_produit like 'FUTURES DEVISE %'
order by f.compte,f.code_titre

update #INVENTAIRE_FUTURES
set Devise_titre = c.Devise_titre
from #INVENTAIRE_FUTURES as f
left outer join #INV_FUTURES_CHANGE as c on c.Compte = f.compte and c.cash = f.code_titre
where c.Code_titre is not null

update #INVENTAIRE_FUTURES
set Devise_titre = 'EUR'
from #INVENTAIRE_FUTURES as f
left outer join #INV_FUTURES_CHANGE as c on c.Compte = f.compte and c.code_titre = f.code_titre
where c.Code_titre is not null

insert into #INVENTAIRE
select * from #INVENTAIRE_FUTURES


-- Partie 3.1: les options : doubler les lignes: 
/*  - la quantité d'option               : Qté_Opt
  - la valeur de la figure             : Val_Fig
  - le delta de l'option                : Delta
  - cours du sous-jacent            : Crs_SsJ
  - la valeur boursière de l'option  : VB_Opt 
  - le sens de l'option : Sens =1 si achat de call ou vente de put, -1 sinon
  - le cours de la devise du sous jacent : Frx_SsJ , 1 si EUR

On remplace alors la ligne de l'option par deux lignes : 
    1 - ligne de type exposition au sous-jacent de 
          - Valeur Boursiere : sens*Qté_Opt*Val_Fig*Delta*Crs_SsJ/Frx_SsJ
          - Quantité             : sens*Qté_Opt*Val_Fig*Delta
    2 - ligne de type cash pour avoir somme(Valeur Boursiere) = PMV et somme(Valeur Comptable) = 0
          - Valeur Boursiere : sens*Qté_Opt*Val_Fig*Delta*Crs_SsJ/Frx_SsJ +VB_Opt
          - Quantité             : sens*Qté_Opt*Val_Fig*Delta*Crs_SsJ +VB_Opt
*/


insert into #INVENTAIRE
SELECT
            g._NOMGROUPE							as Groupe,
            v._dateoperation						as Dateinventaire,
            v._compte								as Compte,
            c.ISIN									as ISIN_Ptf,
            c.LIBELLE								as Libelle_Ptf,
            rtrim(v._assettype) 						as code_Titre,
            case p2._codedutype when 'BENCH' then left(rtrim(v._assettype),12) else left(rtrim(p._sousjacent),12) end	as isin_titre,
            p2._libelle1prod							as Libelle_Titre,
            sum(isnull((case v._BS when 'A' then 1 else -1 end)*v._quantite*p._multiple*h._delta*(h2._coursclose/isnull(dev._coursclose,1)),0)) as valeur_Boursiere,
            --iif((v._BS='A' and v._libelletypeproduit like 'CALL%') OR (v._BS='V' and v._libelletypeproduit like 'PUT%'),1,-1)
            0 as Coupon_Couru,
            sum(v._netentrypriceD  )				as Valeur_Comptable,
            0 as Coupon_Couru_Comptable,
            sum(v._unlossD    )						as PMV,
            0 as PMV_CC,
            rtrim(t._LIBELLE1TYPE) as Type_Produit, 
            v._deviseS								as Devise_Titre,
            s._LIBELLESECTEUR						as Secteur,
            ss._LIBELLESOUSSECTEUR					as Sous_Secteur,
            pi._LIBELLEPAYS							as Pays,
            e._NOMEMETTEUR							as Emetteur,
            GR._libellegrper						as Grp_Emetteur,
            NULL as maturite,
            h._duration								as duration,
            0 as sensibilite,
            h2._coursclose							as coursclose,
            sum(isnull((case v._BS when 'A' then 1 else -1 end)*v._quantite*p._multiple*h._delta,0)) as quantite,
            0 as nominal,
			0 As coupon,
			0 as rendement
		
FROM
          fcp.valoprdt  v
left outer join           com.produit p                 on p._codeprodui=v._assettype
left outer join           com.produit p2                 on p2._codeprodui=p._sousjacent
left outer join			  com.prdtclasspys ps			on p2._codeprodui=ps._codeprodui and ps._classification=0
left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
left outer join			  com.ssectclass ss_s			on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
left outer join           com.prixhist  h               on v._datecours=h._date and p._codeprodui=h._codeprodui
left outer join           com.prixhist  h2              on v._datecours=h2._date and p._sousjacent=h2._codeprodui
left outer join           com.emetteur e                on e._emetteur=p2._emetteur
left outer join           com.grpemetteursratios GR     on  e._grper=GR._codegrper
left outer join           com.pays pi                   on pi._CODEPAYS= gr._codepays
left outer join           #COMPTES c                    on  c.compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
left outer join			  Com.TYPEPROD t				on t._codedutype=p2._codedutype
left outer join           com.DEVHIST dev               on dev._date = v._dateoperation and dev._CONTREDEVISE = v._deviseS and dev._CODEDEVISE = 'EUR'
WHERE          
         v._dateoperation=@dateInventaire and
--<CLAUSE_COMPLEMENTAIRE>         
	     g._codegroupe in (select gr from @groupList) and
	     (_typeproduit like 'CALL%' or _typeproduit like 'PUT%' )

group by
			g._NOMGROUPE,
            v._dateoperation ,
            v._compte,
            c.ISIN,
            c.LIBELLE,
            v._assettype,
            p._sousjacent, 
            p2._libelle1prod,
            t._LIBELLE1TYPE, 
            v._deviseS, 
            s._LIBELLESECTEUR, 
            ss._LIBELLESOUSSECTEUR,
            pi._LIBELLEPAYS, 
            e._NOMEMETTEUR,
			GR._libellegrper,
			p._echeance,
			h._duration,
			h._sensibilite,
			h._rendement,
			h2._coursclose,
			p._methodecot,
			p._coupon,
			h._rendement,p2._codedutype,
            dev._coursclose

-- 3.2 patte cash des options

insert into #INVENTAIRE
SELECT
            g._NOMGROUPE							as Groupe,
            v._dateoperation						as Dateinventaire,
            v._compte								as Compte,
            c.ISIN									as ISIN_Ptf,
            c.LIBELLE								as Libelle_Ptf,
            rtrim(v._assettype) + '_C'					as code_Titre,
            'Cash_Opt'								as isin_titre,  
            'Liquidité(Option' +rtrim(p._sousjacent)+')'						as Libelle_Titre,
            sum(-isnull((case v._BS when 'A' then 1 else -1 end)*v._quantite*p._multiple*h._delta*(h2._coursclose/isnull(dev._coursclose,1)),0)+v._netamountD) as valeur_Boursiere,
            0										as Coupon_Couru,
			0										 as Valeur_Comptable,
            0										as Coupon_Couru_Comptable,  
            0										as PMV,  
            0										as PMV_CC,
            'Cash'									as Type_Produit, 
            'EUR'									as Devise_Titre, 
            'Liquidité'								as Secteur, 
            'Liquidité'								as Sous_Secteur,
            'France'									as Pays,  -- pas d expo sur le Pays France, mais sur l'EURO . voir pour un faux pays
            NULL									as Emetteur,
--            NULL									as Rating,
            NULL									as Grp_Emetteur,   
            NULL									as maturite,
            0										as duration,
			0										as sensibilite,
            1									as coursclose,
            sum(-isnull((case v._BS when 'A' then 1 else -1 end)*v._quantite*p._multiple*h._delta*h2._coursclose,0)+v._netamountD)	as quantite,
            0										as nominal,
			0										as coupon,
			0										as rendement
FROM
          fcp.valoprdt  v
left outer join           com.produit p                 on p._codeprodui=v._assettype
left outer join           #COMPTES c                    on  c.compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
left outer join           com.prixhist  h               on v._datecours=h._date and p._codeprodui=h._codeprodui
left outer join           com.prixhist  h2              on v._datecours=h2._date and p._sousjacent=h2._codeprodui
left outer join           com.DEVHIST dev               on dev._date = v._dateoperation and dev._CONTREDEVISE = v._deviseS and dev._CODEDEVISE = 'EUR'
WHERE          
         v._dateoperation=@dateInventaire and
--<CLAUSE_COMPLEMENTAIRE>
	     g._codegroupe in (select gr from @groupList) and
	     (_typeproduit like 'CALL%' or _typeproduit like 'PUT%' ) 
group by
			g._NOMGROUPE,
            v._dateoperation ,
            v._compte,
            c.ISIN,
            c.LIBELLE,
            v._assettype,
            p._isin,p._sousjacent,
            dev._coursclose


--------------------------------------------------------------------------------------------------------------------------------
------------------------------ ETAPE RATING
select 
    max(r._date) as C33_DateRating,
    r._codeprodui as C05_code_Titre
into #RATINGDATE
from #INVENTAIRE as i
left outer join com.rating as r on r._codeprodui=i.code_Titre and r._codeagence='INT' and r._date<=i.Dateinventaire
group by  r._codeprodui

select 
    r._signature as C32_Rating,
    r._date as C33_DateRating,
    r._codeprodui as C05_code_Titre
into #RATING
from #RATINGDATE as i
left outer join com.rating as r on r._codeprodui=i.C05_code_Titre and r._codeagence='INT' and r._date=i.C33_DateRating
------------------------------ ETAPE OBLIG CALLABLE : prendre la 1ere date de call à la place de l echeance/maturite
-- pour OCALL oblig callable, OPERP obligations perpetuelles, OPERP perp à tx variables et OTCVA Oblig Callable à tx variables
-- l echeancier est dans com.flux avec _callput =1 pour une date de call et _callput =2 pour une date de put
-- _callput =3 pour mixed
select code_Titre into #CALLABLE_PUTABLE_PERPETUEL
from #INVENTAIRE as i
left outer join com.produit p on p._codeprodui=i.code_Titre
where p._callable = 1 and p._codedutype IN ( 'OCALL','OPERP', 'OTVCA', 'OPERV', 'OPUT')

-- pour les callable et putable, prendre la prochaine date  call
-- pour les perp, prendre la date de prochain evenement pour les callables ou garder la date echeance pour les non callables
SELECT _codeprodui as code_Titre , min (_DATE ) as maturite
into #CALLDATE
FROM [OMEGA].[Com].[FLUX] 
where _callput in (1,2,3) and _Date > getDate()  
and _codeprodui in (select code_Titre from #CALLABLE_PUTABLE_PERPETUEL)
group by   _codeprodui


-- pour les callable et putable, calculer la sensibilite à partir du rendement
SELECT 
  v._assettype as code_Titre,
  case when h._rendement=0 then h._sensibilite else h._duration/(1+(h._rendement)/100) end as sensibilite,			
  h._rendement as rendement,
  h._duration as duration
into #SENSI_CALLABLE_PUTABLE_PERPETUEL
FROM
          fcp.valoprdt  v
left outer join com.prixhist h on v._datecours=h._date and v._assettype=h._codeprodui
where 
v._dateoperation=@dateInventaire 
and 
v._assettype in (select code_Titre from #CALLABLE_PUTABLE_PERPETUEL)  


-----------------------------SORTIE
--les titres en general

select LEFT(Groupe,25)as Groupe,Dateinventaire,LEFT(Compte,60)as Compte,LEFT(ISIN_Ptf,12)as ISIN_Ptf,LEFT(Libelle_Ptf,60)as Libelle_Ptf,LEFT(i.code_Titre,15)as code_Titre,LEFT(isin_titre,12)as isin_titre,LEFT(Libelle_Titre,60)as Libelle_Titre,Valeur_Boursiere,Coupon_Couru,Valeur_Comptable,Coupon_Couru_Comptable,PMV,PMV_CC,LEFT(Type_Produit,60) as Type_Produit,LEFT(Devise_Titre,3)as Devise_Titre,LEFT(Secteur,60)as Secteur,LEFT(Sous_Secteur,60)as Sous_Secteur,LEFT(Pays,60)as Pays,LEFT(Emetteur,60)as Emetteur,LEFT(r.C32_Rating,4) as rating,LEFT(Grp_Emetteur,60)as Grp_Emetteur,i.maturite,duration,sensibilite,coursclose,quantite,coupon,rendement 
from #INVENTAIRE as i
left outer join #RATING as r on r.C05_code_Titre = i.code_Titre
where i.code_Titre not in (select code_Titre from #CALLABLE_PUTABLE_PERPETUEL)
and  ( Valeur_Boursiere <> 0 or Coupon_Couru <> 0 )
UNION
-- les titres callables
select LEFT(Groupe,25)as Groupe,Dateinventaire,LEFT(Compte,60)as Compte,LEFT(ISIN_Ptf,12)as ISIN_Ptf,LEFT(Libelle_Ptf,60)as Libelle_Ptf,LEFT(i.code_Titre,15)as code_Titre,LEFT(isin_titre,12)as isin_titre,LEFT(Libelle_Titre,60)as Libelle_Titre,Valeur_Boursiere,Coupon_Couru,Valeur_Comptable,Coupon_Couru_Comptable,PMV,PMV_CC,LEFT(Type_Produit,60) as Type_Produit,LEFT(Devise_Titre,3)as Devise_Titre,LEFT(Secteur,60)as Secteur,LEFT(Sous_Secteur,60)as Sous_Secteur,LEFT(Pays,60)as Pays,LEFT(Emetteur,60)as Emetteur,LEFT(r.C32_Rating,4) as rating,LEFT(Grp_Emetteur,60)as Grp_Emetteur,i.maturite,
callable.duration,
callable.sensibilite,
coursclose,quantite,coupon,callable.rendement 
from #INVENTAIRE as i
left outer join #RATING as r on r.C05_code_Titre = i.code_Titre
left outer join #CALLDATE as c on c.code_Titre = i.code_Titre
left outer join #SENSI_CALLABLE_PUTABLE_PERPETUEL as callable on callable.code_Titre = i.code_Titre
where i.code_Titre in (select code_Titre from #CALLABLE_PUTABLE_PERPETUEL)
and ( Valeur_Boursiere <> 0 or Coupon_Couru <> 0 )

drop table #COMPTES
drop table #INVENTAIRE
drop table #INVENTAIRE_FUTURES
drop table #RATING
drop table #RATINGDATE
drop table #CALLDATE
drop table #CALLABLE_PUTABLE_PERPETUEL
drop table #SENSI_CALLABLE_PUTABLE_PERPETUEL
drop table #INV_FUTURES_CHANGE
