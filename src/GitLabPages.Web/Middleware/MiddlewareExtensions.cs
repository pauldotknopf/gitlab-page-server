using System;
using Microsoft.AspNetCore.Builder;

namespace GitLabPages.Web.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void UseProjects(this IApplicationBuilder app, Action<IApplicationBuilder> configuration)
        {
            var newApp = app.New();
            configuration(newApp);

            var requestMethod = newApp.Build();
            
            app.UseMiddleware<ProjectMiddleware>(requestMethod);
        }
    }
}