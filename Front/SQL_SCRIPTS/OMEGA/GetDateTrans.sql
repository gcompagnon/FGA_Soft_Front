--Recherche tous les dates transparisables

declare @compteReference varchar(10)
set @compteReference = '7201106' -- MM AGIRC RT ACTIONS
--set @compteReference = '6100001' -- FCP FEDERIS ACTIONS FRANCE



select distinct CONVERT(VARCHAR,v._dateoperation,103) As Date, v._dateoperation  
 from fcp.valoprdt v where v._compte=@compteReference and v._dateoperation >'25/12/2010'
ORDER BY v._dateoperation DESC