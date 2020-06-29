CREATE TABLE [dbo].[LikesDislikes]
(
	[UserID] INT NOT NULL , 
    [ProductID] INT NOT NULL, 
    [Likes] INT NOT NULL DEFAULT 0, 
    [Dislikes] INT NOT NULL DEFAULT 0, 
    PRIMARY KEY ([ProductID],[UserID]),
    CONSTRAINT [FK_UserID]  FOREIGN KEY ([UserID]) REFERENCES Users([UserID]),
    CONSTRAINT [FK_ProductID]  FOREIGN KEY ([ProductID]) REFERENCES Products([ProductID]),
)
