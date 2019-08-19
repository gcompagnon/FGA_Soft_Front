/***********************/
-- Controle incoherence: secteur avec type_actifs =Actions
-- Les type_actif Actions (action en direct et fonds actions) sont correctement classifies en Secteur Actions ou  Fonds  Actions 
/************************/

GO

CREATE PROCEDURE ControleSecteur

		@date DATETIME,
		@niveau FLOAT
AS 

select Type_actif,Type_Produit,Secteur, ptf.*  from PTF_TRANSPARISE as ptf
where Numero_Niveau = @niveau
 and  Dateinventaire >= @date
and ( Type_Produit in 
(   select Produit from PTF_TYPE_ACTIF  where Types = 'Actions' )
or Secteur = 'FONDS ACTIONS' )
-- non dans les secteurs actions
and Secteur NOT IN (
select libelle from SECTEUR 
                       where ( id  like 'A %'
                            or id = 'F ACTIONS'
                              ) )
 /***
 doit retourner 0 lignes
 ***/