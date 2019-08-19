
declare @dateTransparence datetime
set @dateTransparence = '30/06/2015'


execute dbo.Trans1 @dateTransparence
execute dbo.Trans2 @dateTransparence
execute dbo.Trans3 @dateTransparence
execute dbo.Trans4 @dateTransparence
execute dbo.Trans5 @dateTransparence