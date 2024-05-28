using CourseAcademyProject.Interfaces;

namespace CourseAcademyProject.Models.Test
{
    public class TestResult
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
        public string UserId { get; set; }
        public TimeOnly CompletionTime {  get; set; }
        public IEnumerable<UserAnswer> UserAnswers { get; set; }
    }
}
