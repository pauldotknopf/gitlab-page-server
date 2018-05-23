namespace GitLabPages
{
    public class GitLabPagesOptions
    {
        public GitLabPagesOptions()
        {
            MaxParentGroups = 1;
        }
        
        public string SecretToken { get; set; }
        
        public string RootProject { get; set; }
        
        public int MaxParentGroups { get; set; }
    }
}