using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Models.Vendors;

namespace OJCommerce.Repositories.Vendors
{
    public class VendorRepository : IVendorRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public VendorRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Vendor> AddVendor(Vendor vendor)
        {
            await _context.Vendors.AddAsync(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<bool> DeleteVendor(Guid VendorId)
        {
            var vendor = GetVendorById(VendorId);
            _context.Remove(vendor);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Vendor> GetVendorById(Guid VendorId)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.PublicVendorId == VendorId);
            if (vendor == null)
            {
                return null;
            }
            return vendor;
        }

        public async Task<List<Vendor>> GetVendors()
        {
            return await _context.Vendors.ToListAsync();
        }

        public async Task<Vendor> UpdateVendor(Guid VendorId, Vendor vendor)
        {
            var existingVendor = await GetVendorById(VendorId);
            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<bool> ExistsForUserAsync(long userId)
        {
            return await _context.Vendors.AnyAsync(v => v.UserId == userId);
        }

    }
}
