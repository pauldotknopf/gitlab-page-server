using System.Dynamic;
using GitLabPages.Api.Types;

namespace GitLabPages
{
    public class PathContext
    {
        public PathContext(int projectId,
            int jobId)
        {
            ProjectId = projectId;
            JobId = jobId;
        }
        
        public int ProjectId { get; }
        
        public int JobId { get; }
    }
}