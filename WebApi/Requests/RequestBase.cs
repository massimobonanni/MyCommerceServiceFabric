using Newtonsoft.Json;

namespace WebApi.Requests
{
    public abstract class RequestBase
    {
        [JsonProperty("lang")]
        public string Language { get; set; } = "it-IT";

    }

    public abstract class RequestBase<TPayload> : RequestBase
    {
        [JsonProperty("data")]
        public TPayload Data { get; set; }

    }
}
