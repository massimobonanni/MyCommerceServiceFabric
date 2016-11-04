using System;
using System.Collections;
using System.Runtime.Serialization;
using MyCommerce.SF.Dto;

namespace Customer.Interfaces
{
    [DataContract]
    public class ShoppingCartInfoDto
    {
        [DataMember]
        public ActorReferenceGuid Reference { get; set; }

    }
}