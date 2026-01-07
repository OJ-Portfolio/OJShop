using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
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
        private readonly AppDbContext _context;

        public VendorService(IVendorRepository vendorRepository, ILogger<VendorService> logger, IMapper mapper, IUserService userService, IRoleService roleService, IUserRepository userRepository, VendorDomainService vendorDomainService, AppDbContext context)
        {
            _logger = logger;
            _vendorRepository = vendorRepository;
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _userRepository = userRepository;
            _vendorDomainService = vendorDomainService;
            _context = context;
        }

        /*public async Task<VendorInfoDto> AddVendor(CreateUpdateVendorDto vendor)
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
        */


        public async Task<bool> VendorRoleExistsAsync()
        {
            // Check if a role with the name "Vendor" exists in the Roles table
            return await _context.Roles
                .AnyAsync(r => r.Name == "Vendor");
        }

        public async Task<VendorInfoDto> AddVendor(CreateUpdateVendorDto vendorDto)
        {
            if (!vendorDto.AcceptTerms)
                throw new ArgumentException("You must accept the terms");

            // Get current user
            var currentUserId = _userService.GetCurrentUser();
            var user = await _userRepository.GetByPublicIdAsync(currentUserId);
            if (user == null)
                throw new ArgumentException("User not found");

            // Ensure user doesn't already have a vendor profile
            await _vendorDomainService.EnsureUserCanCreateVendorProfileAsync(user.Id);

            // Ensure vendor role exists before starting transaction
            var roleExists = await VendorRoleExistsAsync();
            if (!roleExists)
                throw new ArgumentException("Vendor role does not exist");

            // Use a transaction to avoid half-created vendors
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create new vendor
                var newVendor = new Vendor
                {
                    UserId = user.Id,
                    StoreName = vendorDto.StoreName,
                    CreatedAt = DateTime.UtcNow,
                    Rating = 0f
                };

                await _vendorRepository.AddVendor(newVendor);

                // Assign vendor role to user
                await _roleService.AssignVendorRoleAsync(user);

                // Commit transaction
                await transaction.CommitAsync();

                return _mapper.Map<VendorInfoDto>(newVendor);
            }
            catch
            {
                // Rollback if anything fails
                await transaction.RollbackAsync();
                throw; // rethrow exception
            }
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
