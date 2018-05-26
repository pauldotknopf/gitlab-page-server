using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GitLabPages.Api;
using GitLabPages.Api.Requests;
using GitLabPages.Api.Types;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace GitLabPages.Impl
{
    public class JobContextResolver : IJobContextResolver
    {
        readonly GitlabApi _api;
        readonly GitLabPagesOptions _options;
        readonly IMemoryCache _cache;
        
        public JobContextResolver(IOptions<GitLabPagesOptions> options,
            GitlabApi api,
            IMemoryCache cache)
        {
            _api = api;
            _options = options.Value;
            _cache = cache;
        }
        
        public async Task<JobContext> ResolveContext(string path)
        {
            var parts = path.Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries);

            Project project = null;
            
            var current = "";
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(current))
                    current += part;
                else
                    current += $"/{part}";

                project = await LookupProjectById(current);
                
                if(project != null) break;
            }

            if (project == null)
            {
                // No path was found. Let's try to treat the path as the root project.
                if(!string.IsNullOrEmpty(_options.RootProject))
                {
                    project = await LookupProjectById(_options.RootProject);
                }
            }

            if (project == null) return null;

            var job = await LookupJobByByProject(project.Id.ToString());

            if (job != null)
            {
                return new JobContext(
                    project.Id,
                    job.Id,
                    $"/{current}",
                    path.Substring(current.Length)
                );
            }
            
            return null;
        }

        async Task<Project> LookupProjectById(string projectId)
        {
            if (_cache.TryGetValue($"project-{projectId}", out Project project))
            {
                return project;
            }
            
            // Let's see if we can find it through the API.
            project = await _api.Projects.Project(projectId).Get();

            return _cache.Set($"project-{projectId}", project, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30)));
        }

        async Task<Job> LookupJobByByProject(string projectId)
        {
            if (_cache.TryGetValue($"job-{projectId}", out Job job))
            {
                return job;
            }
            
            // Let's try to get the latest succesful pipeline.
            var pipeline = (await _api.Projects.Project(projectId)
                .Pipelines.Get(new PipelinesRequest
                {
                    Ref = "master",
                    Status = "success"
                }))
                .FirstOrDefault();

            if (pipeline != null)
            {
                var jobs = await _api.Projects.Project(projectId)
                    .Pipelines.Pipeline(pipeline.Id)
                    .Jobs.Get();

                job = jobs.FirstOrDefault(x => x.Name == "pages");
            }
            
            return _cache.Set($"job-{projectId}", job, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30)));
        }
    }
}