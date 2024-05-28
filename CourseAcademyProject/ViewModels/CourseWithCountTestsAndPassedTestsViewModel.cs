using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseAcademyProject.ViewModels
{
    public class CourseWithCountTestsAndPassedTestsViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Image { get; set; }
        public int TotalTestsCount { get; set; }
        public int PassedTestsCount { get; set; }
    }
}
