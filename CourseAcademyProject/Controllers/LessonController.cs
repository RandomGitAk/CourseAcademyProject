using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models;
using Microsoft.AspNetCore.Mvc;
using CourseAcademyProject.Data.Helpers;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace CourseAcademyProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LessonController : Controller
    {
        private readonly ILesson _lessons;
        private readonly ICourse _courses; 
        private readonly IWebHostEnvironment _appEnvironment;

        public LessonController(ILesson lesson, IWebHostEnvironment appEnvironment, ICourse courses)
        {
            _lessons = lesson;
            _appEnvironment = appEnvironment;
            _courses = courses;
        }

        [Route("/panel/course-lesson")]
        public IActionResult CourseLessons(QueryOptions options, int courseId)
        {
            return View(_lessons.GetAllLessonsByCourseId(options,courseId));
        }

        [Route("/panel/lessons")]
        public IActionResult Lessons(QueryOptions options)
        {
            return View(nameof(CourseLessons),_lessons.GetAllLessons(options));
        }

        [Route("/panel/delete-lesson")]
        [HttpDelete]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            var currentLesson = await _lessons.GetLessonByIdAsync(lessonId);
            if (currentLesson != null)
            {
                if (currentLesson.FileLessonContent != null)
                {
                    if (System.IO.File.Exists(_appEnvironment.WebRootPath + currentLesson.FileLessonContent))
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + currentLesson.FileLessonContent);
                    }
                }
                await _lessons.DeleteLessonAsync(currentLesson);
            }
            return Ok();
        }

        [Route("/panel/create-lesson")]
        [HttpGet]
        public async Task<IActionResult> CreateLesson()
        {
            var courses = await _courses.GetAllCourseAsync();
            return View(new LessonViewModel
            {
                AllCourses = new SelectList(courses, "Id", "Name")
            });
        }

        [Route("/panel/create-lesson")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateLesson(LessonViewModel lessonViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentLesson = new Lesson
                {
                    Title = lessonViewModel.Title!,
                    Information = lessonViewModel.Information!,
                    LessonContent = lessonViewModel.LessonContent!,
                    VideoUrl = lessonViewModel.VideoUrl!,
                    MaterialsUrl = lessonViewModel.MaterialsUrl,
                    CourseId = lessonViewModel.CourseId
                };
                if (lessonViewModel.File != null && lessonViewModel.File.Length > 0)
                {
                    string fileName = lessonViewModel.File.FileName;
                    string filePath = FileService.СreateFilePathFromFileName(fileName, foldername: "lessonMaterials");
                    currentLesson.FileLessonContent = filePath;
                    await FileService.SaveFile(filePath, lessonViewModel.File, _appEnvironment);
                }
                await _lessons.AddLessonAsync(currentLesson);
                return RedirectToAction(nameof(CourseLessons), new {courseId = lessonViewModel.CourseId});
            }
            return View(lessonViewModel);
        }

        [Route("/panel/edit-lesson")]
        [HttpGet]
        public async Task<IActionResult> EditLesson(int lessonId)
        {
            var currentLesson = await _lessons.GetLessonByIdAsync(lessonId);
            if (currentLesson != null)
            {
                var courses = await _courses.GetAllCourseAsync();
                LessonViewModel lessonViewModel = new LessonViewModel
                {
                    Id = currentLesson.Id,
                    Title = currentLesson.Title,
                    Information = currentLesson.Information,
                    LessonContent = currentLesson.LessonContent,
                    VideoUrl = currentLesson.VideoUrl,
                    MaterialsUrl = currentLesson.MaterialsUrl,
                    CourseId = currentLesson.CourseId,
                    FileLessonContent = currentLesson.FileLessonContent,
                    AllCourses = new SelectList(courses, "Id", "Name")
                };
                return View(lessonViewModel);
            }
            return NotFound();
        }

        [Route("/panel/edit-lesson")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditLesson(LessonViewModel lessonViewModel)
        {
            if (ModelState.IsValid)
            {
                var selectedLesson = await _lessons.GetLessonByIdAsync(lessonViewModel.Id ?? 0);
                if (selectedLesson == null)
                {
                    return NotFound();
                }
                var currentLesson = new Lesson
                {
                    Id = lessonViewModel.Id ?? 0,
                    Title = lessonViewModel.Title!,
                    Information = lessonViewModel.Information!,
                    LessonContent = lessonViewModel.LessonContent,
                    VideoUrl = lessonViewModel.VideoUrl,
                    MaterialsUrl = lessonViewModel.MaterialsUrl,
                    FileLessonContent = lessonViewModel.FileLessonContent,
                    CourseId = lessonViewModel.CourseId,
                };
                if (lessonViewModel.File != null && lessonViewModel.File.Length > 0)
                {
                    if (selectedLesson.FileLessonContent != null)
                    {
                        if (System.IO.File.Exists(_appEnvironment.WebRootPath + selectedLesson.FileLessonContent))
                        {
                            System.IO.File.Delete(_appEnvironment.WebRootPath + selectedLesson.FileLessonContent);
                        }
                    }
                    string fileName = lessonViewModel.File.FileName;
                    string filePath = FileService.СreateFilePathFromFileName(fileName, foldername: "lessonMaterials");
                    currentLesson.FileLessonContent = filePath;
                    await FileService.SaveFile(filePath, lessonViewModel.File, _appEnvironment);
                }
                await _lessons.EditLessonAsync(currentLesson);
                return RedirectToAction(nameof(CourseLessons), new { courseId = lessonViewModel.CourseId });
            }
            return View(lessonViewModel);
        }
    }
}
