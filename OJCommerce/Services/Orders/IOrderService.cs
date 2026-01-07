using OJCommerce.Dtos.Orders;
using OJCommerce.Dtos.PagedR;

namespace OJCommerce.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderDto> CreateFromCartAsync();
        Task<OrderDetailsDto> GetOrderAsync(Guid publicOrderId);
        Task<PagedResult<OrderSummaryDto>> GetMyOrdersAsync(OrderQueryDto query);
    }
}
