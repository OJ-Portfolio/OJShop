using Microsoft.AspNetCore.Mvc;
using OJCommerce.Dtos.Roles;
using OJCommerce.Services.Roles;

namespace OJCommerce.Controllers.Roles
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("get-roles")]
        public async Task<List<RoleDto>> GetAll() => await _roleService.GetAllRolesAsync();

        [HttpPost("add-role")]
        public async Task<RoleDto> Create([FromBody] CreateUpdateRoleDto input) =>
            await _roleService.CreateRoleAsync(input);

        [HttpPut("update-role")]
        public async Task<RoleDto> Update(Guid id, [FromBody] CreateUpdateRoleDto input) =>
            await _roleService.UpdateRoleAsync(id, input);

        [HttpDelete("delete-role")]
        public async Task Delete(Guid id) => await _roleService.DeleteRoleAsync(id);

        [HttpPost("assign-role")]
        public async Task Assign([FromBody] AssignRoleDto input) =>
            await _roleService.AssignRoleToUserAsync(input);

        [HttpPost("remove-role")]
        public async Task Remove([FromBody] AssignRoleDto input) =>
            await _roleService.RemoveRoleFromUserAsync(input);
    }
}
