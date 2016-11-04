using System.Runtime.Serialization;

namespace MyCommerce.SF.Dto
{
    [DataContract]
    public class ErrorDto
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}