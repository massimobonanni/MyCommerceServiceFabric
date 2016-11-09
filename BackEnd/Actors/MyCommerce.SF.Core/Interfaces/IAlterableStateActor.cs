using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace MyCommerce.SF.Core.Interfaces
{
    /// <summary>
    /// Defines the interface of an actor with state editable
    /// </summary>
    /// <seealso cref="Microsoft.ServiceFabric.Actors.IActor" />
    public interface IAlterableStateActor : IActor
    {
        /// <summary>
        /// Retrieves the content (serialized) a property status of the actor.
        /// </summary>
        /// <param name="statePropertyName">Name of the state property.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> GetSerializedState(string statePropertyName);
        /// <summary>
        /// Sets the content (serialized) a property status of the actor.
        /// </summary>
        /// <param name="statePropertyName">Name of the state property.</param>
        /// <param name="serializedState">State of the serialized.</param>
        /// <returns>Task.</returns>
        Task PutSerializedState(string statePropertyName, string serializedState);
    }
}
