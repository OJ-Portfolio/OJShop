namespace OJCommerce.Dtos.Payments.PaymentProviders.Paystack
{
    public class PaystackInitializeData
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }
}
