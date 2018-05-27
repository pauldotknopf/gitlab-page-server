using Serilog.Events;

namespace GitLabPages.Web
{
    public class LoggingOptions
    {
        public LoggingOptions()
        {
            MinimumLevel = LogEventLevel.Information;
        }
        
        public LogEventLevel MinimumLevel { get; set; }
    }
}