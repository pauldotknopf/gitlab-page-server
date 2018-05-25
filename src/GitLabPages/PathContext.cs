using System.Dynamic;
using GitLabPages.Api.Types;

namespace GitLabPages
{
    public class JobContext
    {
        public JobContext(int projectId,
            int jobId,
            string basePath,
            string artifactPath)
        {
            ProjectId = projectId;
            JobId = jobId;
            BasePath = basePath;
            ArtifactPath = artifactPath;
        }
        
        public int ProjectId { get; }
        
        public int JobId { get; }
        
        public string BasePath { get; set; }
        
        public string ArtifactPath { get; set; }
    }
}