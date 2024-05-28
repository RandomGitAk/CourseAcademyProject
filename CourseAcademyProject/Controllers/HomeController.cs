using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.ViewModels;
using CourseAcademyProject.ViewModels.Test;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.IO;

namespace CourseAcademyProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourse _courses;
        private readonly ILesson _lessons;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IComment _comments;
        private readonly ICategory _categories;
        private readonly IUserCourse _userCourse;
        private readonly IBlog _blogs;
        private readonly UserManager<User> _userManager;

        public HomeController(ICategory categories, ICourse courses, ILesson lesson, IWebHostEnvironment appEnvironment, IComment comment, 
            UserManager<User> userManager, IBlog blog)
        {
            _categories = categories;
            _courses = courses;
            _lessons = lesson;
            _appEnvironment = appEnvironment;
            _comments = comment;
            _userManager = userManager;
            _blogs = blog;
        }

        [Route("/")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/courses")]
        [HttpGet]
        public IActionResult Courses(QueryOptions options)
        {
            return View(_courses.GetAllCourses(options));
        }

        [Route("/course-detail")]
        [HttpGet]
        public async Task<IActionResult> CourseDetail(int courseId)
        {
            if (courseId != 0)
            {
                Course currentCourse = await _courses.GetCourseWithLessonsAndCommentsByIdAsync(courseId);
                if (currentCourse != null)
                {
                   return View(currentCourse);
                }
            }
            return NotFound();
        }

       
        [Route("/lesson")]
        [HttpGet]
        public async Task<IActionResult> Lesson(int lessonId)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (lessonId != 0)
                {
                    Lesson currentLesson = await _lessons.GetLessonByIdAsync(lessonId);
                    if (currentLesson != null)
                    {
                        return View(currentLesson);
                    }
                }
                return NotFound();
            }
           return Unauthorized();
        }

        [Route("/download-lesson-file")]
        [HttpGet]
        public async Task<IActionResult> DownloadLessonFile(int lessonId)
        {
            if (lessonId != 0)
            {
                Lesson currentLesson = await _lessons.GetLessonByIdAsync(lessonId);
                if (currentLesson != null)
                {
                    var fileProvider = new PhysicalFileProvider(_appEnvironment.WebRootPath);
                    var fileInfo = fileProvider.GetFileInfo(currentLesson.FileLessonContent);
                    if (!fileInfo.Exists)
                    {
                        return NotFound();
                    }

                    var fileStream = fileInfo.CreateReadStream();
                    string returnFileNameWithoutGuid = currentLesson.FileLessonContent.Split('/')[^1].Substring(36);
                    return File(fileStream, "application/octet-stream", returnFileNameWithoutGuid);
                }
            }
            return NotFound();
        }

        [Route("/add-comment")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddComment(CommentViewModel comment)
        {
            if (ModelState.IsValid)
            {
                User currentUser = await _userManager.FindByNameAsync(comment.UserName);
                
                await _comments.AddCommentAsync(new Comment
                {
                    Content = comment.Content,
                    CourseId = comment.CourseId,
                    ParentCommentId = comment.ParentCommentId,
                    UserId =  currentUser.Id
                });
                return RedirectToAction(nameof(CourseDetail), new {courseId = comment.CourseId});
            }
            return NotFound();
        }

        [Route("/blogs")]
        [HttpGet]
        public IActionResult Blogs(QueryOptions options)
        {
            return View(_blogs.GetAllBlogs(options));
        }

        [Route("/blog")]
        [HttpGet]
        public async Task<IActionResult> Blog(int blogId)
        {
            return View(await _blogs.GetBlogByIdAsync(blogId));
        }
    }
}
