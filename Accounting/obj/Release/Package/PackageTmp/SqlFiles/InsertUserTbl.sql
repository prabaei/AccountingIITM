

CREATE proc [dbo].[InsertUserTbl]
	   @Email nvarchar(max)
      ,@Name nvarchar(max)
      ,@Gender int
      ,@UserPassword varchar(max)
	  ,@UserRol int
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
				declare @tempId int
				insert into UserTbl values(@Email,@Name,@Gender,HASHBYTES('MD5',LTRIM(RTRIM(@UserPassword))),@UserRol,1)
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
