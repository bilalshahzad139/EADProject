SET IDENTITY_INSERT  dbo.Users ON;  
MERGE INTO dbo.Users AS Target 
USING (
	Select 1, N'Admin','admin','admin','',1,1 UNION ALL
	Select 2, N'User','user','user','',0,1 
) 
AS Source (UserID, Name, Login, Password, PictureName, IsAdmin, IsActive) 
ON Target.UserID = Source.UserID
WHEN MATCHED THEN 
UPDATE SET 
	Name = Source.Name,
	Login = Source.Login,
	Password = Source.Password,
	IsAdmin = Source.IsAdmin

WHEN NOT MATCHED BY TARGET THEN 
INSERT (UserID, Name, Login, Password, PictureName, IsAdmin, IsActive) 
VALUES (UserID, Name, Login, Password, PictureName, IsAdmin, IsActive);
--WHEN NOT MATCHED BY SOURCE THEN 
--DELETE;

SET IDENTITY_INSERT  dbo.Users OFF;  