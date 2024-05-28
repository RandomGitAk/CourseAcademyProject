using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.ViewModels;

namespace CourseAcademyProject.Interfaces
{
    public interface ICourse
    {
        PagedList<Course> GetAllCourses(QueryOptions options);
        Task<IEnumerable<Course>> GetAllCourseAsync();
        Task<IEnumerable<CourseWithCountTestsAndPassedTestsViewModel>> GetCoursesWithTestsAndPastedTestsByUserIdAndCategIdAndNameAsync(string userId, int categoryId, string courseName = null);
        Task<Course> GetCourseByIdAsync(int id);
        Task<Course> GetCourseWithLessonsAndCommentsByIdAsync(int id);
        Task AddCourseAsync(Course course);
        Task DeleteCourseAsync(Course course);
        Task EditCourseAsync(Course course);

    }
}
