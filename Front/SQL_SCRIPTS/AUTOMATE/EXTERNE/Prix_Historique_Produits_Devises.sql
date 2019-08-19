----------------------------------------------------------
-- Retourne l historique des prix CLose pour des produits ou des devises
----------------------------------------------------------
-- Paramètres: @date_start et @date_end (par défaut:aujourd hui à 40 jours): entre les 2 dates
--             @code_produit1 à @code_produit7: le ou les codes produits à lister
--             @like_code_produit1 à @like_code_produit2: des portions de codes pour lister plus de produits , par ex 'FW%' pour l ensemble des produits Forwards
--             @code_devise2 à @code_devise2 : la devise voulu contre EUR (par exemple : USD)

use OMEGA

declare @code_produit1 VARCHAR(15)
set @code_produit1 = null
--set @code_produit1 = 'SBF120NET'
declare @code_produit2 VARCHAR(15)
set @code_produit2 = null
declare @code_produit3 VARCHAR(15)
set @code_produit3 = null
declare @code_produit4 VARCHAR(15)
set @code_produit4 = null
declare @code_produit5 VARCHAR(15)
set @code_produit5 = null
declare @code_produit6 VARCHAR(15)
set @code_produit6 = null
declare @code_produit7 VARCHAR(15)
set @code_produit7 = null

declare @like_code_produit1 VARCHAR(15)
set @like_code_produit1 = null
--set @like_code_produit1 = 'FW%'
declare @like_code_produit2 VARCHAR(15)
set @like_code_produit2 = null

declare @code_devise1 CHAR(3)
set @code_devise1 = null
--set @code_devise1 = 'USD'
declare @code_devise2 CHAR(3)
set @code_devise2 = null

declare @date_end datetime
set @date_end = GETDATE()

declare @date_start datetime
set @date_start = DATEADD(day, -40, @date_end)

-- creation de la table des données en sortie (utile pour classer sur des colonnes non visibles)
create table #HISTORIQUE_COURS ( Code varchar(15) not null ,Libelle varchar(max), Date char(10) not null, Cours_Close float null, Date1 datetime not null,PRIMARY KEY (Code,Date1) ) 

insert into #HISTORIQUE_COURS
select d._CODEDEVISE+d._CONTREDEVISE as 'Code','Devise '+d._CONTREDEVISE as 'Libelle',convert(varchar,d._DATE,103) as 'Date',d._COURSCLOSE as 'Cours_Close',d._DATE 
from com.devhist d 
where d._CODEDEVISE = 'EUR' and d._CONTREDEVISE IN ( @code_devise1, @code_devise2)
and d._CONTREDEVISE is not null
and d._DATE between @date_start and @date_end
UNION
select  d._CODEPRODUI as 'Code',p._LIBELLE1PROD as 'Libelle', convert(varchar,d._DATE,103) as 'Date',d._COURSCLOSE as 'Cours_Close',d._DATE
from com.PRIXHIST as d
left outer join com.PRODUIT as p on p._codeprodui = d._codeprodui 
 where ( d._CODEPRODUI IN(  @code_produit1, @code_produit2, @code_produit3, @code_produit4, @code_produit5, @code_produit6, @code_produit7) 
 or ( d._CODEPRODUI like @like_code_produit1 ) or ( d._CODEPRODUI like @like_code_produit2 )
 )
 and d._CODEPRODUI is not null
 and d._DATE between @date_start and @date_end

-- sortie
select Code, Libelle,Date, Cours_Close from #HISTORIQUE_COURS order by Code,Date1 desc

drop table #HISTORIQUE_COURS