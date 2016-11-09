using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ShoppingCart.Interfaces;

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

        public ProductDto ToProductDto()
        {
            return new ProductDto()
            {
                Id = this.Id,
                Description = this.Description,
                Quantity = this.Quantity,
                UnitCost = this.UnitCost
            };
        }
    }
}
