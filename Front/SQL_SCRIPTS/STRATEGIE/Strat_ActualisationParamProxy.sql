
CREATE TABLE #ParamProxy(
	[Date] [datetime] NOT NULL,
	[Isin_Titre] [varchar](12) NOT NULL,
	[Libellé_titre] [varchar](60) NOT NULL,
	[Code_Proxy] [varchar](60) NULL,
	[Libellé_Proxy] [varchar](60) NULL,
	[Source] [varchar](30) NULL
	)
	
	
Declare @DerniereDate Datetime
Set @DerniereDate  = (select max(date) from ptf_param_proxy)


insert into #ParamProxy 
select * from ptf_param_proxy where date = @DerniereDate and code_proxy <> 'FR0011409861'
--Pb avec le titre FR0011409861. Il ne remonte pas d'omega donc on ne va pas le transparise


update #ParamProxy set date = '01-01-2099' where date = @DerniereDate

select * from #ParamProxy

drop table #ParamProxy
