using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string _allowedDomain;

        public ValidEmailDomainAttribute(string allowed_domain)
        {
            _allowedDomain = allowed_domain;
        }

        public override bool IsValid(object? value)
        {
            return value.ToString().Split('@')[1].ToUpper() == _allowedDomain.ToUpper();
        }
    }
}
