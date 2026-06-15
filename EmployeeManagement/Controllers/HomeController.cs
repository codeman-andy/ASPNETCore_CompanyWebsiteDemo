using Microsoft.AspNetCore.Mvc;

using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Controllers
{
    [Route("Home")]
    // [Authorize] with this, none of the actions within this controller can be reached unless the user is logged-in
    // [AllowAnonymous] here would override any [Authorize]-attributes defined within this Controller
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IWebHostEnvironment hostingEnvironment, IEmployeeRepository employeeRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _employeeRepository = employeeRepository;
        }

        [Route("~/")]
        [Route("")]
        [Route("Index")]
        // [AllowAnonymous] would provide an exception to this method if the [Authorize] attribute had been set Controller-wide
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAll();
            return View(model);
        }

        [Route("Details/{id?}")]
        // [AllowAnonymous] would provide an exception to this method if the [Authorize] attribute had been set Controller-wide
        public ViewResult Details(int? id)
        {
            Employee employee = _employeeRepository.GetByID(id.Value);

            if (employee == null)
            {
                Response.StatusCode = 404;

                return View("EmployeeNotFound", id.Value);
            }

            HomeDetailsViewModel viewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("Create")]
        [Authorize] // This tells users they need to at least be logged-in in order to use this functionality
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [Authorize] // This tells users they need to at least be logged-in in order to use this functionality
        public IActionResult Create(HomeCreateViewModel model)
        {
            // The model must be of the exact same type as the @Model used in the View-file
            if (ModelState.IsValid)
            {
                string unique_filename = ProcessFileUpload(model);

                Employee new_employee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = unique_filename
                };

                // After EF inserts new row for the new Employee-object in the underlying
                // Employees-table, it takes the ID column-generated ID value and
                // automatically updates the ID property of this Employee-object
                _employeeRepository.Add(new_employee);

                return RedirectToAction("Details", new { id = new_employee.ID });
            }

            else
            {
                return View();
            }
        }

        [HttpGet]
        [Route("Edit")]
        [Authorize] // This tells users they need to at least be logged-in in order to use this functionality
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetByID(id);

            HomeEditViewModel model = new HomeEditViewModel
            {
                ID = employee.ID,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(model);
        }

        [HttpPost]
        [Route("Edit")]
        [Authorize] // This tells users they need to at least be logged-in in order to use this functionality
        public IActionResult Edit(HomeEditViewModel model)
        {
            // The model must be of the exact same type as the @Model used in the View-file
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetByID(model.ID);

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filepath = Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);

                        System.IO.File.Delete(filepath);
                    }

                    employee.PhotoPath = ProcessFileUpload(model);
                }

                _employeeRepository.Update(employee);

                return RedirectToAction("Details", new { id = employee.ID });
            }

            else
            {
                return View();
            }
        }

        private string ProcessFileUpload(HomeCreateViewModel model)
        {
            string unique_filename = null;

            if (model.Photo != null)
            {
                string uploads_folder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                // GUID = Global Unique IDentifier
                // NewGuid() returns a new GUID which is guaranteed to be unique
                unique_filename = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;

                string file_path = Path.Combine(uploads_folder, unique_filename);

                FileStream fs = new FileStream(file_path, FileMode.Create);
                model.Photo.CopyTo(fs);
                fs.Close();
            }

            return unique_filename;
        }
    }
}
