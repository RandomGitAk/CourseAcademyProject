namespace CourseAcademyProject.Models.Test
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public IEnumerable<Answer> Answers { get; set; }
        public ICollection<UserAnswer>? UserAnswers {  get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
    }
}

