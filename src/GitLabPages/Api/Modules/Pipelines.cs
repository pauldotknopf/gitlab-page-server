using System.Collections.Generic;
using System.Threading.Tasks;
using GitLabPages.Api.Requests;
using GitLabPages.Api.Types;
using Newtonsoft.Json;

namespace GitLabPages.Api.Modules
{
    public class Pipelines : IPipelines
    {
        readonly IProject _project;
        readonly GitlabApi _api;
        
        public Pipelines(IProject project, GitlabApi api)
        {
            _project = project;
            _api = api;
        }

        public string ProjectId => _project.ProjectId;
        
        public IPipeline Pipeline(int pipelineId)
        {
            return new Pipeline(_project, pipelineId, _api);
        }

        public Task<List<Types.Pipeline>> Get(PipelinesRequest request = null)
        {
            Dictionary<string, string> queryParameters = null;
            
            if (request != null)
            {
                queryParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(request));
            }
            
            return _api.Get<List<Types.Pipeline>>($"/projects/{ProjectId}/pipelines", queryParameters);
        }
    }

    public class Pipeline : IPipeline
    {
        private readonly IProject _project;
        private readonly GitlabApi _api;

        public Pipeline(IProject project, int pipelineId, GitlabApi api)
        {
            _project = project;
            _api = api;
            PipelineId = pipelineId;
        }

        public string ProjectId => _project.ProjectId;

        public int PipelineId { get; }

        public Task<Types.Pipeline> Get()
        {
            return _api.Get<Types.Pipeline>($"/projects/{ProjectId}/pipelines/{PipelineId}");
        }

        public IJobs Jobs => new Jobs(_api, _project, this);
    }
}