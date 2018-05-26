using System;
using System.Threading.Tasks;
using Build.Buildary;
using static Bullseye.Targets;
using static Build.Buildary.Directory;
using static Build.Buildary.Path;
using static Build.Buildary.Shell;
using static Build.Buildary.Runner;
using static Build.Buildary.GitVersion;
using static Build.Buildary.File;

namespace Build
{
    static class Program
    {
        static Task<int> Main(string[] args)
        {
            var options = ParseOptions<Options>(args);
            var gitversion = GetGitVersion("./");
            var dockerUsername = Environment.GetEnvironmentVariable("DOCKER_USERNAME");
            var dockerPassword = Environment.GetEnvironmentVariable("DOCKER_PASSWORD");

            var commandBuildArgs = $"--configuration {options.Configuration}";
            if (!string.IsNullOrEmpty(gitversion.PreReleaseTag))
            {
                commandBuildArgs += $" --version-suffix \"{gitversion.PreReleaseTag}\"";
            }
            
            Add("clean", () =>
            {
                CleanDirectory(ExpandPath("./output"));
            });
            
            Add("build", () =>
            {
                RunShell($"dotnet build GitLabPages.sln {commandBuildArgs}");
            });
            
            Add("test", () =>
            {
                // No tests, sue me.
            });
            
            Add("deploy", DependsOn("clean"), () =>
            {
                // Deploy our nuget packages.
                RunShell($"dotnet pack --output {ExpandPath("./output")} {commandBuildArgs}");
                RunShell($"dotnet publish src/GitLabPages.Web --output {ExpandPath("./output/gitlab-pages/linux-x64")} --runtime linux-x64 {commandBuildArgs}");
                CopyFile("./build/docker/Dockerfile", "./output/Dockerfile");
                RunShell("docker build output --tag pauldotknopf/gitlab-pages:build");
            });

            Add("update-version", () =>
            {
                if (FileExists("./build/version.props"))
                {
                    DeleteFile("./build/version.props");
                }
                
                WriteFile("./build/version.props",
$@"<Project>
    <PropertyGroup>
        <VersionPrefix>{gitversion.Version}</VersionPrefix>
    </PropertyGroup>
</Project>");
            });
            
            Add("publish", () =>
            {
                if(Travis.IsTravis)
                {
                    // If we are on travis, we only want to deploy if this is a release tag.
                    if(Travis.EventType != Travis.EventTypeEnum.Push)
                    {
                        // Only pushes (no cron jobs/api/pull rqeuests) can deploy.
                        Log.Warning("Not a push build, skipping publish...");
                        return;
                    }

                    if(Travis.Branch != "master")
                    {
                        // We aren't on master.
                        Log.Warning("Not on master, skipping publish...");
                        return;
                    }
                }

                if (string.IsNullOrEmpty(dockerUsername))
                {
                    throw new Exception("No DOCKER_USERNAME provided.");
                }

                if (string.IsNullOrEmpty(dockerPassword))
                {
                    throw new Exception("No DOCKER_PASSWORD provided.");
                }

                RunShell($"docker login -u {dockerUsername} -p {dockerPassword}");
                RunShell($"docker tag pauldotknopf/gitlab-pages:build pauldotknopf/gitlab-pages:v{gitversion.FullVersion}");
                RunShell("docker tag pauldotknopf/gitlab-pages:build pauldotknopf/gitlab-pages:latest");
                RunShell($"docker push pauldotknopf/gitlab-pages:v{gitversion.FullVersion}");
                RunShell("docker push pauldotknopf/gitlab-pages:latest");
            });
            
            Add("ci", DependsOn("update-version", "test", "deploy", "publish"));
            
            Add("default", DependsOn("build"));

            return Run(options);
        }

        // ReSharper disable ClassNeverInstantiated.Local
        class Options : RunnerOptions
        // ReSharper restore ClassNeverInstantiated.Local
        {
            [PowerArgs.ArgShortcut("config"), PowerArgs.ArgDefaultValue("Release")]
            public string Configuration { get; set; }
        }
    }
}
