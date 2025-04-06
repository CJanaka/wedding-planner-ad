namespace wedding_planer_ad.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public string MessageContent { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
