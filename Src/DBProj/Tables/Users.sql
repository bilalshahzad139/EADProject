CREATE TABLE [dbo].[Users] (
    [UserID]      INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NOT NULL,
    [Login]       VARCHAR (50) NOT NULL,
    [Password]    VARCHAR (50) NOT NULL,
    [PictureName] VARCHAR (50) NOT NULL,
    [IsAdmin]     BIT          NOT NULL,
    [IsActive]    BIT          NOT NULL,
    [PwdRecoveryCode] VARCHAR(50) NULL, 
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserID] ASC)
);

