Create Procedure [dbo].[SP_Product_SearchByName]
@product_name varchar(50),
@minPrice  decimal(18,2),
@maxPrice  decimal(18, 2),
@categoryId int
As
begin
		if(@categoryId=0 AND @maxPrice=0)
		begin
			
			Select *from dbo.Products
			where CHARINDEX(@product_name,Name)>0
			AND Price>= @minPrice;
		end
		else if(@categoryId=0)
		begin
			Select * from dbo.Products
			where CHARINDEX(@product_name,Name)>0
			AND Price between @minPrice AND @maxPrice
		end
		else if(@maxPrice=0) 
		begin
			Select * from dbo.Products
			where CHARINDEX(@product_name,Name)>0
			AND Price>= @minPrice 	
			AND ProductCategoryID=@categoryId
		end
end