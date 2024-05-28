using CourseAcademyProject.Data;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CourseAcademyProject.Repository
{
    public class CourseRepository : ICourse
    {
        private readonly ApplicationContext _applicationContext;

        public CourseRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task AddCourseAsync(Course course)
        {
            await _applicationContext.Courses.AddAsync(course);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(Course course)
        {
            _applicationContext.Courses.Remove(course);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task EditCourseAsync(Course course)
        {
            _applicationContext.Entry(course).State = EntityState.Modified;
            _applicationContext.Entry(course).Property(e=>e.DateOfPublication).IsModified = false;
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Course>> GetAllCourseAsync()
        {
            return await _applicationContext.Courses.AsNoTracking().ToListAsync();
        }

        public PagedList<Course> GetAllCourses(QueryOptions options)
        {
            return new PagedList<Course>(_applicationContext.Courses.Include(e=> e.Category), options);
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
          return  await _applicationContext.Courses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Course> GetCourseWithLessonsAndCommentsByIdAsync(int id)
        {
            return await _applicationContext.Courses
                .Include(e=> e.Lessons)
                .Include(e=> e.CoursesUser).ThenInclude(e=> e.User)
                .Include(e=> e.Comments.OrderBy(c => c.ParentCommentId.HasValue ? c.ParentCommentId : c.Id).ThenBy(c => c.DateOfPublication))
                .ThenInclude(c=>c.User).AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<CourseWithCountTestsAndPassedTestsViewModel>> 
            GetCoursesWithTestsAndPastedTestsByUserIdAndCategIdAndNameAsync(string userId,int categoryId, string courseName)
        {
            IQueryable<Course> query = _applicationContext.Courses.AsNoTracking();
            if (categoryId != 0)
            {
                query = query.Where(e => e.CategoryId == categoryId);
            }
            if (courseName != null)
            {
                query = query.Where(e=> e.Name.Contains(courseName));
            }   
            return await query.Where(c => c.CoursesUser.Any(cu => cu.UserId == userId))
                .Select(course => new CourseWithCountTestsAndPassedTestsViewModel
            {
                CourseId = course.Id,
                CourseName = course.Name,
                Image = course.Image,
                TotalTestsCount = course.Tests.Count(),
                PassedTestsCount = course.Tests
            .SelectMany(test => test.TestResults)
            .Where(result => result.UserId == userId)
            .Count()
            }).ToListAsync();

            //return await _applicationContext.Courses
            //     .Where(c => c.CoursesUser.Any(cu => cu.UserId == userId))
            //     .Select(course => new CourseWithCountTestsAndPassedTestsViewModel
            //{
            //    CourseId = course.Id,
            //    CourseName = course.Name,
            //    Image = course.Image,
            //    TotalTestsCount = course.Tests.Count(),
            //    PassedTestsCount = course.Tests
            //.SelectMany(test => test.TestResults)
            //.Where(result => result.UserId == userId)
            //.Count()
            //}).ToListAsync();
        }
    }
}
