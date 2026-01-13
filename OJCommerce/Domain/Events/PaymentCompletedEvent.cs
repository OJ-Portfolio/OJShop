namespace OJCommerce.Domain.Events
{
    public class PaymentCompletedEvent
    {
        public long PaymentId { get; }

        public PaymentCompletedEvent(long paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
