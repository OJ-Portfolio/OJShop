using OJCommerce.Dtos.Checkout;

namespace OJCommerce.Services.Checkout
{
    public interface ICheckoutService
    {
        Task<CheckoutSummaryDto> ValidateAsync();
    }
}
