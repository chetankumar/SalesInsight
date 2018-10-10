CREATE TABLE [dbo].[FSKPrice] (
    [Id]          BIGINT          IDENTITY (1, 1) NOT NULL,
    [FSKCode]     NVARCHAR (50)   NOT NULL,
    [City]        VARCHAR (200)   NOT NULL,
    [BasePrice]   DECIMAL (18, 2) NOT NULL,
    [AddPrice]    DECIMAL (18, 2) NOT NULL,
    [DeductPrice] DECIMAL (18, 2) NOT NULL,
    [Active]      BIT             NOT NULL,
    CONSTRAINT [PK_FSKPrice] PRIMARY KEY CLUSTERED ([Id] ASC)
);

ALTER TABLE [dbo].[RoleAccess]
    ADD [Enable5skPricing] BIT NULL;

ALTER TABLE [dbo].[Plant]
    ADD [FSKId] BIGINT NULL;

ALTER TABLE [dbo].[Quotation]
     ADD [FskPriceId]  BIGINT NULL;

ALTER TABLE [dbo].[Quotation]
	 ADD CONSTRAINT [FK_Quotation_FSKPrice] FOREIGN KEY ([FskPriceId]) REFERENCES [dbo].[FSKPrice] ([Id]);

ALTER TABLE [dbo].[RawMaterial]
	ADD [FSKMarkup]  FLOAT (53)    NULL,
    [FSKCode]        NVARCHAR (50) NULL;

ALTER TABLE [dbo].[RawMaterialType]
	ADD [IncludeInFSK]   BIT           NULL;

ALTER TABLE [dbo].[Plant]
	ADD CONSTRAINT [FK_Plant_FSKPrice] FOREIGN KEY ([FSKId]) REFERENCES [dbo].[FSKPrice] ([Id])

