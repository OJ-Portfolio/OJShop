namespace OJCommerce.Dtos.Products
{
    public class ProductQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public Guid? CategoryId { get; set; }
        public Guid? VendorId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? Search { get; set; }

        public string? SortBy { get; set; } = "newest"; // price | newest | popularity
        public string? SortOrder { get; set; } = "desc";
    }
}
