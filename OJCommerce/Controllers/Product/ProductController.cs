using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Dtos.Products;
using OJCommerce.Models.Products;
using OJCommerce.Services.Products;

namespace OJCommerce.Controllers.Product
{
    [ApiController]
    [Route("api[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IMapper mapper)
        {
            _logger = logger;
            _productService = productService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost("add-product")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateUpdateProductDto createProductDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _productService.CreateAsync(createProductDto);
            return Ok(product);
        }

        [HttpGet("get-product")]
        public async Task<ProductDto> GetProduct(Guid id)
        {
            return await _productService.GetByIdAsync(id);
        }

        [HttpGet("get-products")]
        public async Task<List<ProductDto>> GetProducts()
        {
            return await _productService.GetAsync();
        }

        [HttpPut("edit-product")]
        public async Task<ProductDto> UpdateProduct(Guid id, CreateUpdateProductDto input)
        {
            var product = await _productService.UpdateAsync(id, input);
            return _mapper.Map<ProductDto>(product);          
        }

        [HttpDelete("remove-product")]
        public async Task<bool> RemoveProduct(Guid id)
        {
            await _productService.DeleteAsync(id);
            return true;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryDto query)
        {
            var products  = await _productService.GetProductsAsync(query);
            return Ok(products);
        }
    }
}
