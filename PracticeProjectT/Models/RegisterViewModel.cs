using System.ComponentModel.DataAnnotations;

namespace PracticeProjectT.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? CellPhone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        public string UserType { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? ProfilePicture { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? ResumeFile { get; set; }

        public string? Qualification { get; set; }

        public string? UniversityName { get; set; }

        public string? CompanyName { get; set; }

        public string? Experience { get; set; }
        public string? Availability { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
    }
}
