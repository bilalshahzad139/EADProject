CREATE TABLE [dbo].[ProductPictureNames]
(
	[PictureID] INT NOT NULL IDENTITY , 
    [ProductID] INT NOT NULL, 
    [PictureName] VARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_ProductPictureNames_Products] FOREIGN KEY ([ProductID]) REFERENCES [Products]([ProductID]), 
    CONSTRAINT [PK_ProductPictureNames] PRIMARY KEY ([PictureID])
)
