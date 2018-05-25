using System.Dynamic;
using GitLabPages.Api.Types;

namespace GitLabPages
{
    public class PathContext
    {
        public PathContext(int projectId,
            int pipelineId,
            int jobId)
        {
            ProjectId = projectId;
            PipelineId = pipelineId;
            JobId = jobId;
        }
        
        public int ProjectId { get; }
        
        public int PipelineId { get; }
        
        public int JobId { get; }
    }
}