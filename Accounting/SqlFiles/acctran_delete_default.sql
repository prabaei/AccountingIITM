

ALTER TABLE AccTransaction ADD CONSTRAINT DF_Deleted DEFAULT 0 FOR deleted
ALTER TABLE AccTransaction ADD CONSTRAINT DF_RecupDone DEFAULT 0 FOR RecupDone
ALTER TABLE AccTransaction ADD CONSTRAINT DF_orderId DEFAULT 1 FOR orderId
ALTER TABLE AccTransaction ADD CONSTRAINT DF_SupplierId DEFAULT 1 FOR supplier
ALTER TABLE AccTransaction ADD CONSTRAINT DF_Mailsent DEFAULT 0 FOR MailSent
ALTER TABLE AccTransaction ADD CONSTRAINT DF_MailDelivered DEFAULT 0 FOR MailDelivered
  ALTER TABLE AccTransaction ADD CONSTRAINT DF_migrated DEFAULT 0 FOR migrated
insert into  [SupplierMstr]([Name],[Address1],[Amount],[Country],[State],[projectType]) values(' ',' ',0,77,' ',' ')


--insert into ImprestMVC.dbo.SupplierMstr (Name,Address1,[State],Country,projectType,Amount) (select distinct Name as Name,'' as Address1,' ' as [State],77 as Country, 0 as projectType,0 as Amount from [ICSRDBTALLY].[dbo].[Ledgers] where Parent like '%Sundry Creditors%'  )



