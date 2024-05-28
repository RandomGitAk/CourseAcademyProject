using CourseAcademyProject.Data;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models.Test;
using Microsoft.EntityFrameworkCore;

namespace CourseAcademyProject.Repository
{
    public class UserAnswerRepository : IUserAnswer
    {
        private readonly ApplicationContext _context;

        public UserAnswerRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddUserAnswerAsync(TestResult testResult)
        {
            _context.TestResults.Add(testResult);
            await _context.SaveChangesAsync();
        }

        public async Task<Test> GetTestWithUserAnswersByTestIdAsync(int testId, string userId)
        {
            return await _context.Tests
                .Where(e=>e.Id == testId)
                .Include(e => e.Questions)
                .ThenInclude(e => e.Answers)
                .Include(e => e.TestResults.Where(e=> e.UserId == userId && e.TestId == testId))
                .ThenInclude(e => e.UserAnswers)
                .ThenInclude(e => e.Answer).FirstOrDefaultAsync();
        }
    }
}
