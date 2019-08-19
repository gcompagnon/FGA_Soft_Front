--------------------------------------------------------------------------------------
--																					--
--							MISE EN PRODUCTION : 11/12/2014							--
--						Ajout Champs dans la table DATA_FACTSET						--
--																					--
--------------------------------------------------------------------------------------

ALTER TABLE [dbo].[DATA_FACTSET]
ADD 
GARPN_TOTAL_NO_ISR_S FLOAT NULL


ALTER TABLE [dbo].[DATA_FACTSET]
ADD 
6100066  FLOAT NULL
 

-- nettoyage des tables  inutiles

drop table COTATION1 
drop table [ACT_FGA_SECTOR_RECOMMANDATION]
drop table [ACT_FILE_LINK]
drop table [ACT_FILE]
drop table [DE_B]
drop table [FR_61]
drop table [FR_85]
drop table [KIM]
drop table [YearMaturity]
drop table [Notation_transco]
drop table [histo_time]
