---------------------------------------------------------------
---------------------------------------------------------------
-- extraction des notes ISR avec les tickers Bloomberg  (equity et corp)
---------------------------------------------------------------

select ISIN,max(Date) as Date
into #ISR_LAST
from ISR_NOTE
group by ISIN



-- la liste des tickers Bloomberg des equity
select  ISIN, max (DATE) as DATE 
into #TICKER
from ACT_TICKER 
where TICKER_BLOOMBERG is not null
group by ISIN

-- la liste des tickers Bloomberg pour les société emettrice de credit bond
select  ISIN, max (DATE) as DATE 
into #CORP
from ACT_TICKER 
where TICKER_BLOOMBERG_CORP is not null
group by ISIN


select tc.TICKER_BLOOMBERG_CORP,isr.DATE,isr.ISIN,t.TICKER_BLOOMBERG,note.NAME,note.EUROPE,note.EURO,note.ExEURO,note.USA,note.CREDIT
from #ISR_LAST as isr
left outer join ISR_NOTE as note on note.ISIN = isr.ISIN and note.DATE = isr.DATE
left outer join #TICKER as d on d.ISIN = isr.ISIN 
left outer join ACT_TICKER as t on t.ISIN = isr.ISIN and t.DATE = d.DATE

left outer join #CORP as c on c.ISIN = isr.ISIN 
left outer join ACT_TICKER as tc on tc.ISIN = isr.ISIN and tc.DATE = c.DATE
order by isr.ISIN

drop table #ISR_LAST
drop table #CORP
drop table #TICKER



select tc.TICKER_BLOOMBERG_CORP,isr.DATE,isr.ISIN,t.TICKER_BLOOMBERG,note.NAME,note.EUROPE,note.EURO,note.ExEURO,note.USA,note.CREDIT
from (select ISIN,max(Date) as Date from ISR_NOTE group by ISIN) as isr
left outer join ISR_NOTE as note on note.ISIN = isr.ISIN and note.DATE = isr.DATE
left outer join (select  ISIN, max (DATE) as DATE from ACT_TICKER where TICKER_BLOOMBERG is not null group by ISIN) as d on d.ISIN = isr.ISIN 
left outer join ACT_TICKER as t on t.ISIN = isr.ISIN and t.DATE = d.DATE

left outer join (select  ISIN, max (DATE) as DATE from ACT_TICKER where TICKER_BLOOMBERG_CORP is not null group by ISIN) as c on c.ISIN = isr.ISIN 
left outer join ACT_TICKER as tc on tc.ISIN = isr.ISIN and tc.DATE = c.DATE
order by isr.ISIN

