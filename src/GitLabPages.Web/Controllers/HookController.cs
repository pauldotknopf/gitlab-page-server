using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        
        public HookController(IOptions<GitLabPagesOptions> options,
            ILogger<HookController> logger)
        {
            _logger = logger;
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
                    var pipelineHook = await DeserializeBody<Api.Hooks.PipelineHook>();
                    _logger.LogInformation("Sending pipeline hook event");
                    Debug.WriteLine(JsonConvert.SerializeObject(pipelineHook, Formatting.Indented));
                    return Ok();
                case "Job Hook":
                    var buildHook = await DeserializeBody<Api.Hooks.BuildHook>();
                    _logger.LogInformation("Sending job hook event");
                    Debug.WriteLine(JsonConvert.SerializeObject(buildHook, Formatting.Indented));
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
    }
}