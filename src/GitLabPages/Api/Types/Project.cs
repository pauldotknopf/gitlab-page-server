using Newtonsoft.Json;

namespace GitLabPages.Api.Types
{
    public class Project
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("path_with_namespace")]
        public string PathWithNamespace { get; set; }
    }
}