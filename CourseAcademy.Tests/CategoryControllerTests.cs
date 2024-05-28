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
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CourseAcademy.Tests
{
    public class CategoryControllerTests
    {
        private readonly string _pathForBlogImages = "D:\\Vova\\CourseAcademy\\CourseAcademyProject\\CourseAcademyProject\\wwwroot";

        [Fact]
        public void Categories_ReturnsViewWithCategories()
        {
            // Arrange
            var mockCategories = new Mock<ICategory>();
            var categoriesList = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" },
                new Category { Id = 3, Name = "Category 3" }
            };
            var options = new QueryOptions();

            mockCategories.Setup(c => c.GetAllCategories(options))
                          .Returns(new PagedList<Category>(categoriesList.AsQueryable(), options));

            var controller = new CategoryController(mockCategories.Object, null);

            // Act
            var result = controller.Categories(options);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Category>>(viewResult.Model);
            Assert.Equal(categoriesList, model);
        }

        [Fact]
        public void CreateCategory_ReturnsView()
        {
            // Arrange
            var controller = new CategoryController(null, null);

            // Act
            var result = controller.CreateCategory();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); 
        }

        [Fact]
        public async Task CreateCategory_ValidModelState_RedirectsToCategories()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            mockAppEnvironment.Setup(m => m.WebRootPath).Returns(_pathForBlogImages);
            var controller = new CategoryController(mockCategoryService.Object, mockAppEnvironment.Object);
            var categoryViewModel = new CategoryViewModel
            {
                Name = "TestCategory",
                Description = "Test Description",
                File = new FormFile(Stream.Null, 0, 0, "testFile", "test.jpg")
            };

            // Act
            var result = await controller.CreateCategory(categoryViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Categories", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task CreateCategory_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CategoryController(mockCategoryService.Object, mockAppEnvironment.Object);
            var categoryViewModel = new CategoryViewModel();
            controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await controller.CreateCategory(categoryViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(categoryViewModel, viewResult.Model);
        }

        [Fact]
        public async Task EditCategory_CategoryFound_ReturnsViewWithModel()
        {
            // Arrange
            int categoryId = 1;
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CategoryController(mockCategoryService.Object, mockAppEnvironment.Object);
            var category = new Category
            {
                Id = categoryId,
                Name = "TestCategory",
                Description = "Test Description",
                Image = "/categoryImages/test.jpg"
            };
            mockCategoryService.Setup(x => x.GetCategoryByIdAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await controller.EditCategory(categoryId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CategoryViewModel>(viewResult.Model);
            Assert.Equal(categoryId, model.Id);
            Assert.Equal("TestCategory", model.Name);
            Assert.Equal("/categoryImages/test.jpg", model.Image);
            Assert.Equal("Test Description", model.Description);
        }

        [Fact]
        public async Task EditCategory_CategoryNotFound_ReturnsNotFound()
        {
            // Arrange
            int categoryId = 1;
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CategoryController(mockCategoryService.Object, mockAppEnvironment.Object);
            mockCategoryService.Setup(x => x.GetCategoryByIdAsync(categoryId)).ReturnsAsync((Category)null);

            // Act
            var result = await controller.EditCategory(categoryId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditCategory_ValidModel_ReturnsRedirectToAction()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CategoryController(mockCategoryService.Object, mockAppEnvironment.Object);
            var categoryViewModel = new CategoryViewModel
            {
                Id = 1,
                Name = "New Category Name",
                Description = "New Description",
                File = null 
            };
            var selectedCategory = new Category
            {
                Id = 1,
                Name = "Old Category Name",
                Description = "Old Description",
                Image = "/categoryImages/oldimage.jpg"
            };
            mockCategoryService.Setup(x => x.GetCategoryByIdAsync(1)).ReturnsAsync(selectedCategory);

            // Act
            var result = await controller.EditCategory(categoryViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Categories", redirectToActionResult.ActionName);
            mockCategoryService.Verify(x => x.EditCategoryAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task EditCategory_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CategoryController(mockCategoryService.Object, mockAppEnvironment.Object);
            var categoryViewModel = new CategoryViewModel
            {
                Id = 1,
                Name = "",
                Description = "New Description",
                File = null 
            };

            controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await controller.EditCategory(categoryViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CategoryViewModel>(viewResult.ViewData.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("", model.Name);
            mockCategoryService.Verify(x => x.EditCategoryAsync(It.IsAny<Category>()), Times.Never); 
        }

        [Fact]
        public async Task DeleteCategory_ValidCategoryId_ReturnsOk()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CategoryController(mockCategoryService.Object, mockAppEnvironment.Object);
            var categoryId = 1;
            var currentCategory = new Category
            {
                Id = categoryId,
                Name = "Test Category",
                Description = "Test Description",
                Image = "/categoryImages/testimage.jpg"
            };
            mockCategoryService.Setup(x => x.GetCategoryByIdAsync(categoryId)).ReturnsAsync(currentCategory);

            // Act
            var result = await controller.DeleteCategory(categoryId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            mockCategoryService.Verify(x => x.DeleteCategoryAsync(currentCategory), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_InvalidCategoryId_DeleteCategoryAsyncTimesNever()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategory>();
            var mockAppEnvironment = new Mock<IWebHostEnvironment>();
            var controller = new CategoryController(mockCategoryService.Object, mockAppEnvironment.Object);
            var categoryId = 1;
            mockCategoryService.Setup(x => x.GetCategoryByIdAsync(categoryId)).ReturnsAsync((Category)null);

            // Act
            var result = await controller.DeleteCategory(categoryId);

            // Assert
            mockCategoryService.Verify(x => x.DeleteCategoryAsync(It.IsAny<Category>()), Times.Never);
        }
    }
}
