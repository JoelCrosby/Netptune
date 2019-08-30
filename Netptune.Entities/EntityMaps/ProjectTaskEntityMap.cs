﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Netptune.Entities.Entites;
using Netptune.Entities.EntityMaps.BaseMaps;
using Netptune.Entities.Enums;

namespace Netptune.Entities.EntityMaps
{
    public class ProjectTaskEntityMap : AuditableEntityMap<ProjectTask, int>
    {
        public override void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            base.Configure(builder);

            builder
                .Property(task => task.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder
                .Property(task => task.Description)
                .HasMaxLength(4096);

            builder
                .Property(task => task.Status)
                .HasDefaultValue(ProjectTaskStatus.New)
                .IsRequired();

            builder
                .HasOne(task => task.Assignee)
                .WithMany(user => user.Tasks)
                .HasForeignKey(task => task.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(task => task.Owner)
                .WithOne()
                .HasForeignKey<ProjectTask>(task => task.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}