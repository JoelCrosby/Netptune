﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Netptune.Core.Repositories;
using Netptune.Core.Repositories.Common;
using Netptune.Entities.Contexts;
using Netptune.Models;
using Netptune.Repositories.Common;

namespace Netptune.Repositories
{
    public class BoardRepository : Repository<DataContext, Board, int>, IBoardRepository
    {
        public BoardRepository(DataContext dataContext, IDbConnectionFactory connectionFactories)
            : base(dataContext, connectionFactories)
        {
        }

        public Task<List<Board>> GetBoardsInProject(int projectId)
        {
            return Entities
                .Where(board => board.ProjectId == projectId)
                .Where(board => !board.IsDeleted)
                .ToListAsync();
        }

        public async Task<Board> DeleteBoard(int id, AppUser user)
        {
            var board = await Entities.FindAsync(id);

            if (board is null) return null;

            board.IsDeleted = true;
            board.DeletedByUserId = user.Id;

            return board;
        }
    }
}