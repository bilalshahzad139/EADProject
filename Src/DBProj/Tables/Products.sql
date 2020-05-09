CREATE TABLE [dbo].[Products] (
    [ProductID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NOT NULL,
    [Price]       FLOAT (53)   NOT NULL,
    [PictureName] VARCHAR (50) NOT NULL,
    [CreatedOn]   DATETIME     NOT NULL,
    [CreatedBy]   INT          NOT NULL,
    [ModifiedOn]  DATETIME     NULL,
    [ModifiedBy]  INT          NULL,
    [IsActive]    BIT          NOT NULL,
    [ProductCategoryID] VARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductID] ASC),
    FOREIGN KEY (ProductCategoryID) REFERENCES ProductCategory(ProductCategoryID)
);


