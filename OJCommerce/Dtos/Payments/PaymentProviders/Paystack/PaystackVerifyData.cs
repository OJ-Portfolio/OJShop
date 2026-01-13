namespace OJCommerce.Dtos.Payments.PaymentProviders.Paystack
{
    public class PaystackVerifyData
    {
        public string status { get; set; }
        public string reference { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
    }
}
