USE [FACCT]
GO

/****** Object:  Table [dbo].[CO_NME]    Script Date: 06-01-2017 17:06:20 ******/
DROP TABLE [dbo].[CO_NME]
GO

/****** Object:  Table [dbo].[CO_NME]    Script Date: 06-01-2017 17:06:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CO_NME](
	[CODE] [nvarchar](4) NULL,
	[NAME] [nvarchar](25) NULL,
	[IIRNO] [nvarchar](4) NOT NULL,
	[DESIG] [nvarchar](20) NULL,
	[DEPT] [nvarchar](3) NULL,
	[LAB] [nvarchar](20) NULL,
	[REM] [nvarchar](5) NULL,
	[CHECK] [nvarchar](1) NULL
) ON [PRIMARY]

GO


