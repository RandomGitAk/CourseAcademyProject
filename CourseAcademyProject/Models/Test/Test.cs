namespace CourseAcademyProject.Models.Test
{
    public class Test
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeOnly TimeToPass { get; set; }

        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<TestResult> TestResults { get; set; }
        
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
