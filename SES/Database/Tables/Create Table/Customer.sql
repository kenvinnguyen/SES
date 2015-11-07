USE [SES]
GO
IF OBJECT_ID('Customer', 'U') IS NOT NULL
  DROP TABLE dbo.Customer; 

CREATE TABLE [dbo].[Customer](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [nvarchar](15) NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[Shoptype] [nvarchar](200) NOT NULL,
	[Agent] [nvarchar](200) NOT NULL,
	[Address] [nvarchar](200) NOT NULL,
	[Email] [nvarchar](200) NOT NULL,
	[Phone] [nvarchar](200) NOT NULL,
	[Fax] [nvarchar](200) NOT NULL,
	[Birthday] varchar(20) NOT NULL,
	[ProvinceID] varchar(15) NOT NULL,
	[DistrictID] varchar(15) NOT NULL,
	[Gender] [nvarchar](4) NOT NULL,
	[LevelHirerachy1] [nvarchar](200) NOT NULL,
	[LevelHirerachy2] [nvarchar](200) NOT NULL,
	[LevelHirerachy3] [nvarchar](200) NOT NULL,
	[LevelHirerachy4] [nvarchar](200) NOT NULL,
	[Desc] [nvarchar](200) NOT NULL,
	[Status] [bit] NOT NULL,
	[Note] [nvarchar](512) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](32) NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[UpdatedBy] [varchar](32) NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


