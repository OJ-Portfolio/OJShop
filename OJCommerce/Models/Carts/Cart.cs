using OJCommerce.Models.Users;

namespace OJCommerce.Models.Carts
{
    public class Cart
    {
        public long Id { get; set; }

        public Guid UserPublicId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<CartItem> Items { get; set; }
    }

}
