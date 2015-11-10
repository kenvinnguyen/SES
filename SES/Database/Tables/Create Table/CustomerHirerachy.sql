

/****** Object:  Table [dbo].[Customer]    Script Date: 11/8/2015 9:21:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF OBJECT_ID('CustomerHirerachy', 'U') IS NOT NULL
  DROP TABLE dbo.CustomerHirerachy; 

CREATE TABLE [dbo].[CustomerHirerachy](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerHirerachyID] [nvarchar](20) NOT NULL,
	[CustomerHirerachyName] [nvarchar](200) NOT NULL,
	[ParentCustomerHirerachyID] [nvarchar](20) NOT NULL,
	[CustomerHirerachyIndex] [nvarchar](200) NOT NULL,
	[Status] bit,
	[CreatedAt] [datetime]  NULL,
	[CreatedBy] [varchar](32)  NULL,
	[UpdatedAt] [datetime]  NULL,
	[UpdatedBy] [varchar](32)  NULL,
 CONSTRAINT [PK_CustomerHirerachy] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CustomerHirerachyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


