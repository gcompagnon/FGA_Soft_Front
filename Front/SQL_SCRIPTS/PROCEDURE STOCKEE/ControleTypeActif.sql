/***********************/
-- Controle incoherence: type_Actif avec type_produits et secteur 
-- le type_Actif et le type_produit doit etre coherent via la table de correspondance PTF_TYPE_ACTIF
--           sauf pour les dérivés (PUT CALL) 
--                et pour les OPCVM où le type_actif et le secteur doivent etre coherents 

/************************/



GO

CREATE PROCEDURE ControleTypeActif

		@date DATETIME,
		@niveau FLOAT
AS 


  select Type_actif,type.Types,Type_Produit,secteur.id,Secteur, ptf.*  from PTF_TRANSPARISE as ptf
  left outer join PTF_TYPE_ACTIF as type on type.Produit = Type_Produit
  left outer join SECTEUR as secteur on secteur.libelle = Secteur
where Numero_Niveau = @niveau
 and  Dateinventaire >= @date
      and ( Type_actif <> type.Types   and type.Types <>  'OPCVM'   )                            
       or (
              ( type.Types  = 'OPCVM' and Type_actif = 'Actions' and secteur.id <> 'F ACTIONS' ) or
               ( type.Types  = 'OPCVM' and Type_actif = 'Monétaire' and secteur.id <> 'F TRESO' )or
                ( type.Types  = 'OPCVM' and Type_actif = 'Obligations' and secteur.id <> 'F OBLIG' ) or
                ( Type_Produit  IN ('PUT', 'CALL') and Type_actif = 'Obligations' and secteur.id not like 'O %' ) or
                ( Type_Produit  IN ('PUT', 'CALL') and Type_actif = 'Actions' and ( secteur.id not like 'A %'  and secteur.id <> 'F ACTIONS' ) )                 
           )
  /***
 doit retourner 0 lignes
 ***/