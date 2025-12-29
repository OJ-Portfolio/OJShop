using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Models.Products;

namespace OJCommerce.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Product> AddAsync(Product input)
        {
            await _context.Products.AddAsync(input);
            await _context.SaveChangesAsync();
            return input;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.PublicProductId == id);
            if (product == null)
                return false;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public IQueryable<Product> GetAllAsync()
        {
            return _context.Products
                .Include(p => p.Vendor)
                .Include(p => p.Category);
        }

        public async Task<List<Product>> GetAsync()
        {
            return await _context.Products.Include(p => p.Images).Include(p => p.Category).Include(p => p.Vendor).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(x => x.PublicProductId == id);
            if (product == null)
                return null;
            return product;
        }

        public async Task<List<Product>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<Product>();
            }
            var products = await _context.Products.Where(x => x.Name.ToLower() == name.ToLower()).ToListAsync();
            return products;
        }

        public async Task<Product> UpdateAsync(Guid id, Product product)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.PublicProductId == id);
            if (existingProduct == null)
                return null;
            _mapper.Map(product, existingProduct);
            await _context.SaveChangesAsync();
            return existingProduct;
        }

    }
}
