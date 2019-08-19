DROP PROCEDURE ACT_Valeur_Note

GO

CREATE PROCEDURE ACT_Valeur_Note

	@date As DATETIME,
	@id_fga As VARCHAR(4)

AS

--Execute ACT_Valeur_Note '26/03/2012', '1'

SELECT
		t.TICKER_BLOOMBERG As 'Ticker',
		a.company_name As 'Company Name',
		a.liquidity_test As 'Liquidity',
		NULL As 'mes thèmes'
FROM ACT_DATA_FACTSET a
LEFT OUTER JOIN ACT_FGA_SECTOR s ON s.id= a.fga_sector
LEFT OUTER JOIN ACT_VALEUR t ON a.isin = t.isin
WHERE a.date=@date and a.FGA_SECTOR=@id_fga


--Select * from ACT_DATA_FACTSET
