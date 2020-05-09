CREATE PROCEDURE [dbo].[InsertPictureName]
	@ProductID int,
	@PictureName varchar(50)
AS
	INSERT INTO PictureNames(ProductID, PictureName)
VALUES (@ProductID, @PictureName)

return
