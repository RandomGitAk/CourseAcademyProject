using CourseAcademyProject.Data;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace CourseAcademyProject.Repository
{
    public class LessonRepository : ILesson
    {
        private readonly ApplicationContext _applicationContext;
        public LessonRepository(ApplicationContext context)
        {
           _applicationContext = context;
        }

        public async Task AddLessonAsync(Lesson lesson)
        {
            await _applicationContext.Lessons.AddAsync(lesson);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task DeleteLessonAsync(Lesson lesson)
        {
            _applicationContext.Lessons.Remove(lesson);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task EditLessonAsync(Lesson lesson)
        {
            _applicationContext.Entry(lesson).State = EntityState.Modified;
            _applicationContext.Entry(lesson).Property(e => e.DateOfPublication).IsModified = false;
            await _applicationContext.SaveChangesAsync();   
        }

        public PagedList<Lesson> GetAllLessonsByCourseId(QueryOptions options, int courseId)
        {
            return new PagedList<Lesson>(_applicationContext.Lessons.Include(e=> e.Course).AsNoTracking().Where(e => e.CourseId == courseId), options);
        }

        public async Task<Lesson> GetLessonByIdAsync(int id)
        {
            return await _applicationContext.Lessons.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public PagedList<Lesson> GetAllLessons(QueryOptions options)
        {
            return new PagedList<Lesson>(_applicationContext.Lessons.Include(e => e.Course), options);
        }

        public async Task<IEnumerable<Lesson>> GetFiveRandomLessonsIdByCourseIdAsync(int courseId,int lessonId)
        {
            return await _applicationContext.Lessons.AsNoTracking().Where(e => e.CourseId == courseId && e.Id != lessonId)
                .OrderBy(e=> Guid.NewGuid()).Take(5).ToListAsync();
        }

        public async Task<Lesson> GetPreviousLessonByCourseAndLessonIdAsync(int courseId, int lessonId)
        {
            return await _applicationContext.Lessons.Where(e => e.CourseId == courseId && e.Id < lessonId).OrderByDescending(e => e.Id).FirstOrDefaultAsync();
        }

        public async Task<Lesson> GetNextLessonByCourseAndLessonIdAsync(int courseId, int lessonId)
        {
            return await _applicationContext.Lessons.Where(e => e.CourseId == courseId && e.Id > lessonId).OrderByDescending(e => e.Id).FirstOrDefaultAsync();
        }
    }
}
