using OJCommerce.Models.Products;

namespace OJCommerce.Repositories.Products
{
    public interface IProductRepository
    {
        public Task<Product> AddAsync(Product product);
        public Task<List<Product>> GetAsync();
        public Task<Product> GetByIdAsync(Guid id);
        public Task<List<Product>> GetByNameAsync(string name);
        public Task<Product> UpdateAsync(Guid id, Product product);
        public Task<bool> DeleteAsync(Guid id);
        public IQueryable<Product> GetAllAsync();
    }
}
