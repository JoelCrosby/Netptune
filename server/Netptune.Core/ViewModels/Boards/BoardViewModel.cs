using System;

using Netptune.Core.Entities;
using Netptune.Core.Enums;

namespace Netptune.Core.ViewModels.Boards
{
    public class BoardViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public BoardType BoardType { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public string OwnerUsername { get; set; }
    }
}