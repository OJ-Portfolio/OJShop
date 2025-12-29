using OJCommerce.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace OJCommerce.Models.Tokens
{
    public class RefreshToken
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
