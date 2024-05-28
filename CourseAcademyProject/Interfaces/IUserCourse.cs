using CourseAcademyProject.Models;

namespace CourseAcademyProject.Interfaces
{
    public interface IUserCourse
    {
        Task AddUserToCourseAsync(UserCourse userCourse);
        Task<bool> IsUserInCourseAsync(string userId, int courseId);
    }
}
