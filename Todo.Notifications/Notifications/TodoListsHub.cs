using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;

namespace Todo.Notifications.Notifications
{
    [UsedImplicitly]
    public sealed class TodoListsHub :
        Hub
    {
        [UsedImplicitly]
        public async Task SubscribeToList(string listId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, listId);
        }
    }
}