select 
	case  
		when row_number()over (ORDER BY E._emetteur)%10='0' then 1
		else 0
	end
	as style,
      2 as "99",
      rtrim(E._emetteur) as "Code Emetteur",
      rtrim(E._nomemetteur) as "Libelle Emetteur",
      isnull(rtrim(E._pays),'') as "Pays Emetteur",
      isnull(rtrim(NE._libellenature),'') as  "Nature Emetteur",
      isnull(rtrim(GR._codegrper),'') as "Code Groupe Emetteur",
      'FIN' as "FIN DE FICHIER"
     
from
    com.emetteur E
    left outer join  com.grpemetteursratios GR on E._grper=GR._codegrper
    left outer join com.natemett NE on E._codenature=NE._codenature

order by E._emetteur