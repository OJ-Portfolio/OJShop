namespace OJCommerce.Models.Coupons
{
    public class Coupon
    {
        public long Id { get; set; }
        public Guid PublicCouponId { get; set; } = Guid.NewGuid();
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
