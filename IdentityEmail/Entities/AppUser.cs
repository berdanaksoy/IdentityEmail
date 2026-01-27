using Microsoft.AspNetCore.Identity;

namespace IdentityEmail.Entities
{
    public class AppUser:IdentityUser
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? ImageUrl { get; set; }
        public string? About { get; set; }
    }
}
