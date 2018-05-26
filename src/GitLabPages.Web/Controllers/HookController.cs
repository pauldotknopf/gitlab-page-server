using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using GitLabPages.Api.Hooks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace GitLabPages.Web.Controllers
{
    public class HookController : Controller
    {
        readonly GitLabPagesOptions _options;
        readonly ILogger<HookController> _logger;
        readonly IJobContextResolver _jobContextResolver;

        public HookController(IOptions<GitLabPagesOptions> options,
            ILogger<HookController> logger,
            IJobContextResolver jobContextResolver)
        {
            _logger = logger;
            _jobContextResolver = jobContextResolver;
            _options = options.Value;
        }
        
        public async Task<ActionResult> Index()
        {
            if (!string.IsNullOrEmpty(_options.SecretToken))
            {
                // We are validating the sender knows our secret token.
                if (_options.SecretToken != GetToken())
                {
                    _logger.LogWarning("Invalid request, secret token didn't match");
                    return Unauthorized();
                }
            }

            var eventType = GetEventType();
            switch (eventType)
            {
                case "Pipeline Hook":
                    var pipelineHook = await DeserializeBody<PipelineHook>();
                    await HandlePipelineHook(pipelineHook);
                    return Ok();
                case "Job Hook":
                    var buildHook = await DeserializeBody<BuildHook>();
                    await HandleBuildHook(buildHook);
                    return Ok();
                default:
                    _logger.LogWarning($"Unknow event type: \"{eventType}\"");
                    return BadRequest();
            }  
        }

        public string GetEventType()
        {
            return Request.Headers.TryGetValue("X-Gitlab-Event", out StringValues eventTypeTemp)
                ? (string) eventTypeTemp
                : "";
        }

        public string GetToken()
        {
            return Request.Headers.TryGetValue("X-Gitlab-Token", out StringValues eventTypeTemp)
                ? (string) eventTypeTemp
                : "";
        }
        
        private async Task<T> DeserializeBody<T>()
        {
            using (var streamReader = new StreamReader(Request.Body))
            {
                var content = await streamReader.ReadToEndAsync();
                //Debug.WriteLine(content);
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        private Task HandleBuildHook(BuildHook buildHook)
        {
            if (buildHook.BuildName == _options.BuildJobName && buildHook.BuildStatus == "success")
            {
                // We ran a job that we may be using somewhere.
                // TODO:
                // This is fine for smaller installs, but inneficient for larger
                // installs. We should really be removing specific cache entries.
                _jobContextResolver.ClearCache();
            }
            
            return Task.CompletedTask;
        }

        private Task HandlePipelineHook(PipelineHook pipelineHook)
        {
            if (pipelineHook.ObjectAttributes.Status == "success")
            {
                if (pipelineHook.ObjectAttributes.Ref == _options.RepositoryBranch)
                {
                    // This pipeline may be a new static site for the project.
                    // TODO:
                    // This is fine for smaller installs, but inneficient for larger
                    // installs. We should really be removing specific cache entries.
                    _jobContextResolver.ClearCache();
                }
            }
            
            return Task.CompletedTask;
        }
    }
}