﻿namespace CourseAcademyProject.Models.Test
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int TestResultId { get; set; }
        public TestResult TestResult { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int? AnswerId { get; set; }
        public Answer Answer { get; set; }
    }

}

