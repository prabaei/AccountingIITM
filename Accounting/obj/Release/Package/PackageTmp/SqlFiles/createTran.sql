
CREATE proc [dbo].[createTran]
@instid NVARCHAR(4),
@UserEmail NVARCHAR(80),
@BankAccountNO NVARCHAR(14),
@TransactionNo NVARCHAR(23) OUT,
@isFailed BIT OUT,
@ermsg NVARCHAR(10) OUT
AS
BEGIN
DECLARE @dd DATETIME
SELECT @dd= DATEADD(MINUTE,-2,GETDATE())
--select @dd
DECLARE @genid NVARCHAR(23)
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE 
BEGIN TRANSACTION 
	IF EXISTS(SELECT * FROM ImprestMaster WHERE InstituteId =@instid)
	BEGIN		
		SELECT @genid= [dbo].[GENERATETRANSID](@instid)
		IF EXISTS(SELECT * FROM AccTransaction WHERE TransCreated > @dd and TransNO=@genid)
			BEGIN
				SET @isFailed=1
				SET @ermsg='duplicate'
				COMMIT TRANSACTION
			END
		ELSE
			BEGIN
				BEGIN TRY
					---print('ready to register')
					INSERT INTO AccTransaction (TransNO,TransCreated,INSTID,TransCommited,UserEmail,brsDone,CNS,RecupDone,BankAccountNO)VALUES (@genid,GETDATE(),@instid,0,@UserEmail,0,0,0,@BankAccountNO)
					SET @isFailed=0
					SET @ermsg='success'
					SET @TransactionNo=@genid
					COMMIT TRANSACTION
				END TRY
				BEGIN CATCH
					SET @isFailed=1
					SET @ermsg='catch'
					ROLLBACK TRANSACTION
				END CATCH
			END
		END
	
	ELSE
	BEGIN
		SET @isFailed=1
		SET @ermsg='noACC'
		COMMIT TRANSACTION
	END
	END







