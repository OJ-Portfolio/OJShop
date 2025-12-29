using System.ComponentModel.DataAnnotations;

namespace OJCommerce.Dtos.Vendors
{
    public class CreateUpdateVendorDto
    {
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        [Required]
        public bool AcceptTerms { get; set; }
    }
}
