using Fiorella.Models;

namespace Fiorella.Areas.AdminArea.ViewModels.Product
{
    public class ProductUpdateVM
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int Count { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public IFormFile[] Photos { get; set; }
    }
}
