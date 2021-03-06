﻿using System;
using Netptune.Models.Enums;

namespace Netptune.Models.ViewModels.ProjectTasks
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        public string AssigneeId { get; set; }

        public string OwnerId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ProjectTaskStatus Status { get; set; }

        public double SortOrder { get; set; }

        public int? ProjectId { get; set; }

        public int? WorkspaceId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public string AssigneeUsername { get; set; }

        public string AssigneePictureUrl { get; set; }

        public string OwnerUsername { get; set; }

        public string ProjectName { get; set; }
    }
}
