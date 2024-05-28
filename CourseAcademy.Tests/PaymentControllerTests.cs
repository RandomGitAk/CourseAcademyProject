using CourseAcademyProject.Controllers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using Microsoft.AspNetCore.Http.Features;
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
using Stripe.Checkout;
using Xunit;
using Microsoft.Extensions.Options;
using Stripe;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace CourseAcademy.Tests
{
    public class PaymentControllerTests
    {
        [Fact]
        public async Task CreateCheckout_ValidModel_ReturnsRedirectToAction()
        {
            // Arrange
            StripeConfiguration.ApiKey = "sk_test_51OxrBr02pbliTYxGEDEzVKtaFGSqpAJ7o5x8zKAI7r7g124q7Z5R6zhGzVGJiHxLduznmWMAaUZjOoptPSDv2NC000DFiX9rm5";
            var mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockUserCourses = new Mock<IUserCourse>();
            var mockCourses = new Mock<ICourse>();
            Mock<SessionService>? mockSessionService = new Mock<SessionService>();
          

            var controller = new PaymentController(mockUserCourses.Object, mockUserManager.Object, mockCourses.Object);

            var currentUser = new User { Id = "1", Email = "test@example.com" };
            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);

            var currentCourse = new Course { Id = 1, Price = 100 };
            mockCourses.Setup(c => c.GetCourseByIdAsync(1))
                .ReturnsAsync(currentCourse);

            mockUserCourses.Setup(uc => uc.IsUserInCourseAsync("1", 1))
                .ReturnsAsync(false);

            var options = new SessionCreateOptions
            {
            };
            mockSessionService.Setup(s => s.Create(options,null))
                .Returns(new Session { Url = "http://example.com" });

            var priceId = "price_1OxrMG02pbliTYxGHlFhYTGu";
            var courseId = 1;

            var context = new DefaultHttpContext();
            context.Features.Set<IHttpRequestFeature>(new HttpRequestFeature());
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("localhost");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            // Act
            var result = await controller.CreateCheckout(priceId, courseId);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;
            Assert.Equal(303, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Cancel_ReturnsCorrectView()
        {
            // Arrange
            var courseId = 123; 
            var controller = new PaymentController(null, null, null); 

            // Act
            var result = await controller.Cancel(courseId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(courseId, viewResult.Model); 
        }

        [Fact]
        public async Task SignUpUserToFreeCourse_ReturnsRedirectToAction_WhenUserIsSignedUp()
        {
            // Arrange
            var courseId = 123;
            var userId = "user_id_here"; 
            var currentUser = new User { Id = userId }; 
            var mockUserManager = MockUserManager<User>();
            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser); 
            var mockUserCourses = new Mock<IUserCourse>();
            mockUserCourses.Setup(u => u.IsUserInCourseAsync(userId, courseId)).ReturnsAsync(true);
            var mockCourses = new Mock<ICourse>(); // Створення імітованого об'єкта для ICourse
            mockCourses.Setup(c => c.GetCourseByIdAsync(courseId)).ReturnsAsync(new Course { Price = null, PriceId = null }); 

            var controller = new PaymentController(mockUserCourses.Object, mockUserManager.Object, mockCourses.Object);

            // Act
            var result = await controller.SignUpUserToFreeCourse(courseId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("CourseDetail", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal(courseId, redirectToActionResult.RouteValues["courseId"]);
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            return userManager;
        }
    }
}
