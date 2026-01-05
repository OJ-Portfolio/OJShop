namespace OJCommerce.Dtos.Checkout
{
    public class CheckoutSummaryDto
    {
        public List<CheckoutItemDto> Items { get; set; } = new();
        public decimal Total { get; set; }
        public bool CanProceed => !Errors.Any();
        public List<string> Errors { get; set; } = new();
    }
}
