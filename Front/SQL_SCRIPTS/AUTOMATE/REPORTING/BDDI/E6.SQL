use omega
select 
	case  
		when row_number()over (ORDER BY P._emetteur)%10='0' then 1
		else 0
	end
	as style,
	  6 as "99",
      rtrim(P._CODEPRODUI) as "Code Instrument", -- rajout 07-08-2012 sur demande ELSA
      rtrim(P._isin) as "Code ISIN", 
      rtrim(replace(P._libelle1prod,',','.')) as "Libelle Instrument",
      "Marche" =case when P._cote=1 then 'cote'
                else 'non cote' end ,
      Instrument=case --cible:Actions/Obligations/OPCVM/Derives
       when m._libellemarche='Comptant' then 'OPCVM'
       when  m._libellemarche in ('Options','Futures') then 'Derives'
       when p._codedutype='CACT' then 'OPCVM'
       when p._codedutype in ('SWLG') then 'Derives'
       when p._codedutype in ('CD','BT','CDUS') then 'Obligations'     
       else  rtrim(m._libellemarche) end,
      
      Classe=case --cible:Actions/droit/dividende/obligation/obligation option implicite/obligation structuree/obligation convertible/monetaire/OPCVM action/OPCVM obligataire pur/OPCVM obligataire convertible/OPCVM diversifie/Future/option/
      when m._libellemarche='Comptant' and left(ss._CODESOUSSECTEUR,5)='FO CV' then 'OPCVM Obligataires Convertibles'
      when m._libellemarche='Comptant' and left(ss._CODESOUSSECTEUR,2)='FO' then 'OPCVM Obligataires purs'
      when m._libellemarche='Comptant' and left(ss._CODESOUSSECTEUR,2)='FA' then 'OPCVM Actions'
      when m._libellemarche='Comptant' and left(ss._CODESOUSSECTEUR,2)='FD' then 'OPCVM Diversifies'
      when m._libellemarche='Comptant' and left(ss._CODESOUSSECTEUR,2)='FS' then 'OPCVM Diversifies'
      when m._libellemarche='Comptant' and left(ss._CODESOUSSECTEUR,2)='FT' then 'OPCVM Monetaires'
      when P._codedutype='DROIT' then 'DROIT'
      when P._codedutype='CACT' then 'OPCVM Actions'
     when P._codedutype in ('CALL','PUT') then 'Options'
      when left(P._codedutype,2) in ('FT','FA','FD') then 'Futures'
      when P._codedutype='OSTRU' then 'Obligations structurees'
     when P._codedutype in ('OCALL','OTVCA','OPUT') then 'Obligations option implicite'
     when P._codedutype in ('OCV') then 'obligations convertibles'
     else rtrim(m._libellemarche)  end,  
      
    "Groupe Exposition"=case --cible: Actions/taux/diversifie
    when  left(ss._CODESOUSSECTEUR,5)='FO CV' then 'Diversifies'
    when  left(ss._CODESOUSSECTEUR,2)='FO' then 'Taux'   
    when  left(ss._CODESOUSSECTEUR,2)='FA' then 'Actions'
    when  left(ss._CODESOUSSECTEUR,2)='FD' then 'Diversifies'
    when  left(ss._CODESOUSSECTEUR,2)='FS' then 'Diversifies'
    when  left(ss._CODESOUSSECTEUR,2)='FT' then 'Taux'
    when P._codedutype='CACT' then 'Actions'
    --les secteurs des droits et options peuvent être en nomneclature fonds pour les sous jacents indices ou secteurs pour les sous jacents valeurs
     when P._codedutype='DROIT' /*and C._valeur='Actions'*/ then 'Actions'
     when left(P._codedutype,4) = 'CALL' and C._valeur='Actions' then 'Actions'
     when left(P._codedutype,4) = 'CALL' and C._valeur='Taux' then 'Taux'
     when left(P._codedutype,3) = 'PUT' and C._valeur='Actions' then 'Actions'
     when left(P._codedutype,3) = 'PUT' and C._valeur='Taux' then 'Taux'     
     when left(P._codedutype,2)='FT' then 'Taux'
     when left(P._codedutype,2)='FA' then 'Actions'
     when left(P._codedutype,2)='FD' then 'Diversifies'
     when left(P._codedutype,2)='FS' then 'Diversifies'
     when P._codedutype='OSTRU' and left(ss._CODESOUSSECTEUR,2)='FA' then 'Actions'
     when P._codedutype in ('OCV') then 'Diversifies' 
    when  m._libellemarche in ('Obligations','money market') then 'Taux'   
    else rtrim(m._libellemarche) end,  
	'' as "Categorie 1",
	'' as "Categorie 2",
      "Type de taux"=case
                       when P._codedutype='OSTRU' then 'Structure'
                       when P._fixevar='1' then 'Fixe'
                       when P._fixevar in ('2','4') then 'Variable'
                       when P._fixevar='3' then 'Indexe' 
                       else ''                      
                    end,   
      isnull(P._coupon,'') as "Taux Fixe",
      "Index"=case
                  when P._codedutype='OIND' then ''
                  when P._codedutype='OINDU' then ''
                  else isnull(P._tauxref,'')
              end,
       "Index inflation"=case
                  when P._codedutype='OIND' then isnull(P._tauxref,'')
                  when P._codedutype='OINDU' then isnull(P._tauxref,'')
                  else ''
                  end,      
       '' as "Periodicite de refixation",
       '' as "Date de refixation",
       isnull(P._marge,'') as Spread,
       "Base Taux" = case 
                      when P._regleint='1' then 'Exact/Nexact'
                      when P._regleint='2' then 'Exact/360'
                      when P._regleint='3' then 'Exact/Exact'
                      when P._regleint='4' then 'Exact/365'
                      when P._regleint='5' then '30/360'
                      when P._regleint='6' then '30E/360'
                      else ''
                  end, 
       
        "Periodicite de paiement" = case
								when P._periodicite='6' then 'ZC'
								when P._periodicite='1' then 'An'
								when P._periodicite='2' then 'Sem'
								when P._periodicite='3' then 'Trim'
								when P._periodicite='4' then 'Mois'
								when P._periodicite='5' then 'Hebdo'
                                else ''
                          end,
		CASE WHEN P._datepremiercoupon IS NULL THEN '' ELSE CONVERT(char,P._datepremiercoupon,103) END as "Date de paiement",
        
        "Type option" = case
                  when left(P._codedutype,4)='CALL'  then 'CALL'
                  when left(P._codedutype,3)='PUT'  then 'PUT'
                  when P._codedutype in ('CALL','PUT')  then P._codedutype
                  when P._codedutype in ('OCALL','OTVCA') then 'CALL'
                  when P._codedutype in ('OPUT') then 'PUT'
                  when P._codedutype in ('OCV') then 'Conversion'
                  else '' 
                  end,
          '' as "Sens de la Branche du swap",
          '' as "Type de sous-jacent",
          isnull(P._sousjacent,'') as "Sous-jacent",
         "Profil d'exercice de l'option"=case
          when P._typoption='1' then 'Americaine'
          when P._typoption='2' then 'Europeenne'
          when P._codedutype in ('OCALL','OTVCA','OPUT') then 'Europeenne'
          else ''
          end,
        
        isnull(P._strike,0) as "Prix d'exercice",
		'' as "Cours garantie",
        isnull(P._tauxmax,0) as "Taux cap",
        isnull(P._tauxmin,0) as "Taux floor",
        isnull(P._multiple,0) as "Quotite sous-jacent",
        "Date d'exercice"=case
         when P._callable=1 and F._date is not null then convert(char,F._date,103)
         when P._echeance is null then ''          
         else convert(char,P._echeance,103) end,
        "Date maturite"= case when P._echeance is null then '' else convert(char,P._echeance,103) end,
        "Seniorite" = case
                 when ss._CODESOUSSECTEUR in ('O CF T1','O CF SFPER','O CF PERP') then 'Tier 1'
                when ss._CODESOUSSECTEUR in ('O CF SUB','O CF LT2','O SFIN LT2','O SFIN UT2','O CFA UT2','O CF UT2') then 'Subordonne'
                when (ss._CODESOUSSECTEUR like 'O%' and ss._CODESOUSSECTEUR not in ('O CF T1','O CF SFPER','O CF PERP','O CF SUB','O CF LT2','O SFIN LT2','O SFIN UT2','O CFA UT2','O CF UT2')) then 'Senior'
         else ' ' end,
            
        "Type collateral" = case 
			when  (ss._CODESOUSSECTEUR='O COVERED' and p._codedutype not in ('CD','BT','CDUS')) then 'Hypothecaires'
			when  P._garanti=1 and left(P._codedutype,2) not in ('FA','FT','FD') then 'Garantis Etats'
			when left(S._codesecteur,3) in ('O C','O A') and P._garanti=0 then 'Credit'
			else '' 
			end,
         --rtrim(r._signature) as Rating_interne,
         --rtrim(s2._signature) as Rating_S2,

         isnull(P._nominal,'') as "Valeur Nominal",
         isnull(P._PRIMEREMBOUR,0)  as "Prime de Remboursement",    
         isnull(P._remboursement,'') as "Profil remboursement", 
         
         "Zone d'Exposition"= case
         when m._libellemarche='Comptant' and ss._CODESOUSSECTEUR in ('FA ASI IND','FA ASIE','FA ASIE PA') then 'Asie'
         when m._libellemarche='Comptant' and ss._CODESOUSSECTEUR in ('FA JAP IND','FA JAPON') then 'Japon'
         when m._libellemarche='Comptant' and ss._CODESOUSSECTEUR in ('FA EX EURO') then 'Zone Ex-Euro'
         when m._libellemarche='Comptant' and ss._CODESOUSSECTEUR in ('FA EUP GEN','FA EUROPE','FA EUP IND','FA EUP PMC','FO EUP') then 'Europe'
         when m._libellemarche='Comptant' and ss._CODESOUSSECTEUR in ('FT TRESO','FA EUR GEN','FA EURO','FA EUR IND','FS VOLATIL','FA EUR PMC','FS PERF AB','FS DIVIDEN','FS DEVISE','FA IMMO','FO INFLATI','FO HI YIEL','FO EURO','FD EURO','FO ETAT','FO CREDIT','FO CV EURO','FO EUR CT','FO EUR LT','FO EUR MT','FO EUR TLT') then 'Euro'
         when m._libellemarche='Comptant' and ss._CODESOUSSECTEUR in ('FA FRA GEN','FA FRA IND','FA FRA PMC') then 'Zone Euro - Zone 1'
         when m._libellemarche='Comptant' and ss._CODESOUSSECTEUR in ('FA US GEN','FA US IND','FA US','FA US PMC') then 'Amerique du nord'
         when m._libellemarche='Comptant' and ss._CODESOUSSECTEUR in ('FA MONDE','FD INT','FO CV INT','FO INT','FA MAT PRE') then 'Autre Monde'
         when m._libellemarche='Comptant' and ss._CODESOUSSECTEUR in ('FA INT EME','FA EMERG','FO EMERG') then 'Emergents'
         when P._codedutype in ('CACT','CALL','PUT','CALLU','PUTUS') and ss._CODESOUSSECTEUR in ('FA ASI IND','FA ASIE','FA ASIE PA') then 'Asie'
         when P._codedutype in ('CACT','CALL','PUT','CALLU','PUTUS') and ss._CODESOUSSECTEUR in ('FA JAP IND','FA JAPON') then 'Japon'
         when P._codedutype in ('CACT','CALL','PUT','CALLU','PUTUS') and ss._CODESOUSSECTEUR in ('FA EUP GEN','FA EX EURO','FA EUROPE','FA EUP IND','FA EUP PMC','FO EUP') then 'Europe'
         when P._codedutype in ('CACT','CALL','PUT','FAEUR','FTEUR','CALLU','PUTUS') and ss._CODESOUSSECTEUR in ('FT TRESO','FA EUR GEN','FA EURO','FA EUR IND','FS VOLATIL','FA EUR PMC','FS PERF AB','FS DIVIDEN','FS DEVISE','FA IMMO','FO INFLATI','FO HI YIEL','FO EURO','FD EURO','FO ETAT','FO CREDIT','FO CV EURO','FO EUR CT','FO EUR LT','FO EUR MT','FO EUR TLT') then 'Euro'
        when P._codedutype in ('CACT','CALL','PUT','CALLU','PUTUS') and ss._CODESOUSSECTEUR in ('FA FRA GEN','FA FRA IND','FA FRA PMC') then 'Zone Euro - Zone 1'
        when P._codedutype in ('CACT','CALL','PUT','CDUS','CALLU','PUTUS') and ss._CODESOUSSECTEUR in ('FA US GEN','FA US IND','FA US','FA US PMC') then 'Amerique du nord'
        when P._codedutype in ('CACT','CALL','PUT','CALLU','PUTUS') and ss._CODESOUSSECTEUR in ('FA MONDE','FD INT','FO CV INT','FO INT','FA MAT PRE') then 'Autre Monde'
        when P._codedutype in ('CACT','CALL','PUT','CALLU','PUTUS') and ss._CODESOUSSECTEUR in ('FA INT EME','FA EMERG','FO EMERG') then 'Emergents'
         else isnull(rtrim(ZG._libellezone),'') end,      
        isnull(rtrim(s._LIBELLESECTEUR),'')    as Secteur,  
         '' as "Liquidite",
        isnull(P._deviseactif,'') as "Code Devise",
        isnull(rtrim(P._emetteur),'') as "Code Emetteur",
        isnull(P._VALEUR_EMISSION_INDEX,'') as "Valeur origine index",
        'FIN' as "FIN DE FICHIER",
        isnull(rtrim(ss._LIBELLESOUSSECTEUR),'')  as "Sous secteur"
from 
    com.produit P
    left outer join com.flux F on F._codeprodui=P._codeprodui and p._callable=1 and F._date=(select min(_date) from com.flux F where F._codeprodui=P._codeprodui and F._callput=1 and F._date>getdate())
    left outer join com.emetteur E on P._emetteur=E._emetteur
    left outer join  com.grpemetteursratios GR on E._grper=GR._codegrper
    left outer join com.natemett NE on E._codenature=NE._codenature
    left outer join com.pays PA on GR._codepays=PA._codepays
    left outer join com.signsgp SG on P._codeprodui=SG._codeprodui
    left outer join           com.rating r   on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where  r._codeprodui=p._codeprodui and r._codeagence='INT')
    left outer join           com.rating s2   on s2._codeprodui=p._codeprodui and s2._codeagence='INT S2' and s2._date=(select max(_date) from com.rating s2 where  s2._codeprodui=p._codeprodui and s2._codeagence='INT S2')
   left outer join com.classamf AM on SG._codeclassAMF=AM._codeclassamf
     left outer join 	  com.prdtclasspys ps	        on p._codeprodui=ps._codeprodui and ps._classification=0
    left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
    left outer join 	  com.ssectclass ss_s           on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
  left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
  left outer join com.classpays GE                    on GR._codepays=GE._codepays and GE._class='2'
  left outer join com.zonesgeo  ZG                      on GE._codezone=ZG._codezone and ZG._classification='2'    
  left outer join com.typeprod ty on P._codedutype=ty._codedutype  
 left outer join com.marche m on ty._marche=m._codemarche
left outer join com.champsprod C on P._codeprodui=C._codeprodui and c._numchamps=7

where 
 P._codeprodui in ('fr0011662014') or
 P._codeprodui in ('FR0012473767') or -- Federis Valeur foncieres
P._codeprodui in (

select distinct v._assettype from  fcp.valoprdt  v
left outer join com.produit p on v._assettype=p._codeprodui

                                         where 
-- 6 mois en arrière
                                                 v._dateoperation between DATEADD (mm ,-6 , getdate() ) and getdate() and p._codedutype<>'SWLG'  and v._compte not in ('3020010','3020011','3020012','3020020','3020021','3020022','3010010','3010011','3010012','3010020','3010021','3010022','3010030','3010031','3010032','53316','5300250','5300251','5300252','5300240','5300241'))


order by  P._emetteur