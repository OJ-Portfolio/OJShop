using OJCommerce.Enums.Payments;

namespace OJCommerce.Dtos.Payments
{
    public class PaymentOptionDto
    {
        public PaymentProvider Provider { get; set; }
        public string ProviderName { get; set; }
        public IEnumerable<PaymentMethod> AvailableMethods { get; set; }
        public bool IsRecommended { get; set; }
    }
}
