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
    public class ProjectMiddleware
    {
        readonly RequestDelegate _next;
        readonly RequestDelegate _action;
        readonly GitlabApi _api;
        readonly IPathContextResolver _pathContextResolver;
        readonly GitLabPagesOptions _options;

        public ProjectMiddleware(RequestDelegate next,
            RequestDelegate action,
            GitlabApi api,
            IPathContextResolver pathContextResolver,
            IOptions<GitLabPagesOptions> options)
        {
            _next = next;
            _action = action;
            _api = api;
            _pathContextResolver = pathContextResolver;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var pathContext = await _pathContextResolver.ResolveContext(
                context.Request.Path);

            if (pathContext == null)
            {
                await _next(context);
                return;
            }

            context.Items["_pathContext"] = pathContext.Item2;
            
            var options = new MapOptions
            {
                Branch = _action,
                PathMatch = pathContext.Item1
            };
                        
            await new MapMiddleware(_next, options).Invoke(context);
        }
    }
}