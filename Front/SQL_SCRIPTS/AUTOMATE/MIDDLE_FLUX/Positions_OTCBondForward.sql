-- requete de sortie des prix du contrat bond forward ou Swap


-- TEST si la date de valorisation est bien égale à la date du jour
declare @value_date DATETIME
set @value_date = ( select CAST( FLOOR( CAST( GETDATE() AS FLOAT ) )AS DATETIME) )
--set @value_date = '06/04/2015'

create table #LAST_PRICE_FORWARD (code varchar(15) primary key, valueDate datetime not null, closePrice float null )

create table #LAST_PRICE_SWAP (code varchar(15) primary key, valueDate datetime not null, closePrice float null )

insert into #LAST_PRICE_FORWARD
select _codeprodui as code, max(_date),null as valueDate from com.prixhist as PR where PR._date<=@value_date and left(PR._codeprodui,2)='FW'
and PR._codeprodui <> 'FW14NA00187635'
group by _codeprodui

insert into #LAST_PRICE_SWAP
select s._codeprodui as code, max(_date),null as valueDate 
from fcp.stokoper s
left outer join fcp.cpartfcp c on s._compte=c._compte
left outer join Fcp.CONSGRPE cg on s._compte=cg._compte
left outer join fcp.grpedeft g on g._codegroupe=cg._codegroupe
left outer join com.prixhist as PR on s._codeprodui=PR._codeprodui
where PR._date<=@value_date and left(PR._codeprodui,2)='SW'
and g._codegroupe in ('M','OP') and(s._qtevalideenongagee+s._qtenonvalideenongagee) <> 0
group by s._codeprodui


update #LAST_PRICE_FORWARD
set closePrice = _coursclose
from com.prixhist 
where _codeprodui = code and _date = valueDate

update #LAST_PRICE_SWAP
set closePrice = _coursclose
from com.prixhist 
where _codeprodui = code and _date = valueDate

-- Test si les instruments ont bien été évalués aujourd'hui/à la date valuedate
-- c'est du "tout ou rien" , on envoie l'ensemble ou rien
declare @test_fw tinyint 
set @test_fw = ( select count(*) from #LAST_PRICE_FORWARD where valueDate <> @value_date ) 

declare @test_sw tinyint 
set @test_sw = ( select count(*) from #LAST_PRICE_SWAP where valueDate <> @value_date ) 

--set @test_fw = 0 -- pour forcer le test à OK
--set @test_sw = 0 -- pour forcer le test à OK
if( @test_fw = 0 ) AND ( @test_sw = 0 )
BEGIN


select 
   'FDR' as 'FDR|Client',
   ISNULL(C._codeexterne,'CASH') as Principal,
   --'CMAVPAR' as Principal,
   Counterparty = case
   when O._codecourt='SGO' then 'SOGEPAR'
   when O._codecourt='BARO' then 'BARCLON'
   when O._codecourt='NATIO' then 'NBPOPAR'
   when O._codecourt='HSBCO' then 'CCFRPAR'
   when O._codecourt='BNPPO' then 'PARBLON'
   when O._codecourt='DBO' then 'DEUTLON'
   when O._codecourt='DEUTCUS' then 'DEUTFRA'
   else '' end,
   'N' as Cleared,
   'FX' as ProductID,
   CAST(((PR._coursclose*O._nominal)/100)-(O._montantbrut) AS DECIMAL (16,2)) as PV1,
   'EUR' as PV1CCY,
    '' as PV2,
    '' as PV2CCY,
    O._numeroint as TradeID,
    --left(convert(char,O._dateoperation,20),10) as TradeDate ,
    --left(convert(char,O._datevaleur,20),10) as MaturityorExpiryDate,
    --left(convert(char,PR._date,20),10) as PVValueDate,
    O._dateoperation as TradeDate ,
    O._datevaleur as MaturityOrExpiryDate,
    PR._date as PVValueDate,   	
    '' as EffectiveDate,
    '' as ExerciceDate,
     'EUR' as Notional1CCY,
     O._nominal as Notional1,
    '' as Notional2CCY,
    '' as Notional2,
    BuySell=Case
  when O._sensoperation='A' then 'B'
  when O._sensoperation='V' then 'S'
  else '' end,
  '' as PutCall,
  '' as InitialMarginCCY,
  '' as InitialMarginAmount,
  '' as Underlying,
  '' as Price,
  '' as ExMatchID,
  '' as TradeMatchID,
  '' as CounterpartyMatchID   
from #LAST_PRICE_FORWARD as F 
left outer join fcp.operat  O on O._codeprodui = F.code
left outer join fcp.cpartfcp C on c._compte = o._compte
left outer join com.prixhist PR on PR._codeprodui=F.code and PR._date=F.valueDate

UNION

select 
   'FDR' as 'FDR|Client',
   ISNULL(C._codeexterne,'CASH') as Principal,
   --'CMAVPAR' as Principal,
   Counterparty = case
   when O._codecourt='SGO' then 'SOGEPAR'
   when O._codecourt='BARO' then 'BARCLON'
   when O._codecourt='NATIO' then 'NBPOPAR'
   when O._codecourt='HSBCO' then 'CCFRPAR'
   when O._codecourt='BNPPO' then 'PARBLON'
   when O._codecourt='DBO' then 'DEUTLON'
   when O._codecourt='CAIO' then 'BSUIPAR'
   when O._codecourt='BREDO' then 'BRED'
   when O._codecourt='DEUTCUS' then 'DEUTFRA'   
   else '' end,
   'N' as Cleared,
   'IS' as ProductID,
   CAST(PR._coursclose AS DECIMAL (16,2)) as PV1,
   'EUR' as PV1CCY,
    '' as PV2,
    '' as PV2CCY,
     O._numeroint as TradeID,
    --left(convert(char,O._dateoperation,20),10) as TradeDate ,
    --left(convert(char,O._datevaleur,20),10) as MaturityorExpiryDate,
    --left(convert(char,PR._date,20),10) as PVValueDate,
    O._dateoperation as TradeDate ,
    P._echeance as MaturityOrExpiryDate,
    PR._date as PVValueDate,   	
    O._datevaleur as EffectiveDate,
    --p._codeprodui,
    '' as ExerciceDate,
     'EUR' as Notional1CCY,
     O._nominal as Notional1,
    '' as Notional2CCY,
    '' as Notional2,
    BuySell=Case
  when O._sensoperation='A' then 'B'
  when O._sensoperation='V' then 'S'
  else '' end,
  '' as PutCall,
  '' as InitialMarginCCY,
  '' as InitialMarginAmount,
  '' as Underlying,
  '' as Price,
  '' as ExMatchID,
  '' as TradeMatchID,
  '' as CounterpartyMatchID   
from #LAST_PRICE_SWAP as F 
left outer join fcp.operat  O on O._codeprodui = F.code
left outer join fcp.cpartfcp C on c._compte = o._compte
left outer join com.produit p on p._codeprodui=F.code
left outer join com.prixhist PR on PR._codeprodui=F.code and PR._date=F.valueDate


END
--select * from #LAST_PRICE_FORWARD
--select * from #LAST_PRICE_SWAP
drop table #LAST_PRICE_SWAP
drop table #LAST_PRICE_FORWARD