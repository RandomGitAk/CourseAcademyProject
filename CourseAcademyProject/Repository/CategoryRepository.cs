using CourseAcademyProject.Data;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace CourseAcademyProject.Repository
{
    public class CategoryRepository : ICategory
    {
        private readonly ApplicationContext _applicationContext;

        public CategoryRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task AddCategoryAsync(Category category)
        {
           await _applicationContext.Categories.AddAsync(category);
           await _applicationContext.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _applicationContext.Categories.Remove(category);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task EditCategoryAsync(Category category)
        {
            _applicationContext.Entry(category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _applicationContext.SaveChangesAsync();
        }

        public PagedList<Category> GetAllCategories(QueryOptions options)
        {
            return new PagedList<Category>(_applicationContext.Categories, options);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _applicationContext.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
           return await _applicationContext.Categories.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
