using API.Model;
using Microsoft.AspNetCore.SignalR;

namespace API.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IDictionary<string, UserRoom> _connection;

        public ChatHub(IDictionary<string, UserRoom> connection)
        {
            _connection = connection;
        }

        public async Task JoinRoom(UserRoom userRoom)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userRoom.Room);
            _connection[Context.ConnectionId] = userRoom;
            await Clients.Group(userRoom.Room!)
                .SendAsync("ReceiveMessage", "Chat Boot Demo", $"{userRoom.User} has Joined the Group", DateTime.Now);
            await SendConnectedUser(userRoom.Room!);
        }

        public async Task SendMessage(string message)
        {
            if (_connection.TryGetValue(Context.ConnectionId, out UserRoom userRoom))
            {
                await Clients.Group(userRoom.Room!)
                    .SendAsync("ReceiveMessage", userRoom.User, message, DateTime.Now);
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (!_connection.TryGetValue(Context.ConnectionId, out UserRoom userRoom))
            {
                return base.OnDisconnectedAsync(exception);
            }

            Clients.Group(userRoom.Room!)
                .SendAsync("ReceiveMessage", "Chat Boot Demo", $"{userRoom.User} has Joined the Group", DateTime.Now);
            SendConnectedUser(userRoom.Room!);

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendConnectedUser(string room)
        {
            var users = _connection.Values
                .Where(u => u.Room == room)  // u => user
                .Select(r => r.User);  // r => result of users
            return Clients.Group(room).SendAsync("ConnectedUser", users);
        }
    }
}
