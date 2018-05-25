using System;
using System.Linq;
using System.Threading.Tasks;
using GitLabPages.Api;
using GitLabPages.Api.Requests;
using GitLabPages.Api.Types;
using Microsoft.Extensions.Options;

namespace GitLabPages.Impl
{
    public class PathContextResolver : IPathContextResolver
    {
        readonly GitlabApi _api;
        readonly GitLabPagesOptions _options;
        
        public PathContextResolver(IOptions<GitLabPagesOptions> options,
            GitlabApi api)
        {
            _api = api;
            _options = options.Value;
        }
        
        public async Task<Tuple<string, PathContext>> ResolveContext(string path)
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

                project = await _api.Projects.Project(current).Get();
                
                if(project != null) break;
            }

            if (project == null)
            {
                // No path was found. Let's try to treat the path as the root project.
                if(!string.IsNullOrEmpty(_options.RootProject))
                {
                    project = await _api.Projects.Project(_options.RootProject).Get();
                }
            }

            if (project == null) return null;
            
            // Let's try to get the latest succesful pipeline.
            var pipeline = (await _api.Projects.Project(project.Id)
                    .Pipelines.Get(new PipelinesRequest
                    {
                        Ref = "master",
                        Status = "success"
                    }))
                .FirstOrDefault();

            if (pipeline == null) return null;

            // Try to get the pages job.

            var jobs = await _api.Projects.Project(project.Id)
                .Pipelines.Pipeline(pipeline.Id)
                .Jobs.Get();

            var pagesJob = jobs.FirstOrDefault(x => x.Name == "pages");

            if (pagesJob != null)
            {
                return new Tuple<string, PathContext>(
                    $"/{current}",
                    new PathContext(
                        project.Id,
                        pipeline.Id,
                        pagesJob.Id)
                );
            }

            return null;
        }
    }
}