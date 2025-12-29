using OJCommerce.Models.Users;

namespace OJCommerce.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByPublicIdAsync(Guid id);
        Task<User> AddAsync(User user);
    }
}
