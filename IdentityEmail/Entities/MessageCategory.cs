using System.ComponentModel.DataAnnotations;

namespace IdentityEmail.Entities
{
    public class MessageCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string UserEmail { get; set; }
        public string Color { get; set; }
    }
}
