--controle interne: suivi mensuel des ETF actions
-- entre 2 dates: par défaut, entre le 1er jour du mois présent et le dernier jour du mois précédent

declare @date2 datetime
set @date2 = DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)

-- dernier jour du mois précédent
declare @date1 datetime
set @date1 = DATEADD(DAY,-1,DATEADD(MONTH,-1,@date2) )

(select
       O._numeroint as numero_ope,
       O._compte as code_compte, 
       C._libellecli as libelle_compte,
      convert(varchar,O._dateheurecreation,103) as dateheurecreation,
      convert(varchar,O._dateoperation,103) as date_operation,
      convert(varchar,O._datevaleur,103) as date_reglement, 
      O._codeoperation as type_operation, 
      O._codeprodui as code_produit,
      P._libelle1prod as libelle_produit,
      P._codedutype as type_produit,
      O._codedevisereglement as devise_rgt,            
      o._prix as prix,
      quantite=case
      when left(p._codedutype,2) in ('FA','FT','FD') then isnull(o._quantite,0)
       else isnull(o._nominal,0) end,
      o._montantbrut as montant_brut_devise, 
      (o._fraishtct+o._fraishtcp) as frais_HT,
      (o._fraishtct+o._fraishtcp+o._impotct+o._vtvact+o._impotcp+o._vtvacp) as somme_frais,
      o._tauxchange,
      o._mtnet as montant_net_euro,
      O._codecourt as courtier,
      O._usercreation as Operateur_saisie,
      isnull(convert(varchar,P._echeance,103),'') as Echeance,
      Type=case
      when P._tracker=1 and P._codedutype in ('FCP','SI','SIUS','SIGB','SIYEN') then 'Tracker'
      else ''
      end,      
      isnull(S._libellesecteur,'') as libellesecteur,
      isnull(O._commentaireavis,'') as commentaires_de_gestion,
      isnull(o._commentaire,'') as commentaire_technique,      
      Type_ordre=case
      when o._numerofiche='AFFMARPRIM' THEN 'PRIMAIRE'
      when o._numerofiche='AFFECTPROD' THEN 'CARNET_ORDRE'
      else ''
      END
from
    fcp.operat O
    left outer join com.produit P on O._codeprodui=P._codeprodui
    left outer join fcp.cpartfcp C on O._compte=C._compte 
    left outer join com.courtier B on O._codecourt=B._codecourt
    left outer join com.operater T on O._usercreation=T._codeoperat
    left outer join 	  com.prdtclasspys ps	        on p._codeprodui=ps._codeprodui and ps._classification=0
    left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
    left outer join 	  com.ssectclass ss_s           on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
  left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
        
where
    
    O._codeoperation not in ('AR','ES','ET','EE','RB')
and 
    O._dateoperation between @date1 and @date2 and (left(P._codedutype,1)='A' or P._tracker=1 and S._libellesecteur='FONDS ACTIONS'))   



order by C._libellecli, O._dateoperation



