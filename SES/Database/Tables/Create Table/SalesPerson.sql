

CREATE TABLE [dbo].[SalesPerson](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyID] [varchar](15) NOT NULL,
	[SalesPersonID] [nvarchar](40) NOT NULL,
	[SalesPersonName] [nvarchar](255) NOT NULL,
	[Gender] [bit] NOT NULL  DEFAULT (1),
	[Description] [nvarchar](500) NULL,
	[DateOfBirth] [datetime] NULL,
	[IdentityCard] [nvarchar](40) NULL,
	[IdentityCardPlace] [nvarchar](120) NULL,
	[IdentityCardDate] [datetime] NULL,
	[IdentityCardAddress] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[Phone] [nvarchar](80) NULL,
	[Address] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [varchar](32) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [varchar](32) NULL,
	[Status] [bit] NOT NULL  DEFAULT (1),
 CONSTRAINT [PK_dbo.SalesPerson] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyID] ASC,
	[SalesPersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


