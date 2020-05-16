Create Procedure [dbo].[SP_Product_SearchByName]
@product_name varchar(50),
@minPrice  decimal(18,2),
@maxPrice  decimal(18, 2),
@categoryId int
As
begin
		if(@categoryId=0 AND @maxPrice=0)
		begin
			
			Select 
				tbl1.ProductID,
				Name,
				Price,
				ProductCategoryID,
				IsActive,
				CreatedOn,
				CreatedBy,
				ModifiedOn,
				ModifiedBy,
				tbl2.PictureName
			from dbo.Products tbl1
			JOIN dbo.ProductPictureNames tbl2
			on tbl1.ProductID=tbl2.ProductID
			where CHARINDEX(@product_name,Name)>0
			AND Price>= @minPrice;
		end
		else if(@categoryId=0)
		begin
			Select 
				tbl1.ProductID,
				Name,
				Price,
				ProductCategoryID,
				IsActive,
				CreatedOn,
				CreatedBy,
				ModifiedOn,
				ModifiedBy,
				tbl2.PictureName
			from dbo.Products tbl1
			JOIN dbo.ProductPictureNames tbl2
			on tbl1.ProductID=tbl2.ProductID
			where CHARINDEX(@product_name,Name)>0
			AND Price between @minPrice AND @maxPrice
		end
		else if(@maxPrice=0) 
		begin
			Select 
				tbl1.ProductID,
				Name,
				Price,
				ProductCategoryID,
				IsActive,
				CreatedOn,
				CreatedBy,
				ModifiedOn,
				ModifiedBy,
				tbl2.PictureName
			from dbo.Products tbl1
			JOIN dbo.ProductPictureNames tbl2
			on tbl1.ProductID=tbl2.ProductID
			where CHARINDEX(@product_name,Name)>0
			AND Price>= @minPrice 	
			AND ProductCategoryID=@categoryId
		end
end