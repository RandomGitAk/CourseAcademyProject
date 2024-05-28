namespace CourseAcademyProject.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string MiniDescription { get; set; }
        public string Content { get; set; }
        public DateTime DateOfPublication { get; set; }
    }
}
