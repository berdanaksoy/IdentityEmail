using System.ComponentModel.DataAnnotations;

namespace IdentityEmail.Validation
{
    public class AllowedEmailDomainAttribute : ValidationAttribute
    {
        private readonly HashSet<string> _allowedDomains;

        public AllowedEmailDomainAttribute(params string[] allowedDomains)
        {
            _allowedDomains = new HashSet<string>(
                allowedDomains.Select(d => d.Trim().ToLowerInvariant())
            );
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (string.IsNullOrWhiteSpace(email))
                return ValidationResult.Success;

            email = email.Trim();

            var atIndex = email.LastIndexOf('@');
            if (atIndex <= 0 || atIndex == email.Length - 1)
                return ValidationResult.Success;

            var domain = email[(atIndex + 1)..].Trim().ToLowerInvariant();

            if (!_allowedDomains.Contains(domain))
            {
                return new ValidationResult(
                    $"Sadece {string.Join(", ", _allowedDomains)} uzantılı email kullanabilirsiniz."
                );
            }

            return ValidationResult.Success;
        }
    }
}