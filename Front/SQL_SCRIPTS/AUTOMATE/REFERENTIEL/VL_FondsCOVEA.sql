use omega
select rtrim(convert(char(10),v._date,103)) as Date, v._codeprodui as ISIN,p._LIBELLE1PROD as Libelle, v._coursclose as VL from com.prixhist as v
left outer join com.produit as p on p._codeprodui=v._codeprodui 
where v._codeprodui in('FR0000445033','FR0000445058','FR0010752865')
 and v._date>getdate()-10
