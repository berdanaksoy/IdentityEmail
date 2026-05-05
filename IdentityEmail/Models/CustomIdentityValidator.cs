using Microsoft.AspNetCore.Identity;

namespace IdentityEmail.Models
{
    public class CustomIdentityValidator : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = "DuplicateUserName",
                Description = "Bu kullanıcı adı zaten alınmış."
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = "InvalidEmail",
                Description = "Geçerli bir email adresi giriniz."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "Bu email adresi zaten kayıtlı."
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = "PasswordTooShort",
                Description = $"Parola en az {length} karakter olmalıdır."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresLower",
                Description = "Parola en az bir küçük harf içermelidir."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresUpper",
                Description = "Parola en az bir büyük harf içermelidir."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresDigit",
                Description = "Parola en az bir rakam içermelidir."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description = "Parola en az bir özel karakter içermelidir."
            };
        }
    }
}