using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels.Test
{
    public class AnswerViewModel
    {
        [Key]
        public int? Id { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage = "Test content not specified")]
        public string? Content { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }
}
