using System;
using System.Collections.Generic;
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
            services.Configure<GitLabPagesOptions>(options => { });
            services.Configure<GitlabApiOptions>(options => { });
            services.AddSingleton<GitlabApi>();
            services.AddScoped<Services.IProjectContext, Services.ProjectContext>();
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
                    var currentProject = context.RequestServices.GetRequiredService<IProjectContext>().CurrentProject;
                    await context.Response.WriteAsync(currentProject.Id.ToString());
                });
            });
        }
    }
}
