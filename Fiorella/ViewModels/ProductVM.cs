using Fiorella.Models;

namespace Fiorella.ViewModels
{
    public class ProductVM
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string CategoryName { get; set; }
        public Category Category { get; set; }
        public string IsMain { get; set; }
    }
}
