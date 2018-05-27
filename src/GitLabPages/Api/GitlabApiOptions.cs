namespace GitLabPages.Api
{
    public class GitlabApiOptions
    {
        public GitlabApiOptions()
        {
        }
        
        public string ServerUrl { get; set; }
        
        public string AuthToken { get; set; }
        
        public string HookToken { get; set; }
    }
}