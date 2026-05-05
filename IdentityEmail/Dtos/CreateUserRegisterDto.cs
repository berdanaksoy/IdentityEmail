using IdentityEmail.Validation;
using System.ComponentModel.DataAnnotations;

namespace IdentityEmail.Dtos
{
    public class CreateUserRegisterDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [AllowedEmailDomain("gmail.com", "outlook.com", "hotmail.com", "live.com")]
        public string Email { get; set; }
        public string Password { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Devam etmek için kullanım şartlarını kabul etmelisiniz.")]
        public bool AcceptTerms { get; set; }
    }
}
