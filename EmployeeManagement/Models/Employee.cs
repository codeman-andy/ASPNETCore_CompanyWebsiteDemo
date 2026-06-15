using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required]
        // Here we could of also used [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid e-mail format.")]
        [Display(Name = "Office E-Mail")]
        public string Email { get; set; }

        // Integers, floats, doubles are all implicitly-required, and thus properties of these types do not need the [Required]-attribute
        // They can be made nullable (i.e. 'Dept?') and then applied the [Required]-attribute, which will allow for required-validation errors during form submissions
        [Required]
        public Dept? Department { get; set; }

        public string? PhotoPath { get; set; }
    }
}
