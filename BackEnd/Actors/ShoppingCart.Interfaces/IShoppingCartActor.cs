using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using MyCommerce.SF.Core.Interfaces;

namespace ShoppingCart.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IShoppingCartActor : IActor, IPublisherActor
    {
        Task<bool> AddProductAsync(string productId, string productDescription, decimal unitCost, int quantity);

        Task<ShoppingStateDto> GetStateAsync();
    }
}
