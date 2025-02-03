using Akka.Actor;
using Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Actors
{
    public class ChatCoordinatorActor : ReceiveActor
    {
        private readonly Dictionary<string, IActorRef> _chatRooms;

        public ChatCoordinatorActor()
        {
            _chatRooms = new Dictionary<string, IActorRef>();

            Receive<JoinRoom>(message => HandleJoinRoom(message));
            Receive<ChatMessage>(message => HandleChatMessage(message));
            Receive<LeaveRoom>(message => HandleLeaveRoom(message));
        }

        private void HandleJoinRoom(JoinRoom message)
        {
            if (!_chatRooms.TryGetValue(message.RoomName, out var roomActor))
            {
                roomActor = Context.ActorOf(Props.Create(() => new ChatRoomActor(message.RoomName)));
                _chatRooms[message.RoomName] = roomActor;
            }

            roomActor.Forward(message);
        }

        private void HandleChatMessage(ChatMessage message)
        {
            try
            {
                if (_chatRooms.TryGetValue(message.RoomName, out var roomActor))
                {
                    roomActor.Tell(message, Sender);
                }
                else
                {
                    Sender.Tell(new ChatMessage
                    {
                        Content = "Sala não encontrada",
                        RoomName = message.RoomName,
                        SenderId = message.SenderId
                    });
                }
            }
            catch (Exception ex)
            {
                Sender.Tell(new ChatMessage
                {
                    Content = $"Erro: {ex.Message}",
                    RoomName = message.RoomName,
                    SenderId = message.SenderId
                });
            }
        }

        private void HandleLeaveRoom(LeaveRoom message)
        {
            if (_chatRooms.TryGetValue(message.RoomName, out var roomActor))
            {
                roomActor.Forward(message);
            }
        }
    }
}
