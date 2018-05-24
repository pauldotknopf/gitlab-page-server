using GitLabPages.Api.Types;

namespace GitLabPages.Web.Services
{
    public interface IProjectContext
    {
        Project CurrentProject { get; }
        
        Pipeline CurrentPipeline { get; }
        
        Job CurrentJob { get; }
    }
}