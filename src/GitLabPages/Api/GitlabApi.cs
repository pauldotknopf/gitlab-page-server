using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GitLabPages.Api.Requests;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GitLabPages.Api
{
    public class GitlabApi
    {
        readonly GitlabApiOptions _options;

        public GitlabApi(IOptions<GitlabApiOptions> options)
        {
            _options = options.Value;
        }

        public IProjects Projects => new Modules.Projects(this);
        
        internal async Task<T> Post<T>(string url, object data)
        {
            using (var client = GetClient())
            {
                using (var input = new StringContent(
                    JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    "application/json"))
                {
                    using (var response = await client.PostAsync($"/api/v4{url}", input))
                    {
                        if(!response.IsSuccessStatusCode)
                            throw new ApiRequestException(string.Empty, response.StatusCode);
                        using (var output = response.Content)
                        {
                            return JsonConvert.DeserializeObject<T>(await output.ReadAsStringAsync());
                        }
                    }
                }
            }
        }
        
        internal async Task<T> Put<T>(string url, object data)
        {
            using (var client = GetClient())
            {
                using (var input = new StringContent(
                    JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    "application/json"))
                {
                    using (var response = await client.PutAsync($"/api/v4{url}", input))
                    {
                        if(!response.IsSuccessStatusCode)
                            throw new ApiRequestException(string.Empty, response.StatusCode);
                        using (var output = response.Content)
                        {
                            return JsonConvert.DeserializeObject<T>(await output.ReadAsStringAsync());
                        }
                    }
                }
            }
        }

        internal async Task GetStream(string url, Func<HttpResponseMessage, Task> streamAction, Dictionary<string, string> queryParameters = null)
        {
            if (queryParameters != null && queryParameters.Count > 0)
            {
                var q = HttpUtility.ParseQueryString(string.Empty);
                foreach (var entry in queryParameters)
                {
                    q.Add(entry.Key, entry.Value);
                }

                url += $"?{q}";
            }
            
            using (var client = GetClient())
            {
                using (var response = await client.GetAsync($"/api/v4{url}"))
                {
                    if(!response.IsSuccessStatusCode)
                        throw new ApiRequestException(string.Empty, response.StatusCode);
                    await streamAction(response);
                }
            }
        }
        
        internal async Task<T> Get<T>(string url, Dictionary<string, string> queryParameters = null)
        {
            if (queryParameters != null && queryParameters.Count > 0)
            {
                var q = HttpUtility.ParseQueryString(string.Empty);
                foreach (var entry in queryParameters)
                {
                    q.Add(entry.Key, entry.Value);
                }

                url += $"?{q}";
            }
            
            using (var client = GetClient())
            {
                using (var response = await client.GetAsync($"/api/v4{url}"))
                {
                    if(!response.IsSuccessStatusCode)
                        throw new ApiRequestException(string.Empty, response.StatusCode);
                    using (var content = response.Content)
                    {
                        return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
                    }
                }
            }
        }
        
        private HttpClient GetClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_options.ServerUrl);
            client.DefaultRequestHeaders.Add("Private-Token", _options.AuthToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }

    public interface IProjects
    {
        IProject Project(int projectId);

        IProject Project(string fullName);
    }

    public interface IProject
    {
        string ProjectId { get; }
        
        Task<Types.Project> Get();

        IPipelines Pipelines { get; }
        
        IJobs Jobs { get; }
    }

    public interface IPipelines
    {
        string ProjectId { get; }

        IPipeline Pipeline(int pipelineId);

        Task<List<Types.Pipeline>> Get(PipelinesRequest request = null);
    }

    public interface IPipeline
    {
        string ProjectId { get; }
        
        int PipelineId { get; }

        Task<Types.Pipeline> Get();

        IJobs Jobs { get; }
    }

    public interface IJobs
    {
        string ProjectId { get; }
        
        int PipelineId { get; }

        Task<List<Types.Job>> Get(JobsRequest request = null);

        IJob Job(int jobId);
    }

    public interface IJob
    {
        string ProjectId { get; }
        
        int PipelineId { get; }
        
        int JobId { get; }

        Task<Types.Job> Get();
        
        Task GetArtifacts(Func<HttpResponseMessage, Task> streamAction);
        
        Task GetArtifact(string path, Func<HttpResponseMessage, Task> streamAction);
    }
}