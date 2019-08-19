--
-- Supprime les doublons des différent fichier excel : Malakoff, Médéric, Auxia
--

CREATE PROCEDURE SuprimmerDoublonChorus
       @date DATETIME
AS 

--DECLARE @date As DATETIME
--SET @date = '31/08/2011'

DELETE FROM PTF_CHORUS where date_inventaire=@date

INSERT INTO PTF_CHORUS
SELECT DISTINCT * FROM PTF_DOUBLON_CHORUS 
where date_inventaire=@date
--order by compte,code_titre

--SELECT DISTINCT compte,code_titre FROM PTF_DOUBLON_CHORUS 
--where date_inventaire='31/08/2011'

DELETE FROM PTF_DOUBLON_CHORUS where date_inventaire=@date

--SELECT * FROM PTF_DOUBLON_CHORUS where date_inventaire='31/08/2011'