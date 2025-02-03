namespace Core.Messages
{
    public class ChatMessage
    {
        public string SenderId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string RoomName { get; set; }
        public string UserName { get; set; }
    }
}
