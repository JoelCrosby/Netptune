using Netptune.Core.BaseEntities;
using Netptune.Core.Enums;
using Netptune.Core.Relationships;
using Netptune.Core.ViewModels.ProjectTasks;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Netptune.Core.Entities
{
    public class ProjectTask : WorkspaceEntity<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ProjectTaskStatus Status { get; set; }

        public int ProjectScopeId { get; set; }

        public bool IsFlagged { get; set; }

        #region ForeignKeys

        public string AssigneeId { get; set; }

        public int? ProjectId { get; set; }

        #endregion

        #region NavigationProperties

        [JsonIgnore]
        public AppUser Assignee { get; set; }

        [JsonIgnore]
        public Project Project { get; set; }

        [JsonIgnore]
        public ICollection<ProjectTaskInBoardGroup> ProjectTaskInBoardGroups { get; set; } = new HashSet<ProjectTaskInBoardGroup>();

        [JsonIgnore]
        public ICollection<ProjectTaskTag> ProjectTaskTags { get; set; } = new HashSet<ProjectTaskTag>();

        [JsonIgnore]
        public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

        #endregion

        #region Methods

        public TaskViewModel ToViewModel()
        {
            return new()
            {
                Id = Id,
                AssigneeId = Assignee == null ? string.Empty : Assignee.Id,
                OwnerId = OwnerId,
                Name = Name,
                Description = Description,
                Status = Status,
                ProjectScopeId = ProjectScopeId,
                SystemId = $"{Project.Key}-{ProjectScopeId}",
                IsFlagged = IsFlagged,
                ProjectId = ProjectId,
                WorkspaceId = WorkspaceId,
                WorkspaceKey = Workspace.Slug,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                AssigneeUsername = Assignee == null ? string.Empty : Assignee.DisplayName,
                AssigneePictureUrl = Assignee == null ? string.Empty : Assignee.PictureUrl,
                OwnerUsername = Owner == null ? string.Empty : Owner.DisplayName,
                OwnerPictureUrl = Owner == null ? string.Empty : Owner.PictureUrl,
                ProjectName = Project == null ? string.Empty : Project.Name,
                Tags = Tags.Select(x => x.Name).OrderBy(x => x).ToList(),
            };
        }

        public ExportTaskViewModel ToExportViewModel()
        {
            return new()
            {
                Name = Name,
                Description = Description,
                Status = Status.ToString(),
                SystemId = $"{Project.Key}-{ProjectScopeId}",
                IsFlagged = IsFlagged,
                Board = Workspace.Slug,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                Assignee = Assignee == null ? string.Empty : Assignee.Email,
                Owner = Owner == null ? string.Empty : Owner.Email,
                Project = Project == null ? string.Empty : Project.Name,
                Group = ProjectTaskInBoardGroups.FirstOrDefault()?.BoardGroup?.Name,
            };
        }

        #endregion
    }
}
