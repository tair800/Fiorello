using System.ComponentModel.DataAnnotations;

namespace Fiorella.Models
{
    public class Category : BaseEntity
    {
        [Required, StringLength(25)]
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}
