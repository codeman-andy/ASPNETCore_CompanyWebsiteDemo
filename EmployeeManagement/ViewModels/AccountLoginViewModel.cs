using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class AccountLoginViewModel
    {
        [Required]
        // Here we could of also used [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid e-mail format.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] // This masks the characters that are input into this field by the user
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}