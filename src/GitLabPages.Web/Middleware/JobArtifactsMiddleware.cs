using System.IO;
using System.Threading.Tasks;
using GitLabPages.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace GitLabPages.Web.Middleware
{
    public class JobArtifactsMiddleware
    {
        readonly RequestDelegate _next;
        readonly IApplicationBuilder _app;
        readonly GitlabApi _api;
        readonly IJobArtifactCache _jobArtifactCache;
        readonly GitLabPagesOptions _options;

        public JobArtifactsMiddleware(RequestDelegate next,
            IApplicationBuilder app,
            GitlabApi api,
            IJobArtifactCache jobArtifactCache,
            IOptions<GitLabPagesOptions> options)
        {
            _next = next;
            _app = app;
            _api = api;
            _jobArtifactCache = jobArtifactCache;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var jobContext = (JobContext)context.Items["_jobContext"];

            if (jobContext == null)
            {
                await _next(context);
                return;
            }
            
            using (var jobCacheSession = await _jobArtifactCache.GetOrAddArtifacts(
                _api.Projects
                .Project(jobContext.ProjectId)
                .Jobs.Job(jobContext.JobId)))
            {
                var path = jobCacheSession.Directory;
                
                if (!string.IsNullOrEmpty(_options.JobArtifactsBasePath))
                {
                    path = Path.Combine(path, _options.JobArtifactsBasePath);
                }

                var app = _app.New();
                
                var fileProvider = new PhysicalFileProvider(path);
                app.UseDefaultFiles(new DefaultFilesOptions
                {
                    FileProvider = fileProvider
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = fileProvider
                });
                
                // If no static file is served, run the next middleware.
                app.Run(async _ =>
                {
                    await _next(_);
                });
                
                var requestMethod = app.Build();
                await requestMethod.Invoke(context);
            }
        }
    }
}