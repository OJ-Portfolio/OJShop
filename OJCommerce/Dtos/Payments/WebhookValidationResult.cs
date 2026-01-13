using OJCommerce.Enums.Payments;

namespace OJCommerce.Dtos.Payments
{
    public class WebhookValidationResult
    {
        public string EventId { get; set; }
        public string EventType { get; set; }
        public bool IsValid { get; set; }
        public string TransactionReference { get; set; }
        public PaymentStatus Status { get; set; }
        public string Message { get; set; }
    }
}
