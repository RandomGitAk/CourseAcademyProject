using CourseAcademyProject.Models.Test;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels.Test
{
    public class TestWithQuestionAndAnswer
    {
        [Key]
        public int? Id { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Test title not specified")]
        public string? Title { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Test description not specified")]
        public string? Description { get; set; }

        [Display(Name = "Time to pass (mm:ss)")]
        [Required(ErrorMessage = "Test time to pass not specified")]
        public TimeOnly TimeToPass { get; set; }

        public List<QuestionViewModel> Questions { get; set; }
        public int CourseId { get; set; }

        [Display(Name = "Courses")]
        public SelectList? AllCourses { get; set; } = null!;
    }
}
