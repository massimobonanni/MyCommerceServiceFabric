using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;

namespace MyCommerce.SF.Core.Entities
{
    [DataContract]
    public class Command
    {
        [JsonProperty]
        [DataMember]
        public Guid CommandId { get; set; }

        [JsonProperty]
        [DataMember]
        public string Name { get; set; }

        [JsonProperty]
        [DataMember]
        public Dictionary<string, object> Parameters { get; set; }

        [JsonProperty]
        [DataMember]
        public Dictionary<string, object> Properties { get; set; }

        [JsonProperty]
        [DataMember]
        public string CallerId { get; private set; }

        [JsonProperty]
        [DataMember]
        public string CallerApplicationName { get; private set; }

        [JsonProperty]
        [DataMember]
        public Uri CallerServiceUri { get; private set; }

        [JsonProperty]
        [DataMember]
        public string CallerServiceName { get; private set; }

        #region Constructors

        public Command()
        {
            this.CommandId = Guid.NewGuid();
            this.Parameters = new Dictionary<string, object>();
            this.Properties = new Dictionary<string, object>();
        }

        public Command(string commandName, Dictionary<string, object> parameters) : this()
        {
            this.Name = commandName;
            if (parameters != null)
            {
                this.Parameters = parameters;
            }
        }

        public Command(Actor caller, string commandName, Dictionary<string, object> parameters) : this(commandName, parameters)
        {
            this.CallerId = caller.Id.ToString();
            this.CallerApplicationName = caller.ApplicationName;
            this.CallerServiceUri = caller.ServiceUri;
            this.CallerServiceName = caller.ActorService.ActorTypeInformation.ServiceName;
        }

        public Command(StatefulService caller, string commandName, Dictionary<string, object> parameters) : this(commandName, parameters)
        {
            this.CallerId = caller.Context.ReplicaOrInstanceId.ToString();
            this.CallerApplicationName = caller.Context.CodePackageActivationContext.ApplicationName;
            this.CallerServiceUri = caller.Context.ServiceName;
            this.CallerServiceName = caller.Context.ServiceTypeName;
        }

        public Command(StatelessService caller, string commandName, Dictionary<string, object> parameters) : this(commandName, parameters)
        {
            this.CallerId = caller.Context.ReplicaOrInstanceId.ToString();
            this.CallerApplicationName = caller.Context.CodePackageActivationContext.ApplicationName;
            this.CallerServiceUri = caller.Context.ServiceName;
            this.CallerServiceName = caller.Context.ServiceTypeName;
        }

        #endregion


        public string ToSqlStatement()
        {
            // TODO da migliorare per gestire meglio il valore dei parametri in base al tipo
            var parameters = this.Parameters.Select((param) => $"@{param.Key} = {param.Value}");
            var statementParameters = string.Join(", ", parameters);
            return $"exec {this.Name} {statementParameters}";
        }

        public TValue DeserializePropertyObject<TValue>(string propertyName) where TValue : class
        {
            if (Properties.ContainsKey(propertyName)) return null;
            try
            {
                return (TValue)Properties[propertyName];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SerializePropertyObject(string propertyName, object value)
        {
            Properties[propertyName] = JsonConvert.SerializeObject(value);
        }

        public TValue DeserializeParameterObject<TValue>(string parameterName) where TValue : class
        {
            if (Parameters.ContainsKey(parameterName)) return null;
            try
            {
                return JsonConvert.DeserializeObject<TValue>((string)Parameters[parameterName]);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SerializeParameterObject(string parameterName, object value)
        {
            Parameters[parameterName] = JsonConvert.SerializeObject(value);
        }
    }
}
