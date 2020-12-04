using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Flurl;
using Netptune.Core.Cache;
using Netptune.Core.Entities;
using Netptune.Core.Messaging;
using Netptune.Core.Models.Messaging;
using Netptune.Core.Repositories;
using Netptune.Core.Responses.Common;
using Netptune.Core.Services;
using Netptune.Core.UnitOfWork;
using Netptune.Core.ViewModels.Users;

namespace Netptune.Services
{
    public class UserService : IUserService
    {
        private readonly INetptuneUnitOfWork UnitOfWork;
        private readonly IIdentityService Identity;
        private readonly IEmailService Email;
        private readonly IHostingService Hosting;
        private readonly IWorkspaceUserCache WorkspaceUserCache;
        private readonly IUserRepository UserRepository;
        private readonly IWorkspaceRepository WorkspaceRepository;

        public UserService(
            INetptuneUnitOfWork unitOfWork,
            IIdentityService identity,
            IEmailService email,
            IHostingService hosting,
            IWorkspaceUserCache workspaceUserCache)
        {
            UnitOfWork = unitOfWork;
            Identity = identity;
            Email = email;
            Hosting = hosting;
            WorkspaceUserCache = workspaceUserCache;
            UserRepository = unitOfWork.Users;
            WorkspaceRepository = unitOfWork.Workspaces;
        }

        public async Task<UserViewModel> Get(string userId)
        {
            var user = await UserRepository.GetAsync(userId, true);

            return user?.ToViewModel();
        }

        public async Task<UserViewModel> GetByEmail(string email)
        {
            var user = await UserRepository.GetByEmail(email, true);

            return user?.ToViewModel();
        }

        public async Task<List<UserViewModel>> GetWorkspaceUsers()
        {
            var workspaceKey = Identity.GetWorkspaceKey();
            var users = await UserRepository.GetWorkspaceUsers(workspaceKey, true);

            return MapUsers(users);
        }

        public async Task<List<UserViewModel>> GetAll()
        {
            var users = await UserRepository.GetAllAsync();

            return MapUsers(users);
        }

        public Task<ClientResponse> InviteUserToWorkspace(string userId)
        {
            return InviteUsersToWorkspace(new[] { userId });
        }

        public async Task<ClientResponse> InviteUsersToWorkspace(
            IEnumerable<string> emailAddresses, bool onlyNewUsers = false)
        {
            var workspaceKey = Identity.GetWorkspaceKey();

            var emailList = emailAddresses.ToList();
            var workspace = await WorkspaceRepository.GetBySlug(workspaceKey, true);

            if (workspace is null)
            {
                return ClientResponse.Failed("workspace not found");
            }

            if (emailList.Count == 0)
            {
                return ClientResponse.Failed("no email addresses provided");
            }

            var users = await UserRepository.GetByEmailRange(emailList, true);

            if (users.Count == 0)
            {
                return ClientResponse.Failed("user not found");
            }

            var userIds = users.Select(user => user.Id).ToList();
            var existing = await UserRepository.IsUserInWorkspaceRange(userIds, workspace.Id);

            var newUserIds = userIds.Except(existing).ToList();

            var result = await UserRepository.InviteUsersToWorkspace(newUserIds, workspace.Id);

            if (result is null) throw new Exception();

            await UnitOfWork.CompleteAsync();

            var usersToInvite = onlyNewUsers ? users.Where(user => newUserIds.Contains(user.Id)).ToList() : users;

            await SendUserInviteEmails(usersToInvite, workspace);

            return ClientResponse.Success();
        }

        public async Task<ClientResponse> RemoveUsersFromWorkspace(IEnumerable<string> emailAddresses)
        {
            var workspaceKey = Identity.GetWorkspaceKey();

            var emailList = emailAddresses.ToList();
            var workspace = await WorkspaceRepository.GetBySlug(workspaceKey, true);

            if (workspace is null)
            {
                return ClientResponse.Failed("workspace not found");
            }

            if (emailList.Count == 0)
            {
                return ClientResponse.Failed("no email addresses provided");
            }

            var users = await UserRepository.GetByEmailRange(emailList, true);
            var userIds = users.Select(user => user.Id).ToList();

            foreach (var userId in userIds)
            {
                WorkspaceUserCache.Remove(new WorkspaceUserKey
                {
                    UserId = userId,
                    WorkspaceKey = workspaceKey
                });
            }

            await UserRepository.RemoveUsersFromWorkspace(userIds, workspace.Id);

            await UnitOfWork.CompleteAsync();

            return ClientResponse.Success();
        }

        public async Task<UserViewModel> Update(AppUser user)
        {
            var updatedUser = await UserRepository.GetAsync(user.Id);

            if (updatedUser is null) return null;

            updatedUser.PhoneNumber = user.PhoneNumber;
            updatedUser.Firstname = user.Firstname;
            updatedUser.Lastname = user.Lastname;
            updatedUser.PictureUrl = user.PictureUrl;

            await UnitOfWork.CompleteAsync();

            return updatedUser.ToViewModel();
        }

        private Task SendUserInviteEmails(IEnumerable<AppUser> users, Workspace workspace)
        {
            var emailModels = users.Select(user =>
            {
                var uri = Hosting.ClientOrigin
                    .AppendPathSegments("app", workspace.Slug)
                    .SetQueryParam("userId", user.Id, true)
                    .SetQueryParam("refer", "invite");

                return new SendEmailModel
                {
                    SendTo = new SendTo
                    {
                        Address = user.Email,
                        DisplayName = user.DisplayName,
                    },
                    Name = user.Firstname,
                    Action = "Go to Workspace",
                    Link = uri,
                    Reason = "workspace invite",
                    Message = $"Hi you've been invited to join the {workspace.Name} workspace in Netptune.",
                    Subject = "Netptune workspace invitation.",
                    PreHeader = $"Hi you've been invited to join the {workspace.Name} in Netptune.",
                    RawTextContent = $"Hi you've been invited to join the {workspace.Name} in Netptune. Click the link below to start. \n\n {uri}"
                };
            });

            return Email.Send(emailModels);
        }

        private static List<UserViewModel> MapUsers(IEnumerable<AppUser> users)
        {
            return users.Select(user => user.ToViewModel()).ToList();
        }
    }
}
