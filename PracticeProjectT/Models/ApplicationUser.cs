using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PracticeProjectT.Models
{
    public class ApplicationUser : IdentityUser
    {
        //1478963Ab@
        public string FullName { get; set; }
        public string? UserType { get; set; }

        [DataType(DataType.Upload)]
        public string? ProfilePicture { get; set; }

        [DataType(DataType.Upload)]
        public string? ResumeFile { get; set; }
        public string? Qualification { get; set; }
        public string? CellPhone { get; set; }
        public string? UniversityName { get; set; }
        public string? CompanyName { get; set; }
        public string? Experience { get; set; }
        public string? Availability { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
    }
}
