using Newtonsoft.Json;

namespace WebApi.Dto
{
    public class ErrorDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
