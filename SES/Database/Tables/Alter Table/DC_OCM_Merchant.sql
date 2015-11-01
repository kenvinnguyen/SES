

ALTER TABLE DC_OCM_Merchant
ADD Status [bit] NOT NULL DEFAULT(1)
GO
alter table [DC_OCM_Merchant]
ADD [ID] INT IDENTITY(1,1) NOT NULL
Go 
alter table [DC_OCM_Merchant]
DROP COLUMN [PKMerchantID] 
Go
alter table [DC_OCM_Merchant]
DROP COLUMN [FKProvince] 
Go
