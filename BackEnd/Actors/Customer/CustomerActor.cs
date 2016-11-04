using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric.Health;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Customer.Interfaces;
using MyCommerce.Common.Interfaces;
using MyCommerce.Common.Entities;
using MyCommerce.DataAccess.Services;
using MyCommerce.SF.Core.Interfaces;
using MyCommerce.SF.Core.Tracing;
using Customer.Extensions;
using MyCommerce.SF.Core.Constants;
using ShoppingCart.Interfaces;

namespace Customer
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
    [ActorService(Name = ServiceNames.CustomerServiceName)]
    internal class CustomerActor : MyCommerce.SF.Core.Actors.ActorBase, ICustomerActor
    {
        const string CustomerInfoStateKey = "CustomerInfo";
        const string ShoppingCartInfoStateKey = "ShoppingCartInfo";

        private IReadOnlyEntityRepository<MyCommerce.Common.Entities.Customer, string> repository;
        private readonly bool isRepositoryInjected;

        public CustomerActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
            isRepositoryInjected = false;
        }

        public CustomerActor(ActorService actorService, ActorId actorId,
            IActorStateManager stateManager, IActorFactory actorFactory, IServiceFactory serviceFactory,
            IReadOnlyEntityRepository<MyCommerce.Common.Entities.Customer, string> repository) :
            base(actorService, actorId, stateManager, actorFactory, serviceFactory)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
            isRepositoryInjected = true;
        }

        private async Task ReadConfiguration()
        {
            if (!isRepositoryInjected)
            {
                var configValue = this.ReadSetting("ConnectionString");
                repository?.Dispose();
                repository = new CustomersService(configValue, true);
            }
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.Message(this, EventLevel.Verbose, "Actor activated.");

            await ReadConfiguration();
            await GetCustomerInfoFromDb();

            await base.OnActivateAsync();
        }


        private async Task<CustomerInfo> GetCustomerInfoFromDb()
        {
            var customer = await repository.GetSingleAsync(this.Id.ToString());
            var customerInfo = new CustomerInfo(customer);
            await StateManager.AddOrUpdateStateAsync(CustomerInfoStateKey, customerInfo, (a, b) => customerInfo);
            return customerInfo;
        }

        private async Task<bool> IsCustomerValid()
        {
            var customerInfo = await StateManager.TryGetStateAsync<CustomerInfo>(CustomerInfoStateKey);
            if (!customerInfo.HasValue) await GetCustomerInfoFromDb();
            return customerInfo.HasValue && customerInfo.Value.IsValid;
        }

        public async Task<CustomerInfoDto> GetCustomerInfoAsync()
        {
            if (!await IsCustomerValid()) return null;
            var customerInfo = await StateManager.TryGetStateAsync<CustomerInfo>(CustomerInfoStateKey);
            if (!customerInfo.HasValue || !customerInfo.Value.IsValid) return null;
            return customerInfo.Value.AsCustomerInfoDto();
        }

        public async Task<ShoppingCartInfoDto> GetCurrentShoppingCartAsync()
        {
            if (!await IsCustomerValid()) return null;
            var shoppingCartInfo = await StateManager.TryGetStateAsync<ShoppingCartInfo>(ShoppingCartInfoStateKey);
            if (!shoppingCartInfo.HasValue) return null;
            return shoppingCartInfo.Value.AsShoppingCartInfoDto();
        }

        public async Task<bool> AddProductToShoppingCartAsync(string productId, string productDescription, decimal unitCost, int quantity)
        {
            if (!await IsCustomerValid()) return false;
            var shoppingCartInfo = await StateManager.TryGetStateAsync<ShoppingCartInfo>(ShoppingCartInfoStateKey);
            bool isNewShoppingCart = true;
            ShoppingCartInfo shoppingCart = null;
            if (!shoppingCartInfo.HasValue)
            {
                isNewShoppingCart = true;
                shoppingCart = new ShoppingCartInfo();
                await StateManager.SetStateAsync(ShoppingCartInfoStateKey, shoppingCart);
            }
            else
            {
                isNewShoppingCart = false;
                shoppingCart = shoppingCartInfo.Value;
            }

            var shoppingCartProxy =
                this.ActorFactory.Create<IShoppingCartActor>(new ActorId(shoppingCart.Reference.ActorId),
                    shoppingCart.Reference.ApplicationName, shoppingCart.Reference.ServiceName);

            try
            {
                if (isNewShoppingCart) await shoppingCartProxy.SetCustomerAsync(this.Id.ToString());
                return await shoppingCartProxy.AddProductAsync(productId, productDescription, unitCost, quantity);
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public async Task<bool> EnableCustomerAsync(bool enable)
        {
            if (!await IsCustomerValid()) return false;
            var customerInfo = await StateManager.TryGetStateAsync<CustomerInfo>(CustomerInfoStateKey);
            if (customerInfo.HasValue)
            {
                customerInfo.Value.IsEnabled = !customerInfo.Value.IsEnabled;
                if (await UpdateCustomerDb(customerInfo.Value))
                {
                    await StateManager.SetStateAsync(CustomerInfoStateKey, customerInfo.Value);
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> UpdateCustomerDb(CustomerInfo customer)
        {
            var command = CommandFactory.CreateCommandForUpdateCustomer(this, customer);
            var storageManager = ActorFactory.Create<ISequentialExecutor>(
                new ActorId("Customers"),
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
