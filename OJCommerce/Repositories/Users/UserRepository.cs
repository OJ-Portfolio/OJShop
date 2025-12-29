using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Models.Users;

namespace OJCommerce.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;

        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User> GetByPublicIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PublicUserId == id);
        }
    }
}
