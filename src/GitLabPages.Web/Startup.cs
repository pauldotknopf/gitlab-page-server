using System.IO;
using GitLabPages.Api;
using GitLabPages.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

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
            services.Configure<GitlabApiOptions>(Configuration.GetSection("Pages"));
            services.Configure<GitLabPagesOptions>(Configuration.GetSection("Pages"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IJobArtifactCache, Impl.JobArtifactCache>();
            services.AddSingleton<IJobContextResolver, Impl.JobContextResolver>();
            services.Configure<GitLabPagesOptions>(options => { });
            services.Configure<GitlabApiOptions>(options => { });
            services.AddSingleton<GitlabApi>();
            services.AddMvc();
            services.AddMemoryCache();
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
            
            app.UseJobContext(jobApp =>
            {
                // This will serve the jobs as static content
                jobApp.UseJobArtifacts();
            });
        }
    }
}
