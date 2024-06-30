using Fiorella.Areas.AdminArea.ViewModels.Product;
using Fiorella.Data;
using Fiorella.Extensions;
using Fiorella.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]

    public class ProductController : Controller
    {
        private readonly FiorelloDbContext _context;

        public ProductController(FiorelloDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM productCreateVM)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            if (!ModelState.IsValid) return View();
            var files = productCreateVM.Photos;
            Product newProduct = new();
            List<ProductImage> list = new();
            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    ModelState.AddModelError("Photos", "Can't be empty!");
                    return View(productCreateVM);
                }
                if (!file.CheckContentType())
                {
                    ModelState.AddModelError("Photos", "Choose right type!");
                    return View(productCreateVM);
                }
                if (file.CheckContentSize(500))
                {
                    ModelState.AddModelError("Photos", "Choose right Size!");
                    return View(productCreateVM);
                }
                ProductImage productImage = new();
                productImage.ImageUrl = await file.SaveFile();
                productImage.ProductId = newProduct.Id;
                if (files[0] == file)
                {
                    productImage.IsMain = true;
                }
                list.Add(productImage);
            }
            newProduct.ProductImages = list;
            newProduct.Name = productCreateVM.Name;
            newProduct.CategoryId = productCreateVM.CategoryId;
            newProduct.Price = productCreateVM.Price;
            newProduct.Count = productCreateVM.Count;

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
