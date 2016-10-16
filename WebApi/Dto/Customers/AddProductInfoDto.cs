using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApi.Dto.Customers
{
    public class AddProductInfoDto
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("productDescription")]
        public string ProductDescription { get; set; }

        [JsonProperty("unitCost")]
        public decimal UnitCost { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
