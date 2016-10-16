using System.Collections.Generic;
using Newtonsoft.Json;
using WebApi.Dto;

namespace WebApi.Responses
{
    public abstract class ResponseBase
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; } = true;

        [JsonProperty("errors")]
        public IEnumerable<ErrorDto> Errors { get; set; }


        // Per supportare metadata nella response
        [JsonIgnore]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }

    public abstract class ResponseBase<TResponseDto> : ResponseBase
    {
        [JsonProperty("data")]
        public TResponseDto Data { get; set; }
    }

}
