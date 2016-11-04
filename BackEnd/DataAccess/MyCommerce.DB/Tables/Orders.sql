CREATE TABLE [dbo].[Orders]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CreationDate] DATETIME2 NOT NULL,
	[UserName] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_Orders_Customers] FOREIGN KEY (UserName) REFERENCES [Customers]([UserName])
)
