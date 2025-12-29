using Microsoft.EntityFrameworkCore;
using OJCommerce.Models.Products;
using System.ComponentModel.DataAnnotations.Schema;

namespace OJCommerce.Models.Categories
{
    [Index("Name", IsUnique = true)]
    public class Category
    {
        public long Id { get; set; }
        public Guid PublicCategoryId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("ParentCategoryId")]
        public long? ParentCategoryId { get; set; }
        // Navigation Properties
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
