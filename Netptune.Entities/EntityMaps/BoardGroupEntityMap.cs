﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Netptune.Entities.EntityMaps.BaseMaps;
using Netptune.Models;

namespace Netptune.Entities.EntityMaps
{
    public class BoardGroupEntityMap : AuditableEntityMap<BoardGroup, int>
    {
        public override void Configure(EntityTypeBuilder<BoardGroup> builder)
        {
            base.Configure(builder);

            builder
                .Property(boardGroup => boardGroup.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder
                .HasOne(boardGroup => boardGroup.Board)
                .WithMany(board => board.BoardGroups)
                .HasForeignKey(board => board.BoardId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}