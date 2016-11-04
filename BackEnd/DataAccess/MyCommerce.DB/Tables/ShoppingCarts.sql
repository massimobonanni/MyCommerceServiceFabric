CREATE TABLE [dbo].[ShoppingCarts]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [CreationDate] DATETIME2 NOT NULL, 
    [UserName] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_ShoppingCarts_Customers] FOREIGN KEY (UserName) REFERENCES [Customers]([UserName])
)
