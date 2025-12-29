using OJCommerce.Dtos.Users;
using OJCommerce.Models.Users;

namespace OJCommerce.Services.Users
{
    public interface IUserService
    {
        Task<UserDto> Register(CreateUpdateUserDto input);
        Task<UserDto> GetByPublicIdAsync(Guid id);
        Task<UserDto> GetByEmailAsync(string email);
        Task<LoginResponseDto> Login(LoginRequestDto input);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken);
        Guid GetCurrentUser();
    }
}
