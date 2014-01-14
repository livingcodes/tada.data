USE [blog]
GO

CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[password] [varchar](20) NOT NULL,
   [age_in_years] int NULL
) ON [PRIMARY]

GO
