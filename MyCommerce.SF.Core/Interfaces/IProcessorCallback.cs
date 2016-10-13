using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace MyCommerce.SF.Core.Interfaces
{
    public interface IProcessorCallback : IActor
    {
        Task ProcessingCompleteCallBackAsync();
    }
}
