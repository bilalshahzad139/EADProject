CREATE TABLE [dbo].[EmailVerifyingCodes]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [email] VARCHAR(50) NOT NULL, 
    [verification_code] VARCHAR(50) NOT NULL, 
    [expired] BIT NOT NULL
)
