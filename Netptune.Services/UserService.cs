using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Flurl;

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
        private readonly IMapper Mapper;
        private readonly IEmailService Email;
        private readonly IHostingService Hosting;
        private readonly IUserRepository UserRepository;
        private readonly IWorkspaceRepository WorkspaceRepository;

        public UserService(
            INetptuneUnitOfWork unitOfWork,
            IMapper mapper,
            IEmailService email,
            IHostingService hosting)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            Email = email;
            Hosting = hosting;
            UserRepository = unitOfWork.Users;
            WorkspaceRepository = unitOfWork.Workspaces;
        }

        public async Task<UserViewModel> Get(string userId)
        {
            var user = await UserRepository.GetAsync(userId);

            if (user is null) return null;

            return MapUser(user);
        }

        public async Task<UserViewModel> GetByEmail(string email)
        {
            var user = await UserRepository.GetByEmail(email);

            if (user is null) return null;

            return MapUser(user);
        }

        public async Task<List<UserViewModel>> GetWorkspaceUsers(string workspaceSlug)
        {
            var users = await UserRepository.GetWorkspaceUsers(workspaceSlug);

            return MapUsers(users);
        }

        public Task<ClientResponse> InviteUserToWorkspace(string userId, string workspaceSlug)
        {
            return InviteUsersToWorkspace(new[] { userId }, workspaceSlug);
        }

        public async Task<ClientResponse> InviteUsersToWorkspace(
            IEnumerable<string> emailAddresses, string workspaceSlug, bool onlyNewUsers = false)
        {
            var emailList = emailAddresses.ToList();
            var workspace = await WorkspaceRepository.GetBySlug(workspaceSlug);

            if (workspace is null)
            {
                return ClientResponse.Failed("workspace not found");
            }

            if (emailList.Count == 0)
            {
                return ClientResponse.Failed("no email addresses provided");
            }

            var users = await UserRepository.GetByEmailRange(emailList);

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

        public async Task<ClientResponse> RemoveUsersFromWorkspace(IEnumerable<string> emailAddresses, string workspaceSlug)
        {
            var emailList = emailAddresses.ToList();
            var workspace = await WorkspaceRepository.GetBySlug(workspaceSlug);

            if (workspace is null)
            {
                return ClientResponse.Failed("workspace not found");
            }

            if (emailList.Count == 0)
            {
                return ClientResponse.Failed("no email addresses provided");
            }

            var users = await UserRepository.GetByEmailRange(emailList);
            var userIds = users.Select(user => user.Id).ToList();

            await UserRepository.RemoveUsersFromWorkspace(userIds, workspace.Id);

            return ClientResponse.Success();
        }

        public async Task<UserViewModel> Update(AppUser user)
        {
            var updatedUser = await UserRepository.GetAsync(user.Id);

            if (updatedUser is null) return null;
            
            updatedUser.PhoneNumber = user.PhoneNumber;
            updatedUser.Firstname = user.Firstname;
            updatedUser.Lastname = user.Lastname;

            await UnitOfWork.CompleteAsync();

            return MapUser(updatedUser);
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
                        DisplayName = user.GetDisplayName(),
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

        private UserViewModel MapUser(AppUser user)
        {
            return Mapper.Map<AppUser, UserViewModel>(user);
        }

        private List<UserViewModel> MapUsers(List<AppUser> users)
        {
            return Mapper.Map<List<AppUser>, List<UserViewModel>>(users);
        }
    }
}
