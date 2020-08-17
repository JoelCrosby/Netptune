using System.Collections.Generic;
using System.Threading.Tasks;

using Netptune.Core.Entities;
using Netptune.Core.Repositories.Common;

namespace Netptune.Core.Repositories
{
    public interface IWorkspaceRepository : IRepository<Workspace, int>
    {
        Task<Workspace> GetBySlug(string slug, bool isReadonly = false);

        Task<Workspace> GetBySlugWithTasks(string slug, bool includeRelated, bool isReadonly = false);

        Task<List<Workspace>> GetWorkspaces(AppUser user);

        Task<bool> Exists(string slug);
    }
}
