namespace OJCommerce.Config
{
    public class PaymentProviderOptions
    {
        public PaystackSettings Paystack { get; set; }
        public PayPalSettings PayPal { get; set; }
        public StripeSettings Stripe { get; set; }
    }

    public class PaystackSettings
    {
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
        public string WebhookSecret { get; set; }
        public string BaseUrl { get; set; } = "https://api.paystack.co";
        public string CallbackUrl { get; set; }
    }

    public class PayPalSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string WebhookId { get; set; }
        public string BaseUrl { get; set; } = "https://api-m.paypal.com"; // Use sandbox URL for testing
        public string Mode { get; set; } = "sandbox"; // sandbox or live
        public string CallbackUrl {  set; get; }
    }

    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
        public string WebhookSecret { get; set; }
        public string BaseUrl { get; set; } = "https://api.stripe.com";
        public string CallbackUrl { get; set; }
    }
}
