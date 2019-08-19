
declare @dateInventaire as datetime
set  @dateInventaire = '27/06/2012'
declare @niveauInventaire as tinyint
set @niveauInventaire = 4

declare @cleReport as char(20)

IF @cleReport IS NULL
BEGIN
  set @cleReport = 'MonitoringRating'
END

create table #Groupe  ( groupe varchar(25) primary key , ordreGroupe smallint null)
insert #Groupe values ('MM AGIRC',1)
insert #Groupe values ('MM ARRCO',2)

insert #Groupe values ('MMP',4)
insert #Groupe values ('INPR',5)
insert #Groupe values ('CAPREVAL',6)
insert #Groupe values ('CMAV',7)
insert #Groupe values ('MUT2M',8)
insert #Groupe values ('SAPREM',9)
insert #Groupe values ('AUXIA',10)
insert #Groupe values ('QUATREM',11)
insert #Groupe values ('AUTRES',12)
declare @maxGroup int 
set @maxGroup = 13
--Les nouveaux groupes seront places arbitrairement � la suite

-- temporaire pour stocker les groupes pr�sents dans les inventaires transparis�s 
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


create table #rating (ordre tinyint, rating varchar(5))
insert into #rating values (1,'AAA')
insert into #rating values (2,'AA+')
insert into #rating values (3,'AA')
insert into #rating values (4,'AA-')
insert into #rating values (5,'A+')
insert into #rating values (6,'A')
insert into #rating values (7,'A-')
insert into #rating values (8,'BBB+')
insert into #rating values (9,'BBB')
insert into #rating values (10,'BBB-')
insert into #rating values (11,'BB+')
insert into #rating values (12,'BB')
insert into #rating values (13,'BB-')
insert into #rating values (14,'B+')
insert into #rating values (15,'B')
insert into #rating values (16,'B-')
insert into #rating values (17,'CCC+')
insert into #rating values (18,'CCC')
insert into #rating values (19,'CCC-')
insert into #rating values (20,'CC')
insert into #rating values (21,'C')
insert into #rating values (22,'D')
insert into #rating values (23,'CD')
insert into #rating values (24,'NR')

select * into #groupe_rating
from #groupe, #rating



create table #res ( cle char(20), groupe varchar(25), ordreGroupe tinyint, dateInventaire dateTime,  rubrique varchar(25), libelle varchar(25), classementRubrique tinyint, valeur float)
insert into #res
Select @cleReport as cle,
		gr.groupe,
		gr.ordreGroupe,
		@dateinventaire as dateInventaire,
		gr.rating as rubrique,
		gr.rating as libelle,
		gr.ordre as classementrubrique,
		SUM(tr.valeur_boursiere+coupon_couru) as valeur
	FROM #groupe_rating as gr
	left outer join [dbo].[PTF_TRANSPARISE] as tr 
	on tr.Groupe = gr.Groupe
	and tr.Dateinventaire = @dateInventaire
	and tr.Numero_Niveau = @niveauInventaire  
	and tr.rating=gr.rating
	and tr.Secteur in (select libelle from #secteurObligations )
	GROUP BY gr.rating, gr.groupe, gr.ordreGroupe, gr.ordre
	order by ordregroupe, classementRubrique
		
		
		
/**Calcul des valeurs par GROUPE**/		
insert into #res 
SELECT @cleReport as cle,
		r.groupe as groupe,
		r.ordreGroupe,
		@dateInventaire as dateInventaire,
		'ENCOURS - Total' as rubrique,
		'ENCOURS - Total' as libelle,
		0 as classementRubrique,
		SUM(r.valeur) as valeur
	FROM #res as r
	GROUP BY r.groupe, r.ordreGroupe
		

/**Calcul des valeurs par RATING**/		
insert into #res 
SELECT @cleReport as cle,
		'FGA' as groupe,
		0 as ordreGroupe,
		@dateInventaire as dateInventaire,
		r.libelle as rubrique,
		r.libelle as libelle,
		r.classementRubrique as classementRubrique,
		SUM(r.valeur) as valeur
	FROM #res as r
	GROUP BY r.libelle, r.classementRubrique


SELECT * FROM #res ORDER BY ordreGroupe, classementRubrique

drop table #rating
drop table #groupe
drop table #secteurObligations
drop table #groupe_rating
drop table #res
