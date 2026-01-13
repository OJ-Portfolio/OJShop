using OJCommerce.Enums.Payments;

namespace OJCommerce.Models.Webhooks
{
    public class PaymentWebhookEvent
    {
        public long Id { get; set; }
        public PaymentProvider Provider { get; set; }

        // Provider event ID (Paystack event id, Stripe event id)
        public string EventId { get; set; }
        public PaymentStatus Status { get; set; }

        public string TransactionReference { get; set; }

        //parsed fields we need
        public string? CustomerCode { get; set; }
        public string? AuthorizationCode { get; set; }
        public string? CardLast4 { get; set; }
        public string? CardBrand { get; set; }
        public bool CardReusable { get; set; }

        //raw json for auditing
        public string Payload { get; set; }
        public bool Processed { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }
}
