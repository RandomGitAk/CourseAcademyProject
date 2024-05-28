using CourseAcademyProject.Controllers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models.Test;
using CourseAcademyProject.Models;
using CourseAcademyProject.ViewModels.Test;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CourseAcademyProject.ViewModels.UserTest;

namespace CourseAcademy.Tests
{
    public class UserTestControllerTests
    {
        [Fact]
        public async Task GetCourseTests_AuthenticatedUser_ReturnsViewResult()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockTests = new Mock<ITest>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var courseId = 1;
            var user = new User { Id = "user_id" };
            var completedTests = new List<Test>();
            var incompletedTests = new List<Test>();

            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(user);

            mockTests.Setup(t => t.GetTestsThatCompletedByCourseIdAsync(courseId, user.Id))
                     .ReturnsAsync(completedTests);

            mockTests.Setup(t => t.GetTestsThatIncompletedByCourseIdAsync(courseId, user.Id))
                     .ReturnsAsync(incompletedTests);

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


            var controller = new UserTestController(mockUserManager.Object, mockTests.Object, mockUserAnswer.Object) 
            {
                ControllerContext = controllerContext
            };

            // Act
            var result = await controller.GetCourseTests(courseId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CompletedAndIncompletedTestsViewModel>(viewResult.Model);
            Assert.Equal(courseId, model.CourseId);
        }

        [Fact]
        public async Task GetCourseTests_UnauthenticatedUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockTests = new Mock<ITest>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var courseId = 1;
            var httpContext = new DefaultHttpContext();
            httpContext.User = null;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new UserTestController(mockUserManager.Object, mockTests.Object, mockUserAnswer.Object)
            {
                ControllerContext = controllerContext
            };


            // Act
            var result = await controller.GetCourseTests(courseId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetCourseTests_InvalidCourseId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockTests = new Mock<ITest>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var courseId = 0;

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

            var controller = new UserTestController(mockUserManager.Object, mockTests.Object, mockUserAnswer.Object)
            {
                ControllerContext = controllerContext
            };

            // Act
            var result = await controller.GetCourseTests(courseId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task FindTest_AuthenticatedUser_ReturnsViewResult()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockTests = new Mock<ITest>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var courseId = 1;
            var testName = "Test Name";
            var user = new User { Id = "user_id" };
            var completedTests = new List<Test>();
            var incompletedTests = new List<Test>();

            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(user);

            mockTests.Setup(t => t.GetTestsThatIncompletedByCourseNameAndIdAsync(courseId, user.Id, testName))
                     .ReturnsAsync(incompletedTests);

            mockTests.Setup(t => t.GetTestsThatCompletedByCourseIdAsync(courseId, user.Id))
                     .ReturnsAsync(completedTests);

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

            var controller = new UserTestController(mockUserManager.Object, mockTests.Object, mockUserAnswer.Object)
            {
                ControllerContext = controllerContext
            };

            // Act
            var result = await controller.FindTest(courseId, testName);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CompletedAndIncompletedTestsViewModel>(viewResult.Model);
            Assert.Equal(courseId, model.CourseId);
            Assert.Equal(testName, model.TestName);
        }

        [Fact]
        public async Task FindTest_UnauthenticatedUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockTests = new Mock<ITest>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var courseId = 1;
            var testName = "Test Name";

            var httpContext = new DefaultHttpContext();
            httpContext.User = null;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new UserTestController(mockUserManager.Object, mockTests.Object, mockUserAnswer.Object)
            {
                ControllerContext = controllerContext
            };

            // Act
            var result = await controller.FindTest(courseId, testName);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task FindTest_InvalidCourseId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockTests = new Mock<ITest>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var courseId = 0;
            var testName = "Test Name";

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

            var controller = new UserTestController(mockUserManager.Object, mockTests.Object, mockUserAnswer.Object)
            {
                ControllerContext = controllerContext
            };

            // Act
            var result = await controller.FindTest(courseId, testName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTest_ValidTestId_ReturnsViewResult()
        {
            // Arrange
            var mockTests = new Mock<ITest>();
            var testId = 1;
            var test = new Test { Id = testId };

            mockTests.Setup(t => t.GetTestWithAnswersAndQuestionsByIdAsync(testId))
                     .ReturnsAsync(test);

            var controller = new UserTestController(null,mockTests.Object, null);

            // Act
            var result = await controller.GetTest(testId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(test, viewResult.Model);
        }

        [Fact]
        public async Task GetTest_InvalidTestId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockTests = new Mock<ITest>();
            var testId = 0;

            var controller = new UserTestController(null, mockTests.Object, null);

            // Act
            var result = await controller.GetTest(testId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PassTest_ValidModel_RedirectsToAction()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var httpContext = new DefaultHttpContext();

            var user = new User { Id = "user_id" };
            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                         .ReturnsAsync(user);
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                 }, "mock"));

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var controller = new UserTestController(mockUserManager.Object, null, mockUserAnswer.Object) 
            {
                ControllerContext = controllerContext
            };

            var testResult = new TestResultViewModel
            {
                TestId = 1,
                CompletionTime = new TimeOnly(30),
                QuestionAnswers = new List<QuestionAnswerViewModel>
                {
                    new QuestionAnswerViewModel { QuestionId = 1, SelectedAnswerId = 1 },
                    new QuestionAnswerViewModel { QuestionId = 2, SelectedAnswerId = 2 }
                }
            };

            // Act
            var result = await controller.PassTest(testResult) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("GetTestResult", result.ActionName);
            Assert.Equal(testResult.TestId, result.RouteValues["testId"]);
        }

        [Fact]
        public async Task PassTest_InvalidModel_RedirectsToGetTest()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var controller = new UserTestController(mockUserManager.Object, null, mockUserAnswer.Object);
            controller.ModelState.AddModelError("Error", "Invalid model");

            var testResult = new TestResultViewModel
            {
                TestId = 1
            };

            // Act
            var result = await controller.PassTest(testResult) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("GetTest", result.ActionName);
            Assert.Equal(testResult.TestId, result.RouteValues["testId"]);
        }

        [Fact]
        public async Task PassTest_NullUser_ReturnsNotFound()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var controller = new UserTestController(mockUserManager.Object, null, mockUserAnswer.Object);

            var testResult = new TestResultViewModel
            {
                TestId = 1
            };

            // Act
            var result = await controller.PassTest(testResult) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTestResult_UserNotNull_ReturnsViewResult()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockUserAnswer = new Mock<IUserAnswer>();

            var testId = 1;
            var user = new User { Id = "user_id" };
            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);
            mockUserAnswer.Setup(m => m.GetTestWithUserAnswersByTestIdAsync(testId, user.Id))
                .ReturnsAsync(new Test());
            var controller = new UserTestController(mockUserManager.Object, null, mockUserAnswer.Object);

            // Act
            var result = await controller.GetTestResult(testId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task GetTestResult_NullUser_ReturnsNotFoundResult()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>();
            var mockUserAnswer = new Mock<IUserAnswer>();
            var controller = new UserTestController(mockUserManager.Object, null, mockUserAnswer.Object);

            var testId = 1;

            // Act
            var result = await controller.GetTestResult(testId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            return userManager;
        }
    }
}
