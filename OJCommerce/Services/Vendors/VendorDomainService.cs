using OJCommerce.Exceptions;
using OJCommerce.Repositories.Vendors;

namespace OJCommerce.Services.Vendors
{
    public class VendorDomainService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ILogger<VendorDomainService> _logger;

        public VendorDomainService(IVendorRepository vendorRepository, ILogger<VendorDomainService> logger)
        {
            _logger = logger;
            _vendorRepository = vendorRepository;
        }

        public async Task EnsureUserCanCreateVendorProfileAsync(long userId)
        {
            var exists = await _vendorRepository.ExistsForUserAsync(userId);
            if (exists)
            {
                throw new BusinessRuleViolationException("you already have a vendor profile");
            }
        }
    }
}
