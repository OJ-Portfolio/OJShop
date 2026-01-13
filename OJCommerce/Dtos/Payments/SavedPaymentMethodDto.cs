using OJCommerce.Enums.Payments;

namespace OJCommerce.Dtos.Payments
{
    public class SavedPaymentMethodDto
    {
        public Guid Id { get; set; }
        public PaymentProvider Provider { get; set; }
        public string ProviderName => Provider.ToString();
        public PaymentMethod Method { get; set; }
        public string MethodName => Method.ToString();
        public string Last4Digits { get; set; }
        public string CardBrand { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
