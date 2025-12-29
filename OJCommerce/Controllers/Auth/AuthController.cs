using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Dtos.Users;
using OJCommerce.Services.Users;

namespace OJCommerce.Controllers.Auth
{
    [ApiController]
    [Route("api[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IMapper mapper, ILogger<AuthController> logger)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUpdateUserDto input)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState
           .Where(x => x.Value.Errors.Count > 0)
           .ToDictionary(
               x => x.Key,
               x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                )
                });
            }
            var user = await _userService.Register(input);
            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid input"
                });
            }

            var result = await _userService.Login(input);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

    }
}
