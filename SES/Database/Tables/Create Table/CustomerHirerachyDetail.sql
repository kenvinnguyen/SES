

/****** Object:  Table [dbo].[CustomerHirerachy]    Script Date: 11/9/2015 10:10:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CustomerHirerachyDetail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] nvarchar(15) Not Null,
	[CustomerHirerachyID] [nvarchar](20) NOT NULL,
	[Status] [bit] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [varchar](32) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [varchar](32) NULL,
 CONSTRAINT [PK_CustomerHirerachyDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CustomerID] ASC,
	[CustomerHirerachyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


