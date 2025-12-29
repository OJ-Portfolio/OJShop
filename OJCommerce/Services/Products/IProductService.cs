using OJCommerce.Dtos.PagedR;
using OJCommerce.Dtos.Products;

namespace OJCommerce.Services.Products
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAsync();
        Task<ProductDto> GetByIdAsync(Guid id);
        Task<List<ProductDto>> GetByNameAsync(string name);
        Task<ProductDto> CreateAsync(CreateUpdateProductDto input);
        Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input);
        Task<bool> DeleteAsync(Guid id);
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryDto query);
    }
}
