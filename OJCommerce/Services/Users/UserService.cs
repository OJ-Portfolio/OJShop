using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OJCommerce.Data;
using OJCommerce.Dtos.Users;
using OJCommerce.Exceptions;
using OJCommerce.Models.Roles;
using OJCommerce.Models.Tokens;
using OJCommerce.Models.Users;
using OJCommerce.Repositories.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OJCommerce.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserService(AppDbContext context, IUserRepository userRepo, IMapper mapper, ILogger<UserService> logger, IConfiguration config, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
            _config = config;
            _contextAccessor = contextAccessor;
        }
        public async Task<UserDto> Register(CreateUpdateUserDto input)
        {
            var exists = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == input.Email.ToLower());
            if (exists != null)
            {
                throw new BusinessRuleViolationException("email is taken");
            }
            string hashedPassowrd = BCrypt.Net.BCrypt.HashPassword(input.Password);
            var user = new User
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                Username = input.Username,
                PasswordHash = hashedPassowrd,
                IsActive = true,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //Assign a default user role
            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == "Customer");

            if (customerRole == null) throw new NotFoundException("Default role 'Customer' does not exist");

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = customerRole.Id
            };
             _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();


            return new UserDto()
            {
                PublicUserId = user.PublicUserId,
                Email = user.Email,
                UserName = user.Username,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Phone = input.Phone,
            };
        }

        public Guid GetCurrentUser()
        {
            var user = _contextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null || !Guid.TryParse(user, out Guid publicUserId))
            {
                throw new UnauthorizedAccessException("No user logged in");
            }
            return publicUserId;
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) throw new NotFoundException("user not found. try again");

            return _mapper.Map<UserDto>(user);

        }

        public async Task<UserDto> GetByPublicIdAsync(Guid id)
        {
            var user = await _userRepo.GetByPublicIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("user not found. try again");
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto input)
        {
            if (input == null)
                throw new BusinessRuleViolationException("invalid request");

            var user = await _context.Users.FirstOrDefaultAsync(x =>
                (x.Email != null && x.Email.ToLower() == input.UserNameOrEmail.ToLower()) ||
                (x.Username != null && x.Username.ToLower() == input.UserNameOrEmail.ToLower())
            );

            if (user == null)
                throw new BusinessRuleViolationException("invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(input.Password, user.PasswordHash))
                throw new BusinessRuleViolationException("invalid credentials");

            var roles = await _context.UserRoles.Include(ur => ur.Role).Where(ur => ur.UserId == user.Id).Select(ur => ur.Role.Name).ToListAsync();

            // ---- Generate Access Token ----
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.PublicUserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes
                .Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            // Generate Refresh Token
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshTokens(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Success = true,
                Message = "login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }


        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || storedToken.IsRevoked)
                throw new Exception("Invalid refresh token");

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
                throw new Exception("Refresh token expired");

            var user = storedToken.User;


            var roles = await _context.UserRoles.Include(ur => ur.Role).Where(ur => ur.UserId == user.Id).Select(ur => ur.Role.Name).ToListAsync();


            // Generate new access token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var newAccessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            // Rotate refresh token
            var newRefreshToken = GenerateRefreshTokens();

            storedToken.IsRevoked = true; // invalidate old token

            var newTokenRecord = new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(newTokenRecord);
            await _context.SaveChangesAsync();

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }



        private string GenerateRefreshTokens()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
