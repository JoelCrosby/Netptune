using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Netptune.Core.Encoding;
using Netptune.Core.Entities;
using Netptune.Core.Relationships;
using Netptune.Core.Repositories;
using Netptune.Core.Requests;
using Netptune.Core.Services;
using Netptune.Core.UnitOfWork;

namespace Netptune.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly INetptuneUnitOfWork UnitOfWork;
        private readonly IIdentityService IdentityService;
        private readonly IWorkspaceRepository WorkspaceRepository;

        public WorkspaceService(INetptuneUnitOfWork unitOfWork, IIdentityService identityService)
        {
            UnitOfWork = unitOfWork;
            IdentityService = identityService;
            WorkspaceRepository = unitOfWork.Workspaces;
        }

        public async Task<Workspace> AddWorkspace(AddWorkspaceRequest request)
        {
            var user = await IdentityService.GetCurrentUser();

            var workspace = new Workspace
            {
                Name = request.Name,
                Description = request.Description,
                CreatedByUserId = user.Id,
                OwnerId = user.Id,
                Slug = request.Name.ToUrlSlug(),
                MetaInfo = request.MetaInfo
            };

            workspace.WorkspaceUsers.Add(new WorkspaceAppUser
            {
                UserId = user.Id,
                WorkspaceId = workspace.Id
            });

            var result = await WorkspaceRepository.AddAsync(workspace);

            await UnitOfWork.CompleteAsync();

            return result;
        }

        public async Task<Workspace> DeleteWorkspace(int id)
        {
            var workspace = await WorkspaceRepository.GetAsync(id);

            if (workspace is null) return null;

            workspace.IsDeleted = true;

            await UnitOfWork.CompleteAsync();

            return workspace;
        }

        public ValueTask<Workspace> GetWorkspace(int id)
        {
            return WorkspaceRepository.GetAsync(id);
        }

        public Task<Workspace> GetWorkspace(string slug)
        {
            return WorkspaceRepository.GetBySlug(slug);
        }

        public async Task<List<Workspace>> GetWorkspaces()
        {
            var user = await IdentityService.GetCurrentUser();

            return await WorkspaceRepository.GetWorkspaces(user);
        }

        public async Task<Workspace> UpdateWorkspace(Workspace workspace)
        {
            var user = await IdentityService.GetCurrentUser();

            if (workspace is null) throw new ArgumentNullException(nameof(workspace));
            if (user is null) throw new ArgumentNullException(nameof(user));

            var result = await WorkspaceRepository.GetAsync(workspace.Id);

            if (result is null) return null;

            result.Name = workspace.Name;
            result.Description = workspace.Description;
            result.ModifiedByUserId = user.Id;
            result.MetaInfo = workspace.MetaInfo;

            if (workspace.IsDeleted)
            {
                result.IsDeleted = true;
                result.DeletedByUserId = user.Id;
            }

            result.UpdatedAt = DateTime.UtcNow;

            await UnitOfWork.CompleteAsync();

            return result;
        }
    }
}
