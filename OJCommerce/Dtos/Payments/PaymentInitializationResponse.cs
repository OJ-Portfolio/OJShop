using OJCommerce.Enums.Payments;

namespace OJCommerce.Dtos.Payments
{
    public class PaymentInitializationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid PaymentId { get; set; }
        public string TransactionReference { get; set; }
        public string AuthorizationUrl { get; set; }  // For redirect payments
        public PaymentStatus Status { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = new();
    }
}
