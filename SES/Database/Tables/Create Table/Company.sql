

CREATE TABLE [dbo].[Company](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyID] [varchar](15) NOT NULL,
	[CompanyName] [nvarchar](250) NOT NULL,
	[ShortName] [nvarchar](50) NOT NULL,
	[EnglishName] [nvarchar](300) NOT NULL,
	[SubDomain] [varchar](300) NOT NULL,
	[Phone] [varchar](100) NOT NULL,
	[MobilePhone] [varchar](100) NOT NULL,
	[Fax] [varchar](100) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[PersonalEmail] [varchar](100) NOT NULL,
	[Address] [nvarchar](300) NOT NULL,
	[Website] [varchar](300) NOT NULL,
	[Descr] [nvarchar](4000) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [varchar](32) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [varchar](32) NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Company] ADD  DEFAULT ((1)) FOR [Status]
GO

