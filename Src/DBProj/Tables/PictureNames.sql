CREATE TABLE [dbo].[PictureNames]
(
	[PictureID] INT NOT NULL IDENTITY , 
    [ProductID] INT NOT NULL, 
    [PictureName] VARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_PictureNames_Products] FOREIGN KEY ([ProductID]) REFERENCES [Products]([ProductID]), 
    CONSTRAINT [PK_PictureNames] PRIMARY KEY ([PictureID])
)
