using System.Collections.Generic;
using System.Threading.Tasks;

using Netptune.Core.Repositories.Common;
using Netptune.Models;

namespace Netptune.Core.Repositories
{
    public interface IWorkspaceRepository : IRepository<Workspace, int>
    {
        Task<IEnumerable<Workspace>> GetWorkspaces(AppUser user);

        Task<Workspace> UpdateWorkspace(Workspace workspace, AppUser user);

        Task<Workspace> AddWorkspace(Workspace workspace, AppUser user);

        Task<Workspace> DeleteWorkspace(int id);
    }
}