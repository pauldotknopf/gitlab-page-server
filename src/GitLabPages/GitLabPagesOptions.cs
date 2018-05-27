namespace GitLabPages
{
    public class GitLabPagesOptions
    {
        public GitLabPagesOptions()
        {
            AdditionalParentGroups = 0; /* By default, only "group/project" will be resolved. */
            ArtifactsCacheDirectory = "artifacts";
            JobArtifactsBasePath = "public";
            RepositoryBranch = "master";
            BuildJobName = "pages";
            CacheProjectType = Types.CacheType.Sliding;
            CacheProjectSeconds = 60;
            CachePipelineType = Types.CacheType.Sliding;
            CachePipelineSeconds = 60;
            CacheJobType = Types.CacheType.Sliding;
            CacheJobSeconds = 60;
        }
        
        public string RootProject { get; set; }
        
        public int AdditionalParentGroups { get; set; }
        
        public string ArtifactsCacheDirectory { get; set; }
        
        public string JobArtifactsBasePath { get; set; }
        
        public string RepositoryBranch { get; set; }
        
        public string BuildJobName { get; set; }

        public Types.CacheType CacheProjectType { get; set; }
        
        public uint CacheProjectSeconds { get; set; }
        
        public Types.CacheType CachePipelineType { get; set; }
        
        public uint CachePipelineSeconds { get; set; }
        
        public Types.CacheType CacheJobType { get; set; }
        
        public uint CacheJobSeconds { get; set; }
        
        public class Types
        {
            public enum CacheType
            {
                None,
                Sliding,
                Absolute
            }
        }
    }
}