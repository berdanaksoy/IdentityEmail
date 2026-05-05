using System.ComponentModel.DataAnnotations;

namespace IdentityEmail.Entities
{
    public class UserContactCategory
    {
        [Key]
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string ContactEmail { get; set; }
        public int CategoryId { get; set; }
        public MessageCategory Category { get; set; }
    }
}