using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models;

namespace CourseAcademyProject.Interfaces
{
    public interface IBlog
    {
        PagedList<Blog> GetAllBlogs(QueryOptions options);
        Task<IEnumerable<Blog>> GetThreeLatestBlogsAsync();
        Task<Blog> GetBlogByIdAsync(int id);
        Task AddBlogAsync(Blog blog);
        Task DeleteBlogAsync(Blog blog);
        Task EditBlogAsync(Blog blog);
    }
}
