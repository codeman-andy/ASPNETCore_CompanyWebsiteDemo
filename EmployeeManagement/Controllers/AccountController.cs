using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> user_manager, SignInManager<ApplicationUser> sign_in_manager)
        {
            _userManager = user_manager;
            _signInManager = sign_in_manager;
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(AccountRegisterViewModel model)
        {
            // The model must be of the exact same type as the @Model used in the View-file
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        // These validation errors are then displayed in the Register-view by the validation-summary tag-helper
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(AccountLoginViewModel model, string? returnUrl)
        {
            // The model must be of the exact same type as the @Model used in the View-file
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // 'returnUrl' parameter name must match the exact same name of its query-string counterpart in order for model-binding to work
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            // Returns to the page the user had previously attempted to go to before they were asked to login in order to do so
                            return Redirect(returnUrl); // Url.IsLocalUrl() achieves the same thing as LocalRedirect()
                        }
                        else
                        {
                            // Returns to the page the user had previously attempted to go to before they were asked to login in order to do so
                            return LocalRedirect(returnUrl); // LocalRedirect() guarantees a user is only redirected to trusted URLs local to our app
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login Attempt");

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet][HttpPost]
        [Route("IsEmailInUse")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            // This method is an example of an AJAX-method --- AJAX allows web pages to be updated asynchronously by exchanging data with a web server behind the scenes.
            // jquery-validate expects a JsonResult object in return
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }
        }

        [HttpGet]
        [Route("~/Administration/AccessDenied")]
        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }
    }
}
