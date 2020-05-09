CREATE TABLE [dbo].[category]
(
	[categoryID] INT NOT NULL PRIMARY KEY, 
    [parentCategoryID] INT NOT NULL, 
    [categoryName] NCHAR(10) NOT NULL
)
