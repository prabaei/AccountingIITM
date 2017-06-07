USE [Accounting]
GO

/****** Object:  StoredProcedure [dbo].[createTran]    Script Date: 07-02-2017 17:21:45 ******/
DROP PROCEDURE [dbo].[createTran]
GO

/****** Object:  StoredProcedure [dbo].[createTran]    Script Date: 07-02-2017 17:21:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE proc [dbo].[createTran]
@instid nvarchar(4),
@TransactionNo nvarchar(23) out,
@isFailed bit out,
@ermsg nvarchar(10) out
as
begin
declare @dd datetime
select @dd= Dateadd(MINUTE,-2,getdate())
--select @dd
declare @genid nvarchar(23)
set transaction isolation level serializable
begin transaction 
	if exists(select * from ImprestMaster where InstituteId =@instid)
	begin		
		select @genid= [dbo].[GENERATETRANSID](@instid)
		if exists(select * from AccTransaction where TransCreated > @dd and TransNO=@genid)
			begin
				set @isFailed=1
				set @ermsg='duplicate'
				commit transaction
			end
		else
			begin
				begin try
					---print('ready to register')
					insert into AccTransaction (TransNO,TransCreated,INSTID,TransCommited)values (@genid,GETDATE(),@instid,0)
					set @isFailed=0
					set @ermsg='success'
					set @TransactionNo=@genid
					commit transaction
				end try
				begin catch
					set @isFailed=1
					set @ermsg='catch'
					rollback transaction
				end catch
			end
		end
	
	else
	begin
		set @isFailed=1
		set @ermsg='noACC'
		commit transaction
	end
	end


GO


