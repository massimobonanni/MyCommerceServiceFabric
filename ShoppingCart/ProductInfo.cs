using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart
{
    [DataContract]
    internal class ProductInfo
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public decimal UnitCost { get; set; }
        [DataMember]
        public int Quantity { get; set; }
    }
}
