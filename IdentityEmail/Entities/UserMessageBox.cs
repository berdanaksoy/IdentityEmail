namespace IdentityEmail.Entities
{
    public class UserMessageBox
    {
        public int UserMessageBoxId { get; set; }

        public string UserMessageBoxEmail { get; set; }

        public int MessageId { get; set; }
        public Message Message { get; set; }

        public int? CategoryId { get; set; }
        public MessageCategory? Category { get; set; }

        public bool IsDraft { get; set; }
        public bool IsTrash { get; set; }
        public bool IsSpam { get; set; }
        public bool IsRead { get; set; }
        public bool IsStarred { get; set; }
    }
}
