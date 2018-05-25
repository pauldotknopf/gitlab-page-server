using System;
using System.IO;
using System.Threading.Tasks;
using GitLabPages.Api;

namespace GitLabPages
{
    public interface IJobArtifactCache
    {
        Task<IJobArtifactCacheSession> GetOrAddArtifacts(IJob job);
    }

    public delegate Task JobArtifactCacheAddDelegate(Func<Stream, Task> streamCallback);
}