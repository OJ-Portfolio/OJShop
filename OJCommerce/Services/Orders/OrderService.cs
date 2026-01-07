using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Dtos.Orders;
using OJCommerce.Dtos.PagedR;
using OJCommerce.Enums;
using OJCommerce.Exceptions;
using OJCommerce.Models.Orders;
using OJCommerce.Repositories.Carts;
using OJCommerce.Services.Users;
using System.Transactions;

namespace OJCommerce.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IUserService _userService;
        private readonly ICartRepository _cartRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUserService userService, ICartRepository cartRepository, AppDbContext context, ILogger<OrderService> logger)
        {
            _cartRepository = cartRepository;
            _userService = userService;
            _context = context;
            _logger = logger;
        }
        public async Task<OrderDto> CreateFromCartAsync()
        {
            var userPublicId = _userService.GetCurrentUser();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cart = await _cartRepository.GetCartByUserAsync(userPublicId);

                    if (cart == null || !cart.Items.Any())
                        throw new BusinessRuleViolationException("Cart is empty");

                    // Load products in bulk
                    var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();

                    var products = await _context.Products.Include(p => p.Vendor)
                        .Where(p => productIds.Contains(p.Id))
                        .ToDictionaryAsync(p => p.Id);

                    decimal totalAmount = 0m;

                    var user = await _context.Users.Where(u => u.PublicUserId == userPublicId).Select(u => u.Id).FirstOrDefaultAsync();
                    if(user == 0)
                    {
                        throw new BusinessRuleViolationException("user not found");
                    }
                    var order = new Order
                    {
                        UserId = user,
                        Status = OrderStatus.Pending
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    foreach (var item in cart.Items)
                    {
                        if (!products.TryGetValue(item.ProductId, out var product))
                            throw new BusinessRuleViolationException("Product no longer exists");

                        if (product.Stock < item.Quantity)
                            throw new BusinessRuleViolationException(
                                $"Insufficient stock for {product.Name}");

                        product.Stock -= item.Quantity;

                        _context.OrderItems.Add(new OrderItem
                        {
                            OrderId = order.Id,

                            ProductId = product.Id,
                            PublicProductId = product.PublicProductId,
                            ProductName = product.Name,

                            VendorId = product.Vendor.Id,
                            PublicVendorId = product.Vendor.PublicVendorId,
                            VendorName = product.Vendor.StoreName,

                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        });


                        totalAmount += item.UnitPrice * item.Quantity;
                    }

                    order.TotalAmount = totalAmount;

                    _context.CartItems.RemoveRange(cart.Items);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new OrderDto
                    {
                        PublicOrderId = order.PublicOrderId,
                        TotalAmount = order.TotalAmount,
                        Status = order.Status,
                        CreatedAt = order.CreatedAt,
                        Items = order.Items.Select(i => new OrderItemDto
                        {
                            ProductId = i.PublicProductId,
                            ProductName = i.ProductName,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            VendorId = i?.PublicVendorId ?? Guid.Empty,
                            VendorName = i.VendorName
                        }).ToList()
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<OrderDetailsDto> GetOrderAsync(Guid publicOrderId)
        {
            var userPublicId = _userService.GetCurrentUser();

            var order = await _context.Orders
            .AsNoTracking()
            .Where(o =>
                o.PublicOrderId == publicOrderId &&
                o.User.PublicUserId == userPublicId)
            .Select(o => new OrderDetailsDto
            {
                PublicOrderId = o.PublicOrderId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.PublicProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    VendorId = i.PublicVendorId,
                    VendorName = i.VendorName
                }).ToList()
            })
            .FirstOrDefaultAsync();


            if (order == null)
                throw new NotFoundException("Order not found");

            return order;
        }

        public async Task<PagedResult<OrderSummaryDto>> GetMyOrdersAsync(OrderQueryDto query)
        {
            var userPublicId = _userService.GetCurrentUser();

            var baseQuery = _context.Orders
                .AsNoTracking()
                .Where(o => o.User.PublicUserId == userPublicId);

            OrderStatus? status = null;

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                if (!Enum.TryParse<OrderStatus>(
                    query.Status, true, out var parsed))
                {
                    throw new BusinessRuleViolationException("Invalid order status");
                }

                status = parsed;
            }


            // 🔹 Filters
            if (status.HasValue)
            {
                baseQuery = baseQuery.Where(o => o.Status == status.Value);
            }

            if (query.FromDate.HasValue)
                baseQuery = baseQuery.Where(o => o.CreatedAt >= query.FromDate);

            if (query.ToDate.HasValue)
                baseQuery = baseQuery.Where(o => o.CreatedAt <= query.ToDate);

            if (query.MinTotal.HasValue)
                baseQuery = baseQuery.Where(o => o.TotalAmount >= query.MinTotal);

            if (query.MaxTotal.HasValue)
                baseQuery = baseQuery.Where(o => o.TotalAmount <= query.MaxTotal);

            // 🔹 Sorting
            baseQuery = query.SortBy.ToLower() switch
            {
                "totalamount" => query.SortOrder == "asc"
                    ? baseQuery.OrderBy(o => o.TotalAmount)
                    : baseQuery.OrderByDescending(o => o.TotalAmount),

                _ => query.SortOrder == "asc"
                    ? baseQuery.OrderBy(o => o.CreatedAt)
                    : baseQuery.OrderByDescending(o => o.CreatedAt)
            };

            var totalCount = await baseQuery.CountAsync();

            var orders = await baseQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(o => new OrderSummaryDto
                {
                    PublicOrderId = o.PublicOrderId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<OrderSummaryDto>
            {
                Items = orders,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }
}
