
DECLARE	@return_value int,
		@IsCommit bit,
		@erMsg nvarchar(20)

EXEC	@return_value = [dbo].[InsertUserTbl]
		@Email = N'admin',
		@Name = N'admin',
		@Gender = 1,
		@UserPassword = N'12345678',
		@UserRol = 2,
		@IsCommit = @IsCommit OUTPUT,
		@erMsg = @erMsg OUTPUT

SELECT	@IsCommit as N'@IsCommit',
		@erMsg as N'@erMsg'

SELECT	'Return Value' = @return_value

