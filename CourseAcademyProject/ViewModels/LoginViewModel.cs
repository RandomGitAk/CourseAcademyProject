using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Input Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Incorrect email!")]
        [Display(Name = "Email")]
        public string? Email {  get; set; }

        [Required(ErrorMessage = "Input password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "Remember?")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }

    }
}
