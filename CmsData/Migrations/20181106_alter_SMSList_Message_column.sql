ALTER TABLE dbo.SMSList
	DROP CONSTRAINT DF_SMSList1_slMessage
GO
ALTER TABLE dbo.SMSList
	ALTER COLUMN Message VARCHAR (1600) NOT NULL
GO
ALTER TABLE dbo.SMSList ADD CONSTRAINT
	DF_SMSList1_slMessage DEFAULT ('') FOR Message
GO