using Newtonsoft.Json;

namespace GitLabPages.Api.Requests
{
    public class JobsRequest
    {
        [JsonProperty("scope", NullValueHandling = NullValueHandling.Ignore)]
        public string Scope { get; set; }
    }
}