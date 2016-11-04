CREATE PROCEDURE [dbo].[ProductInShoppingCart_Update]
	@idShoppingCart uniqueidentifier,
	@idProduct int,
	@shortDescription nvarchar(100),
	@unitPrice DECIMAL, 
    @quantity INT
AS
	IF EXISTS (SELECT id FROM ShoppingCarts  
			   WHERE  Id = @idShoppingCart)
	BEGIN
		IF EXISTS (SELECT id FROM ShoppingCartProducts  
					WHERE  IdShoppingCart = @idShoppingCart
					AND	 IdProduct = @idProduct)
			BEGIN
				update ShoppingCartProducts
				set ShortDescription =@shortDescription,
					Quantity =@quantity,
					UnitPrice=@unitPrice
				where IdShoppingCart = @idShoppingCart
						AND	 IdProduct = @idProduct
			END
		ELSE
			BEGIN
				insert into ShoppingCartProducts (IdShoppingCart,IdProduct,ShortDescription,Quantity,UnitPrice)
				values (@idShoppingCart, @idProduct, @shortDescription, @quantity, @unitPrice)
			END
	END

RETURN 0
