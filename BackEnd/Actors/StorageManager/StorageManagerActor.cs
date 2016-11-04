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

namespace StorageManager
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
    [ActorService(Name = ServiceNames.StorageManagerServiceName)]
    internal class StorageManagerActor : MyCommerce.SF.Core.Actors.SequentialExecutorBase,ISequentialExecutor
    {
        public StorageManagerActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public StorageManagerActor(ActorService actorService, ActorId actorId,
            IActorStateManager stateManager, IActorFactory actorFactory, IServiceFactory serviceFactory) :
            base(actorService, actorId, stateManager, actorFactory, serviceFactory)
        {
        }

        protected override string ProcessorApplicationName
        {
            get
            {
                return this.ActorService.Context.CodePackageActivationContext.ApplicationName;
            }
        }

        protected override string ProcessorServiceName
        {
            get
            {
                return "StorageManagerProcessorActor";
            }
        }
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            await base.OnActivateAsync();
        }

    }
}
