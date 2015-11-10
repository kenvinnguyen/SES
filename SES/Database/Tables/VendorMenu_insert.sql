
INSERT INTO [dbo].[Auth_Menu]
           ([MenuID],[ParentMenuID],[MenuName],[MenuIndex],[ControllerName],[IsVisible],[RowCreatedAt],[RowCreatedBy],[RowUpdatedAt],[RowUpdatedBy])
     VALUES(
           'Vendor','',N'Nhà cung cấp',6,'Vendor',1,GETDATE(),'system',GETDATE(),''
		   )
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Vendor','Delete','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Vendor','Export','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Vendor','Import','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Vendor','Insert','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Vendor','Update','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Vendor','View','1',getdate(),'administrator')






