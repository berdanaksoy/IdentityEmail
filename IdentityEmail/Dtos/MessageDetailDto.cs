namespace IdentityEmail.Dtos
{
    public class MessageDetailDto
    {
        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string MessageDetail { get; set; }
        public DateTime SendDate { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string SenderFullname { get; set; }
        public string SenderImageUrl { get; set; }
        public bool IsRead { get; set; }
        public bool IsStarred { get; set; }
        public bool IsTrash { get; set; }
        public bool IsSpam { get; set; }
        public bool IsDraft { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
    }
}