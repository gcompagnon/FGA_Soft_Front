Declare @date datetime	
set @date ='30/12/2011'

select
	case  
		when row_number()over (ORDER BY h._date desc)%10='0' then 1
		else 0
	end
	as style,
 	8 			as "99",	
        h._codeprodui           as [Code Isin],	
        @date                   as [Date valorisation],	
        h._coursclose           as [Valeur de marché],	
        h._duration             as Duration,	
        isnull(h._nbrparts ,0)  as [Nb de Part total],	
        isnull(h._beta1  ,0)    as Beta,	
        isnull(h._Delta,0)      as Delta,	
        0 			as Dividende,	
        0 			as Volatilite,	
        h._date                 as Date,
	'' 			as [Watch List],
	isnull(rtrim(ra._signature),'') as "Rating Interne",
	isnull(rtrim(s2._signature),'') as "Rating SII",	
	'' as "Rating S&P",
	'' as "Rating Fitch",
	'' as "Rating Moodys",
'FIN' as "FIN DE FICHIER"
	
from com.prixhist h 
right outer join (select max(_date) as max_dat, _codeprodui as cod_prdt from com.prixhist where _date <= @date and _date >=(@date-50) group by _codeprodui ) r	
      on _codeprodui=cod_prdt and _date=max_dat
left outer join           com.rating ra   on ra._codeprodui =cod_prdt and  ra._codeagence='INT' and ra._date=(select max(_date) from com.rating raa where  raa._codeprodui=cod_prdt and raa._codeagence='INT'and _date<=max_dat)
left outer join           com.rating s2  on s2._codeprodui=cod_prdt and s2._codeagence='INT S2' and s2._date=(select max(_date) from com.rating s2aa where  s2aa._codeprodui=cod_prdt and s2aa._codeagence='INT S2'and _date<=max_dat)
 	
where h._codeprodui in (select distinct v._assettype from  fcp.valoprdt  v
					left outer join com.produit p on v._assettype=p._codeprodui
                    where -- 6 mois en arrière
                    v._dateoperation between DATEADD (mm ,-6 , getdate() ) and getdate() and p._codedutype<>'SWLG'  and v._compte not in ('3020010','3020011','3020012','3020020','3020021','3020022','3010010','3010011','3010012','3010020','3010021','3010022','3010030','3010031','3010032','53316','5300250','5300251','5300252','5300240','5300241')
                    )
or h._codeprodui in ('fr0011662014')
order by h._date desc
