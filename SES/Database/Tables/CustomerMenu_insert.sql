USE [SES]
GO

INSERT INTO [dbo].[Auth_Menu]
           ([MenuID],[ParentMenuID],[MenuName],[MenuIndex],[ControllerName],[IsVisible],[RowCreatedAt],[RowCreatedBy],[RowUpdatedAt],[RowUpdatedBy])
     VALUES(
           'Customer','','Khách hàng',5,'Customer',1,GETDATE(),'system',GETDATE(),''
		   )
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedAt) values('1','Customer','Delete','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedAt) values('1','Customer','Export','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedAt) values('1','Customer','Import','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedAt) values('1','Customer','Insert','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedAt) values('1','Customer','Update','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedAt) values('1','Customer','View','1',getdate(),'administrator')






