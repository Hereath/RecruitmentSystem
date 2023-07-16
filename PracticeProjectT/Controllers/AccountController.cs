using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using PracticeProjectT.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PracticeProjectT.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save uploaded files to the file system or database

                // Save profile picture
                string profilePicturePath = await SaveFile(model.ProfilePicture);

                // Save resume (if job seeker)
                string resumePath = null;
                if (model.UserType == "JobSeeker" && model.ResumeFile != null)
                {
                    resumePath = await SaveFile(model.ResumeFile);
                }

                // Create the ApplicationUser object and set properties
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    UserType = model.UserType,
                    ProfilePicture = profilePicturePath,
                    ResumeFile = resumePath,
                    Qualification = model.Qualification,
                    UniversityName = model.UniversityName,
                    CompanyName = model.CompanyName,
                    CompanyRegistrationNumber = model.CompanyRegistrationNumber,
                    Experience = model.Experience,
                    Availability = model.Availability,
                    CellPhone = model.CellPhone,
                };

                var result = await _userManager.CreateAsync(user, model.Password.Trim());

                if (result.Succeeded)
                {
                    // Check if the "Admin" role exists
                    var adminRoleExists = await _roleManager.RoleExistsAsync(model.UserType);

                    if (!adminRoleExists)
                    {
                        // If the "Admin" role does not exist, create it
                        var adminRole = new IdentityRole { Name = model.UserType };
                        await _roleManager.CreateAsync(adminRole);
                    }

                    // Assign the user to the "Admin" role
                    await _userManager.AddToRoleAsync(user, model.UserType);

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                // Redirect to a success page or appropriate action
                return View(model);
            }

            // If model validation fails, redisplay the registration view with errors
            return View(model);
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Generate a unique file name or use the user's ID or email for naming

                // Save the file to the file system
                var uploadsDirectoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsDirectoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the saved file path relative to the project
                var relativeFilePath = filePath.Replace(_webHostEnvironment.WebRootPath, string.Empty).TrimStart(Path.DirectorySeparatorChar);
                return relativeFilePath;
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadResume()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.ResumeFile))
            {
                return NotFound();
            }

            // Create the file path to the resume file
            var resumePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ResumeFile);

            // Check if the file exists
            if (!System.IO.File.Exists(resumePath))
            {
                return NotFound();
            }

            // Set the content type for the response
            var contentType = "application/pdf"; // Update with the appropriate content type

            // Return the file as a download
            return PhysicalFile(resumePath, contentType, Path.GetFileName(resumePath));
        }

    }
}
