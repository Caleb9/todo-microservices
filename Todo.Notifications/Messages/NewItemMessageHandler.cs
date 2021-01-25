using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using Todo.Notifications.Notifications;
using Todo.Shared;

namespace Todo.Notifications.Messages
{
    [UsedImplicitly]
    public sealed class NewItemMessageHandler :
        IHandleMessages<NewItemMessage>
    {
        private readonly IHubContext<TodoListsHub> _hubContext;
        private readonly ILogger<NewItemMessageHandler> _logger;

        public NewItemMessageHandler(
            IHubContext<TodoListsHub> hubContext,
            ILogger<NewItemMessageHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }
        
        public async Task Handle(
            NewItemMessage message)
        {
            _logger.LogInformation($"Received {message}");
            await _hubContext.Clients.Group(message.ListId.ToString()).SendAsync("sendToClient", message.ItemId);
        }
    }
}