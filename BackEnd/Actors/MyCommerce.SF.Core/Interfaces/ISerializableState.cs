using System.Threading.Tasks;

namespace MyCommerce.SF.Core.Interfaces
{
    public interface ISerializableState
    {
        Task<string> GetSerializedState();
        Task PutSerializedState(string serializedState);
    }
}