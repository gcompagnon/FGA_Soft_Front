--------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------
--  EXTRACTION : liste des Zones geographiques et pays pour la partie ACTIONS
-- voir la configuration des sous groupes ( poches dans les groupes) cmme MMP FP , MMP PREV et MMP SANTE

-- decomposition par compte puis recalcul par canton/sousgroupe et groupe
--------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------
declare @dateInventaire as datetime

declare @dateInventaireToday as datetime
set  @dateInventaireToday = '30/06/2015'

declare @dateInventaireMTD as datetime
set  @dateInventaireMTD = '29/05/2015'

declare @dateInventaireYTD as datetime
set  @dateInventaireYTD = '31/12/2014'

set @dateInventaire = @dateInventaireToday 
declare @niveauInventaire as tinyint
set @niveauInventaire = 5
declare @cleReport as char(20)
IF @cleReport IS NULL
BEGIN
  set @cleReport = 'EXT_ZONEG'
END

declare @incClassCompartiment as float
set @incClassCompartiment = 0 /** 1 pour encours, 2 pour Oblig, 3 pour divers, 4 pour monétaire, 5 pour Actions **/
declare @incClassRubrique as float
set @incClassRubrique = 0     /** A  l interieur d un compartiment, va de 0 à n pour la décomposition **/ 

/** la table des colonnes calculees / des sur-groupes  **/
create table #Groupe  ( groupe varchar(25) COLLATE DATABASE_DEFAULT primary key , ordreGroupe smallint null)
insert #Groupe values ('MM AGIRC',1)
insert #Groupe values ('MM ARRCO',2)

insert #Groupe values ('MMP',4)
insert #Groupe values ('INPR',5)
insert #Groupe values ('CAPREVAL',6)
insert #Groupe values ('CMAV',7)
insert #Groupe values ('MM MUTUELLE',8)
insert #Groupe values ('AUXIA',9)
insert #Groupe values ('QUATREM',10)
insert #Groupe values ('AUTRES',11)
declare @maxGroup int 
set @maxGroup = 12
create table #SousGroupeCalcule ( groupeCalcule varchar(25) COLLATE DATABASE_DEFAULT , groupeComposant varchar(25) COLLATE DATABASE_DEFAULT null)
insert #SousGroupeCalcule values ('MM ARRCO RT','3203105')
insert #SousGroupeCalcule values ('MM ARRCO RT','3203106')
insert #SousGroupeCalcule values ('MM ARRCO RT','3203107')
insert #SousGroupeCalcule values ('MM ARRCO FG','3303105')
insert #SousGroupeCalcule values ('MM ARRCO FG','3303106')
insert #SousGroupeCalcule values ('MM ARRCO FG','3303107')
insert #SousGroupeCalcule values ('MM ARRCO FS','3403105')
insert #SousGroupeCalcule values ('MM ARRCO FS','3403106')
insert #SousGroupeCalcule values ('MM ARRCO FS','3403107')

insert #SousGroupeCalcule values ('MMP CAA','4010017')
insert #SousGroupeCalcule values ('MMP PERE','4010015')
insert #SousGroupeCalcule values ('MMP PERE','4010025')
insert #SousGroupeCalcule values ('MMP PERE','4010027')
insert #SousGroupeCalcule values ('MMP EDITION','4010040')
insert #SousGroupeCalcule values ('MMP EDITION','4010042')
insert #SousGroupeCalcule values ('MMP RETRAITE','4010069')
insert #SousGroupeCalcule values ('MMP RETRAITE','4010081')
insert #SousGroupeCalcule values ('MMP RETRAITE','4010083')
insert #SousGroupeCalcule values ('MMP FP','4030001')
insert #SousGroupeCalcule values ('MMP FP','4030002')
insert #SousGroupeCalcule values ('MMP FP','4030003')
insert #SousGroupeCalcule values ('MMP PREV','4030005')
insert #SousGroupeCalcule values ('MMP PREV','4030006')
insert #SousGroupeCalcule values ('MMP PREV','4030007')
insert #SousGroupeCalcule values ('MMP SANTE','4030009')
insert #SousGroupeCalcule values ('MMP SANTE','4030010')
insert #SousGroupeCalcule values ('MMP SANTE','4030011')
insert #SousGroupeCalcule values ('MMP ARCELOR','04225')
insert #SousGroupeCalcule values ('MMP ARCELOR','04226')
insert #SousGroupeCalcule values ('MMP ARCELOR','04227')
insert #SousGroupeCalcule values ('MM AGIRC FG','7301105')
insert #SousGroupeCalcule values ('MM AGIRC FG','7301106')
insert #SousGroupeCalcule values ('MM AGIRC FG','7301107')
insert #SousGroupeCalcule values ('MM AGIRC FS','7401105')
insert #SousGroupeCalcule values ('MM AGIRC FS','7401106')
insert #SousGroupeCalcule values ('MM AGIRC FS','7401107')

insert #SousGroupeCalcule values ('QUATREM FONDS PROPRES','6300111')
insert #SousGroupeCalcule values ('QUATREM FONDS PROPRES','6300112')
insert #SousGroupeCalcule values ('QUATREM FONDS PROPRES','6300110')
insert #SousGroupeCalcule values ('QUATREM PERE','6300010')
insert #SousGroupeCalcule values ('QUATREM PÈRE M','6300021')
insert #SousGroupeCalcule values ('QUATREM PÈRE M','6300020')
insert #SousGroupeCalcule values ('QUATREM PERE','6300009')
insert #SousGroupeCalcule values ('QUATREM PREVOYANCE','6300131')
insert #SousGroupeCalcule values ('QUATREM PREVOYANCE','6300132')
insert #SousGroupeCalcule values ('QUATREM PREVOYANCE','6300130')
insert #SousGroupeCalcule values ('QUATREM RETRAITE','6300121')
insert #SousGroupeCalcule values ('QUATREM RETRAITE','6300122')
insert #SousGroupeCalcule values ('QUATREM RETRAITE','6300120')
insert #SousGroupeCalcule values ('QUATREM SANTE','6300141')
insert #SousGroupeCalcule values ('QUATREM SANTE','6300140')

 
--Les nouveaux groupes seront places arbitrairement à la suite
create table #Compte( compte varchar(60) COLLATE DATABASE_DEFAULT primary key , Libelle_Ptf varchar(60)COLLATE DATABASE_DEFAULT , groupe varchar(25)COLLATE DATABASE_DEFAULT, ssgroupe varchar(25)COLLATE DATABASE_DEFAULT , ordreGroupe smallint null, ordreSSGroupe smallint null)

insert into #Compte
select distinct Compte, Libelle_Ptf, f.GROUPE, 
case when ssgr.groupeCalcule is null then f.Groupe else ssgr.groupeCalcule end as 'ssgroupe',
case when gr.ordreGroupe is NULL then @maxGroup else gr.ordreGroupe end as 'ordreGroupe',
0 as 'ordreSSGroupe'
from PTF_FGA as f
left outer join #Groupe as gr on gr.groupe = f.groupe
left outer join #SousGroupeCalcule as ssgr on ssgr.groupeComposant = f.compte
where Dateinventaire = @dateInventaire

update #Compte
   set ordreSSGroupe = case when o.ordre is null then 0 else o.ordre end
   from #Compte as a
   left outer join ( select compte, ssgroupe,ROW_NUMBER()OVER(PARTITION BY ssGroupe Order by groupe) as ordre from #Compte
where ssgroupe is not null
 ) as o on o.compte = a.compte 

select * from #Compte

-- classification pour l'ensemble des actions sur la zone géographique (champ [PTF_TRANSPARISE.Zone_Géo]
--  create table #groupe_ZoneGeoAction ( groupe varchar(25) COLLATE DATABASE_DEFAULT, ordreGroupe int null,rubriquePrincipale varchar(30) COLLATE DATABASE_DEFAULT, codeZoneGeoLibelle varchar (30) COLLATE DATABASE_DEFAULT,
--  classifRubrique float, codeZoneGeo varchar(60) COLLATE DATABASE_DEFAULT)  

create table #Compte_ZoneGeoAction ( groupe varchar(25) COLLATE DATABASE_DEFAULT, ssgroupe varchar(25)COLLATE DATABASE_DEFAULT ,compte varchar(60) COLLATE DATABASE_DEFAULT, ordreCompte int null,rubriquePrincipale varchar(30) COLLATE DATABASE_DEFAULT, ZoneGeoLibelle varchar (30) COLLATE DATABASE_DEFAULT,
  classifRubrique float, ZoneGeo varchar(60) COLLATE DATABASE_DEFAULT, Pays varchar(60) COLLATE DATABASE_DEFAULT, )

create table #ZoneGeoPaysAction ( ZoneGeo varchar(60) COLLATE DATABASE_DEFAULT, ordreZG int null, Pays varchar(60) COLLATE DATABASE_DEFAULT, ordrePays int null )



  --insert into #groupe_ZoneGeoAction  (groupe, ordreGroupe,rubriquePrincipale, codeZoneGeoLibelle, classifRubrique, codeZoneGeo)
  --select distinct gr.groupe ,gr.ordreGroupe,'ACTIONS_ZONE_GEO','   '+tr.Zone_Géo,  100*@incClassCompartiment+ @incClassRubrique+0.1, tr.Zone_Géo as 'codeZoneGeo' 
  --from #groupe as gr,
  --[PTF_TRANSPARISE] as tr 
  -- where tr.Dateinventaire = @dateInventaire
  -- and tr.Numero_Niveau = @niveauInventaire
  -- and Zone_Géo is not null

  --insert into #compte_ZoneGeoAction  (groupe, Compte, ordreCompte,rubriquePrincipale, codeZoneGeoLibelle, classifRubrique, codeZoneGeo)
  --select distinct gr.groupe,gr.compte ,gr.ordreCompte,'ACTIONS_ZONE_GEO','   '+tr.Zone_Géo,  100*@incClassCompartiment+ @incClassRubrique+0.1 , tr.Zone_Géo as 'codeZoneGeo' 
  --from #COmpte as gr,
  --[PTF_TRANSPARISE] as tr 
  -- where tr.Dateinventaire = @dateInventaire
  -- and tr.Numero_Niveau = @niveauInventaire
  -- and Zone_Géo is not null


  insert into #ZoneGeoPaysAction (ZoneGeo,Pays,ordrePays,ordreZG)
  select distinct f.Zone_Géo,f.Pays ,ROW_NUMBER()OVER(PARTITION BY Zone_Géo Order by Pays) as 'Ordrepays',0 as 'ordreZG'
  from ( select distinct Zone_Géo,Pays from  [PTF_TRANSPARISE] as tr 
   where tr.Dateinventaire = @dateInventaire
   and tr.Numero_Niveau = @niveauInventaire
   and  tr.Type_actif = 'Actions' 
   and Zone_Géo is not null ) as f  
   
   update #ZoneGeoPaysAction
   set ordreZG = zg.ordre
   from #ZoneGeoPaysAction as a
   left outer join ( select a2.ZoneGeo, ROW_NUMBER()OVER(Order by ZoneGeo) as 'ordre' from (select distinct ZoneGeo from #ZoneGeoPaysAction) as a2 ) as zg on zg.ZoneGeo = a.ZoneGeo

  insert into #compte_ZoneGeoAction  (groupe,ssgroupe, Compte, ordreCompte,rubriquePrincipale, ZoneGeoLibelle, classifRubrique, ZoneGeo, Pays)
  select distinct gr.groupe,gr.ssgroupe,gr.compte ,gr.ordreGroupe,'ACTIONS_ZONE_GEO','   '+zg.ZoneGeo, zg.ordreZG,zg.ZoneGeo , null
  from #Compte as gr, (select distinct ZoneGeo,ordreZG from #ZoneGeoPaysAction) as zg 
     
  insert into #compte_ZoneGeoAction  (groupe,ssgroupe, Compte, ordreCompte,rubriquePrincipale, ZoneGeoLibelle, classifRubrique, ZoneGeo, Pays)
  select distinct gr.groupe,gr.ssgroupe,gr.compte ,gr.ordreGroupe,'ACTIONS_ZONE_GEO','        '+zg.Pays, zg.ordreZG+0.01*zg.ordrePays ,zg.ZoneGeo , zg.Pays
  from #Compte as gr, #ZoneGeoPaysAction as zg 


------------------------------------------------------------------

------------------------------------------------------------------
-- temp table for all the data
CREATE TABLE #DATA(
    [date] [datetime] NOT NULL,
	[cle] [char](20) NOT NULL,
	[libelle] [varchar](60) NULL,
	[groupe] [varchar](25) NOT NULL,
	[ssgroupe] [varchar](25) NOT NULL,
	[compte] varchar(60) COLLATE DATABASE_DEFAULT NOT NULL,	
	[ordre] [int] NULL,
	[classementRubrique] [real] NULL,
	[rubrique] [varchar](120) NOT NULL,
	[valeur] [float] NULL
)
------------------------------------------------------------------
-- output table
CREATE TABLE #MMARRCORT(
    [date] [datetime] NOT NULL,
	[libelle] [varchar](60) NULL,
	[groupe] [varchar](25) NOT NULL,
	[ssgroupe] [varchar](25) NOT NULL,
	[ordre] [int] NULL,
	[classementRubrique] [real] NULL,
	[ref] [real] NULL,	
	[encours] [float] NULL
)
CREATE TABLE #PREVOYANCE(
    [date] [datetime] NOT NULL,
	[libelle] [varchar](60) NULL,
	[groupe] [varchar](25) NOT NULL,
	[ssgroupe] [varchar](25) NOT NULL,
	[ordre] [int] NULL,
	[classementRubrique] [real] NULL,
	[ref] [real] NULL,
	[encours] [float] NULL
)
------------------------------------------------------------------
START:

PRINT @dateInventaire
------------------------------------------------------------------
-- PAR COMPTE
------------------------------------------------------------------
insert 
into  #DATA(cle,groupe,ssGroupe,compte,ordre,date,rubrique,libelle ,classementRubrique,valeur )
select @cleReport as cle,
		 grZoneGeoAction.Groupe as Groupe,
		 grZoneGeoAction.ssGroupe as ssGroupe,
		 grZoneGeoAction.compte as Compte,
		 grZoneGeoAction.ordreCompte as ordre,
		 @dateInventaire,
		  'ENCOURS_ALL',
		  'Total Encours',
		 0, -- prendre la partie entiere seulement
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from  (select distinct Groupe,ssgroupe,Compte, ordreCompte from #Compte_ZoneGeoAction) as grZoneGeoAction 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Compte = grZoneGeoAction.Compte
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  group by grZoneGeoAction.Groupe, grZoneGeoAction.ssGroupe,grZoneGeoAction.compte,  grZoneGeoAction.ordreCompte
------------------------------------------------------------------------
--  Decomposition ACTION: pur action et fonds treso et cash
------------------------------------------------------------------------
  
insert 
into  #DATA(cle,groupe,ssGroupe,compte,ordre,date,rubrique,libelle ,classementRubrique,valeur )
select @cleReport as cle,
		 grZoneGeoAction.Groupe as Groupe,
		 grZoneGeoAction.ssGroupe as ssGroupe,
		 grZoneGeoAction.compte as Compte,
		 grZoneGeoAction.ordreCompte as ordre,
		 @dateInventaire,
		  'ACTIONS_ZONE_GEO',
		  'ACTIONS par Zone Géographique',
		 0.1 as classementRubrique, 
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from  (select distinct Groupe,ssGroupe,Compte, ordreCompte from #Compte_ZoneGeoAction where rubriquePrincipale =  'ACTIONS_ZONE_GEO') as grZoneGeoAction 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Compte = grZoneGeoAction.Compte
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and  tr.Type_actif = 'Actions' 
  group by grZoneGeoAction.Groupe,grZoneGeoAction.ssGroupe, grZoneGeoAction.compte,  grZoneGeoAction.ordreCompte

insert 
into  #DATA(cle,groupe,ssGroupe,compte,ordre,date,rubrique,libelle ,classementRubrique,valeur )
select @cleReport as cle,
		 grZoneGeoAction.Groupe as Groupe,
		 grZoneGeoAction.ssGroupe as ssGroupe,
		 grZoneGeoAction.compte as Compte,
		 grZoneGeoAction.ordreCompte as ordre,
		 @dateInventaire,
		  'ACTIONS_ZONE_GEO_PUR',
		  'expo ACTIONS - titres',
		 0.11 as classementRubrique, 
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from  (select distinct Groupe,ssGroupe,Compte, ordreCompte from #Compte_ZoneGeoAction where rubriquePrincipale =  'ACTIONS_ZONE_GEO') as grZoneGeoAction 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Compte = grZoneGeoAction.Compte
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and ( tr.Secteur IN (select libelle from [SECTEUR] 
                       where ( id  like 'A %'
                            or id = 'F ACTIONS'
                              ) )       
       or ( tr.Secteur = 'Not Available' and tr.Type_actif = 'Actions' )
      )   
  group by grZoneGeoAction.Groupe,grZoneGeoAction.ssGroupe, grZoneGeoAction.compte,  grZoneGeoAction.ordreCompte
  
  
insert 
into  #DATA(cle,groupe,ssGroupe,compte,ordre,date,rubrique,libelle ,classementRubrique,valeur )
select @cleReport as cle,
		 grZoneGeoAction.Groupe as Groupe,
		 grZoneGeoAction.ssGroupe as ssGroupe,
		 grZoneGeoAction.compte as Compte,
		 grZoneGeoAction.ordreCompte as ordre,
		 @dateInventaire,
		  'ACTIONS_ZONE_GEO_TRESO',
		  'expo ACTIONS - Liquidite',
		 0.12 as classementRubrique, 
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from  (select distinct Groupe,ssGroupe,Compte, ordreCompte from #Compte_ZoneGeoAction where rubriquePrincipale =  'ACTIONS_ZONE_GEO') as grZoneGeoAction 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Compte = grZoneGeoAction.Compte
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Type_actif = 'Actions' 
  and tr.Secteur NOT IN (select libelle from [SECTEUR] 
                       where ( id  like 'A %'
                            or id = 'F ACTIONS'
                              ) )
  and tr.Secteur <> 'Not Available'
  group by grZoneGeoAction.Groupe,grZoneGeoAction.ssGroupe, grZoneGeoAction.compte,  grZoneGeoAction.ordreCompte
  
  
  ------------------------------------------------------------------------
  -- par zone geo
insert 
into  #DATA(cle,groupe,ssGroupe,compte,ordre,date,rubrique,libelle ,classementRubrique,valeur )
  select @cleReport as cle,
  		 grZoneGeoAction.Groupe as Groupe,
  		 grZoneGeoAction.ssGroupe as ssGroupe,
		 grZoneGeoAction.compte as compte,		 
		 grZoneGeoAction.ordreCompte as ordre,
		 @dateInventaire,
		  grZoneGeoAction.rubriquePrincipale+grZoneGeoAction.ZoneGeo as rubrique,
		  grZoneGeoAction.ZoneGeoLibelle  as libelle,
		 grZoneGeoAction.classifRubrique as classementRubrique, 
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from  (select * from #Compte_ZoneGeoAction where pays is null and ZoneGeo <> 'NA')  as grZoneGeoAction 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Compte = grZoneGeoAction.Compte
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Zone_Géo = grZoneGeoAction.ZoneGeo 
  where  ( tr.Secteur IN (select libelle from [SECTEUR] 
                       where ( id  like 'A %'
                            or id = 'F ACTIONS'
                              ) )       
       or ( tr.Secteur = 'Not Available' and tr.Type_actif = 'Actions' ) 
          )
  group by  grZoneGeoAction.groupe,grZoneGeoAction.ssGroupe,grZoneGeoAction.compte,  grZoneGeoAction.ordreCompte, grZoneGeoAction.ZoneGeoLibelle, grZoneGeoAction.rubriquePrincipale,grZoneGeoAction.ZoneGeo,grZoneGeoAction.classifRubrique
  
 
insert 
into  #DATA(cle,groupe,ssgroupe,compte,ordre,date,rubrique,libelle ,classementRubrique,valeur )
  select @cleReport as cle,
		 grZoneGeoAction.Groupe as Groupe,
		 grZoneGeoAction.ssGroupe as ssGroupe,
		 grZoneGeoAction.compte as compte,
		 grZoneGeoAction.ordreCompte as ordreCompte,
		 @dateInventaire,
		 grZoneGeoAction.rubriquePrincipale+'_NA' as rubrique,
		 '   Non Attribué' as libelle,
		 grZoneGeoAction.classifRubrique as classementRubrique,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from  #Compte_ZoneGeoAction  as grZoneGeoAction 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Compte = grZoneGeoAction.compte
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and ( tr.Zone_Géo = 'NA' or tr.Zone_Géo is null ) 
  and grZoneGeoAction.ZoneGeo ='NA'
  and grZoneGeoAction.Pays is null
  where 
  ( tr.Secteur IN (select libelle from [SECTEUR] 
                       where ( id  like 'A %'
                            or id = 'F ACTIONS'
                              ) )       
       or ( tr.Secteur = 'Not Available' and tr.Type_actif = 'Actions' )
  )       
  group by grZoneGeoAction.Groupe, grZoneGeoAction.ssGroupe,grZoneGeoAction.compte,  grZoneGeoAction.ordreCompte, grZoneGeoAction.rubriquePrincipale,grZoneGeoAction.classifRubrique
  
-- detail : par pays  
insert 
into  #DATA(cle,groupe,ssgroupe,compte,ordre,date,rubrique,libelle ,classementRubrique,valeur )
  select @cleReport as cle,
		 grZoneGeoAction.Groupe as Groupe,
		 grZoneGeoAction.ssGroupe as ssGroupe,
		 grZoneGeoAction.compte as compte,
		 grZoneGeoAction.ordreCompte as ordreCompte,
		 @dateInventaire,
		 grZoneGeoAction.rubriquePrincipale+grZoneGeoAction.ZoneGeo+tr.Pays as rubrique,
		 grZoneGeoAction.ZoneGeoLibelle as libelle,
		 grZoneGeoAction.classifRubrique,
         SUM( Valeur_Boursiere + Coupon_Couru ) as valeur
  from  (select * from #Compte_ZoneGeoAction where pays is not null)  as grZoneGeoAction 
  left outer join [PTF_TRANSPARISE] as tr 
  on tr.Compte = grZoneGeoAction.Compte
  and Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and tr.Zone_Géo = grZoneGeoAction.ZoneGeo
  and tr.Pays =  grZoneGeoAction.Pays
  where  ( tr.Secteur IN (select libelle from [SECTEUR] 
                       where ( id  like 'A %'
                            or id = 'F ACTIONS'
                              ) )       
       or ( tr.Secteur = 'Not Available' and tr.Type_actif = 'Actions' ) 
          )
  group by tr.Pays,grZoneGeoAction.groupe,grZoneGeoAction.ssgroupe,grZoneGeoAction.compte,  grZoneGeoAction.ordreCompte, grZoneGeoAction.ZoneGeoLibelle, grZoneGeoAction.rubriquePrincipale,grZoneGeoAction.ZoneGeo,grZoneGeoAction.classifRubrique
  
  
-----------------------------------------------------------------------------------
-- FIN DES PIVOT
insert into #MMARRCORT (date,ordre,classementRubrique,libelle,groupe,ssgroupe,encours,ref) 
select date,ordre, classementRubrique,libelle,groupe,ssgroupe,SUM(isnull(valeur,0))as encours
, case  
when classementRubrique>=1 and classementRubrique = ROUND(classementRubrique,0,1) then 0.10
when classementRubrique = 0.1 then 0
when classementRubrique = 0.11 then 0.1
when classementRubrique = 0.12 then 0.1 
else null end as ref -- valeur prise en reference pour le pourcentage
 from #DATA
where ssgroupe = 'MM ARRCO RT'
and date = @dateInventaire
group by date,libelle,groupe,ssgroupe,ordre, classementRubrique
 
insert into #PREVOYANCE (date,ordre,classementRubrique,libelle,groupe,ssgroupe,encours,ref)
select date,ordre, classementRubrique,libelle,groupe,ssgroupe,SUM(isnull(valeur,0))as encours
, case  
when classementRubrique>=1 and classementRubrique = ROUND(classementRubrique,0,1) then 0.10
when classementRubrique = 0.1 then 0
when classementRubrique = 0.11 then 0.1
when classementRubrique = 0.12 then 0.1 
else null end as ref -- valeur prise en reference pour le pourcentage

from #DATA
where groupe in ('MMP', 'MM MUTUELLE', 'INPR', 'QUATREM', 'AUXIA', 'CMAV')
and date = @dateInventaire 
group by date,libelle,groupe,ssgroupe,ordre, classementRubrique
-- loop for the 3 dates
IF @dateInventaire = @dateInventaireToday 
 BEGIN
	set @dateInventaire = @dateInventaireMTD 
	GOTO START  
 END
ELSE
 BEGIN
  IF @dateInventaire = @dateInventaireMTD 
  BEGIN
	set @dateInventaire = @dateInventaireYTD 
	GOTO START      
  END
 END
--------------------------------------------------------------------------
-- END OF LOOP
--------------------------------------------------------------------------
--select * from #compte_ZoneGeoAction order by ordreCompte,ssgroupe, compte,classifRubrique


--select * from #DATA where ssgroupe = 'QUATREM' and date='18/03/2015'
--order by ordre ,compte, classementRubrique 

---------------------------------------------------------
-- End of DATA collecting
DECLARE @sqlrequest nvarchar(max); 

select e.*, e.encours/ref.encours from #MMARRCORT as e
left outer join #MMARRCORT as ref on ref.classementRubrique = e.ref and ref.date = e.date
where e.date = @dateInventaireToday
order by ordre , groupe,ssgroupe,classementRubrique 


SET @sqlrequest = N'select e.*, e.encours/ref.encours as ''pc'', eM.encours as '''+convert(char(10), @dateInventaireMTD,103)+''',eM.encours/refM.encours as ''pc'',
eY.encours as '''+convert(char(10),@dateInventaireYTD,103) +''',eY.encours/refY.encours as ''pc'' from #MMARRCORT as e
left outer join #MMARRCORT as ref on ref.classementRubrique = e.ref and ref.date = e.date
left outer join #MMARRCORT as eM on eM.classementRubrique = e.classementRubrique  and eM.date = '''+convert(char(10), @dateInventaireMTD,103)+'''
left outer join #MMARRCORT as refM on refM.classementRubrique = eM.ref and refM.date = eM.date
left outer join #MMARRCORT as eY on eY.classementRubrique = e.classementRubrique  and eY.date = '''+convert(char(10), @dateInventaireYTD,103)+'''
left outer join #MMARRCORT as refY on refY.classementRubrique = eM.ref and refY.date = eY.date
where e.date = '''+convert(char(10), @dateInventaireToday,103)+'''
order by ordre , groupe,ssgroupe,classementRubrique' 
print @sqlrequest
exec(@sqlrequest)


select e.*, case 
  when ref.encours is null or ref.encours = 0 then 0
  else e.encours/ref.encours end as 'pc' from #PREVOYANCE as e
left outer join #PREVOYANCE as ref on ref.classementRubrique = e.ref and ref.groupe = e.groupe and ref.ssgroupe = e.ssgroupe  and ref.date = e.date
where e.date = @dateInventaireToday
order by ordre , groupe,ssgroupe,classementRubrique 

SET @sqlrequest = N'select e.*, case 
  when ref.encours is null or ref.encours = 0 then NULL
  else e.encours/ref.encours end as ''pc'',
  eM.encours as '''+convert(char(10), @dateInventaireMTD,103)+''', case when refM.encours is null or refM.encours = 0 then NULL
  else eM.encours/refM.encours end as ''pc''
  ,
eY.encours as '''+convert(char(10), @dateInventaireYTD,103)+''', case when refY.encours is null or refY.encours = 0 then NULL
  else eY.encours/refY.encours  end as ''pc''
from #PREVOYANCE as e  
left outer join #PREVOYANCE as ref on ref.classementRubrique = e.ref and ref.groupe = e.groupe and ref.ssgroupe = e.ssgroupe  and ref.date = e.date
left outer join #PREVOYANCE as eM on eM.classementRubrique = e.classementRubrique  and eM.groupe = e.groupe and eM.ssgroupe = e.ssgroupe   and eM.date = '''+convert(char(10), @dateInventaireMTD,103)+'''
left outer join #PREVOYANCE as refM on refM.classementRubrique = eM.ref and refM.groupe = e.groupe and refM.ssgroupe = e.ssgroupe   and refM.date = eM.date
left outer join #PREVOYANCE as eY on eY.classementRubrique = e.classementRubrique and eY.groupe = e.groupe and eY.ssgroupe = e.ssgroupe   and eY.date = '''+convert(char(10), @dateInventaireYTD,103)+'''
left outer join #PREVOYANCE as refY on refY.classementRubrique = eM.ref and refY.groupe = e.groupe and refY.ssgroupe = e.ssgroupe   and refY.date = eY.date
where e.date = '''+convert(char(10), @dateInventaireToday,103)+'''
order by ordre , groupe,ssgroupe,classementRubrique' 
exec(@sqlrequest)


--- affichage prevoyance en tableau 



--CREATE TABLE #PREVOYANCE_TABLEAU(
--    [date] [datetime] NOT NULL,
--    [groupe] [varchar](25) NOT NULL,
--    [ssgroupe] [varchar](25) NOT NULL, -- rassemblement de comptes
--    [libelle] [varchar](60) NULL,
--	[ordre] [int] NULL,
--    [encours] [float] NULL, -- total sur le libellé
--	[poidsActions] [float] NULL,
--	[poidsActionsBenchmark] [float] NULL,

--	[poidsEuro] [float] NULL,

--	[poidsExEuro] [float] NULL,

--	[poidsUS] [float] NULL,
--	[poidsAsie] [float] NULL,
--	[poidsEmergent] [float] NULL,	
--	[poidsNA] [float] NULL	 -- la partie qui n est pas definie en Zone Géo
--)

--insert into #PREVOYANCE_TABLEAU ( date,groupe,ssgroupe,libelle, ordre,encours)
--select date,groupe,ssgroupe,libelle, ordre, SUM(isnull(valeur,0))as encours
--from #DATA
--where groupe in ('MMP', 'MM MUTUELLE', 'INPR', 'QUATREM', 'AUXIA', 'CMAV')
--and date = '31/03/2015' 
--and rubrique ='ENCOURS_ALL'
--group by date,libelle,groupe,ssgroupe,ordre, classementRubrique

--select * from #PREVOYANCE_TABLEAU


--select e.*, case 
--  when ref.encours is null or ref.encours = 0 then 0
--  else e.encours/ref.encours end as 'pc' from #PREVOYANCE as e
--left outer join #PREVOYANCE as ref on ref.classementRubrique = e.ref and ref.groupe = e.groupe and ref.ssgroupe = e.ssgroupe  and ref.date = e.date
--where e.date = @dateInventaireToday
--order by ordre , groupe,ssgroupe,classementRubrique 


drop table #PREVOYANCE
drop table #MMARRCORT
  
drop table #Groupe
drop table #Compte
drop table #Compte_ZoneGeoAction
drop table #ZoneGeoPaysAction
drop table #SousGroupeCalcule  
drop table #DATA
  
  