namespace Core.Messages
{
    public class LeaveRoom
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoomName { get; set; }
    }
}
