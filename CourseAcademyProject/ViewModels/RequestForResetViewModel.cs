using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels
{
    public class RequestForResetViewModel
    {
        [Required(ErrorMessage = "Enter your e-mail address")]
        [Display(Name = "Email", Prompt = "email address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Incorrect e-mail address")]
        [Remote(action: "IsEmailExists", controller: "Account", ErrorMessage = "Email address not found")]
        public string Email { get; set; }

    }
}
