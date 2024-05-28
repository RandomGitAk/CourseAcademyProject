using CourseAcademyProject.Models;

namespace CourseAcademyProject.ViewModels
{
    public class LessonsWithCurrentLessonViewModel
    {
        public IEnumerable<Lesson> Lessons { get; set; }
        public Lesson Lesson { get; set; }
    }
}
