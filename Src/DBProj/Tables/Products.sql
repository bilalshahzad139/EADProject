CREATE TABLE [dbo].[Products] (
    [ProductID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NOT NULL,
    [Price]       FLOAT (53)   NOT NULL,
    [CreatedOn]   DATETIME     NOT NULL,
    [CreatedBy]   INT          NOT NULL,
    [ModifiedOn]  DATETIME     NULL,
    [ModifiedBy]  INT          NULL,
    [IsActive]    BIT          NOT NULL,
    [PictureName1] VARCHAR(50) NOT NULL, 
    [PictureName2] VARCHAR(50) NULL, 
    [PictureName3] VARCHAR(50) NULL, 
    [PictureName4] VARCHAR(50) NULL, 
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductID] ASC)
);

