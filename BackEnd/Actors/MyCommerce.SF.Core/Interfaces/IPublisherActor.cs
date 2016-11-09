using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace MyCommerce.SF.Core.Interfaces
{
    public interface IPublisherActor : IActor
    {
        Task<bool> SubscribeAsync(ISubscriberActor subscriber);

        Task<bool> UnsubscribeAsync(ISubscriberActor subscriber);
    }
}
