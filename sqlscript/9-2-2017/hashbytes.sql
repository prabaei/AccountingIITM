/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [Email]
      ,[Name]
      ,[Gender]
      ,[Password]
  FROM [Accounting].[dbo].[UserTbl] where [password]=hashbytes('md5','admin')

  insert into UserTbl values ('prabaei','praba',1,hashbytes('md5','admin'))
  insert into UserTbl values ('arabaei','praba',1,hashbytes('md5','admin'))


  create proc InsertUserTbl
Email nvarchar(max)
      ,Name nvarchar(max)
      ,Gender int
      ,Password varbinary(max)
  as
  begin

  end