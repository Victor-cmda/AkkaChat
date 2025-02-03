using Akka.Actor;
using Core.Messages;

namespace Core.Actors
{
    public class ChatRoomActor : ReceiveActor
    {
        private readonly Dictionary<string, IActorRef> _users;
        private readonly string _roomName;

        public ChatRoomActor(string roomName)
        {
            _users = new Dictionary<string, IActorRef>();
            _roomName = roomName;

            Receive<JoinRoom>(message => HandleJoinRoom(message));
            Receive<ChatMessage>(message => HandleChatMessage(message));
            Receive<LeaveRoom>(message => HandleLeaveRoom(message));
        }

        private void HandleJoinRoom(JoinRoom message)
        {
            if (!_users.ContainsKey(message.UserId))
            {
                _users.Add(message.UserId, Sender);
                BroadcastMessage(new ChatMessage
                {
                    SenderId = "System",
                    Content = $"{message.UserName} entrou na sala",
                    RoomName = message.RoomName
                });
            }

        }

        private void HandleChatMessage(ChatMessage message)
        {
            BroadcastMessage(message);
            Sender.Tell(message);
        }

        private void HandleLeaveRoom(LeaveRoom message)
        {
            if (_users.Remove(message.UserId))
            {
                var leaveMessage = new ChatMessage
                {
                    SenderId = "System",
                    Content = $"Alguém saiu da sala",
                    RoomName = message.RoomName
                };

                BroadcastMessage(leaveMessage);

                Sender.Tell(leaveMessage);
            }
        }

        private void BroadcastMessage(ChatMessage message)
        {
            foreach (var user in _users.Values)
            {
                user.Tell(message);
            }
        }
    }
}
