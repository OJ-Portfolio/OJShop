using OJCommerce.Dtos.Shipments;

namespace OJCommerce.Services.Shipments
{
    public interface IShipmentService
    {
        Task<ShipmentDto> GetShipmentByOrderAsync(Guid orderId);
        Task<ShipmentDto> TrackShipmentAsync(Guid publicShipmentId);
        Task UpdateShipmentStatusAsync(Guid publicShipmentId, UpdateShipmentStatusDto request);
    }
}
