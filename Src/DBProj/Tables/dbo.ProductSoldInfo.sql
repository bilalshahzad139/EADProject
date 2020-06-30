CREATE TABLE [dbo].[ProductSoldInfo]
(
	[ProductID] INT NOT NULL, 
    [SoldQuantity] INT NOT NULL  , 
    [SoldDate] DATE NOT NULL,
    PRIMARY KEY ([ProductID],[SoldDate]),
    CONSTRAINT [FKProductId]  FOREIGN KEY (ProductID) REFERENCES Products([ProductID]),


 )
    
