/****** Script for SelectTopNRows command from SSMS  ******/
declare @date1 datetime
declare @date2 datetime

set @date2=(Select GETDATE())
set @date1=DATEADD(m,DATEDIFF(m,0,@date2),0)

(SELECT  
      5 as 'style',
      g._nomgroupe as Nom_Groupe
      ,convert(VARCHAR(10),D._DATEOPERATION,103) as Date_opération
      ,convert(VARCHAR(10),D._DATEVALEUR,103) as Date_valeur
      ,''''+D._COMPTE as Compte
      ,C._libellecli as Libelle_ptf
      ,D._CODEPRODUI
      ,P._isin
      ,P._libelle1prod     
      ,'CPN' as type_mvt
      ,(D._QUANTITE*P._nominal)as Nominal_total         
      ,D._MONTANTNET
      
      
  FROM [OMEGA].[Fcp].[DIVID] D
  left outer join com.produit P on D._codeprodui=P._codeprodui
  left outer join fcp.cpartfcp C on D._compte=C._compte
  left outer join fcp.CONSGRPE cg on D._compte=cg._compte
  left outer join fcp.grpedeft g on g._codegroupe=cg._codegroupe
   where 
D._DATEVALEUR between @date1 and @date2 and C._actif='1' and g._codegroupe in ('CG','QX','GA')and C._libellecli not like '%actions%')


union

(select
  5 as 'style',
  g._nomgroupe
      ,convert(VARCHAR(10),O._DATEOPERATION,103) as Date_opération
	  ,convert(VARCHAR(10),O._DATEVALEUR,103) as Date_valeur
      ,''''+O._COMPTE
      ,C._libellecli
      ,O._CODEPRODUI
      ,P._isin
      ,P._libelle1prod     
      ,'RB' as type_mvt
      ,(O._QUANTITE*P._nominal)as Nominal_total         
      ,O._MTNET
FROM [OMEGA].fcp.operat O
  left outer join com.produit P on O._codeprodui=P._codeprodui
  left outer join fcp.cpartfcp C on O._compte=C._compte
  left outer join fcp.CONSGRPE cg on O._compte=cg._compte
  left outer join fcp.grpedeft g on g._codegroupe=cg._codegroupe
   where 
O._DATEVALEUR between @date1 and @date2 and C._actif='1' and g._codegroupe in ('CG','QX','GA')and C._libellecli not like '%actions%' and O._codeoperation='RB')

order by Date_valeur