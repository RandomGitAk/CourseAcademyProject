using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models;

namespace CourseAcademyProject.Interfaces
{
    public interface ICategory
    {
        PagedList<Category> GetAllCategories(QueryOptions options);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
        Task EditCategoryAsync(Category category);

    }
}
