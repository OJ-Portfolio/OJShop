using OJCommerce.Models.Products;
using OJCommerce.Models.Vendors;

namespace OJCommerce.Models.Carts
{
    public class CartItem
    {
        public long Id { get; set; }
        public long CartId { get; set; }

        public long ProductId { get; set; }
        public long VendorId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
        public virtual Vendor Vendor { get; set; }
    }

}
