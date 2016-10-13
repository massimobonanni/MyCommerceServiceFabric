using System.Runtime.Serialization;

namespace Customer.Interfaces
{
    [DataContract]
    public class CustomerInfoDto
    {
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