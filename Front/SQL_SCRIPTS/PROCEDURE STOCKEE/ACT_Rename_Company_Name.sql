-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Aurélien Brandicourt
-- Create date: 29/11/2012
-- Description:	Rename old company name records
-- =============================================
CREATE PROCEDURE ACT_Rename_Company_Name 
	-- Add the parameters for the stored procedure here
	@table VARCHAR(100) = 0, 
	@old_name VARCHAR(100) = 0,
	@new_name VARCHAR(100) = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    DECLARE @sql as VARCHAR(MAX)
    
    --DECLARE @table as VARCHAR(100)
    --DECLARE @old_name as VARCHAR(100)
    --DECLARE @new_name as VARCHAR(100)
    --SET @table = 'ACT_DATA_FACTSET'
    --SET @old_name = 'Societe Generale'
    --SET @new_name = 'Societe Generale'
    
    SET @sql = 'UPDATE ' + @table
    SET @sql = @sql + ' SET COMPANY_NAME=''' + @new_name + ''''
    SET @sql = @sql + ' WHERE COMPANY_NAME=''' + @old_name + ''''
    
    EXEC (@sql)
    
    --EXEC ('SELECT * FROM ' + @table + ' WHERE COMPANY_NAME=''' + @new_name + '''')
    
END
GO
