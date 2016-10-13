using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using MyCommerce.SF.Core.Entities;

namespace MyCommerce.SF.Core.Interfaces
{
    public interface IExecutor : IActor
    {
        Task ExecuteAsync(Command command);
    }
}
