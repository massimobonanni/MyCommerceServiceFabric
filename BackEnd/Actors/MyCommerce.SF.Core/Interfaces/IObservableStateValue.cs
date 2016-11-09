using System.Threading.Tasks;

namespace MyCommerce.SF.Core.Interfaces
{
    public interface IObservableStateValue
    {
        Task SubscribeAsync(IStateChangedSubscriber subscriber);

        Task UnsubscribeAsync(IStateChangedSubscriber subscriber);
    }
}