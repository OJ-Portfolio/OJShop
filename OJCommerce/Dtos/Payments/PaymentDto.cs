using OJCommerce.Enums;
using OJCommerce.Enums.Payments;

namespace OJCommerce.Dtos.Payments
{
    public class PaymentDto
    {
        public Guid PublicPaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public PaymentProvider Provider { get; set; }      // 1=Paystack, 2=PayPal, 3=Stripe
        public PaymentMethod Method { get; set; }          // 1=Card, 2=BankTransfer, 3=PayPal
        public PaymentStatus Status { get; set; }          // 1=Pending, 2=Processing, 3=Completed, etc.
        public string TransactionReference { get; set; }
        public string AuthorizationUrl { get; set; }       // URL to redirect user for payment
        public string FailureReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
