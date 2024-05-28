using CourseAcademyProject.Data;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace CourseAcademyProject.Repository
{
    public class BlogRepository : IBlog
    {
        private readonly ApplicationContext _applicationContext;
        public BlogRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }
        public async Task AddBlogAsync(Blog blog)
        {
            await _applicationContext.Blogs.AddAsync(blog);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task DeleteBlogAsync(Blog blog)
        {
            _applicationContext.Blogs.Remove(blog);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task EditBlogAsync(Blog blog)
        {
            _applicationContext.Blogs.Entry(blog).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _applicationContext.Blogs.Entry(blog).Property(e => e.DateOfPublication).IsModified = false;
            await _applicationContext.SaveChangesAsync();
        }

        public PagedList<Blog> GetAllBlogs(QueryOptions options)
        {
            return new PagedList<Blog>(_applicationContext.Blogs.OrderByDescending(e=> e.DateOfPublication), options);
        }

        public async Task<Blog> GetBlogByIdAsync(int id)
        {
            return await _applicationContext.Blogs.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Blog>> GetThreeLatestBlogsAsync()
        {
            return await _applicationContext.Blogs.OrderByDescending(e => e.DateOfPublication).Take(3).ToListAsync();
        }
    }
}
