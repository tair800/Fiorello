using System.ComponentModel.DataAnnotations;

namespace Fiorella.Models
{
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductImage> ProductImages { get; set; }

        public Product()
        {
            ProductImages = new();
        }

        public int Count { get; set; }

    }
}
