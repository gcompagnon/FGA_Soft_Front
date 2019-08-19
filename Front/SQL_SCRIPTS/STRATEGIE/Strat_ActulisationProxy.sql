

CREATE TABLE #Proxy(
	[Date] [datetime] NOT NULL,
	[Code_Proxy] [varchar](20) NOT NULL,
	[Libelle_Proxy] [varchar](60) NULL,
	[code_titre] [varchar](20) NOT NULL,
	[Libelle_Titre] [varchar](60) NULL,
	[Poids_VB] [float] NULL,
	[Poids_CC] [float] NULL,
	[Type_Produit] [varchar](60) NULL,
	[Devise_Titre] [varchar](3) NULL,
	[Secteur] [varchar](60) NULL,
	[Sous_Secteur] [varchar](60) NULL,
	[Pays] [varchar](30) NULL,
	[Emetteur] [varchar](60) NULL,
	[Rating] [varchar](4) NULL,
	[Groupe_Emet] [varchar](60) NULL,
	[Maturité] [datetime] NULL,
	[Duration] [float] NULL,
	[Sensibilite] [float] NULL
	)

Declare @DerniereDate Datetime
Set @DerniereDate  = (select max(date) from ptf_proxy)

insert into #proxy 
select * from ptf_proxy where date =  @DerniereDate

update #proxy set date = '01-01-2099' where date = @DerniereDate

Select * from #Proxy

drop table #Proxy

