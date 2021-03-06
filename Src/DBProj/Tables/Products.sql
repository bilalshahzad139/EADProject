﻿CREATE TABLE [dbo].[Products] (
    [ProductID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NOT NULL,
    [Price]       FLOAT   NOT NULL,
    [CreatedOn]   DATETIME     NOT NULL,
    [CreatedBy]   INT          NOT NULL,
    [ModifiedOn]  DATETIME     NULL,
    [ModifiedBy]  INT          NULL,
    [IsActive]    BIT          NOT NULL,
    [ProductCategoryID] INT NOT NULL, 
    [Quantity] INT NOT NULL, 
    [Sold] INT NOT NULL, 
    [Description] VARCHAR(100) NULL, 
    [isOnSale] INT NULL, 
    [saleDescription] VARCHAR(100) NULL, 
    [percentageDiscount] INT NULL, 
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductID] ASC),
    FOREIGN KEY (ProductCategoryID) REFERENCES ProductCategory(ProductCategoryID)
);


