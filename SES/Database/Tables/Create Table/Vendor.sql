IF OBJECT_ID('Vendor', 'U') IS NOT NULL
  DROP TABLE dbo.Vendor; 

CREATE TABLE [dbo].[Vendor](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[VendorID] [nvarchar](15) NOT NULL,
	[VendorName] [nvarchar](300) NOT NULL,
	[FullName] [nvarchar](300) NOT NULL,
	[Phone] [varchar](100) NOT NULL,
	[Fax] [varchar](100) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Address] [nvarchar](300) NOT NULL,
	[TaxCode] [nvarchar](30) NOT NULL,
	[ProvinceID] [nvarchar](10) NOT NULL,
	[Website] [varchar](300) NOT NULL,
	[SignOffDate] [datetime] NOT NULL,
	[Url] [varchar](300) NOT NULL,
	[Hotline] [varchar](100) NOT NULL,
	[Note] [nvarchar](4000) NOT NULL,
	[Status] [bit] NOT NULL DEFAULT ((1)),
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[UpdatedBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CM_VendorInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


