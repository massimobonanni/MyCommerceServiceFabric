using System;

namespace MyCommerce.SF.Core.Tracing
{
    internal class EventSourcePayload
    {
        public string Type;
        public string Id;
        public string ApplicationTypeName;
        public string ApplicationName;
        public string ServiceTypeName;
        public string ServiceName;
        public string NodeName;
        public Guid PartitionId;
        public long ReplicaOrInstanceId;
        public string MessageOrAction;
        public string ExceptionOrMember;
        public string CorrelationId;


        public EventSourcePayload() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, default(Guid), default(long), string.Empty)
        {
        }

        public EventSourcePayload(
            string type,
            string id,
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            Guid partitionId,
            long replicaOrInstanceId,
            string nodeName,
            string messageOrAction = "",
            string exceptionOrMember = "",
            string correlationId = "")
        {
            this.Type = (type != null ? type : string.Empty);
            this.Id = id;
            this.ApplicationTypeName = applicationTypeName;
            this.ApplicationName = applicationName;
            this.ServiceTypeName = serviceTypeName;
            this.ServiceName = serviceName;
            this.PartitionId = partitionId;
            this.ReplicaOrInstanceId = replicaOrInstanceId;
            this.NodeName = nodeName;
            this.MessageOrAction = (messageOrAction != null ? messageOrAction : string.Empty);
            this.ExceptionOrMember = (exceptionOrMember != null ? exceptionOrMember : string.Empty);
            this.CorrelationId = (correlationId != null ? correlationId : string.Empty);
        }


        public bool Validate()
        {
            return this.Type != null
                && this.Id != null
                && this.ApplicationTypeName != null
                && this.ApplicationName != null
                && this.ServiceTypeName != null
                && this.ServiceName != null
                && this.NodeName != null
                && this.MessageOrAction != null
                && this.ExceptionOrMember != null
                && this.CorrelationId != null;
        }
    }
}
