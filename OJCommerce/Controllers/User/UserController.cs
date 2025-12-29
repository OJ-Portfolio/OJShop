using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Services.Users;

namespace OJCommerce.Controllers.User
{
    [ApiController]
    [Route("api/ [controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper, AppDbContext context)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("get-current-user")]
        public async Task<IActionResult> CurrentUser()
        {
            var userId = _userService.GetCurrentUser();
            var roles = await _context.UserRoles.Where(ur => ur.User.PublicUserId == userId)
                .Select(ur => ur.Role.Name).ToListAsync();
            return Ok(new
            {
                UserId = userId,
                role = roles
            });
        }
    }
}
