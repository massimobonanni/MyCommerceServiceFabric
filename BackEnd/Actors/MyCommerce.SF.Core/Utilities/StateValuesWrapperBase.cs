using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using MyCommerce.SF.Core.Interfaces;

namespace MyCommerce.SF.Core.Utilities
{
    public abstract class StateValuesWrapperBase : IObservableStateValue, ISerializableState
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected StateValuesWrapperBase(IActorStateManager stateManager)
        {
            if (stateManager == null) throw new ArgumentNullException("stateManager");
            SubscribersName = $"__subscribers";
            StateManager = stateManager;
        }

        protected IActorStateManager StateManager { get; }

        protected string SubscribersName { get; }

        public virtual async Task<T> GetValueAsync<T>(string statusName)
        {
            if (await StateManager.ContainsStateAsync(statusName))
            {
                return await StateManager.GetStateAsync<T>(statusName);
            }
            return default(T);
        }

        public virtual Task SetValueAsync(string statusName, object value)
        {
            return StateManager.SetStateAsync(statusName, value);
        }

        public async Task SubscribeAsync(IStateChangedSubscriber subscriber)
        {
            var subscriberId = await subscriber.GetSubscriberId();

            await StateManager.AddOrUpdateStateAsync(SubscribersName,
                new Dictionary<string, IStateChangedSubscriber> { { subscriberId, subscriber } },
                (key, value) =>
                {
                    value[subscriberId] = subscriber;
                    return value;
                });
        }

        public async Task UnsubscribeAsync(IStateChangedSubscriber subscriber)
        {
            var subscriberId = await subscriber.GetSubscriberId();
            await StateManager.AddOrUpdateStateAsync(SubscribersName, new Dictionary<string, IStateChangedSubscriber>(),
                (key, value) =>
                {
                    if (value.ContainsKey(subscriberId))
                    {
                        value.Remove(subscriberId);
                    }
                    return value;
                });
        }

        public async Task<IEnumerable<IStateChangedSubscriber>> GetSubscribers()
        {
            var subscribers = await StateManager.TryGetStateAsync<Dictionary<string, IStateChangedSubscriber>>(SubscribersName);
            if (subscribers.HasValue)
            {
                return subscribers.Value.Select(kvp => kvp.Value);
            }
            else
            {
                // return an empty enumerable
                return new IStateChangedSubscriber[0];
            }
        }

        public async Task NotifySubscribersAsync(ActorId actorId)
        {
            var subscribers = await GetSubscribers();
            foreach (var subscriber in subscribers)
            {
                await subscriber.StateChanged(actorId);
            }
        }

        public abstract Task<string> GetSerializedState();
        public abstract Task PutSerializedState(string serializedState);
    }

}
