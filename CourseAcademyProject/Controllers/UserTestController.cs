using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Test;
using CourseAcademyProject.ViewModels.Test;
using CourseAcademyProject.ViewModels.UserTest;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseAcademyProject.Controllers
{
    public class UserTestController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ITest _tests;
        private readonly IUserAnswer _userAnswer;

        public UserTestController(UserManager<User> userManager, ITest tests, IUserAnswer userAnswer)
        {
            _userManager = userManager;
            _tests = tests;
            _userAnswer = userAnswer;
        }

        [Route("/course-tests")]
        [HttpGet]
        public async Task<IActionResult> GetCourseTests(int courseId)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (courseId != 0)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser != null)
                    {
                        var completedTests = await _tests.GetTestsThatCompletedByCourseIdAsync(courseId, currentUser.Id);
                        var incompletedTests = await _tests.GetTestsThatIncompletedByCourseIdAsync(courseId, currentUser.Id);
                        return View(new CompletedAndIncompletedTestsViewModel
                        {
                            CompletedTests = completedTests.ToList(),
                            IncompletedTests = incompletedTests.ToList(),
                            CourseId = courseId
                        });
                    }
                }
                return NotFound();
            }
            return Unauthorized();
        }

        [Route("/find-course-test")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> FindTest(int courseId, string testName)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (courseId != 0)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser != null)
                    {
                        var incompletedTests = await _tests.GetTestsThatIncompletedByCourseNameAndIdAsync(courseId, currentUser.Id, testName);
                        var completedTests = await _tests.GetTestsThatCompletedByCourseIdAsync(courseId, currentUser.Id);
                        return View(nameof(GetCourseTests), new CompletedAndIncompletedTestsViewModel
                        {
                            CompletedTests = completedTests.ToList(),
                            IncompletedTests = incompletedTests.ToList(),
                            TestName = testName,
                            CourseId = courseId
                        });
                    }
                }
                return NotFound();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("/test")]
        public async Task<IActionResult> GetTest(int testId)
        {
            if (testId != 0)
            {
                var currentTest = await _tests.GetTestWithAnswersAndQuestionsByIdAsync(testId);
                if (currentTest != null)
                {
                    return View(currentTest);
                }
            }
            return NotFound();
        }

        [Route("/pass-test")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> PassTest(TestResultViewModel testResult)
        {
            if (ModelState.IsValid)
            {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null)
                    {
                        return NotFound();
                    }
                    var currentTestResult = new TestResult
                    {
                        TestId = testResult.TestId,
                        UserId = currentUser.Id,
                        CompletionTime = testResult.CompletionTime,
                        UserAnswers = testResult.QuestionAnswers.Select(ua => new UserAnswer
                        {
                            AnswerId = ua.SelectedAnswerId,
                            QuestionId = ua.QuestionId,
                        }).ToList()
                    };
                    await _userAnswer.AddUserAnswerAsync(currentTestResult);
                    return RedirectToAction("GetTestResult", new {testId = testResult.TestId }); 
            }
            return RedirectToAction(nameof(GetTest), new { testId = testResult.TestId });
        }


        [HttpGet]
        [Route("/test-result")]
        public async Task<IActionResult> GetTestResult(int testId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            Test testWithUserAnswers = await _userAnswer.GetTestWithUserAnswersByTestIdAsync(testId, currentUser.Id);
            return View(testWithUserAnswers);
        }
    }
}
