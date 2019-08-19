
/****** Object:  StoredProcedure [dbo].[ReportMonitoringMaturiteSe]    Script Date: 09/18/2012 10:09:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


 
/** configuration du rapport SupervisionGroupe:
Les regroupements/groupes des comptes clients sont déduits des positions Omega 
presentes dans les inventaires en transparence

Pour chaque groupe, des rubriques définies par libellé sont fournies avec leur valeur.(NULL pour une rubrique non définies pour un groupe) 

Chaque groupe possède un ordre prédéfini pour l'affichage (cet ordre est unique pour un groupe donné)
Dans un groupe donné, la rubrique possède un ordre en priorité de l'ordre naturel (alphabétique)

Chaque rapport est identifié par une clé et une date (se rapportant à l'inventaire) ce qui garantie que les données sont cohérentes entre elles suivant ces 2 valeurs.
**/  

--USE FGA_DEV
-- avec l'utilisateur fga 
--GO
-- execute as fga
-- DROP PROCEDURE ReportMonitoringGroupe


ALTER PROCEDURE [dbo].[ReportMonitoringMaturiteSe] 
        @dateInventaire  datetime ,
        @niveauInventaire tinyint ,
        @cleReport char(20) OUTPUT
AS

/** Parametres: --------------------------------------**/
/*
declare @dateInventaire as datetime
set  @dateInventaire = '31/01/2012'
declare @niveauInventaire as tinyint
set @niveauInventaire = 3
declare @cleReport as char(20)
*/
IF @cleReport IS NULL
BEGIN
  set @cleReport = 'MonitoringMaturiteSe'
END

/** Parametres: ---FIN--------------------------------**/

-- test d existence de la date demandée pour l'inventaire
/** TODO **/
--select distinct DateInventaire from  [PTF_TRANSPARISE] order by DateInventaire
-- select distinct Groupe from  [PTF_TRANSPARISE] where DateInventaire = '2011-08-24'

-- test d existence du niveau d inventaire
/** TODO **/

-- suppression pour la date demandée mais on pourra archiver sur une autre table ces rapports 
delete from [PTF_RAPPORT]  
   where date = @dateInventaire 
   and cle = @cleReport


create table #Groupe  ( groupe varchar(25) COLLATE DATABASE_DEFAULT primary key , ordreGroupe smallint null)
insert #Groupe values ('MM AGIRC',1)
insert #Groupe values ('MM ARRCO',2)

insert #Groupe values ('MMP',4)
insert #Groupe values ('INPR',5)
insert #Groupe values ('CAPREVAL',6)
insert #Groupe values ('CMAV',7)
insert #Groupe values ('MM MUTUELLE',8)
insert #Groupe values ('SAPREM',9)
insert #Groupe values ('AUXIA',10)
insert #Groupe values ('QUATREM',11)
insert #Groupe values ('AUTRES',12)
declare @maxGroup int 
set @maxGroup = 13


-- temporaire pour stocker les groupes présents dans les inventaires transparisés 
   insert into #Groupe
  select distinct Groupe, -1 as ordreGroupe   
  from [PTF_TRANSPARISE]
  where Dateinventaire = @dateInventaire
  and Numero_Niveau = @niveauInventaire
  and Groupe not in (select groupe from #Groupe)

  update #Groupe 
  set   ordreGroupe = @maxGroup,
  @maxGroup = @maxGroup +1 
  where ordreGroupe = -1
  
 -- une table rassemblant tous les secteurs de type Obligataire 
    select libelle 
    into #secteurObligations
    from [SECTEUR] 
    where id like 'O CNF%' 
       or id like 'O CF%' 
       or id= 'F OBLIG' 
       or id =  'O GVT' 
       or id = 'O AGENCIES'
       or id = 'O COVERED'


create table #maturite (ordre tinyint, maturite varchar(15) COLLATE DATABASE_DEFAULT)
insert into #maturite values (1,'0-1 ans')
insert into #maturite values (2,'1-3 ans')
insert into #maturite values (3,'3-5 ans')
insert into #maturite values (4,'5-7 ans')
insert into #maturite values (5,'7-10 ans')
insert into #maturite values (6,'10-15 ans')
insert into #maturite values (7,'15-20 ans')
insert into #maturite values (8,'+ 20ans')


select * into #groupe_maturite
from #groupe, #maturite


create table #encours(groupe varchar (25) COLLATE DATABASE_DEFAULT, maturite varchar(15) COLLATE DATABASE_DEFAULT, encours float)
insert into #encours
SELECT gm.groupe as groupe,
		gm.maturite as maturite,
		SUM(valeur_boursiere+coupon_couru) as encours
	FROM #groupe_maturite as gm
	left outer join [dbo].[PTF_TRANSPARISE] as tr 
	on tr.Groupe = gm.Groupe
	and tr.Dateinventaire = @dateInventaire
	and tr.Numero_Niveau = @niveauInventaire  
	and tr.tranche_de_maturite=gm.maturite
	and tr.Secteur in (select libelle from #secteurObligations )
	GROUP BY  gm.groupe, gm.maturite
	ORDER BY gm.groupe
	
create table #sensibilite(groupe varchar (25) COLLATE DATABASE_DEFAULT, maturite varchar(15) COLLATE DATABASE_DEFAULT, sensi float)
insert into #sensibilite
SELECT gm.groupe as groupe,
		gm.maturite as maturite,
		SUM((valeur_boursiere+coupon_couru)*sensibilite) as sensi
	FROM #groupe_maturite as gm
	left outer join [dbo].[PTF_TRANSPARISE] as tr 
	on tr.Groupe = gm.Groupe
	and tr.Dateinventaire = @dateInventaire
	and tr.Numero_Niveau = @niveauInventaire  
	and tr.tranche_de_maturite=gm.maturite
	and tr.Secteur in (select libelle from #secteurObligations )
	GROUP BY gm.groupe, gm.maturite
	ORDER BY gm.groupe
	
	
create table #res ( cle char(25) COLLATE DATABASE_DEFAULT, groupe varchar(25) COLLATE DATABASE_DEFAULT, ordreGroupe tinyint, dateInventaire dateTime,  rubrique varchar(25) COLLATE DATABASE_DEFAULT, libelle varchar(25) COLLATE DATABASE_DEFAULT,classementRubrique tinyint, valeur float)

insert into #res
SELECT @cleReport as cle,
		gm.groupe as groupe,
		gm.ordreGroupe,
		@dateInventaire as dateInventaire,
		gm.maturite as rubrique,
		gm.maturite as libelle,
		gm.ordre as classementRubrique,
		s.sensi/e.encours as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.groupe=gm.groupe AND s.maturite = gm.maturite
	join #encours as e
		ON e.groupe=gm.groupe AND e.maturite = gm.maturite
	order by ordregroupe, classementRubrique

/**Calcul des valeurs pour le surGroupe RETRAITE**/	
insert into #res
SELECT @cleReport as cle,
		'RETRAITE' as groupe,
		0 as ordreGroupe,
		@dateInventaire as dateInventaire,
		gm.maturite as rubrique,
		gm.maturite as libelle,
		gm.ordre as classementRubrique,
		SUM(s.sensi)/SUM(e.encours) as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.groupe=gm.groupe AND s.maturite = gm.maturite
	join #encours as e
		ON e.groupe=gm.groupe AND e.maturite = gm.maturite
	WHERE gm.groupe in ('MM AGIRC','MM ARRCO')
	GROUP BY gm.maturite,gm.ordre
	order by classementRubrique

/**Calcul des valeurs pour le surGroupe ASSURANCE**/	
insert into #res
SELECT @cleReport as cle,
		'ASSURANCE' as groupe,
		0 as ordreGroupe,
		@dateInventaire as dateInventaire,
		gm.maturite as rubrique,
		gm.maturite as libelle,
		gm.ordre as classementRubrique,
		SUM(s.sensi)/SUM(e.encours) as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.groupe=gm.groupe AND s.maturite = gm.maturite
	join #encours as e
		ON e.groupe=gm.groupe AND e.maturite = gm.maturite
	WHERE gm.groupe in ('MMP','INPR','CAPREVAL','CMAV','MM MUTUELLE','SAPREM','AUXIA','QUATREM','AUTRES')
	GROUP BY gm.maturite,gm.ordre
	order by classementRubrique


/**Calcul des valeurs pour le surGroupe EXTERNE**/	
insert into #res
SELECT @cleReport as cle,
		'EXTERNE' as groupe,
		0 as ordreGroupe,
		@dateInventaire as dateInventaire,
		gm.maturite as rubrique,
		gm.maturite as libelle,
		gm.ordre as classementRubrique,
		SUM(s.sensi)/SUM(e.encours) as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.groupe=gm.groupe AND s.maturite = gm.maturite
	join #encours as e
		ON e.groupe=gm.groupe AND e.maturite = gm.maturite
	WHERE gm.groupe in ('ARCELOR MITTAL France','IDENTITES MUTUELLE','IRCEM MUTUELLE','IRCEM PREVOYANCE','IRCEM RETRAITE','UNMI')
	GROUP BY gm.maturite,gm.ordre
	order by classementRubrique

   
/**Calcul des valeurs par GROUPE**/	
insert into #res
SELECT @cleReport as cle,
		gm.groupe as groupe,
		gm.ordreGroupe,
		@dateInventaire as dateInventaire,
		'SENSIBILITE - Total' as rubrique,
		'SENSIBILITE - Total' as libelle,
		0 as classementRubrique,
		SUM(s.sensi)/SUM(e.encours) as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.groupe=gm.groupe
	join #encours as e
		ON e.groupe=gm.groupe
	GROUP BY gm.groupe, gm.ordreGroupe
	order by ordregroupe, classementRubrique


/**Calcul des valeurs par RATING**/	
insert into #res
SELECT @cleReport as cle,
		'FGA' as groupe,
		0  as ordreGroupe,
		@dateInventaire as dateInventaire,
		gm.maturite as rubrique,
		gm.maturite as libelle,
		gm.ordre as classementRubrique,
		SUM(s.sensi)/SUM(e.encours) as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.maturite = gm.maturite
	join #encours as e
		ON e.maturite = gm.maturite
	GROUP BY gm.maturite, gm.ordre
	order by ordregroupe, classementRubrique


/***Calcul du total RETRAITE***/
insert into #res
SELECT @cleReport as cle,
		'RETRAITE' as groupe,
		0 as ordreGroupe,
		@dateInventaire as dateInventaire,
		'SENSIBILITE - Total' as rubrique,
		'SENSIBILITE - Total' as libelle,
		0 as classementRubrique,
		SUM(s.sensi)/SUM(e.encours) as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.groupe in ('MM AGIRC','MM ARRCO') 
	join #encours as e
		ON e.groupe in ('MM AGIRC','MM ARRCO')
	order by ordregroupe, classementRubrique

/***Calcul du total ASSURANCE***/	
insert into #res
SELECT @cleReport as cle,
		'ASSURANCE' as groupe,
		0 as ordreGroupe,
		@dateInventaire as dateInventaire,
		'SENSIBILITE - Total' as rubrique,
		'SENSIBILITE - Total' as libelle,
		0 as classementRubrique,
		SUM(s.sensi)/SUM(e.encours) as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.groupe in ('MMP','INPR','CAPREVAL','CMAV','MM MUTUELLE','SAPREM','AUXIA','QUATREM','AUTRES')
	join #encours as e
		ON e.groupe in ('MMP','INPR','CAPREVAL','CMAV','MM MUTUELLE','SAPREM','AUXIA','QUATREM','AUTRES')
	order by ordregroupe, classementRubrique
	
/***Calcul du total EXTERNE***/	
insert into #res
SELECT @cleReport as cle,
		'EXTERNE' as groupe,
		0 as ordreGroupe,
		@dateInventaire as dateInventaire,
		'SENSIBILITE - Total' as rubrique,
		'SENSIBILITE - Total' as libelle,
		0 as classementRubrique,
		SUM(s.sensi)/SUM(e.encours) as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.groupe in ('ARCELOR MITTAL France','IDENTITES MUTUELLE','IRCEM MUTUELLE','IRCEM PREVOYANCE','IRCEM RETRAITE','UNMI')

	join #encours as e
		ON e.groupe in ('ARCELOR MITTAL France','IDENTITES MUTUELLE','IRCEM MUTUELLE','IRCEM PREVOYANCE','IRCEM RETRAITE','UNMI')

	order by ordregroupe, classementRubrique

/***Calcul du total obligataire***/
insert into #res
SELECT @cleReport as cle,
		'FGA' as groupe,
		0 as ordreGroupe,
		@dateInventaire as dateInventaire,
		'SENSIBILITE - Total' as rubrique,
		'SENSIBILITE - Total' as libelle,
		0 as classementRubrique,
		SUM(s.sensi)/SUM(e.encours) as valeur
	FROM #groupe_maturite as gm
	join #sensibilite as s
		ON s.groupe=gm.groupe 
	join #encours as e
		ON e.groupe=gm.groupe
    where  gm.groupe in ('ASSURANCE','RETRAITE','EXTERNE')				
	order by ordregroupe, classementRubrique
	
	

INSERT INTO ptf_RAPPORT
Select cle,
libelle,
valeur,
 groupe, 
 dateInventaire as date, 
 ordreGroupe,
  classementRubrique, 
  rubrique
from #res


drop table #maturite
drop table #groupe
drop table #secteurObligations
drop table #groupe_maturite
drop table #encours
drop table #sensibilite
drop table #res




GO

