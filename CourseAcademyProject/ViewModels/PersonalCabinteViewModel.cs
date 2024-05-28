using CourseAcademyProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseAcademyProject.ViewModels
{
    public class PersonalCabinteViewModel
    {
        public IEnumerable<CourseWithCountTestsAndPassedTestsViewModel> Courses { get; set; }
        public IEnumerable<Category> AllCategories { get; set; } = null!;
        public int? CategoryId {  get; set; }
        public string? CourseName { get; set; }
    }
}
