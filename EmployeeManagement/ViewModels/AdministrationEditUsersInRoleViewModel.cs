using EmployeeManagement.Models;

namespace EmployeeManagement.ViewModels
{
    public class AdministrationEditUsersInRoleViewModel
    {
        public List<ApplicationUser> Users { get; set; }

        public List<bool> IsSelected { get; set; }

        public void InitIsSelected()
        {
            this.IsSelected = new List<bool>(this.Users.Count);
        }
    }
}
