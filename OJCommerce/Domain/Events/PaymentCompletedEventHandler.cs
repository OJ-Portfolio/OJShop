using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Enums.Shipments;
using OJCommerce.Exceptions;
using OJCommerce.Models.Orders;
using OJCommerce.Models.Shipments;

namespace OJCommerce.Domain.Events
{
    public class PaymentCompletedEventHandler
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentCompletedEventHandler> _logger;

        public PaymentCompletedEventHandler(AppDbContext context, ILogger<PaymentCompletedEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Handle(PaymentCompletedEvent @event)
        {
            var payment = await _context.PaymentTransactions.Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == @event.PaymentId);
            if (payment == null)
            {
                throw new NotFoundException("payment not found");
            }

            var shipment = new Shipment
            {
                PublicShipmentId = Guid.NewGuid(),
                OrderId = payment.Order.Id,
                Status = ShipmentStatus.Pending,
                Carrier = "Internal",
                TrackingNumber = GenerateTrackingNumber(),
                RecipientName = string.Join(" ", payment.Order.User.FirstName, payment.Order.User.LastName).Trim(),
                AddressLine1 = payment.Order.ShippingAddressLine1,
                AddressLine2 = payment.Order.ShippingAddressLine2,
                PhoneNumber = payment.Order.ShippingPhoneNumber,
                City = payment.Order.ShippingCity,
                State = payment.Order.ShippingState,
                Country = payment.Order.ShippingCountry,
                CreatedAt = DateTime.UtcNow
            };

            // Idempotency: prevent duplicate shipments for same order
            var exists = await _context.Shipments
                .AnyAsync(s => s.OrderId == payment.Order.Id);

            if (exists)
            {
                _logger.LogInformation(
                    "Shipment already exists for order {OrderId}", payment.Order.Id);
                return;
            }

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Shipment created for order {OrderId} with tracking {TrackingNumber}",
                payment.Order.Id, shipment.TrackingNumber);
        }


        private static string GenerateTrackingNumber()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            var randomPart = new string(
                Enumerable.Range(0, 6)
                    .Select(_ => chars[random.Next(chars.Length)])
                    .ToArray()
            );

            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");

            return $"SHP-{datePart}-{randomPart}";
        }


    }
}
