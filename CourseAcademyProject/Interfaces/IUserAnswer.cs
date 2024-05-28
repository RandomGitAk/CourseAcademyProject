using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Test;

namespace CourseAcademyProject.Interfaces
{
    public interface IUserAnswer
    {
        Task<Test> GetTestWithUserAnswersByTestIdAsync(int testId, string userId);
        Task AddUserAnswerAsync(TestResult testResult);
    }
}
