using System.ComponentModel.DataAnnotations;

namespace OJCommerce.Dtos.Products
{
    public class CreateUpdateProductDto
    {
        [Required]
        public Guid? CategoryId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock {  get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
