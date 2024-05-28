using CourseAcademyProject.Controllers;
using CourseAcademyProject.Data.Helpers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Email;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class AccountControllerTests
    {
        [Fact]
        public void Login_ReturnsCorrectView_WhenNotAuthenticated()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var httpContext = new DefaultHttpContext();
            httpContext.User = null;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock)
            {
                ControllerContext = controllerContext,
            };

            // Act
            var result = controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<LoginViewModel>(viewResult.Model);
        }

        [Fact]
        public void Login_ReturnsRedirectToAction_WhenAuthenticated()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

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

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock)
            {
                ControllerContext = controllerContext,
            };

            // Act
            var result = controller.Login();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Login_ReturnsCorrectView_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();
            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);
            controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await controller.Login(new LoginViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<LoginViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task Login_AddsModelError_WhenModelStateIsValid_AndSignInFails()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();
            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);
            var model = new LoginViewModel { Email = "test@example.com", Password = "password", RememberMe = false };
            var expectedErrorMessage = "Incorrect login and/or password";

            mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                             .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(controller.ModelState.ErrorCount > 0);
            Assert.Contains(controller.ModelState, x => x.Key == string.Empty);
        }

        [Fact]
        public async Task Logout_RedirectsToHomeIndex()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();
            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            var result = await controller.Logout();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        [Fact]
        public void Registration_ReturnsCorrectView_WhenNotAuthenticated()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();
            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var httpContext = new DefaultHttpContext();
            httpContext.User = null;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock)
            {
                ControllerContext = controllerContext,
            };

            // Act
            var result = controller.Registration();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Registration_RedirectsToHomeIndex_WhenRegistrationSuccess()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();
            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);
            var model = new RegViewModel { Email = "test@example.com", Password = "password", FirstName = "John", LastName = "Doe" };

            var user = new User { Email = model.Email, UserName = model.Email, FirstName = model.FirstName, LastName = model.LastName };
            var identityResult = IdentityResult.Success;

            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(identityResult);
            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(identityResult);
            mockSignInManager.Setup(x => x.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), null)).Returns(Task.CompletedTask);

            // Act
            var result = await controller.Registration(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task Registration_ReturnsViewWithError_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();
            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);
            var model = new RegViewModel();

            controller.ModelState.AddModelError("Email", "The Email field is required.");

            // Act
            var result = await controller.Registration(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model); 
        }

        [Fact]
        public void ForgotPassword_ReturnsView()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();
            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            var result = controller.ForgotPassword();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); 
        }


        [Fact]
        public async Task IsEmailInUse_ReturnsFalse_WhenEmailExists()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var fakeUsers = new List<User>
            {
                new User { Email = "user1@gmail.com" },
                new User { Email = "user2@gmail.com" }
            };

            mockUserManager.Setup(x => x.Users).Returns(fakeUsers.AsQueryable());

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            var result = controller.IsEmailInUse("user1@gmail.com");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value);
        }

        [Fact]
        public async Task IsEmailInUseReturnsTrueWhenEmailNotExists()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var fakeUsers = new List<User>
            {
                new User { Email = "user1@gmail.com" },
                new User { Email = "user2@gmail.com" }
            };

            mockUserManager.Setup(x => x.Users).Returns(fakeUsers.AsQueryable());

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            var result = controller.IsEmailInUse("user0@gmail.com");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.True((bool)jsonResult.Value);
        }

        [Fact]
        public async Task IsEmailExistReturnsTrueWhenEmailExists()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var fakeUsers = new List<User>
            {
                new User { Email = "user1@gmail.com" },
                new User { Email = "user2@gmail.com" }
            };

            mockUserManager.Setup(x => x.Users).Returns(fakeUsers.AsQueryable());

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            var result = controller.IsEmailExists("user1@gmail.com");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.True((bool)jsonResult.Value);
        }

        [Fact]
        public async Task IsEmailExistReturnsFalseWhenEmailNotExists()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var fakeUsers = new List<User>
            {
                new User { Email = "user1@gmail.com" },
                new User { Email = "user2@gmail.com" }
            };

            mockUserManager.Setup(x => x.Users).Returns(fakeUsers.AsQueryable());

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            var result = controller.IsEmailExists("user0@gmail.com");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value);
        }

        [Fact]
        public void ForgotPasswordConfirmation_ReturnsView()
        {
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            ViewResult result = controller.ForgotPassword() as ViewResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ResetPassword_ReturnsCorrectView_WhenTokenAndEmailAreProvided()
        {
            // Arrange
            var token = "test_token";
            var email = "test@example.com";
            var expectedViewName = "ResetPassword";
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            var result = controller.ResetPassword(token, email);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ResetPasswordViewModel>(viewResult.Model);
            Assert.Equal(token, model.Token);
            Assert.Equal(email, model.Email);
        }

        [Fact]
        public void ResetPassword_RedirectsToLogin_WhenTokenOrEmailAreNullOrEmpty()
        {
            // Arrange
            var token = string.Empty;
            var email = "test@example.com";
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            var result = controller.ResetPassword(token, email);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ResetPassword_RedirectsToResetPasswordConfirmation_WhenUserIsNull()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();
            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);
            var resetPasswordViewModel = new ResetPasswordViewModel { Email = "test@example.com", Token = "token", Password = "newpassword" };

            // Act
            var result = await controller.ResetPassword(resetPasswordViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ResetPasswordConfirmation", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ResetPassword_ReturnsAViewResultWithResetPasswordViewModel_WhenModelStateIsValid()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);
            controller.ModelState.AddModelError("Email", "Required");
            ResetPasswordViewModel viewModel = new ResetPasswordViewModel();

            // Act
            var result = await controller.ResetPassword(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ResetPasswordViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task ResetPassword_ReturnsARedirectWhenResetPassword_WhenSucceeded()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            ResetPasswordViewModel viewModel = new ResetPasswordViewModel { Email = "user@gmail.com", Password = "123", Token = "token" };
            mockUserManager.Setup(repo => repo.ResetPasswordAsync(new User { Id = "1" }, viewModel.Token, viewModel.Password)).ReturnsAsync(IdentityResult.Success);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            var result = await controller.ResetPassword(viewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ResetPasswordConfirmation", redirectToActionResult.ActionName);
        }

        [Fact]
        public void ResetPasswordConfirmation_ViewResultNotNull()
        {
            // Arrange
            var mockEmailConfiguration = new Mock<EmailConfiguration>();

            var mockEmailSender = new Mock<EmailSender>(mockEmailConfiguration.Object);
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            var appEnvironmentMock = Mock.Of<IWebHostEnvironment>();

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockEmailSender.Object, appEnvironmentMock);

            // Act
            ViewResult result = controller.ResetPasswordConfirmation() as ViewResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task PersonalCabinet_AuthenticatedUser_ReturnsView()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockCourse = new Mock<ICourse>();
            var mockCategory = new Mock<ICategory>();
            var user = new User { Id = "userId" };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            var controllerContext = new ControllerContext { HttpContext = httpContext };
            var categoryId = 1;
            var courseName = "TestCourse";
            var expectedViewModel = new PersonalCabinteViewModel
            {
                Courses = new List<CourseWithCountTestsAndPassedTestsViewModel>(), 
                AllCategories = new List<Category>(), 
                CategoryId = categoryId
            };

            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            mockCourse.Setup(x => x.GetCoursesWithTestsAndPastedTestsByUserIdAndCategIdAndNameAsync(user.Id, categoryId,courseName)).ReturnsAsync(new List<CourseWithCountTestsAndPassedTestsViewModel>());
            mockCategory.Setup(x => x.GetAllCategoriesAsync()).ReturnsAsync(new List<Category>());
            var controller = new AccountController(mockUserManager.Object, null, null, null)
            {
                ControllerContext = controllerContext
            };

            // Act
            var result = await controller.PersonalCabinet(mockCourse.Object, mockCategory.Object, categoryId, courseName);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PersonalCabinteViewModel>(viewResult.ViewData.Model);
           
            Assert.Equal(expectedViewModel.Courses, model.Courses);
            Assert.Equal(expectedViewModel.AllCategories, model.AllCategories);
            Assert.Equal(expectedViewModel.CategoryId, model.CategoryId);
        }

        [Fact]
        public async Task PersonalCabinet_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockCourse = new Mock<ICourse>();
            var mockCategory = new Mock<ICategory>();

            var httpContext = new DefaultHttpContext();
            httpContext.User = null;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new AccountController(mockUserManager.Object, null, null, null)
            {
                ControllerContext = controllerContext,
            };

            // Act
            var result = await controller.PersonalCabinet(mockCourse.Object, mockCategory.Object, 1, "TestCourse");

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task FindCourse_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockCourse = new Mock<ICourse>();
            var mockCategory = new Mock<ICategory>();

            var httpContext = new DefaultHttpContext();
            httpContext.User = null;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new AccountController(mockUserManager.Object, null, null, null)
            {
                ControllerContext = controllerContext,
            };

            // Act
            var result = await controller.FindCourse(mockCourse.Object, mockCategory.Object, 1, "TestCourse");

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task FindCourse_AuthenticatedUser_ReturnsView()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockCourse = new Mock<ICourse>();
            var mockCategory = new Mock<ICategory>();
            var user = new User { Id = "userId" };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            var controllerContext = new ControllerContext { HttpContext = httpContext };
            var categoryId = 1;
            var courseName = "TestCourse";
            var expectedViewModel = new PersonalCabinteViewModel
            {
                Courses = new List<CourseWithCountTestsAndPassedTestsViewModel>(),
                AllCategories = new List<Category>(),
                CategoryId = categoryId,
                CourseName = courseName
            };

            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            mockCourse.Setup(x => x.GetCoursesWithTestsAndPastedTestsByUserIdAndCategIdAndNameAsync(user.Id, categoryId, courseName)).ReturnsAsync(new List<CourseWithCountTestsAndPassedTestsViewModel>());
            mockCategory.Setup(x => x.GetAllCategoriesAsync()).ReturnsAsync(new List<Category>());
            var controller = new AccountController(mockUserManager.Object, null, null, null)
            {
                ControllerContext = controllerContext
            };

            // Act
            var result = await controller.FindCourse(mockCourse.Object, mockCategory.Object, 1, "TestCourse");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("PersonalCabinet", viewResult.ViewName);
        }

        [Fact]
        public async Task AccountSettings_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockCourse = new Mock<ICourse>();
            var mockCategory = new Mock<ICategory>();

            var httpContext = new DefaultHttpContext();
            httpContext.User = null;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new AccountController(mockUserManager.Object, null, null, null)
            {
                ControllerContext = controllerContext,
            };

            // Act
            var result = await controller.AccountSettings();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task AccountSettings_AuthenticatedUser_ReturnsViewWithCorrectData()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var controller = new AccountController(mockUserManager.Object, null, null, null);

            var user = new User
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Image = "avatar.jpg"
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            var mockUserStore = new Mock<IUserStore<User>>();
            mockUserStore.Setup(store => store.FindByIdAsync(user.Id, CancellationToken.None)).ReturnsAsync(user);
            mockUserManager.Setup(manager => manager.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

            // Act
            var result = await controller.AccountSettings();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AccountSettingsViewModel>(viewResult.ViewData.Model);
            Assert.Equal(user.Id, model.Id);
            Assert.Equal(user.FirstName, model.FirstName);
            Assert.Equal(user.LastName, model.LastName);
            Assert.Equal(user.Email, model.Email);
            Assert.Equal(user.Image, model.Image);
        }

        [Fact]
        public async Task AccountSettingsPost_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var controller = new AccountController(mockUserManager.Object, null, null, null);
            var model = new AccountSettingsViewModel { Id = "1", FirstName = "John", LastName = "Doe" };

            // Act
            var result = await controller.AccountSettings(model);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AccountSettingsPost_ValidModel_ReturnsRedirectToAction()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var controller = new AccountController(mockUserManager.Object, null, null, null);
            var user = new User { Id = "1", FirstName = "John", LastName = "Doe" };
            var model = new AccountSettingsViewModel { Id = "1", FirstName = "Jane", LastName = "Doe" };
            mockUserManager.Setup(manager => manager.FindByIdAsync(user.Id)).ReturnsAsync(user);
            mockUserManager.Setup(manager => manager.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.AccountSettings(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(AccountController.AccountSettings), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AccountSettingsPost_InvalidModel_ReturnsViewWithModelError()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var controller = new AccountController(mockUserManager.Object, null, null, null);
            var user = new User { Id = "1", FirstName = "John", LastName = "Doe" };
            var model = new AccountSettingsViewModel { Id = "1", FirstName = "Jane", LastName = "Doe" };
            mockUserManager.Setup(manager => manager.FindByIdAsync(user.Id)).ReturnsAsync(user);
            mockUserManager.Setup(manager => manager.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError()));

            // Act
            var result = await controller.AccountSettings(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var modelState = Assert.IsAssignableFrom<Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary>(viewResult.ViewData.ModelState);
            Assert.True(modelState.ContainsKey(""));
            var modelError = Assert.Single(modelState[""].Errors);
            Assert.Equal("An error occurred while saving", modelError.ErrorMessage);
        }

        [Fact]
        public async Task AccountPasswordSettings_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockCourse = new Mock<ICourse>();
            var mockCategory = new Mock<ICategory>();

            var httpContext = new DefaultHttpContext();
            httpContext.User = null;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new AccountController(mockUserManager.Object, null, null, null)
            {
                ControllerContext = controllerContext,
            };

            // Act
            var result = await controller.AccountPasswordSettings();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task AccountPasswordSettings_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var controller = new AccountController(mockUserManager.Object, null, null, null);
            var user = new User { Id = "1", Email = "test@example.com" };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Email) }, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.AccountPasswordSettings();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AccountPasswordSettings_AuthenticatedUser_ReturnsViewWithModel()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var controller = new AccountController(mockUserManager.Object, null, null, null);
            var user = new User { Id = "1", Email = "test@example.com" };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Email) }, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(user);

            // Act
            var result = await controller.AccountPasswordSettings();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ResetPasswordViewModel>(viewResult.ViewData.Model);
            Assert.Equal("true", model.Token);
            Assert.Equal(user.Email, model.Email);
        }
    }
}
