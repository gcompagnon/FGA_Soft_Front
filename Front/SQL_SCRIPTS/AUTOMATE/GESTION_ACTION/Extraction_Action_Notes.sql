-- extraction de l ensemble des notes sur l indice ou sur un ptf modele
declare @value_date DATETIME
set @value_date = '24/02/2015'


select * from DATA_FACTSET where Date ='23/02/2015' order by COMPANY_NAME 

select SECTOR ,SECTOR_NAME ,GARPN_TOTAL_S as 'Note', GARPN_TOTAL_NO_ISR_S as 'Note Fi(HorsISR)',
GARPN_GROWTH_S as 'Croissance', GARPN_VALUE_S as 'Value', GARPN_YIELD_S as 'Profit', GARPN_ISR_S as 'ISR', GARPN_QUINTILE_S, GARPN_RANKING_S ,
TICKER , COMPANY_NAME, SECTOR_LABEL
  from DATA_FACTSET where Date ='23/02/2015'
  order by GARPN_TOTAL_NO_ISR_S
  

select * from ref_security.SECTOR_TRANSCO 
select * from ref_security.SECTOR

select SECTOR ,SECTOR_NAME ,GARPN_TOTAL_S as 'Note', GARPN_TOTAL_NO_ISR_S as 'Note Fi(HorsISR)',
GARPN_GROWTH_S as 'Croissance', GARPN_VALUE_S as 'Value', GARPN_YIELD_S as 'Profit', GARPN_ISR_S as 'ISR', GARPN_QUINTILE_S, GARPN_RANKING_S ,
TICKER , COMPANY_NAME, SECTOR_LABEL
  from DATA_FACTSET where Date ='23/02/2015'
  order by SECTOR_NAME , GARPN_RANKING_S 
  
--- Difference entre 2 dates  
  SELECT * into #qrvalues1 from DATA_FACTSET where DATE ='20/02/2015' AND ISIN IS NOT NULL
  SELECT * into #qrvalues2 from DATA_FACTSET where DATE = '23/02/2015'  And ISIN IS NOT NULL  
  SELECT * into #sectors from DATA_FACTSET WHERE DATE = '23/02/2015' AND ISIN IS NULL AND GICS_SUBINDUSTRY IS NULL  
  select * from #sectors
  SELECT distinct sect3.label as SECTOR,
   sect2.label as INDUSTRY, facs.SUIVI as SUIVI, fac1.TICKER as TICKER,
    fac1.COMPANY_NAME AS COMPANY, CONVERT(DATE, fac1.DATE, 103) as Date1,
     fac1.GARPN_RANKING_S as Rang1, fac1.GARPN_QUINTILE_S as Quint1,
      CONVERT(DATE, fac2.DATE, 103) as Date2, fac2.GARPN_RANKING_S as Rang2, 
      fac2.GARPN_QUINTILE_S as Quint2 ,fac1.COUNTRY as Pays 
      from #qrvalues1 fac1 
      inner join #qrvalues2 as fac2 on fac2.ISIN = fac1.ISIN 
      inner join ref_security.SECTOR as sect on fac1.SECTOR = sect.code
      inner join ref_security.SECTOR_TRANSCO as transco on transco.id_sector1 = sect.id 
      inner join ref_security.SECTOR as sect2 on sect2.id = transco.id_sector2 
      inner join ref_security.SECTOR as sect3 on sect3.id = sect.id_parent 
      inner join #sectors as facs on facs.FGA_SECTOR = sect2.code 
      WHERE fac1.GARPN_QUINTILE_S <> fac2.GARPN_QUINTILE_S AND fac1.ISIN = fac2.ISIN