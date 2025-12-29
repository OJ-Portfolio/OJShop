using OJCommerce.Dtos.Roles;
using OJCommerce.Models.Roles;
using OJCommerce.Models.Users;

namespace OJCommerce.Services.Roles
{
    public interface IRoleService
    {
        Task<RoleDto> CreateRoleAsync(CreateUpdateRoleDto input);
        Task<RoleDto> UpdateRoleAsync(Guid roleId, CreateUpdateRoleDto input);
        Task DeleteRoleAsync(Guid roleId);
        Task<List<RoleDto>> GetAllRolesAsync();
        Task AssignRoleToUserAsync(AssignRoleDto input);
        Task RemoveRoleFromUserAsync(AssignRoleDto input);
        Task AssignVendorRoleAsync(User user);
    }
}
