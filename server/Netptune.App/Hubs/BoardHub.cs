using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using Netptune.Core.Hubs;
using Netptune.Core.Requests;
using Netptune.Core.Responses.Common;
using Netptune.Core.Services;
using Netptune.Core.ViewModels.ProjectTasks;
using Netptune.Core.ViewModels.Tags;

namespace Netptune.App.Hubs
{
    [Authorize]
    public class BoardHub : Hub<IBoardHub>
    {
        public const string Path = "/hubs/board-hub";

        private readonly IUserConnectionService UserConnection;
        private readonly ITaskService TaskService;
        private readonly ITagService TagService;

        public BoardHub(IUserConnectionService userConnection, ITaskService taskService, ITagService tagsService)
        {
            UserConnection = userConnection;
            TaskService = taskService;
            TagService = tagsService;
        }

        public override async Task OnConnectedAsync()
        {
            await UserConnection.Add(Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await UserConnection.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddToGroup(string group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);

            var userConnection = await UserConnection.Get(Context.ConnectionId);

            await Clients
                .OthersInGroup(group)
                .JoinBoard(userConnection);
        }

        public async Task RemoveFromGroup(string group)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);

            var userConnection = await UserConnection.Get(Context.ConnectionId);

            await Clients
                .OthersInGroup(group)
                .LeaveBoard(userConnection);
        }

        public async Task<ClientResponse> MoveTaskInBoardGroup(string group, MoveTaskInGroupRequest request)
        {
            var result = await TaskService.MoveTaskInBoardGroup(request);

            await Clients
                .OthersInGroup(group)
                .MoveTaskInBoardGroup(request);

            return result;
        }

        public async Task<TaskViewModel> Create(string group, AddProjectTaskRequest request)
        {
            var result = await TaskService.Create(request);

            await Clients
                .OthersInGroup(group)
                .Create(result);

            return result;
        }

        public async Task<ClientResponse> Delete(string group, int id)
        {
            var response = await TaskService.Delete(id);

            await Clients
                .OthersInGroup(group)
                .Delete(response, id);

            return response;
        }

        public async Task<ClientResponse> DeleteMultiple(string group, List<int> ids)
        {
            var response = await TaskService.Delete(ids);

            await Clients
                .OthersInGroup(group)
                .DeleteMultiple(response, ids);

            return response;
        }

        public async Task<TaskViewModel> Update(string group, UpdateProjectTaskRequest request)
        {
            var result = await TaskService.Update(request);

            await Clients
                .OthersInGroup(group)
                .Update(result);

            return result;
        }

        public async Task<TagViewModel> AddTagToTask(string group, AddTagRequest request)
        {
            var response = await TagService.AddTagToTask(request);

            await Clients
                .OthersInGroup(group)
                .AddTagToTask(response);

            return response;
        }

        public async Task<ClientResponse> DeleteTagFromTask(string group, DeleteTagFromTaskRequest request)
        {
            var response = await TagService.DeleteFromTask(request);

            await Clients
                .OthersInGroup(group)
                .DeleteTagFromTask(response);

            return response;
        }
    }
}