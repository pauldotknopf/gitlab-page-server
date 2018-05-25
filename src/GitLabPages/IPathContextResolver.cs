using System;
using System.Threading.Tasks;

namespace GitLabPages
{
    public interface IPathContextResolver
    {
        Task<Tuple<string, PathContext>> ResolveContext(string path);
    }
}