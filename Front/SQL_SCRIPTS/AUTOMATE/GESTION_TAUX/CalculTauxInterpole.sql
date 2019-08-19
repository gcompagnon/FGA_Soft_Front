
declare @date as datetime
SET @date = (Select GETDATE())


execute [dbo].[TX_YTM_Interpolation] @date,'BARCLAYS'
execute [dbo].[TX_YTM_Interpolation] @date,'IBOXX_EUR'