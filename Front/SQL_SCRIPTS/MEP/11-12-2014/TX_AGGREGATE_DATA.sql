SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TX_AGGREGATE_DATA]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TX_AGGREGATE_DATA](
	[Date] Datetime not null,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[key1] [char](5) not null,
	[key2] nvarchar(60) null,
	[key3] nvarchar(60) null,
	[key4] nvarchar(60) null,
	[key5] nvarchar(60) null,
	[Value] float 
 CONSTRAINT [pk_TX_AGGREGATE_DATA] PRIMARY KEY CLUSTERED 
(
	[Date] ASC,
	[Id] ASC,
	[key1] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END

