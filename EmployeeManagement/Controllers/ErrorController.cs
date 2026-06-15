using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode) // Parameter name must match name of placeholder in route-attribute
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch(statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you request could not be found.";

                    _logger.LogWarning($"404 Error! Path: {statusCodeResult.OriginalPath}" +
                        $" QueryString: {statusCodeResult.OriginalQueryString}");

                    break;
            }

            return View("NotFound");
        }

        [Route("Error")]
        [AllowAnonymous] // Even anonymous users should get to reach this action-method
        public IActionResult Error()
        {
            var exception_details = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // With string interpolation, the expression inside the curly
            // braces will be replaced with the expression value at RUNTIME
            _logger.LogError($"The path {exception_details.Path} threw an exception {exception_details.Error}");

            return View("Error");
        }
    }
}
