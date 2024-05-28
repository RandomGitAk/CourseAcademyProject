using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CourseAcademyProject.ViewComponents
{
    public class BottomLessonNavigation : ViewComponent
    {
        private readonly ILesson _lessons;

        public BottomLessonNavigation(ILesson lessons)
        {
            _lessons = lessons;
        }
        public async Task<IViewComponentResult> InvokeAsync(int courseId, int lessonId)
        {
            Lesson previousLesson = await _lessons.GetPreviousLessonByCourseAndLessonIdAsync(courseId, lessonId);
            Lesson nextLesson = await _lessons.GetNextLessonByCourseAndLessonIdAsync(courseId, lessonId);
            var prevAndNextLessons = new Lesson[2];
            prevAndNextLessons[0] = previousLesson;
            prevAndNextLessons[1] = nextLesson;
   
            return View("BottomLessonNavigation",prevAndNextLessons);
        }
    }
}
