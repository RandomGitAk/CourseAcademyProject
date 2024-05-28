using Microsoft.AspNetCore.Identity;

namespace CourseAcademyProject.Models
{
    public class User : IdentityUser
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }   
        public string Image {  get; set; }
    }
}
