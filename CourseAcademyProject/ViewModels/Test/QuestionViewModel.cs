using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels.Test
{
    public class QuestionViewModel
    {
        [Key]
        public int? Id { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage = "Question content not specified")]
        public string? Content { get; set; }

        public List<AnswerViewModel> Answers { get; set; }
        public int TestId { get; set; }
    }
}
