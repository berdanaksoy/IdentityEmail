namespace IdentityEmail.Dtos
{
    public class InboxDto
    {
        public int MessageId { get; set; }
        public string Subject { get; set; }
        public DateTime SendDate { get; set; }
        public string ImageUrl { get; set; }
        public string SenderFullname { get; set; }
        public bool IsRead { get; set; }
        public bool IsStarred { get; set; }
        public bool IsDraft { get; set; }
        public bool IsTrash { get; set; }
        public bool IsSpam { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
    }
}