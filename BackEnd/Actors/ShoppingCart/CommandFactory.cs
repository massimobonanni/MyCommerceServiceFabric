using System.Collections.Generic;
using Microsoft.ServiceFabric.Actors.Runtime;
using MyCommerce.SF.Core.Entities;

namespace ShoppingCart
{
    internal static class CommandFactory
    {
        public static Command CreateCommandForUpdateProductInCart(Actor actor, ProductInfo product)
        {
            return new Command(actor,
                "[dbo].[ProductInShoppingCart_Update]",
                new Dictionary<string, object>() {
                    { "@idShoppingCart",actor.Id.ToString()},
                    { "@idProduct",product.Id },
                    { "@shortDescription",product.Description },
                    { "@unitPrice",product.UnitCost },
                    { "@quantity",product.Quantity}
                });
        }

        public static Command CreateCommandForCreateCart(Actor actor, string username)
        {
            return new Command(actor,
                "[dbo].[ShoppingCart_Create]",
                new Dictionary<string, object>() {
                    { "@idShoppingCart",actor.Id.ToString()},
                    { "@userName",username}
                });
        }
    }
}
