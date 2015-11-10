
INSERT INTO [dbo].[Auth_Menu]
           ([MenuID],[ParentMenuID],[MenuName],[MenuIndex],[ControllerName],[IsVisible],[RowCreatedAt],[RowCreatedBy],[RowUpdatedAt],[RowUpdatedBy])
     VALUES(
           'Promotion','',N'Chương trình khuyến mãi',7,'Promotion',1,GETDATE(),'system',GETDATE(),''
		   )
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Promotion','Delete','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Promotion','Export','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Promotion','Import','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Promotion','Insert','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Promotion','Update','1',getdate(),'administrator')
insert into Auth_Action(RoleID,MenuID,Action,IsAllowed,RowCreatedAt,RowCreatedBy) values('1','Promotion','View','1',getdate(),'administrator')






