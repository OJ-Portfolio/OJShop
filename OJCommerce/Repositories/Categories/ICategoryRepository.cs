using OJCommerce.Dtos.Categories;
using OJCommerce.Models.Categories;

namespace OJCommerce.Repositories.Categories
{
    public interface ICategoryRepository
    {
        Task<Category> AddAsync(Category category);
        Task<Category> GetByIdAsync(Guid id);
        Task<List<Category>> GetAllAsync();
        Task<Category> UpdateCategoryIdAsync(Guid id, Category category);
        Task<bool> DeleteByIdAsync(Guid id);
    }
}
