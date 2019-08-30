﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Netptune.Entities.Entites.Relationships;
using Netptune.Entities.EntityMaps.BaseMaps;

namespace Netptune.Entities.EntityMaps
{
    public class WorkspaceProjectEntityMap : KeyedEntityMap<WorkspaceProject, int>
    {
        public override void Configure(EntityTypeBuilder<WorkspaceProject> builder)
        {
            base.Configure(builder);

            // (Many-to-many) Workspace > Project

            builder
                .HasKey(workspaceProject => new { workspaceProject.WorkspaceId, workspaceProject.ProjectId });

            builder
                .HasOne(workspaceProject => workspaceProject.Workspace)
                .WithMany(workspace => workspace.WorkspaceProjects)
                .HasForeignKey(workspaceProject => workspaceProject.WorkspaceId);

            builder
                .HasOne(workspaceProject => workspaceProject.Project)
                .WithMany(project => project.WorkspaceProjects)
                .HasForeignKey(workspaceProject => workspaceProject.ProjectId);
        }
    }
}