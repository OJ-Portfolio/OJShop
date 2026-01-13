using OJCommerce.Enums.Payments;

namespace OJCommerce.Dtos.Payments
{
    public class PaymentVerificationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime? PaidAt { get; set; }
        public Dictionary<string, object> ProviderData { get; set; } = new();
    }
}
