namespace OJCommerce.Domain.Events
{
    public class PaymentCompletedEvent
    {
        public long PaymentId { get; set; }
        public Guid PublicOrderId { get; set; } // or whatever type your PublicOrderId is

        public PaymentCompletedEvent() { } // parameterless constructor for JSON serialization


        public PaymentCompletedEvent(long paymentId, Guid publicOrderId)
        {
            PaymentId = paymentId;
            PublicOrderId = publicOrderId;

        }
    }
}
