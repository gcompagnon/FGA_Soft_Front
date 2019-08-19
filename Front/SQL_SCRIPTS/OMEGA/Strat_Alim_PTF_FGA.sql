

Declare @Datejour Datetime --varchar(20)
Set @Datejour = CONVERT(VARCHAR(10), GETDATE(), 105)



CREATE TABLE #To_Ptf_FGA (	[Groupe] [varchar](25) NULL,[Dateinventaire] [datetime] NOT NULL,
	[Compte] [varchar](60) NOT NULL,[ISIN_Ptf] [varchar](12) NULL,	[Libelle_Ptf] [varchar](60) NULL,
	[code_Titre] [varchar](15) NOT NULL,[isin_titre] [varchar](15) NULL,[Libelle_Titre] [varchar](60) NULL,
	[Valeur_Boursiere] [float] NULL,[Coupon_Couru] [float] NULL,[Valeur_Comptable] [float] NULL,
	[Coupon_Couru_Comptable] [float] NULL,[PMV] [float] NULL,[PMV_CC] [float] NULL,[Type_Produit] [varchar](60) NULL,
	[Devise_Titre] [varchar](3) NULL,[Secteur] [varchar](60) NULL,[Sous_Secteur] [varchar](60) NULL,
	[Pays] [varchar](60) NULL,[Emetteur] [varchar](60) NULL,[Rating] [varchar](4) NULL,
	[Grp_Emetteur] [varchar](60) NULL,[maturite] [datetime] NULL,[duration] [float] NULL,
	[sensibilite] [float] NULL,[coursclose] [float] NULL,[quantite] [float] NULL,
	[nominal] [decimal](32, 10) NULL,[coupon] [float] NULL,[rendement] [float] NULL)
	
Insert into #To_Ptf_FGA
SELECT
            g._NOMGROUPE							as Groupe,
            @Datejour								as Dateinventaire,
            v._compte								as Compte,
            c._username								as ISIN_Ptf,
            c._LIBELLECLI							as Libelle_Ptf,
            v._codeprodui							as code_Titre,
            p._isin									as isin_titre,
            p._libelle1prod							as Libelle_Titre,
            v._valorisation							as Valeur_Boursiere,
            0										as Coupon_Couru,
            NULL									as Valeur_Comptable,
            NULL									as Coupon_Couru_Comptable,
            NULL									as PMV,
            NULL									as PMV_CC,
            t._libelle1type							as Type_Produit, --rtrim(v._libelletypeproduit) + (case when (( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)='EMP') then ' TAUX' 		else '' end) as Type_Produit, 
            v._codedevise							as Devise_Titre,
            s._LIBELLESECTEUR						as Secteur,
            ss._LIBELLESOUSSECTEUR					as Sous_Secteur,
            pi._LIBELLEPAYS							as Pays,
            e._NOMEMETTEUR							as Emetteur,
            Null as rating, --r._signature							as Rating, Suppressin du rating pour diminuer temps de la requete
            GR._libellegrper						as Grp_Emetteur,
            case when ((( t._libelle1type	 ='CALL' OR t._libelle1type	 ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)<>'EMP') OR 		(left(t._libelle1type,12)='Certificat A') OR (left(t._libelle1type,9)='FUTURES A' ) ) then NULL else p._echeance end as maturite,--									as maturite, --case when ((( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)<>'EMP') OR 		(left(v._libelletypeproduit,12)='Certificat A') OR (left(v._libelletypeproduit,9)='FUTURES A' ) ) then NULL else p._echeance end as maturite,
            h._duration								as duration,
			case when h._rendement=0 then h._sensibilite else h._duration/(1+(h._rendement)/100) end as sensibilite,
            h._coursclose							as coursclose,
            case when p._methodecot=3 then	sum( v._position*p._nominal) else sum( v._position) end as quantite,
            sum(p._nominal) as nominal,
			p._coupon As coupon,
			case when h._rendement<>0 or h._sensibilite=0 then h._rendement else (h._duration/h._sensibilite -1 ) *100 end as rendement		
--into #TitrePTFFGA
FROM
          App.omg_fnt_FEPoOuGrpe('01',convert(char(10),@Datejour,105)  ,50, 0, 1, 0)  v
left outer join           com.produit p                 on p._codeprodui=v._codeprodui
left outer join			  com.prdtclasspys ps			on p._codeprodui=ps._codeprodui and ps._classification=0
left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
left outer join			  com.ssectclass ss_s			on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
--left outer join           com.rating r					on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where r._codeprodui=p._codeprodui)
left outer join           com.prixhist  h               on v._datederniercours=h._date and p._codeprodui=h._codeprodui
left outer join           com.emetteur e                on e._emetteur=p._emetteur
left outer join           com.grpemetteursratios GR     on  e._grper=GR._codegrper
left outer join           com.pays pi                   on pi._CODEPAYS= gr._codepays
left outer join           fcp.cpartfcp c                on  c._compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
left outer join			  com.TYPEPROD t				on t._codedutype= p._codedutype
WHERE          
         --v._datederniercours='27-03-2013'
	     g._codegroupe in ('IO','UM') 
	     --v._libelletypeproduit not like 'FUTURES %'
	     and v._compte in('3203105','3203106','3203107','4030005','4030006','4030007')
group by
			g._NOMGROUPE,
            v._datederniercours ,
            v._compte,
            c._username,
            c._LIBELLECLI,
            v._codeprodui,
            p._isin,  
            p._libelle1prod,
            v._libelle1prod, 
            v._codedevise, 
            s._LIBELLESECTEUR, 
            ss._LIBELLESOUSSECTEUR,
            pi._LIBELLEPAYS, 
            e._NOMEMETTEUR,
--            r._signature,
			GR._libellegrper,
			p._echeance,
			h._duration,
			h._sensibilite,
			h._rendement,
			h._coursclose,
			p._methodecot,
			p._coupon,
			h._rendement,
			v._valorisation,
			t._libelle1type
			--r._signature
order by groupe, compte,code_titre

--select * from fcp.valoprdt where _dateoperation ='05-04-2013'


select _contredevise as devise ,max(_date) as DerniereDate into #Devise from Com.DEVHIST group by _contredevise

select	s._compte as compte,
		sum(case when _coursClose is not Null then (s._MontantValide+s._MontantNonValide)/com._coursClose else  (s._MontantValide+s._MontantNonValide) end ) as Total
	into #liquidite from Com.DEVHIST com 
	inner join #devise tmp on tmp.derniereDate = com._date and com._contredevise=tmp.devise
	right join fcp.stockliq	s on s._codedevise =	com._contredevise						
where _compte in('3203105','3203106','3203107','4030005','4030006','4030007')         
group by s._compte


--traitement des liquidités des mandats
Insert into #To_Ptf_FGA
SELECT
            g._NOMGROUPE							as Groupe,
            @Datejour								as Dateinventaire,
            l.compte								as Compte,
            c._username								as ISIN_Ptf,
            c._LIBELLECLI							as Libelle_Ptf,
            'Cash Mandat'							as code_Titre,
            'Cash Mandat'  								as isin_titre,  
            'Liquidité(Mandat)'						as Libelle_Titre,
            l.total as Valeur_Boursiere,
            0										as Coupon_Couru,
			l.total as Valeur_Comptable,
            0										as Coupon_Couru_Comptable,  
            0										as PMV,  
            0										as PMV_CC,
            'Cash'									as Type_Produit, 
            'EUR'									as Devise_Titre, 
            'Liquidité'								as Secteur, 
            'Liquidité'								as Sous_Secteur,
            NULL									as Pays,  -- pas d expo sur le Pays France, mais sur l'EURO . voir pour un faux pays
            NULL									as Emetteur,
            NULL									as Rating,
            NULL									as Grp_Emetteur,   
            NULL									as maturite,
            NULL									as duration,
			NULL									as sensibilite,
            NULL									as coursclose,
            NULL									as quantite,
            NULL									as nominal,
			NULL									as coupon,
			NULL									as rendement
--into #CashMandat
FROM
          #liquidite l
left outer join           fcp.cpartfcp c                on  c._compte=l.compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=l.compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
WHERE          
	     g._codegroupe in ('IO','UM')
group by
			g._NOMGROUPE,
			l.total,
            l.compte,
            c._username,
            c._LIBELLECLI

--Récupération des OPCVM

Insert into #To_Ptf_FGA
SELECT
            g._NOMGROUPE							as Groupe,
            @Datejour								as Dateinventaire,
            v._compte								as Compte,
            c._username								as ISIN_Ptf,
            c._LIBELLECLI							as Libelle_Ptf,
            v._codeprodui							as code_Titre,
            p._isin									as isin_titre,
            p._libelle1prod							as Libelle_Titre,
            v._valorisation							as Valeur_Boursiere,
            0										as Coupon_Couru,
            NULL									as Valeur_Comptable,
            NULL									as Coupon_Couru_Comptable,
            NULL									as PMV,
            NULL									as PMV_CC,
            t._libelle1type							as Type_Produit, --rtrim(v._libelletypeproduit) + (case when (( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)='EMP') then ' TAUX' 		else '' end) as Type_Produit, 
            v._codedevise							as Devise_Titre,
            s._LIBELLESECTEUR						as Secteur,
            ss._LIBELLESOUSSECTEUR					as Sous_Secteur,
            pi._LIBELLEPAYS							as Pays,
            e._NOMEMETTEUR							as Emetteur,
            Null as rating, --r._signature							as Rating, Suppressin du rating pour diminuer temps de la requete
            GR._libellegrper						as Grp_Emetteur,
            case when ((( t._libelle1type	 ='CALL' OR t._libelle1type	 ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)<>'EMP') OR 		(left(t._libelle1type,12)='Certificat A') OR (left(t._libelle1type,9)='FUTURES A' ) ) then NULL else p._echeance end as maturite, --case when ((( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)<>'EMP') OR 		(left(v._libelletypeproduit,12)='Certificat A') OR (left(v._libelletypeproduit,9)='FUTURES A' ) ) then NULL else p._echeance end as maturite,
            h._duration								as duration,
			case when h._rendement=0 then h._sensibilite else h._duration/(1+(h._rendement)/100) end as sensibilite,
            h._coursclose							as coursclose,
            case when p._methodecot=3 then	sum( v._position*p._nominal) else sum( v._position) end as quantite,
            sum(p._nominal) as nominal,
			p._coupon As coupon,
			case when h._rendement<>0 or h._sensibilite=0 then h._rendement else (h._duration/h._sensibilite -1 ) *100 end as rendement		
--into #OPCVM
FROM
          App.omg_fnt_FEPoOuGrpe('OP',convert(char(10),@Datejour,105),50, 0, 1, 0)  v
left outer join           com.produit p                 on p._codeprodui=v._codeprodui
left outer join			  com.prdtclasspys ps			on p._codeprodui=ps._codeprodui and ps._classification=0
left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
left outer join			  com.ssectclass ss_s			on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
--left outer join           com.rating r					on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where r._codeprodui=p._codeprodui)
left outer join           com.prixhist  h               on v._datederniercours=h._date and p._codeprodui=h._codeprodui
left outer join           com.emetteur e                on e._emetteur=p._emetteur
left outer join           com.grpemetteursratios GR     on  e._grper=GR._codegrper
left outer join           com.pays pi                   on pi._CODEPAYS= gr._codepays
left outer join           fcp.cpartfcp c                on  c._compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
left outer join			  com.TYPEPROD t				on t._codedutype= p._codedutype
WHERE          
         --v._datederniercours='27-03-2013'
	     g._codegroupe in ('OP') 
	     --v._libelletypeproduit not like 'FUTURES %'
	     --and v._compte in('3203105','3203106','3203107','4030005','4030006','4030007')
group by
			g._NOMGROUPE,
            v._datederniercours ,
            v._compte,
            c._username,
            c._LIBELLECLI,
            v._codeprodui,
            p._isin,  
            p._libelle1prod,
            v._libelle1prod, 
            v._codedevise, 
            s._LIBELLESECTEUR, 
            ss._LIBELLESOUSSECTEUR,
            pi._LIBELLEPAYS, 
            e._NOMEMETTEUR,
--            r._signature,
			GR._libellegrper,
			p._echeance,
			h._duration,
			h._sensibilite,
			h._rendement,
			h._coursclose,
			p._methodecot,
			p._coupon,
			h._rendement,
			v._valorisation,
			t._libelle1type
			--r._signature
order by groupe, compte,code_titre


select	s._compte as compte,
		sum(case when _coursClose is not Null then (s._MontantValide+s._MontantNonValide)/com._coursClose else  (s._MontantValide+s._MontantNonValide) end ) as Total
	into #liquiditeOPCVM
	from Com.DEVHIST com 
	inner join #devise tmp on tmp.derniereDate = com._date and com._contredevise=tmp.devise
	right join fcp.stockliq	s on s._codedevise =	com._contredevise						
--where _compte in('3203105','3203106','3203107','4030005','4030006','4030007')         
group by s._compte

Insert into #To_Ptf_FGA
SELECT
            g._NOMGROUPE							as Groupe,
            @Datejour								as Dateinventaire,
            l.compte								as Compte,
            c._username								as ISIN_Ptf,
            c._LIBELLECLI							as Libelle_Ptf,
            'Cash OPCVM'							as code_Titre,
            'Cash OPCVM'  								as isin_titre,  
            'Liquidité(OPCVM)'						as Libelle_Titre,
            l.total as Valeur_Boursiere,
            0										as Coupon_Couru,
			l.total as Valeur_Comptable,
            0										as Coupon_Couru_Comptable,  
            0										as PMV,  
            0										as PMV_CC,
            'Cash'									as Type_Produit, 
            'EUR'									as Devise_Titre, 
            'Liquidité'								as Secteur, 
            'Liquidité'								as Sous_Secteur,
            NULL									as Pays,  -- pas d expo sur le Pays France, mais sur l'EURO . voir pour un faux pays
            NULL									as Emetteur,
            NULL									as Rating,
            NULL									as Grp_Emetteur,   
            NULL									as maturite,
            NULL									as duration,
			NULL									as sensibilite,
            NULL									as coursclose,
            NULL									as quantite,
            NULL									as nominal,
			NULL									as coupon,
			NULL									as rendement
--into #CashOPCVM
FROM
          #liquiditeOPCVM l
left outer join           fcp.cpartfcp c                on  c._compte=l.compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=l.compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
WHERE          
	     g._codegroupe in ('OP')
group by
			g._NOMGROUPE,
			l.total,
            l.compte,
            c._username,
            c._LIBELLECLI
        

select * from #To_Ptf_FGA


Drop table #OPCVM
Drop table #TitrePTFFGA
Drop table #Devise
drop table #liquidite
drop table #liquiditeOPCVM
Drop table #CashOPCVM
Drop table #CashMandat
Drop table #To_Ptf_FGA



