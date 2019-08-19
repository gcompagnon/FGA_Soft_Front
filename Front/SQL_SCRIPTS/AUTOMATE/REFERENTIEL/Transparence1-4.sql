
declare @dateTransparence datetime
set @dateTransparence = '16/05/2012'


execute dbo.Trans1 @dateTransparence
execute dbo.Trans2 @dateTransparence
execute dbo.Trans3 @dateTransparence
execute dbo.Trans4 @dateTransparence