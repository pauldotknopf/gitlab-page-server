using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GitLabPages.Api.Types;

namespace GitLabPages.Api.Modules
{
    public class Projects : IProject, IProjects
    {
        readonly GitlabApi _api;

        public Projects(GitlabApi api)
        {
            _api = api;
        }
        
        public string ProjectId { get; private set; }

        public IProject Project(int projectId)
        {
            ProjectId = projectId.ToString();
            return this;
        }

        public IProject Project(string fullName)
        {
            ProjectId = WebUtility.UrlEncode(fullName);
            return this;
        }

        public async Task<Project> Get()
        {
            try
            {
                return await _api.Get<Project>($"/projects/{ProjectId}");
            }
            catch (ApiRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound) return null;
                throw;
            }
        }

        public IPipelines Pipelines()
        {
            return new Pipelines(this, _api);
        }
    }
}