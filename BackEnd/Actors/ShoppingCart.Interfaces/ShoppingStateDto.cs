using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ShoppingCart.Interfaces
{
    [DataContract]
    public class ShoppingStateDto
    {
        [DataMember]
        public ActorId Id { get; set; }

        [DataMember]
        public IEnumerable<ProductDto> Products { get; set; }
    }
}
