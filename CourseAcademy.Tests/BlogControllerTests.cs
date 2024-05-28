using CourseAcademyProject.Controllers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CourseAcademy.Tests
{
    public class BlogControllerTests
    {
        private readonly string _pathForBlogImages = "D:\\Vova\\CourseAcademy\\CourseAcademyProject\\CourseAcademyProject\\wwwroot";
        [Fact]
        public void BlogsReturns_Returns_ViewResultWithListOfBlogs()
        {
            // Arrange
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new BlogController(mockBlogService.Object,mockEnvironment.Object);
            var options = new QueryOptions();

            mockBlogService.Setup(repo => repo.GetAllBlogs(It.IsAny<QueryOptions>()))
              .Returns(new PagedList<Blog>(GetTestBlogs(), new QueryOptions()));

            // Act
            var result = controller.Blogs(new QueryOptions()) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<PagedList<Blog>>(result.ViewData.Model);
            Assert.Equal(GetTestBlogs().ToList().Count(), model.Count());
        }


        [Fact]
        public void CreateBlog_Returns_ViewResult()
        {
            // Arrange
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new BlogController(mockBlogService.Object, mockEnvironment.Object);

            // Act
            var result = controller.CreateBlog();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CreateBlog_ValidModelState_RedirectToAction()
        {
            // Arrange
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns(_pathForBlogImages);

            var controller = new BlogController(mockBlogService.Object, mockEnvironment.Object);

            var blogViewModel = new BlogViewModel
            {
                Name = "Test Blog",
                MiniDescription = "Test Description",
                Content = "Test Content",
                File = new FormFile(new MemoryStream(), 0, 0, "TestFile", "test.jpg")
            };

            // Act
            var result = await controller.CreateBlog(blogViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(BlogController.Blogs), result.ActionName);
        }

        [Fact]
        public async Task CreateBlog_InvalidModelState_ReturnsView()
        {
            // Arrange
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new BlogController(mockBlogService.Object, mockEnvironment.Object);
            controller.ModelState.AddModelError("Name", "Name is required");

            var blogViewModel = new BlogViewModel();

            // Act
            var result = await controller.CreateBlog(blogViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(blogViewModel, result.Model);
        }

        [Fact]
        public async Task EditBlog_ReturnsView_WithValidBlogId()
        {
            // Arrange
            int validBlogId = 1;
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new BlogController(mockBlogService.Object, mockEnvironment.Object);

            var existingBlog = new Blog
            {
                Id = validBlogId,
                Name = "Test Blog",
                Image = "test.jpg",
                MiniDescription = "Test Description",
                Content = "Test Content"
            };

            mockBlogService.Setup(x => x.GetBlogByIdAsync(validBlogId)).ReturnsAsync(existingBlog);

            // Act
            var result = await controller.EditBlog(validBlogId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BlogViewModel>(viewResult.ViewData.Model);
            Assert.Equal(existingBlog.Id, model.Id);
            Assert.Equal(existingBlog.Name, model.Name);
            Assert.Equal(existingBlog.Image, model.Image);
            Assert.Equal(existingBlog.MiniDescription, model.MiniDescription);
            Assert.Equal(existingBlog.Content, model.Content);
        }

        [Fact]
        public async Task EditBlog_ReturnsNotFound_WithInvalidBlogId()
        {
            // Arrange
            int invalidBlogId = 999; // Assuming this id does not exist
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new BlogController(mockBlogService.Object, mockEnvironment.Object);

            // Return null for a non-existing blog id
            mockBlogService.Setup(x => x.GetBlogByIdAsync(invalidBlogId)).ReturnsAsync((Blog)null);

            // Act
            var result = await controller.EditBlog(invalidBlogId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditBlog_ReturnsRedirectToAction_WithValidModelState()
        {
            // Arrange
            var validBlogId = 1;
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns(_pathForBlogImages);
            var controller = new BlogController(mockBlogService.Object, mockEnvironment.Object);

            var blogViewModel = new BlogViewModel
            {
                Id = validBlogId,
                Name = "Updated Blog Name",
                MiniDescription = "Updated Description",
                Content = "Updated Content",
                File = new FormFile(new MemoryStream(), 0, 0, "TestFile", "test.jpg")
            };

            var existingBlog = new Blog
            {
                Id = validBlogId,
                Name = "Old Blog Name",
                MiniDescription = "Old Description",
                Content = "Old Content",
                Image = "old.jpg"
            };

            mockBlogService.Setup(x => x.GetBlogByIdAsync(validBlogId)).ReturnsAsync(existingBlog);

            // Act
            var result = await controller.EditBlog(blogViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(BlogController.Blogs), result.ActionName);
        }

        [Fact]
        public async Task EditBlog_ReturnsView_WithInvalidModelState()
        {
            // Arrange
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new BlogController(mockBlogService.Object, mockEnvironment.Object);
            controller.ModelState.AddModelError("Name", "Name is required");

            var blogViewModel = new BlogViewModel();

            // Act
            var result = await controller.EditBlog(blogViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(blogViewModel, result.Model);
        }

        [Fact]
        public async Task EditBlog_ReturnsNotFound_WithNonExistingBlogId()
        {
            // Arrange
            var nonExistingBlogId = 999;
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new BlogController(mockBlogService.Object, mockEnvironment.Object);

            var blogViewModel = new BlogViewModel
            {
                Id = nonExistingBlogId,
                Name = "Updated Blog Name",
                MiniDescription = "Updated Description",
                Content = "Updated Content"
            };

            // Return null for a non-existing blog id
            mockBlogService.Setup(x => x.GetBlogByIdAsync(nonExistingBlogId)).ReturnsAsync((Blog)null);

            // Act
            var result = await controller.EditBlog(blogViewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteBlog_ReturnsOkResult_WhenBlogExists()
        {
            // Arrange
            var validBlogId = 1;
            var mockBlogService = new Mock<IBlog>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns(_pathForBlogImages);
            var controller = new BlogController(mockBlogService.Object, mockEnvironment.Object);

            var existingBlog = new Blog
            {
                Id = validBlogId,
                Name = "Existing Blog",
                MiniDescription = "Existing Description",
                Content = "Existing Content",
                Image = "existing.jpg"
            };

            mockBlogService.Setup(x => x.GetBlogByIdAsync(validBlogId)).ReturnsAsync(existingBlog);

            // Act
            var result = await controller.DeleteBlog(validBlogId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockBlogService.Verify(x => x.DeleteBlogAsync(existingBlog), Times.Once);
        }

        private IQueryable<Blog> GetTestBlogs()
        {
            var products = new List<Blog>
            {
                new Blog { Id = 1, Name = "Blog 1", Content = "Content 1" },
                new Blog { Id = 2, Name = "Blog 2", Content = "Content 2" },
                new Blog { Id = 3, Name = "Blog 3", Content = "Content 3" }
            };
            return products.AsQueryable();
        }
    }
}
