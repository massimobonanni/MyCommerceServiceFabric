CREATE TABLE [dbo].[Customers]
(
	[UserName] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [IsEnabled] BIT NOT NULL DEFAULT 1
)
