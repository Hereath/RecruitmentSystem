using System.ComponentModel.DataAnnotations;

namespace PracticeProjectT.Models
{
    public class LoginViewModel
    {
        // 1478963Ab@
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
