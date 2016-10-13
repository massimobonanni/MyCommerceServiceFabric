using System.Threading.Tasks;
using MyCommerce.SF.Core.Entities;

namespace MyCommerce.SF.Core.Interfaces
{
    public interface ISequentialExecutor: IExecutor
    {
        Task<int> GetQueueLengthAsync();

        Task<Command> GetCurrentProcessingCommandAsync();
    }
}
