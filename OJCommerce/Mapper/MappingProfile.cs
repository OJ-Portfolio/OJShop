using AutoMapper;
using OJCommerce.Dtos.Carts;
using OJCommerce.Dtos.Categories;
using OJCommerce.Dtos.PagedR;
using OJCommerce.Dtos.Products;
using OJCommerce.Dtos.Roles;
using OJCommerce.Dtos.Users;
using OJCommerce.Dtos.Vendors;
using OJCommerce.Models.Carts;
using OJCommerce.Models.Categories;
using OJCommerce.Models.Products;
using OJCommerce.Models.Roles;
using OJCommerce.Models.Users;
using OJCommerce.Models.Vendors;
using System.Text.Json;

namespace OJCommerce.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //MAPPER FOR PRODUCT
            CreateMap<Product, Product>().ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CreateUpdateProductDto, Product>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.AttributesJson, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicProductId, opt => opt.Ignore());
            

            CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicProductId))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src =>
                src.Category != null ? src.Category.PublicCategoryId : Guid.Empty))
            .ForMember(dest => dest.Attributes, opt => opt.Ignore())
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.Vendor)) // map vendor
            .AfterMap((src, dest) =>
            {
                if (!string.IsNullOrEmpty(src.AttributesJson))
                {
                    dest.Attributes = JsonSerializer.Deserialize<Dictionary<string, string>>(src.AttributesJson);
                }
                else
                {
                    dest.Attributes = new Dictionary<string, string>();
                }
            });
            CreateMap<Product, ProductQueryDto>().ReverseMap();

            //MAPPER FOR CATEGORY
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateUpdateCategoryDto, Category>().ReverseMap();

            //MAPPER FOR USER
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUpdateUserDto>().ReverseMap();


            //MAPPER FOR VENDOR
            CreateMap<Vendor, VendorInfoDto>()
            .ForMember(dest => dest.PublicVendorId, opt => opt.MapFrom(src => src.PublicVendorId))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.StoreName))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));

            CreateMap<Vendor, CreateUpdateVendorDto>().ReverseMap();
            CreateMap<VendorInfoDto, Vendor>().ReverseMap();

            //ROLES MAPPING
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<Role, CreateUpdateRoleDto>().ReverseMap();

            // CART
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.Items));

            // CART ITEM (ENTITY => DTO)
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductId,
                    opt => opt.MapFrom(src => src.Product.PublicProductId))
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Vendor,
                    opt => opt.MapFrom(src => src.Vendor));
            CreateMap<CreateUpdateCartItemDto, CartItem>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.VendorId, opt => opt.Ignore());

        }
    }
}
