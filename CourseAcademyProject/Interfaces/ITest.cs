using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Test;
using System.Threading.Tasks;

namespace CourseAcademyProject.Interfaces
{
    public interface ITest
    {
        PagedList<Test> GetAllTestsWithQuestions(QueryOptions options);
        Task<IEnumerable<Test>> GetTestsThatIncompletedByCourseNameAndIdAsync(int courseId, string userId, string testName);
        Task<IEnumerable<Test>> GetTestsThatCompletedByCourseIdAsync(int courseId, string userId);
        Task<IEnumerable<Test>> GetTestsThatIncompletedByCourseIdAsync(int courseId, string userId);
        Task<Test> GetTestWithAnswersAndQuestionsByIdAsync(int id);
        Task AddTestAsync(Test test);
        Task DeleteTestAsync(Test test);
        Task EditTestAsync(Test test);
    }
}
