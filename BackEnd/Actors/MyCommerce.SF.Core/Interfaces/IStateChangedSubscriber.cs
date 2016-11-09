using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace MyCommerce.SF.Core.Interfaces
{
    public interface IStateChangedSubscriber : IActor
    {
        Task StateChanged(ActorId actorId);
        Task<string> GetSubscriberId();
    }
}