using Newtonsoft.Json;

namespace GitLabPages.Api.Types
{
    public class Pipeline
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("sha")]
        public string Sha { get; set; }
        
        [JsonProperty("ref")]
        public string Ref { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}