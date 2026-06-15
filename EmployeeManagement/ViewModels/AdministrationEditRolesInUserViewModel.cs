using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.ViewModels
{
    public class AdministrationEditRolesInUserViewModel
    {
        public List<IdentityRole> Roles { get; set; }

        public List<bool> IsSelected { get; set; }

        public void InitIsSelected()
        {
            this.IsSelected = new List<bool>(this.Roles.Count);
        }
    }
}
