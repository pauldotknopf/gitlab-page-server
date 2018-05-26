using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
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
            if (string.IsNullOrEmpty(path) || !path.StartsWith("/"))
            {
                throw new ArgumentOutOfRangeException(nameof(path));
            }
            
            Project project = null;
            // Skip first slash
            var currentIndex = 1;
            var current = "/";
            var currentIteraton = 0;
            while (currentIteraton <= _options.MaxParentGroups && project == null)
            {
                currentIteraton++;
                var nextSlash = path.IndexOf("/", currentIndex + 1, StringComparison.OrdinalIgnoreCase);
                if (nextSlash == -1) break;
                var nextHunk = path.Substring(currentIndex, nextSlash - currentIndex);
                current += nextHunk;
                currentIndex += nextHunk.Length;

                project = await LookupProjectById(current.Substring(1));
            }
            
            if (project == null)
            {
                // No path was found. Let's try to treat the path as the root project.
                if(!string.IsNullOrEmpty(_options.RootProject))
                {
                    project = await LookupProjectById(_options.RootProject);
                    currentIndex = 0;
                }
            }
            
            if (project == null) return null;

            // This is /index.html, or w/e.
            var artifactPath = path.Substring(current.Length);
            
            // Just to see if this is a reference to a specific job's artifacts
            // -\/job\/[0-9]+(?:\/)
            var match = Regex.Match(artifactPath, @"\/-\/job\/([0-9])+(?:\/)");
            if (match.Success)
            {
                var jobId = int.Parse(match.Groups[1].Value);
                current += match.Value.Substring(0, match.Value.Length - 1);
                artifactPath = path.Substring(current.Length);
                return new JobContext(
                    project.Id,
                    jobId,
                    current,
                    artifactPath
                );
            }
            
            var job = await LookupJobByByProject(project.Id.ToString());

            if (job != null)
            {
                return new JobContext(
                    project.Id,
                    job.Id,
                    current,
                    artifactPath
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