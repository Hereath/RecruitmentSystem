using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PracticeProjectT.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace PracticeProjectT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if(user.UserType == "Employer")
            {
                return RedirectToAction("EmployerIndex");
            }

            return View(user);
        }

        public async Task<IActionResult> EmployerIndex()
        {
            // Get all job seekers
            var jobSeekers = await _userManager.GetUsersInRoleAsync("JobSeeker");

            // Pass the job seekers to the view
            return View(jobSeekers);
        }


        public IActionResult Privacy()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                // Update user's profile properties based on the data in the model
                user.PhoneNumber = model.CellPhone;
                user.CellPhone = model.CellPhone;
                user.Qualification = model.Qualification;
                user.Availability = model.Availability;
                user.Experience = model.Experience;
                user.FullName = model.FullName;
                user.UniversityName = model.UniversityName;

                // Save the updated user profile
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If model validation fails or the update fails, redisplay the edit profile view with errors
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfileConfirmed()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Delete the user
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // If the deletion fails, redisplay the delete profile view with errors
                return View();
            }
        }


        public async Task<IActionResult> ViewJobSeeker(string id)
        {
            var jobSeeker = await _userManager.FindByIdAsync(id);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            return View(jobSeeker);
        }

        public async Task<IActionResult> DownloadResume(string id)
        {
            var jobSeeker = await _userManager.FindByIdAsync(id);
            if (jobSeeker == null || string.IsNullOrEmpty(jobSeeker.ResumeFile))
            {
                return NotFound();
            }

            // Create the file path to the resume file
            var resumePath = Path.Combine(_webHostEnvironment.WebRootPath, jobSeeker.ResumeFile);

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