using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using MyCommerce.SF.Core.Extensions;
using MyCommerce.SF.Core.Interfaces;

namespace MyCommerce.SF.Core.Utilities
{

    public class SubscribersWrapper
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SubscribersWrapper(IActorStateManager stateManager)
        {
            SubscribersName = $"__subscribers";
            StateManager = stateManager;
        }

        protected IActorStateManager StateManager { get; }

        protected string SubscribersName { get; }

        public async Task SubscribeAsync(ISubscriberActor subscriber)
        {
            var actorId = subscriber.GetActorId();
            await StateManager.AddOrUpdateStateAsync(SubscribersName,
                new Dictionary<string, ISubscriberActor> { { actorId.GetStringId(), subscriber } },
                (key, value) =>
                {
                    value[actorId.GetStringId()] = subscriber;
                    return value;
                });
        }

        public async Task UnsubscribeAsync(ISubscriberActor subscriber)
        {
            var actorId = subscriber.GetActorId();
            await StateManager.AddOrUpdateStateAsync(SubscribersName, new Dictionary<string, ISubscriberActor>(),
                (key, value) =>
                {
                    if (value.ContainsKey(actorId.GetStringId()))
                    {
                        value.Remove(actorId.GetStringId());
                    }
                    return value;
                });
        }

        public async Task<IEnumerable<ISubscriberActor>> GetSubscribers()
        {
            var subscribers = await StateManager.TryGetStateAsync<Dictionary<string, ISubscriberActor>>(SubscribersName);
            if (subscribers.HasValue)
            {
                return subscribers.Value.Select(kvp => kvp.Value);
            }
            else
            {
                // return an empty enumerable
                return new ISubscriberActor[0];
            }
        }

        public async Task NotifySubscribersAsync(IPublisherActor actor)
        {
            var subscribers = await GetSubscribers();
            foreach (var subscriber in subscribers)
            {
                await subscriber.NotifyAsync(actor);
            }
        }

    }
}