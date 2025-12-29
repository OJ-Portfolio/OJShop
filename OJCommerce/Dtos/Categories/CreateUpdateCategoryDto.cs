using System.ComponentModel.DataAnnotations;

namespace OJCommerce.Dtos.Categories
{
    public class CreateUpdateCategoryDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
    }
}
