using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Newtonsoft.Json;

namespace MyCommerce.SF.Core.Utilities
{
    public class SingleValueJsonWrapper<T> : StateValueWrapperBase<T>
    {
        private readonly Func<T> _newValueConstructor;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SingleValueJsonWrapper(string name, Func<T> newValueConstructor, IActorStateManager stateManager)
            : base(name, stateManager)
        {
            _newValueConstructor = newValueConstructor;
        }

        public override async Task<T> GetValueAsync()
        {
            if (await StateManager.ContainsStateAsync(Name))
            {
                return await StateManager.GetStateAsync<T>(Name);
            }
            else
            {
                var newValue = _newValueConstructor.Invoke();
                await StateManager.SetStateAsync(Name, newValue);
                return newValue;
            }
        }

        public override Task SetValueAsync(T value)
        {
            return StateManager.SetStateAsync(Name, value);
        }

        public override async Task<string> GetSerializedState()
        {
            var value = await GetValueAsync();
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public override Task PutSerializedState(string serializedState)
        {
            var value = JsonConvert.DeserializeObject<T>(serializedState);
            return SetValueAsync(value);
        }
    }
}