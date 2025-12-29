using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Dtos.Vendors;
using OJCommerce.Helpers;
using OJCommerce.Models.Vendors;
using OJCommerce.Repositories.Users;
using OJCommerce.Repositories.Vendors;
using OJCommerce.Services.Roles;
using OJCommerce.Services.Users;

namespace OJCommerce.Services.Vendors
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        private readonly ILogger<VendorService> _logger;
        private readonly IMapper _mapper;
        private readonly VendorDomainService _vendorDomainService;

        public VendorService(IVendorRepository vendorRepository, ILogger<VendorService> logger, IMapper mapper, IUserService userService, IRoleService roleService, IUserRepository userRepository, VendorDomainService vendorDomainService)
        {
            _logger = logger;
            _vendorRepository = vendorRepository;
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _userRepository = userRepository;
            _vendorDomainService = vendorDomainService;
        }
        public async Task<VendorInfoDto> AddVendor(CreateUpdateVendorDto vendor)
        {
            if(!vendor.AcceptTerms)
            {
                throw new ArgumentException("you must accept the terms");
            }
            var currentUser = _userService.GetCurrentUser();
            var user = await _userRepository.GetByPublicIdAsync(currentUser);
            if(user == null)
            {
                throw new ArgumentException("user not found");
            }

            await _vendorDomainService.EnsureUserCanCreateVendorProfileAsync(user.Id);

            var newVendor = new Vendor
            {
                UserId = user.Id,
                StoreName = vendor.StoreName,
                CreatedAt = DateTime.UtcNow,
                Rating = 0f
            };
            await _vendorRepository.AddVendor(newVendor);
            await _roleService.AssignVendorRoleAsync(user);
            return _mapper.Map<VendorInfoDto>(newVendor);

        }

        public async Task<bool> DeleteVendor(Guid vendorId)
        {
            var vendor = await GetVendorById(vendorId);
            return true;
        }

        public async Task<VendorInfoDto> GetVendorById(Guid vendorId)
        {
            var vendor = await _vendorRepository.GetVendorById(vendorId);
            if (vendor == null)
            {
                throw new ArgumentException("vendor not found");
            }
            return _mapper.Map<VendorInfoDto>(vendor);
        }

        public async Task<List<VendorInfoDto>> GetVendors()
        {
            var vendors = await _vendorRepository.GetVendors();
            return _mapper.Map<List<VendorInfoDto>>(vendors);
        }

        public async Task<VendorInfoDto> UpdateVendor(Guid id, CreateUpdateVendorDto vendor)
        {
            var existingVendor = await _vendorRepository.GetVendorById(id);
            if (existingVendor == null)
            {
                throw new ArgumentException("vendor not found");
            }
            _mapper.Map(vendor,existingVendor);
            await _vendorRepository.UpdateVendor(id, existingVendor);
            return _mapper.Map<VendorInfoDto>(existingVendor);
        }
    }
}
