using CourseAcademyProject.Data;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace CourseAcademyProject.Repository
{
    public class CommentRepository : IComment
    {
        private readonly ApplicationContext _context;

        public CommentRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> DeleteCommentAndReturnDeletsIdAsync(Comment comment)
        {
            List<int> deletedIds = new List<int>();
            await DeleteCommentRecursiveAsync(comment, deletedIds);

            await _context.SaveChangesAsync();
            return deletedIds;
        }

        public PagedList<Comment> GetAllCommentsWithCourse(QueryOptions options)
        {
            return new PagedList<Comment>(_context.Comments.Include(e=> e.Course).OrderByDescending(e=> e.DateOfPublication), options);
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(e => e.Id == id);
        }

        private async Task DeleteCommentRecursiveAsync(Comment comment, List<int> deletedIds)
        {
            deletedIds.Add(comment.Id);
            var childComments = _context.Comments.Where(e => e.ParentCommentId == comment.Id).ToList();
            foreach (var childComment in childComments)
            {
                await DeleteCommentRecursiveAsync(childComment, deletedIds);
            }
            _context.Comments.Remove(comment);
        }
    }
}
