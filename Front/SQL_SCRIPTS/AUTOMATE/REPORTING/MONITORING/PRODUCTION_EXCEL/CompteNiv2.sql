declare @date as datetime
SET @date =(Select max (dateInventaire) from PTF_FGA where Compte = '7201106')

declare @rapportCle as char(20)
set @rapportCle = 'CompteExpoPays'


declare @comptes varchar(200)
set @comptes='6100020,6100021'

declare @cle_Montant as char(20)
set @cle_Montant = 'CompteExpoPays'

declare @encoursTotal as float
set @encoursTotal = (Select valeur from ptf_rapport_niv2 where date=@date and cle=@cle_montant
and classementRubrique=0 and groupe=@comptes)


DECLARE @query NVARCHAR(4000)
SET @query=N'
SELECT case when (100*pvt.classementRubrique +10*pvt.classementSousRubrique)=0 then 1 
		when (100*pvt.classementRubrique +10*pvt.classementSousRubrique)/10000=CONVERT(INT,(100*pvt.classementRubrique +10*pvt.classementSousRubrique)/10000) then 0
		when (100*pvt.classementRubrique +10*pvt.classementSousRubrique)/100=CONVERT(INT,(100*pvt.classementRubrique +10*pvt.classementSousRubrique)/100) then 2
                when pvt.classementRubrique/10=CONVERT(INT,pvt.classementRubrique/10) then 3
                else 4
		end as style,
case when pvt.classementRubrique=0 then '' ''
		when (100*pvt.classementRubrique +10*pvt.classementSousRubrique)/10000=CONVERT(INT,(100*pvt.classementRubrique +10*pvt.classementSousRubrique)/10000) then ''graph1''
		else '' ''
		end as graph,
					pvt.classementRubrique,pvt.classementSousRubrique,
					pvt.rubrique,pvt.SousRubrique,pvt.libelle,'
					+ '['+replace(@comptes,',','],[')+']'+',['+@comptes+'] as Total, r.valeur/''' + CONVERT(varchar(30),@encoursTotal,2)+''' as "%"
FROM	(	SELECT    p.groupe,p.classementRubrique,p.classementSousRubrique,
						p.SousRubrique,p.rubrique,p.libelle,p.valeur
			FROM      ptf_rapport_niv2 as p
			WHERE p.date='''+ CONVERT ( varchar(20),@date,103 ) + ''' and p.cle=''' + @rapportCle + ''') as pp 
PIVOT ( SUM(valeur) FOR [groupe] IN (' + '['+replace(@comptes,',','],[') + ']'+',['+@comptes+'])
      ) As pvt
JOIN ptf_rapport_niv2 as r on r.cle='''+@cle_montant+'''
			and r.date='''+ CONVERT ( varchar(20),@date,103 ) + '''
			and r.classementRubrique=pvt.classementRubrique 
			and r.classementSousRubrique=pvt.classementSousRubrique  
			and r.groupe='''+@comptes+'''
ORDER BY classementRubrique, classementSousRubrique'

PRINT @query
EXECUTE(@query)
