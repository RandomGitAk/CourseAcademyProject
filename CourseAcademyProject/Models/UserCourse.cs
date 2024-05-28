namespace CourseAcademyProject.Models
{
    public class UserCourse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal? PayPrice { get; set; }
        public DateTime? CreatedDate { get; set; }
        public User User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
