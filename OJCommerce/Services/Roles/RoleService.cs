using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Dtos.Roles;
using OJCommerce.Exceptions;
using OJCommerce.Models.Roles;
using OJCommerce.Models.Users;

namespace OJCommerce.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IMapper mapper, AppDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AssignVendorRoleAsync(User user)
        {
            var vendorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == "vendor");
            if (vendorRole == null)
            {
                throw new NotFoundException("vendor role does not exist");
            }
            var exists = await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == vendorRole.Id);
            if(!exists)
            {
                _context.UserRoles.Add(new UserRole
                {
                    RoleId = vendorRole.Id,
                    UserId = user.Id,

                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignRoleToUserAsync(AssignRoleDto input)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PublicUserId == input.UserId);
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.PublicRoleId == input.RoleId);
            if (user == null) throw new NotFoundException("user not found");

            if (role == null) throw new NotFoundException("role not found");
            if(!await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id))
            {
                _context.UserRoles.Add(new UserRole
                {
                    RoleId = role.Id,
                    UserId = user.Id,
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<RoleDto> CreateRoleAsync(CreateUpdateRoleDto input)
        {
            var role = new Role
            {
                Name = input.Name,
                Description = input.Description,
            };
            await _context.AddAsync(role);
            await _context.SaveChangesAsync();
            return _mapper.Map<RoleDto>(role);
        }

        public async Task DeleteRoleAsync(Guid roleId)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.PublicRoleId == roleId);
            if (role == null)
                throw new NotFoundException("role not found");
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return _mapper.Map<List<RoleDto>>(roles);
        }

        public async Task RemoveRoleFromUserAsync(AssignRoleDto input)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PublicUserId == input.UserId);
            if (user == null) throw new NotFoundException("user not found");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.PublicRoleId == input.RoleId);
            if (role == null) throw new NotFoundException("role not found");

            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
            if(userRole != null)
            {
                _context.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<RoleDto> UpdateRoleAsync(Guid roleId, CreateUpdateRoleDto input)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.PublicRoleId == roleId);
            if (role == null)
                throw new NotFoundException("Role not found");
            role.Name = input.Name;
            role.Description = input.Description;
             _context.Update(role);
            await _context.SaveChangesAsync();
            return _mapper.Map<RoleDto>(role);

        }

       
    }
}
