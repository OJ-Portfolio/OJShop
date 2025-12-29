using OJCommerce.Dtos.Vendors;

namespace OJCommerce.Services.Vendors
{
    public interface IVendorService
    {
        Task<List<VendorInfoDto>> GetVendors();
        Task<VendorInfoDto> GetVendorById(Guid vendorId);
        Task<bool> DeleteVendor(Guid vendorId);
        Task<VendorInfoDto> AddVendor(CreateUpdateVendorDto vendor);
        Task<VendorInfoDto> UpdateVendor(Guid id,CreateUpdateVendorDto vendor);
    }
}
