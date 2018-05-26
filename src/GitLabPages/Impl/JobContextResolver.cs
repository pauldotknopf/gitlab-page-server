using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GitLabPages.Api;
using GitLabPages.Api.Requests;
using GitLabPages.Api.Types;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace GitLabPages.Impl
{
    public class JobContextResolver : IJobContextResolver
    {
        readonly GitlabApi _api;
        readonly GitLabPagesOptions _options;
        readonly IMemoryCache _cache;
        CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

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
            var maxIterations = 2;
            if (_options.AdditionalParentGroups > 0)
            {
                maxIterations += _options.AdditionalParentGroups;
            }
            while (currentIteraton < maxIterations && project == null)
            {
                currentIteraton++;
                var nextSlash = path.IndexOf("/", currentIndex + 1, StringComparison.OrdinalIgnoreCase);
                if (nextSlash == -1) break;
                var nextHunk = path.Substring(currentIndex, nextSlash - currentIndex);
                current += nextHunk;
                currentIndex += nextHunk.Length;

                if(currentIteraton != 1) // Don't lookup for glob, it's just a group.
                    project = await LookupProjectById(current.Substring(1));
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

            // This is /index.html, or w/e.
            var artifactPath = path.Substring(current.Length);
            
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
            
            match = Regex.Match(artifactPath, @"\/-\/pipeline\/([0-9])+(?:\/)");
            if (match.Success)
            {
                var pipelineId = int.Parse(match.Groups[1].Value);
                current += match.Value.Substring(0, match.Value.Length - 1);
                artifactPath = path.Substring(current.Length);
                var pipelineJob = await LookupJobByPipelineId(project.Id.ToString(), pipelineId);
                if (pipelineJob != null)
                {
                    return new JobContext(
                        project.Id,
                        pipelineJob.Id,
                        current,
                        artifactPath
                    );
                }
            }

            var pipeline = await LookupPipelineByProjectId(project.Id.ToString());

            if (pipeline == null) return null;

            var job = await LookupJobByPipelineId(project.Id.ToString(), pipeline.Id);
            
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

        public string LookupProjectByIdCacheKey(string projectId)
        {
            return $"project-{projectId}";
        }
        
        public async Task<Project> LookupProjectById(string projectId)
        {
            var key = LookupProjectByIdCacheKey(projectId);
            
            if (_cache.TryGetValue(key, out Project project))
            {
                return project;
            }
            
            // Let's see if we can find it through the API.
            project = await _api.Projects.Project(projectId).Get();

            return _cache.Set(key, project, BuildMemoryCacheOptions());
        }

        public string LookupPipelineByProjectIdCacheKey(string projectId)
        {
            return $"pipeline-for-project-{projectId}";
        }
        
        public async Task<Pipeline> LookupPipelineByProjectId(string projectId)
        {
            var key = LookupPipelineByProjectIdCacheKey(projectId);
            
            if (_cache.TryGetValue(key, out Pipeline pipeline))
            {
                return pipeline;
            }
            
            // Let's try to get the latest succesful pipeline.
            pipeline = (await _api.Projects.Project(projectId)
                    .Pipelines.Get(new PipelinesRequest
                    {
                        Ref = _options.RepositoryBranch,
                        Status = "success"
                    }))
                .FirstOrDefault();

            return _cache.Set(key, pipeline, BuildMemoryCacheOptions());
        }

        public string LookupJobByPipelineIdCacheKey(string projectId, int pipelineId)
        {
            return $"job-for-pipeline-{projectId}-{pipelineId}";
        }
        
        public async Task<Job> LookupJobByPipelineId(string projectId, int pipelineId)
        {
            var key = LookupJobByPipelineIdCacheKey(projectId, pipelineId);
            
            if (_cache.TryGetValue(key, out Job job))
            {
                return job;
            }
            
            var jobs = await _api.Projects.Project(projectId)
                .Pipelines.Pipeline(pipelineId)
                .Jobs.Get(new JobsRequest
                {
                    Scope = "success"
                });

            job = jobs.LastOrDefault(x => x.Name == _options.BuildJobName);

            return _cache.Set(key, job, BuildMemoryCacheOptions());
        }
        
        public void ClearCache()
        {
            var resetToken = _resetCacheToken;
            _resetCacheToken = new CancellationTokenSource();
            
            if (resetToken != null && !resetToken.IsCancellationRequested && resetToken.Token.CanBeCanceled)
            {
                resetToken.Cancel();
                resetToken.Dispose();
            }
        }

        private MemoryCacheEntryOptions BuildMemoryCacheOptions()
        {
            return new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
        }
    }
}