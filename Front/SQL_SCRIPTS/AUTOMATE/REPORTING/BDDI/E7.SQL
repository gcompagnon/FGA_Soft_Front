declare @datedebut datetime
declare @datefin datetime
set @datedebut ='31/12/2011'
set @datefin ='30/03/2012'


select 
	case  
		when row_number()over (ORDER BY c._username)%10='0' then 1
		else 0
	end
	as style,
	7 as "99", 	
 	rtrim(c._username) as "Code Isin Fonds",	
	convert(char,rtrim(v._assettype)) as "Code Isin Titre" , 	
	rtrim(convert(char,v._dateoperation,103))  as "Date rel",				
	"Valeur Boursiere" 	= case left(v._libelletypeproduit,6)	
		 	            when 'FUTURE' then v._unlossD	
		        	    else v._netamountD + v._netamountCC	
		             	end,	
	"Sens rel" 		= case v._BS	
                                    when 'A' then 'acheteur'	
			            else 'vendeur'	
		            	end,
	'FIN' as "FIN DE FICHIER"/*, v._compte*/
from  fcp.valoprdt  v	
left outer join           ( 
							select _compte, _username as _username from fcp.cpartfcp where _username is not null
							union
							select _compte, _codeprodui as _username from fcp.cpartfcp where   _codeprodui is not null
							union
							select _compte, _codeprodui as _username from fcp.multiparts
) c 	on c._compte=v._compte 	
left outer join           fcp.CONSGRPE cg       on cg._compte=v._compte	
left outer join           fcp.grpedeft g        on g._codegroupe=cg._codegroupe	
where 	
        v._dateoperation= (select max(vvv._dateoperation) 
			   from fcp.valoprdt vvv  
			   where vvv._dateoperation<=@datefin 
                           and   vvv._dateoperation>=@datedebut
                           and   vvv._compte=v._compte group by vvv._compte)	
and 	g._codegroupe ='OP'	