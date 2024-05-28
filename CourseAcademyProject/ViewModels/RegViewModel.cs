using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels
{
    public class RegViewModel
    {
        [Required(ErrorMessage = "Input Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Incorrect email!")]
        [Display(Name = "Email")]
        [Remote(action: "IsEmailInUse", controller: "Account", ErrorMessage = "The email address is already in use")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Input first name")]
        [Display(Name = "First name")]
        public string? FirstName {  get; set; }

        [Required(ErrorMessage = "Input last name")]
        [Display(Name = "Last name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string? PasswordConfirm { get; set; }

    }
}
