namespace CourseAcademyProject.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Information { get; set; }
        public string LessonContent {  get; set; }
        public string VideoUrl { get; set; }
        public string MaterialsUrl { get; set; }
        public string FileLessonContent { get; set; }
        public DateTime DateOfPublication { get; set; }

        public int CourseId { get; set; }   
        public Course Course { get; set; }
    }
}
