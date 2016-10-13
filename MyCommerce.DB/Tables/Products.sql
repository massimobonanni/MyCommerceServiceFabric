CREATE TABLE [dbo].[Products]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ShortDescription] NVARCHAR(100) NOT NULL, 
    [LongDescription] NVARCHAR(MAX) NOT NULL, 
    [IsAvailable] BIT NOT NULL, 
    [UnitPrice] DECIMAL NOT NULL, 
    [UnitInStore] INT NOT NULL
)
