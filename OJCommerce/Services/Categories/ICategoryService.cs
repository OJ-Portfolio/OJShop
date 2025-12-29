using OJCommerce.Dtos.Categories;

namespace OJCommerce.Services.Categories
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input);
        Task<CategoryDto> GetByIdAsync(Guid id);
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> EditCategory(Guid id, CreateUpdateCategoryDto input);
        Task<bool> RemoveCategory(Guid id);
    }
}
