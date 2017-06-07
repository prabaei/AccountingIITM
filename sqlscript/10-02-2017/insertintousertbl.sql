USE [Accounting]
GO

/****** Object:  StoredProcedure [dbo].[InsertUserTbl]    Script Date: 10-02-2017 17:32:58 ******/
DROP PROCEDURE [dbo].[InsertUserTbl]
GO

/****** Object:  StoredProcedure [dbo].[InsertUserTbl]    Script Date: 10-02-2017 17:32:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE proc [dbo].[InsertUserTbl]
	   @Email nvarchar(max)
      ,@Name nvarchar(max)
      ,@Gender int
      ,@UserPassword varchar(max)
	  ,@IsCommit bit out
	  ,@erMsg nvarchar(20) out
  as
  begin
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
  BEGIN TRAN
	  BEGIN TRY
		IF exists( SELECT * FROM UserTbl WHERE Email = @Email)
			begin
				set @IsCommit=0
				set @erMsg='EmAlEx'
				COMMIT TRAN
			end
		else
			begin
				insert into UserTbl values(@Email,@Name,@Gender,HASHBYTES('MD5',LTRIM(RTRIM(@UserPassword))))
				set @IsCommit=1
				set @erMsg='success'
				COMMIT TRAN
			end
		
	  END TRY
	  BEGIN CATCH
			set @IsCommit=0
			set @erMsg='catch'
			ROLLBACK TRAN
	  END CATCH
  end


GO


