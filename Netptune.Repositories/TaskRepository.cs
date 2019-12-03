using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Netptune.Core.Repositories;
using Netptune.Core.Repositories.Common;
using Netptune.Entities.Contexts;
using Netptune.Models;
using Netptune.Models.Enums;
using Netptune.Models.ViewModels.ProjectTasks;
using Netptune.Repositories.Common;

namespace Netptune.Repositories
{
    public class TaskRepository : Repository<DataContext, ProjectTask, int>, ITaskRepository
    {
        public TaskRepository(DataContext context, IDbConnectionFactory connectionFactory)
            : base(context, connectionFactory)
        {
        }

        public async Task<TaskViewModel> GetTaskViewModel(int taskId)
        {
            var task = await Entities
                .Where(x => x.Id == taskId)
                .OrderBy(x => x.SortOrder)
                .Include(x => x.Assignee)
                .Include(x => x.Project)
                .Include(x => x.Owner)
                .FirstOrDefaultAsync();

            if (task is null) return null;

            return new TaskViewModel
            {
                Id = task.Id,
                AssigneeId = task.AssigneeId,
                OwnerId = task.OwnerId,
                Name = task.Name,
                Description = task.Description,
                Status = task.Status,
                SortOrder = task.SortOrder,
                ProjectId = task.ProjectId,
                WorkspaceId = task.WorkspaceId,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                AssigneeUsername = task.Assignee?.GetDisplayName(),
                AssigneePictureUrl = task.Assignee?.GetDisplayName(),
                OwnerUsername = task.Owner?.GetDisplayName(),
                ProjectName = task.Project?.Name
            };
        }

        public async Task<List<TaskViewModel>> GetTasksAsync(int workspaceId)
        {
            var tasks = Entities
                .Where(x => x.Workspace.Id == workspaceId && !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .Include(x => x.Assignee)
                .Include(x => x.Project)
                .Include(x => x.Owner);

            return await tasks
                .Select(r => new TaskViewModel
                {
                    Id = r.Id,
                    AssigneeId = r.Assignee == null ? string.Empty : r.Assignee.Id,
                    OwnerId = r.OwnerId,
                    Name = r.Name,
                    Description = r.Description,
                    Status = r.Status,
                    SortOrder = r.SortOrder,
                    ProjectId = r.ProjectId,
                    WorkspaceId = r.WorkspaceId,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    AssigneeUsername = r.Assignee == null ? string.Empty : r.Assignee.GetDisplayName(),
                    AssigneePictureUrl = r.Assignee == null ? string.Empty : r.Assignee.GetDisplayName(),
                    OwnerUsername = r.Owner == null ? string.Empty : r.Owner.GetDisplayName(),
                    ProjectName = r.Project == null ? string.Empty : r.Project.Name
                }).ToListAsync();
        }

        public async Task<ProjectTask> AddTask(ProjectTask projectTask, AppUser user)
        {
            if (projectTask is null) throw new ArgumentNullException(nameof(projectTask));

            if (projectTask.ProjectId is null) throw new ArgumentNullException(nameof(projectTask.ProjectId));

            if (user is null) throw new ArgumentNullException(nameof(user));

            // Load the relationship tables.
            Context.ProjectTasks.Include(m => m.Workspace).ThenInclude(e => e.Projects);

            var relational = await (from w in Context.Workspaces
                                    join p in Context.Projects
                                    on projectTask.ProjectId equals p.Id
                                    where
                                    w.Id == projectTask.WorkspaceId
                                    select new
                                    {
                                        project = p,
                                        workspace = w
                                    }).FirstOrDefaultAsync();

            if (relational is null) return null;

            Context.AppUsers.Include(x => x.WorkspaceUsers).ThenInclude(x => x.Workspace);

            projectTask.WorkspaceId = relational.workspace.Id;
            projectTask.ProjectId = relational.project.Id;
            projectTask.AssigneeId = user.Id;
            projectTask.OwnerId = user.Id;
            projectTask.CreatedByUserId = user.Id;

            var result = await Entities.AddAsync(projectTask);

            return result.Entity;
        }

        public async Task<ProjectTaskCounts> GetProjectTaskCount(int projectId)
        {
            var tasks = await Entities
                .Where(x => x.ProjectId == projectId && !x.IsDeleted)
                .ToListAsync();

            return new ProjectTaskCounts
            {
                AllTasks = tasks.Count,
                CompletedTasks = tasks.Count(x => x.Status == ProjectTaskStatus.Complete),
                InProgressTasks = tasks.Count(x => x.Status == ProjectTaskStatus.InProgress),
                BacklogTasks = tasks.Count(x => x.Status == ProjectTaskStatus.UnAssigned)
            };
        }
    }
}