using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmployeeManagement.Controllers
{
    // [Authorize(Roles = "Admin,User")] == Admin OR User
    // [Authorize(Roles = "Admin")], [Authorize(Roles = "User")] == Admin AND User
    // [Authorize] on action-methods will override the Controller-attribute
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(RoleManager<IdentityRole> role_manager,
                                        UserManager<ApplicationUser> userManager,
                                        ILogger<AdministrationController> logger)
        {
            _roleManager = role_manager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("Administration/Create/Role")]
        public IActionResult CreateRole()
        {
            // Must specify full-path otherwise MVC will not search inside sub-folders
            return View("~/Views/Administration/Create/Role.cshtml");
        }

        [HttpPost]
        [Route("Administration/Create/Role")]
        public async Task<IActionResult> CreateRole(AdministrationCreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole { Name = model.RoleName };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // Must specify full-path otherwise MVC will not search inside sub-folders
            return View("~/Views/Administration/Create/Role.cshtml", model);
        }

        [HttpGet]
        [Route("Administration/Roles")]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;

            return View("~/Views/Administration/Roles.cshtml", roles);
        }

        [HttpPost]
        [Route("Administration/Roles")]
        [Authorize(Policy = "SuperAdminPolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            // i.e. it has already been deleted by someone else before we pressed the button.
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID = {id} cannot be found.";

                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await _roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View("ListRoles");
                    }
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"Error deleting role {ex}");

                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} cannot be deleted as there are still users assigned to this role";

                    return View("Error");
                }
            }
        }

        [HttpGet]
        [Route("Administration/Edit/Role/{id}")]
        [Authorize(Policy = "SuperAdminPolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID = {id} cannot be found.";

                return View("NotFound");
            }
            else
            {
                var model = new AdministrationEditRoleViewModel
                {
                    ID = id,
                    RoleName = role.Name
                };

                foreach (var user in await _userManager.Users.ToListAsync())
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        model.Users.Add(user.UserName);
                    }
                }

                return View("~/Views/Administration/Edit/Role.cshtml", model);
            }
        }

        [HttpPost]
        [Route("Administration/Edit/Role")]
        [Authorize(Policy = "SuperAdminPolicy")]
        public async Task<IActionResult> EditRole(AdministrationEditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.ID);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID = {model.ID} cannot be found.";

                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("~/Views/Administration/Edit/Role.cshtml", model);
                }
            }
        }

        [HttpGet]
        [Route("Administration/Edit/UsersInRole/{roleID}")]
        public async Task<IActionResult> EditUsersInRole(string roleID)
        {
            ViewBag.RoleID = roleID;

            var role = await _roleManager.FindByIdAsync(roleID);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID = {roleID} cannot be found.";

                return View("NotFound");
            }

            var model = new AdministrationEditUsersInRoleViewModel
            {
                Users = _userManager.Users.ToList()
            };

            model.InitIsSelected();

            for (int i = 0; i < model.Users.Count; i++)
            {
                model.IsSelected.Add(await _userManager.IsInRoleAsync(model.Users[i], role.Name));
            }

            return View("~/Views/Administration/Edit/UsersInRole.cshtml", model);
        }

        [HttpPost]
        [Route("Administration/Edit/UsersInRole/{roleID}")]
        public async Task<IActionResult> EditUsersInRole(AdministrationEditUsersInRoleViewModel model, string roleID)
        {
            var role = await _roleManager.FindByIdAsync(roleID);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID = {roleID} cannot be found.";

                return View("NotFound");
            }
            else
            {
                model.Users = _userManager.Users.ToList();

                IdentityResult result = null; // Cannot assign 'null' to an implicitly-typed variable (i.e. 'var result = null;')

                for (int i = 0; i < model.Users.Count; i++)
                {
                    if (model.IsSelected[i] && !(await _userManager.IsInRoleAsync(model.Users[i], role.Name)))
                    {
                        result = await _userManager.AddToRoleAsync(model.Users[i], role.Name);
                    }
                    else if (!model.IsSelected[i] && (await _userManager.IsInRoleAsync(model.Users[i], role.Name)))
                    {
                        result = await _userManager.RemoveFromRoleAsync(model.Users[i], role.Name);
                    }
                    else
                    {
                        continue;
                    }
                }

                return RedirectToAction("EditRole", new {id = roleID} );
            }
        }

        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> ListUsers()
        {
            var users = _userManager.Users;

            return View("~/Views/Administration/Users.cshtml", users);
        }

        [HttpPost]
        [Route("Users")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            // i.e. it has already been deleted by someone else before we pressed the button.
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {id} cannot be found.";

                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListUsers");
                }
            }
        }

        [HttpGet]
        [Route("Edit/User/{id}")]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {id} cannot be found.";

                return View("NotFound");
            }
            else
            {
                var user_claims = await _userManager.GetClaimsAsync(user);
                var user_roles  = await _userManager.GetRolesAsync(user);

                var model = new AdministrationEditUserViewModel
                {
                    ID = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    City = user.City,
                    Claims = user_claims.Select(c => c.Value).ToList(),
                    Roles = user_roles
                };

                return View("~/Views/Administration/Edit/User.cshtml", model);
            }
        }

        [HttpPost]
        [Route("Edit/User/{id}")]
        public async Task<IActionResult> EditUser(AdministrationEditUserViewModel model, string id)
        {
            var user = await _userManager.FindByIdAsync(model.ID);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {model.ID} cannot be found.";

                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("~/Views/Administration/Edit/User.cshtml", model);
                }
            }
        }

        [HttpGet]
        [Route("Administration/Edit/RolesInUser/{userID}")]
        public async Task<IActionResult> EditRolesInUser(string userID)
        {
            ViewBag.UserID = userID;

            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {userID} cannot be found.";

                return View("NotFound");
            }

            var model = new AdministrationEditRolesInUserViewModel
            {
                Roles = _roleManager.Roles.ToList()
            };

            model.InitIsSelected();

            for (int i = 0; i < model.Roles.Count; i++)
            {
                model.IsSelected.Add(await _userManager.IsInRoleAsync(user, model.Roles[i].Name));
            }

            return View("~/Views/Administration/Edit/RolesInUser.cshtml", model);
        }

        [HttpPost]
        [Route("Administration/Edit/RolesInUser/{userID}")]
        public async Task<IActionResult> EditRolesInUser(AdministrationEditRolesInUserViewModel model, string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {userID} cannot be found.";

                return View("NotFound");
            }
            else
            {
                model.Roles = _roleManager.Roles.ToList();

                IdentityResult result = null; // Cannot assign 'null' to an implicitly-typed variable (i.e. 'var result = null;')

                for (int i = 0; i < model.Roles.Count; i++)
                {
                    if (model.IsSelected[i] && !(await _userManager.IsInRoleAsync(user, model.Roles[i].Name)))
                    {
                        result = await _userManager.AddToRoleAsync(user, model.Roles[i].Name);
                    }
                    else if (!model.IsSelected[i] && (await _userManager.IsInRoleAsync(user, model.Roles[i].Name)))
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, model.Roles[i].Name);
                    }
                    else
                    {
                        continue;
                    }
                }

                return RedirectToAction("EditUser", new { id = userID });
            }
        }

        [HttpGet]
        [Route("Administration/Edit/ClaimsInUser/{userID}")]
        public async Task<IActionResult> EditClaimsInUser(string userID)
        {
            ViewBag.UserID = userID;

            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {userID} cannot be found.";

                return View("NotFound");
            }

            var user_claims = await _userManager.GetClaimsAsync(user);

            var model = new AdministrationEditClaimsInUserViewModel
            {
                Claims = ClaimsStore.AllClaims
            };

            model.InitIsSelected();

            for (int i = 0;  i < model.Claims.Count; i++)
            {
                model.IsSelected.Add(user_claims.Any(c => c.Type == model.Claims[i].Type));
            }

            return View("~/Views/Administration/Edit/ClaimsInUser.cshtml", model);
        }

        [HttpPost]
        [Route("Administration/Edit/ClaimsInUser/{userID}")]
        public async Task<IActionResult> EditClaimsInUser(AdministrationEditClaimsInUserViewModel model, string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {userID} cannot be found.";

                return View("NotFound");
            }
            else
            {
                var user_claims = await _userManager.GetClaimsAsync(user);

                model.Claims = ClaimsStore.AllClaims;

                IdentityResult result = null; // Cannot assign 'null' to an implicitly-typed variable (i.e. 'var result = null;')

                for (int i = 0; i < model.Claims.Count; i++)
                {
                    var has_claim = user_claims.Any(c => c.Type == model.Claims[i].Type);

                    if (model.IsSelected[i] && !(has_claim))
                    {
                        result = await _userManager.AddClaimAsync(user, model.Claims[i]);
                    }
                    else if (!model.IsSelected[i] && (has_claim))
                    {
                        result = await _userManager.RemoveClaimAsync(user, model.Claims[i]);
                    }
                    else
                    {
                        continue;
                    }
                }

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add selected claims to the user.");

                    return View(model);
                }
                else
                {
                    return RedirectToAction("EditUser", new { id = userID });
                }
            }
        }
    }
}
