using GitLabPages.Api.Types;

namespace GitLabPages.Web.Services
{
    public interface IProjectContext
    {
        Project CurrentProject { get; }
    }
}