using System.ComponentModel.DataAnnotations;

namespace IdentityEmail.Entities
{
    public class SpamSender
    {
        [Key]
        public int SpamSenderId { get; set; }
        public string UserEmail { get; set; }
        public string SenderEmail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
