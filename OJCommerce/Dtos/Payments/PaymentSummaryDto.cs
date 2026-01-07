using OJCommerce.Enums;

namespace OJCommerce.Dtos.Payments
{
    public class PaymentSummaryDto
    {
        public string Provider { get; set; }          // Paystack, Flutterwave, etc.
        public string Reference { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
