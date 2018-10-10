alter table Project ADD Unload int
alter table Project ADD WaitOnJob int
alter table Project ADD AverageLoadSize float

ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_ReasonForLoss] FOREIGN KEY([ReasonLostId])
REFERENCES [dbo].[ReasonsForLoss] ([ReasonLostId])
GO

alter table RoleAccess ADD EditActual bit;
update RoleAccess set EditActual = 0;
alter table RoleAccess alter column EditActual bit NOT NULL;

--- IMPORTED ---

DROP TABLE [dbo].[DistrictMarketSegment];

CREATE TABLE [dbo].[DistrictMarketSegment](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DistrictId] [int] NOT NULL,
	[MarketSegmentId] [int] NOT NULL,
	[Spread] [money] NULL,
	[ContMarg] [money] NULL,
	[Profit] [money] NULL,
	[CydHr] [float] NULL,
 CONSTRAINT [PK_DistrictMarketSegment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DistrictMarketSegment]  WITH CHECK ADD  CONSTRAINT [FK_DistrictToMarketSegment_DistrictToMarketSegment] FOREIGN KEY([DistrictId])
REFERENCES [dbo].[District] ([DistrictId])
GO

ALTER TABLE [dbo].[DistrictMarketSegment] CHECK CONSTRAINT [FK_DistrictToMarketSegment_DistrictToMarketSegment]
GO

ALTER TABLE [dbo].[DistrictMarketSegment]  WITH CHECK ADD  CONSTRAINT [FK_DistrictToMarketSegment_MarketSegment] FOREIGN KEY([MarketSegmentId])
REFERENCES [dbo].[MarketSegment] ([MarketSegmentId])
GO

ALTER TABLE [dbo].[DistrictMarketSegment] CHECK CONSTRAINT [FK_DistrictToMarketSegment_MarketSegment]
GO

-- MAY BE IMPORTED TILL HERE --



-- UOM

CREATE TABLE [dbo].[Uom](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Category] [nvarchar](50) NOT NULL,
	[BaseConversion] [float] NOT NULL,
 CONSTRAINT [PK_Uom] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Uom] ADD  CONSTRAINT [DF_Uom_BaseConversion]  DEFAULT ((1)) FOR [BaseConversion]
GO

-- Global settings

CREATE TABLE [dbo].[GlobalSettings](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MD1] [nvarchar](50) NULL,
	[MD2] [nvarchar](50) NULL,
	[MD3] [nvarchar](50) NULL,
	[MD4] [nvarchar](50) NULL,
	[JI1] [nvarchar](50) NULL,
	[JI2] [nvarchar](50) NULL,
	[NonFutureCutoff] [int] NOT NULL,
 CONSTRAINT [PK_GlobalSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GlobalSettings] ADD  CONSTRAINT [DF_GlobalSettings_NonFutureCutoff]  DEFAULT ((5)) FOR [NonFutureCutoff]
GO

-- Raw material types

CREATE TABLE [dbo].[RawMaterialType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[IsCementitious] [bit] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_RawMaterialType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

-- Raw Materials

CREATE TABLE [dbo].[RawMaterial](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MaterialCode] [varchar](100) NOT NULL,
	[Description] [varchar](255) NULL,
	[RawMaterialTypeId] [bigint] NOT NULL,
	[Active] [bit] NOT NULL,
	[MeasurementType] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RawMaterial] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[RawMaterial]  WITH CHECK ADD  CONSTRAINT [FK_RawMaterial_RawMaterialType] FOREIGN KEY([RawMaterialTypeId])
REFERENCES [dbo].[RawMaterialType] ([Id])
GO

ALTER TABLE [dbo].[RawMaterial] CHECK CONSTRAINT [FK_RawMaterial_RawMaterialType]
GO

-- Raw material costs

CREATE TABLE [dbo].[RawMaterialCostProjection](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RawMaterialId] [bigint] NOT NULL,
	[PlantId] [int] NOT NULL,
	[ChangeDate] [datetime] NOT NULL,
	[Cost] [decimal](18, 2) NOT NULL,
	[UomId] [bigint] NOT NULL,
 CONSTRAINT [PK_RawMaterialCostProjection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RawMaterialCostProjection]  WITH CHECK ADD  CONSTRAINT [FK_RawMaterialCostProjection_Plant] FOREIGN KEY([PlantId])
REFERENCES [dbo].[Plant] ([PlantId])
GO

ALTER TABLE [dbo].[RawMaterialCostProjection] CHECK CONSTRAINT [FK_RawMaterialCostProjection_Plant]
GO

ALTER TABLE [dbo].[RawMaterialCostProjection]  WITH CHECK ADD  CONSTRAINT [FK_RawMaterialCostProjection_RawMaterial] FOREIGN KEY([RawMaterialId])
REFERENCES [dbo].[RawMaterial] ([Id])
GO

ALTER TABLE [dbo].[RawMaterialCostProjection] CHECK CONSTRAINT [FK_RawMaterialCostProjection_RawMaterial]
GO

ALTER TABLE [dbo].[RawMaterialCostProjection]  WITH CHECK ADD  CONSTRAINT [FK_RawMaterialCostProjection_Uom] FOREIGN KEY([UomId])
REFERENCES [dbo].[Uom] ([Id])
GO

ALTER TABLE [dbo].[RawMaterialCostProjection] CHECK CONSTRAINT [FK_RawMaterialCostProjection_Uom]
GO

-- Addons

CREATE TABLE [dbo].[Addon](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[AddonType] [nvarchar](50) NOT NULL,
	[QuoteUomId] [bigint] NULL,
	[MixCostUomId] [bigint] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_Addon] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Addon]  WITH CHECK ADD  CONSTRAINT [FK_Addon_Mix_Uom] FOREIGN KEY([MixCostUomId])
REFERENCES [dbo].[Uom] ([Id])
GO

ALTER TABLE [dbo].[Addon] CHECK CONSTRAINT [FK_Addon_Mix_Uom]
GO


-- Addon Projections

CREATE TABLE [dbo].[AddonPriceProjection](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AddonId] [bigint] NOT NULL,
	[DistrictId] [int] NOT NULL,
	[ChangeDate] [datetime] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[UomId] [bigint] NOT NULL,
	[PriceMode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AddonPriceProjection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[AddonPriceProjection] ADD  CONSTRAINT [DF_AddonPriceProjection_PriceMode]  DEFAULT ('QUOTE') FOR [PriceMode]
GO

ALTER TABLE [dbo].[AddonPriceProjection]  WITH CHECK ADD  CONSTRAINT [FK_AddonPriceProjection_Addon] FOREIGN KEY([AddonId])
REFERENCES [dbo].[Addon] ([Id])
GO

ALTER TABLE [dbo].[AddonPriceProjection] CHECK CONSTRAINT [FK_AddonPriceProjection_Addon]
GO

ALTER TABLE [dbo].[AddonPriceProjection]  WITH CHECK ADD  CONSTRAINT [FK_AddonPriceProjection_District] FOREIGN KEY([DistrictId])
REFERENCES [dbo].[District] ([DistrictId])
GO

ALTER TABLE [dbo].[AddonPriceProjection] CHECK CONSTRAINT [FK_AddonPriceProjection_District]
GO

ALTER TABLE [dbo].[AddonPriceProjection]  WITH CHECK ADD  CONSTRAINT [FK_AddonPriceProjection_Uom] FOREIGN KEY([UomId])
REFERENCES [dbo].[Uom] ([Id])
GO

ALTER TABLE [dbo].[AddonPriceProjection] CHECK CONSTRAINT [FK_AddonPriceProjection_Uom]
GO

-- Standard Mix

CREATE TABLE [dbo].[StandardMix](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Number] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](200) NOT NULL,
	[SalesDesc] [nvarchar](200) NULL,
	[PSI] [int] NULL,
	[Slump] [nvarchar](50) NULL,
	[Air] [nvarchar](50) NULL,
	[MD1] [nvarchar](50) NULL,
	[MD2] [nvarchar](50) NULL,
	[MD3] [nvarchar](50) NULL,
	[MD4] [nvarchar](50) NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_StandardMix] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Mix Formulations

CREATE TABLE [dbo].[MixFormulation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[StandardMixId] [bigint] NOT NULL,
	[PlantId] [int] NOT NULL,
 CONSTRAINT [PK_Formulation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[MixFormulation]  WITH CHECK ADD  CONSTRAINT [FK_Formulation_Plant] FOREIGN KEY([PlantId])
REFERENCES [dbo].[Plant] ([PlantId])
GO

ALTER TABLE [dbo].[MixFormulation] CHECK CONSTRAINT [FK_Formulation_Plant]
GO

ALTER TABLE [dbo].[MixFormulation]  WITH CHECK ADD  CONSTRAINT [FK_Formulation_StandardMix] FOREIGN KEY([StandardMixId])
REFERENCES [dbo].[StandardMix] ([Id])
GO

ALTER TABLE [dbo].[MixFormulation] CHECK CONSTRAINT [FK_Formulation_StandardMix]
GO

-- Mix Constituents

CREATE TABLE [dbo].[StandardMixConstituents](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MixFormulationId] [bigint] NOT NULL,
	[RawMaterialId] [bigint] NOT NULL,
	[UomId] [bigint] NOT NULL,
	[Quantity] [float] NOT NULL,
	[PerCementWeight] [bit] NULL,
 CONSTRAINT [PK_MixConstituents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[StandardMixConstituents]  WITH CHECK ADD  CONSTRAINT [FK_StandardMixConstituents_RawMaterial] FOREIGN KEY([Id])
REFERENCES [dbo].[RawMaterial] ([Id])
GO

ALTER TABLE [dbo].[StandardMixConstituents] CHECK CONSTRAINT [FK_StandardMixConstituents_RawMaterial]
GO

ALTER TABLE [dbo].[StandardMixConstituents]  WITH CHECK ADD  CONSTRAINT [FK_StandardMixConstituents_StandardMix] FOREIGN KEY([MixFormulationId])
REFERENCES [dbo].[MixFormulation] ([Id])
GO

ALTER TABLE [dbo].[StandardMixConstituents] CHECK CONSTRAINT [FK_StandardMixConstituents_StandardMix]
GO

ALTER TABLE [dbo].[StandardMixConstituents]  WITH CHECK ADD  CONSTRAINT [FK_StandardMixConstituents_Uom] FOREIGN KEY([UomId])
REFERENCES [dbo].[Uom] ([Id])
GO

ALTER TABLE [dbo].[StandardMixConstituents] CHECK CONSTRAINT [FK_StandardMixConstituents_Uom]
GO

-- Modify District Columns

alter table District ADD [Load] float
alter table District ADD Wash float
alter table District ADD [Unload] float
alter table District ADD ToJob float
alter table District ADD [Return] float
alter table District ADD AcceptanceExpiration int
alter table District ADD QuoteExpiration int
alter table District ADD ProjectEntryFormNotification nvarchar(MAX)
alter table District ADD EmailQuoteToCustomer nvarchar(MAX)
alter table District ADD Disclaimers nvarchar(MAX)
alter table District ADD Disclosures nvarchar(MAX)
alter table District ADD TermsAndConditions nvarchar(MAX)
alter table District ADD Acceptance nvarchar(MAX)
alter table District ADD DispatchAddress nvarchar(200)
alter table District ADD DispatchCityStateZip nvarchar(200)
alter table District ADD DispatchPhone nvarchar(200)
alter table District ADD DispatchFax nvarchar(200)

alter table District ADD PriceInclude bit
alter table District ADD PriceSequence int

alter table District ADD QuantityInclude bit
alter table District ADD QuantitySequence int

alter table District ADD MixIdInclude bit
alter table District ADD MixIdSequence int

alter table District ADD DescriptionInclude bit
alter table District ADD DescriptionSequence int

alter table District ADD PsiInclude bit
alter table District ADD PsiSequence int

alter table District ADD PublicCommentsInclude bit
alter table District ADD PublicCommentsSequence int

alter table District ADD SlumpInclude bit
alter table District ADD SlumpSequence int

alter table District ADD AirInclude bit
alter table District ADD AirSequence int

-- Add District to Competitor

CREATE TABLE [dbo].[DistrictCompetitor](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DistrictId] [int] NOT NULL,
	[CompetitorId] [int] NOT NULL,
 CONSTRAINT [PK_DistrictCompetitor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DistrictCompetitor]  WITH CHECK ADD  CONSTRAINT [FK_DistrictCompetitor_Competitor] FOREIGN KEY([CompetitorId])
REFERENCES [dbo].[Competitor] ([CompetitorId])
GO

ALTER TABLE [dbo].[DistrictCompetitor] CHECK CONSTRAINT [FK_DistrictCompetitor_Competitor]
GO

ALTER TABLE [dbo].[DistrictCompetitor]  WITH CHECK ADD  CONSTRAINT [FK_DistrictCompetitor_District] FOREIGN KEY([DistrictId])
REFERENCES [dbo].[District] ([DistrictId])
GO

ALTER TABLE [dbo].[DistrictCompetitor] CHECK CONSTRAINT [FK_DistrictCompetitor_District]
GO

-- Add District to Customer

CREATE TABLE [dbo].[DistrictCustomer](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DistrictId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_DistrictCustomer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DistrictCustomer]  WITH CHECK ADD  CONSTRAINT [FK_DistrictCustomer_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO

ALTER TABLE [dbo].[DistrictCustomer] CHECK CONSTRAINT [FK_DistrictCustomer_Customer]
GO

ALTER TABLE [dbo].[DistrictCustomer]  WITH CHECK ADD  CONSTRAINT [FK_DistrictCustomer_District] FOREIGN KEY([DistrictId])
REFERENCES [dbo].[District] ([DistrictId])
GO

ALTER TABLE [dbo].[DistrictCustomer] CHECK CONSTRAINT [FK_DistrictCustomer_District]
GO

-- Tax Codes

CREATE TABLE [dbo].[TaxCode](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TaxCode] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_TaxCode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


