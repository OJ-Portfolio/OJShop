using OJCommerce.Enums.Payments;

namespace OJCommerce.Models.PaymentMethods
{
    public class SavedPaymentMethod
    {
        public long Id { get; set; }
        public Guid SavedPaymentMethodId { get; set; }
        public Guid UserId { get; set; }
        public PaymentProvider Provider { get; set; }
        public PaymentMethod Method { get; set; }
        public string ProviderCustomerId { get; set; }  // Stripe customer ID, etc.
        public string Last4Digits { get; set; }
        public string CardBrand { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
