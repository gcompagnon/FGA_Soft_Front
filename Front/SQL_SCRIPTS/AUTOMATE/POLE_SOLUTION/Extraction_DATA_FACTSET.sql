---------------------------------------------------------------------
-- Extraction des données ACTION/FACTSET pour LE POLE SOLUTION
--
---------------------------------------------------------------------
select DATE, TICKER,ISIN,COUNTRY,PRICE,PRICE_EUR,SECTOR,SECTOR_NAME,
secteursGICS.label as 'Secteurs GICS',
sssecteursGICS.label as 'Sous Secteurs GICS',
MXEU,MXEM,MXEUM, MXFR,MXUSLC,
GARPN_QUINTILE_S,GARPN_RANKING_S,
GARPN_TOTAL_S,
GARPN_TOTAL_NO_ISR_S,
GARPN_GROWTH_S,
GARPN_VALUE_S,
GARPN_YIELD_S,
GARPN_ISR_S, -- la note ISR brute (non winzorise) est dans ISR_NOTE
-- decomposition des notes
EPS_CHG_NTM, 
d.*
from DATA_FACTSET as d
left outer join ref_security.SECTOR as secteursGICS on secteursGICS.class_name='GICS' and secteursGICS.level = 0 and secteursGICS.code = SUBSTRING (d.SECTOR,1,2)
left outer join ref_security.SECTOR as sssecteursGICS on sssecteursGICS.class_name='GICS' and sssecteursGICS.level = 1 and sssecteursGICS.code = SUBSTRING (d.SECTOR,1,8)
where d.ISIN is not null -- Ne pas prendre les lignes d'agrégats de secteurs
and d.DATE between '01/01/2015' and '16/04/2015'
