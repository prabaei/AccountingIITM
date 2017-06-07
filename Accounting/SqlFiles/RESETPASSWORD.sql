
CREATE PROC [dbo].[RESETPASSWORD]
@Email NVARCHAR(80),
@Password VARCHAR(MAX),
@Updatedone BIT OUT,
@Errmsg NVARCHAR(10) OUT
AS
BEGIN
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
BEGIN TRAN
	BEGIN TRY
		IF exists( SELECT * FROM UserTbl WHERE Email = @Email)
			BEGIN
				UPDATE UserTbl SET UserPassword=HASHBYTES('MD5',LTRIM(RTRIM(@Password))),NewReg=0 where Email = @Email
				SET @Updatedone = 1
				SET @Errmsg= 'DONE'
				COMMIT TRAN
			END
		ELSE
			BEGIN
				SET @Updatedone = 0
				SET @Errmsg= 'NOTEXIST'
				ROLLBACK TRAN
			END
	END TRY
	BEGIN CATCH
		SET @Updatedone = 0
		SET @Errmsg= 'CATCH'
		ROLLBACK TRAN
	END CATCH
END

