SELECT   GETDATE() 'Today'
select convert(varchar(10),getdate(),121)

declare @timestamp DATETIME
SET @timestamp = GETDATE()
select datename (millisecond,@timestamp) 
select CONVERT(NVARCHAR(2),DATEPART(DAY, GETDATE()))

DECLARE @day CHAR(8)
SET @day = CONVERT(NVARCHAR(4),DATEPART(YEAR, GETDATE()))+RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, GETDATE())), 2)+RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(DAY, GETDATE())), 2)


WAITFOR DELAY '00:00:00.002';
SELECT CONVERT(NVARCHAR(4),DATEPART(YEAR, GETDATE()))+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(DAY, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(HOUR, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(minute, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(second, GETDATE())), 2)+
RIGHT('000' + CONVERT(NVARCHAR(3), DATEPART(millisecond, GETDATE())), 3)
print @day

CREATE TABLE TRANSSTATUS(
INSTID NVARCHAR(4),
TRANSNO NVARCHAR(21) PRIMARY KEY DEFAULT 
CONVERT(NVARCHAR(4),DATEPART(YEAR, GETDATE()))+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(DAY, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(HOUR, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(minute, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(second, GETDATE())), 2)+
RIGHT('000' + CONVERT(NVARCHAR(3), DATEPART(millisecond, GETDATE())), 3),
DATEINP DATETIME not null default GETDATE() 
)

insert into transstatus (INSTID) values('4')
CREATE TABLE transReg
(
TRANSNO NVARCHAR(23) not null primary key,
instid nvarchar(4) not null,
working bit not null,-- 0-waiting 1-working
completed bit not null ,--0 not completed,1--completed
RequestTime datetime not null default getdate()
)

alter FUNCTION GENERATETRANSID (@INSTID NVARCHAR(6))
RETURNS NVARCHAR(23)
AS
BEGIN
DECLARE @TRANSID NVARCHAR(23)
SET @TRANSID= RIGHT('0000' + Ltrim(Rtrim(@INSTID)), 6)+
CONVERT(NVARCHAR(4),DATEPART(YEAR, GETDATE()))+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(DAY, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(HOUR, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(minute, GETDATE())), 2)+
RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(second, GETDATE())), 2)+
RIGHT('000' + CONVERT(NVARCHAR(3), DATEPART(millisecond, GETDATE())), 3)
RETURN @TRANSID
END

SELECT [dbo].[GENERATETRANSID]('8521')
GO

ALTER PROC transRegister
@instid nvarchar(6)
AS
BEGIN 
declare @transid nvarchar(23)
SET @transid = [dbo].[GENERATETRANSID](@instid)
set transaction isolation level serializable
begin transaction 
if exists(SELECT * FROM transReg where transno = @transid)
print('transaction no exist')
else
	begin
		if exists(SELECT * FROM transReg where instid = @instid and completed = 0 )
		--INSERT INTO transstatus (TRANSNO,instid,working,completed) VALUES(@transid,@instid,0,0)
		print('transpending-- trigger timeout')
		else
		begin
			INSERT INTO transReg (TRANSNO,instid,working,completed) VALUES(@transid,@instid,0,0)
			print('success')
		end
	end
commit transaction
END

EXEC TRANSREGISTER @instid='8521'


create table transTimeout(
transno nvarchar(23),
instid nvarchar(6) not null,
settimeout bit not null
)
