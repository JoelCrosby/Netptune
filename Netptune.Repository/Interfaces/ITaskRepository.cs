using Netptune.Entities.Entites;
using Netptune.Entities.VeiwModels.ProjectTasks;
using Netptune.Repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netptune.Repository.Interfaces
{
    public interface ITaskRepository
    {
        Task<RepoResult<IEnumerable<TaskViewModel>>> GetTasksAsync(int workspaceId);

        Task<RepoResult<ProjectTask>> GetTask(int id);

        Task<RepoResult<TaskViewModel>> UpdateTask(ProjectTask projectTask, AppUser user);

        Task<RepoResult<TaskViewModel>> AddTask(ProjectTask projectTask, AppUser user);

        Task<RepoResult<int>> DeleteTask(int id, AppUser user);

        Task<RepoResult<ProjectTaskCounts>> GetProjectTaskCount(int projectId);
    }
}