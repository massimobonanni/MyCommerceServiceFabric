CREATE PROCEDURE [dbo].[ShoppingCart_Create]
	@idShoppingCart uniqueidentifier,
	@userNAme nvarchar(50)
AS
	
	insert into ShoppingCarts ( Id ,CreationDate,UserName)
	values (@idShoppingCart,GETDATE(),@username)

RETURN 0
