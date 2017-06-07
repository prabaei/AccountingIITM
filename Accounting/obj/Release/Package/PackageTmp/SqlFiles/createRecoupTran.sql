
CREATE proc [dbo].[createRecoupTran]
@accno NVARCHAR(14),
@UserEmail NVARCHAR(80),
@recoupid NVARCHAR(33) OUT,
@isFailed BIT OUT,
@ermsg NVARCHAR(10) OUT
AS
BEGIN
DECLARE @dd DATETIME
SELECT @dd= DATEADD(MINUTE,-2,GETDATE())
--select @dd
DECLARE @genid NVARCHAR(33)
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE 
BEGIN TRANSACTION 
	--IF EXISTS(SELECT * FROM RecoupRecord WHERE InstituteId =@instid)
	--BEGIN		
		SELECT @genid= [dbo].[GENERATERECOUPID](@accno)
		IF EXISTS(SELECT * FROM RecoupRecord WHERE Created > @dd and RecoupId=@genid)
			BEGIN
				SET @isFailed=1
				SET @ermsg='duplicate'
				COMMIT TRANSACTION
			END
		ELSE
			BEGIN
				BEGIN TRY
					---print('ready to register')
					INSERT INTO RecoupRecord (RecoupId,AccountNo,Created,UserID,Deleted)VALUES (@genid,@accno,GETDATE(),@UserEmail,0)
					SET @isFailed=0
					SET @ermsg='success'
					SET @recoupid=@genid
					COMMIT TRANSACTION
				END TRY
				BEGIN CATCH
					SET @isFailed=1
					SET @ermsg='catch'
					ROLLBACK TRANSACTION
				END CATCH
			END
		END
