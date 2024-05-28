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
    public class CourseControllerTests
    {
        private readonly string _pathForBlogImages = "D:\\Vova\\CourseAcademy\\CourseAcademyProject\\CourseAcademyProject\\wwwroot";
        [Fact]
        public void Courses_ReturnsViewResult()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockCategoryService = new Mock<ICategory>();
            var controller = new CourseController(mockCourseService.Object, null, mockCategoryService.Object);

            var courseList = new List<Course>
            {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" },
                new Course { Id = 3, Name = "Course 3" }
            };

            var options = new QueryOptions();
            mockCourseService.Setup(c => c.GetAllCourses(options))
                             .Returns(new PagedList<Course>(courseList.AsQueryable(),options));

            // Act
            var result = controller.Courses(options);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.ViewData.Model);
            Assert.Equal(courseList.Count, model.Count());
        }

        [Fact]
        public async Task DeleteCourse_ReturnsOkResult()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CourseController(mockCourseService.Object, mockEnvironment.Object, null);

            var courseId = 1;
            var currentCourse = new Course { Id = courseId, Image = "image_path.jpg" };
            mockCourseService.Setup(c => c.GetCourseByIdAsync(courseId))
                             .ReturnsAsync(currentCourse);

            // Act
            var result = await controller.DeleteCourse(courseId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            mockCourseService.Verify(c => c.DeleteCourseAsync(currentCourse), Times.Once);
        }

        [Fact]
        public async Task DeleteCourse_InvalidCourseId_DeleteCourseAsyncTimesNever()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CourseController(mockCourseService.Object, mockEnvironment.Object, null);

            var courseId = 1;
            mockCourseService.Setup(x => x.GetCourseByIdAsync(courseId)).ReturnsAsync((Course)null);

            // Act
            var result = await controller.DeleteCourse(courseId);

            // Assert
            mockCourseService.Verify(x => x.DeleteCourseAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task CreateCourse_ReturnsViewResult_WithCategories()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockCategoryService = new Mock<ICategory>();
            var controller = new CourseController(mockCourseService.Object, null, mockCategoryService.Object);

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" },
                new Category { Id = 3, Name = "Category 3" }
            };
            mockCategoryService.Setup(c => c.GetAllCategoriesAsync())
                               .ReturnsAsync(categories);

            // Act
            var result = await controller.CreateCourse();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);
            Assert.Equal(categories.Count, model.AllCategories.Count());
        }

        [Fact]
        public async Task CreateCourse_ValidModel_RedirectsToCourses()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CourseController(mockCourseService.Object, mockAppEnvironment.Object, null);

            var model = new CourseViewModel
            {
                Name = "Test Course",
                AboutCourse = "Test Description",
            };

            // Act
            var result = await controller.CreateCourse(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Courses", redirectToActionResult.ActionName);
            mockCourseService.Verify(x => x.AddCourseAsync(It.IsAny<Course>()), Times.Once);
        }

        [Fact]
        public async Task CreateCourse_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CourseController(mockCourseService.Object, mockAppEnvironment.Object, null);
            controller.ModelState.AddModelError("Name", "Name is required");

            var model = new CourseViewModel();

            // Act
            var result = await controller.CreateCourse(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task EditCourse_ExistingCourse_ReturnsViewResultWithCourseViewModel()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockCategoryService = new Mock<ICategory>();
            var controller = new CourseController(mockCourseService.Object, null, mockCategoryService.Object);

            var existingCourseId = 1;
            var existingCourse = new Course
            {
                Id = existingCourseId,
                Name = "Existing Course",
            };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            mockCourseService.Setup(c => c.GetCourseByIdAsync(existingCourseId)).ReturnsAsync(existingCourse);
            mockCategoryService.Setup(c => c.GetAllCategoriesAsync()).ReturnsAsync(categories);

            // Act
            var result = await controller.EditCourse(existingCourseId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CourseViewModel>(viewResult.Model);
            Assert.Equal(existingCourseId, model.Id);
        }

        [Fact]
        public async Task EditCourse_NonExistingCourse_ReturnsNotFoundResult()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockCategoryService = new Mock<ICategory>();
            var controller = new CourseController(mockCourseService.Object, null, mockCategoryService.Object);

            var nonExistingCourseId = 999; 

            mockCourseService.Setup(c => c.GetCourseByIdAsync(nonExistingCourseId)).ReturnsAsync((Course)null);

            // Act
            var result = await controller.EditCourse(nonExistingCourseId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditCourse_ValidModelState_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CourseController(mockCourseService.Object, mockAppEnvironment.Object,mockCategoryService.Object);

            var courseViewModel = new CourseViewModel
            {
                Id = 1,
                Name = "Updated Course",
                File = null
            };

            var existingCourse = new Course
            {
                Id = 1,
                Name = "Old Course",
                Image = "/courseImages/oldimage.jpg"
            };
            mockCourseService.Setup(c => c.GetCourseByIdAsync(1)).ReturnsAsync(existingCourse);

            // Act
            var result = await controller.EditCourse(courseViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Courses", redirectToActionResult.ActionName);
            mockCourseService.Verify(x => x.EditCourseAsync(It.IsAny<Course>()), Times.Once);
        }

        [Fact]
        public async Task EditCourse_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var mockCourseService = new Mock<ICourse>();
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CourseController(mockCourseService.Object, mockAppEnvironment.Object, mockCategoryService.Object);

            var courseViewModel = new CourseViewModel
            {
                Id = 1,
                Name = "",
                File = null
            };

            controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await controller.EditCourse(courseViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CourseViewModel>(viewResult.ViewData.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("", model.Name);
            mockCourseService.Verify(x => x.EditCourseAsync(It.IsAny<Course>()), Times.Never);
        }
    }
}
