using CourseAcademyProject.Controllers;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CourseAcademyProject.ViewModels;
using CourseAcademyProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CourseAcademy.Tests
{
    public class PanelControllerTests
    {
        [Fact]
        public void Users_ReturnsViewResultWithUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "1", UserName = "user1@example.com" },
                new User { Id = "2", UserName = "user2@example.com" },
                new User { Id = "3", UserName = "user3@example.com" }
            };
            var mockUserManager = MockUserManager<User>();

            mockUserManager.Setup(x => x.Users).Returns(users.AsQueryable());
            var controller = new PanelController(mockUserManager.Object);
            // Act
            var result = controller.Users(new QueryOptions());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.ViewData.Model);
            Assert.Equal(users.Count, model.Count());
        }

        [Fact]
        public async Task CreateOrUpdateUser_WithNullUserId_ReturnsViewWithNewViewModel()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var controller = new PanelController(mockUserManager.Object);

            // Act
            var result = await controller.CreateOrUpdateUser((string)null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateOrUpdateUserViewModel>(viewResult.ViewData.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public async Task CreateOrUpdateUser_WithValidUserId_ReturnsViewWithModel()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var controller = new PanelController(mockUserManager.Object);
            var userId = "1";
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };
            mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await controller.CreateOrUpdateUser(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateOrUpdateUserViewModel>(viewResult.ViewData.Model);
            Assert.Equal(userId, model.Id);
            Assert.Equal(user.Email, model.Email);
            Assert.Equal(user.FirstName, model.FirstName);
            Assert.Equal(user.LastName, model.LastName);
        }

        [Fact]
        public async Task CreateOrUpdateUser_WithInvalidUserId_ReturnsNotFound()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var controller = new PanelController(mockUserManager.Object);
            var invalidUserId = "invalid_id";
            mockUserManager.Setup(u => u.FindByIdAsync(invalidUserId)).ReturnsAsync((User)null);

            // Act
            var result = await controller.CreateOrUpdateUser(invalidUserId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateOrUpdateUser_WithValidModelAndNullId_RedirectsToUsers()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                   .ReturnsAsync(IdentityResult.Success);
            var controller = new PanelController(mockUserManager.Object);
            var model = new CreateOrUpdateUserViewModel
            {
                Id = null,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123"
            };

            // Act
            var result = await controller.CreateOrUpdateUser(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task CreateOrUpdateUser_WithValidModelAndExistingEmail_ReturnsViewWithError()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
            var controller = new PanelController(mockUserManager.Object);
            var model = new CreateOrUpdateUserViewModel
            {
                Id = null,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123"
            };

            // Act
            var result = await controller.CreateOrUpdateUser(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<CreateOrUpdateUserViewModel>(viewResult.Model);
            Assert.Equal("This e-mail address is already taken.", viewResult.ViewData.ModelState["Email"].Errors[0].ErrorMessage);
            Assert.Equal(model, viewModel);
        }


        [Fact]
        public async Task CreateOrUpdateUser_WithValidModelAndExistingId_RedirectsToUsers()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User() { Email = "0@example.com" });
            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                  .ReturnsAsync(IdentityResult.Success);
            var controller = new PanelController(mockUserManager.Object);
            var model = new CreateOrUpdateUserViewModel
            {
                Id = "1",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await controller.CreateOrUpdateUser(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task CreateOrUpdateUser_WithInvalidModel_ReturnsViewWithModelError()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var controller = new PanelController(mockUserManager.Object);
            controller.ModelState.AddModelError("Name", "Name is required");
            var model = new CreateOrUpdateUserViewModel(); 

            // Act
            var result = await controller.CreateOrUpdateUser(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<CreateOrUpdateUserViewModel>(viewResult.Model);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.Equal(model, viewModel);
        }

        [Fact]
        public async Task DeleteUser_ValidUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = "1";
            var mockUserManager = MockUserManager<User>();
            var controller = new PanelController(mockUserManager.Object);
            var userToDelete = new User { Id = userId };
            mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(userToDelete);
            mockUserManager.Setup(m => m.DeleteAsync(userToDelete)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.DeleteUser(userId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockUserManager.Verify(m => m.DeleteAsync(userToDelete), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsOkResult()
        {
            // Arrange
            var userId = "1";
            var mockUserManager = MockUserManager<User>();
            var controller = new PanelController(mockUserManager.Object);
            User userToDelete = null;
            mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(userToDelete);

            // Act
            var result = await controller.DeleteUser(userId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public void Index_ReturnsViewResultWithMainAdminPageViewModel()
        {
            // Arrange

            var mockUserManager = MockUserManager<User>();
            var controller = new PanelController(mockUserManager.Object);

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>()
             .UseSqlServer("Server=DESKTOP-4HEB46F;Database=academyDb;Trusted_Connection=True;TrustServerCertificate=True;");
            // Act
            var result = controller.Index(new ApplicationContext(optionsBuilder.Options));

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MainAdminPageViewModel>(viewResult.ViewData.Model);
        }

       
        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            return userManager;
        }
    }
}

