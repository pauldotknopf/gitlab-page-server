using Newtonsoft.Json;

namespace GitLabPages.Api.Types
{
    public class Job
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("stage")]
        public string Stage { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("ref")]
        public string Ref { get; set; }
        
        [JsonProperty("tag")]
        public bool Tag { get; set; }
        
        // TODO: unknown type
        //[JsonProperty("coverage")]
        //public string Coverage { get; set; }
        
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("started_at")]
        public string StartedAt { get; set; }
        
        [JsonProperty("finished_at")]
        public string FinishedAt { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

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
                
                [JsonProperty("username")]
                public string Username { get; set; }
            }
        }

//        "user": {
//            "id": 1,
//            "name": "Administrator",
//            "username": "root",
//            "state": "active",
//            "avatar_url": "https://www.gravatar.com/avatar/e64c7d89f26bd1972efa854d13d7dd61?s=80&d=identicon",
//            "web_url": "http://192.168.0.6/root",
//            "created_at": "2018-05-16T01:08:05.548Z",
//            "bio": null,
//            "location": null,
//            "skype": "",
//            "linkedin": "",
//            "twitter": "",
//            "website_url": "",
//            "organization": null
//        },
//    "commit": {
//    "id": "1e91ebffe90b8b9a4436593d7982225874358937",
//    "short_id": "1e91ebff",
//    "title": "Update config.toml",
//    "created_at": "2018-05-23T18:59:23.000Z",
//    "parent_ids": [
//    "82da40482aeebd6c7ce06aafa8461323d0e0f538"
//    ],
//    "message": "Update config.toml",
//    "author_name": "Administrator",
//    "author_email": "admin@example.com",
//    "authored_date": "2018-05-23T18:59:23.000Z",
//    "committer_name": "Administrator",
//    "committer_email": "admin@example.com",
//    "committed_date": "2018-05-23T18:59:23.000Z"
//},
//"pipeline": {
//"id": 10,
//"sha": "1e91ebffe90b8b9a4436593d7982225874358937",
//"ref": "new-branch",
//"status": "success"
//},
//"artifacts_file": {
//"filename": "artifacts.zip",
//"size": 1901783
//},
//"runner": {
//"id": 1,
//"description": "local-runner",
//"active": true,
//"is_shared": false,
//"name": "gitlab-runner",
//"online": true,
//"status": "online"
//}

    }
}