using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;

namespace CourseAcademyProject.Interfaces
{
    public interface IComment
    {
        PagedList<Comment> GetAllCommentsWithCourse(QueryOptions options);
        Task<Comment> GetCommentByIdAsync(int id);
        Task AddCommentAsync(Comment comment);
        Task<List<int>> DeleteCommentAndReturnDeletsIdAsync(Comment comment);
    }
}
