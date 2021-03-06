﻿using System.ComponentModel.DataAnnotations;
using Netptune.Models.Enums;

namespace Netptune.Models.Requests
{
    public class AddProjectTaskRequest
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ProjectTaskStatus? Status { get; set; }

        public int ProjectId { get; set; }

        public double? SortOrder { get; set; }

        public string Workspace { get; set; }

        public AppUser Assignee { get; set; }

        public string AssigneeId { get; set; }
    }
}
