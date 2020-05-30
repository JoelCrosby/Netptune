﻿using Netptune.Core.BaseEntities;
using Netptune.Core.Enums;
using Netptune.Core.Relationships;
using Netptune.Core.ViewModels.ProjectTasks;

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Netptune.Core.Entities
{
    public class ProjectTask : AuditableEntity<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ProjectTaskStatus Status { get; set; }

        public double SortOrder { get; set; }

        #region ForeignKeys

        public string AssigneeId { get; set; }

        public int? ProjectId { get; set; }

        public int? WorkspaceId { get; set; }

        #endregion

        #region NavigationProperties

        [JsonIgnore]
        public AppUser Assignee { get; set; }

        [JsonIgnore]
        public Project Project { get; set; }

        [JsonIgnore]
        public Workspace Workspace { get; set; }

        [JsonIgnore]
        public ICollection<ProjectTaskInBoardGroup> ProjectTaskInBoardGroups { get; set; } = new HashSet<ProjectTaskInBoardGroup>();

        #endregion

        #region Methods

        public TaskViewModel ToViewModel()
        {
            return new TaskViewModel
            {
                Id = Id,
                AssigneeId = Assignee == null ? string.Empty : Assignee.Id,
                OwnerId = OwnerId,
                Name = Name,
                Description = Description,
                Status = Status,
                SortOrder = SortOrder,
                ProjectId = ProjectId,
                WorkspaceId = WorkspaceId,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                AssigneeUsername = Assignee == null ? string.Empty : Assignee.GetDisplayName(),
                AssigneePictureUrl = Assignee == null ? string.Empty : Assignee.PictureUrl,
                OwnerUsername = Owner == null ? string.Empty : Owner.GetDisplayName(),
                ProjectName = Project == null ? string.Empty : Project.Name
            };
        }

        #endregion

    }
}