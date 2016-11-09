using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting;

namespace MyCommerce.SF.Core.Interfaces
{
    /// <summary>
    /// Defines a reliable service interface that allows the change of a state of an actor
    /// </summary>
    /// <seealso cref="Microsoft.ServiceFabric.Services.Remoting.IService" />
    public interface IAlterActorStateService : IService
    {
        /// <summary>
        /// Retrieves the content (serialized) a property status of an actor.
        /// </summary>
        /// <param name="actorReference">The actor reference.</param>
        /// <param name="statePropertyName">Name of the state property.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> GetSerializedState(ActorReference actorReference, string statePropertyName);

        /// <summary>
        /// Sets the content (serialized) a property status of an actor
        /// </summary>
        /// <param name="actorReference">The actor reference.</param>
        /// <param name="statePropertyName">Name of the state property.</param>
        /// <param name="serializedState">State value (serialized).</param>
        /// <returns>Task.</returns>
        Task PutSerializedState(ActorReference actorReference, string statePropertyName, string serializedState);
    }
}