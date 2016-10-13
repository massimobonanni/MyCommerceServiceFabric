using System.Runtime.Serialization;
using Microsoft.ServiceFabric.Actors;
using MyCommerce.SF.Core.Constants;
using MyCommerce.SF.Dto;

namespace Customer
{
    [DataContract]
    internal class ShoppingCartInfo
    {
        public ShoppingCartInfo()
        {
            this.Reference = new ActorReferenceGuid();
            this.Reference.ActorId = System.Guid.NewGuid();
            this.Reference.ServiceName = ServiceNames.ShoppingCartServiceName;
            this.Reference.ApplicationName = ServiceNames.ApplicationName;
        }

        [DataMember]
        public ActorReferenceGuid Reference { get; set; }


    }
}