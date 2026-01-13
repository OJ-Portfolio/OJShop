namespace OJCommerce.Services.Payments
{
    public interface IPaymentProviderSelector
    {
        Task<IPaymentProvider> SelectOptimalProviderAsync(
        string currency,
        string country,
        decimal amount);
    }
}
