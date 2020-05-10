CREATE TABLE [dbo].[Orders] (
    [UserID]        INT     NOT NULL,
    [ProductID]     INT     NOT NULL,    
    [Quantity]      INT     NOT NULL,
    CONSTRAINT [PK_OrdersUsers] PRIMARY KEY  ([UserID],[ProductID] ASC),
    CONSTRAINT [FK_OrdersUsers] FOREIGN KEY ([UserID]) REFERENCES Users(UserID),
    CONSTRAINT [FK_OrdersProducts] FOREIGN KEY ([ProductID]) REFERENCES Products(ProductID),
);