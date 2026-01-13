using OJCommerce.Dtos.Payments;
using OJCommerce.Enums.Payments;

namespace OJCommerce.Services.Payments
{
    public interface IPaymentService
    {
        Task<PaymentDto> InitiatePaymentAsync(InitiatePaymentDto request);
        Task<PaymentDto> VerifyPaymentAsync(string transactionReference);
        Task<PaymentDto> GetPaymentAsync(Guid paymentId);
        Task<PaymentDto> GetPaymentByOrderAsync(Guid orderId);
        Task<bool> HandleWebhookAsync(PaymentProvider provider, string payload, string signature);
        Task<List<PaymentDto>> GetUserPaymentsAsync();
        Task ProcessPendingWebhookAsync(long webhookEventId); // ← Add this line


        Task<IEnumerable<PaymentOptionDto>> GetAvailablePaymentOptionsAsync(
            string currency,
            decimal amount);

        Task<PaymentDto> PayWithSavedMethodAsync(
            Guid orderId,
            Guid savedPaymentMethodId);


        Task<PaymentDto> GetPaymentByOrderIdAsync(Guid orderId);
        Task<IEnumerable<SavedPaymentMethodDto>> GetUserSavedPaymentMethodsAsync();
        Task DeleteSavedPaymentMethodAsync(Guid methodId);
        Task SetDefaultPaymentMethodAsync(Guid methodId);
    }
}
