using System.Collections.Generic;
using Newtonsoft.Json;

namespace GitLabPages.Api.Hooks
{
    public class PipelineHook : Hook
    {
        [JsonProperty("object_attributes")]
        public Types.ObjectAttributes ObjectAttributes { get; set; }
        
        [JsonProperty("user")]
        public Types.User User { get; set; }
        
        [JsonProperty("commit")]
        public Types.Commit Commit { get; set; }
        
        [JsonProperty("builds")]
        public List<Types.Build> Builds { get; set; }
        
        public class Types
        {
            public class ObjectAttributes
            {
                [JsonProperty("id")]
                public int Id { get; set; }
                
                [JsonProperty("ref")]
                public string Ref { get; set; }
                
                [JsonProperty("tag")]
                public bool Tag { get; set; }
                
                [JsonProperty("sha")]
                public string Sha { get; set; }
                
                [JsonProperty("before_sha")]
                public string BeforeSha { get; set; }
                
                [JsonProperty("status")]
                public string Status { get; set; }
                
                [JsonProperty("detailed_status")]
                public string DetailedStatus { get; set; }
                
                [JsonProperty("stages")]
                public List<string> Stages { get; set; }
                
                [JsonProperty("created_at")]
                public string CreatedAt { get; set; }
                
                [JsonProperty("finished_at")]
                public string FinishedAt { get; set; }
                
                [JsonProperty("duration")]
                public int Duration { get; set; }
            }

            public class User
            {
                [JsonProperty("name")]
                public string Name { get; set; }
                
                [JsonProperty("username")]
                public string Username { get; set; }
                
                [JsonProperty("avatar_url")]
                public string AvatarUrl { get; set; }
            }

            public class Commit
            {
                [JsonProperty("id")]
                public string Id { get; set; }
                
                [JsonProperty("message")]
                public string Message { get; set; }
                
                [JsonProperty("timestamp")]
                public string Timestamp { get; set; }
                
                [JsonProperty("url")]
                public string Url { get; set; }

                [JsonProperty("author")]
                public Author Author { get; set; }
            }

            public class Author
            {
                [JsonProperty("name")]
                public string Name { get; set; }
                
                [JsonProperty("email")]
                public string Email { get; set; }
            }

            public class Build
            {
                [JsonProperty("id")]
                public int Id { get; set; }
                
                [JsonProperty("stage")]
                public string Stage { get; set; }
                
                [JsonProperty("name")]
                public string Name { get; set; }
                
                [JsonProperty("status")]
                public string Status { get; set; }
                
                [JsonProperty("created_at")]
                public string CreatedAt { get; set; }
                
                [JsonProperty("started_at")]
                public string StartedAt { get; set; }
                
                [JsonProperty("finished_at")]
                public string FinishedAt { get; set; }
                
                [JsonProperty("when")]
                public string When { get; set; }
                
                [JsonProperty("manual")]
                public bool Manual { get; set; }
                
                [JsonProperty("user")]
                public User User { get; set; }
                
                [JsonProperty("runner")]
                public Runner Runner { get; set; }

                [JsonProperty("artifacts_file")]
                public ArtifactFile ArtifactsFile { get; set; }
            }

            public class ArtifactFile
            {
                [JsonProperty("filename")]
                public string Filename { get; set; }
                
                [JsonProperty("size")]
                public ulong Size { get; set; }
            }
            
            public class Runner
            {
                [JsonProperty("id")]
                public int Id { get; set; }
                
                [JsonProperty("description")]
                public string Description { get; set; }
                
                [JsonProperty("active")]
                public bool Active { get; set; }
                
                [JsonProperty("is_shared")]
                public bool IsShared { get; set; }
            }
        }
    }
}