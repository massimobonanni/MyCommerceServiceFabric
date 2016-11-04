CREATE TABLE [dbo].[ShoppingCartProducts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[IdShoppingCart] UNIQUEIDENTIFIER NOT NULL,
	[IdProduct] INT NOT NULL, 
	[ShortDescription] NVARCHAR(100) NOT NULL, 
    [LongDescription] NVARCHAR(MAX) NOT NULL, 
    [UnitPrice] DECIMAL NOT NULL, 
    [Quantity] INT NOT NULL, 
    CONSTRAINT [FK_ShoppingCarts_ShoppingCartProducts] FOREIGN KEY ([IdShoppingCart]) REFERENCES [ShoppingCarts](Id)
)
