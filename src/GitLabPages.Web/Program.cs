using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GitLabPages.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        static IConfiguration CreateConfiguration()
        {
            using (var tmpHost = WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((_, c) => { c.AddJsonFile("config.json", true); })
                .Configure(app => { })
                .Build())
            {
                return tmpHost.Services.GetRequiredService<IConfiguration>();
            }
        }

        static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var configuration = CreateConfiguration();

            var loggingOptions = new LoggingOptions();
            var section = configuration.GetSection("Logging");
            section.Bind(loggingOptions);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(loggingOptions.MinimumLevel)
                .WriteTo.Console()
                .CreateLogger();
            
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(dispose: true);
                })
                .UseStartup<Startup>();
        }
    }
}
