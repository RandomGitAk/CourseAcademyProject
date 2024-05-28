using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels.UserTest
{
    public class TestResultViewModel
    {
        [Key]
        public int TestId { get; set; }
        public TimeOnly CompletionTime { get; set; }
        public List<QuestionAnswerViewModel> QuestionAnswers { get; set; }
    }
}
