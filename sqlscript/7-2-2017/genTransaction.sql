USE [Accounting]
GO

/****** Object:  UserDefinedFunction [dbo].[GENERATETRANSID]    Script Date: 07-02-2017 17:21:54 ******/
DROP FUNCTION [dbo].[GENERATETRANSID]
GO

/****** Object:  UserDefinedFunction [dbo].[GENERATETRANSID]    Script Date: 07-02-2017 17:21:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create FUNCTION [dbo].[GENERATETRANSID] (@INSTID NVARCHAR(6))
RETURNS NVARCHAR(23)
AS
BEGIN
DECLARE @TRANSID NVARCHAR(23)
SET @TRANSID= RIGHT('0000' + Ltrim(Rtrim(@INSTID)), 6)+
CONVERT(NVARCHAR(4),DATEPART(YEAR, GETDATE()))+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(DAY, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(HOUR, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(minute, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(second, GETDATE())), 2)+
RIGHT('000' + CONVERT(NVARCHAR(3), DATEPART(millisecond, GETDATE())), 3)
RETURN @TRANSID
END
GO

