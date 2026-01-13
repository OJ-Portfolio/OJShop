using OJCommerce.Dtos.Payments;
using OJCommerce.Enums.Payments;

namespace OJCommerce.Services.Payments
{
    public interface IPaymentProvider
    {
        PaymentProvider providerType { get; }
        IEnumerable<PaymentMethod> SupportedMethods { get; }
        IEnumerable<string> SupportedCurrencies { get; }
        Task<PaymentInitializationResponse> InitializePaymentAsync(PaymentRequest request);
        Task<PaymentVerificationResponse> VerifyPaymentAsync(string transactionReference);
        Task<WebhookValidationResult> ValidateAndProcessWebhookAsync(string payload, string signature);
    }
}
