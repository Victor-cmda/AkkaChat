using Akka;
using Akka.Actor;
using Core.Messages;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IActorRef _chatCoordinatorActor;
        private static readonly Dictionary<string, string> _userRooms = new();

        public ChatHub(IActorRef chatCoordinatorActor)
        {
            _chatCoordinatorActor = chatCoordinatorActor;
        }

        public async Task JoinChatRoom(string userName, string chatRoom)
        {
            var userId = Context.ConnectionId;
            _userRooms[userId] = chatRoom;

            var joinMessage = new JoinRoom
            {
                UserId = userId,
                UserName = userName,
                RoomName = chatRoom
            };

            await _chatCoordinatorActor.Ask<ChatMessage>(joinMessage);
            await Groups.AddToGroupAsync(userId, chatRoom);
            await Clients.Group(chatRoom).SendAsync("ReceiveMessage", "System", $"{userName} entrou na sala");
        }

        public async Task SendMessage(string chatRoom, string userName, string message)
        {
            try
            {
                var chatMessage = new ChatMessage
                {
                    SenderId = Context.ConnectionId,
                    Content = message,
                    RoomName = chatRoom
                };

                var response = await _chatCoordinatorActor.Ask<ChatMessage>(chatMessage);

                if (response != null)
                {
                    await Clients.Group(chatRoom).SendAsync("ReceiveMessage", userName, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.ConnectionId;
            if (_userRooms.TryGetValue(userId, out var room))
            {
                await _chatCoordinatorActor.Ask<ChatMessage>(new LeaveRoom { UserId = userId, RoomName = room });
                _userRooms.Remove(userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
