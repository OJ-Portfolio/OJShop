namespace OJCommerce.Dtos.Payments.PaymentProviders.Paystack
{
    public class PaystackInitializeResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public PaystackInitializeData data { get; set; }
    }
}
