declare @date datetime
set @date='30/03/2012'


select
	case  
		when row_number()over (ORDER BY d._contredevise)%10='0' then 1
		else 0
	end
	as style,
       3 as "99", 
      rtrim(d._contredevise) as "Code Devise",
      d._date as "Date valeur",
      d._coursclose as "Parite Euro",
      '' as "Valeur DEV Compt",
      'FIN' as "FIN DE FICHIER"
from
          com.devhist d,
          com.devise de
where
     d._contredevise=de._codedevise
and d._date =@date
