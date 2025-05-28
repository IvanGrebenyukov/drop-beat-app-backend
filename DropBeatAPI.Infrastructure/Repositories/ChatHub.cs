using Microsoft.AspNetCore.SignalR;

namespace DropBeatAPI.Infrastructure.Repositories;

public class ChatHub : Hub
{
    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task SendMessage(string chatId, string text)
    {
        await Clients.Group(chatId).SendAsync("ReceiveMessage", new {
            ChatId = chatId,
            Text = text,
            SenderId = Context.UserIdentifier,
            SentAt = DateTime.UtcNow
        });
    }

    public override async Task OnConnectedAsync()
    {
        var chatId = Context.GetHttpContext()?.Request.Query["chatId"];
        if (!string.IsNullOrEmpty(chatId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var chatId = Context.GetHttpContext()?.Request.Query["chatId"];
        if (!string.IsNullOrEmpty(chatId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}

