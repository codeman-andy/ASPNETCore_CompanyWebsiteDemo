using System.Security.Claims;

namespace EmployeeManagement.ViewModels
{
    public class AdministrationEditClaimsInUserViewModel
    {
        public List<Claim> Claims { get; set; }

        public List<bool> IsSelected { get; set; }

        public void InitIsSelected()
        {
            this.IsSelected = new List<bool>(this.Claims.Count);
        }
    }
}
