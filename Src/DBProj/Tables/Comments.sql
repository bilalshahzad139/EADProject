CREATE TABLE [dbo].[Comments] (
    [CommentId]   INT           IDENTITY (1, 1) NOT NULL,
    [UserID]      INT           NOT NULL,
    [ProductID]   INT           NOT NULL,
    [CommentText] VARCHAR (100) NOT NULL,
    [CommentOn]   DATETIME      NOT NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED ([CommentId] ASC)
);

