using OJCommerce.Models.Vendors;

namespace OJCommerce.Repositories.Vendors
{
    public interface IVendorRepository
    {
        Task<Vendor> AddVendor(Vendor vendor);
        Task<Vendor> UpdateVendor(Guid VendorId, Vendor vendor);
        Task<bool> DeleteVendor(Guid VendorId);
        Task<Vendor> GetVendorById(Guid VendorId);
        Task<List<Vendor>> GetVendors();
        Task<bool> ExistsForUserAsync(long uswrId);
    }
}
