using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyCommerce.SF.Dto
{
    [DataContract]
    public class ResultDto
    {
        [DataMember]
        public bool IsSucceded { get; set; }
        [DataMember]
        public ErrorDto Error { get; set; }
    }

    [DataContract]
    public class ResultDto<TDto> : ResultDto
    {
        [DataMember]
        public TDto Data { get; set; }
    }
}
