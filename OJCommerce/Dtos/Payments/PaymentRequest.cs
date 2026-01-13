using OJCommerce.Enums.Payments;

namespace OJCommerce.Dtos.Payments
{
    public class PaymentRequest
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string CustomerEmail { get; set; }
        public PaymentProvider Provider { get; set; }
        public PaymentMethod Method { get; set; }
        public string CallbackUrl { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
