using Fiorella.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Controllers
{
    public class ProductController : Controller
    {
        readonly FiorelloDbContext _context;

        public ProductController(FiorelloDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //var products = _context.Products
            //    .Take(4)
            //    .Select(p=>new ProductVM
            //    {
            //        Name= p.Name,
            //        Price= p.Price,
            //        CategoryName = p.Category.Name,
            //        IsMain=p.ProductImages.FirstOrDefault(i=>i.IsMain).ImageUrl,
            //    }).ToList();


            //List<ProductVM> list = new();
            //foreach (var item in products)
            ////{
            //    ProductVM productVM = new();
            //    productVM.Name = item.Name;
            //    productVM.Price = item.Price;
            //    productVM.CategoryName = item.Category.Name;
            //    productVM.IsMain = item.ProductImages.FirstOrDefault(i => i.IsMain).ImageUrl;
            //    list.Add(productVM);

            //}

            var datas = _context.Products
                .Include(p => p.ProductImages)
                .ToList();
            ViewBag.ProductCount = datas.Count;

            return View(datas);
        }
        public IActionResult Detail(int? id)
        {
            if (id is null) return BadRequest();
            var data = _context.Products.Include(p => p.ProductImages).FirstOrDefault(n => n.Id == id);
            if (data is null) return BadRequest();
            return View(data);
        }
        public IActionResult LoadMore(int offset = 4)
        {
            var products = _context.Products.Include(m => m.ProductImages).Skip(offset).Take(4).ToList();
            return PartialView("_ProductPartialView", products);
        }
    }
}
