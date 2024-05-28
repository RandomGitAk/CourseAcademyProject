using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models.Test;
using CourseAcademyProject.ViewModels;
using CourseAcademyProject.ViewModels.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseAcademyProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TestController : Controller
    {
        private readonly ITest _tests;
        private readonly ICourse _courses;
        public TestController(ITest tests, ICourse courses)
        {
            _tests = tests;
            _courses = courses;
        }
        
        [Route("/panel/tests")]
        [HttpGet]
        public IActionResult Tests(QueryOptions options)
        {
            return View(_tests.GetAllTestsWithQuestions(options));
        }

  
        [Route("/panel/create-test")]
        [HttpGet]
        public async Task<IActionResult> CreateTest() 
        {
            var courses = await _courses.GetAllCourseAsync();
            return View(new TestWithQuestionAndAnswer
            {
                AllCourses = new SelectList(courses, "Id", "Name")
            });
        }

        [Route("/panel/create-test")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateTest(TestWithQuestionAndAnswer testViewModel)
        {
            if (ModelState.IsValid)
            {
                Test test = new Test
                {
                    Title = testViewModel.Title!,
                    Description = testViewModel.Description!,
                    TimeToPass = testViewModel.TimeToPass,
                    CourseId = testViewModel.CourseId,
                    Questions = testViewModel.Questions.Select(q =>
                    new Question
                    {
                        Content = q.Content!,
                        Answers = q.Answers.Select(a =>
                        new Answer
                        {
                            Content = a.Content!,
                            IsCorrect = a.IsCorrect,
                        }).ToList()
                    }).ToList()
                };

                await _tests.AddTestAsync(test);
                return RedirectToAction(nameof(Tests));
            }
            return View(testViewModel);
        }


        [Route("/panel/edit-test")]
        [HttpGet]
        public async Task<IActionResult> EditTest(int testId)
        {
            var currentTest = await _tests.GetTestWithAnswersAndQuestionsByIdAsync(testId);
            if (currentTest != null)
            {
                var courses = await _courses.GetAllCourseAsync();
                var viewModel = new TestWithQuestionAndAnswer
                {
                    Id = testId,
                    Title = currentTest.Title,
                    Description = currentTest.Description,
                    TimeToPass = currentTest.TimeToPass,
                    CourseId = currentTest.CourseId,
                    AllCourses = new SelectList(courses, "Id", "Name"),
                    Questions = currentTest.Questions.Select(q =>
                    new QuestionViewModel
                    {
                        Id = q.Id,
                        TestId = testId,
                        Content = q.Content,
                        Answers = q.Answers.Select(a =>
                        new AnswerViewModel
                        {
                            Id = a.Id,
                            QuestionId = q.Id,
                            Content = a.Content,
                            IsCorrect = a.IsCorrect,
                        }).ToList()
                    }).ToList()
                };
                return View(viewModel);
            }
            return NotFound();
        }

        [Route("/panel/edit-test")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditTest(TestWithQuestionAndAnswer testViewModel)
        {
            if (ModelState.IsValid)
            {
                var selectedTest = await _tests.GetTestWithAnswersAndQuestionsByIdAsync(testViewModel.Id ?? 0);
                if (selectedTest == null)
                {
                    return NotFound();
                }
                var currentTest = new Test
                {
                    Id = testViewModel.Id ?? 0,
                    Title = testViewModel.Title!,
                    Description = testViewModel.Description!,
                    TimeToPass = testViewModel.TimeToPass,
                    CourseId = testViewModel.CourseId,
                    Questions = testViewModel.Questions.Select(q =>
                    new Question
                    {
                        Id = q.Id ?? 0,
                        TestId = q.TestId,
                        Content = q.Content!,
                        Answers = q.Answers.Select(a =>
                        new Answer
                        {
                            Id = a.Id ?? 0,
                            QuestionId = a.QuestionId,
                            Content = a.Content!,
                            IsCorrect = a.IsCorrect,
                        }).ToList()
                    }).ToList()
                };
                await _tests.EditTestAsync(currentTest);
                return RedirectToAction(nameof(Tests));
            }
            return View(testViewModel);
        }

        [Route("/panel/delete-test")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTest(int testId)
        {
            var currentTest = await _tests.GetTestWithAnswersAndQuestionsByIdAsync(testId);
            if (currentTest != null)
            {
                await _tests.DeleteTestAsync(currentTest);
            }
            return Ok();
        }
    }
}
