namespace OJCommerce.Dtos.Payments.PaymentProviders.Paystack
{
    public class PaystackVerifyResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public PaystackVerifyData data { get; set; }
    }
}
