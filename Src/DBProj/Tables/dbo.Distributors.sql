CREATE TABLE [dbo].[Distributors]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Country] VARCHAR(50) NOT NULL, 
    [Address] VARCHAR(MAX) NOT NULL, 
    [Phone] VARCHAR(50) NOT NULL, 
    [Website] VARCHAR(50) NULL
);
