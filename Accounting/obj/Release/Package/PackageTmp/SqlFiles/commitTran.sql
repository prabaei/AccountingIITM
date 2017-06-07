CREATE PROC [dbo].[commitTran]
@TransNo NVARCHAR(23),
@ProjectNo NVARCHAR(max),
@voucherType INT,
@voucherNo NVARCHAR(max),
@Amount DECIMAL(18,2),
@bankDate DATETIME,
@chequeno NVARCHAR(max),
@narration NVARCHAR(250),
@remarks NVARCHAR(250),
@CommitmentNO NVARCHAR(MAX),
@BankAccountNO NVARCHAR(14),
@ProjectType int,
@head nvarchar(100),
@desc nvarchar(250),
--@UserEmail NVARCHAR(80),
@currentamount DECIMAL(18,2) OUT,
@avilableamount DECIMAL(18,2) OUT,
@errmsg NVARCHAR(10) OUT,
@transdone BIT OUT
AS
BEGIN
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
BEGIN TRANSACTION 
BEGIN TRY
	DECLARE @inid NVARCHAR(4);
	DECLARE @impamount DECIMAL(18,2);
	DECLARE @curbal DECIMAL(18,2);
	DECLARE @COMMITED BIT;
	IF EXISTS(SELECT * FROM AccTransaction WHERE  TransNO=@TransNo)
		BEGIN
			IF EXISTS(SELECT * FROM AccTransaction WHERE  TransNO=@TransNo)
			SELECT @inid=INSTID,@COMMITED=TransCommited FROM AccTransaction WHERE  TransNO=@TransNo
			IF(@COMMITED=0)
				BEGIN
					IF EXISTS(SELECT * FROM ImprestMaster WHERE InstituteId = @inid)
					BEGIN
					---comment start here
						SELECT @impamount = Amount FROM ImprestMaster WHERE InstituteId = @inid
						SET @avilableamount=@impamount
						IF(@voucherType=1)
							BEGIN
								SET @curbal=@impamount+@Amount
								SET @currentamount=@curbal
								--UPDATE ImprestMaster SET Amount=@curbal WHERE InstituteId = @inid
							END
						ELSE IF(@voucherType=8)
							BEGIN
								SET @curbal=@impamount+@Amount
								SET @currentamount=@curbal
								--UPDATE ImprestMaster SET Amount=@curbal WHERE InstituteId = @inid
							END
						ELSE
							BEGIN
								set @curbal=@impamount-@Amount
								SET @currentamount=@curbal
								--UPDATE ImprestMaster SET Amount=@curbal WHERE InstituteId = @inid
							END
					--comment ends here
						UPDATE AccTransaction SET ProjectNo=@ProjectNo,voucherType=@voucherType,
						voucherNo=@voucherNo,Amount=@Amount,bankDate=@bankDate,AvailableBal=null,--@avilableamount,
						currentBal=null,--@currentamount,
						ChequeNO=@chequeno,narration=@narration,TransUpdated=GETDATE(),
						Remarks=@remarks,TransCommited=1,Head=@head,[Description]=@desc,CommitmentNO=@CommitmentNO,BankAccountNO=@BankAccountNO,ProjectType=@ProjectType WHERE TransNO=@TransNo
						SET @transdone=1
						COMMIT TRANSACTION
					END
					ELSE
					BEGIN
						COMMIT TRANSACTION
						SET @transdone=0
						SET @errmsg ='NOACAMT'
					END
				END
			ELSE
				BEGIN
					COMMIT TRANSACTION
					SET @transdone=0
					SET @errmsg ='ALCMT'
				END
		END
	ELSE
		BEGIN
			COMMIT TRANSACTION
			SET @transdone=0
			SET @errmsg ='NOTRNO'
		END
END TRY
BEGIN CATCH
SET @transdone=0
SET @errmsg ='catch'
ROLLBACK TRANSACTION
END CATCH
END