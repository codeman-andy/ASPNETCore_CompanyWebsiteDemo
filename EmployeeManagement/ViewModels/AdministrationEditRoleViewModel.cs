using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class AdministrationEditRoleViewModel
    {
        public AdministrationEditRoleViewModel()
        {
            Users = new List<string>();
        }

        public string ID { get; set; }

        [Required(ErrorMessage = "Role Name is required.")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
