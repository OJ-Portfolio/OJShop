using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Dtos.Categories;
using OJCommerce.Models.Categories;

namespace OJCommerce.Repositories.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CategoryRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.PublicCategoryId == id);
            if (category == null)
            {
                return false;
            }
             _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.PublicCategoryId == id);
        }

        public async Task<Category> UpdateCategoryIdAsync(Guid id,Category category)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.PublicCategoryId == id);
            if (existingCategory == null)
                return null;
            _mapper.Map(category, existingCategory);
            await _context.SaveChangesAsync();
            return existingCategory;
        }
    }
}
