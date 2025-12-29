using OJCommerce.Enums;
using OJCommerce.Models.Orders;

namespace OJCommerce.Models.Transactions
{
    public class PaymentTransaction
    {
        public long Id { get; set; }
        public Guid PublicPaymentTransactionId { get; set; } = Guid.NewGuid();
        public long OrderId { get; set; }
        public string PaymentGateway { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Order Order { get; set; }
    }
}
