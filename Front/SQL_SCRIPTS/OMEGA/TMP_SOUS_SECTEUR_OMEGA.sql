--Recherche tous les sous secteur dans OMEGA
select ss._CODESOUSSECTEUR As 'id', ss._LIBELLESOUSSECTEUR As 'libelle', ss_s._codesecteur As 'id_secteur', 'OMEGA' As 'id_utilisateur'
from com.soussect ss , com.ssectclass ss_s
where ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR 