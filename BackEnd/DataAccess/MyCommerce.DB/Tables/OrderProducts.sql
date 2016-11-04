CREATE TABLE [dbo].[OrderProducts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[IdOrder] INT NOT NULL,
	[ShortDescription] NVARCHAR(100) NOT NULL, 
    [LongDescription] NVARCHAR(MAX) NOT NULL, 
    [UnitPrice] DECIMAL NOT NULL, 
    [Quantita] INT NOT NULL, 
    CONSTRAINT [FK_Orders_OrderProducts] FOREIGN KEY ([IdOrder]) REFERENCES [Orders](Id)
)
