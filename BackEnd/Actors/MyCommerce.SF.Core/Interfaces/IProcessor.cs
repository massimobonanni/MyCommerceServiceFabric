using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using MyCommerce.SF.Core.Entities;

namespace MyCommerce.SF.Core.Interfaces
{
    public interface IProcessor : IActor
    {
        Task ProcessAsync(Command command, Uri callbackServiceUri);
    }
}
