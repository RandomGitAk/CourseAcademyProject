using CourseAcademyProject.Data;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace CourseAcademyProject.Repository
{
    public class TestRepository : ITest
    {
        private readonly ApplicationContext _applicationContext;

        public TestRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task AddTestAsync(Test test)
        {
            await _applicationContext.Tests.AddAsync(test);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task DeleteTestAsync(Test test)
        {
            _applicationContext.Tests.Remove(test);
            var questions = await _applicationContext.Questions.Where(q => q.TestId == test.Id).ToListAsync();
            foreach (var question in questions)
            {
                var answers = await _applicationContext.Answers.Where(a => a.QuestionId == question.Id).ToListAsync();
                _applicationContext.Answers.RemoveRange(answers);

                _applicationContext.Questions.Remove(question);
            }
            await _applicationContext.SaveChangesAsync();
        }

        public async Task EditTestAsync(Test test)
        {
            _applicationContext.Entry(test).State = EntityState.Modified;
            foreach (var question in test.Questions)
            {
                _applicationContext.Entry(question).State = EntityState.Modified;
                foreach (var answer in question.Answers)
                {
                  _applicationContext.Entry(answer).State = EntityState.Modified;
                }
            }

            await _applicationContext.SaveChangesAsync();
        }

        public PagedList<Test> GetAllTestsWithQuestions(QueryOptions options)
        {
            return new PagedList<Test>(_applicationContext.Tests.Include(e=> e.Questions), options);
        }

        public async Task<Test> GetTestWithAnswersAndQuestionsByIdAsync(int id)
        {
            return await _applicationContext.Tests.Include(e=>e.Questions).ThenInclude(q=>q.Answers.OrderBy(q => Guid.NewGuid())).AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Test>> GetTestsThatCompletedByCourseIdAsync(int courseId, string userId)
        {
            return await _applicationContext.Tests
                .Where(t => t.CourseId == courseId)
                 .Where(t => _applicationContext.TestResults.Any(tr => tr.TestId == t.Id && tr.UserId == userId)).ToListAsync();
        }
        public async Task<IEnumerable<Test>> GetTestsThatIncompletedByCourseIdAsync(int courseId, string userId)
        {
            return await _applicationContext.Tests
               .Where(t => t.CourseId == courseId)
                .Where(t => !_applicationContext.TestResults.Any(tr => tr.TestId == t.Id && tr.UserId == userId)).ToListAsync();
        }

        public async Task<IEnumerable<Test>> GetTestsThatIncompletedByCourseNameAndIdAsync(int courseId, string userId, string testName)
        {
            return await _applicationContext.Tests
                .Where(t => t.CourseId == courseId && t.Title.Contains(testName))
                 .Where(t => !_applicationContext.TestResults.Any(tr => tr.TestId == t.Id && tr.UserId == userId)).ToListAsync();
        }
    }
}
