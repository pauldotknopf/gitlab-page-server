using System;

namespace GitLabPages
{
    public interface IJobArtifactCacheSession : IDisposable
    {
        string Directory { get; }
    }
}