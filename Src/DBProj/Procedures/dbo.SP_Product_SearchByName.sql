CREATE Procedure [dbo].[SP_Product_SearchByName]
@product_name varchar(50),
@minPrice  decimal(18,2),
@maxPrice  decimal(18, 2),
@categoryId int
As
DECLARE @priceConstraint varchar(max)='';
DECLARE @nameConstraint varchar(max)='';
DECLARE @categoryConstraint varchar(max)='';
DECLARE @query varchar(max)='';
begin
	if(@product_name!='')
	begin
		 SET @nameConstraint=' CHARINDEX (''' + @product_name + ''',Name)>0 '; 
	end
	if(@categoryId!=0)
	begin
		if(@product_name!='')
		begin
			SET @categoryConstraint=' AND '; 
		end
		SET @categoryConstraint=@categoryConstraint+' ProductCategoryID= '+CAST(@categoryId as varchar);
	end
	if(@maxPrice!=0)
	begin
		if(@product_name!='' OR @categoryConstraint!='')
		begin
			SET @priceConstraint=' AND '; 
		end
		SET @priceConstraint=@priceConstraint+' Price between '+CAST( @minPrice as varchar)+' AND '+CAST(@maxPrice as varchar);
	end
	else if(@maxPrice=0)
	begin
	if(@product_name!='' OR @categoryConstraint!='')
		begin
			SET @priceConstraint=' AND '; 
		end
		SET @priceConstraint=@priceConstraint+' Price>= '+CAST(@minPrice as varchar);
	end
	
	SET @query='Select 
			tbl1.ProductID,
			Name,
			Price,
			Description,
			Quantity,
			Sold,
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
		where' +@nameConstraint + @categoryConstraint +
		@priceConstraint;
	EXEC(@query);
end
