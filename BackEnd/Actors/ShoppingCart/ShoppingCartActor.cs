using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using MyCommerce.SF.Core.Constants;
using MyCommerce.SF.Core.Interfaces;
using ShoppingCart.Interfaces;

namespace ShoppingCart
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    [ActorService(Name = ServiceNames.ShoppingCartServiceName)]
    internal class ShoppingCartActor : MyCommerce.SF.Core.Actors.ActorBase, IShoppingCartActor
    {
        const string CustomerStateKey = "Customer";
        const string ProductItemStatePrefix = "Product";

        /// <summary>
        /// Initializes a new instance of ShoppingCart
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ShoppingCartActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public ShoppingCartActor(ActorService actorService, ActorId actorId,
            IActorStateManager stateManager, IActorFactory actorFactory, IServiceFactory serviceFactory) :
            base(actorService, actorId, stateManager, actorFactory, serviceFactory)
        {

        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            return Task.Delay(0);
        }

        private string GetProductStateKey(string productId)
        {
            return $"{ProductItemStatePrefix}_{productId}";
        }

        public async Task SetCustomerAsync(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                await CreateCartDb(username);
                await this.StateManager.GetOrAddStateAsync(CustomerStateKey, username);
            }
        }

        public async Task<bool> AddProductAsync(string productId, string productDescription, decimal unitCost, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productId)) return false;

            var productInCart =
                await this.StateManager.GetOrAddStateAsync(GetProductStateKey(productId), new ProductInfo()
                {
                    Id = productId,
                    Description = productDescription
                });
            productInCart.UnitCost = unitCost;
            productInCart.Quantity += quantity;

            await UpdateProductDb(productInCart);
            await this.StateManager.SetStateAsync(GetProductStateKey(productId), productInCart);
            return true;
        }

        private async Task<bool> UpdateProductDb(ProductInfo product)
        {
            var command = CommandFactory.CreateCommandForUpdateProductInCart(this, product);
            var storageManager = ActorFactory.Create<ISequentialExecutor>(
                new ActorId("ShoppingCarts"),
                ServiceNames.ApplicationName,
                ServiceNames.StorageManagerServiceName);

            try
            {
                await storageManager.ExecuteAsync(command);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> CreateCartDb(string customer)
        {
            var command = CommandFactory.CreateCommandForCreateCart(this, customer);
            var storageManager = ActorFactory.Create<ISequentialExecutor>(
                new ActorId("ShoppingCarts"),
                ServiceNames.ApplicationName,
                ServiceNames.StorageManagerServiceName);

            try
            {
                await storageManager.ExecuteAsync(command);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
