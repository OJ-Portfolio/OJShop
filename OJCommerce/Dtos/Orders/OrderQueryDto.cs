using OJCommerce.Enums;

namespace OJCommerce.Dtos.Orders
{
    public class OrderQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public String? Status { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }

        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "desc";
    }
}
