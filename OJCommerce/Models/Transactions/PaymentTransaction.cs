using Microsoft.EntityFrameworkCore;
using OJCommerce.Enums;
using OJCommerce.Enums.Payments;
using OJCommerce.Models.Orders;

namespace OJCommerce.Models.Transactions
{
    [Index(nameof(Provider), nameof(ProviderTransactionReference), IsUnique = true)]
    public class PaymentTransaction
    {
        public long Id { get; set; }
        public Guid PublicPaymentId { get; set; } = Guid.NewGuid();
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentProvider Provider { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        // Provider-specific reference (e.g., Paystack reference, PayPal order ID, Stripe payment intent ID)
        public string ProviderTransactionReference { get; set; }

        // For redirect-based payments (Paystack, PayPal)
        public string AuthorizationUrl { get; set; }

        // Additional metadata
        public string Currency { get; set; }
        public string CustomerEmail { get; set; }
        public string? FailureReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        // Navigation Properties
        public virtual Order Order { get; set; }
    }
}
