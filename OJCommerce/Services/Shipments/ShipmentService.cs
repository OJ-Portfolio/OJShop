using OJCommerce.Dtos.Shipments;
using OJCommerce.Enums.Shipments;
using OJCommerce.Enums;
using OJCommerce.Exceptions;
using OJCommerce.Models.Shipments;
using OJCommerce.Services.Users;
using OJCommerce.Data;
using Microsoft.EntityFrameworkCore;

namespace OJCommerce.Services.Shipments
{
    public class ShipmentService : IShipmentService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<ShipmentService> _logger;

        public ShipmentService(
            AppDbContext context,
            IUserService userService,
            ILogger<ShipmentService> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        public async Task UpdateShipmentStatusAsync(Guid publicShipmentId, UpdateShipmentStatusDto request)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s => s.PublicShipmentId == publicShipmentId);

            if (shipment == null)
                throw new NotFoundException("Shipment not found");

            // Update status
            shipment.Status = request.Status;

            // Update timestamps based on status
            switch (request.Status)
            {
                case ShipmentStatus.PickedUp:
                    shipment.PickedUpAt = DateTime.UtcNow;
                    break;

                case ShipmentStatus.InTransit:
                    shipment.InTransitAt = DateTime.UtcNow;
                    break;

                case ShipmentStatus.OutForDelivery:
                    shipment.OutForDeliveryAt = DateTime.UtcNow;
                    break;

                case ShipmentStatus.Delivered:
                    shipment.DeliveredAt = DateTime.UtcNow;
                    shipment.DeliverySignature = request.DeliverySignature;
                    shipment.DeliveryNotes = request.DeliveryNotes;
                    shipment.Order.Status = OrderStatus.Delivered; // Update order status
                    break;

                case ShipmentStatus.Failed:
                    shipment.FailedAt = DateTime.UtcNow;
                    shipment.FailureReason = request.FailureReason;
                    shipment.Order.Status = OrderStatus.Processing; // Keep order in processing
                    break;

                case ShipmentStatus.Returned:
                    shipment.Order.Status = OrderStatus.Cancelled;
                    break;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Shipment {ShipmentId} status updated to {Status}",
                publicShipmentId, request.Status);
        }

        public async Task<ShipmentDto> GetShipmentByOrderAsync(Guid orderId)
        {
            var publicUserId = _userService.GetCurrentUser();

            var shipment = await _context.Shipments
                .Include(s => s.Order)
                .ThenInclude(o => o.User)
                .Where(s => s.Order.PublicOrderId == orderId &&
                           s.Order.User.PublicUserId == publicUserId)
                .FirstOrDefaultAsync();

            if (shipment == null)
                throw new NotFoundException("Shipment not found");

            return MapToDto(shipment);
        }

        public async Task<ShipmentDto> TrackShipmentAsync(Guid publicShipmentId)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s => s.PublicShipmentId == publicShipmentId);

            if (shipment == null)
                throw new NotFoundException("Shipment not found");

            return MapToDto(shipment);
        }

        private ShipmentDto MapToDto(Shipment shipment)
        {
            return new ShipmentDto
            {
                PublicShipmentId = shipment.PublicShipmentId,
                OrderId = shipment.Order.PublicOrderId,
                TrackingNumber = shipment.TrackingNumber,
                Carrier = shipment.Carrier,
                Status = shipment.Status.ToString(),
                RecipientName = shipment.RecipientName,
                ShippingAddress = shipment.AddressLine1,
                City = shipment.City,
                State = shipment.State,
                Country = shipment.Country,
                EstimatedDeliveryDate = shipment.EstimatedDeliveryDate,
                DeliveredAt = shipment.DeliveredAt,
                CreatedAt = shipment.CreatedAt
            };
        }
    }
}
