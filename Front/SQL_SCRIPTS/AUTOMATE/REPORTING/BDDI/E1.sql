select
	case  
		when row_number()over (ORDER BY GR._codegrper)%10='0' then 1
		else 0
	end
	as style,
	1 as "99",
    rtrim(GR._codegrper) as "Code Groupe Emetteur",
    rtrim(GR._libellegrper) as "Libelle Groupe Emetteur",
    isnull(rtrim(GR._codepays),'') as "Pays Groupe Emetteur",
    'FIN' as "FIN DE FICHIER"      
from
       com.grpemetteursratios GR 