using OJCommerce.Enums.Payments;

namespace OJCommerce.Services.Payments
{
    public class PaymentProviderSelector : IPaymentProviderSelector
    {
        private readonly IEnumerable<IPaymentProvider> _providers;

        public async Task<IPaymentProvider> SelectOptimalProviderAsync(
            string currency,
            string country,
            decimal amount)
        {
            // Priority logic
            var rules = new[]
            {
            // African currencies -> Paystack/Flutterwave
            (condition: currency == "NGN" || currency == "GHS" || currency == "KES",
             provider: PaymentProvider.Paystack),
            
            // USD/EUR -> Stripe preferred
            (condition: currency == "USD" || currency == "EUR",
             provider: PaymentProvider.Stripe),
            
            // Fallback to PayPal for international
            (condition: true,
             provider: PaymentProvider.PayPal)
        };

            var selectedType = rules.First(r => r.condition).provider;
            return _providers.FirstOrDefault(p => p.providerType == selectedType);
        }
    }
}
