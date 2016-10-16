using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApi.Dto.Customers
{
    public class CustomerInfoDto
    {
        public CustomerInfoDto()
        {

        }

        public CustomerInfoDto(Customer.Interfaces.CustomerInfoDto customerInfo)
        {
            this.FirstName = customerInfo.FirstName;
            this.LastName = customerInfo.LastName;
            this.IsEnabled = customerInfo.IsEnabled;
        }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
