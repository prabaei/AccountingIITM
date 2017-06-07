
CREATE PROC [dbo].[verifyUser]
@Email nvarchar(max),
@Password varchar(30),
@ermsg nvarchar(10) out,
@valid bit out,
@role int out

AS
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
DECLARE @binpass VARBINARY(8000)
DECLARE @USRL INT
DECLARE @NREG BIT
	IF EXISTS(SELECT * FROM UserTbl WHERE Email = @Email)
		BEGIN
			SELECT @binpass=UserPassword,@USRL=UsrRole,@NREG=NewReg FROM UserTbl WHERE Email = @Email
				IF(@NREG=1)
					BEGIN
						
						SET @ermsg='RSTPAS'
						SET @valid=0
					END
				ELSE
					BEGIN
						IF(@binpass=hashbytes('MD5',@Password))
							BEGIN
								SET @ermsg='VLDUSR'
								SET @valid=1
								SET @role=@USRL
							END
						ELSE
							BEGIN
								SET @ermsg='NTVLDUS'
								SET @valid=0
							END
					END
		END
	ELSE
		BEGIN
			SET @ermsg='NCRCD'
			SET @valid=0
		END	
END


