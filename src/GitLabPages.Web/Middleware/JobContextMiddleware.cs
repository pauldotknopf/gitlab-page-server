using System;
using System.Linq;
using System.Threading.Tasks;
using GitLabPages.Api;
using GitLabPages.Api.Requests;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http;
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
    public class JobContextMiddleware
    {
        readonly RequestDelegate _next;
        readonly RequestDelegate _action;
        readonly IJobContextResolver _jobContextResolver;

        public JobContextMiddleware(RequestDelegate next,
            RequestDelegate action,
            IJobContextResolver jobContextResolver)
        {
            _next = next;
            _action = action;
            _jobContextResolver = jobContextResolver;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var jobContext = await _jobContextResolver.ResolveContext(
                context.Request.Path);

            if (jobContext == null)
            {
                await _next(context);
                return;
            }

            context.Items["_jobContext"] = jobContext;
            
            var options = new MapOptions
            {
                Branch = _action,
                PathMatch = jobContext.BasePath == "/" ? "" : jobContext.BasePath
            };
                        
            await new MapMiddleware(_next, options).Invoke(context);
        }
    }
}