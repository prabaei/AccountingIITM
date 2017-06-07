USE [Accounting]
GO

/****** Object:  StoredProcedure [dbo].[commitTran]    Script Date: 07-02-2017 17:21:39 ******/
DROP PROCEDURE [dbo].[commitTran]
GO

/****** Object:  StoredProcedure [dbo].[commitTran]    Script Date: 07-02-2017 17:21:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE proc [dbo].[commitTran]
@TransNo nvarchar(23),
@ProjectNo nvarchar(max),
@voucherType int,
@voucherNo nvarchar(max),
@Amount decimal(18,2),
@bankDate datetime,
@chequeno nvarchar(max),
@narration nvarchar(250),
@remarks nvarchar(250),
@currentamount decimal(18,2) out,
@avilableamount decimal(18,2) out,
@errmsg nvarchar(10) out,
@transdone bit out
as
begin
set transaction isolation level serializable
begin transaction 
begin try
	declare @inid nvarchar(4);
	declare @impamount decimal(18,2);
	declare @curbal decimal(18,2);
	DECLARE @COMMITED BIT;
	IF EXISTS(SELECT * FROM AccTransaction WHERE  TransNO=@TransNo)
		BEGIN
			IF EXISTS(SELECT * FROM AccTransaction WHERE  TransNO=@TransNo)
			SELECT @inid=INSTID,@COMMITED=TransCommited FROM AccTransaction WHERE  TransNO=@TransNo
			if(@COMMITED=0)
				begin
					IF EXISTS(SELECT * FROM ImprestMaster where InstituteId = @inid)
					BEGIN
					select @impamount = Amount from ImprestMaster where InstituteId = @inid
					set @avilableamount=@impamount
						IF(@voucherType=1)
							begin
								set @curbal=@impamount+@Amount
								set @currentamount=@curbal
								update ImprestMaster set Amount=@curbal where InstituteId = @inid
							end
						ELSE
							begin
								set @curbal=@impamount-@Amount
								set @currentamount=@curbal
								update ImprestMaster set Amount=@curbal where InstituteId = @inid
							end
						UPDATE AccTransaction set ProjectNo=@ProjectNo,voucherType=@voucherType,
						voucherNo=@voucherNo,Amount=@Amount,bankDate=@bankDate,AvailableBal=@impamount,
						currentBal=@curbal,ChequeNO=@chequeno,narration=@narration,TransUpdated=GETDATE(),
						Remarks=@remarks,TransCommited=1 where TransNO=@TransNo
						set @transdone=1
						set @errmsg ='SUCCESS'
						commit transaction
					END
					ELSE
					BEGIN
						commit transaction
						set @transdone=0
						set @errmsg ='NOACAMT'
					END
				end
			else
				begin
					commit transaction
					set @transdone=0
					set @errmsg ='ALCMT'
				end
		END
	ELSE
		BEGIN
			commit transaction
			set @transdone=0
			set @errmsg ='NOTRNO'
		END

end try
begin catch
set @transdone=0
set @errmsg ='catch'
rollback transaction
end catch
end



GO


