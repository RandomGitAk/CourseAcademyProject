using CourseAcademyProject.Controllers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CourseAcademy.Tests
{
    public class HomeControllerTests
    {
        private readonly string _pathForBlogImages = "D:\\Vova\\CourseAcademy\\CourseAcademyProject\\CourseAcademyProject\\wwwroot";
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var mockCategories = new Mock<ICategory>();
            var mockCourses = new Mock<ICourse>();
            var mockLessons = new Mock<ILesson>();
            var mockComments = new Mock<IComment>();
            var mockBlogs = new Mock<IBlog>();
            var mockUserManager = MockUserManager<User>();

            var controller = new HomeController(mockCategories.Object, mockCourses.Object, mockLessons.Object,
                null, mockComments.Object, mockUserManager.Object, mockBlogs.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void Courses_ReturnsViewResultWithListOfCourses()
        {
            // Arrange
            var mockCourses = new Mock<ICourse>();
            var coursesList = new List<Course>
            {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" },
                new Course { Id = 3, Name = "Course 3" }
            };
            var options = new QueryOptions();

            mockCourses.Setup(c => c.GetAllCourses(options))
                          .Returns(new PagedList<Course>(coursesList.AsQueryable(), options));

            var controller = new HomeController(null, mockCourses.Object, null, null, null, null, null);

            // Act
            var result = controller.Courses(options);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.Model);
            Assert.Equal(coursesList, model);
        }

        [Fact]
        public async Task CourseDetail_ValidCourseId_ReturnsViewResultWithCourse()
        {
            // Arrange
            int courseId = 1;
            var mockCourses = new Mock<ICourse>();
            var expectedCourse = new Course { Id = courseId, Name = "Test Course" };
            mockCourses.Setup(c => c.GetCourseWithLessonsAndCommentsByIdAsync(courseId)).ReturnsAsync(expectedCourse);

            var controller = new HomeController(null, mockCourses.Object, null, null, null, null, null);

            // Act
            var result = await controller.CourseDetail(courseId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
            Assert.Equal(expectedCourse, model);
        }

        [Fact]
        public async Task CourseDetail_InvalidCourseId_ReturnsNotFoundResult()
        {
            // Arrange
            int courseId = 0; 
            var mockCourses = new Mock<ICourse>();
            mockCourses.Setup(c => c.GetCourseWithLessonsAndCommentsByIdAsync(courseId)).ReturnsAsync((Course)null);

            var controller = new HomeController(null, mockCourses.Object, null, null, null, null, null);

            // Act
            var result = await controller.CourseDetail(courseId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Lesson_AuthenticatedUser_ValidLessonId_ReturnsViewResultWithLesson()
        {
            // Arrange
            int lessonId = 1;
            var mockLessons = new Mock<ILesson>();
            var expectedLesson = new Lesson { Id = lessonId, Title = "Test Lesson" };
            mockLessons.Setup(l => l.GetLessonByIdAsync(lessonId)).ReturnsAsync(expectedLesson);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                 }, "mock"));

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new HomeController(null, null, mockLessons.Object, null, null, null, null)
            {
                ControllerContext = controllerContext
            };

            // Act
            var result = await controller.Lesson(lessonId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Lesson>(viewResult.ViewData.Model);
            Assert.Equal(expectedLesson, model);
        }

        [Fact]
        public async Task Lesson_AuthenticatedUser_InvalidLessonId_ReturnsNotFoundResult()
        {
            // Arrange
            int lessonId = 0; // Invalid lessonId
            var mockLessons = new Mock<ILesson>();
            mockLessons.Setup(l => l.GetLessonByIdAsync(lessonId)).ReturnsAsync((Lesson)null);


            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                 }, "mock"));

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new HomeController(null, null, mockLessons.Object, null, null, null, null)
            {
                ControllerContext = controllerContext
            };


            // Act
            var result = await controller.Lesson(lessonId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Lesson_UnauthenticatedUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.User = null;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new HomeController(null, null, null, null, null, null, null)
            {
                ControllerContext = controllerContext
            };

            // Ensure User.Identity.IsAuthenticated is false

            // Act
            var result = await controller.Lesson(1);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DownloadLessonFile_ValidLessonId_ReturnsFileResult()
        {
            // Arrange
            int lessonId = 1;
            var mockLessons = new Mock<ILesson>();
            var expectedLesson = new Lesson { Id = lessonId, FileLessonContent = "/lessonMaterials/bffec2e8-63b9-40d2-be73-c25e4f5c4126first.docx" };
            mockLessons.Setup(l => l.GetLessonByIdAsync(lessonId)).ReturnsAsync(expectedLesson);

            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(e => e.WebRootPath).Returns(_pathForBlogImages);
            var fileProvider = new PhysicalFileProvider(_pathForBlogImages);
            var fileInfo = fileProvider.GetFileInfo("/lessonMaterials/bffec2e8-63b9-40d2-be73-c25e4f5c4126first.docx");
            var fileStream = new MemoryStream(new byte[] { 0x20, 0x21 });
            var mockFileStream = new Mock<FileStreamResult>(fileStream, "application/octet-stream");

            var controller = new HomeController(null, null, mockLessons.Object,mockEnvironment.Object, null, null, null);

            // Act
            var result = await controller.DownloadLessonFile(lessonId);

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("application/octet-stream", fileResult.ContentType);
            Assert.Equal("first.docx", fileResult.FileDownloadName); 
        }

        [Fact]
        public async Task DownloadLessonFile_LessonFileNotExists_ReturnsNotFoundResult()
        {
            // Arrange
            int lessonId = 1;
            var mockLessons = new Mock<ILesson>();
            var expectedLesson = new Lesson { Id = lessonId, FileLessonContent = "/lessonMaterials/bffec2e8-63b9-40d2-be73-c25e4f5c4126first.docxf" };
            mockLessons.Setup(l => l.GetLessonByIdAsync(lessonId)).ReturnsAsync(expectedLesson);

            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(e => e.WebRootPath).Returns(_pathForBlogImages);

            var controller = new HomeController(null, null, mockLessons.Object, mockEnvironment.Object, null, null, null);

            // Act
            var result = await controller.DownloadLessonFile(lessonId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddComment_ValidModel_ReturnsRedirectToAction()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockComments = new Mock<IComment>();
            var controller = new HomeController(null, null, null, null, mockComments.Object, mockUserManager.Object, null);
            var commentViewModel = new CommentViewModel
            {
                Content = "Test Comment",
                CourseId = 1,
                ParentCommentId = 0,
                UserName = "testUser"
            };
            var currentUser = new User { Id = "userId" };
            mockUserManager.Setup(u => u.FindByNameAsync(commentViewModel.UserName)).ReturnsAsync(currentUser);

            // Act
            var result = await controller.AddComment(commentViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("CourseDetail", redirectToActionResult.ActionName);
            Assert.Equal(commentViewModel.CourseId, redirectToActionResult.RouteValues["courseId"]);
            mockComments.Verify(c => c.AddCommentAsync(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task AddComment_InvalidModel_ReturnsNotFoundResult()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockComments = new Mock<IComment>();
            var controller = new HomeController(null, null, null, null, mockComments.Object, mockUserManager.Object, null);
            controller.ModelState.AddModelError("Content", "Content is required");
            var commentViewModel = new CommentViewModel(); 

            // Act
            var result = await controller.AddComment(commentViewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Blogs_ReturnsViewResult()
        {
            // Arrange
            var mockBlogs = new Mock<IBlog>();
            var controller = new HomeController(null, null, null, null, null, null, mockBlogs.Object);
            var options = new QueryOptions();
            var blogList = new List<Blog> { new Blog { Id = 1, Name = "Test Blog 1" }, new Blog { Id = 2, Name = "Test Blog 2" } };
            var pagedList = new PagedList<Blog>(blogList.AsQueryable(), options);
            mockBlogs.Setup(b => b.GetAllBlogs(options)).Returns(pagedList);

            // Act
            var result = controller.Blogs(options);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PagedList<Blog>>(viewResult.ViewData.Model);
            Assert.Equal(blogList.Count, model.Count());
        }

        [Fact]
        public async Task Blog_ReturnsViewResult_WithValidBlogId()
        {
            // Arrange
            int blogId = 1;
            var mockBlogs = new Mock<IBlog>();
            var controller = new HomeController(null, null, null, null, null, null, mockBlogs.Object);
            var blog = new Blog { Id = blogId, Name = "Test Blog" };
            mockBlogs.Setup(b => b.GetBlogByIdAsync(blogId)).ReturnsAsync(blog);

            // Act
            var result = await controller.Blog(blogId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Blog>(viewResult.ViewData.Model);
            Assert.Equal(blog.Id, model.Id);
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            return userManager;
        }
    }
}
