using System;
using System.Linq;
using System.Threading.Tasks;
using GitLabPages.Api;
using GitLabPages.Api.Requests;
using GitLabPages.Api.Types;
using GitLabPages.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GitLabPages.Web.Middleware
{
    /// <summary>
    /// This will try to match the incoming request with the first
    /// project it can find with the first parts of the path.
    /// Example: for request /parent-group/another-group/project
    ///  1st /parent-group
    ///  2nd /parent-group/another-group
    ///  3rd /parent-group/another-group/project -- this is matched
    /// </summary>
    public class ProjectMiddleware
    {
        readonly RequestDelegate _next;
        readonly RequestDelegate _action;
        readonly GitlabApi _api;
        readonly GitLabPagesOptions _options;

        public ProjectMiddleware(RequestDelegate next,
            RequestDelegate action,
            GitlabApi api,
            IOptions<GitLabPagesOptions> options)
        {
            _next = next;
            _action = action;
            _api = api;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var parts = context.Request.Path.HasValue
                ? context.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries)
                : new string[] {};

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

            if (project != null)
            {
                context.Items["_currentProject"] = project;
                
                // Let's try to get the latest succesful pipeline.
                var pipeline = (await _api.Projects.Project(project.Id).Pipelines().Get(new PipelinesRequest
                {
                    Ref = "master",
                    Status = "success"
                })).FirstOrDefault();
                
                if(pipeline != null)
                {
                    context.Items["_currentPipeline"] = pipeline;
                    
                    // Try to get the pages job.

                    var jobs = await _api.Projects.Project(project.Id).Pipelines().Pipeline(pipeline.Id).Jobs().Get();

                    var pagesJob = jobs.FirstOrDefault(x => x.Name == "pages");

                    if (pagesJob != null)
                    {
                        context.Items["_currentJob"] = pagesJob;

                        var options = new MapOptions()
                        {
                            Branch = _action,
                            PathMatch = $"/{current}"
                        };
                        
                        await new MapMiddleware(_next, options).Invoke(context);
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}