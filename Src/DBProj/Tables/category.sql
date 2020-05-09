CREATE TABLE [dbo].[category]
(
	[CategoryID] INT NOT NULL PRIMARY KEY, 
    [ParentCategoryID] INT NOT NULL, 
    [CategoryName] NCHAR(50) NOT NULL, 
    [Modifiedby] INT NOT NULL, 
    [Modifiedon] DATE NOT NULL, 
    [Createdby] INT NOT NULL, 
    [Createdon] DATE NOT NULL
)
