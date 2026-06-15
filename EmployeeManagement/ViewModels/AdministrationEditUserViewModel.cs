using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class AdministrationEditUserViewModel
    {
        public AdministrationEditUserViewModel()
        {
            Claims = new List<string>();
            Roles  = new List<string>();
        }

        public string ID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required][EmailAddress]
        public string Email { get; set; }

        public string City { get; set; }

        public IList<string> Claims { get; set; }

        public IList<string> Roles { get; set; }
    }
}
