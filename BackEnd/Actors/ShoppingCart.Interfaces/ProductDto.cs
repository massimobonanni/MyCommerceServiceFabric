using System.Runtime.Serialization;

namespace ShoppingCart.Interfaces
{
    [DataContract]
    public class ProductDto
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