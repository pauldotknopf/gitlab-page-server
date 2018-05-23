namespace GitLabPages.Api
{
    public class GitlabApiOptions
    {
        public GitlabApiOptions()
        {
            Url = "http://192.168.0.6";
            Token = "AdNAuSLZxGvU1cHycNxU";
        }
        
        public string Url { get; set; }
        
        public string Token { get; set; }
    }
}