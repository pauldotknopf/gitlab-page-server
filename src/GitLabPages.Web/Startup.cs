using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitLabPages.Api;
using GitLabPages.Web.Middleware;
using GitLabPages.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace GitLabPages.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IJobArtifactCache, Impl.JobArtifactCache>();
            services.Configure<GitLabPagesOptions>(options => { });
            services.Configure<GitlabApiOptions>(options => { });
            services.AddSingleton<GitlabApi>();
            services.AddScoped<IProjectContext, ProjectContext>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseMvc(routes =>
            {
                routes.MapRoute("hook", "hook", new
                {
                    controller = "Hook",
                    action = "Index"
                });
            });
            
            app.UseProjects(appProjects =>
            {
                appProjects.Run(async (context) =>
                {
                    var projectContext = context.RequestServices.GetRequiredService<IProjectContext>();
                    var api = context.RequestServices.GetRequiredService<GitlabApi>();
                    var jobCache = context.RequestServices.GetRequiredService<IJobArtifactCache>();
  
                    using (var jobCacheSession = await jobCache.GetOrAddArtifacts(api.Projects
                        .Project(projectContext.CurrentProject.Id)
                        .Jobs.Job(projectContext.CurrentJob.Id)))
                    {
                        var fileProvider = new PhysicalFileProvider(Path.Combine(jobCacheSession.Directory, "public"));
                        var newBuild = appProjects.New();
                        newBuild.UseDefaultFiles(new DefaultFilesOptions
                        {
                            FileProvider = fileProvider
                        });
                        newBuild.UseStaticFiles(new StaticFileOptions
                        {
                            FileProvider = fileProvider
                        });
                        var requestMethod = newBuild.Build();
                        await requestMethod.Invoke(context);
                    }
                });
            });
        }
    }
}
