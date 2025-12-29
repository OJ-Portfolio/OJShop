using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Dtos.Users;
using OJCommerce.Services.Users;

namespace OJCommerce.Controllers.Auth
{
    public class RefreshTokenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<RefreshTokenController> _logger;
        private readonly IMapper _mapper;

        public RefreshTokenController(IUserService userService, ILogger<RefreshTokenController> logger, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentException("invalid request");
            }
            var newToken = await _userService.RefreshTokenAsync(request.RefreshToken);
            return Ok(newToken);
        }
    }
}
