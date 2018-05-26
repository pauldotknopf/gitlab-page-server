using Newtonsoft.Json;

namespace GitLabPages.Api.Hooks
{
    public class BuildHook
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }
        
        [JsonProperty("tag")]
        public bool Tag { get; set; }
        
        [JsonProperty("before_sha")]
        public string BeforeSha { get; set; }
        
        [JsonProperty("sha")]
        public string Sha { get; set; }
        
        [JsonProperty("build_id")]
        public int BuildId { get; set; }
        
        [JsonProperty("build_name")]
        public string BuildName { get; set; }
        
        [JsonProperty("build_stage")]
        public string BuildStage { get; set; }
        
        [JsonProperty("build_status")]
        public string BuildStatus { get; set; }
        
        // TODO: unknown type
        [JsonProperty("build_started_at")]
        public string BuildStartedAt { get; set; }
        
        // TODO: unknown type
        [JsonProperty("build_finished_at")]
        public string BuildFinishedAt { get; set; }
        
        // TODO: unknown type
        [JsonProperty("build_duration")]
        public string BuildDuration { get; set; }
        
        [JsonProperty("build_allow_failure")]
        public bool BuildAllowFailure { get; set; }
        
        [JsonProperty("project_id")]
        public int ProjectId { get; set; }
        
        [JsonProperty("project_name")]
        public string ProjectName { get; set; }
        
        [JsonProperty("user")]
        public Types.User User { get; set; }
        
        public class Types
        {
            public class User
            {
                [JsonProperty("id")]
                public int Id { get; set; }
                
                [JsonProperty("name")]
                public string Name { get; set; }
                
                [JsonProperty("email")]
                public string Email { get; set; }
            }
            
            public class Commit
            {
                [JsonProperty("id")]
                public int Id { get; set; }
                
                [JsonProperty("sha")]
                public string Sha { get; set; }
                
                [JsonProperty("message")]
                public string Message { get; set; }
                
                [JsonProperty("author_name")]
                public string AuthorName { get; set; }
                
                [JsonProperty("author_email")]
                public string AuthorEmail { get; set; }
                
                [JsonProperty("author_url")]
                public string AuthorUrl { get; set; }
                
                [JsonProperty("status")]
                public string Status { get; set; }
                
                // TODO: unknown type
                [JsonProperty("duration")]
                public string Duration { get; set; }
                
                // TODO: unknown type
                [JsonProperty("started_at")]
                public string StartedAt { get; set; }
                
                // TODO: unknown type
                [JsonProperty("finished_at")]
                public string FinishedAt { get; set; }
            }
        }
    }
}