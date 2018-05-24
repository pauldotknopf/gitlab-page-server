using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GitLabPages.Api.Requests;
using GitLabPages.Api.Types;
using Newtonsoft.Json;

namespace GitLabPages.Api.Modules
{
    public class Jobs : IJobs
    {
        readonly GitlabApi _api;
        readonly IProject _project;
        readonly IPipeline _pipeline;

        public Jobs(
            GitlabApi api,
            IProject project,
            IPipeline pipeline)
        {
            _api = api;
            _project = project;
            _pipeline = pipeline;
        }

        public string ProjectId => _project.ProjectId;

        public int PipelineId => _pipeline.PipelineId;
        
        public Task<List<Types.Job>> Get(JobsRequest request = null)
        {
            Dictionary<string, string> queryParameters = null;
            
            if (request != null)
            {
                queryParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(request));
            }
            
            return _api.Get<List<Types.Job>>($"/projects/{ProjectId}/pipelines/{_pipeline.PipelineId}/jobs",
                queryParameters);
        }

        public IJob Job(int jobId)
        {
            return new Job(_api, _project, _pipeline, jobId);
        }
    }

    public class Job : IJob
    {
        readonly GitlabApi _api;
        readonly IProject _project;
        readonly IPipeline _pipeline;

        public Job(
            GitlabApi api,
            IProject project,
            IPipeline pipeline,
            int jobId)
        {
            _api = api;
            _project = project;
            _pipeline = pipeline;
            JobId = jobId;
        }

        public string ProjectId => _project.ProjectId;

        public int PipelineId => _pipeline.PipelineId;

        public int JobId { get; }

        public Task<Types.Job> Get()
        {
            return _api.Get<Types.Job>($"/projects/{ProjectId}/jobs/{JobId}");
        }

        public Task GetArtifacts(Func<HttpResponseMessage, Task> streamAction)
        {
            return _api.GetStream($"/projects/{ProjectId}/jobs/{JobId}/artifacts", streamAction);
        }

        public Task GetArtifact(string path, Func<HttpResponseMessage, Task> streamAction)
        {
            if(string.IsNullOrEmpty(path)) throw new ArgumentOutOfRangeException(nameof(path));
            
            if (!path.StartsWith("/"))
                path = $"/{path}";
            
            return _api.GetStream($"/projects/{ProjectId}/jobs/{JobId}/artifacts{path}", streamAction);
        }
    }
}