using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Dtos.PagedR;
using OJCommerce.Dtos.Products;
using OJCommerce.Exceptions;
using OJCommerce.Models.Categories;
using OJCommerce.Models.Products;
using OJCommerce.Repositories.Products;
using OJCommerce.Services.Users;
using System.Text.Json;

namespace OJCommerce.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _prodRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public ProductService(IProductRepository prodRepo, IMapper mapper, AppDbContext context, IUserService userService)
        {
            _mapper = mapper;
            _prodRepo = prodRepo;
            _context = context;
            _userService = userService;
        }
        public async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            if (input.CategoryId == Guid.Empty)
                throw new ArgumentException("Category id is required");

            // Get the category by public GUID
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.PublicCategoryId == input.CategoryId);
            if (category == null)
                throw new ArgumentException("Category does not exist");

            // Get current user and vendor
            var currentUser = _userService.GetCurrentUser();
            var vendor = await _context.Vendors
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.User.PublicUserId == currentUser);

            if (vendor == null)
                throw new ArgumentException("Vendor does not exist");

            // Map DTO to entity
            var newProduct = _mapper.Map<Product>(input);

            // Assign internal IDs and attributes
            newProduct.CategoryId = category.Id;  // internal DB ID
            newProduct.VendorId = vendor.Id;      // internal DB ID
            newProduct.AttributesJson = JsonSerializer.Serialize(input.Attributes);

            // Save product to DB
            await _prodRepo.AddAsync(newProduct);

            // Save product images if provided
            if (input.ImageUrls != null && input.ImageUrls.Any())
            {
                foreach (var url in input.ImageUrls)
                {
                    var img = new ProductImage
                    {
                        ProductId = newProduct.Id,
                        ImageUrl = url
                    };
                    _context.ProductImages.Add(img);
                }
                await _context.SaveChangesAsync();
            }

            // Reload product with vendor and category to return
            var savedProduct = await _context.Products
                .Include(p => p.Vendor)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == newProduct.Id);

            // Map to DTO
            return _mapper.Map<ProductDto>(savedProduct);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _prodRepo.GetByIdAsync(id);
            if(product == null)
            {
                throw new NotFoundException("product not found");
            }
            await _prodRepo.DeleteAsync(id);
            return true;           
        }

        public async Task<List<ProductDto>> GetAsync()
        {
            var products = await _prodRepo.GetAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
           var product = await _prodRepo.GetByIdAsync(id);
            if(product == null)
                throw new NotFoundException("product not found");
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<List<ProductDto>> GetByNameAsync(string name)
        {
            var product = await _prodRepo.GetByNameAsync(name);
            if(product == null)
                throw new NotFoundException("product not found");
            return _mapper.Map<List<ProductDto>>(product);
        }

        public async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
        {
            var existingProduct = await _prodRepo.GetByIdAsync(id);
            if(existingProduct == null)
            {
                throw new NotFoundException("product not found");
            }
            _mapper.Map(input, existingProduct);
            await _prodRepo.UpdateAsync(id, existingProduct);
            return _mapper.Map<ProductDto>(existingProduct);
        }

        //PAGINATED ENDPOINT
        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryDto query)
        {
            var productQuery = _prodRepo.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(query.Search))
                productQuery = productQuery.Where(p => p.Name.Contains(query.Search));

            if (query.CategoryId.HasValue)
                productQuery = productQuery.Where(p => p.Category.PublicCategoryId == query.CategoryId);

            if (query.VendorId.HasValue)
                productQuery = productQuery.Where(p => p.Vendor.PublicVendorId == query.VendorId);

            if (query.MinPrice.HasValue)
                productQuery = productQuery.Where(p => p.Price >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                productQuery = productQuery.Where(p => p.Price <= query.MaxPrice.Value);

            productQuery = query.SortBy switch
            {
                "price" when query.SortOrder == "asc" => productQuery.OrderBy(p => p.Price),
                "price" => productQuery.OrderByDescending(p => p.Price),
                "createdAt" when query.SortOrder == "asc" => productQuery.OrderBy(p => p.CreatedAt),
                _ => productQuery.OrderByDescending(p => p.CreatedAt) // default newest
            };


            var totalCount = await productQuery.CountAsync();

            var items = await productQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
                Items = _mapper.Map<List<ProductDto>>(items)
            };
        }


    }
}

