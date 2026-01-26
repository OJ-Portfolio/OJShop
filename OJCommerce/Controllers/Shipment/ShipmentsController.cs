using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Dtos.Shipments;
using OJCommerce.Services.Shipments;

namespace OJCommerce.Controllers.Shipment
{
    [Authorize]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentsController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        // Customer tracks their shipment by order ID
        [HttpGet("orders/{orderId}")]
        public async Task<IActionResult> GetShipmentByOrder(Guid orderId)
        {
            var shipment = await _shipmentService.GetShipmentByOrderAsync(orderId);
            return Ok(shipment);
        }

        // Public tracking link (no auth required)
        [HttpGet("{publicShipmentId}/track")]
        //[AllowAnonymous]
        public async Task<IActionResult> TrackShipment(Guid publicShipmentId)
        {
            var shipment = await _shipmentService.TrackShipmentAsync(publicShipmentId);
            return Ok(shipment);
        }

        // Admin/Carrier updates shipment status
        [HttpPut("{publicShipmentId}/status")]
        //[Authorize(Roles = "Admin")] // Only admins can update
        public async Task<IActionResult> UpdateShipmentStatus(
            Guid publicShipmentId,
            [FromBody] UpdateShipmentStatusDto request)
        {
            await _shipmentService.UpdateShipmentStatusAsync(publicShipmentId, request);
            return Ok(new { message = "Shipment status updated successfully" });
        }
    }
}
