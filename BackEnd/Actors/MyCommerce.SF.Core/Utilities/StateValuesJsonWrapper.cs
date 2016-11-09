using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using MyCommerce.SF.Core.Interfaces;
using Newtonsoft.Json;

namespace MyCommerce.SF.Core.Utilities
{
    public  class StateValuesJsonWrapper : StateValuesWrapperBase
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public StateValuesJsonWrapper(IActorStateManager stateManager) : base(stateManager)
        {

        }
        public override async Task<string> GetSerializedState()
        {
            //var value = await GetValueAsync();
            //return JsonConvert.SerializeObject(value, Formatting.Indented);
            throw new NotImplementedException();
        }

        public override Task PutSerializedState(string serializedState)
        {
            //var value = JsonConvert.DeserializeObject<T>(serializedState);
            //return SetValueAsync(value);
            throw new NotImplementedException();
        }
    }
}
