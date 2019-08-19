USE [TEST]
GO

--|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|--
--|																											|--
--|											SCRIPT MIGRATION BASE ACTIONS									|--
--|																											|--
--|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|--



--[께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께�]--
--[											TABLE	MODIFIED												]--
--[께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께�]--
ALTER TABLE [dbo].[ACT_RECO_VALEUR] 
ADD reco_MXUSLC VARCHAR(4)
--------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ISR_NOTE]') AND type in (N'U'))
BEGIN
	DROP TABLE [dbo].[ISR_NOTE]

	CREATE TABLE [dbo].[ISR_NOTE](
		[DATE] [datetime] NOT NULL,
		[NAME] [varchar](100) NULL,
		[ISIN] [nchar](12) NOT NULL,
		[CORP] [varchar](10) NOT NULL,
		[Note Actions] [float] NULL,
		[Note Credit] [float] NULL,
	 CONSTRAINT [pk_ISR_NOTE] PRIMARY KEY CLUSTERED 
	(
		[DATE] ASC,
		[ISIN] ASC,
		[CORP] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
--------------------------------------------------------------------------------------------

--[께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께�]--
--[												NEW TABLE									  			    ]--
--[께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께께�]--

IF NOT EXISTS (
SELECT  schema_name
FROM    information_schema.schemata
WHERE   schema_name = 'ref_security' ) 

BEGIN
EXEC sp_executesql N'CREATE SCHEMA ref_security'
END
--------------------------------------------------------------------------------------------
IF NOT EXISTS (
SELECT  schema_name
FROM    information_schema.schemata
WHERE   schema_name = 'ref_common' ) 

BEGIN
EXEC sp_executesql N'CREATE SCHEMA ref_common'
END
--------------------------------------------------------------------------------------------
IF NOT EXISTS (
SELECT  schema_name
FROM    information_schema.schemata
WHERE   schema_name = 'ref_rating' ) 

BEGIN
EXEC sp_executesql N'CREATE SCHEMA ref_rating'
END
--------------------------------------------------------------------------------------------
--CREATE TABLE [ref_rating].[RATING](
--	[Id] [bigint] IDENTITY(1,1) NOT NULL,
--	[ISINId] [nchar](12) NOT NULL,
--	[AssetId] [bigint] NOT NULL,
--	[Value] [nvarchar](max) NULL,
--	[RatingScheme] [nvarchar](max) NULL,
--	[ValueDate] [datetime] NULL,
--	[Moody] [nvarchar](max) NULL,
--	[MoodyDate] [datetime] NULL,
--	[SnP] [nvarchar](max) NULL,
--	[SnPDate] [datetime] NULL,
--	[Fitch] [nvarchar](max) NULL,
--	[FitchDate] [datetime] NULL,
--PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]
--GO
--------------------------------------------------------------------------------------------
CREATE TABLE [dbo].[ACT_AGR_FORMAT](
	[Champs_FACTSET] [varchar](50) NOT NULL,
	[FORMAT] [int] NOT NULL,
	[PRECISION] [int] NULL,
	[GENERAL] [nchar](1) NULL,
	[GROWTH] [nchar](1) NULL,
	[VALUE] [nchar](1) NULL,
	[QUALITY] [nchar](1) NULL,
	[MOMENTUM] [nchar](1) NULL,
	[SYNTHESE] [nchar](1) NULL
) ON [PRIMARY]
GO
--------------------------------------------------------------------------------------------
CREATE TABLE [dbo].[ACT_PTF](
	[_datederniercours] [datetime] NOT NULL,
	[_compte] [varchar](20) NULL,
	[_libellecli] [varchar](50) NULL,
	[_codedutype] [nchar](3) NULL,
	[_codereuter] [varchar](50) NULL,
	[_codeprodui] [nchar](12) NULL,
	[_isin] [nchar](12) NULL,
	[_libelle1prod] [varchar](50) NULL,
	[_position] [float] NULL,
	[_prixmarche] [float] NULL,
	[_valorisation] [float] NULL,
	[_codedevise] [nchar](3) NULL,
	[forexEURXXX] [float] NULL,
	[dateForex] [datetime] NULL,
	[ActifNet] [float] NULL,
	[total] [float] NULL,
	[Poids] [float] NULL
) ON [PRIMARY]
--------------------------------------------------------------------------------------------
CREATE TABLE [dbo].[ACT_TICKER_CONVERSION](
	[ISIN] [nchar](12) NULL,
	[TICKER] [varchar](20) NULL,
	[BBG] [varchar](20) NULL,
	[EXCH_F] [nchar](2) NULL,
	[EXCH_B] [nchar](2) NULL
) ON [PRIMARY]
GO
--------------------------------------------------------------------------------------------
CREATE TABLE [ref_common].[IDENTIFICATION](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](350) NULL,
	[MaturityDate] [datetime] NULL,
	[ISIN] [nchar](12) NULL,
	[Country] [nchar](2) NULL,
	[OtherIdentification] [nvarchar](35) NULL,
	[ProprietaryIdentificationSource] [nvarchar](35) NULL,
	[Bloomberg] [nchar](20) NULL,
	[Reuters] [nchar](20) NULL,
	[SEDOL] [nchar](20) NULL,
	[CUSIP] [nchar](20) NULL,
	[Ticker] [nchar](20) NULL,
	[TradingIdentification] [nvarchar](70) NULL,
 CONSTRAINT [PK__IDENTIFI__3214EC0729E1370A] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
--------------------------------------------------------------------------------------------
CREATE TABLE [ref_security].[ASSET](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ISIN] [nchar](12) NOT NULL,
	[FinancialInstrumentName] [nvarchar](350) NULL,
	[MaturityDate] [datetime] NULL,
	[FinancialAssetCategory] [char](1) NULL,
	[IdentificationId] [bigint] NOT NULL,
	[RatingId] [bigint] NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_ASSET] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [ref_security].[ASSET]  WITH CHECK ADD  CONSTRAINT [Security_Identification] FOREIGN KEY([IdentificationId])
REFERENCES [ref_common].[IDENTIFICATION] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [ref_security].[ASSET] CHECK CONSTRAINT [Security_Identification]
GO
ALTER TABLE [ref_security].[ASSET]  WITH CHECK ADD  CONSTRAINT [Security_Rating] FOREIGN KEY([RatingId])
REFERENCES [ref_rating].[RATING] ([Id])
GO
ALTER TABLE [ref_security].[ASSET] CHECK CONSTRAINT [Security_Rating]
GO
--------------------------------------------------------------------------------------------
CREATE TABLE [ref_security].[EQUITY](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ISIN] [nchar](12) NOT NULL,
	[label] [varchar](150) NULL,
 CONSTRAINT [PK_EQUITY_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[ISIN] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [ref_security].[EQUITY]  WITH CHECK ADD  CONSTRAINT [FK_EQUITY_ASSET] FOREIGN KEY([Id])
REFERENCES [ref_security].[ASSET] ([Id])
GO
ALTER TABLE [ref_security].[EQUITY] CHECK CONSTRAINT [FK_EQUITY_ASSET]
GO
--------------------------------------------------------------------------------------------
CREATE TABLE [ref_security].[SECTOR](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[code] [nchar](20) NULL,
	[id_parent] [bigint] NULL,
	[level] [int] NULL,
	[label] [varchar](150) NULL,
	[class_name] [nchar](10) NULL,
 CONSTRAINT [PK_SECTOR] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [ref_security].[SECTOR]  WITH CHECK ADD  CONSTRAINT [FK_SECTOR_SECTOR] FOREIGN KEY([id_parent])
REFERENCES [ref_security].[SECTOR] ([id])
GO
ALTER TABLE [ref_security].[SECTOR] CHECK CONSTRAINT [FK_SECTOR_SECTOR]
GO
--------------------------------------------------------------------------------------------
CREATE TABLE [ref_security].[SECTOR_TRANSCO](
	[id_sector1] [bigint] NOT NULL,
	[id_sector2] [bigint] NOT NULL,
	[class_name] [nchar](10) NOT NULL,
 CONSTRAINT [PK_SECTOR_TRANSCO] PRIMARY KEY CLUSTERED 
(
	[id_sector1] ASC,
	[id_sector2] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [ref_security].[SECTOR_TRANSCO]  WITH CHECK ADD  CONSTRAINT [FK_SECTOR_TRANSCO_SECTOR] FOREIGN KEY([id_sector1])
REFERENCES [ref_security].[SECTOR] ([id])
GO
ALTER TABLE [ref_security].[SECTOR_TRANSCO] CHECK CONSTRAINT [FK_SECTOR_TRANSCO_SECTOR]
GO
ALTER TABLE [ref_security].[SECTOR_TRANSCO]  WITH CHECK ADD  CONSTRAINT [FK_SECTOR_TRANSCO_SECTOR1] FOREIGN KEY([id_sector2])
REFERENCES [ref_security].[SECTOR] ([id])
GO
ALTER TABLE [ref_security].[SECTOR_TRANSCO] CHECK CONSTRAINT [FK_SECTOR_TRANSCO_SECTOR1]
GO
--------------------------------------------------------------------------------------------
CREATE TABLE [ref_security].[ASSET_TO_SECTOR](
	[id_asset] [bigint] NOT NULL,
	[id_sector] [bigint] NOT NULL,
	[class_name] [nchar](10) NULL,
	[code_sector] [nchar](20) NULL,
	[source] [varchar](50) NOT NULL,
	[date] [datetime] NOT NULL,
 CONSTRAINT [PK_ASSET_TO_SECTOR] PRIMARY KEY CLUSTERED 
(
	[id_asset] ASC,
	[id_sector] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [ref_security].[ASSET_TO_SECTOR]  WITH CHECK ADD  CONSTRAINT [FK_ASSET_TO_SECTOR_ASSET] FOREIGN KEY([id_asset])
REFERENCES [ref_security].[ASSET] ([Id])
GO
ALTER TABLE [ref_security].[ASSET_TO_SECTOR] CHECK CONSTRAINT [FK_ASSET_TO_SECTOR_ASSET]
GO
ALTER TABLE [ref_security].[ASSET_TO_SECTOR]  WITH CHECK ADD  CONSTRAINT [FK_ASSET_TO_SECTOR_SECTOR] FOREIGN KEY([id_sector])
REFERENCES [ref_security].[SECTOR] ([id])
GO
ALTER TABLE [ref_security].[ASSET_TO_SECTOR] CHECK CONSTRAINT [FK_ASSET_TO_SECTOR_SECTOR]
GO
--------------------------------------------------------------------------------------------
CREATE TABLE [dbo].[DATA_FACTSET](
	[DATE] [datetime] NOT NULL,
	[TICKER] [varchar](20) NULL,
	[ISIN] [nchar](12) NULL,
	[SECTOR_LABEL] [varchar](50) NULL,
	[FGA_SECTOR] [nchar](6) NULL,
	[GICS_SECTOR] [nchar](2) NULL,
	[GICS_SUBINDUSTRY] [nchar](8) NULL,
	[COMPANY_NAME] [varchar](50) NULL,
	[LIQUIDITY] [float] NULL,
	[LIQUIDITY_TEST] [nchar](1) NULL,
	[COUNTRY] [nchar](2) NULL,
	[CURRENCY] [nchar](3) NULL,
	[PRICE] [float] NULL,
	[PRICE_EUR] [float] NULL,
	[SECTOR] [nchar](8) NULL,
	[SECTOR_NAME] [varchar](50) NULL,
	[MXEU] [float] NULL,
	[MXEM] [float] NULL,
	[MXEUM] [float] NULL,
	[MXFR] [float] NULL,
	[MXUSLC] [float] NULL,
	[6100001] [float] NULL,
	[6100002] [float] NULL,
	[6100004] [float] NULL,
	[6100024] [float] NULL,
	[6100026] [float] NULL,
	[6100030] [float] NULL,
	[6100033] [float] NULL,
	[6100062] [float] NULL,
	[6100063] [float] NULL,
	[AVEURO] [float] NULL,
	[AVEUROPE] [float] NULL,
	[SUIVI] [nchar](2) NULL,
	[MARKET_CAP_EUR] [float] NULL,
	[EV_NTM_EUR] [float] NULL,
	[SALES_NTM_EUR] [float] NULL,
	[FUNDAMENTALS] [float] NULL,
	[SALES_PPTM] [float] NULL,
	[SALES_PTM] [float] NULL,
	[SALES_LTM] [float] NULL,
	[SALES_NTM] [float] NULL,
	[SALES_STM] [float] NULL,
	[CAPEX_PPTM] [float] NULL,
	[CAPEX_PTM] [float] NULL,
	[CAPEX_LTM] [float] NULL,
	[CAPEX_NTM] [float] NULL,
	[CAPEX_STM] [float] NULL,
	[EBIT_PPTM] [float] NULL,
	[EBIT_PTM] [float] NULL,
	[EBIT_LTM] [float] NULL,
	[EBIT_NTM] [float] NULL,
	[EBIT_STM] [float] NULL,
	[FCF_PPTM] [float] NULL,
	[FCF_PTM] [float] NULL,
	[FCF_LTM] [float] NULL,
	[FCF_NTM] [float] NULL,
	[FCF_STM] [float] NULL,
	[EPS_PPTM] [float] NULL,
	[EPS_PTM] [float] NULL,
	[EPS_LTM] [float] NULL,
	[EPS_NTM] [float] NULL,
	[EPS_STM] [float] NULL,
	[NI_CFO_PPTM] [float] NULL,
	[NI_CFO_PTM] [float] NULL,
	[NI_CFO_LTM] [float] NULL,
	[NI_CFO_NTM] [float] NULL,
	[NI_CFO_STM] [float] NULL,
	[NI_CFO_STD] [float] NULL,
	[EBIT_MARGIN_PPTM] [float] NULL,
	[EBIT_MARGIN_PTM] [float] NULL,
	[EBIT_MARGIN_LTM] [float] NULL,
	[EBIT_MARGIN_NTM] [float] NULL,
	[EBIT_MARGIN_STM] [float] NULL,
	[FCF_YLD_PPTM] [float] NULL,
	[FCF_YLD_PTM] [float] NULL,
	[FCF_YLD_LTM] [float] NULL,
	[FCF_YLD_NTM] [float] NULL,
	[FCF_YLD_STM] [float] NULL,
	[CAPEX_SALES_LTM] [float] NULL,
	[CAPEX_SALES_NTM] [float] NULL,
	[CAPEX_SALES_STM] [float] NULL,
	[GEARING_LTM] [float] NULL,
	[GEARING_NTM] [float] NULL,
	[GEARING_STM] [float] NULL,
	[NET_DEBT_EBITDA_LTM] [float] NULL,
	[NET_DEBT_EBITDA_NTM] [float] NULL,
	[NET_DEBT_EBITDA_STM] [float] NULL,
	[ROE_LTM] [float] NULL,
	[ROE_NTM] [float] NULL,
	[ROE_STM] [float] NULL,
	[ROTE_LTM] [float] NULL,
	[ROTE_NTM] [float] NULL,
	[ROTE_STM] [float] NULL,
	[PE_LTM] [float] NULL,
	[PE_NTM] [float] NULL,
	[PE_STM] [float] NULL,
	[PB_LTM] [float] NULL,
	[PB_NTM] [float] NULL,
	[PB_STM] [float] NULL,
	[P_TBV_LTM] [float] NULL,
	[P_TBV_NTM] [float] NULL,
	[P_TBV_STM] [float] NULL,
	[EV_SALES_LTM] [float] NULL,
	[EV_SALES_NTM] [float] NULL,
	[EV_SALES_STM] [float] NULL,
	[EV_EBITDA_LTM] [float] NULL,
	[EV_EBITDA_NTM] [float] NULL,
	[EV_EBITDA_STM] [float] NULL,
	[EV_EBITA_LTM] [float] NULL,
	[EV_EBITA_NTM] [float] NULL,
	[EV_EBITA_STM] [float] NULL,
	[EV_EBIT_LTM] [float] NULL,
	[EV_EBIT_NTM] [float] NULL,
	[EV_EBIT_STM] [float] NULL,
	[PAYOUT_LTM] [float] NULL,
	[PAYOUT_NTM] [float] NULL,
	[PAYOUT_STM] [float] NULL,
	[DIV_YLD_LTM] [float] NULL,
	[DIV_YLD_NTM] [float] NULL,
	[DIV_YLD_STM] [float] NULL,
	[COST_INCOME_LTM] [float] NULL,
	[COST_INCOME_NTM] [float] NULL,
	[COST_INCOME_STM] [float] NULL,
	[TIER_1_LTM] [float] NULL,
	[TIER_1_NTM] [float] NULL,
	[TIER_1_STM] [float] NULL,
	[PBT_RWA_LTM] [float] NULL,
	[PBT_RWA_NTM] [float] NULL,
	[PBT_RWA_STM] [float] NULL,
	[PBT_SALES_LTM] [float] NULL,
	[PBT_SALES_NTM] [float] NULL,
	[PBT_SALES_STM] [float] NULL,
	[RORWA_LTM] [float] NULL,
	[RORWA_NTM] [float] NULL,
	[RORWA_STM] [float] NULL,
	[P_EMB_VALUE_LTM] [float] NULL,
	[P_EMB_VALUE_NTM] [float] NULL,
	[P_EMB_VALUE_STM] [float] NULL,
	[COMBINED_RATIO_LTM] [float] NULL,
	[COMBINED_RATIO_NTM] [float] NULL,
	[COMBINED_RATIO_STM] [float] NULL,
	[LOSS_RATIO_LTM] [float] NULL,
	[LOSS_RATIO_NTM] [float] NULL,
	[LOSS_RATIO_STM] [float] NULL,
	[EXPENSE_RATIO_LTM] [float] NULL,
	[EXPENSE_RATIO_NTM] [float] NULL,
	[EXPENSE_RATIO_STM] [float] NULL,
	[PE_NTM_MED5Y] [float] NULL,
	[PB_NTM_MED5Y] [float] NULL,
	[P_TBV_NTM_MED5Y] [float] NULL,
	[EV_SALES_NTM_MED5Y] [float] NULL,
	[EV_EBITDA_NTM_MED5Y] [float] NULL,
	[EV_EBIT_NTM_MED5Y] [float] NULL,
	[EBIT_MARGIN_NTM_MED5Y] [float] NULL,
	[PE_ON_MED5Y] [float] NULL,
	[PB_ON_MED5Y] [float] NULL,
	[P_TBV_ON_MED5Y] [float] NULL,
	[EV_SALES_ON_MED5Y] [float] NULL,
	[EV_EBITDA_ON_MED5Y] [float] NULL,
	[EV_EBIT_ON_MED5Y] [float] NULL,
	[EBIT_MARGIN_ON_MED5Y] [float] NULL,
	[IGROWTH_NTM] [float] NULL,
	[PEG_NTM] [float] NULL,
	[EV_EBITDA_TO_G_NTM] [float] NULL,
	[EV_EBITA_TO_G_NTM] [float] NULL,
	[EV_EBIT_TO_G_NTM] [float] NULL,
	[CALCULATION] [float] NULL,
	[SALES_COUNT] [float] NULL,
	[SALES_TREND] [float] NULL,
	[SALES_RSD] [float] NULL,
	[SALES_RSD_STD_CALC] [float] NULL,
	[SALES_RSD_AVG_CALC] [float] NULL,
	[SALES_VAR_RSD] [float] NULL,
	[CAPEX_COUNT] [float] NULL,
	[CAPEX_TREND] [float] NULL,
	[CAPEX_RSD] [float] NULL,
	[CAPEX_RSD_STD_CALC] [float] NULL,
	[CAPEX_RSD_AVG_CALC] [float] NULL,
	[CAPEX_VAR_RSD] [float] NULL,
	[EBIT_COUNT] [float] NULL,
	[EBIT_TREND] [float] NULL,
	[EBIT_RSD] [float] NULL,
	[EBIT_RSD_STD_CALC] [float] NULL,
	[EBIT_RSD_AVG_CALC] [float] NULL,
	[EBIT_VAR_RSD] [float] NULL,
	[FCF_COUNT] [float] NULL,
	[FCF_TREND] [float] NULL,
	[FCF_RSD] [float] NULL,
	[FCF_VAR_RSD] [float] NULL,
	[EPS_COUNT] [float] NULL,
	[EPS_TREND] [float] NULL,
	[EPS_RSD] [float] NULL,
	[EPS_RSD_STD_CALC] [float] NULL,
	[EPS_RSD_AVG_CALC] [float] NULL,
	[EPS_VAR_RSD] [float] NULL,
	[EBIT_MARGIN_COUNT] [float] NULL,
	[EBIT_MARGIN_TREND] [float] NULL,
	[EBIT_MARGIN_RSD] [float] NULL,
	[EBIT_MARGIN_RSD_STD_CALC] [float] NULL,
	[EBIT_MARGIN_RSD_AVG_CALC] [float] NULL,
	[EBIT_MARGIN_VAR_RSD] [float] NULL,
	[FCF_YLD_COUNT] [float] NULL,
	[FCF_YLD_TREND] [float] NULL,
	[FCF_YLD_RSD] [float] NULL,
	[FCF_YLD_VAR_RSD] [float] NULL,
	[VARIATION] [float] NULL,
	[SALES_CHG_NTM] [float] NULL,
	[SALES_CHG_STM] [float] NULL,
	[CAPEX_CHG_NTM] [float] NULL,
	[CAPEX_CHG_STM] [float] NULL,
	[EBIT_CHG_NTM] [float] NULL,
	[EBIT_CHG_STM] [float] NULL,
	[FCF_CHG_NTM] [float] NULL,
	[FCF_CHG_STM] [float] NULL,
	[EPS_CHG_NTM] [float] NULL,
	[EPS_CHG_STM] [float] NULL,
	[EBIT_MARGIN_DIFF_NTM] [float] NULL,
	[EBIT_MARGIN_DIFF_STM] [float] NULL,
	[FCF_YLD_DIFF_NTM] [float] NULL,
	[FCF_YLD_DIFF_STM] [float] NULL,
	[PBT_SALES_DIFF_NTM] [float] NULL,
	[PBT_SALES_DIFF_STM] [float] NULL,
	[PBT_RWA_DIFF_NTM] [float] NULL,
	[PBT_RWA_DIFF_STM] [float] NULL,
	[MOMENTUM] [float] NULL,
	[PE_PCTIL] [float] NULL,
	[PB_PCTIL] [float] NULL,
	[EV_SALES_PCTIL] [float] NULL,
	[EV_EBITDA_PCTIL] [float] NULL,
	[EV_EBIT_PCTIL] [float] NULL,
	[EBIT_MARGIN_PCTIL] [float] NULL,
	[FCF_YLD_PCTIL] [float] NULL,
	[PERF_1M] [float] NULL,
	[PERF_3M] [float] NULL,
	[PERF_6M] [float] NULL,
	[PERF_1YR] [float] NULL,
	[PERF_MTD] [float] NULL,
	[PERF_YTD] [float] NULL,
	[VOL_1M] [float] NULL,
	[VOL_3M] [float] NULL,
	[VOL_1YR] [float] NULL,
	[BETA_1YR] [float] NULL,
	[PRICE_PCTIL_1M] [float] NULL,
	[PRICE_PCTIL_3M] [float] NULL,
	[PRICE_PCTIL_6M] [float] NULL,
	[PRICE_PCTIL_1YR] [float] NULL,
	[PRICE_PCTIL_5YR] [float] NULL,
	[PRICE_52_HIGH] [float] NULL,
	[PRICE_52_LOW] [float] NULL,
	[EPS_NTMA] [float] NULL,
	[EPS_CHG_1M] [float] NULL,
	[EPS_CHG_3M] [float] NULL,
	[EPS_CHG_6M] [float] NULL,
	[EPS_CHG_1YR] [float] NULL,
	[EPS_CHG_YTD] [float] NULL,
	[TARGET] [float] NULL,
	[UPSIDE] [float] NULL,
	[RATING_POS_PCT] [float] NULL,
	[RATING_TOT] [float] NULL,
	[EPS_BROKER_UP_REV] [float] NULL,
	[PRICE_BROKER_UP_REV] [float] NULL,
	[PE_PREMIUM_ON_HIST] [float] NULL,
	[PB_PREMIUM_ON_HIST] [float] NULL,
	[PE_NTM_AVG] [float] NULL,
	[PB_NTM_AVG] [float] NULL,
	[PE_NTM_MED5YR_AVG] [float] NULL,
	[PB_NTM_MED5YR_AVG] [float] NULL,
	[GARPN_QUINTILE_S] [float] NULL,
	[GARPN_RANKING_S] [float] NULL,
	[GARPN_YIELD_S] [float] NULL,
	[GARPN_VALUE_S] [float] NULL,
	[GARPN_GROWTH_S] [float] NULL,
	[GARPN_ISR_S] [float] NULL,
	[GARPN_NOTE_S] [float] NULL,
	[GARPN_TOTAL_S] [float] NULL,
	[BLEND_TOTAL] [float] NULL,
	[BLEND_GROWTH] [float] NULL,
	[BLEND_YIELD] [float] NULL,
	[BLEND_VALUE] [float] NULL,
	[ESG] [varchar](10) NULL,
	[N_EPS_VAR_RSD] [float] NULL,
	[N_EPS_TREND] [float] NULL,
	[N_EPS_CHG_NTM] [float] NULL,
	[N_SALES_CHG_NTM] [float] NULL,
	[N_SALES_TREND] [float] NULL,
	[N_SALES_VAR_RSD] [float] NULL,
	[N_PE_NTM] [float] NULL,
	[N_PE_ON_MED5Y] [float] NULL,
	[N_PE_PREMIUM_ON_HIST] [float] NULL,
	[N_P_TBV_NTM] [float] NULL,
	[N_PB_NTM] [float] NULL,
	[N_PB_ON_MED5Y] [float] NULL,
	[N_P_TBV_ON_MED5Y] [float] NULL,
	[N_PB_PREMIUM_ON_HIST] [float] NULL,
	[N_DIV_YLD_NTM] [float] NULL,
	[N_PBT_SALES_NTM] [float] NULL,
	[N_EBIT_MARGIN_NTM] [float] NULL,
	[N_PBT_RWA_NTM] [float] NULL,
	[N_FCF_TREND] [float] NULL,
	[N_NET_DEBT_EBITDA_NTM] [float] NULL,
	[N_ROE_NTM] [float] NULL,
	[N_ROTE_NTM] [float] NULL,
	[N_COST_INCOME_NTM] [float] NULL
) ON [PRIMARY]
--------------------------------------------------------------------------------------------