declare @date as datetime
SET @date = (Select max (dateInventaire) from PTF_FGA where Compte = '7201106')

declare @rapportCle as char(20)
set @rapportCle = 'CompteRating'


declare @comptes varchar(200)
set @comptes='3020010,3020011,3020012'

declare @cle_Montant as char(20)
set @cle_Montant = 'CompteRating'

declare @encoursTotal as float
set @encoursTotal = (Select valeur from ptf_rapport where date=@date and cle=@cle_montant
and classementRubrique=0 and groupe=@comptes)


DECLARE @query NVARCHAR(1000)
set @query=N'SELECT case when pvt.classementRubrique=0 then 0
					else 1
					end as style,
					case when pvt.classementRubrique=0 then ''''
					else ''graph1;serie1''
					end as graph,
					pvt.classementRubrique,pvt.rubrique,pvt.libelle,'
					+ '['+replace(@comptes,',','],[')+']'+',['+@comptes+'] as Total, r.valeur/''' + CONVERT(varchar(30),@encoursTotal,2) +''' as "%"
FROM	(	SELECT    p.groupe,p.classementRubrique,p.rubrique,p.libelle,p.valeur
			FROM      ptf_rapport as p
			WHERE p.date='''+ CONVERT ( varchar(20),@date,103 ) + ''' and p.cle=''' + @rapportCle + ''') as pp 
PIVOT ( SUM(valeur) FOR [groupe] IN (' + '['+replace(@comptes,',','],[') + ']'+',['+@comptes+'])
      ) As pvt
JOIN ptf_rapport as r on r.cle='''+@cle_montant+''' 
			and r.date='''+ CONVERT ( varchar(20),@date,103 ) + '''
			and r.classementRubrique=pvt.classementRubrique 
			and r.groupe='''+@comptes+'''
ORDER BY classementRubrique'

PRINT @query
EXECUTE(@query)



set @rapportCle = 'CompteRatingDu'
set @cle_Montant = 'CompteRating'

set @query=N'SELECT case when pvt.classementRubrique=0 then 2
					else 3
					end as style,
					case when pvt.classementRubrique=0 then ''''
					else ''graph2;serie2''
					end as graph,
					pvt.classementRubrique,pvt.rubrique,pvt.libelle,'
					+ '['+replace(@comptes,',','],[')+']'+',['+@comptes+'] as Total, r.valeur/''' + CONVERT(varchar(30),@encoursTotal,2) +''' as "%"
FROM	(	SELECT    p.groupe,p.classementRubrique,p.rubrique,p.libelle,p.valeur
			FROM      ptf_rapport as p
			WHERE p.date='''+ CONVERT ( varchar(20),@date,103 ) + ''' and p.cle=''' + @rapportCle + ''') as pp 
PIVOT ( SUM(valeur) FOR [groupe] IN (' + '['+replace(@comptes,',','],[') + ']'+',['+@comptes+'])
      ) As pvt
JOIN ptf_rapport as r on r.cle='''+@cle_montant+''' 
			and r.date='''+ CONVERT ( varchar(20),@date,103 ) + '''
			and r.classementRubrique=pvt.classementRubrique 
			and r.groupe='''+@comptes+'''
ORDER BY classementRubrique'

PRINT @query
EXECUTE(@query)

set @rapportCle = 'CompteMaturite'
set @cle_Montant = 'CompteMaturite'

set @query=N'SELECT case when pvt.classementRubrique=0 then 2
					else 3
					end as style,
					case when pvt.classementRubrique=0 then ''''
					else ''graph3''
					end as graph,
					pvt.classementRubrique,pvt.rubrique,pvt.libelle,'
					+ '['+replace(@comptes,',','],[')+']'+',['+@comptes+'] as Total, r.valeur/''' + CONVERT(varchar(30),@encoursTotal,2) +''' as "%"
FROM	(	SELECT    p.groupe,p.classementRubrique,p.rubrique,p.libelle,p.valeur
			FROM      ptf_rapport as p
			WHERE p.date='''+ CONVERT ( varchar(20),@date,103 ) + ''' and p.cle=''' + @rapportCle + ''') as pp 
PIVOT ( SUM(valeur) FOR [groupe] IN (' + '['+replace(@comptes,',','],[') + ']'+',['+@comptes+'])
      ) As pvt
JOIN ptf_rapport as r on r.cle='''+@cle_montant+''' 
			and r.date='''+ CONVERT ( varchar(20),@date,103 ) + '''
			and r.classementRubrique=pvt.classementRubrique 
			and r.groupe='''+@comptes+'''
ORDER BY classementRubrique'

PRINT @query
EXECUTE(@query)


