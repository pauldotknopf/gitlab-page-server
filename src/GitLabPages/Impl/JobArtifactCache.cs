using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using GitLabPages.Api;
using GitLabPages.Api.Modules;
using Microsoft.Extensions.Options;

namespace GitLabPages.Impl
{
    public class JobArtifactCache : IJobArtifactCache
    {
        readonly GitLabPagesOptions _options;
        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public JobArtifactCache(IOptions<GitLabPagesOptions> options)
        {
            _options = options.Value;
            if (!Path.IsPathRooted(_options.ArtifactsCacheDirectory))
            {
                _options.ArtifactsCacheDirectory = Path.GetFullPath(_options.ArtifactsCacheDirectory);
            }
        }
        
        public async Task<IJobArtifactCacheSession> GetOrAddArtifacts(IJob job)
        {
            return await Task.Run<IJobArtifactCacheSession>(async () =>
            {
                await _semaphore.WaitAsync();

                var jobPath = Path.Combine(_options.ArtifactsCacheDirectory, $"{job.ProjectId}-{job.JobId}");
                
                try
                {
                    if (Directory.Exists(jobPath))
                    {
                        _semaphore.Release();
                        return new JobArtifactSession(jobPath);
                    }
                }
                catch (Exception)
                {
                    _semaphore.Release();
                    throw;
                }

                // No artifacts path exists for this job
                try
                {
                    Directory.CreateDirectory(jobPath);

                    try
                    {
                        await job.GetArtifacts(async response =>
                        {
                            using (var responseContent = response.Content)
                            {
                                using (var responseStream = await responseContent.ReadAsStreamAsync())
                                {
                                    using (var archive = new ZipArchive(responseStream, ZipArchiveMode.Read))
                                    {
                                        archive.ExtractToDirectory(jobPath);
                                    }
                                }
                            }
                        });
                        
                        return new JobArtifactSession(jobPath);
                    }
                    catch (Exception ex)
                    {
                        Directory.Delete(jobPath, true);
                        throw;
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }

        private class JobArtifactSession : IJobArtifactCacheSession
        {
            public JobArtifactSession(string directory)
            {
                Directory = directory;
            }

            public string Directory { get; }

            public void Dispose()
            {
            }
        }
    }
}