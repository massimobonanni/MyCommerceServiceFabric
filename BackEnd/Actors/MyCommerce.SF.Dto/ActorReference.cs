using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyCommerce.SF.Dto
{
    [DataContract]
    [KnownType(typeof(ActorReferenceString))]
    [KnownType(typeof(ActorReferenceLong))]
    [KnownType(typeof(ActorReferenceGuid))]
    public class ActorReference<T>
    {
        [DataMember]
        public T ActorId { get; set; }

        [DataMember]
        public string ServiceName { get; set; }

        [DataMember]
        public string ApplicationName { get; set; }
    }

    [DataContract]
    public class ActorReferenceString : ActorReference<string>
    {

    }

    [DataContract]
    public class ActorReferenceLong : ActorReference<long>
    {

    }

    [DataContract]
    public class ActorReferenceGuid : ActorReference<Guid>
    {

    }
}
