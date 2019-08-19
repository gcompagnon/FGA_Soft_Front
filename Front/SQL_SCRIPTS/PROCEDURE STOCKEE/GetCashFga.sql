/*
 Ajout les lignes de cash dans la table PTF_FGA lors de l'ajout d'un nouveau fichier excel dans la table
*/
       
GO

CREATE PROCEDURE GetCashFga

       @date DATETIME
AS 

SELECT  Compte, SUM(Valeur_Boursiere+Coupon_Couru) AS Somme into #tmp_som_ligne FROM PTF_FGA 
WHERE DateInventaire =  @date 
GROUP BY Compte ORDER BY Compte

SELECT p.compte, AN-somme AS CASH FROM PTF_AN p, #tmp_som_ligne t where p.compte=t.compte and p.date = @date 
ORDER BY compte 

drop table #tmp_som_ligne
        