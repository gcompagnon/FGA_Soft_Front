--------------------------------------------------------------------------------------
--																					--
--							MISE EN PRODUCTION : 14/01/2014							--
--						Ajout Champs dans la table DATA_FACTSET						--
--																					--
--------------------------------------------------------------------------------------

ALTER TABLE [dbo].[DATA_FACTSET]
ADD 
	PERF_MTD_EUR FLOAT NULL,
	PERF_YTD_EUR FLOAT NULL,
	PERF_1M_EUR FLOAT NULL,
	PERF_1YR_EUR FLOAT NULL
GO