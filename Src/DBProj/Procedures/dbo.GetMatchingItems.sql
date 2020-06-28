CREATE PROCEDURE [dbo].[GetMatchingItems]
	@term varchar(50)
AS
	SELECT Name 
	FROM [dbo].[Products]
	WHERE Name LIKE @term +'%'
RETURN 0
