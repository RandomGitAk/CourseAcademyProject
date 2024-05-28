using CourseAcademyProject.Data;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseAcademyProject.Repository
{
    public class UserCourseRepository : IUserCourse
    {
        private readonly ApplicationContext _applicationContext;

        public UserCourseRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task AddUserToCourseAsync(UserCourse userCourse)
        {
            await _applicationContext.UserCourses.AddAsync(userCourse);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<bool> IsUserInCourseAsync(string userId, int courseId)
        {
            return await _applicationContext.UserCourses.AnyAsync(e => e.UserId == userId && e.CourseId == courseId); 
        }
    }
}
