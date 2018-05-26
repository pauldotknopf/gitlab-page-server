using System;
using System.Threading.Tasks;
using GitLabPages.Api.Types;

namespace GitLabPages
{
    public interface IJobContextResolver
    {
        Task<JobContext> ResolveContext(string path);

        void ClearCache();
    }
}