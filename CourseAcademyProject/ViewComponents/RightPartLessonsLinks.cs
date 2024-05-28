using CourseAcademyProject.Interfaces;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CourseAcademyProject.ViewComponents
{
    public class RightPartLessonsLinks : ViewComponent
    {
        private readonly ILesson _lessons;

        public RightPartLessonsLinks(ILesson lessons)
        {
            _lessons = lessons;
        }
        public async Task<IViewComponentResult> InvokeAsync(int courseId, int lessonId)
        {
            var lessons = await _lessons.GetFiveRandomLessonsIdByCourseIdAsync(courseId, lessonId);
            var currentLesson = await _lessons.GetLessonByIdAsync(lessonId);
            return View("RightPartLessonsLinks", new LessonsWithCurrentLessonViewModel
            {
                Lessons = lessons,
                Lesson = currentLesson
            });
        }
    }
}
