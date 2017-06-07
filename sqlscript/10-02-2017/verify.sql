USE [Accounting]
GO

/****** Object:  StoredProcedure [dbo].[verifyUser]    Script Date: 10-02-2017 17:32:16 ******/
DROP PROCEDURE [dbo].[verifyUser]
GO

/****** Object:  StoredProcedure [dbo].[verifyUser]    Script Date: 10-02-2017 17:32:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[verifyUser]
@Email nvarchar(max),
@Password varchar(30),
@ermsg nvarchar(10) out,
@valid bit out
AS
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
DECLARE @binpass VARBINARY(8000)
	IF EXISTS(SELECT * FROM UserTbl WHERE [Email] = @Email)
		BEGIN
			SELECT @binpass=UserPassword FROM UserTbl WHERE [Email] = @Email
				IF(@binpass=hashbytes('MD5',@Password))
					BEGIN
						SET @ermsg='VLDUSR'
						SET @valid=1
					END
				ELSE
					BEGIN
						SET @ermsg='NTVLDUS'
						SET @valid=0
					END
		END
	ELSE
		BEGIN
			SET @ermsg='NCRCD'
			SET @valid=0
		END	
END
GO


