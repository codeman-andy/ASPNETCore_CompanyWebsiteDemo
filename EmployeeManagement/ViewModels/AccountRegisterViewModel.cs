using EmployeeManagement.Models;
using EmployeeManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class AccountRegisterViewModel
    {
        [Required]
        // Here we could of also used [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid e-mail format.")]
        [Remote(action: "IsEmailInUse", controller: "Account")] // This allows for remote validation using the IsEmailInUse() AJAX method
        [ValidEmailDomain("email.com", ErrorMessage = "E-mail domain must be @email.com")] // Custom validation-attribute
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] // This masks the characters that are input into this field by the user
        public string Password { get; set; }

        [DataType(DataType.Password)] // This masks the characters that are input into this field by the user
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")] // Here, "Password" corresponds to the name of the property set in this model
        public string ConfirmPassword { get; set; }

        public string? City { get; set; }
    }
}
