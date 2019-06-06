﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Netptune.Entities.Entites;
using Netptune.Repositories.Models;

namespace Netptune.Repository.Interfaces
{
    public interface IProjectRepository
    {
        Task<RepoResult<IEnumerable<Project>>> GetProjects(int workspaceId);

        Task<RepoResult<Project>> GetProject(int id);

        Task<RepoResult<Project>> UpdateProject(Project project, AppUser user);

        Task<RepoResult<Project>> AddProject(Project project, AppUser user);

        Task<RepoResult<Project>> DeleteProject(int id);
    }
}
