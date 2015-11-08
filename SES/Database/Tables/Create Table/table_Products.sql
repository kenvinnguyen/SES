USE [DecaInsightDev]
GO

/****** Object:  Table [dbo].[DC_AD_Items]    Script Date: 11/7/2015 12:46:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Products](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](15) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Desc] [nvarchar](500) NOT NULL,
	[Size] [varchar](50) NOT NULL,
	[Price] [float] NOT NULL,
	[VATPrice] [float] NOT NULL,
	[Unit] [varchar](50) NULL,
	[Type] [nvarchar](400)  NULL,
	[WHID] [varchar](50)  NULL,
	[WHLID] [varchar](50)  NULL,
	[ShapeTemplate] [varchar](30)  NULL,
	[GroupID] [varchar](30)  NULL,
	[BrandID] [numeric] (10) NULL ,
	[Status] [bit] NOT NULL,
	[CreatedAt] [datetime]  NULL,
	[CreatedBy] [varchar](32)  NULL,
	[UpdatedAt] [datetime]  NULL,
	[UpdatedBy] [varchar](32) NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


