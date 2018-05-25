using System;
using System.Threading.Tasks;

namespace GitLabPages
{
    public interface IJobContextResolver
    {
        Task<JobContext> ResolveContext(string path);
    }
}