using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GitLabPages.Web.Middleware
{
    public class JobServerMiddleware
    {
        private readonly RequestDelegate _next;

        public JobServerMiddleware(string projectId, string jobId, RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            return _next(context);
        }
    }
}