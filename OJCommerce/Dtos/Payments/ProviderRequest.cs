using OJCommerce.Enums.Payments;

namespace OJCommerce.Dtos.Payments
{
    public class ProviderRequest
    {
        public PaymentProvider? Provider { get; set; }
        public PaymentMethod? Method { get; set; }
    }
}
