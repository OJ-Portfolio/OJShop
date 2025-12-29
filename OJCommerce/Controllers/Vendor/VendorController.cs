using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJCommerce.Data;
using OJCommerce.Dtos.Vendors;
using OJCommerce.Services.Vendors;

namespace OJCommerce.Controllers.Vendor
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger<VendorController> _logger;

        public VendorController(IVendorService vendorService, ILogger<VendorController> logger)
        {
            _vendorService = vendorService;
            _logger = logger;
        }

        [HttpPost("become-a-vendor")]
        public async Task<IActionResult> BecomeVendor(CreateUpdateVendorDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState
                   .Where(x => x.Value.Errors.Count > 0)
                   .ToDictionary(
                       x => x.Key,
                   x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                    });
            }
            var vendor = await _vendorService.AddVendor(input);
            return Ok(vendor);

        }
    }
}
