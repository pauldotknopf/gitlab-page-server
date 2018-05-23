using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GitLabPages.Api.Hooks
{
    public class Hook
    {
        [JsonProperty("object_kind")]
        public HookTypes.ObjectKind ObjectKind { get; set; }
        
        [JsonProperty("repository")]
        public HookTypes.Repository Repository { get; set; }
        
        [JsonProperty("project")]
        public HookTypes.Repository Project { get; set; }
        
        public class HookTypes
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum ObjectKind
            {
                [EnumMember(Value="push")]
                Push,
                [EnumMember(Value="merge_request")]
                MergeRequest,
                [EnumMember(Value="build")]
                Build,
                [EnumMember(Value="pipeline")]
                Pipeline
            }
            
            public class Project
            {
                [JsonProperty("id")]
                public int Id { get; set; }
        
                [JsonProperty("name")]
                public string Name { get; set; }
        
                [JsonProperty("description")]
                public string Description { get; set; }
        
                [JsonProperty("web_url")]
                public string WebUrl { get; set; }
        
                [JsonProperty("avatar_url")]
                public string AvatarUrl { get; set; }
        
                [JsonProperty("git_ssh_url")]
                public string GitSshUrl { get; set; }
        
                [JsonProperty("git_http_url")]
                public string GitHttpUrl { get; set; }
        
                [JsonProperty("namespace")]
                public string Namespace { get; set; }
        
                [JsonProperty("visibility_level")]
                public int VisibilityLevel { get; set; }
        
                [JsonProperty("path_with_namespace")]
                public string PathWithNamespace { get; set; }
        
                [JsonProperty("default_branch")]
                public string DefaultBranch { get; set; }
        
                [JsonProperty("ci_config_path")]
                public string CiConfigPath { get; set; }
            }

            public class Repository
            {
                [JsonProperty("name")]
                public string Name { get; set; }
                
                [JsonProperty("url")]
                public string Url { get; set; }
                
                [JsonProperty("description")]
                public string Description { get; set; }
                
                [JsonProperty("homepage")]
                public string Homepage { get; set; }
                
                [JsonProperty("git_http_url")]
                public string GitHttpUrl { get; set; }
                
                [JsonProperty("git_ssh_url")]
                public string GitSshUrl { get; set; }
                
                [JsonProperty("visibility_level")]
                public int VisibilityLevel { get; set; }
            }
        }
    }
}