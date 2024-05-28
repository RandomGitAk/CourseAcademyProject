using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels
{
    public class AccountSettingsViewModel
    {
        [Key]
        public string Id { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "The first name cannot be empty")]
        public string? FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "The last name cannot be empty")]
        public string? LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email annot be empty")]
        public string? Email { get; set; }

        [Display(Name = "Image")]
        public IFormFile? File { get; set; }
        public string? Image { get; set; }
    }
}
