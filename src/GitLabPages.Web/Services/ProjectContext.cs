using GitLabPages.Api.Types;
using Microsoft.AspNetCore.Http;

namespace GitLabPages.Web.Services
{
    public class ProjectContext : IProjectContext
    {
        readonly IHttpContextAccessor _httpContext;

        public ProjectContext(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        
        public Project CurrentProject => _httpContext.HttpContext.Items.ContainsKey("_currentProject")
            ? (Project)_httpContext.HttpContext.Items["_currentProject"]
            : null;
    }
}