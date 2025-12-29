using AutoMapper;
using OJCommerce.Dtos.Categories;
using OJCommerce.Exceptions;
using OJCommerce.Models.Categories;
using OJCommerce.Repositories.Categories;

namespace OJCommerce.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }
        public async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
        {
            if(input.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepo.GetByIdAsync(input.ParentCategoryId.Value);
                if(parent == null)
                {
                    throw new NotFoundException("parent category does not exist");
                }
            }
            var category = _mapper.Map<Category>(input);
            await _categoryRepo.AddAsync(category);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException("category not found");
            }
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> EditCategory(Guid id, CreateUpdateCategoryDto input)
        {
            var existingCategory = await _categoryRepo.GetByIdAsync(id);
            if (existingCategory == null)
            {
                throw new NotFoundException("category not found");
            }
            _mapper.Map(input, existingCategory);
            var category = await _categoryRepo.UpdateCategoryIdAsync(id, existingCategory);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> RemoveCategory(Guid id)
        {
            return await _categoryRepo.DeleteByIdAsync(id);
        }
    }
}
