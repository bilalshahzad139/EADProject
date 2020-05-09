CREATE TABLE [dbo].[category]
(
	[CategoryID] INT NOT NULL PRIMARY KEY, 
    [ParentCategoryID] INT NOT NULL, 
    [CategoryName] NCHAR(10) NOT NULL
)
