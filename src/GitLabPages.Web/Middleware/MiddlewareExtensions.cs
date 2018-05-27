using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace GitLabPages.Web.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void UseJobContext(this IApplicationBuilder app, Action<IApplicationBuilder> configuration)
        {
            var newApp = app.New();
            configuration(newApp);

            var requestMethod = newApp.Build();
            
            app.UseMiddleware<JobContextMiddleware>(requestMethod);
        }

        public static void UseJobArtifacts(this IApplicationBuilder app)
        {
            app.UseMiddleware<JobArtifactsMiddleware>(app);
        }

        public static void RunNotFound(this IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Not found");
            });
        }
    }
}