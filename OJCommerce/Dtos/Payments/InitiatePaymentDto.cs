using OJCommerce.Enums.Payments;

namespace OJCommerce.Dtos.Payments
{
    // INITIATE PAYMENT FRO CLIENT
    public class InitiatePaymentDto
    {
        public Guid OrderId { get; set; }
        public PaymentProvider? Provider { get; set; }
        public PaymentMethod? Method { get; set; }
        public bool SaveForFuture { get; set; } = false;

    }
}
