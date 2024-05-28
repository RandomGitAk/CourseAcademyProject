using CourseAcademyProject.Controllers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Http;

namespace CourseAcademy.Tests
{
    public class LessonControllerTests
    {
        [Fact]
        public void CourseLessons_ReturnsViewResult_WithValidCourseId()
        {
            // Arrange
            int courseId = 1;
            var mockLessons = new Mock<ILesson>();
            var controller = new LessonController(mockLessons.Object, null, null);
            var lessonsList = new List<Lesson> { new Lesson { Id = 1, Title = "Lesson 1" }, new Lesson { Id = 2, Title = "Lesson 2" } };

            var options = new QueryOptions();
            mockLessons.Setup(c => c.GetAllLessonsByCourseId(options,courseId))
                          .Returns(new PagedList<Lesson>(lessonsList.AsQueryable(), options));

            // Act
            var result = controller.CourseLessons(options, courseId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Lesson>>(viewResult.Model);
            Assert.Equal(lessonsList, model);
        }

        [Fact]
        public void Lessons_ReturnsViewResult()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var controller = new LessonController(mockLessons.Object, null, null);
            var lessonsList = new List<Lesson> { new Lesson { Id = 1, Title = "Lesson 1" }, new Lesson { Id = 2, Title = "Lesson 2" } };
            var options = new QueryOptions();
            mockLessons.Setup(c => c.GetAllLessons(options))
                          .Returns(new PagedList<Lesson>(lessonsList.AsQueryable(), options));

            // Act
            var result = controller.Lessons(options);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(nameof(LessonController.CourseLessons), viewResult.ViewName);
            var model = Assert.IsAssignableFrom<IEnumerable<Lesson>>(viewResult.Model);
            Assert.Equal(lessonsList.Count(), model.Count());
        }

        [Fact]
        public async Task DeleteLesson_ExistingLesson_ReturnsOkResult()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new LessonController(mockLessons.Object, mockAppEnvironment.Object, null);
            var lessonId = 1;
            var currentLesson = new Lesson { Id = lessonId, Title = "Test Lesson" };
            mockLessons.Setup(l => l.GetLessonByIdAsync(lessonId)).ReturnsAsync(currentLesson);

            // Act
            var result = await controller.DeleteLesson(lessonId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            mockLessons.Verify(l => l.DeleteLessonAsync(currentLesson), Times.Once);
        }

        [Fact]
        public async Task DeleteLesson_NonExistingLesson_DeleteLessonAsyncTimesNever()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new LessonController(mockLessons.Object, mockAppEnvironment.Object, null);
            var lessonId = 1;
            mockLessons.Setup(l => l.GetLessonByIdAsync(lessonId)).ReturnsAsync((Lesson)null);

            // Act
            var result = await controller.DeleteLesson(lessonId);

            // Assert
            mockLessons.Verify(l => l.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never);
        }

        [Fact]
        public async Task CreateLesson_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var mockCourses = new Mock<ICourse>();
            var controller = new LessonController(null, null, mockCourses.Object);
            var courseList = new List<Course>
            {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" },
                new Course { Id = 3, Name = "Course 3" }
            };
            mockCourses.Setup(c => c.GetAllCourseAsync()).ReturnsAsync(courseList);

            // Act
            var result = await controller.CreateLesson();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<LessonViewModel>(viewResult.ViewData.Model);
            Assert.Equal(courseList.Count, model.AllCourses.Count());
        }

        [Fact]
        public async Task CreateLesson_ValidModel_RedirectsToCourseLessons()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var controller = new LessonController(mockLessons.Object, null, null);
            var viewModel = new LessonViewModel
            {
                Title = "Lesson Title",
                Information = "Lesson Information",
                LessonContent = "Lesson Content",
                VideoUrl = "http://example.com",
                MaterialsUrl = "http://example.com/materials",
                CourseId = 1
            };

            // Act
            var result = await controller.CreateLesson(viewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("CourseLessons", redirectToActionResult.ActionName);
            Assert.Equal(viewModel.CourseId, redirectToActionResult.RouteValues["courseId"]);

            mockLessons.Verify(lessons => lessons.AddLessonAsync(It.IsAny<Lesson>()), Times.Once);
        }

        [Fact]
        public async Task CreateLesson_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var controller = new LessonController(mockLessons.Object, null, null);
            var viewModel = new LessonViewModel();
            controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await controller.CreateLesson(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task EditLesson_LessonExists_ReturnsViewResultWithLessonViewModel()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var mockCourses = new Mock<ICourse>();
            var controller = new LessonController(mockLessons.Object, null, mockCourses.Object);
            var lessonId = 1;
            var lesson = new Lesson { Id = lessonId, Title = "Lesson Title" };
            var courses = new[] { new Course { Id = 1, Name = "Course 1" } };
            mockLessons.Setup(lessons => lessons.GetLessonByIdAsync(lessonId)).ReturnsAsync(lesson);
            mockCourses.Setup(courses => courses.GetAllCourseAsync()).ReturnsAsync(courses);

            // Act
            var result = await controller.EditLesson(lessonId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LessonViewModel>(viewResult.Model);
            Assert.Equal(lesson.Id, model.Id);
            Assert.Equal(lesson.Title, model.Title);
            Assert.NotNull(model.AllCourses);
            Assert.NotEmpty(model.AllCourses);
        }

        [Fact]
        public async Task EditLesson_LessonDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var controller = new LessonController(mockLessons.Object, null, null);
            var lessonId = 1;
            mockLessons.Setup(lessons => lessons.GetLessonByIdAsync(lessonId)).ReturnsAsync((Lesson)null);

            // Act
            var result = await controller.EditLesson(lessonId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditLesson_ValidModel_ReturnsRedirectToAction()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new LessonController(mockLessons.Object, mockAppEnvironment.Object, null);
            var lessonViewModel = new LessonViewModel
            {
                Id = 1,
                Title = "New Lesson Title",
                Information = "New Lesson Information",
                LessonContent = "New Lesson Content",
                VideoUrl = "https://example.com/new-video",
                MaterialsUrl = "https://example.com/new-materials",
                CourseId = 1,
                FileLessonContent = "/lessonMaterials/new-lesson-file.txt",
                File = null
            };
            var selectedLesson = new Lesson
            {
                Id = 1,
                Title = "Old Lesson Title",
                Information = "Old Lesson Information",
                LessonContent = "Old Lesson Content",
                VideoUrl = "https://example.com/old-video",
                MaterialsUrl = "https://example.com/old-materials",
                CourseId = 1,
                FileLessonContent = "/lessonMaterials/old-lesson-file.txt"
            };
            mockLessons.Setup(lessons => lessons.GetLessonByIdAsync(lessonViewModel.Id ?? 0)).ReturnsAsync(selectedLesson);

            // Act
            var result = await controller.EditLesson(lessonViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("CourseLessons", redirectToActionResult.ActionName);
            Assert.Equal(lessonViewModel.CourseId, redirectToActionResult.RouteValues["courseId"]);
            mockLessons.Verify(lessons => lessons.EditLessonAsync(It.IsAny<Lesson>()), Times.Once);
        }

        [Fact]
        public async Task EditLesson_InvalidModel_ReturnsViewResultWithLessonViewModel()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var controller = new LessonController(mockLessons.Object, null, null);
            var lessonViewModel = new LessonViewModel { };
            controller.ModelState.AddModelError("Title", "Title is required.");

            // Act
            var result = await controller.EditLesson(lessonViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LessonViewModel>(viewResult.Model);
            Assert.Equal(lessonViewModel, model);
            mockLessons.Verify(lessons => lessons.EditLessonAsync(It.IsAny<Lesson>()), Times.Never);
        }

        [Fact]
        public async Task EditLesson_LessonNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockLessons = new Mock<ILesson>();
            var controller = new LessonController(mockLessons.Object, null, null);
            var lessonViewModel = new LessonViewModel { Id = 1 };
            mockLessons.Setup(lessons => lessons.GetLessonByIdAsync(lessonViewModel.Id ?? 0)).ReturnsAsync((Lesson)null);

            // Act
            var result = await controller.EditLesson(lessonViewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockLessons.Verify(lessons => lessons.EditLessonAsync(It.IsAny<Lesson>()), Times.Never);
        }
    }
}

