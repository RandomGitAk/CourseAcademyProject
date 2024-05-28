using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels
{
    public class ResetPasswordViewModel
    {
        [MinLength(5, ErrorMessage = "Minimum password length 5 characters")]
        [Required(ErrorMessage = "Enter a new password")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [MinLength(5, ErrorMessage = "Minimum password length is 5 characters")]
        [Required(ErrorMessage = "Confirm password")]
        [Compare("Password", ErrorMessage = "The passwords don't match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string? PasswordConfirm { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }

    }
}
