using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Dtos.Categories;
using OJCommerce.Services.Categories;

namespace OJCommerce.Controllers.Category
{
    [ApiController]
    [Route("api[controller]")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, IMapper mapper, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("add-category")]
        public async Task<CategoryDto> AddCategory(CreateUpdateCategoryDto input)
        {
            var category = await _categoryService.CreateAsync(input);
            return category;
        }

        [HttpGet("get-categories")]
        public async Task<List<CategoryDto>> GetCategories()
        {
            return await _categoryService.GetAllAsync();
        }

        [HttpGet("get-category")]
        public async Task<CategoryDto> GetCategory(Guid id)
        {
            return await _categoryService.GetByIdAsync(id);
        }

        [HttpPut("edit-category")]
        public async Task<CategoryDto> EditCategory(Guid id, CreateUpdateCategoryDto input)
        {
            return await _categoryService.EditCategory(id, input);
        }

        [HttpDelete("remove-category")]
        public async Task<bool> RemoveCategory(Guid id)
        {
            return await _categoryService.RemoveCategory(id);
        }
    }
}
