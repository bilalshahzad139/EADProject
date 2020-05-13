CREATE TABLE [dbo].[feedback]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [name] VARCHAR(30) NOT NULL, 
    [email] VARCHAR(30) NOT NULL, 
    [pNo] CHAR(11) NOT NULL,
    [message] VARCHAR(1000) NOT NULL
    CONSTRAINT [PK_feedback] PRIMARY KEY CLUSTERED ([Id] ASC), 
);
