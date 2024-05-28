using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models;

namespace CourseAcademyProject.Interfaces
{
    public interface ILesson
    {
        PagedList<Lesson> GetAllLessonsByCourseId(QueryOptions options, int courseId);
        PagedList<Lesson> GetAllLessons(QueryOptions options);
        Task<IEnumerable<Lesson>> GetFiveRandomLessonsIdByCourseIdAsync(int courseId, int lessonId);
        Task<Lesson> GetLessonByIdAsync(int id);
        Task<Lesson> GetPreviousLessonByCourseAndLessonIdAsync(int courseId, int lessonId);
        Task<Lesson> GetNextLessonByCourseAndLessonIdAsync(int courseId, int lessonId);
        Task AddLessonAsync(Lesson lesson);
        Task DeleteLessonAsync(Lesson lesson);
        Task EditLessonAsync(Lesson lesson);
    }
}
