CREATE PROCEDURE [dbo].[InsertProductPictureName]
	@ProductID int,
	@PictureName varchar(50)
AS
	INSERT INTO ProductPictureNames(ProductID, PictureName)
VALUES (@ProductID, @PictureName)

return
