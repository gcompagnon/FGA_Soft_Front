declare @date datetime
set @date='30/12/2011'

select
	case  
		when row_number()over (ORDER BY p._codeprodui desc)%10='0' then 1
		else 0
	end
	as style,
   10 as "99",	
   p._codeprodui as "Code Indice",	
   h._coursclose as "Valeur Indice",	
   rtrim(convert(char,h._date,103))  as "Date valeur indice"	,
   'FIN' as "FIN DE FICHIER"
from 	
   com.produit p,	
   com.prixhist h	
where 	
    (p._codedutype = 'BENCH' or (p._codedutype = 'TCT' and p._codeprodui in ('EURIBOR3M','EURIBOR6M')))	
and p._codeprodui=h._codeprodui	
and h._date = @date	
