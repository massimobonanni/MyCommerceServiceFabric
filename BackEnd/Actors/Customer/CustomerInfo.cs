using System.Runtime.Serialization;

namespace Customer
{
    [DataContract]
    internal class CustomerInfo
    {
        public CustomerInfo()
        {
        }
        public CustomerInfo(MyCommerce.Common.Entities.Customer customer)
        {
            this.FirstName = customer?.FirstName;
            this.LastName = customer?.LastName;
            this.IsEnabled = customer == null ? false : customer.IsEnabled;
            this.IsValid = customer != null;
        }

        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public bool IsValid { get; set; }
        [DataMember]
        public bool IsEnabled { get; set; }

    }
}