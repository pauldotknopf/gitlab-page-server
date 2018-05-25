namespace GitLabPages
{
    public class GitLabPagesOptions
    {
        public GitLabPagesOptions()
        {
            MaxParentGroups = 1;
            ArtifactsCacheDirectory = "artifacts";
        }
        
        public string SecretToken { get; set; }
        
        public string RootProject { get; set; }
        
        public int MaxParentGroups { get; set; }
        
        public string ArtifactsCacheDirectory { get; set; }
    }
}