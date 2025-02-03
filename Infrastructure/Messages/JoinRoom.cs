using Akka.Actor;

namespace Core.Messages
{
    public class JoinRoom
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoomName { get; set; }
    }
}
