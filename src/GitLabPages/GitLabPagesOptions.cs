namespace GitLabPages
{
    public class GitLabPagesOptions
    {
        public GitLabPagesOptions()
        {
            MaxParentGroups = 1;
            ArtifactsCacheDirectory = "artifacts";
            JobArtifactsBasePath = "public";
            RepositoryBranch = "master";
            BuildJobName = "pages";
        }
        
        public string SecretToken { get; set; }
        
        public string RootProject { get; set; }
        
        public int MaxParentGroups { get; set; }
        
        public string ArtifactsCacheDirectory { get; set; }
        
        public string JobArtifactsBasePath { get; set; }
        
        public string RepositoryBranch { get; set; }
        
        public string BuildJobName { get; set; }
    }
}