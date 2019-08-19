--------------------------------------------------
-- Fichier en encodage UTF-8 (ANSI ou ASCII fonctionne si aucun caractères accentués)

-- REQUETE DE SYNCHRONISATION entre OMEGA et la base FGA Soft (ref.PORTFOLIO  et dbo.PTF_SSGROUPE )
--  exploite la composition des groupes / sous groupes (personnalisés notamment)
-- pour les OPCVMS : configuration multi parts

-- TODO: ref.PORTFOLIO  doit migrer vers ref_holding.PORTFOLIO qui modeliserait les mandats, opcvms, fonds modèles

-- A EXECUTER A LA DEMANDE dès qu un nouveau fond apparait ou disparait , nouveau groupe ...
--------------------------------------------------------------------------------------------------------------------------------

use omega
declare @dateInventaire as datetime
--set  @dateInventaire = (select top 1 _DATEOPERATION from fcp.vlrstion v where _DATEOPERATION > '01/04/2015' order by _DATEOPERATION )
set  @dateInventaire = '31/03/2015'


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

-- FIN de LA LISTE DES COMPTES
select * from #COMPTES

-- DECLARATION DES SOUS GROUPES PRINCIPAUX
declare @ssgroupList as table (ssgr char(2), groupe varchar(30))
insert into @ssgroupList (ssgr,groupe) VALUES ('AU','AUXIA')
insert into @ssgroupList (ssgr,groupe) VALUES ('MT','AUXIA')
insert into @ssgroupList (ssgr,groupe) VALUES ('C','MMP')
insert into @ssgroupList (ssgr,groupe) VALUES ('CM','CMAV')
insert into @ssgroupList (ssgr,groupe) VALUES ('MI','CMAV')
insert into @ssgroupList (ssgr,groupe) VALUES ('CQ','CMAV')
insert into @ssgroupList (ssgr,groupe) VALUES ('CR','CMAV')
insert into @ssgroupList (ssgr,groupe) VALUES ('SO','CMAV')
insert into @ssgroupList (ssgr,groupe) VALUES ('A5','IRCEM MUTUELLE')
insert into @ssgroupList (ssgr,groupe) VALUES ('A4','IRCEM PREVOYANCE')
insert into @ssgroupList (ssgr,groupe) VALUES ('A6','IRCEM RETRAITE')
insert into @ssgroupList (ssgr,groupe) VALUES ('PV','LA PORTE VERTE')
insert into @ssgroupList (ssgr,groupe) VALUES ('MD','MMP')
insert into @ssgroupList (ssgr,groupe) VALUES ('GG','MM AGIRC')
insert into @ssgroupList (ssgr,groupe) VALUES ('GS','MM AGIRC')
insert into @ssgroupList (ssgr,groupe) VALUES ('GT','MM AGIRC')
insert into @ssgroupList (ssgr,groupe) VALUES ('RG','MM ARRCO')
insert into @ssgroupList (ssgr,groupe) VALUES ('RS','MM ARRCO')
insert into @ssgroupList (ssgr,groupe) VALUES ('RT','MM ARRCO')
insert into @ssgroupList (ssgr,groupe) VALUES ('AS','MM ASSURANCES')
insert into @ssgroupList (ssgr,groupe) VALUES ('M4','MM MUTUELLE')
insert into @ssgroupList (ssgr,groupe) VALUES ('M1','MM MUTUELLE')
insert into @ssgroupList (ssgr,groupe) VALUES ('MB','MM MUTUELLE')
insert into @ssgroupList (ssgr,groupe) VALUES ('UA','MMP')
insert into @ssgroupList (ssgr,groupe) VALUES ('R2','MMP')
insert into @ssgroupList (ssgr,groupe) VALUES ('MQ','MMP')
insert into @ssgroupList (ssgr,groupe) VALUES ('R1','MMP')
insert into @ssgroupList (ssgr,groupe) VALUES ('RE','MMP')
insert into @ssgroupList (ssgr,groupe) VALUES ('R3','MMP')
insert into @ssgroupList (ssgr,groupe) VALUES ('PE','MMP')
insert into @ssgroupList (ssgr,groupe) VALUES ('AL','MUTUELLE ALLASSO')
insert into @ssgroupList (ssgr,groupe) VALUES ('QU','QUATREM')
insert into @ssgroupList (ssgr,groupe) VALUES ('QA','QUATREM')
insert into @ssgroupList (ssgr,groupe) VALUES ('QF','QUATREM')
insert into @ssgroupList (ssgr,groupe) VALUES ('QO','QUATREM')
insert into @ssgroupList (ssgr,groupe) VALUES ('QE','QUATREM')
insert into @ssgroupList (ssgr,groupe) VALUES ('QM','QUATREM')
insert into @ssgroupList (ssgr,groupe) VALUES ('QP','QUATREM')
insert into @ssgroupList (ssgr,groupe) VALUES ('QR','QUATREM')
insert into @ssgroupList (ssgr,groupe) VALUES ('QS','QUATREM')

declare @GroupeComposition as table ( groupeCalcule varchar(25) COLLATE DATABASE_DEFAULT , groupeComposant varchar(25) COLLATE DATABASE_DEFAULT null)
insert @GroupeComposition values ('RETRAITE', 'MM AGIRC')
insert @GroupeComposition values ('RETRAITE', 'MM ARRCO')
insert @GroupeComposition values ('ASSURANCE', 'MMP')
insert @GroupeComposition values ('ASSURANCE', 'MM MUTUELLE')
insert @GroupeComposition values ('ASSURANCE', 'CMAV')
insert @GroupeComposition values ('ASSURANCE', 'QUATREM')
insert @GroupeComposition values ('ASSURANCE', 'AUXIA')
insert @GroupeComposition values ('ASSURANCE', 'CAPREVAL')
insert @GroupeComposition values ('ASSURANCE', 'INPR')
insert @GroupeComposition values ('ASSURANCE', 'AUTRES')
-- les Groupes personnalisés :
-- ALLOCATION: Pour Robert , un regroupement diminué de MMP et QUATREM
insert @GroupeComposition values ('ALLOCATION', 'MMP ALLOC')
insert @GroupeComposition values ('ALLOCATION', 'QUATREM ALLOC')
insert into @ssgroupList (ssgr,groupe) VALUES ('R2','MMP ALLOC')
insert into @ssgroupList (ssgr,groupe) VALUES ('R1','MMP ALLOC')
insert into @ssgroupList (ssgr,groupe) VALUES ('R3','MMP ALLOC')
insert into @ssgroupList (ssgr,groupe) VALUES ('RE','MMP ALLOC')
insert into @ssgroupList (ssgr,groupe) VALUES ('QR','QUATREM ALLOC')
insert into @ssgroupList (ssgr,groupe) VALUES ('QP','QUATREM ALLOC')
insert into @ssgroupList (ssgr,groupe) VALUES ('QF','QUATREM ALLOC')
insert into @ssgroupList (ssgr,groupe) VALUES ('QS','QUATREM ALLOC')


select compo.groupeCalcule as 'type',  groupe as 'pere',g._nomgroupe as 'ssgroupe',cg._compte as 'compte' from @ssgroupList as ss
left outer join Fcp.CONSGRPE as cg on cg._codegroupe = ss.ssgr
left outer join fcp.grpedeft g                on g._codegroupe=cg._codegroupe
left outer join @GroupeComposition as compo on compo.groupeComposant = ss.groupe
where g._nomgroupe is not null --exclusion des groupes inexistants 
order by _nomgroupe

drop table #COMPTES

----------------------------------------------------
-- POUR ARCHIVE : creation de la table de synchro
-- sur la base FGA (virer les tables PTF_LOT*)
----------------------------------------------------

-- objectif :donner la liste des numero de ptf concerné par un regroupement fonctionnel (type = assurance ou retraite) 
-- ou niveau client avec les groupes MM AGIRC , et même des regroupements personnalisés (pour l ALLOCATION)
--create table dbo.PTF_SSGROUPE
--(
--	[Id] int IDENTITY(1,1) NOT NULL,
--	[type] [varchar](60) NULL,
--	[pere] [varchar](60) NULL,
--	[ssgroupe] [varchar](60) NULL,
--	[ptf] [varchar](60) NULL
--PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]

--ALTER TABLE ref.PORTFOLIO	
--ADD Type varchar(60) NULL
--ALTER TABLE ref.PORTFOLIO
--ADD ISIN char(12) NULL
