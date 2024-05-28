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
using CourseAcademyProject.Models.Test;
using CourseAcademyProject.ViewModels.Test;

namespace CourseAcademy.Tests
{
    public class TestControllerTests 
    {
        [Fact]
        public void Tests_ReturnsViewWithTests()
        {
            // Arrange
            var mockTests = new Mock<ITest>();
            var testsList = new List<Test>
            {
                new Test { Id = 1, Title = "Test 1" },
                new Test { Id = 2, Title = "Test 2" },
                new Test { Id = 3, Title = "Test 3" }
            };
            var options = new QueryOptions();

            mockTests.Setup(c => c.GetAllTestsWithQuestions(options))
                          .Returns(new PagedList<Test>(testsList.AsQueryable(), options));

            var controller = new TestController(mockTests.Object, null);

            // Act
            var result = controller.Tests(options);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Test>>(viewResult.Model);
            Assert.Equal(testsList, model);
        }

        [Fact]
        public async Task CreateTest_ReturnsView_TestWithQuestionAndAnswer()
        {
            // Arrange
            var mockTests = new Mock<ITest>(); 
            var mockCourses = new Mock<ICourse>(); 

            var courses = new List<Course>
            {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" },
                new Course { Id = 3, Name = "Course 3" }
            };
            mockCourses.Setup(c => c.GetAllCourseAsync()).ReturnsAsync(courses);

            var controller = new TestController(mockTests.Object, mockCourses.Object);

            // Act
            var result = await controller.CreateTest();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<TestWithQuestionAndAnswer>(viewResult.Model);
            Assert.Equal(courses.Count, model.AllCourses.Count()); 
        }

        [Fact]
        public async Task CreateTest_Post_ValidModel_RedirectsToTests()
        {
            // Arrange
            var mockTests = new Mock<ITest>(); 
            var mockCourses = new Mock<ICourse>(); 

            var controller = new TestController(mockTests.Object, mockCourses.Object);
            var testViewModel = new TestWithQuestionAndAnswer
            {
                Title = "Test Title",
                Description = "Test Description",
                TimeToPass = new TimeOnly(60),
                CourseId = 1,
                Questions = new List<QuestionViewModel>
                {
                    new QuestionViewModel
                    {
                        Content = "Question 1",
                        Answers = new List<AnswerViewModel>
                        {
                            new AnswerViewModel { Content = "Answer 1", IsCorrect = true },
                            new AnswerViewModel { Content = "Answer 2", IsCorrect = false }
                        }
                    }
                }
            };

            // Act
            var result = await controller.CreateTest(testViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Tests", redirectToActionResult.ActionName); 
            mockTests.Verify(t => t.AddTestAsync(It.IsAny<Test>()), Times.Once); 
        }

        [Fact]
        public async Task CreateTest_Post_InvalidModel_ReturnsView()
        {
            // Arrange
            var mockTests = new Mock<ITest>(); 
            var mockCourses = new Mock<ICourse>(); 

            var controller = new TestController(mockTests.Object, mockCourses.Object);
            controller.ModelState.AddModelError("Title", "Title is required"); 

            var testViewModel = new TestWithQuestionAndAnswer();

            // Act
            var result = await controller.CreateTest(testViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(testViewModel, viewResult.Model); 
            mockTests.Verify(t => t.AddTestAsync(It.IsAny<Test>()), Times.Never); 
        }

        [Fact]
        public async Task EditTest_Get_TestExists_ReturnsViewWithModel()
        {
            // Arrange
            var mockTests = new Mock<ITest>(); 
            var mockCourses = new Mock<ICourse>(); 

            var testId = 1;
            var test = new Test
            {
                Id = testId,
                Title = "Test Title",
                Description = "Test Description",
                TimeToPass = new TimeOnly(60),
                CourseId = 1,
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        Content = "Question 1",
                        Answers = new List<Answer>
                        {
                            new Answer { Id = 1, Content = "Answer 1", IsCorrect = true },
                            new Answer { Id = 2, Content = "Answer 2", IsCorrect = false }
                        }
                    }
                }
            };
            mockTests.Setup(t => t.GetTestWithAnswersAndQuestionsByIdAsync(testId))
                .ReturnsAsync(test);
            var controller = new TestController(mockTests.Object, mockCourses.Object);

            // Act
            var result = await controller.EditTest(testId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<TestWithQuestionAndAnswer>(viewResult.Model);
            Assert.Equal(test.Id, viewModel.Id); 
            Assert.Equal(test.Title, viewModel.Title); 
        }

        [Fact]
        public async Task EditTest_Get_TestDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var mockTests = new Mock<ITest>(); 
            var mockCourses = new Mock<ICourse>(); 

            var testId = 1;

            mockTests.Setup(t => t.GetTestWithAnswersAndQuestionsByIdAsync(testId))
                .ReturnsAsync((Test)null); 

            var controller = new TestController(mockTests.Object, mockCourses.Object);

            // Act
            var result = await controller.EditTest(testId);

            // Assert
            Assert.IsType<NotFoundResult>(result); 
        }

        [Fact]
        public async Task EditTest_Post_ModelStateValid_ReturnsRedirectToAction()
        {
            // Arrange
            var mockTests = new Mock<ITest>();
            var testViewModel = new TestWithQuestionAndAnswer
            {
                Id = 1,
                Title = "Test Title",
                Description = "Test Description",
                TimeToPass = new TimeOnly(60),
                CourseId = 1,
                Questions = new List<QuestionViewModel>
                {
                    new QuestionViewModel
                    {
                        Id = 1,
                        Content = "Question 1",
                        Answers = new List<AnswerViewModel>
                        {
                            new AnswerViewModel { Id = 1, Content = "Answer 1", IsCorrect = true },
                            new AnswerViewModel { Id = 2, Content = "Answer 2", IsCorrect = false }
                        }
                    }
                }
            };

            mockTests.Setup(t => t.GetTestWithAnswersAndQuestionsByIdAsync(It.IsAny<int>()))
                     .ReturnsAsync(new Test());

            var controller = new TestController(mockTests.Object, null);

            // Act
            var result = await controller.EditTest(testViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Tests", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditTest_Post_ModelStateInvalid_ReturnsViewWithModel()
        {
            // Arrange
            var mockTests = new Mock<ITest>();
            var testViewModel = new TestWithQuestionAndAnswer();

            var controller = new TestController(mockTests.Object, null);
            controller.ModelState.AddModelError("Title", "Title is required"); 

            // Act
            var result = await controller.EditTest(testViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(testViewModel, viewResult.Model);
        }

        [Fact]
        public async Task EditTest_Post_SelectedTestNull_ReturnsNotFound()
        {
            // Arrange
            var mockTests = new Mock<ITest>();
            var testViewModel = new TestWithQuestionAndAnswer
            {
                Id = 1,
            };

            mockTests.Setup(t => t.GetTestWithAnswersAndQuestionsByIdAsync(It.IsAny<int>()))
                     .ReturnsAsync((Test)null);

            var controller = new TestController(mockTests.Object, null);

            // Act
            var result = await controller.EditTest(testViewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTest_TestExists_ReturnsOk()
        {
            // Arrange
            var mockTests = new Mock<ITest>();
            var testId = 1;

            mockTests.Setup(t => t.GetTestWithAnswersAndQuestionsByIdAsync(testId))
                     .ReturnsAsync(new Test());

            var controller = new TestController(mockTests.Object, null);

            // Act
            var result = await controller.DeleteTest(testId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockTests.Verify(x => x.DeleteTestAsync(It.IsAny<Test>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTest_TestNotExists_DeleteTestTimesNever()
        {
            // Arrange
            var mockTests = new Mock<ITest>();
            var testId = 1;

            mockTests.Setup(t => t.GetTestWithAnswersAndQuestionsByIdAsync(testId))
                     .ReturnsAsync((Test)null);

            var controller = new TestController(mockTests.Object, null);

            // Act
            var result = await controller.DeleteTest(testId);

            // Assert
            mockTests.Verify(x => x.DeleteTestAsync(It.IsAny<Test>()), Times.Never);
        }
    }
}
